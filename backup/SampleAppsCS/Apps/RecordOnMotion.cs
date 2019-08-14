/// @example RecordOnMotion.cs   Configure the Recorder to start recording when a motion starts. 
/*  RecorderInterrupts.cs
 
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

 For any questions regarding this sample code please visit www.roboticsys.com.
 ==================================================================================

 This application is designed to demonstrate receiving interrupts from the MotionController
 when the Recorder's buffer is getting filled.
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    public class RecordOnMotion
    {
        const int X = 0;                        // axis number
        const int Y = 1;                        // axis number
        const int RECORDER_PERIOD = 1;          // record every sample
        const int NUM_OF_AXES = 2;                     // axis count

        // RapidCode objects
        MotionController controller;
        Axis x;
        Axis y;
        MultiAxis multiAxisXY;


        public enum RecordingIndexes
        {
            XActual = 0,
            YActual,
        }

        // used for storing 64-bit signed positions
        public class DataRecord
        {
            public double ActualPositionX = 0;
            public double ActualPositionY = 0;
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

        public List<DataRecord> dataRecordList = new List<DataRecord>();



        public void Main()
        {
            try
            {
                controller = MotionController.Create();

                //Set MotionCount if needed.
                int requiredMotionCount = CalculateMotionCountRequired();
                if (controller.MotionCountGet() < requiredMotionCount)
                    controller.MotionCountSet(requiredMotionCount);

                x = controller.AxisGet(X);
                y = controller.AxisGet(Y);
                multiAxisXY = controller.MultiAxisGet(controller.AxisCountGet()); //The first free Motion Supervisor should equal the Axis Count as Motion Supervisors are Zero Ordinate.
                multiAxisXY.AxisAdd(x);
                multiAxisXY.AxisAdd(y);


                controller.RecorderStop(); // kill it, in case it was left recording
                controller.RecorderPeriodSet(RECORDER_PERIOD);
                controller.RecorderDataCountSet( Enum.GetValues(typeof(RecordingIndexes)).Length); // how much data in each record?
                controller.RecorderDataAddressSet((int)RecordingIndexes.XActual, x.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeACTUAL_POSITION));
                controller.RecorderDataAddressSet((int)RecordingIndexes.YActual, y.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeACTUAL_POSITION));
       
                // enable interrupts, so we don't overflow the buffer -- the internal library will read them out.
                controller.InterruptEnableSet(true);


                // configure to start recording when motion starts
                controller.RecorderConfigureToTriggerOnMotion(multiAxisXY, true);

                // command a simple, multi-axis motion
                double[] pos = new double[NUM_OF_AXES]  { 2000, 2000};
                double[] v = new double[NUM_OF_AXES] { 1000, 1000 };
                double[] a = new double[NUM_OF_AXES] { 1000, 1000 };
                multiAxisXY.MoveTrapezoidal(pos, v, a, a);
                
                bool done = false;
                while (!done)
                {
                    ReadRecordedData();

                    if (multiAxisXY.MotionDoneGet() == true)
                    {
                        done = true;
                    }
                }

                // print the List of records
                PrintRecordedData();

                controller.InterruptEnableSet(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ReadRecordedData()
        {
            while (controller.RecorderRecordCountGet() > 0)
            {
                controller.RecorderRecordDataRetrieve();  // read one record from the recorder into MotionController class memory
                
                DataRecord record = new DataRecord();

                record.ActualPositionX = controller.RecorderRecordDataDoubleGet((int)RecordingIndexes.XActual);
                record.ActualPositionY = controller.RecorderRecordDataDoubleGet((int)RecordingIndexes.YActual);

                dataRecordList.Add(record);
            }
        }

        private void PrintRecordedData()
        {
            foreach (DataRecord record in dataRecordList)
            {
                Console.WriteLine("X: " + record.ActualPositionX + " Y: " + record.ActualPositionY.ToString());
            }
        }
    }
}
