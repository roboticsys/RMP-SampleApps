/// @example UserLimitActionBasedOnDigitalInput.cs   Set a stopping Action based on an digital input trigger
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
using NUnit.Framework;
using RSI.RapidCode.SynqNet.dotNET;
using RSI.RapidCode.SynqNet.dotNET.Enums;

namespace SampleApplications
{    
    [TestFixture]
    class UserLimitActionBasedOnDigitalInput
    {
        const int AXIS_NUM = 0;
        const int INPUT_BIT_NUMBER = 0;
        const int USERLIMIT = 0;
        const int CONDITION = 0;

        // Set User Units: 4294967296 counts/motor rev, 360 degrees per gearhead rev.
        // 4294967296 / 360 = 11930464
        const double COUNTS_PER_DEGREE = 11930464;
        
        [Test]
        public void RunProgram()
        {
            try
            {
                // RapidCode Objects
                MotionController controller = MotionController.CreateFromBoard(0);
                Axis axis = controller.AxisGet(AXIS_NUM);
                IOPoint EStopBit = IOPoint.CreateDigitalInput(axis, RSIMotorGeneralIo.RSIMotorGeneralIo2);  //AKD DIN3 X7, Pin 4 
                                               
                // this sample uses Interrupts
                controller.InterruptEnableSet(true);

                // Prepare axis for motion
                axis.Abort();
                axis.ClearFaults();
                axis.AmpEnableSet(true);

                // Setup apropriate EStop time in seconds
                axis.EStopTimeSet(2); 
                
                // Set axis User Units to Degrees
                axis.UserUnitsSet(COUNTS_PER_DEGREE);
   
                // Command a 3600 deg/s velocity move.
                axis.MoveVelocity(3600, 36000);

                // configure user limit to evaluate Digital in EStopBit-> trigger state HIGH
                controller.UserLimitConditionSet(USERLIMIT,
                    CONDITION,
                    RSIUserLimitLogic.RSIUserLimitLogicEQ,
                    EStopBit.AddressGet(),
                    EStopBit.MaskGet(),
                    EStopBit.MaskGet());

                // configure user limit to evaluate Digital in EStopBit-> trigger state LOW
                //controller.UserLimitConditionSet(USERLIMIT,
                //    CONDITION,
                //    RSIUserLimitLogic.RSIUserLimitLogicNE,
                //    EStopBit.AddressGet(),
                //    EStopBit.MaskGet(),
                //    EStopBit.MaskGet());
                     
                // set the configuration
                controller.UserLimitConfigSet(USERLIMIT, RSIUserLimitTriggerType.RSIUserLimitTriggerTypeSINGLE_CONDITION, RSIAction.RSIActionE_STOP_ABORT, AXIS_NUM, 0);
                Console.Write("Waiting for input to trigger...");

                // wait for user limit to trigger
                while (controller.InterruptWait((int)RSIWait.RSIWaitFOREVER) != RSIEventType.RSIEventTypeUSER_LIMIT){ }
                
                Console.Write("Axis should be ABORTED!\n\n");
                
                // disable User Limit 
                controller.UserLimitDisable(USERLIMIT);
                                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail();
            }


        }


    }
}


