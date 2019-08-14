/// @example MotorFeedback.cs   Reads the primary and secondary feedback for a motor.
/*  MotorFeedback.cs

 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 MotorFeedback : Reads primary and secondary feedback for a motor.

 The function axis.EncoderPositionGet() retrieves raw encoder position for an axis. The encoder 
 position is not scaled by the origin. The encoder position is sometimes called 'motor feedback position.'
 To get actual position retrieved by controller please take a look at axis.ActualPositionGet()

 For any questions regarding this sample code please visit our documentation at 
 www.roboticsys.com/rapidcode

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
    public class MotorFeedback
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
                // initialize MotionController class
		        controller = MotionController.Create();
                axis = controller.AxisGet(AXIS_NUMBER);
              
		        //returns the motor type
		        Console.WriteLine("Motor Type: ");
                switch(axis.MotorTypeGet()) 
                {
	                case RSIMotorType.RSIMotorTypeSERVO:
                        Console.WriteLine("Servo\n"); 
		                break;
                    case RSIMotorType.RSIMotorTypeSTEPPER:
                        Console.WriteLine("Stepper\n"); 
		                break;
                    case RSIMotorType.RSIMotorTypePHANTOM:
                        Console.WriteLine("Phantom\n"); 
		                break;
        			default:
                        Console.WriteLine("Invalid\n");
		                break;
                }
                //returns the type of encoder being used 
                Console.WriteLine("Encoder Type : ");
                switch(axis.EncoderTypeGet())
                {
                    case RSIMotorEncoderType.RSIMotorEncoderTypeQUAD_AB:
                        Console.WriteLine("QUAD_AB\n");
		                break;
                    case RSIMotorEncoderType.RSIMotorEncoderTypeDRIVE:
                        Console.WriteLine("DRIVE\n");
		                break;
                    case RSIMotorEncoderType.RSIMotorEncoderTypeSSI:
                        Console.WriteLine("SSI\n");
		                break;
                    default:
                        Console.WriteLine("Invalid\n");
		                break;
                }
                while (true)
	                {
		                //returns the raw encoder position for an axis
                        Console.WriteLine("\nPrimary: " + axis.EncoderPositionGet(RSIMotorEncoder.RSIMotorEncoderPRIMARY) +  						                
                                        "\nSecondary: " + axis.EncoderPositionGet(RSIMotorEncoder.RSIMotorEncoderSECONDARY));
                        controller.OS.Sleep(1000);
                    }	
	        }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
