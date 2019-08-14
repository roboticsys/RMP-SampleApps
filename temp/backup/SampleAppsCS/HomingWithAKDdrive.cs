/*! 
*  @example    HomingWithAKDdrive.cs
 
*  @page       homing-with-akd-drive-cs	 HomingWithAKDdrive.cs

*  @brief      Drive Based AKD Homing sample application.

*  @details    
   Drive based homing is one of many methods that exist to home a servo motor. RSI encourages this homing method over all others because it avoids network latencies. Since the AKD drive does not
 * follow the DS402 standard completely, we created this sample application to help you home your servo motor with an AKD drive. This sample application is meant to be used by AKD drives only. 
 
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
 
*  @include HomingWithAKDdrive.cs
*/

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class HomingWithAKDdrive
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                // Specify which axis/motor to control.

            const int modeOfOpIndex = 0x6060;         // Mode of Operation (DS402 Standard)
            const int modeOfOpSubindex = 0x0;         // Subindex
            const int modeOfOpByteSize = 1;           // INTEGER8 (1 byte)
            const int modeOfOpValueToHOME = 6;        // Hex value to determine Mode (6 = Homing mode)
            const int modeOfOpValueToDEFAULT = 8;     // Hex value to determine Mode (8 = Cyclic synchronous position mode)

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialize correctly.
            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.            
            //controller.AxisCountSet(1);                                                           // Uncomment if using Phantom Axes.
            Axis axis = controller.AxisGet(AXIS_NUMBER);                                            // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialize correctly.

            try
            {
                axis.ErrorLimitActionSet(RSIAction.RSIActionNONE);                                // Position Error must be disabled for drive based homing.

                // 1. READY DRIVE
                axis.NetworkNode.ServiceChannelWrite(modeOfOpIndex, modeOfOpSubindex, modeOfOpByteSize, modeOfOpValueToHOME);               // Mode of Operation (Homing Mode = 6)
                axis.NetworkNode.AKDASCIICommand("DRV.OPMODE 2");                                                                           // Sets the drive operation mode (0 - current | 1 = velocity | 2 = position).
                axis.NetworkNode.AKDASCIICommand("HOME.AUTOMOVE 0");                                                                        // 0 = Disabled | 1 = Homing starts when drive is enabled.

                // Make sure you know your motor's position, velocity, and acceleration units before you send any values.                

                // 2. SET YOUR LIMIT SWITCHES
                axis.NetworkNode.AKDASCIICommand("DIN5.MODE 18");                               // Sets the digital input modes.                        - DI5 is now our Positive Limit Switch.
                axis.NetworkNode.AKDASCIICommand("DIN5.INV 1");                                 // Sets the indicated polarity of a digital input mode. - DI5 is now active when Low.
                axis.NetworkNode.AKDASCIICommand("DIN6.MODE 19");                               // Sets the digital input modes.                        - DI6 is now our Negative Limit Switch.
                axis.NetworkNode.AKDASCIICommand("DIN6.INV 1");                                 // Sets the indicated polarity of a digital input mode. - DI6 is now active when Low.

                // 3. CONFIGURE DRIVE HOMING PARAMETERS
                axis.NetworkNode.AKDASCIICommand("HOME.MODE 1");                                // Selects the homing method; active in opmode 2 (position) only. MODE1 = Find limit input
                axis.NetworkNode.AKDASCIICommand("HOME.V 20");                                  // Sets homing velocity; active in opmode 2 (position) only.
                axis.NetworkNode.AKDASCIICommand("HOME.ACC 200");                               // Sets homing acceleration; active in opmode 2 (position) only.
                axis.NetworkNode.AKDASCIICommand("HOME.DEC 200");                               // Sets homing deceleration; active in opmode 2 (position) only.
                axis.NetworkNode.AKDASCIICommand("HOME.DIR 0");                                 // Sets homing direction; active in opmode 2 (position) only. (0 = negative | 1 = positive)
                axis.NetworkNode.AKDASCIICommand("HOME.P 0");                                   // Sets home position; active in opmode 2 (position) only.
                axis.NetworkNode.AKDASCIICommand("HOME.DIST 0");                                // Sets homing distance; active in opmode 2 (position) only.
                axis.NetworkNode.AKDASCIICommand("HOME.MAXDIST 0");                             // Sets the maximum distance the motor is allowed to move during the homing routine. (Disabled when value = 0)
                axis.NetworkNode.AKDASCIICommand("HOME.IPEAK");                                 // Sets the current limit during homing procedure to a mechanical stop; active in opmode 2 (position) only.

                // 4. READY AXIS
                axis.Abort();                                                                   // Disable axis.
                axis.ClearFaults();                                                             // Clear any faults.
                axis.AmpEnableSet(true);                                                        // Enable the axis.
                System.Threading.Thread.Sleep(1000);                                            // Allow time for amp enable

                // 5. START HOMING
                axis.NetworkNode.AKDASCIICommand("HOME.MOVE");                                  // Starts a homing procedure; active in opmode 2 (position) only.
                Console.WriteLine("HOME.MOVE");

                // 6. CHECK IF HOMING IS DONE
                UInt16 statusWordValue;
                int isHomedvalue = 0;

                while (isHomedvalue != 1)                                                       // When isHomedValue = 1, homing has finished.
                {
                    statusWordValue = axis.NetworkNode.StatusWordGet(0);                        // Get the status word value (index 0x6060).
                    isHomedvalue = statusWordValue >> 12;                                       // Get the 12th bit only. This bit tells us homing is done when it goes HIGH.
                }

                Console.WriteLine("Axis homed.");

                // 7. CLEAN UP
                axis.Abort();                                                                                                               // Disable the axis.
                axis.NetworkNode.ServiceChannelWrite(modeOfOpIndex, modeOfOpSubindex, modeOfOpByteSize, modeOfOpValueToDEFAULT);            // Mode of Operation (Homing Mode = 6)

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
