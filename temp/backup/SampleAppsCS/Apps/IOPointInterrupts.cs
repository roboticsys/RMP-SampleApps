/// @example IOPointInterrupts.cs   Generate interrupts based on I/O points
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

//namespace SampleApplications
//{

//    public class IOPointInterrupts
//    {
//        private const int PHANTOM_AXIS_NUMBER = 0;
//        const int  BIT_NUMBER = 0;                                 // Controller I/O bit number
//        const int TIMEOUT_MILLISECONDS = 10000;                   // 10 seconds before timeout              


//        // need one User Limit for each state to be triggered (one ON, one OFF)
//        private const RSIEventType BIT_ON = RSIEventType.RSIEventTypeLIMIT_USER0;
//        private const RSIEventType BIT_OFF = RSIEventType.RSIEventTypeLIMIT_USER1;



//        private void ConfigureUserLimit(IOPoint point, Axis axis, RSIEventType userLimit, bool triggerState)
//        {
//            int limitValue = point.MaskGet();  // use mask for triggering when bit turns ON

//            if (triggerState == false)
//            {
//                limitValue = 0; // use zero to trigger when bit turns OFF
//            }

//            axis.UserLimitConditionSet(userLimit, 0, RSIXmpLimitType.RSIXmpLimitTypeBIT_CMP,
//                                        point.AddressGet(), point.MaskGet(), limitValue);

//            axis.UserLimitConfigSet(userLimit, RSIXmpStatus.RSIXmpStatusLIMIT, RSIXmpLogic.RSIXmpLogicSINGLE, 0.0);
//        }


//        public void Main()
//        {
//            try
//            {
//                //RapidCode Objects
//                MotionController controller = MotionController.Create();
//                IOPoint bit = IOPoint.CreateDigitalOutput(controller, BIT_NUMBER);
//                Axis phantom = controller.AxisGet(PHANTOM_AXIS_NUMBER);
//                bool done = false;

//                phantom.ClearFaults(); // won't generate interrupts if in an ERROR state
//                //Assert.That(phantom.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

//                ConfigureUserLimit(bit, phantom, BIT_ON, true);
//                ConfigureUserLimit(bit, phantom, BIT_OFF, false);
                
//                // since the User Limits are associated with the Axis, be sure to enable interrupts on the Axis
//                phantom.InterruptEnableSet(true);

//                // enable the MotionController to receive interrupts from all sources
//                controller.InterruptEnableSet(true);

//                while (!done)
//                {
//                    RSIEventType interrupt = controller.InterruptWait(TIMEOUT_MILLISECONDS);

//                    switch (interrupt)
//                    {
//                        case RSIEventType.RSIEventTypeTIMEOUT:
//                            done = true;
//                            Console.WriteLine("Timeout!");
//                            break;

//                        case BIT_ON:
//                            Console.WriteLine("Bit turned on.");
//                            break;

//                        case BIT_OFF:
//                            Console.WriteLine("Bit turned off.");
//                            break;

//                        default:
//                            break;
//                    }
//                }

//                // cleanup
//                phantom.UserLimitDisable(BIT_ON);
//                phantom.UserLimitDisable(BIT_OFF);
//                controller.InterruptEnableSet(false);   // disable and flush all interrupts

//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//        }
//    }
//}
