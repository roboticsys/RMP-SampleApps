/*! 
 *  @example    MultiAxisVelocityMotion.cs
 
 *  @page       multi-axis-velocity-motion-cs MultiAxisVelocityMotion.cs
 
 *  @brief      Multi-Axis Velocity Motion sample application.
 
 *  @details    
    This sample application updates a multi-axis velocity synchronously and on the fly. 
   
    <BR>It can update velocities at a rate of ~2ms. 

    <BR>Before runninng this sample app:
        <BR>1. Make sure to configure your Multi-Axis on RapidSetup.
        <BR>2. Make sure to adjust your Limits and Action for all axes.
 
 *  @pre        This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) prior to running this program so that the motor can rotate in a stable manner.
 
 *  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.
 
 *  @copyright 
	Copyright &copy; 1998-2017 by Robotic Systems Integration, Inc. All rights reserved.
	This software contains proprietary and confidential information of Robotic 
	Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
	in the license agreement under which this software is supplied, disclosure, 
	reproduction, or use with controls other than those provided by RSI or suppliers
	for RSI is strictly prohibited without the prior express written consent of 
	Robotic Systems Integration.
 
 *  @include MultiAxisVelocityMotion.cs
 */

using System;
using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;

namespace SampleAppsCS
{
    class MultiAxisVelocityMotion
    {
        static void Main(string[] args)
        {
            // RapidCode Objects
            MotionController controller;                // Declare what controller is.
            Axis axis0;                                 // Declare what axis1 is.
            Axis axis1;                                 // Declare what axis2 is.
            Axis axis2;                                 // Declare what axis3 is.
            Axis axis3;                                 // Declare what axis4 is.
            Axis axis4;                                 // Declare what axis5 is.
            Axis axis5;                                 // Declare what axis6 is.
            MultiAxis multi;                            // Declare what multi is.

            // Constants                                
            const int cycles        = 5;                // Specify how many times you want to update the velocity.

            int i;
            double[] accelerations  = new double[6] { 1000, 1000, 1000, 1000, 1000, 1000 };         // Specify the acceleration for all 6 axes.
            double[] velocities     = new double[6];                                                // Initialize the array that will contain the velocities of all 6 axes.
            Random rnd              = new Random();                                                 // Initialize the Random object that we will use to generate random velocities.

            // Initialize RapidCode Objects
            controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);                     // If your project is not on the RapidSetup folder, insert the path location of the RMP.rta (usually the RapidSetup folder).
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialize correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.

            axis0 = controller.AxisGet(0);                                                          // Initialize axis1. (You can use RapidSetup to see what axes you are using.)
            axis1 = controller.AxisGet(1);                                                          // Initialize axis2.
            axis2 = controller.AxisGet(2);                                                          // Initialize axis3.
            axis3 = controller.AxisGet(3);                                                          // Initialize axis4.
            axis4 = controller.AxisGet(4);                                                          // Initialize axis5.
            axis5 = controller.AxisGet(5);                                                          // Initialize axis6.

            SampleAppsCS.HelperFunctions.CheckErrors(axis0);                                        // Check that the axis1 has been initialize correctly.
            SampleAppsCS.HelperFunctions.CheckErrors(axis1);                                        // Check that the axis2 has been initialize correctly.
            SampleAppsCS.HelperFunctions.CheckErrors(axis2);                                        // Check that the axis3 has been initialize correctly.
            SampleAppsCS.HelperFunctions.CheckErrors(axis3);                                        // Check that the axis4 has been initialize correctly.
            SampleAppsCS.HelperFunctions.CheckErrors(axis4);                                        // Check that the axis5 has been initialize correctly.
            SampleAppsCS.HelperFunctions.CheckErrors(axis5);                                        // Check that the axis6 has been initialize correctly.

            multi = controller.MultiAxisGet(6);                                                     // Configure your MultiAxis on RapidSetup (Make sure MotionCount is 1 higher than AxisCount)
            SampleAppsCS.HelperFunctions.CheckErrors(multi);                                        // Check that multi has been initialized correctly.

            multi.AxisRemoveAll();                                                                  // If there are any current axes on multi, remove them.

            multi.AxisAdd(axis0);                                                                   // Add axis1 to your Multi-Axis controller.
            multi.AxisAdd(axis1);                                                                   // Add axis2 to your Multi-Axis controller.
            multi.AxisAdd(axis2);                                                                   // Add axis3 to your Multi-Axis controller.
            multi.AxisAdd(axis3);                                                                   // Add axis4 to your Multi-Axis controller.
            multi.AxisAdd(axis4);                                                                   // Add axis5 to your Multi-Axis controller.
            multi.AxisAdd(axis5);                                                                   // Add axis6 to your Multi-Axis controller.

            try
            {
                multi.Abort();                                                                      // If there is any motion happening, abort it.
                multi.ClearFaults();                                                                // Clear faults and enable all axes.
                multi.AmpEnableSet(true);                                                           // Enable the motor.

                Console.WriteLine("Start Multi-Axis Move\n");

                for (i = 0; i < cycles; i++)                                                        // This loop will iterate 5 times based on the value of "cycles"
                {
                    int random_vel1 = rnd.Next(1, 100);                                             // random_vel1 is a number [ >= 1 and < 100 ]
                    int random_vel2 = rnd.Next(1, 100);  
                    int random_vel3 = rnd.Next(1, 100);  
                    int random_vel4 = rnd.Next(1, 100); 
                    int random_vel5 = rnd.Next(1, 100); 
                    int random_vel6 = rnd.Next(1, 100);  

                    velocities = new double[6] {random_vel1,                                        // Update axis1's velocity.
                                                random_vel2,                                        // Update axis2's velocity.
                                                random_vel3,                                        // Update axis3's velocity.
                                                random_vel4,                                        // Update axis4's velocity.
                                                random_vel5,                                        // Update axis5's velocity.
                                                random_vel6 };                                      // Update axis6's velocity.

                    multi.MoveVelocity(velocities, accelerations);                                  // Move your Multi-Axis. (this will also update the move on the fly)

                    System.Threading.Thread.Sleep(100);                                             // Sleep for 100ms before iterating again.
                }

                multi.Abort();                                                                      // Stop motion on all axes.

                Console.WriteLine("\nTest Complete\n");

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
