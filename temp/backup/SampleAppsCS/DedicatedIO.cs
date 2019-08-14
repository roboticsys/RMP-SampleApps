/*! 
 *  @example    DedicatedIO.cs
 
 *  @page       dedicated-io-cs DedicatedIO.cs
 
 *  @brief      Dedicated IO sample application.
 
 *  @details    This application demonstrates how to access Dedicated IO..
 
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

 *  @include DedicatedIO.cs
 */


using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class DedicatedIO
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                                                  // Specify the axis that will be used.

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                       // [Helper Function] Check that the controller has been initialize correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                   // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                     // Initialize axis. (Use RapidSetup Tool to see what is your axis number)
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                             // [Helper Function] Check that the axis has been initialize correctly.

            Console.WriteLine("Axis {0}:\n", AXIS_NUMBER);
            Console.WriteLine("Dedicated Inputs:");
            
            try
            {
                // Retrieve dedicated inputs with generic and specific function.
                Console.WriteLine("RSIMotorDedicatedInLIMIT_HW_NEG: {0} and {1}",
                                    axis.DedicatedInGet(RSIMotorDedicatedIn.RSIMotorDedicatedInLIMIT_HW_NEG),
                                    axis.NegativeLimitGet());

                Console.WriteLine("RSIMotorDedicatedInLIMIT_HW_POS: {0} and {1}",
                                    axis.DedicatedInGet(RSIMotorDedicatedIn.RSIMotorDedicatedInLIMIT_HW_POS),
                                    axis.PositiveLimitGet());

                Console.WriteLine("RSIMotorDedicatedInHOME: {0} and {1}",
                                    axis.DedicatedInGet(RSIMotorDedicatedIn.RSIMotorDedicatedInHOME),
                                    axis.HomeSwitchGet());

                Console.WriteLine("RSIMotorDedicatedInAMP_FAULT: {0} and {1}",
                                    axis.DedicatedInGet(RSIMotorDedicatedIn.RSIMotorDedicatedInAMP_FAULT),
                                    axis.AmpFaultGet());

                Console.WriteLine("RSIMotorDedicatedInAMP_ACTIVE: {0} and {1}",
                                    axis.DedicatedInGet(RSIMotorDedicatedIn.RSIMotorDedicatedInAMP_ACTIVE),
                                    axis.AmpEnableGet());
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