/*! 
 *  @example    HomeToNegativeLimit.cs

 *  @page       home-to-negative-limit-cs HomeToNegativeLimit.cs
 
 *  @brief      Home to a Negative Limit sample application.
 
 *  @details        
    This sample code performs a simple homing routine that triggers off an input pulse, captures the hardware position, sets the origin and then moves back to that home position.
    <BR>The home method used in this sample code (RSIHomeMethodNEGATIVE_LIMIT) is one of the35 homing routines available in our homing documenation.
 
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
 
 *  @include HomeToNegativeLimit.cs
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class HomeToNegativeLimit
    {
        static void Main(string[] args)
        {

            // constants
            const int AXIS_NUMBER = 0;          // Specify which axis/motor to control.
            const int USER_UNITS = 1048576;     // Specify your counts per unit / user units.           (the motor used in this sample app has 1048576 encoder pulses per revolution) 
            const int VELOCITY = 1;             // Specify your velocity.       -   units: Units/Sec    (it will do 1048576 counts/1 revolution every 1 second.)
            const int ACCELERATION = 10;        // Specify your acceleration.   -   units: Units/Sec^2
            const int DECELERATION = 10;        // Specify your deceleration.   -   units: Units/Sec^2

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialize correctly.
            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.            
            //controller.AxisCountSet(1);                                                           // Uncomment if using Phantom Axes.
            Axis axis = controller.AxisGet(AXIS_NUMBER);                                            // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialize correctly.

            try
            {
                axis.UserUnitsSet(USER_UNITS);                                                      // Specify the counts per Unit.

                axis.Abort();                                                                       // If there is any motion happening, abort it.
                axis.ClearFaults();                                                                 // Clear faults.
                axis.AmpEnableSet(true);                                                            // Enable the motor.
                axis.HardwareNegLimitActionSet(RSIAction.RSIActionSTOP);                            // Neg Limit action set to STOP.
                axis.HomeMethodSet(RSIHomeMethod.RSIHomeMethodNEGATIVE_LIMIT);                      // Set the method to be used for homing.                  
                axis.HomeVelocitySet(VELOCITY);                                                     // Set the home velocity.
                axis.HomeSlowVelocitySet(VELOCITY / 10);                                            // Set the slow home velocity. (used for final move, if necessary)
                axis.HomeAccelerationSet(ACCELERATION);                                             // Set the acceleration used for homing.
                axis.HomeDecelerationSet(DECELERATION);                                             // Set the deceleration used for homing.
                axis.HomeOffsetSet(0.5);                                                            // HomeOffsetSet sets the position offset from the home (zero) position.

                axis.Home();                                                                        // Execute the homing routine.

                if (axis.HomeStateGet() == true)                                                    // HomeStateGet returns true if the Axis is homed.
                {
                    Console.WriteLine("Homing successful\n");
                }

                axis.ClearFaults();                                                                 // Clear faults created by homing.
                axis.AmpEnableSet(false);                                                           // Disable the motor.
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