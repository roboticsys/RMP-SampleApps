/// @example Capture.cs  Perform a velocity move and capture position based on encoder Index pulse.
/* Capture.cs
 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 In this sample code, we initialize the controller and axis. Then we disable the capture arm 
 before calling CaptureConfigSet in which we set the paramters for capture. 
 
 The inputs are discussed below:
 CaptureConfigSet(CAPTURE_TYPE, CAPTURE_SOURCE, CAPTURE_TRIGGER_EDGE,
  			     CAPTURE_FEEDBACK, CAPTURE_ENCODER, CAPTURE_GLOBAL_TRIGGER)
 
 CAPTURE_TYPE: User can decide whether he wants to capture based on Position or Time.
 CAPTURE_SOURCE: Which output trigger is desired to capture position. capture1.cpp uses INDEX.
 CAPTURE_TRIGGER_EDGE: Choose from three option: Rising, Falling or Either.
 CAPTURE_FEEDBACK: Axis number being used.
 CAPTURE_ENCODER: User can choose from motor Primary or some other Secondary encoder.
 CAPTURE_GLOBAL_TRIGGER will always be set to 0 in Rapid Code.
 To understand the above inputs graphically please use our software 'Rapid Setup' and visit Homing Tab.

 Once CaptureConfigSet has been called we can set function CaptureArm() back to true. We then perform a 
 Velocity Move until the encoder's index pulse occurs.

 Finally when the capture occurs, the position is displayed.

 For any questions regarding this sample code please visit our documentation at www.roboticsys.com

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.
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
    public class Capture
    {
        // RapidCode objects
        MotionController controller;
        Axis axis;

        //constants
        const int AXIS_NUMBER = 0; 
        const int CAPTURE_FEEDBACK_AXIS_NUMBER = AXIS_NUMBER;
        const RSICaptureType CAPTURE_TYPE = RSICaptureType.RSICaptureTypeTIME;
        const RSICaptureSource CAPTURE_SOURCE = RSICaptureSource.RSICaptureSourceHOME;
        const RSICaptureEdge CAPTURE_TRIGGER_EDGE = RSICaptureEdge.RSICaptureEdgeEITHER;
        const RSIMotorEncoder CAPTURE_ENCODER = RSIMotorEncoder.RSIMotorEncoderPRIMARY;
        const bool CAPTURE_GLOBAL_TRIGGER = false;
         
        const int VELOCITY = 2000;
        const int ACCEL = 5000;

        [Test]
        public void Main()
        {
            double capturedPosition;
            try
            {
                //Initialize Controller Class
                controller = MotionController.Create();

                //Initialize Axis Class
                axis = controller.AxisGet(AXIS_NUMBER);

                // enable interrupts
                axis.InterruptEnableSet(false);
		        axis.InterruptEnableSet(true);
                
		        // make sure Capture is not armed
		        axis.CaptureArm(false);

		        // setting the Capture Configurations, parameters discussed above
		        axis.CaptureConfigSet(CAPTURE_TYPE, CAPTURE_SOURCE, CAPTURE_TRIGGER_EDGE,
							           CAPTURE_FEEDBACK_AXIS_NUMBER, CAPTURE_ENCODER,
							           CAPTURE_GLOBAL_TRIGGER);

                //Clear Faults and enable Axis
                axis.ClearFaults();
                axis.AmpEnableSet(true);

		        // arming Capture before commanding a velocity move
		        axis.CaptureArm(true);
		        
		        // setup Home Action (the home action will trigger when CaptureStatus == Captured)
                axis.HomeActionSet(RSIAction.RSIActionE_STOP);
        		
		        // commanding a velocity move
		        axis.MoveVelocity(VELOCITY, ACCEL);
        		
		        Console.WriteLine("\n Moving...Waiting for Capture Source" +  
                    " (Encoder Index pulse) edge to Capture a position... \n");

                // wait (sleep) until motion done interrupt occurs
                while (axis.InterruptWait((int)RSIWait.RSIWaitFOREVER) != RSIEventType.RSIEventTypeMOTION_DONE)
                {
                }

		        // check the CaptureState
                if (axis.CaptureStateGet() == RSICaptureState.RSICaptureStateCAPTURED)
		        {
			        // subtract Origin Position from Captured Position
			        capturedPosition =  (axis.CapturePositionGet() - axis.OriginPositionGet());
			        Console.WriteLine("\n Captured Position: " + capturedPosition + "\n\n ");
		        }
		        else
		        {
                    Console.WriteLine(" The Capture never triggered.\n");
		        }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

