/*!
 *  @example    StopRate.cs
   
 *  @page       stop-rate-cs StopRate.cs
 
 *  @brief      Stop Rate sample application.
 
 *  @details    This sample application demonstrates how to adjust the default STOP and ESTOP times of an axis/motor.
 
 *  @pre        This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) prior to running this program so that the motor can rotate in a stable manner.
 
 *  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.
 
 *  @copyright 
	Copyright(c) 1998-2019 by Robotic Systems Integration, Inc. All rights reserved.
	This software contains proprietary and confidential information of Robotic 
	Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
	in the license agreement under which this software is supplied, disclosure, 
	reproduction, or use with controls other than those provided by RSI or suppliers
	for RSI is strictly prohibited without the prior express written consent of 
	Robotic Systems Integration.
 
 *  @include StopRate.cs
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using System;


namespace SampleAppsCS
{
    class StopRate
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                      // Specify which axis/motor to control.
            const double STOP_RATE_DEFAULT = 1.0;           // Specify the default STOP rate in seconds.
            const double ESTOP_RATE_DEFAULT = 0.05;         // Specify the default ESTOP rate in seconds.
            const double ESTOP_DECELERATION_RATE = 1000;    // Specify the default ESTOP deceleration rate in seconds.


            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Verify the controller has started correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.            //controller.AxisCountSet(1);                                                           // Uncomment if using Phantom Axes.
            Axis axis = controller.AxisGet(AXIS_NUMBER);                                                 // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialize correctly.

            try
            {
                Console.WriteLine("OLD: StopRate = " + Math.Round(axis.StopTimeGet(), 4) + "\t\teStopRate = " + Math.Round(axis.EStopTimeGet(), 4) + "\n" + "\t\tDecelRate = " + Math.Round(axis.EStopDecelerationGet(), 4) + "\n");

                axis.StopTimeSet(STOP_RATE_DEFAULT);                                                // Set the default STOP time to STOP_RATE_DEFAULT secs.
                axis.EStopTimeSet(ESTOP_RATE_DEFAULT);                                              // Set the default ESTOP time to ESTOP_RATE_DEFAULT secs.
                axis.EStopDecelerationSet(ESTOP_DECELERATION_RATE);                                 // Set the default ESTOP time to ESTOP_DECELERATION_RATE secs.

                Console.WriteLine("NEW: StopRate = " + Math.Round(axis.StopTimeGet(), 4) + "\t\teStopRate = " + Math.Round(axis.EStopTimeGet(), 4) + "\n" + "\t\tDecelRate = " + Math.Round(axis.EStopDecelerationGet(), 4) + "\n");


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