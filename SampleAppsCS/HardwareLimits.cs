/*! 
*  @example    HardwareLimits.cs

*  @page       hardware-limits-cs HardwareLimits.cs

*  @brief      Hardware Limits sample application.

*  @details    
   There are four configurations available for a motor's positive and negative hardware limit inputs:
       <BR><BR>1) Event Action   (action such as RSIActionE_STOP, RSIActionNONE, etc...)
       <BR>2) Event Trigger  (trigger polarity such as active HIGH and active LOW)
       <BR>3) Duration       (the limit condition will exist for a programmable number of seconds before anevent occurs)

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

*  @include HardwareLimits.cs
*/

using RSI.RapidCode.dotNET;         // Import our RapidCode library
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class HardwareLimits
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                  // Specify which axis/motor we will be controlling.
            const bool ACTIVE_HIGH = true;              // Constant for active high.
            const bool ACTIVE_LOW = false;              // Constant for active low.
            const double HW_POS_DURATION_TIME = 0.01;   // Positive limit duration. (in seconds)
            const double HW_NEG_DURATION_TIME = 0.01;   // Negative limit duration. (in seconds)

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialize correctly.
            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.            
            //controller.AxisCountSet(1);                                                           // Uncomment if using Phantom Axes.
            Axis axis = controller.AxisGet(AXIS_NUMBER);                                            // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialize correctly.

            try
            {
                Console.WriteLine("Hardware Limits Example\n");

                // Change Hardware POSITIVE (+) Limit characteristics.
                axis.HardwarePosLimitActionSet(RSIAction.RSIActionE_STOP);                              // Set the positive limit action to E_STOP.
                Console.WriteLine("\nHardware Positive Limit Action set to " + axis.HardwarePosLimitActionGet());

                axis.HardwarePosLimitTriggerStateSet(ACTIVE_HIGH);                                      // Set the positive limit trigger state to ACTIVE_HIGH.
                Console.WriteLine("\nHardware Positive Limit TriggerState set to " + axis.HardwarePosLimitTriggerStateGet());

                axis.HardwarePosLimitDurationSet(HW_POS_DURATION_TIME);                                 // Set the positive limit duration to 0.01 seconds.
                Console.WriteLine("\nHardware Positive Limit Duration set to " + axis.HardwarePosLimitDurationGet() + " seconds");

                // Change Hardware NEGATIVE (-) Limit charateristics.
                axis.HardwareNegLimitActionSet(RSIAction.RSIActionE_STOP);                              // Set the negative limit action to E_STOP.
                Console.WriteLine("\nHardware Negative Limit Action set to " + axis.HardwareNegLimitActionGet());

                axis.HardwareNegLimitTriggerStateSet(ACTIVE_LOW);                                       // Set the negative limit trigger state to ACTIVE_LOW.
                Console.WriteLine("\nHardware Negative Limit TriggerState set to " + axis.HardwareNegLimitTriggerStateGet());

                axis.HardwareNegLimitDurationSet(HW_NEG_DURATION_TIME);                                 // Set the negative limit duration to 0.01 seconds.
                Console.WriteLine("\nHardware Negative Limit Duration set to " + axis.HardwareNegLimitDurationGet() + " seconds\n");

                Console.WriteLine("\nAll Hardware Limit characteristics set\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("\nPress Any Key To Exit");                                               // Allow time to read Console.
            Console.ReadKey();
        }
    }
}
