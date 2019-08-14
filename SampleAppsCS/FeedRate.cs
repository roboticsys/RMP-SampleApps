/*! 
 *  @example    FeedRate.cs 
 
 *  @page       feed-rate-cs FeedRate.cs

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
 * @include FeedRate.cs
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using System;

namespace SampleAppsCS
{
    class FeedRate
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                              // Specify which axis/motor to control.
            const int USER_UNITS = 1048576;                         // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution)       

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);  // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                     // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                                 // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                              // Initialize the axis.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                           // [Helper Function] Check that the axis has been initialized correctly.

            try
            {
                // Get Axis Ready for Motion

                axis.UserUnitsSet(USER_UNITS);                                                      // Specify the counts per Unit.
                axis.ErrorLimitTriggerValueSet(1);                                                  // Specify the position error limit trigger. (Learn more about this on our support page)
                axis.PositionSet(0);                                                                // Make sure motor starts at position 0 everytime.
                axis.DefaultVelocitySet(1);                                                         // Specify velocity.
                axis.DefaultAccelerationSet(10);                                                    // Specify acceleration.
                axis.DefaultDecelerationSet(10);                                                    // Specify deceleration.
                axis.FeedRateSet(1);                                                                // Make sure the FeedRate has its default value.

                axis.Abort();                                                                       // If there is any motion happening, abort it.
                axis.ClearFaults();                                                                 // Clear faults.
                axis.AmpEnableSet(true);                                                            // Enable the motor.


                // Start Motion
                Console.WriteLine("Motion Start");
                axis.MoveSCurve(15);                                                                // Call MoveScurve to move to a position.

                while (axis.ActualPositionGet() < 10) { }                                           // Wait here until we reach position "15".

                axis.Stop();                                                                        // Stop the axis/motor.
                axis.MotionDoneWait();                                                              // Wait for move to complete.
                axis.FeedRateSet(-1);                                                               // Change FeedRate to reverse motion.
                axis.Resume();                                                                      // Start Reverse Motion.

                Console.WriteLine("New Feed Rate Start");

                while (axis.ActualPositionGet() > 5) { }                                            // Wait here until we reach position "5".

                axis.Stop();                                                                        // Stop the axis/motor.
                axis.MotionDoneWait();                                                              // Wait for move to complete.
                axis.FeedRateSet(1);                                                                // Change FeedRate to default value.
                axis.Resume();                                                                      // Resume the MoveScurve Motion.

                Console.WriteLine("New Feed Rate Start");

                axis.MotionDoneWait();                                                              // Wait for move to complete.
                axis.AmpEnableSet(true);                                                            // Disable axis/motor.


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                       // If there are any exceptions/issues this will be printed out.
            }
            Console.WriteLine("\nPress Any Key To Exit");                                         // Allow time to read Console.
            Console.ReadKey();
        }
    }
}
