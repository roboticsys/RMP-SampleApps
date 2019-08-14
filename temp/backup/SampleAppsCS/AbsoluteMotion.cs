/*! 
 *  @example   AbsoluteMotion.cs
 
 *  @page       absolute-motion-cs AbsoluteMotion.cs

 *  @brief      Absolute Motion sample application.

 *  @details    This sample application moves a single axis in trapezoidal profile to an absolute distance set by RELATIVE_POSITION below.
 
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
 * @include AbsoluteMotion.cs
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using System;

namespace SampleAppsCS
{
    class AbsoluteMotion
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;            // Specify which axis/motor to control.
            const int USER_UNITS = 1048576;      // Specify your counts per unit / user units.           (the motor used in this sample app has 1048576 encoder pulses per revolution)       
            const int POSITION = 10;           // Specify the position to travel to.
            const int VELOCITY = 1;            // Specify your velocity.       -   units: Units/Sec    (it will do 1048576 counts/1 revolution every 1 second.) 
            const int ACCELERATION = 10;           // Specify your acceleration.   -   units: Units/Sec^2
            const int DECELERATION = 10;           // Specify your deceleration.   -   units: Units/Sec^2
            
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
                axis.ErrorLimitTriggerValueSet(1);                                                  // Specify the position error limit trigger. (Learn more about this on our support page)
                axis.PositionSet(0);                                                                // Make sure motor starts at position 0 everytime.
                axis.ErrorLimitTriggerValueSet(1);                                                  // Set the position error trigger value
                axis.Abort();                                                                       // If there is any motion happening, abort it.
                axis.ClearFaults();                                                                 // Clear faults.
                axis.AmpEnableSet(true);                                                            // Enable the motor.

                Console.WriteLine("Absolute Move\n\n");
                Console.WriteLine("Trapezoidal Profile: In Motion...\n");

                //axis.ErrorLimitActionSet(RSIAction.RSIActionNONE);                                // Uncomment when using Phantom Axes.

                axis.MoveTrapezoidal(POSITION, VELOCITY, ACCELERATION, DECELERATION);               // Command simple trapezoidal motion.
                axis.MotionDoneWait();                                                              // Wait for motion to be done.

                Console.WriteLine("Trapezoidal Profile: Completed\n\n");                            // If motion is completed this will be printed out.

                axis.AmpEnableSet(false);                                                           // Disable the motor.

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