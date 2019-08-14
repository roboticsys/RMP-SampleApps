/* @example SequenceAbort.cs   Configure a sequencer to abort other axes. */
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

 For any questions regarding this sample code please visit www.roboticsys.com.
 ==================================================================================

 * 
 * This sample will configure a sequencer to abort one axis based of another axis.
 * 
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

//namespace SampleApplications
//{
//    [TestFixture]
//    public class SequencerAbort
//    {
//        // RapidCode objects
//        MotionController mc;
//        Axis firstAxis;
//        Axis secondAxis;

//        // constants        
//        const int FIRST_AXIS = 5;  // axis number
//        const int SECOND_AXIS = 6;  // axis number
//        const int ENABLED_SEQUENCERS = 1;
//        const int SEQUENCER_NUMBER = 0;
//        const int AMP_DISABLED_BITMASK = 0x0;

//        const string mapFile = "c:\\synqnet\\stdmei.map";

//        // stuff we'll need
//        int firstAxisAmpEnabledOutputAddress;

//        [SetUp]
//        public void Setup()
//        {
//            mc = MotionController.Create();
//            firstAxis = mc.AxisGet(FIRST_AXIS);
//            secondAxis = mc.AxisGet(SECOND_AXIS);

//            firstAxis.AmpEnableSet(true);
//            secondAxis.AmpEnableSet(true);

//            mc.SequencerCountSet(ENABLED_SEQUENCERS);
//        }


//        [Test]
//        public void SequencerOn()
//        {
//            // Enabling sequencer
//            mc.SequencerEnableSet(SEQUENCER_NUMBER, true);

//            firstAxisAmpEnabledOutputAddress = mc.AddressFromStringGet("Motor[" + FIRST_AXIS.ToString() + "].IO.DedicatedOut.IO", mapFile);

//            mc.CommandWaitLong(SEQUENCER_NUMBER,RSICommandOperator.RSICommandOperatorEQUAL, firstAxisAmpEnabledOutputAddress, AMP_DISABLED_BITMASK);

//            mc.CommandAction(SEQUENCER_NUMBER, secondAxis,RSICommandMotion.RSICommandMotionABORT);

//            mc.SequencerStart(SEQUENCER_NUMBER);
//        }

//        [Test]
//        public void SequencerOff()
//        {
//            mc.SequencerStop(SEQUENCER_NUMBER);

//            // Disabling sequencer
//            mc.SequencerEnableSet(SEQUENCER_NUMBER, false);

//        }
//    }
//}
