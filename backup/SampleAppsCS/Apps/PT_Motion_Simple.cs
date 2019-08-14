/// @example PT_Motion_Simple.cs  
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
  
 This sample applications demonstrates how to command PVAJT motion. 

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

    class PT_Motion_Simple
    {
        const int AXIS_X = 0;
        const int AXIS_Y = 1;
        const int POINTS = 3;
        const int AXIS_COUNT_IN_AXIS = 1;
        const int AXIS_COUNT_IN_MULTIAXIS = 2;

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
                
                double[] p = new double[POINTS * AXIS_COUNT_IN_AXIS];
                double[] t = new double[POINTS];

                double[] p1 = new double[POINTS * AXIS_COUNT_IN_AXIS];
                double[] t1 = new double[POINTS];

                double[] p2 = new double[POINTS * AXIS_COUNT_IN_MULTIAXIS];
                double[] t2 = new double[POINTS];

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


                /*************************************************************
                 * Single Axis Move PT (BSPLINE) Example  
                 * **********************************************************/
                
                
                //Clear faults and Enable MultiAxis group
                axisX.Abort();
                axisX.ClearFaults();
                //Assert.That(axisX.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set Position to 0
                axisX.PositionSet(0);

                axisX.AmpEnableSet(true);
                Console.WriteLine("initialize BSLINE positions");

                //Position and time points 
                p[0] = 050;   t[0] = 1;
                p[1] = 400;   t[1] = 3.5;
                p[2] = 450;   t[2] = 1;

                // Load arrays and Execute PT BSpline motion. 
                axisX.MovePT(RSIMotionType.RSIMotionTypeBSPLINE, p, t, POINTS, -1, false, true);
                
                //While loop to wait for completion of PT motion (normally do a axis.MotionDoneWait())
                while (p[POINTS-1] != axisX.CommandPositionGet()) { }
                Console.WriteLine("MovePT-BSPLINE Motion Complete");
                
                controller.OS.Sleep(250);
                axisX.Abort();
                controller.OS.Sleep(250);

                
                /*************************************************************
                 * Single Axis Move PT (SimplePT) Example  
                 * **********************************************************/


                //Clear faults and Enable MultiAxis group
                axisY.ClearFaults();
                //Assert.That(axisY.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set Position to 0
                axisY.PositionSet(0);

                axisY.AmpEnableSet(true);
                Console.WriteLine("initialize Simple PT positions");

                //Position and time points 
                p1[0] = 50; t1[0] = 1;
                p1[1] = 400; t1[1] = 3.5;
                p1[2] = 450; t1[2] = 1;
                
                // Load arrays and Execute SimplePT motion. 
                axisY.MovePT(RSIMotionType.RSIMotionTypePT, p1, t1, POINTS, -1, false, true);

                //While loop to wait for completion of PT motion (normally do a axis.MotionDoneWait())
                while (p1[POINTS-1] != axisY.CommandPositionGet()) { }
                Console.WriteLine("PT-SimplePT Motion Complete");

                controller.OS.Sleep(250);
                axisY.Abort();
                controller.OS.Sleep(250);


                /*************************************************************
                 * MultiAxis Move PT (BSPLINE) Example  
                 * **********************************************************/


                //Clear faults and Enable MultiAxis group
                multiAxisXY.ClearFaults();
                //Assert.That(multiAxisXY.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set Position to 0
                axisX.PositionSet(0);
                axisY.PositionSet(0);

                multiAxisXY.AmpEnableSet(true);
                
                Console.WriteLine("initialize MultiAxis PT BSPLINE positions");

                //Motion Segment #1
                p2[0] = 50; t2[0] = 1;
                p2[1] = 25;
                //Motion Segment #2
                p2[2] = 400; t2[1] = 3.5;
                p2[3] = 200;
                //Motion Segment #3
                p2[4] = 450; t2[2] = 1;
                p2[5] = 225;


                // Load arrays and Execute SimplePT motion. 
                multiAxisXY.MovePT(RSIMotionType.RSIMotionTypeBSPLINE, p2, t2, POINTS, -1, false, true);

                //While loop to wait for completion of PT motion (normally do a axis.MotionDoneWait())
                while (p2[4] != axisX.CommandPositionGet() | p2[5] != axisY.CommandPositionGet()) { }
                Console.WriteLine("MultiAxisXY PT-BSPLINE Motion Complete");

                controller.OS.Sleep(250);
                multiAxisXY.Abort();       
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}