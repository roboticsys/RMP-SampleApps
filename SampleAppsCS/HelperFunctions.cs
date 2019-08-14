/*! 
 *  @example    HelperFunctions.cs
 
 *  @page       helper-functions-cs HelperFunctions.cs
 
 *  @brief      This class includes the source code of all our SampleAppsCS helper functions.
 
 *  @details    Helper functions were created to reduce code. Please note that if you would like to use this classes on your personal project you will have to replicate this class.
 
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
 
 *  @include HelperFunctions.cs
    
 */

using System;
using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;

namespace SampleAppsCS
{
    public static class HelperFunctions
    {
        /// <summary>
        /// CheckErrors from Creation Methods because they don't throw exceptions.
        /// </summary>
        /// <param name="rsiObject"></param>
        /// @code
        /// public static void CheckErrors(IRapidCodeObject rsiObject)
        /// {
        ///    while (rsiObject.ErrorLogCountGet() > 0)
        ///    {
        ///        Console.WriteLine("RSI Object: " + rsiObject + ")\n\n " + rsiObject.ErrorLogGet().Message);
        ///    }
        /// }
        /// @endcode
        /// @see HelperFunctions
        public static void CheckErrors(IRapidCodeObject rsiObject)
        {
            while (rsiObject.ErrorLogCountGet() > 0)
            {
                Console.WriteLine("RSI Object: " + rsiObject + ")\n\n " + rsiObject.ErrorLogGet().Message);
            }
        }

        /// <summary>
        /// Make sure you start the network with no errors.
        /// </summary>
        /// <param name="MotionController">controller</param>
        /// @code
        ///public static void StartTheNetwork(MotionController controller)
        ///{
        ///    // Initialize the Network
        ///    if (controller.NetworkStateGet() != RSINetworkState.RSINetworkStateOPERATIONAL)         // Check if network is started already.
        ///    {
        ///        Console.WriteLine("Starting Network..");
        ///        controller.NetworkStart();                                                          // If not. Initialize The Network. (This can also be done from RapidSetup Tool)
        ///    }
        ///
        ///    if (controller.NetworkStateGet() != RSINetworkState.RSINetworkStateOPERATIONAL)         // Check if network is started again.
        ///    {
        ///        int messagesToRead = controller.NetworkLogMessageCountGet();                        // Some kind of error starting the network, read the network log messages
        ///
        ///        for (int i = 0; i < messagesToRead; i++)
        ///        {
        ///            Console.WriteLine(controller.NetworkLogMessageGet(i));                          // Print all the messages to help figure out the problem
        ///        }
        ///        Console.WriteLine("Expected OPERATIONAL state but the network did not get there.");
        ///        //throw new RsiError();                                                             // Uncomment if you want your application to exit when the network isn't operational. (Comment when using phantom axis)
        ///    }
        ///    else                                                                                    // Else, of network is operational.
        ///    {
        ///        Console.WriteLine("Network Started");
        ///    }
        ///}
        /// @endcode
        /// @see HelperFunctions
        public static void StartTheNetwork(MotionController controller)
        {
            // Initialize the Network
            if (controller.NetworkStateGet() != RSINetworkState.RSINetworkStateOPERATIONAL)         // Check if network is started already.
            {
                Console.WriteLine("Starting Network..");
                controller.NetworkStart();                                                          // If not. Initialize The Network. (This can also be done from RapidSetup Tool)
            }

            if (controller.NetworkStateGet() != RSINetworkState.RSINetworkStateOPERATIONAL)         // Check if network is started again.
            {
                int messagesToRead = controller.NetworkLogMessageCountGet();                        // Some kind of error starting the network, read the network log messages

                for (int i = 0; i < messagesToRead; i++)
                {
                    Console.WriteLine(controller.NetworkLogMessageGet(i));                          // Print all the messages to help figure out the problem
                }
                Console.WriteLine("Expected OPERATIONAL state but the network did not get there.");
                //throw new RsiError();                                                             // Uncomment if you want your application to exit when the network isn't operational. (Comment when using phantom axis)
            }
            else                                                                                    // Else, of network is operational.
            {
                Console.WriteLine("Network Started");
            }
        }
    }
}
