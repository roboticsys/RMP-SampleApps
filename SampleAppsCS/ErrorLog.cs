/*! 
 *  @example    ErrorLog.cs
 
 *  @page       error-log-cs ErrorLog.cs
 
 *  @brief      Error Log sample application.

 *  @details    This sample application demonstrates how to check errors on creation of RapidCode Objects and catch/throw exceptions througout a program.
 
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
 
 *  @include ErrorLog.cs
    
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using System;

namespace SampleAppsCS
{
    class ErrorLog
    {
        static void Main(string[] args)
        {
            try
            {
                // Constants
                const int AXIS_NUMBER = 2;                  // Specify which axis/motor to control.
                
                // Initialize RapidCode Objects
                MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
                SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialize correctly.
                Axis axis = controller.AxisGet(AXIS_NUMBER);

                PrintErrors(controller);

                SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network. (Function logic at the bottom of source code)Axis axis = controller.AxisGet(AXIS_NUMBER);                                                 // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
                SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialize correctly.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                           // If there are any exceptions/issues this will be printed out.
            }
            Console.WriteLine("\n\nPress Any Key To Exit");                                         // Allow time to read Console.
            Console.ReadKey();
        }
        private static void PrintErrors(MotionController rsiClass)
        {
            RsiError err;

            while (rsiClass.ErrorLogCountGet() > 0)
            {
                err = rsiClass.ErrorLogGet();

                Console.WriteLine("%s\n", err.text);
            }
        }

    }

}