/// @example UserLimitSetBitBasedOnPosition.cs   Set an IO bit when a position is exceeded
/*  
 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.

 For any questions regarding this sample code please visit www.roboticsys.com.
 ==================================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    public class UserLimitSetBitBasedOnPosition
    {

        const int IO_NODE_NUMBER = 1;
        const int OUTPUT_BIT_NUMBER = 0;
        const int USERLIMIT = 0;
        const int CONDITION = 0;

        // Set User Units: 4294967296 counts/motor rev, 360 degrees per gearhead rev.
        // 4294967296 / 360 = 11930464
        const double COUNTS_PER_DEGREE = 11930464;
        double[] TriggerArray = new double[6] { 45, 90, 135, 180, 225, 270 };


        public void RunProgram()
        {
            try
            {
                // RapidCode Objects
                MotionController controller = MotionController.CreateFromSoftware();
                Axis axis = controller.AxisGet(0);
                IO sliceio = controller.IOGet(IO_NODE_NUMBER);
                IOPoint Xray = IOPoint.CreateDigitalOutput(sliceio, OUTPUT_BIT_NUMBER);

                // turn off xray
                Xray.Set(false);
                
                // this sample uses Interrupts
                controller.InterruptEnableSet(true);
                
                // Prepare axis for motion
                axis.Abort();
                axis.ClearFaults();
                axis.AmpEnableSet(true);
                
                // Set axis User Units to Degrees
                axis.UserUnitsSet(COUNTS_PER_DEGREE);

                // set the command/actual position to zero
                axis.PositionSet(0);
                
                // Command a 360 degree move.
                axis.MoveTrapezoidal(360, 10, 100, 100);

                foreach (double d in TriggerArray)
                {
                    // configure user limit to evaluate Actual position
                    controller.UserLimitConditionSet(USERLIMIT,
                        CONDITION,
                        RSIUserLimitLogic.RSIUserLimitLogicGE,
                        axis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeACTUAL_POSITION),
                        d * COUNTS_PER_DEGREE);  

                    // set the configuration
                    controller.UserLimitConfigSet(USERLIMIT, RSIUserLimitTriggerType.RSIUserLimitTriggerTypeSINGLE_CONDITION, RSIAction.RSIActionNONE, 0, 0);

                    // set the Output to trigger the Xray output bit
                    controller.UserLimitOutputSet(USERLIMIT, 1, (uint)Xray.MaskGet(), Xray.AddressGet(), true);
                    Console.Write("Waiting for axis 0 to trigger at position " + d + ": ");

                    // wait for user limit to trigger
                    while (controller.InterruptWait((int)RSIWait.RSIWaitFOREVER) != RSIEventType.RSIEventTypeUSER_LIMIT){ }
                    Console.Write("FIRE!\n\n");

                    // wait long enough to see LED turn on
                    controller.OS.Sleep(500);

                    // disable User Limit 
                    controller.UserLimitDisable(USERLIMIT);

                    // Turn off Xray bit
                    Xray.Set(false);
                    
                }
                
                // Wait for motion to finish
                axis.MotionDoneWait();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }


        }


    }
}
