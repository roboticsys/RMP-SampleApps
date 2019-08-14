/// @example RecorderInterrupts.cs   Demonstrates receiving interrupts from the MotionController when the Recorder's buffer is getting filled. 
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

    public class RecorderInterrupts
    {
        const int X = 0;                        // axis number
        const int Y = 1;                        // axis number
        const int RECORDER_PERIOD = 1000;       //samples
        const int INTERRUPT_TIMEOUT = 2000;     // milliseconds -- a little longer than recorder period

        // RapidCode objects
        MotionController controller;
        Axis x;
        Axis y;


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


        public void Main()
        {
            try
            {
                controller = MotionController.Create();
                x = controller.AxisGet(X);
                y = controller.AxisGet(Y);

                controller.RecorderStop(); // kill it, in case it was left recording
                controller.RecorderPeriodSet(RECORDER_PERIOD);
                controller.RecorderDataCountSet( Enum.GetValues(typeof(RecordingIndexes)).Length); // how much data in each record?
                controller.RecorderDataAddressSet((int)RecordingIndexes.XActual, x.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeACTUAL_POSITION));
                controller.RecorderDataAddressSet((int)RecordingIndexes.YActual, y.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeACTUAL_POSITION));
                controller.RecorderBufferHighCountSet(1);

                // enable interrupts
                controller.InterruptEnableSet(true);

                // start recording
                controller.RecorderStart();

                bool done = false;
                while (!done)
                {
                    RSIEventType interruptType = controller.InterruptWait(INTERRUPT_TIMEOUT); // make sure timeout value is greater than recording period
      
                    switch (interruptType)
                    {
                        case RSIEventType.RSIEventTypeTIMEOUT:
                            done = true;
                            break;

                        case RSIEventType.RSIEventTypeRECORDER_HIGH:
                            ReadRecordedData();
                            break;

                        default:
                            // print the details of any other interrupts
                            Console.WriteLine(controller.InterruptNameGet());
                            break;
                    }
                }

                controller.RecorderStop();
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

                Console.WriteLine("X: " + record.ActualPositionX + " Y: " + record.ActualPositionY.ToString());

            }
        }
           
    }
}
