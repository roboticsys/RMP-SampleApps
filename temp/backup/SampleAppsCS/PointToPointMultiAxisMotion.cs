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

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class PointToPointMultiaxisMotion
    {
        static void Main(string[] args)
        {
            // Constants
            const int X_AXIS = 0;                        // Specify which axis will be the x axis
            const int Y_AXIS = 1;                        // Specify which axis will be the y axis
            const int NUM_OF_AXES = 2;                   // Specify the number of axes (Make sure your axis count in RapidSetup is 2!)
            const int USER_UNITS = 1048576;              // Specify your counts per unit / user units.(the motor used in this sample app has 1048576 encoder pulses per revolution)      

            // Parameters
            double[] positions1 = new double[2] { 100, 200 };   // The first set of positions to be moved to
            double[] positions2 = new double[2] { 300, 300 };   // The second set of positions to be moved to
            double[] velocities1 = new double[2] { 5, 10 };     // The velocity for the two axes for the first move- Units: units/sec (driver will execute 10 rotations per second)
            double[] velocities2 = new double[2] { 10, 5 };     // The velocity for the two axes for the second move
            double[] accelerations = new double[2] { 10, 10 };  // The acceleration for the two axes
            double[] decelerations = new double[2] { 10, 10 };  // The deceleration for the two axes
            double[] jerkPercent = new double[2] { 50, 50 };    // The jerk percent for the two axes

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);  // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                     // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                                 // [Helper Function] Initialize the network.

            //controller.AxisCountSet(NUM_OF_AXES);                                                   // Uncomment when using Phantom Axes.

            Axis axis0 = controller.AxisGet(X_AXIS);                                                  // Initialize axis0
            Axis axis1 = controller.AxisGet(Y_AXIS);                                                  // Initialize axis1

            SampleAppsCS.HelperFunctions.CheckErrors(axis0);                                          // [Helper Function] Check that 'axis0' has been initialized correctly
            SampleAppsCS.HelperFunctions.CheckErrors(axis1);                                          // [Helper Function] Check that 'axis1' has been initialized correctly

            controller.MotionCountSet(3);                                                             // We will need a motion supervisor for every Axis object and MultiAxis object
                                                                                                      // In this application, we have two Axis objects and one MultiAxis object, so three motion supervisors are required
            MultiAxis multi = controller.MultiAxisGet(NUM_OF_AXES);                                   // Initialize a new MultiAxis object. MultiAxisGet takes a motion supervisor number as its argument.
                                                                                                      // This number is equal to the number of axes since motion supervisors are zero indexed (i.e., motion supervisors
                                                                                                      // 0 and 1 are used for axis0 and axis1, so motion supervisor 2 is available for our MultiAxis object).

            SampleAppsCS.HelperFunctions.CheckErrors(multi);                                          // [Helper Function] Check that 'multi' has been initialized correctly
            controller.AxisCountSet(NUM_OF_AXES);                                                           // Set the number of axis being used. A phantom axis will be created if for any axis not on the network. You may need to refresh rapid setup to see the phantom axis.

            multi.AxisRemoveAll();                                                                    // Remove all current axis if any. So we can add new ones

            multi.AxisAdd(axis0);                                                                     // Add axis0 to the MultiAxis object
            multi.AxisAdd(axis1);                                                                     // Add axis1 to the MultiAxis object

            try
            {

                axis0.UserUnitsSet(USER_UNITS);                                                       // Specify the counts per unit.
                axis0.ErrorLimitTriggerValueSet(1);                                                   // Specify the position error limit trigger. (Learn more about this on our support page)
                axis1.UserUnitsSet(USER_UNITS);
                axis1.ErrorLimitTriggerValueSet(1);

                multi.Abort();                                                                        // If there is any motion happening, abort it

                axis0.PositionSet(0);                                                                 // Zero the position (in case the program is run multiple times)
                axis1.PositionSet(0);                                                                 // This negates homing, so only do it in test/sample code

                multi.ClearFaults();                                                                  // Clear any faults

                multi.AmpEnableSet(true);                                                             // Enable the motor

                Console.WriteLine("MultiAxis Point-to-Point Motion Example\n");

                Console.WriteLine("\nBegin SCurve Motion\n");

                //axis0.ErrorLimitActionSet(RSIAction.RSIActionNONE);                                 // Uncomment when using Phantom Axes.
                axis1.ErrorLimitActionSet(RSIAction.RSIActionNONE);                                   // Uncomment when using Phantom Axes.

                multi.MoveSCurve(positions1, velocities1, accelerations, decelerations, jerkPercent); // Move to the positions specified in positions1 using a trapezoidal motion profile
                multi.MotionDoneWait();                                                               // Wait for motion to finish

                Console.WriteLine("\nSCurve Motion Complete\n");
                Console.WriteLine("\nBegin Trapezoidal Motion\n");

                multi.MoveTrapezoidal(positions2, velocities2, accelerations, decelerations);         // Move to the positions specified in positions2 using a SCurve motion profile
                multi.MotionDoneWait();                                                               // Wait for the motion to finish

                multi.AmpEnableSet(false);                                                            // Disable the axes

                Console.WriteLine("\nTrapezoidal Motion Complete\n");
                Console.WriteLine("\nTest Complete\n");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("\nPress Any Key To Exit");                                         // Allow time to read Console.
            Console.ReadKey();
        }
    }
}