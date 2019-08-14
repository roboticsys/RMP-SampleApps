/*! 
 *  @example    AxisSettling.cs
 
 *  @page       axis-settling-cs AxisSettling.cs

 *  @brief      Axis Settling sample application.
 
 *  @details        
 *  Configure the following characteristics for axis:
        <BR>1) Fine Position Tolerance.
        <BR>2) Coarse Position Tolerance.
        <BR>3) Velocity Tolerance.
        <BR>4) Settling Time Tolerance.
 
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
 
 *  @include AxisSettling.cs
 */

using RSI.RapidCode.dotNET;                     // Import our RapidCode Library
using System;

namespace SampleAppsCS
{
    class AxisSettling
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                  // Specify which axis/motor to control.
            const int POSITION_TOLERANCE_FINE = 200;    // Specify the fine position tolerance.
            const int POSITION_TOLERANCE_COARSE = 300;  // Specify the coarse position tolerance.
            const int VELOCITY_TOLERANCE = 12000;       // Specify the velocity tolerance.
            const int SETTLING_TIME = 5;                // Specify the settling time.
            
            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                                 // Initialize the axis.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialized correctly.

            try
            {
                Console.WriteLine("Settling Criteria Example\n");
                Console.WriteLine("\nOld Criteria\n");

                PrintParameters(axis);                                                              // Print current axis settling parameters.

                axis.PositionToleranceFineSet(POSITION_TOLERANCE_FINE);                             // Set fine position tolerance.
                axis.PositionToleranceCoarseSet(POSITION_TOLERANCE_COARSE);                         // Set coarse position tolerance.
                axis.VelocityToleranceSet(VELOCITY_TOLERANCE);                                      // Set velocity tolerance.
                axis.SettlingTimeSet(SETTLING_TIME);                                                // Set settling time.

                Console.WriteLine("\nNew Criteria\n");

                PrintParameters(axis);                                                              // Print current axis settling parameters.

                Console.WriteLine("\nPress Any Key To Exit");                                       // Allow time to read Console.
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                       // If there are any exceptions/issues this will be printed out.
            }
        }

        // A method to display all of the setlling criteria
        static void PrintParameters(Axis axis)
        {
            Console.WriteLine("Fine Position Tolerance: " + axis.PositionToleranceFineGet());       // Print fine position tolerance.
            Console.WriteLine("Coarse Position Tolerance: " + axis.PositionToleranceCoarseGet());   // Print coarse position tolerance.
            Console.WriteLine("Velocity Tolerance: " + axis.VelocityToleranceGet());                // Print velocity tolerance.
            Console.WriteLine("Settling Time: " + axis.SettlingTimeGet() + "\n");                   // Print settling time.
        }
    }
}