/*! 
*  @example    PointToPointMultiaxisMotion.cs

*  @page       point-to-point-multi-axis-motion-cs PointToPointMultiaxisMotion.cs

*  @brief      Point to point multi-axis motion sample application.

*  @details
We have created several arrays below. These represent the positions and velocities for the two movements we will perform, as well as the accelerations and decelerations for these movements (these two arrays are common across the two movements). We will demonstrate both types of point to point motion: SCurve and Trapezoidal.

<BR>Note: MoveSCurve requires an additional argument, jerkPercent. This array is also created below, but is only used with the MoveSCurve command, not the MoveTrapezoidal command.

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

*  @include PointToPointMultiaxisMotion.cs
*


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void PointToPointMultiaxisMotionMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int X_AXIS = 0;                         // Specify which axis will be the x axis
    const int Y_AXIS = 1;                         // Specify which axis will be the y axis
    const int NUM_OF_AXES = 2;                    // Specify the number of axes (Make sure your axis count in RapidSetup is 2!)
    const int USER_UNITS = 1048576;               // Specify your counts per unit / user units.(the motor used in this sample app has 1048576 encoder pulses per revolution)      

    // Parameters
    double positions1[] = { 100, 200 };           // The first set of positions to be moved to
    double positions2[] = { 300, 300 };           // The second set of positions to be moved to
    double velocities1[] = { 5, 10 };             // The velocity for the two axes for the first move- Units: units/sec (driver will execute 10 rotations per second)
    double velocities2[] = { 10, 5 };             // The velocity for the two axes for the second move
    double accelerations[] = { 10, 10 };          // The acceleration for the two axes
    double decelerations[] = { 10, 10 };          // The deceleration for the two axes
    double jerkPercent[] = { 50, 50 };            // The jerk percent for the two axes

    char rmpPath[] = "C:\\RSI\\X.X.X\\";          // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);    // [Helper Function] Initialize the network.
        controller->AxisCountSet(2);                                     // Set the number of axis being used. A phantom axis will be created if for any axis not on the network. You may need to refresh rapid setup to see the phantom axis.

        // initialize Axis class  
        Axis *axis0 = controller->AxisGet(X_AXIS);
        Axis *axis1 = controller->AxisGet(Y_AXIS);
        SampleAppsCPP::HelperFunctions::CheckErrors(axis0);             // [Helper Function] Check that the axis has been initialize correctly. 
        SampleAppsCPP::HelperFunctions::CheckErrors(axis1);             // [Helper Function] Check that the axis has been initialize correctly. 

        controller->AxisCountSet(NUM_OF_AXES);                          // Uncomment when using Phantom Axes.

        axis0 = controller->AxisGet(X_AXIS);                            // Initialize axis0->
        axis1 = controller->AxisGet(Y_AXIS);                            // Initialize axis1->

        SampleAppsCPP::HelperFunctions::CheckErrors(axis0);             // [Helper Function] Check that 'axis0->' has been initialized correctly
        SampleAppsCPP::HelperFunctions::CheckErrors(axis1);             // [Helper Function] Check that 'axis1->' has been initialized correctly

        controller->MotionCountSet(3);                                  // We will need a motion supervisor for every Axis object and MultiAxis object
        // In this application, we have two Axis objects and one MultiAxis object, so three motion supervisors are required
        MultiAxis *multi = controller->MultiAxisGet(NUM_OF_AXES);                  // Initialize a new MultiAxis object. MultiAxisGet takes a motion supervisor number as its argument.
        // This number is equal to the number of axes since motion supervisors are zero indexed (i.e., motion supervisors
        // 0 and 1 are used for axis0-> and axis1->, so motion supervisor 2 is available for our MultiAxis object).

        SampleAppsCPP::HelperFunctions::CheckErrors(multi);             // [Helper Function] Check that 'multi' has been initialized correctly

        multi->AxisRemoveAll();                                         // Remove all current axis if any. So we can add new ones

        multi->AxisAdd(axis0);                                          // Add axis0-> to the MultiAxis object
        multi->AxisAdd(axis1);                                          // Add axis1-> to the MultiAxis object


        axis0->UserUnitsSet(USER_UNITS);                                // Specify the counts per unit.
        axis0->ErrorLimitTriggerValueSet(1);                            // Specify the position error limit trigger. (Learn more about this on our support page)
        axis1->UserUnitsSet(USER_UNITS);
        axis1->ErrorLimitTriggerValueSet(1);

        multi->Abort();                                                // If there is any motion happening, abort it

        axis0->PositionSet(0);                                         // Zero the position (in case the program is run multiple times)
        axis1->PositionSet(0);                                         // This negates homing, so only do it in test/sample code

        multi->ClearFaults();                                          // Clear any faults

        multi->AmpEnableSet(true);                                     // Enable the motor

        printf("MultiAxis Point-to-Point Motion Example\n");

        printf("\nBegin SCurve Motion\n");

        //axis0->ErrorLimitActionSet(RSIAction.RSIActionNONE);                                   // Uncomment when using Phantom Axes.
        axis1->ErrorLimitActionSet(RSIAction::RSIActionNONE);                                    // Uncomment when using Phantom Axes.

        multi->MoveSCurve(positions1, velocities1, accelerations, decelerations, jerkPercent);   // Move to the positions specified in positions1 using a trapezoidal motion profile
        multi->MotionDoneWait();                                                                 // Wait for motion to finish

        printf("\nSCurve Motion Complete\n");
        printf("\nBegin Trapezoidal Motion\n");

        multi->MoveTrapezoidal(positions2, velocities2, accelerations, decelerations);           // Move to the positions specified in positions2 using a SCurve motion profile
        multi->MotionDoneWait();                                                                 // Wait for the motion to finish

        multi->AmpEnableSet(false);                                                              // Disable the axes

        printf("\nTrapezoidal Motion Complete\n");
        printf("\nTest Complete\n");

    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order. 
}
