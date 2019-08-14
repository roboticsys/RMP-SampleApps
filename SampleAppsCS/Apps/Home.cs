/// @example Home.cs  This sample code performs a simple homing routine that triggers home off an input pulse, captures the hardware position, sets the origin and then moves back to that home position.
/*  
 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.
 
 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.
 
 Home.cs : This sample code performs a simple homing routine that triggers off 
 an input pulse, captures the hardware position, sets the origin and then moves back 
 to that home position.

 The home method used in this sample code (RSIHomeMethodNEGATIVE_LIMIT) is one of the
 35 homing routines available in our homing documenation.
  
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
using NUnit.Framework.SyntaxHelpers;
using RSI.RapidCode.SynqNet.dotNET;
using RSI.RapidCode.SynqNet.dotNET.Enums;

namespace SampleApplications
{
    [TestFixture]
    public class Home
    {
        // RapidCode objects
        MotionController controller;             
        Axis axis;
                
        //constants
        const int AXIS_NUMBER    = 0;
        const int VELOCITY       = 8000;
        const int ACCELERATION   = 6000;
        const int DECELERATION   = 6000;

        [Test]
        public void Main()
        {
            try
            {
                //RapidCode Objects
                controller = MotionController.Create();
                axis = controller.AxisGet(AXIS_NUMBER);
                
                axis.ClearFaults();
                axis.AmpEnableSet(true);

                axis.HardwareNegLimitActionSet(RSIAction.RSIActionSTOP);  //Neg Limit action set to STOP
                axis.HomeActionSet(RSIAction.RSIActionSTOP);
                axis.HomeMethodSet(RSIHomeMethod.RSIHomeMethodNEGATIVE_LIMIT);
                axis.HomeVelocitySet(VELOCITY);
                axis.HomeSlowVelocitySet(VELOCITY / 10);  // used for final move, if necessary
                axis.HomeAccelerationSet(ACCELERATION);
                axis.HomeDecelerationSet(DECELERATION);
                axis.HomeOffsetSet(0.0);
                axis.Home();

                if(axis.HomeStateGet() == true)
                {
                    Console.WriteLine("Homing successful\n");
                }
                axis.ClearFaults();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

