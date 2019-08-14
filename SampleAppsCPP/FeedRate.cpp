/*! 
*  @example    FeedRate.cpp

*  @page       feed-rate-cs FeedRate.cpp

*  @brief      Feed Rate sample application.

*  @details
This sample application demonstrates how to use FeedRate to adjust the speed of your motion without affecting the current move/motion profile.
You can use FeedRate to reverse a move. Move back to a certain position in your motion profile, and then resume to finish the assigned motion.
FeedRate is great because you can bring an axis to complete stop without actually changing the state of the controller, unlike STOP, ABORT, ESTOP, etc.

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
* @include FeedRate.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void FeedRateMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                      // Specify the axis that will be used.
    const int USER_UNITS = 1048576;                 // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution)       


    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);                      // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                                    // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                                // [Helper Function] Check that the axis has been initialize correctly.

        // Get Axis Ready for Motion
        axis->UserUnitsSet(USER_UNITS);                                                   // Specify the counts per Unit.
        axis->ErrorLimitTriggerValueSet(1);                                               // Specify the position error limit trigger. (Learn more about this on our support page)
        axis->PositionSet(0);                                                             // Make sure motor starts at position 0 everytime.
        axis->DefaultVelocitySet(1);                                                      // Specify velocity.
        axis->DefaultAccelerationSet(10);                                                 // Specify acceleration.
        axis->DefaultDecelerationSet(10);                                                 // Specify deceleration.
        axis->FeedRateSet(1);                                                             // Make sure the FeedRate has its default value.

        axis->Abort();                                                                    // If there is any motion happening, abort it.
        axis->ClearFaults();                                                              // Clear faults.>
        axis->AmpEnableSet(true);                                                         // Enable the motor.

        // Start Motion
        printf("Motion Start\n");
        axis->MoveSCurve(15);                                                             // Call MoveScurve to move to a position.

        while (axis->ActualPositionGet() < 10) {}                                         // Wait here until we reach position "15".

        axis->Stop();                                                                     // Stop the axis/motor.
        axis->MotionDoneWait();                                                           // Wait for move to complete.
        axis->FeedRateSet(-1);                                                            // Change FeedRate to reverse motion.
        axis->Resume();                                                                   // Start Reverse Motion.

        printf("New Feed Rate Start\n");

        while (axis->ActualPositionGet() > 5) {}                                          // Wait here until we reach position "5".

        axis->Stop();                                                                     // Stop the axis/motor.
        axis->MotionDoneWait();                                                           // Wait for move to complete.
        axis->FeedRateSet(1);                                                             // Change FeedRate to default value.
        axis->Resume();                                                                   // Resume the MoveScurve Motion.

        printf("New Feed Rate Start\n");

        axis->MotionDoneWait();                                                           // Wait for move to complete.
        axis->AmpEnableSet(true);                                                         // Disable axis/motor.
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                                  // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


