/// @example MotionFinalVelocity.cs   Perform a point-to-point motion which finishes at a non-zero velocity. 
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
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    public class MotionFinalVelocity
    {
        const int X = 1;                        // axis number
        const double maxVelocity = 100.0;
        const double accel = 1000.0;
        const double finalVelocity = 10.0;
        const double jerkPercent = 10.0;
        const double relativeIncrement = 200.0;



        public void Main()
        {
            try
            {
                //RapidCode Objects
                MotionController controller = MotionController.Create();
                Axis x = controller.AxisGet(X);
          

                x.ClearFaults();
                x.AmpEnableSet(true);

                // performa a relative motion, but finish with a final (non-zero) velocity
                x.MoveRelative(relativeIncrement, maxVelocity, accel, accel, jerkPercent, finalVelocity);


                // AT_VELOCITY is set when the motion is "complete" and moving at final velocity
                while (!(x.StatusBitGet(RSIEventType.RSIEventTypeMOTION_AT_VELOCITY)))
                {
                    Console.WriteLine("Pos: " + x.CommandPositionGet() + " Vel: " + x.CommandVelocityGet());

                    if (x.StateGet() == RSIState.RSIStateERROR)
                    {
                        break;
                    }

                    controller.OS.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
