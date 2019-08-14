


using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    class PointToPointMotion_SCurve
    {
        const int AXIS_X = 0;
        const int AXIS_Y = 1;
        const int MOTOR_RES_16 = 65536;  //Motor Resolution = counts per 1 motor Rev
        const int REVS_PER_INCH_X = 1;   //Number of Revs per inch on the X axis
        const int REVS_PER_INCH_Y = 1;   //Number of Revs per inch on the Y axis

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
                double[] startPosition = new double[2] { 0, 0 };
                double[] point1 = new double[2] { 20, 50 };
                double[] point2 = new double[2] { 60, 20 };
                double[] point3 = new double[2] { 0, 0 };

                double[] velocity = new double[2] { 2, 5 };
                double[] accel = new double[2] { 100, 100 };
                double[] jerk = new double[2] { 0, 0 };
                

                double Xunits = MOTOR_RES_16 * REVS_PER_INCH_X;
                double Yunits = MOTOR_RES_16 * REVS_PER_INCH_Y;

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
                axisX.UserUnitsSet(Xunits); 
                axisY.UserUnitsSet(Yunits);

                
                //Abort, Clear Faults and check for IDLE State
                multiAxisXY.Abort();
                multiAxisXY.ClearFaults();
                //Assert.That(multiAxisXY.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set the Start Position.
                axisX.PositionSet(startPosition[0]);
                axisY.PositionSet(startPosition[1]);
                multiAxisXY.AmpEnableSet(true);
                controller.OS.Sleep(10);
                
                Console.WriteLine("Starting Motion");
                //Start the
                multiAxisXY.MoveSCurve(point1, velocity, accel, accel, jerk);
                multiAxisXY.MotionDoneWait();
                //multiAxisXY.MoveSCurve(point2, velocity, accel, accel, jerk);
                //multiAxisXY.MotionDoneWait();
                //multiAxisXY.MoveSCurve(point3, velocity, accel, accel, jerk);
                //multiAxisXY.MotionDoneWait();

                
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
