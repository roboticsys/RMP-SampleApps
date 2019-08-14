/// @example Slice_QuadEncoderInput.cs   Setup a TSIO-9001/9002 for Quadrature Encoder Input
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

 For any questions regarding this sample code please visit www.roboticsys.com.
 ==================================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using RSI.RapidCode.SynqNet.dotNET;
using RSI.RapidCode.SynqNet.dotNET.Enums;


namespace SampleApplications
{
    [TestFixture]
    class Slice_QuadEncoderInput
    {
        const int SLICE_NODE = 1;               // node number as discovered on SynqNet network
        const int SEGMENT_NUMBER_9001 = 0;      // segment (slice) number as found on Slice I/O 
        const int PARAMTER_NUMBER = 0;          // parameter to set the counter mode
        const string PARAMETER_VALUE = "3";     // counter mode 3 = Encoder x1, 4 = Encoder x2, 5 = Encoder x4 
        const double USER_UNITS = 65536;        //Counts per Rev
        const int THRESH1 = 2000;      //1 rev of the Aux encoder
        const int THRESH2 = 4000;      //2 revs
        const int THRESH3 = 6000;      //3 revs

        
        // RapidCode objects
        MotionController controller;
        Axis axis;
        IO slice;
        IOPoint clamp;

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

        private void MovePrimaryAxis(int vel)
        {
            axis.Abort();
            axis.ClearFaults();
            axis.PositionSet(0);
            axis.UserUnitsSet(USER_UNITS);
            Assert.That(axis.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));
            axis.AmpEnableSet(true);
            axis.MoveVelocity(vel, vel * 10);
        }

        private void ConfigureSlice()
        {
            //Configure as Encoder Input mode
            slice.SegmentParameterSet(SEGMENT_NUMBER_9001, PARAMTER_NUMBER, 1, PARAMETER_VALUE);
        }

        private int ReadPosition()
        {
            int countValue = 0;
            countValue = slice.SegmentDigitalInGet(SEGMENT_NUMBER_9001, 0, 24);
            return countValue;
        }

        private void ClearAuxFBPosition()
        {
            int Clearbit = 14;
            slice.SegmentDigitalOutSet(SEGMENT_NUMBER_9001, Clearbit, true);
            controller.OS.Sleep(100);
            int testPos = 0;
            testPos = ReadPosition();
            Assert.That(testPos, Is.EqualTo(0));
            slice.SegmentDigitalOutSet(SEGMENT_NUMBER_9001, Clearbit, false);
            controller.OS.Sleep(100);
        }

        private bool CompareAuxFB()
        {
            double primaryPos = axis.ActualPositionGet();
            int auxPos = ReadPosition();

            Console.WriteLine(" Aux: " + auxPos + "  Pri: " + primaryPos);
            if (auxPos >= THRESH1 && auxPos <= THRESH2)
            {
                clamp.Set(true);
            }
            else
            {
                clamp.Set(false);
            }
            if (auxPos >= THRESH3)
            {
                Console.WriteLine("Aux Feedback has reached THRESH3");
                return false;
            }
            return true;
        }

        //private void SetRandomAuxPosition()
        //{
        //    int tempCounter = RandomInt(0, 5000);
        //    int start_byte = 16;
        //    int count_bytes = 3;
        //    char[] initialCounterValue = new char[3];

        //    initialCounterValue[0] = (char)(tempCounter & 0x000000FF);
        //    initialCounterValue[1] = (char)((tempCounter >> 8) & 0x000000FF);
        //    initialCounterValue[2] = (char)((tempCounter >> 16) & 0x000000FF);

        //    string s = new string(initialCounterValue);

        //    slice.SegmentMemorySet(SEGMENT_NUMBER_9001, start_byte, count_bytes, s);

        //    int LatchPostionBit = 13;
        //    slice.SegmentDigitalOutSet(SEGMENT_NUMBER_9001, LatchPostionBit, true);
        //    controller.OS.Sleep(100);
        //    int testPos = 0;
        //    testPos = ReadPosition();
        //    Assert.That(testPos, Is.EqualTo(tempCounter));
        //    slice.SegmentDigitalOutSet(SEGMENT_NUMBER_9001, LatchPostionBit, false);
        //    controller.OS.Sleep(100);
        //}

        [Test]
        public void Main()
        {
            try
            {
                //Initialize RapidCode Objects
                controller = MotionController.Create();
                CheckCreationErrors(controller);
                axis = controller.AxisGet(0);
                CheckCreationErrors(axis);
                slice = controller.IOGet(SLICE_NODE);
                CheckCreationErrors(slice);
                clamp = IOPoint.CreateDigitalOutput(slice, 16); //initialize output bit 16 as clamp
                clamp.Set(false);
                Assert.That(controller.SynqNetStateGet(), Is.EqualTo(RSISynqNetState.RSISynqNetStateSYNQ));
                
                // setup as Encoder input on the TSIO-9001/2
                ConfigureSlice();

                // Clear the position back to 0
                ClearAuxFBPosition();
                
                //move primary axis at 10 userunits/sec
                MovePrimaryAxis(10); 
               
                bool run = true;
                while (run)
                {
                    run = CompareAuxFB();
                    controller.OS.Sleep(1000); //sleep for 1000ms
                }
                axis.Abort();                                             
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
