/// @example AbsoluteMotion.cs   Performs point to point trapezoidal profile motion. 
/* AbsoluteMotion.cs 

 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.
 
 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 This sample application moves a single axis in trapezoidal profile to an absolute 
 distance set by RELATIVE_POSITION below. For a simple trapezoidal motion profile using
 'Relative move' please see motion1_relative.cpp
 
 There is a minimal error checking in this sample.

 For any questions regarding this sample code please visit our documentation at www.roboticsys.com

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.
*/

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using RSI.RapidCode.SynqNet.dotNET;
using RSI.RapidCode.SynqNet.dotNET.Enums;

namespace SampleApplications
{
    [TestFixture]
    public class AbsoluteMotion
    {
        // RapidCode objects
        MotionController controller;
        Axis axis;

        //constants
        const int AXIS_NUMBER = 0;
        const int RELATIVE_POSITION = 20000;
        const int VELOCITY = 5000;
        const int ACCELERATION = 10000;
        const int DECELERATION = 20000;

        [Test]
        public void Main()
        {
            try
            {
                //Initialize Controller Class
                controller = MotionController.Create();

                //Initialize Axis Class
                axis = controller.AxisGet(AXIS_NUMBER);

                //clear faults and enable axis
		        axis.ClearFaults();
		        axis.AmpEnableSet(true);

		        Console.WriteLine("Absolute Move\n\n");
		        Console.WriteLine("Trapezoidal Profile: In Motion...\n");
		        //Command simple trapezoidal motion
		        axis.MoveTrapezoidal(RELATIVE_POSITION, VELOCITY, ACCELERATION, DECELERATION);

		        axis.MotionDoneWait();

		        Console.WriteLine("Trapezoidal Profile: Completed\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

