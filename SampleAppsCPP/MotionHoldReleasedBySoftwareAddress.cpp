/*! 
*  @example    MotionHoldReleasedBySoftwareAddress.cpp

*  @page       motion-hold-released-by-software-address-cpp MotionHoldReleasedBySoftwareAddress.cpp

*  @brief      Motion Hold Move Released By Software Address sample application.

*  @details    This sample code is done in AKD Drive with one Actual axis-> There are a lots of available/free firmware address. Some are suggested in comment. Avaiable/free firmware addess can be found using vm3 as long as there is no label on address, it can be used.

*  @pre        This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) prior to running this program so that the motor can rotate in a stable manner.

*  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.

*  @copyright
Copyright &copy; 1998-2019 by Robotic Systems Integration, Inc. All rights reserved.
This software contains proprietary and confidential information of Robotic
Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth
in the license agreement under which this software is supplied, disclosure,
reproduction, or use with controls other than those provided by RSI or suppliers
for RSI is strictly prohibited without the prior express written consent of
Robotic Systems Integration.
*
* @include MotionHoldReleasedBySoftwareAddress.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
#include <thread>                                   // std::this_thread::sleep_for

void MotionHoldReleasedBySoftwareAddressMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int    AXIS_NUMBER = 0;                   // Specify which axis/motor to control.
    const int    USER_UNITS = 1048576;              // Specify USER UNITS
    unsigned int SOFTWARE_ADDRESS = 0x026000B4;     // Specify Avaiable firmware address (starting from 0x026000B4 to 0x027FFFFF) can be checked in VM3.In VM3, look for Address without label which are available/free.

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        // GET AXIS READY
        axis->UserUnitsSet(USER_UNITS);                                                      // Specify the counts per Unit.
        axis->PositionSet(0);                                                                // Make sure motor starts at position 0 everytime.
        axis->ErrorLimitTriggerValueSet(1);                                                  // Set the position error trigger value
        axis->Abort();                                                                       // If there is any motion happening, abort it.
        axis->ClearFaults();                                                                 // Clear faults.>
        axis->AmpEnableSet(true);                                                            // Enable the motor.

        // SET MOTION HOLD ON AVAILABLE SOFTWARE ADDRESS
        unsigned long hostAddress = controller->HostAddressGet(SOFTWARE_ADDRESS);            // Get host address from software address
        axis->MotionHoldTypeSet(RSIMotionHoldType::RSIMotionHoldTypeCUSTOM);                 // Use TypeCUSTOM to hold execution based on a particular bit turning ON or OFF.
        axis->MotionHoldUserAddressSet(hostAddress);                                         // Specify the available hostAddress . This address' value will be used to evaluate the motion hold condition.
        axis->MotionHoldUserMaskSet(0x1);                                                    // Specify the bit you want to mask/watch from the MotionHoldUserAddressSet' address value (this evaluates using a logic AND)
        axis->MotionHoldUserPatternSet(0x1);                                                 // Specify the bit value that will release the motion hold. (When this value is met, motion hold will be released)

        // Check the condition to be false at first
        if (controller->MemoryGet(hostAddress) != 0x0)                                       // Check Available host address value is mask to be false (in this case 0x0)
        {
            controller->MemorySet(hostAddress, 0x0);                                         // if not, mask it to false value/condition (in this case 0x0)
        }

        // SET MOTION HOLD
        axis->MotionAttributeMaskOnSet(RSIMotionAttrMask::RSIMotionAttrMaskHOLD);             // Set the HOLD motion attribute mask ON. (This initializes the HOLD on a particular motion)

        axis->MoveRelative(10);                                                               // Command simple relative motion. (This motion will be HOLD by the condition above)
        std::this_thread::sleep_for(std::chrono::seconds(3));                                  // Sleep for 3 second before releasing motion hold.


        // RELEASE MOTION HOLD 
        controller->MemorySet(hostAddress, 0x1);                                              // Release Motion Hold by specifying the host address value to SET Condition (in this case 0x10000)
        axis->MotionDoneWait();                                                               // Wait for motion to be done
        controller->MemorySet(hostAddress, 0x0);                                              // Specify host address value back to false value/condition (in this case 0x0)

        // COMMAND MOTION AGAIN
        axis->MoveRelative(10);                                                               // Command simple relative motion again. (This motion will be HOLD by the condition above)
        std::this_thread::sleep_for(std::chrono::seconds(3));                                 // Sleep for 3 second before releasing motion hold.

        // RELEASE MOTION HOLD
        controller->MemorySet(hostAddress, 0x1);                                              // Release Motion Hold by specifying the host address value to SET Condition (in this case 0x1)
        axis->MotionDoneWait();                                                               // Wait for motion to be done
        controller->MemorySet(hostAddress, 0x0);                                              // Specify host address value back to false value/condition (in this case 0x0)

        // CLEAR MOTION HOLD
        axis->MotionAttributeMaskOffSet(RSIMotionAttrMask::RSIMotionAttrMaskHOLD);            // Set the HOLD motion attribute mask OFF. (This will clear any motion HOLDS that were set on this Axis)

        axis->MoveRelative(10);                                                               // This motion will have no HOLD since the previous line has set the motion attribute mask OFF.
        axis->MotionDoneWait();                                                               // Wait for Motion to be completed.

        // Abort and Clear Faults
        axis->Abort();
        axis->ClearFaults();
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

