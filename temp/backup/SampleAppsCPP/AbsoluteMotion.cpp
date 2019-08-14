/*! 
@example    AbsoluteMotion.cpp

*  @page       absolute-motion-cpp AbsoluteMotion.cpp

*  @brief      Absolute Motion sample application.

*  @details    This sample application moves a single axis in trapezoidal profile to an absolute distance set by RELATIVE_POSITION below.

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
* @include AbsoluteMotion.cpp
*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

void AbsoluteMotionMain()
{
    using namespace RSI::RapidCode;

    const int AXIS_NUMBER = 0;                  // Specify which axis/motor to control.
    const int USER_UNITS = 1048576;             // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution)  
    const int POSITION = 10;                    // Specify which position to travel to.
    const int VELOCITY = 5;                     // Specify your velocity - units: Units/Sec     (it will do "10485760 counts/10 motor revolution per second)
    const int ACCELERATION = 10;                // Specify your acceleration - units: Units/Sec^2
    const int DECELERATION = 10;                // Specify your deceleration - units: Units/Sec^2

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

        printf("Absolute Move\n\n");
        printf("Trapezoidal Profile: In Motion...\r");

        axis->MoveTrapezoidal(POSITION, VELOCITY, ACCELERATION, DECELERATION);  //Command simple trapezoidal motion

        axis->MotionDoneWait();                                                 // Wait for motion to be done.

        axis->AmpEnableSet(false);                                              // Disable the motor.

        printf("Trapezoidal Profile: Completed\n\n");
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                                  // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

