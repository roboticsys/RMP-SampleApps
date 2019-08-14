/*! 
 *  @example    SetUserUnits.cs
 
 *  @page       set-user-unit-cs SetUserUnits.cs
 
 *  @brief      Set User Units sample application.
 
 *  @details    This application demonstrates how to set User Units.
 
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
 
 *  @include SetUserUnits.cs
    
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using System;

namespace SampleAppsCS
{
    class SetUserUnits
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;            // Specify which axis/motor to control.
            const int USER_UNITS = 1048576;            // Specify your counts per unit/user units.             (the motor used in this sample app has 1048576 encoder pulses per revolution) ("user unit = 1" means it will do one count out of the 1048576)  

            // Other Variables
            double currentUserUnits;

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);                   // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                               // Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                           // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                             // Initialize Axis Class.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                     // [Helper Function] Check that the axis has been initialize correctly.

            try
            {
                axis.UserUnitsSet(1);                                                  // SET YOUR USER UNITS!
                axis.ErrorLimitTriggerValueSet(1);                                              // Specify the position error limit trigger. (Learn more about this on our support page)
                axis.PositionSet(0);                                                            // Make sure motor starts at position 0 everytime.

                currentUserUnits = axis.UserUnitsGet();                                         // Verify that your user units were changed!

                Console.WriteLine("The axis current user unit is: {0}", currentUserUnits);


                axis.UserUnitsSet(USER_UNITS);                                                  // SET YOUR USER UNITS!
                axis.ErrorLimitTriggerValueSet(1);                                              // Specify the position error limit trigger. (Learn more about this on our support page)
                axis.PositionSet(0);                                                            // Make sure motor starts at position 0 everytime.

                currentUserUnits = axis.UserUnitsGet();                                         // Verify that your user units were changed!

                Console.WriteLine("The axis current user unit is: {0}", currentUserUnits);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                   // If there are any exceptions/issues this will be printed out.
            }
            Console.WriteLine("\nPress Any Key To Exit");                                         // Allow time to read Console.
            Console.ReadKey();
        }
    }
}