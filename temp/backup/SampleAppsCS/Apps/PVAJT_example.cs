

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    class PVAJT_example
    {
        const int AXIS_X = 0;
        const int POINTS = 9;
        const int AXES = 1;
        const int IO_NODE = 1;
        const int NUM_MOVES = 3;


        public void Main()
        {
            try
            {
                //Create RapidCode objects
                MotionController controller;
                Axis axisX;
                
                double[] p = new double[POINTS * AXES];
                double[] v = new double[POINTS * AXES];
                double[] a = new double[POINTS * AXES];
                double[] j = new double[POINTS * AXES];
                double[] t = new double[POINTS];

                //Initialize controller object
                controller = MotionController.CreateFromSoftware();

                //Initialize axis objects
                axisX = controller.AxisGet(AXIS_X);
                
                //Clear faults and Enable MultiAxis group
                axisX.Abort();
                axisX.ClearFaults();
                //Assert.That(axisX.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Set Position to 0
                axisX.PositionSet(0);

                axisX.AmpEnableSet(true);
                Console.WriteLine("initialize positions");

                /**********************************************
                 * These arrays are not accurate for including jerk
                 * must have a jerk segment for:
                 * increasing accel
                 * constant accel
                 * decreasing accel
                 * ********************************************/


                //Position, velocity, acceleration, jerk, and time points 
                p[0] = 0000; v[0] = 0000; a[0] = 000; j[0] = 100; t[0] = .25;
                p[0] = 0000; v[0] = 0150; a[0] = 100; j[0] = 0; t[0] = 5;
                p[0] = 0000; v[0] = 0100; a[0] = 100; j[0] = -100; t[0] = .25;
                p[1] = 50; v[1] = 100; a[1] = 0000; j[1] = 0; t[1] = 3.5;
                p[2] = 400; v[2] = 100; a[2] = -100; j[2] = .9; t[2] = 1;
                
                // Load arrays and Execute PVAJT motion. 
                axisX.MovePVAJT(p, v, a, j, t, POINTS, -1, false, true);
                
                //While loop to wait for completion of PVT motion
                while (p[POINTS - 1] != axisX.CommandPositionGet()) { }
                Console.WriteLine("PVAJT- Motion Complete");

                
                Console.WriteLine("PVAJT Motion Complete");
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}