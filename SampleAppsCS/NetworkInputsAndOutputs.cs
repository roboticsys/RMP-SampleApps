/*! 
 *  @example    NetworkInputsAndOutputs.cs
 
 *  @page       network-inputs-and-outputs-cs NetworkInputsAndOutputs.cs
 
 *  @brief      Network Inputs and Outputs sample application.
 
 *  @details    This sample apps will demonstrate how to read the different values from your system's interchangable PDOs and SDOs.
 
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
 
 *  @include NetworkInputsAndOutputs.cs
 */

using System;
using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;

namespace SampleAppsCS
{
    class NetworkInputsAndOutputs
    {
        static void Main(string[] args)
        {
            // RapidCode Objects
            MotionController controller;                                                    // Declare what controller is.

            // Initialize RapidCode Objects
            controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);             // If your project is not on the RapidSetup folder, insert the path location of the RMP.rta (usually the RapidSetup folder).
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                           // [Helper Function] Check that the controller has been initialize correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                       // [Helper Function] Initialize the network. (Function logic at the bottom of source code)


            try
            {
                // Get Input Values

                int inputCount = controller.NetworkInputCountGet();                         // Get number of Network Inputs (PDOs)

                for (int i = 0; i < inputCount; i++)
                {
                    int size = controller.NetworkInputBitSizeGet(i);                        // Read Input BitSize
                    int offset = controller.NetworkInputBitOffsetGet(i);                    // Read Input BitOffset
                    string name = controller.NetworkInputNameGet(i);                        // Read Input Name
                    UInt64 value = controller.NetworkInputValueGet(i);                      // Read Input Value
                }


                // Get Output Values

                int outputCount = controller.NetworkOutputCountGet();                       // Get number of Network Outputs (SDOs)

                for (int i = 0; i < outputCount; i++)
                {
                    int size = controller.NetworkOutputBitSizeGet(i);                       // Read Output BitSize
                    int offset = controller.NetworkOutputBitOffsetGet(i);                   // Read Output BitOffset
                    string name = controller.NetworkOutputNameGet(i);                       // Read Output Name
                    UInt64 value = controller.NetworkOutputSentValueGet(i);                 // Read Output Value
                    controller.NetworkOutputOverrideValueSet(i, value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                                       // If there are any exceptions/issues this will be printed out.
            }
        }
    }
}

