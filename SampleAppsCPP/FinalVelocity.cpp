/*! 
*  @example    FinalVelocity.cpp

*  @page       final-velocity-cpp FinalVelocity.cpp

*  @brief      Final Velocity sample application.

*  @details    This sample application demonstrates how to make a move that does not stop a certain position/distance.
A final velocity move will get to the specified target position and once the referenced position has been achieved final velocity.

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

*  @include FinalVelocity.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void FinalVelocityMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                    // Specify which axis/motor to control.
    const int USER_UNITS = 1048576;                // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution)  
    const int RELATIVE_POSITION = 50;           // Specify which position to travel to.
    const int VELOCITY = 10;                    // Specify your velocity - units: Units/Sec     (it will do "10485760 counts/10 motor revolution per second)
    const int ACCELERATION = 100;                // Specify your acceleration - units: Units/Sec^2
    const int DECELERATION = 100;                  // Specify your deceleration - units: Units/Sec^2
    const int JERK_PCT = 50;                    // Specify your jerk percent - units: Units/Sec^3
    const int FINAL_VELOCITY = 5;                // Specify your final velocity - units: Units/Sec

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);  // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                             // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);                     // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                                   // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                               // [Helper Function] Check that the axis has been initialize correctly.

        axis->UserUnitsSet(USER_UNITS);                                                  // Specify the counts per unit. (every 1048576 counts my motor does one full turn) (varies per motor)
        axis->ErrorLimitTriggerValueSet(1);                                              // Specify the position error limit trigger value. (learn more about this on our support page)
        axis->PositionSet(0);                                                            // Ensure the motor starts at position 0 every time.

        axis->Abort();                                                                   // If there is any motion happening, abort it.
        axis->ClearFaults();                                                             // Clear any faults.
        axis->AmpEnableSet(true);                                                        // Enable the axis->

        printf("\nFinal Velocity Example");
        printf("\nSCurve Motion in progress...\n");

        axis->MoveSCurve(RELATIVE_POSITION,                                              // Command an SCurve move with a final velocity of FINAL_VELOCITY.
            VELOCITY,
            ACCELERATION,                                                                // Once the commanded position has been reached, the motor will begin
            DECELERATION,                                                                // spinning with a speed of FINAL_VELOCITY, and continue to spin at that
            JERK_PCT,                                                                    // velocity until stopped.
            FINAL_VELOCITY);

        controller->OS->Sleep(8000);                                                     // Motion should take 8 seconds to complete.


        printf("\nSCurve Motion Done\n");
        printf("\nMotor now spinning at a speed of %f revs/sec\n", axis->ActualVelocityGet());

        controller->OS->Sleep(2500);                                                     // Let the axis spin for 2.5 seconds before stopping.

        printf("\nMotor Stopped\n");
        axis->Abort();                                                                   // Stop the motion.
        axis->AmpEnableSet(false);                                                       // Disable the axis->
        axis->ClearFaults();                                                             // Clear any faults so the axis is ready to be used again.
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                                       // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


