/*! 
 *  @example    RelativeMotion.cs
 
 *  @page       relative-motion-cs RelativeMotion.cs
 
 *  @brief      Relative Motion sample application.
 
 *  @details    This sample application demonstrates how to use relative motion by moving an axis by RELATIVE_POSITION1 and RELATIVE_POSITION2 units, specified below.
 
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
 
 *  @include RelativeMotion.cs
 */

using RSI.RapidCode.dotNET;                     // Import our RapidCode Library
using System;

namespace SampleAppsCS
{
    class RelativeMotion
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;            // Specify which axis/motor to control
            const int RELATIVE_POSITION1 = 25;    // Specify the first relative position to move to
            const int RELATIVE_POSITION2 = -50;   // Specify the second relative position to move to
            const int USER_UNITS = 1048576;       // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution) 
            const int VELOCITY = 10;              // Specify your velocity - units: Units/Sec     (it will do "1048576 counts / 1 motor revolution" per second)
            const int ACCELERATION = 100;         // Specify your acceleration - units: Units/Sec^2
            const int DECELERATION = 100;         // Specify your deceleration - units: Units/Sec^2
            const int JERK_PCT = 50;              // Specify your jerk percent - units: Units/Sec^2

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                                 // Initialize the axis.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialized correctly.

            try
            {
                axis.UserUnitsSet(USER_UNITS);                                                          // Specify the counts per Unit. (every 1048576 counts my motor does one full turn) (varies per motor)
                axis.ErrorLimitTriggerValueSet(1);                                                      // Specify the position error limit trigger. (Learn more about this on our support page)
                axis.PositionSet(0);                                                                    // Ensure motor starts at position 0 every time

                axis.Abort();                                                                           // If there is any motion happening, abort it
                axis.ClearFaults();                                                                     // Clear any fualts
                axis.AmpEnableSet(true);                                                                // Enable the axis

                Console.WriteLine("Relative Motion Example\n");

                axis.MoveSCurve(25, VELOCITY, ACCELERATION, DECELERATION, JERK_PCT);                    // To demonstrate relative motion, we will start the axis at position 25.
                axis.MotionDoneWait(3000);                                                              // Then, when we make a relative move to RELATIVE_POSITION1, we will end up at position 50, and
                                                                                                        // and when we move to RELATIVE_POSITION2, we will be back at position 0.
                                                                                                        // NOTE that this move is an ABSOLUTE MOVE, so it moves the axis to position 25 rather than 25
                                                                                                        // units ahead of the current position.

                // Wait for Motion to finish. (the motion should take 2.5 seconds)
                // (if the motion is not done in 3000 milliseconds, MotionDoneWait() will throw an error)

                Console.WriteLine("\nRelative move of " + RELATIVE_POSITION1 + " from " + axis.ActualPositionGet() + "...\n");

                axis.MoveRelative(RELATIVE_POSITION1, VELOCITY, ACCELERATION, DECELERATION, JERK_PCT);  // Command a relative move
                axis.MotionDoneWait(3000);                                                              // Wait for motion to finish. (the motion should take 2.5 seconds)
                                                                                                        // (if the motion is not done in 3000 milliseconds, MotionDoneWait() will throw an error)

                Console.WriteLine("\nFinal Position: " + axis.ActualPositionGet() + "\n");
                Console.WriteLine("\nRelative move of " + RELATIVE_POSITION2 + " from " + axis.ActualPositionGet() + "...\n");

                axis.MoveRelative(RELATIVE_POSITION2, VELOCITY, ACCELERATION, DECELERATION, JERK_PCT);  // Command a relative move. (the motion should take 5 seconds)
                axis.MotionDoneWait(6000);                                                              // Wait for the motion to finish. (the motion should take 5 seconds)
                                                                                                        // (if the motion is not done in 6000 milliseconds, MotionDoneWait() will throw an error)
                Console.WriteLine("\nFinal Position: " + axis.ActualPositionGet() + "\n");
                Console.WriteLine("\nDone\n");

                axis.AmpEnableSet(false);                                                               // Disable the axis
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("\nPress Any Key To Exit");                                           // Allow time to read Console.
            Console.ReadKey();
        }
    }
}