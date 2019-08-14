/*! 
*  @example    HomingWithDS402drive.cs

*  @page       homing-with-ds402-drive-cs	 HomingWithDS402drive.cs

*  @brief      Drive Based DS402 Homing sample application.

*  @details    
   Drive based homing is one of many methods that exist to home a servo motor.  RSI encourages this homing method over all others because it avoids network latencies.  Many drives follow the DS402 standard.
*  Therefore, we have created a sample app to help you home your servo motor with your DS402 drive.

*  @pre        This sample code presumes that the user has set general configuration prior to running this program so that the motor can rotate in a stable manner.

*  @warning    This is a sample program to assist in the integration of your motion controller with your application.  It may not contain all of the logic and safety features that your application requires.

*  @copyright 
   Copyright &copy; 1998-2017 by Robotic Systems Integration, Inc. All rights reserved.
   This software contains proprietary and confidential information of Robotic 
   Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
   in the license agreement under which this software is supplied, disclosure, 
   reproduction, or use with controls other than those provided by RSI or suppliers
   for RSI is strictly prohibited without the prior express written consent of 
   Robotic Systems Integration.

*  @include HomingWithDS402drive.cs
*/

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using System;
using System.Threading;

namespace SampleAppsCS
{
    class HomingWithDS402drive
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                      // Specify which axis/motor to control. 

            const int offsetIndex = 0x607C;           // Homing Offset.
            const int offsetSubindex = 0x0;              //
            const int offsetByteSize = 4;                //
            const int offsetValue = 0;                //

            const int methodIndex = 0x6098;           // Home Method
            const int methodSubindex = 0x0;              //
            const int methodByteSize = 1;                //
            const int methodValue = 24;               //

            const int targetSpeedIndex = 0x6099;       // Speed To Switch
            const int targetSpeedSubindex = 0x1;          //
            const int targetSpeedByteSize = 4;            //
            const int targetSpeedValue = 2;            //

            const int originSpeedIndex = 0x6099;       // Speed to Zero
            const int originSpeedSubindex = 0x2;          //
            const int orignSpeedByteSize = 4;            //
            const int originSpeedValue = 10;           //

            const int accelerationIndex = 0x609A;       // Homing Acceleration
            const int accelerationSubindex = 0x0;          //
            const int accelerationByteSize = 4;            //
            const int accelerationValue = 100;          //

            const int modeOfOpIndex = 0x6060;       // Mode of Operation - For Switching To/From Homing Mode.
            const int modeOfOpSubindex = 0x0;          //
            const int modeOfOpByteSize = 1;            //
            const int modeOfOpValueToHOME = 6;            //
            const int modeOfOpValueToDEFAULT = 8;           //

            const int axisControlWordIndex = 0;             // This value is just an example.  It will likely be different for your topology.

            //Desired DS402 Enabled Output.
            const int CONTROL_WORD_TO_PREP_HOMING = 15;
            const int CONTROL_WORD_TO_START_HOMING = 31;
            const int ACCEPTABLE_DELAY_IN_MS = 20;
            const int STATUS_WORD_TARGET_REACHED_BIT = 0x400;   // Status Bit 10 (0x400) indicates Target Reached (Homing is Complete).
            const int STATUS_WORD_HOMING_ATTAINED_BIT = 0x1000;   // Status Bit 12 (0x1000) indicates Homing Attained (Homing is Successful).
            const int STATUS_WORD_HOMING_ERROR_BIT = 0x2000;   // Status Bit 13 (0x2000) indicates Homing Error (Homing ran into a problem).

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialize correctly.
            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.            
            //controller.AxisCountSet(1);                                                           // Uncomment if using Phantom Axes.
            Axis axis = controller.AxisGet(AXIS_NUMBER);                                            // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialize correctly.

            try
            {
                //1. CONFIGURE (Writing to SDOs)
                axis.NetworkNode.ServiceChannelWrite(offsetIndex, offsetSubindex, offsetByteSize, offsetValue);                             // Home Offset
                axis.NetworkNode.ServiceChannelWrite(methodIndex, methodSubindex, methodByteSize, methodValue);                             // Home Method (Home type)
                axis.NetworkNode.ServiceChannelWrite(targetSpeedIndex, targetSpeedSubindex, targetSpeedByteSize, targetSpeedValue);         // Home Speed during search for switch
                axis.NetworkNode.ServiceChannelWrite(originSpeedIndex, originSpeedSubindex, orignSpeedByteSize, originSpeedValue);          // Home Speed during search for zero
                axis.NetworkNode.ServiceChannelWrite(accelerationIndex, accelerationSubindex, accelerationByteSize, accelerationValue);     // Home acceleration

                //2. READY AXIS
                axis.Abort();                                                                                                               // Disable axis.
                axis.ClearFaults();                                                                                                         // Clear any faults.
                axis.AmpEnableSet(true);                                                                                                    // Enable the axis to trigger the home start.

                axis.rsiControl.NetworkOutputOverrideValueSet(axisControlWordIndex, CONTROL_WORD_TO_PREP_HOMING);                                   // Control Word should be 15 before Switching to Homing Mode.
                axis.rsiControl.NetworkOutputOverrideSet(axisControlWordIndex, true);                                                       // Override Control Word.
                Thread.Sleep(ACCEPTABLE_DELAY_IN_MS);                                                                                       // Delay to give transitions time -or- setup something more complex to ensure drive is in the appropriate state.

                axis.NetworkNode.ServiceChannelWrite(modeOfOpIndex, modeOfOpSubindex, modeOfOpByteSize, modeOfOpValueToHOME);               // Mode of Operation (Homing Mode = 6)
                Thread.Sleep(ACCEPTABLE_DELAY_IN_MS);                                                                                       // Delay to give transitions time -or- setup something more complex to ensure drive is in the appropriate state.

                //3. HOME
                axis.rsiControl.NetworkOutputOverrideValueSet(axisControlWordIndex, CONTROL_WORD_TO_START_HOMING);                                  // Start Homing.
                Thread.Sleep(ACCEPTABLE_DELAY_IN_MS);                                                                                       // Delay to give transitions time -or- setup something more complex to ensure drive is in the appropriate state.

                UInt16 statusWordValue;                                                                                                     // Takes the axis index. This will return the status word value.
                bool cancelHome = false;

                statusWordValue = axis.NetworkNode.StatusWordGet(AXIS_NUMBER);
                while ((!cancelHome) && ((statusWordValue & STATUS_WORD_TARGET_REACHED_BIT) == 0))                                          // When Status Word Indicates Target Reached (Success or Fail) or external evaluator (cancelHome)
                {
                    statusWordValue = axis.NetworkNode.StatusWordGet(AXIS_NUMBER);                                                          // Get the status word value (index 0x6060).
                    //A timeout that sets cancelHome would be a good idea.
                }

                // 4. EVALUATE HOMING SUCCESS
                if ((statusWordValue & STATUS_WORD_HOMING_ATTAINED_BIT) == 1)
                {
                    Console.WriteLine("Axis homed.");
                }
                else if ((statusWordValue & STATUS_WORD_HOMING_ERROR_BIT) == 1)
                {
                    Console.WriteLine("Error Occured during homing.");
                }

                //5. CLEAN UP
                axis.AmpEnableSet(false);                                                                                                   // Disable the axis.
                axis.ClearFaults();
                axis.rsiControl.NetworkOutputOverrideSet(axisControlWordIndex, false);
                axis.NetworkNode.ServiceChannelWrite(modeOfOpIndex, modeOfOpSubindex, modeOfOpByteSize, modeOfOpValueToDEFAULT);            // Restore the mode of operation to the control mode you want to run in. Mode of Operation (Default Mode = 8?)
                Thread.Sleep(ACCEPTABLE_DELAY_IN_MS);                                                                                       // Delay to give transitions time -or- setup something more complex to ensure drive is in the appropriate state.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
