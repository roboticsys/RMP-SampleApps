/// @example PointToPointMotion_MoveVector.cs  
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
  
 This sample application demonstrates point to point using multiaxis.MoveVector() 

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

    class PointToPointMotion_MoveVector
    {
        const int AXIS_X = 0;
        const int AXIS_Y = 1;
        const int MOTOR_RES_16 = 65536;  //Motor Resolution = counts per 1 motor Rev

        // Count required is the number of Axes + Number of MultiAxis Objects
        int CalculateMotionCountRequired()
        {
            //This application needs 1 extra motion supervisor for the MultiAxis Object.
            int requiredSupervisors = -1;

            //Without a specific Axis Count, we look for the highest defined Axis Number.  Generally you should just know the number of Axes.
            requiredSupervisors = Math.Max(requiredSupervisors, AXIS_X);
            requiredSupervisors = Math.Max(requiredSupervisors, AXIS_Y);
            requiredSupervisors++; //Axis Numbers are Zero Ordinate.  You'll always want 1 more.  1+1 = 2 Axes.

            requiredSupervisors++; //Add 1 for each MultiAxis required.  1 for this application.

            return requiredSupervisors;
        }
                

        public void Main()
        {
            try
            {
                //Create RapidCode objects
                MotionController controller;
                Axis axisX;
                Axis axisY;
                MultiAxis multiAxisXY;

                //double[] XYRatio = new double[2] { Xunits, Yunits };
                double[] startPosition = new double[2] { 20, 100 };
                double[] point1 = new double[2] { 80, 60 };
                double[] point2 = new double[2] { 40, 20 };
                double[] point3 = new double[2] { 20, 100 };
                
                
                //Initialize controller object
                controller = MotionController.CreateFromSoftware();

                //Set MotionCount if needed.
                int requiredMotionCount = CalculateMotionCountRequired();
                if (controller.MotionCountGet() < requiredMotionCount)
                    controller.MotionCountSet(requiredMotionCount);
                            
                //Initialize axis and multiaxis objects
                axisX = controller.AxisGet(AXIS_X);
                axisY = controller.AxisGet(AXIS_Y);
                multiAxisXY = controller.MultiAxisGet(controller.AxisCountGet()); //The first free Motion Supervisor should equal the Axis Count as Motion Supervisors are Zero Ordinate.
                multiAxisXY.AxisAdd(axisX);
                multiAxisXY.AxisAdd(axisY);
                
                //Do not use UserUnits with Path Motion!
                axisX.UserUnitsSet(MOTOR_RES_16); 
                axisY.UserUnitsSet(MOTOR_RES_16);

                //Abort, Clear Faults and check for IDLE State
                multiAxisXY.Abort();
                multiAxisXY.ClearFaults();
                //Assert.That(multiAxisXY.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set the Start Position.
                axisX.PositionSet(startPosition[0]);
                axisY.PositionSet(startPosition[1]);
                multiAxisXY.AmpEnableSet(true);

                //Vector Paramters set motion attributes along the path of the end effector. 
                //Units are in raw counts. Vectors are NOT scaled by UserUnits or PathRatio for MoveVector().
                multiAxisXY.VectorVelocitySet(10 * MOTOR_RES_16);  
                multiAxisXY.VectorAccelerationSet(100 * MOTOR_RES_16);    
                multiAxisXY.VectorDecelerationSet(100 * MOTOR_RES_16);  
               
                //Start the Path List, add Path segments, and End the List
                multiAxisXY.MoveVector(point1);
                multiAxisXY.MotionDoneWait();
                multiAxisXY.MoveVector(point2);
                multiAxisXY.MotionDoneWait();
                multiAxisXY.MoveVector(point3);
                multiAxisXY.MotionDoneWait();
                
                multiAxisXY.Abort();
                multiAxisXY.Unmap();
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}
