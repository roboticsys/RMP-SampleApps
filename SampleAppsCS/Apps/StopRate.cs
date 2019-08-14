/// @example StopRate.cs   Set the deceleration time for RSIActionSTOP and RSIActionE_STOP.
/* StopRate.cs

 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.
 
 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 This sample code demonstrates how to configure the RSIActionSTOP and
 RSIActionE_STOP deceleration rates for a given Axis.

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
    public class StopRate
    {
        // RapidCode objects
        MotionController controller;             
        Axis axis;
  
        //constants
        const int AXIS_NUMBER	= 0;						
         //Define STOP and ESTOP rates in seconds. 
         //The value entered here will be the new STOP and ESTOP rates (second).
        const double STOP_RATE_DEFAULT = 1.0;  
        const double ESTOP_RATE_DEFAULT  = 0.050; 

        [Test]
        public void Main()
        {
            /* Command line arguments and defaults */
            double stopRate = STOP_RATE_DEFAULT;
            double eStopRate = ESTOP_RATE_DEFAULT;
            
            try
            {
                //Initialize Controller Class
                controller = MotionController.Create();
                
                //Initialize Axis Class
                axis = controller.AxisGet(AXIS_NUMBER);
                
		        //display old STOP and ESTOP rates.
		        Console.WriteLine("OLD: StopRate = " + Math.Round(axis.StopTimeGet(),4) +
                    "\t\teStopRate = " + Math.Round(axis.EStopTimeGet(), 4) + "\n");
            
		        //Replacing new values of STOP and ESTOP rates(seconds). Values set above.
		        axis.StopTimeSet(stopRate);
		        axis.EStopTimeSet(eStopRate);

		        //display new STOP and ESTOP rates.
                Console.WriteLine("NEW: StopRate = " + Math.Round(axis.StopTimeGet(), 4) +
                    "\t\teStopRate = " + Math.Round(axis.EStopTimeGet(), 4) + "\n");           
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

