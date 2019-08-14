/// @example Gear.cs Configure an axis' commanded position to be geared off of another axis 
/*  
 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.
 
 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.
 
 Axis gearing on the XMP is based off of a Axis, RSIAxisMasterType, numerator, and denominator.
 The Axis points to a master axis to gear to. The RSIAxisMasterType specifies what feedback source 
 to gear to. The ratio between the lead and follower axes is set by a ratio of two longs 
 -- a numerator and a denominator. 
 For example:

 Ratio   Numerator   Denominator
   1        1           1
   2        2           1
   0.5      1           2
   0.1      1           10
   10       10          1

 Function used is: 
 GearingEnable(Axis masterAxis, RSIAxisMasterType gearingSource, long numerator, long denominator);
 User has several options for gearingSource declared above:
  
 RSIAxisMasterType.RSIAxisMasterTypeADDRESS;
 RSIAxisMasterType.RSIAxisMasterTypeAXIS_ACTUAL_POSITION;     
 RSIAxisMasterType.RSIAxisMasterTypeAXIS_COMMANDED_POSITION  
 RSIAxisMasterType.RSIAxisMasterTypeMOTOR_FEEDBACK_PRIMARY;
 RSIAxisMasterType.RSIAxisMasterTypeMOTOR_FEEDBACK_SECONDARY
  
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
using NUnit.Framework.SyntaxHelpers;
using RSI.RapidCode.SynqNet.dotNET;
using RSI.RapidCode.SynqNet.dotNET.Enums;

namespace SampleApplications
{
    [TestFixture]
    public class Gear
    {
        // RapidCode objects
        MotionController controller;             
        Axis axis0;
        Axis axis1;
        

        [Test]
        public void Main()
        {
            try
            {
                //RapidCode Objects
                controller = MotionController.Create();
                axis0 = controller.AxisGet(0);
                axis1 = controller.AxisGet(1);
                                    
                //constants
                const double VELOCITY = 1000.0;
                const double ACCELERATION = 10000.0;
                const double DECELERATION = 10000.0;
                const double JERK_PERCENT = 100.0;

                // zero the positions (in case the program is run multiple times)
                axis0.PositionSet(0.0); 
                axis1.PositionSet(0.0);
        		
                //Clear Faults and Enable Amplifiers
                axis0.ClearFaults();
                axis1.ClearFaults();

                axis0.AmpEnableSet(true);
                axis1.AmpEnableSet(true);
        		
                //Perform Gearing
                Console.WriteLine("\nTesting for Gearing\n");
                
                //Configure X1 axis to be a slave to X0 axis (-1:1 ratio)
                axis1.GearingEnable(axis0, RSIAxisMasterType.RSIAxisMasterTypeAXIS_COMMANDED_POSITION, -1, 1);

                // perform a S-curve motion on master axis
                axis0.MoveSCurve(2500, VELOCITY, ACCELERATION, DECELERATION, JERK_PERCENT);
                axis0.MotionDoneWait();

                Console.WriteLine("\nTesting for GearingRatioChange\n");
                        		
                //Change the slave's gear ratio   (-2:1 ratio)
                axis1.GearingRatioChange(-2, 1);
        		
                //Move the master again
                axis0.MoveSCurve(4000, VELOCITY, ACCELERATION, DECELERATION, JERK_PERCENT);
                axis0.MotionDoneWait();
              

                //disable Gearing on slave
                axis1.GearingDisable();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
