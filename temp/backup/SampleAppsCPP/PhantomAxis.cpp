/*! 
*  @example    phantomAxis->cpp

*  @page       phantom-axis-cpp phantomAxis->cpp

*  @brief      Phantom Axis sample application.

*  @details
This sample application demonstrates how to set up a phantom axis->
Phantom axes can be used to test your applications when the network is unavailable or you need more axes than are currently connected to your network.

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

*  @include phantomAxis->cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

void PhantomAxisMain()
{
    using namespace RSI::RapidCode;

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        controller->AxisCountSet(controller->AxisCountGet() + 1);                         // Configure one additional axis to be used for the phantom axis

        int axisNumber = controller->AxisCountGet() - 1;                                  // Set the axis number to the last axis on the network (subtract one because the axes are zero indexed)

        printf("\nPhantom Axis Example");

        printf("\nCreate Phantom Axis\n");

        Axis *phantomAxis = controller->AxisGet(axisNumber);                              // Initialize Axis class
        SampleAppsCPP::HelperFunctions::CheckErrors(phantomAxis);                         // [Helper Function] Check that the axis has been initialized correctly

        // These limits are not meaningful for a Phantom Axis (e.g., a phantom axis has no actual position so a position error trigger is not necessary)
        // Therefore, you must set all of their actions to "NONE".

        printf("\nSetting all limit actions to NONE...\n");

        phantomAxis->ErrorLimitActionSet(RSIAction::RSIActionNONE);                       // Set Error Limit Action.
        phantomAxis->HardwareNegLimitActionSet(RSIAction::RSIActionNONE);                 // Set Hardware Negative Limit Action.
        phantomAxis->HardwarePosLimitActionSet(RSIAction::RSIActionNONE);                 // Set Hardware Positive Limit Action.
        phantomAxis->HomeActionSet(RSIAction::RSIActionNONE);                             // Set Home Action.
        phantomAxis->SoftwareNegLimitActionSet(RSIAction::RSIActionNONE);                 // Set Software Negative Limit Action.
        phantomAxis->SoftwarePosLimitActionSet(RSIAction::RSIActionNONE);                 // Set Software Positive Limit Action.

        printf("\nComplete\n");

        printf("\nSetting MotorType...\n");

        phantomAxis->MotorTypeSet(RSIMotorType::RSIMotorTypePHANTOM);                     // Set the MotorType to phantom

        printf("\nComplete\n");

        printf("\nPhantom Axis created\n");

    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


