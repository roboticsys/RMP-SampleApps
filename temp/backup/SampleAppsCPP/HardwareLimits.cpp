/*! 
@example    hardwareLimits.cpp

*  @page       hardware-limits-cpp hardwareLimits.cpp

*  @brief      Controller Interrupts sample application.

*  @details
There are four configurations available for a motor's positive and negative hardware limit inputs:
1) Event Action   (action such as RSIActionE_STOP, RSIActionNONE, etc...)
2) Event Trigger  (trigger polarity such as active HIGH and active LOW)
3) Direction Flag ("ENABLED" will cause the command direction of motion to qualify the events, "DISABLED" will ignore direction, based solely on the limit input state)
4) Duration       (the limit condition will exist for a programmable number of seconds before an event occurs)

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
*  @include hardwareLimits.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void HardwareLimitsMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                      // Specify which axis/motor we will be controlling.
    const bool ACTIVE_HIGH = true;                  // Constant for active high.
    const bool ACTIVE_LOW = false;                  // Constant for active low.
    const double HW_POS_DURATION_TIME = 0.01;       // Positive limit duration. (in seconds)
    const double HW_NEG_DURATION_TIME = 0.01;       // Negative limit duration. (in seconds)

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        printf("Hardware Limits Example\n");

        // Change Hardware POSITIVE (+) Limit characteristicpp.
        axis->HardwarePosLimitActionSet(RSIAction::RSIActionE_STOP);                              // Set the positive limit action to E_STOP.
        printf("\nHardware Positive Limit Action set to %i", axis->HardwarePosLimitActionGet());

        axis->HardwarePosLimitTriggerStateSet(ACTIVE_HIGH);                                       // Set the positive limit trigger state to ACTIVE_HIGH.
        printf("\nHardware Positive Limit TriggerState set to %i", axis->HardwarePosLimitTriggerStateGet());

        axis->HardwarePosLimitDurationSet(HW_POS_DURATION_TIME);                                  // Set the positive limit duration to 0.01 seconds.
        printf("\nHardware Positive Limit Duration set to %f seconds", axis->HardwarePosLimitDurationGet());

        // Change Hardware NEGATIVE (-) Limit charateristicpp.
        axis->HardwareNegLimitActionSet(RSIAction::RSIActionE_STOP);                              // Set the negative limit action to E_STOP.
        printf("\nHardware Negative Limit Action set to %i", axis->HardwareNegLimitActionGet());

        axis->HardwareNegLimitTriggerStateSet(ACTIVE_LOW);                                        // Set the negative limit trigger state to ACTIVE_LOW.
        printf("\nHardware Negative Limit TriggerState set to %i", axis->HardwareNegLimitTriggerStateGet());

        axis->HardwareNegLimitDurationSet(HW_NEG_DURATION_TIME);                                  // Set the negative limit duration to 0.01 seconds.
        printf("\nHardware Negative Limit Duration set to %f seconds", axis->HardwareNegLimitDurationGet());

        printf("\nAll Hardware Limit characteristicpp set\n");
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

