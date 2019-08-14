/*! 
 *  @example    UserLimitCommandPositionDirectSet.cs 
 
 *  @page       userlimit-commandpositiondirectset-cs UserLimitCommandPositionDirectSet.cs
 
 *  @brief      UserLimit directly sets a command position sample application.
 
 *  @details    Configure two UserLimits.  The first will trigger on a digital input and copy the Axis Actual Position to the UserBuffer and decelerate to zero velocity with TRIGGERED_MODIFY.
 *   The second UserLimit will trigger after the first UserLimit triggers and the Axis gets to IDLE stae and it will directly set the command position of an Axis from the position stored in the UserBuffer.
 
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
 
 *  @include UserLimitCommandPositionDirectSet.cs 
 */

using System;
using System.Runtime.InteropServices;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleAppsCS
{
    class UserLimitCommandPositionDirectSet
    {
        // RapidCode objects
        static MotionController controller;                                                                                        // Declare what 'controller' is.
        static Axis axis;
        
        // Constants
        const int AXIS_INDEX = 0;                                                                           // This is the index of the axis you will use to command motion.
        const int USER_LIMIT_FIRST = 0;                                                                     // Specify which user limit to use.
        const int USER_LIMIT_SECOND = 1;
        const int USER_LIMIT_COUNT = 2;
        const RSIUserLimitLogic LOGIC = RSIUserLimitLogic.RSIUserLimitLogicEQ;                              // Logic for input value comparison.
        const RSIUserLimitTriggerType TRIGGER_TYPE = RSIUserLimitTriggerType.RSIUserLimitTriggerTypeSINGLE_CONDITION;       // Choose the how the condition (s) should be evaluated. 
        const RSIAction ACTION = RSIAction.RSIActionTRIGGERED_MODIFY;                                       // Choose the action you want to cause when the User Limit triggers.

        const double VELOCITY = 10000.0;
        const double ACCEL = 100000.0;
        const double DECEL = 1000000000000000000000.0;
        const double JERK_PCT = 0.0;
        const int DURATION = 0;                                                                             // Enter the time delay before the action is executed after the User Limit has triggered.
        const bool ONE_SHOT = true;                                                                         // if true, User Limit will only trigger ONCE

        // User Limit Interrupt constants
        const int COMMAND_POSITION_INDEX = 0;
        const int ACTUAL_POSITION_INDEX = 1;
        const int TC_COMMAND_POSITION_INDEX = 2;
        const int TC_ACTUAL_POSITION_INDEX = 3;

        static void ConfigureUserLimitInterrupts(int userLimitIndex)
        {
            controller.UserLimitInterruptUserDataAddressSet(userLimitIndex, COMMAND_POSITION_INDEX, axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeCOMMAND_POSITION));
            controller.UserLimitInterruptUserDataAddressSet(userLimitIndex, ACTUAL_POSITION_INDEX, axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeACTUAL_POSITION));
            //controller.UserLimitInterruptUserDataAddressSet(userLimitIndex, TC_COMMAND_POSITION_INDEX, axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeTC_COMMAND_POSITION));
            //controller.UserLimitInterruptUserDataAddressSet(userLimitIndex, TC_ACTUAL_POSITION_INDEX, axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeTC_ACTUAL_POSITION));
        }

        static void WaitForInterrupts()
        {
            bool done = false;
            int timeout_millseconds = 10000;

            while(!done)
            {
                RSIEventType eventType = controller.InterruptWait(timeout_millseconds);

                Console.WriteLine("IRQ: " + eventType.ToString() + " at sample " + controller.InterruptSampleTimeGet());

                switch(eventType)
                {
                    case RSIEventType.RSIEventTypeUSER_LIMIT:
                        Console.WriteLine("UserLimit " + controller.InterruptSourceNumberGet());
                        Console.WriteLine("CmdPos: " + BitConverter.ToDouble(BitConverter.GetBytes(controller.InterruptUserDataGet(COMMAND_POSITION_INDEX)), 0));
                        Console.WriteLine("ActPos: " + BitConverter.ToDouble(BitConverter.GetBytes(controller.InterruptUserDataGet(ACTUAL_POSITION_INDEX)), 0));
                        Console.WriteLine("TC.CmdPos: " + BitConverter.ToDouble(BitConverter.GetBytes(controller.InterruptUserDataGet(TC_COMMAND_POSITION_INDEX)), 0));
                        Console.WriteLine("TC.ActPos: " + BitConverter.ToDouble(BitConverter.GetBytes(controller.InterruptUserDataGet(TC_ACTUAL_POSITION_INDEX)), 0));
                        break;
                    case RSIEventType.RSIEventTypeTIMEOUT:
                        done = true;
                        break;
                    default:
                        break;
                }
            }
        }


        static void Main(string[] args)
        {
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
                // set the triggered modify values to stop very quickly
                axis.TriggeredModifyDecelerationSet(DECEL);
                axis.TriggeredModifyJerkPercentSet(JERK_PCT);

                //////////// FIRST USER LIMIT ////////////
                // USER LIMIT CONDITION 0 (trigger on digital input)
                controller.UserLimitConditionSet(USER_LIMIT_FIRST, 0, LOGIC, axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeDIGITAL_INPUTS), 0x400000, 0x400000);              // Set your User Limit Condition (1st step to setting up your user limit)

                //// USER LIMIT OUTPUT  (copy TC.ActualPosition to UserBuffer)
               // controller.UserLimitOutputSet(USER_LIMIT_FIRST, RSIDataType.RSIDataTypeDOUBLE,  axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeTC_ACTUAL_POSITION), controller.AddressGet(RSIControllerAddressType.RSIControllerAddressTypeUSER_BUFFER), true);

                //// USER LIMIT CONFIGURATION  (cause a TRIGGERED_MODIFY action on the Axis)
                controller.UserLimitConfigSet(USER_LIMIT_FIRST, TRIGGER_TYPE, ACTION, axis.NumberGet(), DURATION, ONE_SHOT);                    // Set your User Limit Configuration. (2nd step to setting up your user limit)


                //////////// SECOND USER LIMIT ////////////
                // CONDITION 0  (wait for first user limit to trigger)
                controller.UserLimitConditionSet(USER_LIMIT_SECOND, 0, RSIUserLimitLogic.RSIUserLimitLogicEQ, controller.AddressGet(RSIControllerAddressType.RSIControllerAddressTypeUSERLIMIT_STATUS, USER_LIMIT_FIRST), 1, 1);

                // CONDITION 1 (AND wait for Axis command velcity = 0.0)
                controller.UserLimitConditionSet(USER_LIMIT_SECOND, 1, RSIUserLimitLogic.RSIUserLimitLogicEQ, axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeCOMMAND_VELOCITY), 0.0);
                
                // OUTPUT (copy value from UserBuffer to TC.CommandPosition when trigered)
                //controller.UserLimitOutputSet(USER_LIMIT_SECOND, RSIDataType.RSIDataTypeDOUBLE, controller.AddressGet(RSIControllerAddressType.RSIControllerAddressTypeUSER_BUFFER), axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeTC_COMMAND_POSITION), true);
                controller.UserLimitConfigSet(USER_LIMIT_SECOND, RSIUserLimitTriggerType.RSIUserLimitTriggerTypeCONDITION_AND, RSIAction.RSIActionNONE, 0, 0, ONE_SHOT);


                // get the Axis moving
                axis.ClearFaults();
                axis.AmpEnableSet(true);
                axis.MoveVelocity(VELOCITY, ACCEL);

                // configure and enable interrupts
                ConfigureUserLimitInterrupts(USER_LIMIT_FIRST);
                ConfigureUserLimitInterrupts(USER_LIMIT_SECOND);
                controller.InterruptEnableSet(true);

                // wait for (and print) interrupts
                WaitForInterrupts();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
