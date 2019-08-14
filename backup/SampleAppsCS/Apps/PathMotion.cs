/// @example PathMotion.cs  
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
  
 This sample application demonstrates how to command Path motion. 

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

    class PathMotion
    {
        const int AXIS_X = 0;
        const int AXIS_Y = 1;
        const int MOTOR_RES_16 = 65536;  //Motor Resolution = counts per 1 motor Rev
        const int REVS_PER_INCH_X = 5;   //Number of Revs per inch on the X axis
        const int REVS_PER_INCH_Y = 2;   //Number of Revs per inch on the Y axis

        // RapidCode creation methods don't throw exceptions, they log errors. CreateFromBoard(), AxisGet(), MultiAxisGet(), etc.
        private void CheckCreationErrors(IRapidCodeObject rapidObject)
        {
            RsiError e = null;
            while(rapidObject.ErrorLogCountGet() > 0)
            {
                e = rapidObject.ErrorLogGet();
                Console.WriteLine(e.Message + e.StackTrace); // print all logged errors
            }
            if (e != null)
                throw e;  // this will cause the program to exit
        }

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

                double Xunits = MOTOR_RES_16 * REVS_PER_INCH_X;
                double Yunits = MOTOR_RES_16 * REVS_PER_INCH_Y;

                double[] XYRatio = new double[2] { Xunits, Yunits };
                double[] startPosition = new double[2] { 20, 20 };
                double[] lineA = new double[2] { 20, 80 };
                double[] lineB = new double[2] { 60, 100 };
                double[] lineC = new double[2] { 60, 80};
                double[] arcD = new double[2] { 60, 50 };

                //Initialize controller object
                controller = MotionController.CreateFromSoftware();
                CheckCreationErrors(controller);

                //Set MotionCount if needed.
                int requiredMotionCount = CalculateMotionCountRequired();
                if (controller.MotionCountGet() < requiredMotionCount)
                    controller.MotionCountSet(requiredMotionCount);
               
                //Initialize axis and multiaxis objects
                axisX = controller.AxisGet(AXIS_X);
                CheckCreationErrors(axisX); 
                axisY = controller.AxisGet(AXIS_Y);
                CheckCreationErrors(axisY);
                multiAxisXY = controller.MultiAxisGet(controller.AxisCountGet()); //The first free Motion Supervisor should equal the Axis Count as Motion Supervisors are Zero Ordinate.
                multiAxisXY.AxisAdd(axisX);
                multiAxisXY.AxisAdd(axisY);
                CheckCreationErrors(multiAxisXY);
                
                //Do not use UserUnits with Path Motion!
                axisX.UserUnitsSet(1); 
                axisY.UserUnitsSet(1); 

                //Abort, Clear Faults and check for IDLE State
                multiAxisXY.Abort();
                multiAxisXY.ClearFaults();
                //Assert.That(multiAxisXY.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set the Start Position scaled by X and Y units.
                axisX.PositionSet(startPosition[0] * Xunits);
                axisY.PositionSet(startPosition[1] * Yunits);
                multiAxisXY.AmpEnableSet(true);

                //Vector Paramters set motion attributes along the path of the end effector. 
                multiAxisXY.VectorVelocitySet(10);  
                multiAxisXY.VectorAccelerationSet(100);  //changing accel/decel has little impact on peak accel/decel.  
                multiAxisXY.VectorDecelerationSet(100);  //Adjust TimeSlice to improve smoothness.
               
                //Set the Time slice value to get the best balance of accuracy vs smoothness.
                multiAxisXY.PathTimeSliceSet(0.16);

                //Set the PathRatio value before PathListStart (Versions greater than 4.0.2)
                multiAxisXY.PathRatioSet(XYRatio); 

                //Start the Path List, add Path segments, and End the List
                multiAxisXY.PathListStart(startPosition);
                multiAxisXY.PathLineAdd(lineA);
                multiAxisXY.PathLineAdd(lineB);
                multiAxisXY.PathLineAdd(lineC);
                multiAxisXY.PathArcAdd(arcD, -180);
                multiAxisXY.PathListEnd();
                
                //Execute motion, and Wait for done
                multiAxisXY.PathMotionStart();
                multiAxisXY.MotionDoneWait();

                multiAxisXY.Abort();
                

                /******************************************************************
                 * 
                 * Corners Example: How to get into corners while still using Path. 
                 * This Example will get into the corner between A and B but
                 * Blend between B, C and ArcD.
                 * 
                 * ****************************************************************/

                //Abort, Clear Faults and check for IDLE State
                multiAxisXY.Abort();
                multiAxisXY.ClearFaults();
                //Assert.That(multiAxisXY.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set the Start Position scaled by X and Y units.
                axisX.PositionSet(startPosition[0] * Xunits);
                axisY.PositionSet(startPosition[1] * Yunits);
                multiAxisXY.AmpEnableSet(true);

                //Start the Path List, add Path segments, and End the List. 
                multiAxisXY.PathListStart(startPosition);
                multiAxisXY.PathLineAdd(lineA);
                multiAxisXY.PathListEnd();
                multiAxisXY.PathMotionStart();

                //Wait until move is done (corner achieved) before adding more segments
                multiAxisXY.MotionDoneWait();

                //Start the Path List, add Path segments, and End the List
                multiAxisXY.PathListStart(lineA);
                multiAxisXY.PathLineAdd(lineB);
                multiAxisXY.PathLineAdd(lineC);
                multiAxisXY.PathArcAdd(arcD, -180);
                multiAxisXY.PathListEnd();
                multiAxisXY.PathMotionStart();
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
