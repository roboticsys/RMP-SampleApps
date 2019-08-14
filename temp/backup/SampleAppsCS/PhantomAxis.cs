/*! 
 *  @example    PhantomAxis.cs  
 
 *  @page       phantom-axis-cs PhantomAxis.cs
 
 *  @brief      Phantom Axis sample application.
 
 *  @details 
    This sample application demonstrates how to set up a phantom axis.
    Phantom axes can be used to test your applications when the network is unavailable or you need more axes than are currently connected to your network.
 
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
 
 *  @include PhantomAxis.cs
 */

using RSI.RapidCode.dotNET;                     // Import our RapidCode Library
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class PhantomAxis
    {
        static void Main(string[] args)
        {
            // RapidCode objects
            Axis phantomAxis;                                   // Declare what 'axis' is

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                               // [Helper Function] Check that the controller has been initialize correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                           // [Helper Function] Initialize the network.

            try
            {
                controller.AxisCountSet(controller.AxisCountGet() + 1);                         // Configure one additional axis to be used for the phantom axis

                int axisNumber = controller.AxisCountGet() - 1;                                 // Set the axis number to the last axis on the network (subtract one because the axes are zero indexed)

                Console.WriteLine("\nPhantom Axis Example");

                Console.WriteLine("\nCreate Phantom Axis\n");

                phantomAxis = controller.AxisGet(axisNumber);                                   // Initialize Axis class
                SampleAppsCS.HelperFunctions.CheckErrors(phantomAxis);                          // [Helper Function] Check that the axis has been initialized correctly

                // These limits are not meaningful for a Phantom Axis (e.g., a phantom axis has no actual position so a position error trigger is not necessary)
                // Therefore, you must set all of their actions to "NONE".

                Console.WriteLine("\nSetting all limit actions to NONE...\n");

                phantomAxis.ErrorLimitActionSet(RSIAction.RSIActionNONE);                       // Set Error Limit Action.
                phantomAxis.HardwareNegLimitActionSet(RSIAction.RSIActionNONE);                 // Set Hardware Negative Limit Action.
                phantomAxis.HardwarePosLimitActionSet(RSIAction.RSIActionNONE);                 // Set Hardware Positive Limit Action.
                phantomAxis.HomeActionSet(RSIAction.RSIActionNONE);                             // Set Home Action.
                phantomAxis.SoftwareNegLimitActionSet(RSIAction.RSIActionNONE);                 // Set Software Negative Limit Action.
                phantomAxis.SoftwarePosLimitActionSet(RSIAction.RSIActionNONE);                 // Set Software Positive Limit Action.

                Console.WriteLine("\nComplete\n");

                Console.WriteLine("\nSetting MotorType...\n");

                phantomAxis.MotorTypeSet(RSIMotorType.RSIMotorTypePHANTOM);                     // Set the MotorType to phantom

                Console.WriteLine("\nComplete\n");

                Console.WriteLine("\nPhantom Axis created\n");
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
