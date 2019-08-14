/*! 
*  @example    HelperFunctions.h

*  @page       helper-functions-cpp HelperFunctions.h

*  @brief      This class includes the source code of all our SampleAppsCPP helper functions.

*  @details    Helper functions were created to reduce code. Please note that if you would like to use this classes on your personal project you will have to replicate this class.

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

*  @include HelperFunctions.h

*/
#ifndef CPP_HELPER_FUNCTIONS
#define CPP_HELPER_FUNCTIONS

#include "rsi.h"                                    // Import our RapidCode Library. 
#include <iostream>
#include <cstdlib>

using namespace std;
using namespace RSI::RapidCode;

namespace SampleAppsCPP
{
    class HelperFunctions
    {
    public:
        /// <summary>
        /// CheckErrors from Creation Methods because they don't throw exceptions.
        /// </summary>
        /// <param name="rsiObject"></param>
        /// @code
        /// public:
        ///        static void CheckErrors(RapidCodeObject *rsiObject)
        ///        {
        ///            RsiError *err;
        ///            while(rsiObject->ErrorLogCountGet() > 0)
        ///            {
        ///                err = rsiObject->ErrorLogGet();
        ///                printf("%s\n", err->text);
        ///            }
        ///        }
        /// @endcode
        /// @see HelperFunctions
        static void CheckErrors(RapidCodeObject *rsiObject)
        {
            RsiError *err;
            while (rsiObject->ErrorLogCountGet() > 0)
            {
                err = rsiObject->ErrorLogGet();
                printf("%s\n", err->text);
            }
        }

        /// <summary>
        /// Make sure you start the network with no errors.
        /// </summary>
        /// <param name="MotionController">controller</param>
        /// @code
        ///    public:
        ///        static void StartTheNetwork(MotionController *controller)
        ///        {
        ///            // Initialize the Network
        ///            if (controller->NetworkStateGet() != RSINetworkState::RSINetworkStateOPERATIONAL)            // Check if network is started already.
        ///            {
        ///                cout << "Starting Network.." << endl;
        ///                controller->NetworkStart();                                                                // If not. Initialize The Network. (This can also be done from RapidSetup Tool)
        ///            }
        ///
        ///            if (controller->NetworkStateGet() != RSINetworkState::RSINetworkStateOPERATIONAL)            // Check if network is started again.
        ///            {
        ///                int messagesToRead = controller->NetworkLogMessageCountGet();                            // Some kind of error starting the network, read the network log messages
        ///
        ///                for (int i = 0; i < messagesToRead; i++)
        ///                {
        ///                    cout << controller->NetworkLogMessageGet(i) << endl;                                // Print all the messages to help figure out the problem
        ///                }
        ///                cout << "Expected OPERATIONAL state but the network did not get there." << endl;
        ///                //throw new RsiError();                                                                    // Uncomment if you want your application to exit when the network isn't operational. (Comment when using phantom axis)
        ///            }
        ///            else                                                                                        // Else, of network is operational.
        ///            {
        ///                cout << "Network Started" << endl;
        ///            }
        ///        }
        /// @endcode
        /// @see HelperFunctions
        static void StartTheNetwork(MotionController *controller)
        {
            // Initialize the Network
            if (controller->NetworkStateGet() != RSINetworkState::RSINetworkStateOPERATIONAL)        // Check if network is started already.
            {
                cout << "Starting Network.." << endl;
                controller->NetworkStart();                                                          // If not. Initialize The Network. (This can also be done from RapidSetup Tool)
            }

            if (controller->NetworkStateGet() != RSINetworkState::RSINetworkStateOPERATIONAL)        // Check if network is started again.
            {
                int messagesToRead = controller->NetworkLogMessageCountGet();                        // Some kind of error starting the network, read the network log messages

                for (int i = 0; i < messagesToRead; i++)
                {
                    cout << controller->NetworkLogMessageGet(i) << endl;                                // Print all the messages to help figure out the problem
                }
                cout << "Expected OPERATIONAL state but the network did not get there." << endl;
                //throw new RsiError();                                                             // Uncomment if you want your application to exit when the network isn't operational. (Comment when using phantom axis)
            }
            else                                                                                    // Else, of network is operational.
            {
                cout << "Network Started" << endl;
            }
        }

    };
}
#endif
