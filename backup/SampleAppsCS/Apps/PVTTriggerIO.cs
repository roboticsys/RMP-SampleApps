/// @example PVTTriggerIO.cs  
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
  
 This sample applications demonstrates how to trigger IO events during PVT motion. 
 This is usful for users that want to read and write inputs and outputs based on 
 motionID.

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

    class PVTTriggerIO
    {
        const int AXIS_X = 0;
        const int AXIS_Y = 1;
        const int IO_NODE = 2;
        const int OUTPUT_BIT_NUMBER = 0;
        const int NUM_OF_POINTS = 3;
        const int NUM_OF_AXES = 2;
        const int NUM_MOVES = 3;
        const double MOTOR_RES_16 = 65536;  //Motor Resolution = counts per 1 motor Rev
        const double REVS_PER_DEG = 0.0027777;   //Number of Revs per degree on the X axis

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

        // Count required is the number of Axes + Number of MultiAxis Objects
        int CalculateMotionCountRequired()
        {
            //This application needs 1 extra motion supervisor for the MultiAxis Object.
            int requiredSupervisors = -1;

            //Here we have a specific Axis Count
            requiredSupervisors = NUM_OF_AXES;

            requiredSupervisors++; //Add 1 for each MultiAxis required.  1 for this application.

            return requiredSupervisors;
        }


        public void Main()
        {
            try
            {
                //Create RapidCode objects
                MotionController controller = MotionController.CreateFromSoftware();
                CheckCreationErrors(controller);

                //Set MotionCount if needed.
                int requiredMotionCount = CalculateMotionCountRequired();
                if (controller.MotionCountGet() < requiredMotionCount)
                    controller.MotionCountSet(requiredMotionCount);
               
                Axis axisX = controller.AxisGet(AXIS_X);
                CheckCreationErrors(axisX);
                Axis axisY = controller.AxisGet(AXIS_Y);
                CheckCreationErrors(axisY);


                MultiAxis multiAxis = controller.MultiAxisGet(controller.AxisCountGet()); //The first free Motion Supervisor should equal the Axis Count as Motion Supervisors are Zero Ordinate. 
                CheckCreationErrors(multiAxis);
                multiAxis.AxisAdd(axisX);
                multiAxis.AxisAdd(axisY);

                IO sliceio = controller.IOGet(IO_NODE);
                CheckCreationErrors(sliceio);

                IOPoint DigitalOut1 = IOPoint.CreateDigitalOutput(sliceio, OUTPUT_BIT_NUMBER);

                //Initialize PVT Arrays
                double[] pos = new double[NUM_OF_POINTS * NUM_OF_AXES];
                double[] t = new double[NUM_OF_POINTS];
                double[] v = new double[NUM_OF_POINTS * NUM_OF_AXES];

                double[] pos1 = new double[NUM_OF_POINTS * NUM_OF_AXES];
                double[] t1 = new double[NUM_OF_POINTS];
                double[] v1 = new double[NUM_OF_POINTS * NUM_OF_AXES];

                double[] pos2 = new double[NUM_OF_POINTS * NUM_OF_AXES];
                double[] t2 = new double[NUM_OF_POINTS];
                double[] v2 = new double[NUM_OF_POINTS * NUM_OF_AXES];
                                 

                //Clear faults 
                multiAxis.Abort();
                multiAxis.ClearFaults();
                //Assert.That(multiAxis.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Setup User Units 
                axisX.UserUnitsSet(MOTOR_RES_16 * REVS_PER_DEG); //degrees
                axisY.UserUnitsSet(MOTOR_RES_16 * REVS_PER_DEG); //degrees
                
                //Set Position to 0
                axisX.PositionSet(0);
                axisY.PositionSet(0);

                //Enable MultiAxis group
                multiAxis.AmpEnableSet(true);
                
                //Position, velocity, and time points for FIRST MovePVT
                pos[0] = 50; v[0] = 100; t[0] = 1;
                pos[1] = 50; v[1] = 100; t[1] = 0.5;
                pos[2] = 100; v[2] = 100; t[2] = 0.5;
                pos[3] = 100; v[3] = 100;
                pos[4] = 150; v[4] = 100;
                pos[5] = 150; v[5] = 100;
                
                //Position, velocity, and time points for SECOND MovePVT
                pos1[0] = 200; v1[0] = 100; t1[0] = 0.5;
                pos1[1] = 200; v1[1] = 100; t1[1] = 0.5;
                pos1[2] = 250; v1[2] = 100; t1[2] = 0.5;
                pos1[3] = 250; v1[3] = 100;
                pos1[4] = 300; v1[4] = 100;
                pos1[5] = 300; v1[5] = 100;

                //Position, velocity, and time points for THIRD MovePVT
                pos2[0] = 350; v2[0] = 100; t2[0] = 0.5;
                pos2[1] = 350; v2[1] = 100; t2[1] = 0.5;
                pos2[2] = 400; v2[2] = 100; t2[2] = 1;
                pos2[3] = 400; v2[3] = 100;
                pos2[4] = 450; v2[4] = 0;
                pos2[5] = 450; v2[5] = 0;

               
                //Turn ON the HOLD motion attribute, Set the Type to Gate, and Set the Gate to TRUE
                multiAxis.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);
                multiAxis.MotionHoldTypeSet(RSIMotionHoldType.RSIMotionHoldTypeGATE);
                multiAxis.MotionHoldGateSet(true);
               

                // Load FIRST segment of PVT motion. 
                // Assign this segment a motion ID
                multiAxis.MotionIdSet(1000);
                multiAxis.MovePVT(pos, v, t, NUM_OF_POINTS, -1, false, false);

                /**** You MUST Turn OFF Motion Attribute after first motion command if loading more than one. ****/
                multiAxis.MotionAttributeMaskOffSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);

                // Load Second segment of PVT motion. 
                // Assign this segment a motion ID
                multiAxis.MotionIdSet(1001);
                multiAxis.MovePVT(pos1, v1, t1, NUM_OF_POINTS, -1, false, false);

                // Load Second segment of PVT motion. 
                // Assign this segment a motion ID
                multiAxis.MotionIdSet(1002);
                multiAxis.MovePVT(pos2, v2, t2, NUM_OF_POINTS, -1, false, true);
                
                // Clear the gate. This allows MovePVT to execute!       
                Console.WriteLine("PVT Motion Begins");
                multiAxis.MotionHoldGateSet(false);
                
                int OldMotionID = 0;
                int i;
                for(i= 0; i<NUM_MOVES; i++)
                {
                    //Wait here until motion ID changes
                    while (OldMotionID == multiAxis.MotionIdExecutingGet()) { }; 

                    Console.WriteLine("MotionID{0} = {1}", i, multiAxis.MotionIdExecutingGet());
                    switch (multiAxis.MotionIdExecutingGet())
                    {
                        case 1000:
                            DigitalOut1.Set(true);
                            Console.WriteLine("Digital Out High");
                            OldMotionID = 1000;
                            break;
                        case 1001:
                            DigitalOut1.Set(false);
                            Console.WriteLine("Digital Out Low");
                            OldMotionID = 1001;
                            break;
                        case 1002:
                            DigitalOut1.Set(true);
                            Console.WriteLine("Digital Out High");
                            OldMotionID = 1002;
                            break;
                        default:
                            DigitalOut1.Set(false);
                            Console.WriteLine("Digital Out Low");
                            break;
                    }    
                }

                //While loop to wait for completion of PVT motion
                multiAxis.MotionDoneWait();

                multiAxis.Abort();
                multiAxis.Unmap();

                DigitalOut1.Set(false);
                Console.WriteLine("Motion done: Disable output");
                Console.WriteLine("PVT Motion Complete");
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}