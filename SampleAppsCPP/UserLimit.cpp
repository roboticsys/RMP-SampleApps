/*!
@example    userLimit.cpp

*  @page       user-limit-cpp userLimit.cpp

*  @brief      User Limit sample application.

*  @details
This sample code shows how to configure the XMP controller's User Limits to compare an input bit to a specific pattern.
If the pattern matches, then the specified output bit is activated and a User Event is generated to the host.

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
*  @include userLimit.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

void UserLimitMain()
{
    using namespace RSI::RapidCode;

    const int AXIS_NUMBER = 0;                    // Specify which axis/motor to control.
    const int USER_UNITS = 1048576;             // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution)  
    const int IO_NODE_NUMBER = 0;    // which IO node?
    const int USER_LIMIT = 0;    // which user limit to use?
    const int CONDITION = 0;    // which condition to use (0 or 1)

    char rmpPath[] = "C:\\RSI\\X.X.X\\";        // Insert the path location of the RMP.rta (usually the RapidSetup folder)  
    // Initialize MotionController class.
    MotionController      *controller = MotionController::CreateFromSoftware(/*rmpPath*/);    // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                                // [Helper Function] Check that the axis has been initialize correctly. 
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // initialize Axis class  
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly. 

        axis->UserUnitsSet(USER_UNITS);                                         // Specify the counts per Unit.
        axis->PositionSet(0);                                                   // Make sure motor starts at position 0 everytime.
        axis->ErrorLimitTriggerValueSet(1);                                     // Set the position error trigger value
        axis->Abort();                                                          // If there is any motion happening, abort it.
        axis->ClearFaults();                                                    // Clear faults.
        axis->AmpEnableSet(true);                                               // Enable the motor.

        IO *io = controller->IOGet(IO_NODE_NUMBER);
        SampleAppsCPP::HelperFunctions::CheckErrors(io);

        // we will use Interrupts 
        controller->InterruptEnableSet(true);

        IOPoint *inputBit = IOPoint::CreateDigitalInput(io, 3); // digital in bit 3
        IOPoint *outputBit = IOPoint::CreateDigitalOutput(io, 0); // digital out bit 0

        // configure user limit to evaluate input bit
        controller->UserLimitConditionSet(USER_LIMIT,
            CONDITION,
            RSIUserLimitLogic::RSIUserLimitLogicEQ,
            inputBit->AddressGet(),
            inputBit->MaskGet(),
            inputBit->MaskGet());

        // configure user limit to set OUTPUT_BIT_MASK high when limit is true
        controller->UserLimitOutputSet(USER_LIMIT,
            0xFFFFFFFF,
            outputBit->MaskGet(),
            outputBit->AddressGet(),
            true);

        // alternatively, if you wanted to clear the bit (turn it off), use this code instead:
        //axis->UserLimitOutputSet(USER_LIMIT, 
        //                        ~(outputBit->MaskGet()),
        //                        0,
        //                        outputBit->AddressGet(),
        //                        true); 

        // enable the user limit
        controller->UserLimitConfigSet(USER_LIMIT, RSIUserLimitTriggerType::RSIUserLimitTriggerTypeSINGLE_CONDITION, RSIAction::RSIActionNONE, 0, 0.0);

        printf("Waiting for the input bit to go high...\n");

        // wait for user limit to trigger
        while (controller->InterruptWait(RSIWaitFOREVER) != RSIEventType::RSIEventTypeUSER_LIMIT)
        {
        }

        printf("Input triggered!\n");

        // disable User Limit
        controller->UserLimitDisable(USER_LIMIT);

        // set output low
        outputBit->Set(false);

    }
    catch (RsiError const& rsiError)
    {
        printf("Text:  %s\n", rsiError.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.

    return;
}
