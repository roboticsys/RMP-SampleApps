/// @example MotionHold_MovePVT.cs  
/*  
 Copyright(c) 1998-2013 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.
  
 This sample applications demonstrates how to Hold a PVT move. Can also use this
 example for how to hold any type of move like PT, Path, or point to point moves.

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

    class MotionHold_MovePVT
    {
        const int AXIS_X = 0;
        const int CONTROLLER = 0;
        const int NUM_OF_POINTS = 4;
        const int NUM_OF_AXES = 1;
        const double MOTOR_RES_16 = 65536;  //Motor Resolution = counts per 1 motor Rev
        const double REVS_PER_DEG = 0.27777;   //Number of Revs per degree on the X axis

        // RapidCode creation methods don't throw exceptions, they log errors. CreateFromBoard(), AxisGet(), MultiAxisGet(), etc.
        private void CheckCreationErrors(IRapidCodeObject rapidObject)
        {
            RsiError e = null;
            while (rapidObject.ErrorLogCountGet() > 0)
            {
                e = rapidObject.ErrorLogGet();
                Console.WriteLine(e.Message + e.StackTrace); // print all logged errors
            }
            if (e != null)
                throw e;  // this will cause the program to exit
        }
        

        public void Main()
        {
            try
            {
                //Create RapidCode objects
                MotionController controller;
                Axis axisX;
                                
                double[] pos = new double[NUM_OF_POINTS * NUM_OF_AXES];
                double[] t = new double[NUM_OF_POINTS];
                double[] v = new double[NUM_OF_POINTS * NUM_OF_AXES];
                                          
                //Initialize controller object
                controller = MotionController.CreateFromSoftware();
                CheckCreationErrors(controller);
                
                //Initialize axis objects
                axisX = controller.AxisGet(AXIS_X);
                CheckCreationErrors(axisX);
                
                //setup User units
                axisX.UserUnitsSet(MOTOR_RES_16 * REVS_PER_DEG); //degrees
                     
                //Clear faults
                axisX.Abort();
                axisX.ClearFaults();
                //Assert.That(axisX.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));
                
                //Set Position to 0
                axisX.PositionSet(0);

                //Enable the Axis
                axisX.AmpEnableSet(true);

                //Position, velocity, and time points for MovePVT
                pos[0] = 50; v[0] = 100; t[0] = 1;
                pos[1] = 400; v[1] = 100; t[1] = 3.5;
                pos[2] = 450; v[2] = 0; t[2] = 1;
                pos[3] = 450; v[3] = 0; t[3] = 1;

                //Turn ON the HOLD motion attribute, Set the Type to Gate, and Set the Gate to TRUE
                axisX.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);
                axisX.MotionHoldTypeSet(RSIMotionHoldType.RSIMotionHoldTypeGATE);
                axisX.MotionHoldGateSet(true);

                //Load the MovePVT array.  ***Will not command motion until the hold gate is cleared***
                axisX.MovePVT(pos, v, t, NUM_OF_POINTS, -1, false, true);
                Console.WriteLine("MovePVT Array is Loaded. Should not move yet.");
                Console.WriteLine("State is " + axisX.StateGet());
                Console.WriteLine("CommandVelocity: " + axisX.CommandVelocityGet());
                Console.WriteLine("Wait 10 Seconds to proove it is holding....");
                controller.OS.Sleep(10000);        
                Console.WriteLine("State is " + axisX.StateGet());
                Console.WriteLine("CommandVelocity: " + axisX.CommandVelocityGet());
                Console.WriteLine("Now releasing the gate to start PVT Motion...");

                //Turn OFF Motion Attribute; only need Hold attribute on first move command
                axisX.MotionAttributeMaskOffSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);

                // Clear the gate. This allows MovePVT to execute!
                axisX.MotionHoldGateSet(false);

                while (!axisX.MotionDoneGet())
                {
                    controller.OS.Sleep(100);
                    Console.WriteLine("CommandVelocity: " + axisX.CommandVelocityGet());
                }
                Console.WriteLine("PVT Motion Complete");
                Console.WriteLine("State is " + axisX.StateGet());
                axisX.Abort();
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}

