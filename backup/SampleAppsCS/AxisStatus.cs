/*! 
 *  @example    AxisStatus.cs
 
 *  @page       axis-status-cs AxisStatus.cs

 *  @brief      Axis Status sample application.
 
 *  @details        
 *  Learn how to use:
        <BR>1) Axis.StateGet
        <BR>2) Axis.SourceGet
        <BR>3) Axis.SourceNameGet
        <BR>4) Axis.StatusBitGet
 
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
 
 *  @include AxisStatus.cs
 */

using RSI.RapidCode.dotNET;                     // Import our RapidCode Library
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class AxisStatus
    {
        static void Main(string[] args)
        {
            // Constants
            const int AXIS_NUMBER = 0;                  // Specify which axis/motor to control.
            
            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                                 // Initialize the axis.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialized correctly.

            try
            {
                // CHECK AXIS STATE
                RSIState state = axis.StateGet();                                                   // StateGet will return RSIState enum name of the current state of the Axis or MultiAxis. (Ex: RSIStateERROR)

                RSISource source;                                                                   // Declare a RSISource variable.

                switch (state)
                {
                    case RSIState.RSIStateIDLE:
                    case RSIState.RSIStateMOVING:
                        PrintState(state);
                        break;
                    case RSIState.RSIStateERROR:
                    case RSIState.RSIStateSTOPPING_ERROR:
                    case RSIState.RSIStateSTOPPED:
                    case RSIState.RSIStateSTOPPING:
                        source = axis.SourceGet();                                                  // SourceGet will return the RSISource enum name of the first status bit that is active. (Ex: RSISourceAMP_FAULT)
                        PrintState(state);
                        PrintSource(axis, source);
                        break;
                    default:
                        Console.WriteLine("");
                        break;
                }

                // or USE STATUS BIT GET

                bool isAmpFault_Active = axis.StatusBitGet(RSIEventType.RSIEventTypeAMP_FAULT);            // StatusBitGet returns the state of a status bit, true or false.
                bool isPositionErrorLimitActive = axis.StatusBitGet(RSIEventType.RSIEventTypeLIMIT_ERROR);
                bool isHWNegativeLimitActive = axis.StatusBitGet(RSIEventType.RSIEventTypeLIMIT_HW_NEG);
                bool isHWPostiveLimitActive = axis.StatusBitGet(RSIEventType.RSIEventTypeLIMIT_HW_POS);         // This can be done for all RSIEventTypes

                Console.WriteLine("\nPress Any Key To Exit");                                         // Allow time to read Console.
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                                       // If there are any exceptions/issues this will be printed out.
            }
        }

        private static void PrintSource(Axis axis, RSISource source)
        {
            Console.WriteLine("\nThe source of the axis error is: " + axis.SourceNameGet(source));                  // SourceNameGet will return the first status bit which is set on an Axis or MultiAxis.
        }

        private static void PrintState(RSIState state)
        {
            Console.WriteLine("\nYour Axis is in state: " + state);
        }

    }
}

