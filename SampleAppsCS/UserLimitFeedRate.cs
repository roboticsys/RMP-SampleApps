/*! 
 *  @example    UserLimitFeedRate.cs 
 
 *  @page       userlimit-feedrate-cs UserLimitFeedRate.cs
 
 *  @brief      UserLimit FeedRate sample application.
 
 *  @details    Configure a UserLimit to change the FeedRate when the axis has reached a specified position.
 
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
 
 *  @include UserLimitFeedRate.cs 
 */

using System;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleAppsCS
{
    class UserLimitFeedRate
    {
        static void Main(string[] args)
        {
            // RapidCode objects
            MotionController controller;                                                                                        // Declare what 'controller' is.
            Axis axis;                                                                                                          // Declare what 'axis' is.


            // Constants
            const int       AXIS_INDEX          = 0;                                                                            // This is the index of the axis you will use to command motion.
            const int       USER_LIMIT          = 0;                                                                            // Specify which user limit to use.
            const int       USER_LIMIT_COUNT    = 1;
            const int       CONDITION           = 0;                                                                            // Specify which condition to use. (0 or 1) ("0" to compare 1 input || "1" to compare 2 inputs)
            const RSIUserLimitLogic LOGIC       = RSIUserLimitLogic.RSIUserLimitLogicGT;                                        // Logic for input value comparison.
            const double    POSITION_TRIGGER_VALUE = 250;                                                                       // The value to be compared which needs to be set here.
            const RSIUserLimitTriggerType TRIGGER_TYPE = RSIUserLimitTriggerType.RSIUserLimitTriggerTypeSINGLE_CONDITION;       // Choose the how the condition (s) should be evaluated. 
            const RSIAction ACTION              = RSIAction.RSIActionNONE;                                                      // Choose the action you want to cause when the User Limit triggers.
            const int       DURATION            = 0;                                                                            // Enter the time delay before the action is executed after the User Limit has triggered.
            const double    DEFAULT_FEED_RATE   = 1.0;
            const double    DESIRED_FEED_RATE   = 2.0;


            // Other Global Variables
            ulong feedRateAddress;


            // Initialize RapidCode Objects
            controller = MotionController.CreateFromSoftware();                                                                 // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                                               // [Helper Function] Check that the controller has been initialize correctly. 


            // Some Necessary Pre User Limit Configuration
            controller.UserLimitCountSet(USER_LIMIT_COUNT);                                                                     // Set the amount of UserLimits that you want to use.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                                                           // [Helper Function] Initialize the network.

            axis = controller.AxisGet(AXIS_INDEX);                                                                              // Initialize your axis object.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);


            try
            {
                axis.FeedRateSet(DEFAULT_FEED_RATE);                                                                            // Restore FeedRate to default value.

                // USER LIMIT CONDITION
                controller.UserLimitConditionSet(USER_LIMIT, CONDITION, LOGIC, axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeACTUAL_POSITION), POSITION_TRIGGER_VALUE);              // Set your User Limit Condition (1st step to setting up your user limit)

                // USER LIMIT CONFIGURATION
                controller.UserLimitConfigSet(USER_LIMIT, TRIGGER_TYPE, ACTION, axis.NumberGet(), DURATION);                    // Set your User Limit Configuration. (2nd step to setting up your user limit)

                // USER LIMIT OUTPUT 
                feedRateAddress = axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeTARGET_FEEDRATE);
                controller.UserLimitOutputSet(USER_LIMIT, DESIRED_FEED_RATE, feedRateAddress, true);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
