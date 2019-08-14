/// @example SynchronizedStart.cs  
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
  
 This sample application demonstrates how to start multiple different types of 
 motion  at the exact same time by releasing a single hold gate. This is usful for 
 users that want make sure multiple axes or multiaxes all start at the same time.

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

    class SynchronizedStart
    {
        const int CONTROLLER = 0;
        const int NUM_OF_POINTS = 6;
        const int NUM_OF_POINTS1 = 8;
        const int NUM_OF_AXES = 1;
        const int GATENUMBER = 13;

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
                Axis axisY;
                Axis axisZ;
                                                
                double[] pos = new double[NUM_OF_POINTS * NUM_OF_AXES];
                double[] t = new double[NUM_OF_POINTS];
                
                double[] pos1 = new double[NUM_OF_POINTS1 * NUM_OF_AXES];
                double[] t1 = new double[NUM_OF_POINTS1];
                double[] v1 = new double[NUM_OF_POINTS1 * NUM_OF_AXES];
                                          
                //Initialize controller object
                controller = MotionController.CreateFromSoftware();
                CheckCreationErrors(controller);
                
                //Initialize axis objects
                axisX = controller.AxisGet(0);
                CheckCreationErrors(axisX);
                axisY = controller.AxisGet(1);
                CheckCreationErrors(axisY);
                axisZ = controller.AxisGet(2);
                CheckCreationErrors(axisZ);
                
                //Clear faults.
                axisX.Abort();
                axisY.Abort();
                axisZ.Abort();
                axisX.ClearFaults();
                axisY.ClearFaults();
                axisZ.ClearFaults();
                
                //Assert.That(axisX.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));
                //Assert.That(axisY.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));
                //Assert.That(axisZ.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));
                
                //Set Position to 0
                axisX.PositionSet(0);
                axisY.PositionSet(0);
                axisZ.PositionSet(0);
                
                axisX.AmpEnableSet(true);
                axisY.AmpEnableSet(true);
                axisZ.AmpEnableSet(true);

                axisX.UserUnitsSet(1);
                axisY.UserUnitsSet(1);
                axisZ.UserUnitsSet(1);

                //Position, and time points for FIRST MovePT
                pos[0] = 50;  t[0] = 1;
                pos[1] = 100;  t[1] = .5;
                pos[2] = 150;  t[2] = .5;
                pos[3] = 200;  t[3] = .5;
                pos[4] = 250; t[4] = .5;
                pos[5] = 300; t[5] = 1;

                //Position, velocity, and time points for Second MovePVT
                pos1[0] = 05; v1[0] = 10; t1[0] = 1;
                pos1[1] = 10; v1[1] = 10; t1[1] = .5;
                pos1[2] = 15; v1[2] = 10; t1[2] = .5;
                pos1[3] = 20; v1[3] = 00; t1[3] = 1;
                pos1[4] = 25; v1[4] = 10; t1[4] = 1;
                pos1[5] = 30; v1[5] = 10; t1[5] = .5;
                pos1[6] = 35; v1[6] = 10; t1[6] = .5;
                pos1[7] = 40; v1[7] = 00; t1[7] = 1;

                //Set controller's HOLD Gate to TRUE to prevent motion from exectuing
                controller.MotionHoldGateSet(GATENUMBER, true); 

                //Set the hold type to gate; attribute to hold; and assign the axis or multiaxis to the controller's hold gate number.
                axisX.MotionHoldTypeSet(RSIMotionHoldType.RSIMotionHoldTypeGATE);
                axisX.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);
                axisX.MotionHoldGateNumberSet(GATENUMBER);
                axisY.MotionHoldTypeSet(RSIMotionHoldType.RSIMotionHoldTypeGATE);
                axisY.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);
                axisY.MotionHoldGateNumberSet(GATENUMBER);
                axisZ.MotionHoldTypeSet(RSIMotionHoldType.RSIMotionHoldTypeGATE);
                axisZ.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);
                axisZ.MotionHoldGateNumberSet(GATENUMBER);


                //load motion commands
                axisX.MovePT(RSIMotionType.RSIMotionTypeBSPLINE, pos, t, NUM_OF_POINTS, -1, false, true);
                axisY.MovePVT(pos1, v1, t1, NUM_OF_POINTS1, -1, false, true);
                axisZ.MoveTrapezoidal(40, 10, 20, 20);
                Console.WriteLine("motion commands are loaded but should not move yet.");
                
                //axis state may return moving, but command velocity will remain at zero until the hold gate is released.
                Console.WriteLine("XState: " + axisX.StateGet() + " YState: " + axisY.StateGet() + " ZState: " + axisZ.StateGet());
                Console.WriteLine("XCmdVel: " + axisX.CommandVelocityGet() + " YCmdVel: " + axisY.CommandVelocityGet() + " ZCmdVel: " + axisZ.CommandVelocityGet());
                Console.WriteLine("Wait 10 seconds to proove that motion is being held.");
                controller.OS.Sleep(10000);
                Console.WriteLine("XCmdVel: " + axisX.CommandVelocityGet() + " YCmdVel: " + axisY.CommandVelocityGet() + " ZCmdVel: " + axisZ.CommandVelocityGet());
                Console.WriteLine("Now releasing the gate to start PVT Motion...");

                //Release all axes or multiaxis objects assigned to the controller's GATENUMBER to execute motion
                controller.MotionHoldGateSet(GATENUMBER, false);

                while (!axisX.MotionDoneGet() | !axisY.MotionDoneGet() | !axisZ.MotionDoneGet())
                {
                    Console.WriteLine("XCmdVel: " + axisX.CommandVelocityGet() + " YCmdVel: " + axisY.CommandVelocityGet() + " ZCmdVel: " + axisZ.CommandVelocityGet());
                    controller.OS.Sleep(500);
                }
                Console.WriteLine("ALL Motion is Complete");

                axisX.Abort();
                axisY.Abort();
                axisZ.Abort();
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}

