/// @example MultiAxisMotion.cs   Map two axis together using the MultiAxis class and move them in a synchronized motion. 
/*  MultiAxisMotion.cs

 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 We have created arrays for start_position, end_position, velocity, accel, decel 
 and jerkPct and used them in function called MoveSCurve which commands motion for 
 both the axis. 

 SyncStart commands both motors to start moving at the same time towards their 
 respective end_positions.
 SyncEnd commands both the motors to end their motion at the same time when moving 
 towards start_positions.

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
    public class MultiAxisMotion
    {
        // RapidCode objects
        MotionController controller;             
        Axis axisX;
        Axis axisY;
        MultiAxis multiAxis;
                
        //constants
        const int AXIS_COUNT     = 2;

        [Test]
        public void Main()
        {
            long[] axisNumber = new long [AXIS_COUNT] {0, 1};
            double[] start_position = new double [2] {0, 0};
            double[] end_position = new double [2] {2000, 6000};
            double[] velocity= new double [2] {1000, 1000};
            double[] accel = new double [2] {10000, 10000};
            double[] decel = new double [2] {10000, 10000};
            double[] jerkPct = new double [2] {0, 0};
            
            try
            {
                //Initialize Controller Object
                controller = MotionController.Create();
                
                //Initialize Axis Object
                axisX = controller.AxisGet(0);
                axisY = controller.AxisGet(1);
                
                //Initialize MultiAxis Objects
                multiAxis = controller.MultiAxisGet(axisX);
                multiAxis.AxisAdd(axisY);
		        
                //clear faults and enable axes
                multiAxis.ClearFaults();
		        multiAxis.AmpEnableSet(true);

		       
	            // Set SYNC_START motion attribute mask.
	            multiAxis.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskSYNC_START);
	            Console.WriteLine("\nMotionStart...");

	            // Commanding motion using SyncStart to start motion for both the axis at the same time.
	            multiAxis.MoveSCurve(end_position, velocity, accel, decel, jerkPct);	
	            multiAxis.MotionDoneWait();
    			
	            // Calling function created on top.
	            PrintResult(axisX, axisY);

	            // Set SYNC_END motion attribute mask
	            multiAxis.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskSYNC_END);
	            Console.WriteLine("\nMotionStart...");
    			
	            // Commanding motion using SyncEnd to end motion for both the axis at the same time.
	            multiAxis.MoveSCurve(start_position, velocity, accel, decel, jerkPct);	
    			
	            // while loop to keep motor spinning while motion not completed.
	            multiAxis.MotionDoneWait();
	            PrintResult(axisX, axisY);
		    
		        Console.WriteLine("\nTrapezoidal Motion Done\n");             
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        void PrintResult(Axis X, Axis Y)
        {
            Console.WriteLine("Motion Done \nAxisX position=  Commanded: " + X.CommandPositionGet() 
                + "\tActual: " + X.ActualPositionGet() + "\n");

            Console.WriteLine("Motion Done \nAxisY position=  Commanded: " + Y.CommandPositionGet()
                + "\tActual: " + Y.ActualPositionGet() + "\n");
        }
    }
}

