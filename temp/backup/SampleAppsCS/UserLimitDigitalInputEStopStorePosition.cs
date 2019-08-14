/*! 
 *  @example    UserLimitDigitalInputEStopStorePosition.cs
 
 *  @page       user-limit-digital-input-estop-store-position-cs UserLimitDigitalInputEStopStorePosition.cs
 
 *  @brief      User Limit Digital Input E-Stop Store Position sample application.
 
 *  @details    
    This sample code shows how to configure a RMP controller's User Limit to compare an input bit to a specific signal (high signal (1) OR low signal (0)).  
    If the (1 condition) pattern matches, then the specified input bit has been activated (turned high) and a User limit Event will trigger.
   
    In this example we configure a user limit to trigger when our INPUT turns high(1). Once the INPUT turns high(1) then our user limit will command an E-Stop action on the Axis and store the Axis Command Position.
  
        The INPUT is specified in UserLimitConditionSet()
        The User Limit configuration is done on UserLimitConfigSet()
        The specified address to record on User Limit Event is specified in UserLimitInterruptUserDataAddressSet()
        The Data from the speficified addres is retrieved by calling InterruptUserDataGet()

    In this example Beckhoff IO Terminals (Model EL1088 for Inputs) were used to control the Digital IO signals. 
  
    Make sure to check the correct digital IO signal indexes of your system in: RapidSetup -.
 Tools -.
 NetworkIO 
  
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
 
 *  @include UserLimitDigitalInputEStopStorePosition.cs
 */

using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;
using System;


namespace SampleAppsCS
{
    class UserLimitDigitalInputEStopStorePosition
    {
        static void Main(string[] args)
        {
            // RapidCode objects
            MotionController controller;                                                                                            // Declare what 'controller' is.
            Axis axis;                                                                                                              // Declare what 'axis' is.

            // Constants
            const int                       INPUT_INDEX     = 10;                                                                   // This is the index of the digital input you will use to trigger the user limit.
            const int                       AXIS_INDEX      = 0;                                                                    // This is the index of the axis you will use to command motion.
            const int                       AXIS_COUNT      = 1;                                                                    // Axes on the network.
            const int                       USER_UNITS      = 1048576;                                                              // Specify your motor's count per units.
            const int                       USER_LIMIT      = 1;                                                                    // Specify which user limit to use.
            const int                       CONDITION       = 0;                                                                    // Specify which condition to use. (0 or 1) ("0" to compare 1 input || "1" to compare 2 inputs)
            const RSIUserLimitLogic         LOGIC           = RSIUserLimitLogic.RSIUserLimitLogicEQ;                                // Logic for input value comparison.
            const int                       INPUT_MASK      = 1;                                                                    // Decide the bits in an address which need to be used when comparing. Use 0xFFFFFFFF when a long or float comparison is desired.
            const int                       LIMIT_VALUE     = 1;                                                                    // The value to be compared which needs to be set here.
            const RSIUserLimitTriggerType   TRIGGER_TYPE    = RSIUserLimitTriggerType.RSIUserLimitTriggerTypeSINGLE_CONDITION;      // Choose the how the condition (s) should be evaluated. 
            const RSIAction                 ACTION          = RSIAction.RSIActionE_STOP_ABORT;                                      // Choose the action you want to cause when the User Limit triggers.
            const int                       DURATION        = 0;                                                                    // Enter the time delay before the action is executed after the User Limit has triggered.
            const int                       USER_DATA_INDEX = 0;

            // Other Global Variables
            ulong inputAddress;

            // Initialize RapidCode Objects
            controller = MotionController.CreateFromSoftware();                                                                     // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                                                   // [Helper Function] Check that the controller has been initialize correctly. 
            
            // Some Necessary Pre User Limit Configuration
            controller.UserLimitCountSet(1);                                                                                        // Set the amount of UserLimits that you want to use.
            controller.InterruptEnableSet(true);                                                                                    // Enable User Limit Interrupts. (When a user limit input comparison is met, and interrupt is triggered)

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                                                               // [Helper Function] Initialize the network.

            axis = controller.AxisGet(AXIS_INDEX);                                                                                  // Initialize your axis object.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                                                         // [Helper Function] Check that the axis has been initialize correctly.

            try
            {
                // GET AXIS READY & MOVING
                axis.UserUnitsSet(USER_UNITS);                                                                                      // Specify the counts per unit. (every 1048576 counts my motor does one full turn) (varies per motor)
                axis.ErrorLimitTriggerValueSet(1);                                                                                  // Specify the position error limit trigger value. (learn more about this on our support page)
                axis.PositionSet(0);                                                                                                // Ensure the motor starts at position 0 every time.
                axis.Abort();                                                                                                       // If there is any motion happening, abort it.
                axis.ClearFaults();                                                                                                 // Clear any faults.
                axis.AmpEnableSet(true);                                                                                            // Enable the axis.
                axis.MoveVelocity(1.0, 10.0);                                                                                       // Command a velocity move (Velocity=1.0, Acceleration=10.0).


                // USER LIMIT CONDITION
                inputAddress    = controller.NetworkInputAddressGet(INPUT_INDEX);                                                   // 10 was the index of my 1st input. (To check your IO indexes go to RapidSetup -.


                controller.UserLimitConditionSet(USER_LIMIT, CONDITION, LOGIC, inputAddress, INPUT_MASK, LIMIT_VALUE);              // Set your User Limit Condition (1st step to setting up your user limit)


                // USER LIMIT CONFIGURATION
                controller.UserLimitConfigSet(USER_LIMIT, TRIGGER_TYPE, ACTION, AXIS_INDEX, DURATION);                              // Set your User Limit Configuration. (2nd step to setting up your user limit)


                // USER LIMIT USER DATA SET
                controller.UserLimitInterruptUserDataAddressSet(USER_LIMIT,                                                                         // Specify the user limit you want to add User Data for.
                                                                USER_DATA_INDEX,                                                                    // Specify what user data index you would like to use. (must be a value from 0 to 4)
                                                                axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeCOMMAND_POSITION));            // Specify the address of the data value you want to store in your User Data so that you can retrieve it later after the UserLimit limit triggers.


                // WAIT FOR DIGITAL INPUT TO TRIGGER USER LIMIT EVENT.
                Console.WriteLine("Waiting for the input bit to go high...\n");
                while (controller.InterruptWait((int)RSIWait.RSIWaitFOREVER) != RSIEventType.RSIEventTypeUSER_LIMIT)                                // Wait until your user limit triggers.
                {
                }

                int     triggeredUserLimit  = controller.InterruptSourceNumberGet()-AXIS_COUNT;                                                     // Check that the correct user limit has triggered. (an extra user limit is allocated for each axis)
                UInt64  data                = controller.InterruptUserDataGet(USER_DATA_INDEX);                                                     // Get the data stored in the user data you configured.
                double  interruptPosition   = BitConverter.ToDouble(BitConverter.GetBytes(data), 0);                                                // Convert from raw 64-bit memory bytes into 64-bit double.

                Console.WriteLine("Input bit went HIGH and User Limit {0} triggered!", triggeredUserLimit);                                         // Get the index of the user limit that triggered.
                Console.WriteLine("User Limit Interrupt Position = "+ interruptPosition/USER_UNITS);                                                // Get the position of the axis when it user limit event triggered.
                Console.WriteLine("\nDoes InterruptSourceNumberGet() - AxisCountGet() == userLimit: " + triggeredUserLimit.Equals(USER_LIMIT));

                controller.UserLimitDisable(USER_LIMIT);                                                                                             // Disable User Limit.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

