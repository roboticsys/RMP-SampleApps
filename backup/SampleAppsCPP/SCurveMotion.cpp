/*! 
*  @example    SCurveMotion.cpp

*  @page       scurve-motion-cs SCurveMotion.cpp

*  @brief      SCurve Motion sample application.

*  @details    This sample application moves a single axis in an SCurve profile to an absolute position.

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

*  @include ScurveMotion.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void SCurveMotionMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                    // Specify which axis/motor to control
    const int RELATIVE_POSITION = 100;            // Specify the position to travel to.
    const int USER_UNITS = 1048576;               // Specify your counts per unit / user units.           (the motor used in this sample app has 1048576 encoder pulses per revolution)
    const int VELOCITY = 10;                      // Specify your velocity.       -   units: Units/Sec    (it will do "1048576 counts / 1 motor revolution" per second)
    const int ACCELERATION = 10;                  // Specify your acceleration.   -   units: Units/Sec^2
    const int DECELERATION = 10;                  // Specify your deceleration.   -   units: Units/Sec^2
    const int JERK_PCT = 50;                      // Specify your jerk percent (0.0 to 100.0)

    // Insert the path location of the RMP.rta (usually the RapidSetup folder)  
    char rmpPath[] = "C:\\RSI\\X.X.X\\";

    // Initialize MotionController class.
    MotionController *controller = MotionController::CreateFromSoftware(/*rmpPath*/);
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);           // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                         // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                     // [Helper Function] Check that the axis has been initialize correctly.

        axis->UserUnitsSet(USER_UNITS);                                        // Specify the counts per Unit.
        axis->ErrorLimitTriggerValueSet(1);                                    // Specify the position error limit trigger. (Learn more about this on our support page)
        axis->PositionSet(0);                                                  // Make sure motor starts at position 0 everytime.

        axis->Abort();                                                         // If there is any motion happening, abort it.
        axis->ClearFaults();                                                   // Clear faults.>
        axis->AmpEnableSet(true);                                              // Enable the motor.

        printf("SCurve Motion Example\n");
        printf("SCurve Profile: In Motion...\n");

        axis->MoveSCurve(RELATIVE_POSITION, VELOCITY, ACCELERATION, DECELERATION, JERK_PCT); // Command SCurve Motion
        axis->MotionDoneWait();                                                              // Wait for motion to finish

        printf("SCurve Profile: Completed\n");

        axis->AmpEnableSet(false);                                                           // Disable the motor
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


