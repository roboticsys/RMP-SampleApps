/// @example PhantomAxis.cs   Setup a Phantom Axis
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

    public class PhantomAxis
    {
        MotionController controller;             // RapidCode objects
        Axis phantomAxis;
        
        private void SetupPhantom(Axis phantom)
        {
            //Set all actions for phantom axis to NONE
            phantom.ErrorLimitActionSet(RSIAction.RSIActionNONE);
            phantom.HardwareNegLimitActionSet(RSIAction.RSIActionNONE);
            phantom.HardwarePosLimitActionSet(RSIAction.RSIActionNONE);
            phantom.HomeActionSet(RSIAction.RSIActionNONE);
            phantom.SoftwareNegLimitActionSet(RSIAction.RSIActionNONE);
            phantom.SoftwarePosLimitActionSet(RSIAction.RSIActionNONE);

            //Setup an axis to be PHANTOM (virtual) -- this configures the actual position equal command position
            phantomAxis.MotorTypeSet(RSIMotorType.RSIMotorTypePHANTOM);
        }


        public void PhantomMain()
        {
            try
            {
                int axisNumber;

                //Create Controller Object
                controller = MotionController.Create();
                             
                // you might need to increase the MotionController's AxisCount so extra Axes are available
                // if so, you'll need controller.AxisCountSet(...)

                // get the last Axis number
                axisNumber = controller.AxisCountGet() - 1;
                phantomAxis = controller.AxisGet(axisNumber);

                //Disable all limits and actions, set motor type
                SetupPhantom(phantomAxis);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
