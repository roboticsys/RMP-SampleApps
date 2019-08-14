/// @example MotionHoldMultiAxisHomeSwitch.cs   Hold execution of a MultiAxis motion until an Axis' Home Input changes state. 
/*  MotionHoldMultiAxisHomeSwitch.cs
 
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

    public class MotionHoldMultiAxisHomeSwitch
    {
        const int X = 0;                        // axis number
        const int Y = 1;                        // axis number
        const int Z = 4;                        // axis number

        // Count required is the number of Axes + Number of MultiAxis Objects
        int CalculateMotionCountRequired()
        {
            //This application needs 1 extra motion supervisor for the MultiAxis Object.
            int requiredSupervisors = -1;

            //Without a specific Axis Count, we look for the highest defined Axis Number.  Generally you should just know the number of Axes.
            requiredSupervisors = Math.Max(requiredSupervisors, X);
            requiredSupervisors = Math.Max(requiredSupervisors, Y);
            requiredSupervisors = Math.Max(requiredSupervisors, Z);
            requiredSupervisors++; //Axis Numbers are Zero Ordinate.  You'll always want 1 more.  4+1 = 5 Axes.

            requiredSupervisors++; //Add 1 for each MultiAxis required.  1 for this application.

            return requiredSupervisors;
        }


        public void Main()
        {
            try
            {
                UInt64 address;
                int mask = 0;
                int pattern = 0;

                // some random trajectory parameters
                double[] end_position = { 35000, 41000 };
                double[] velocity = { 25000, 41000 };
                double[] accel = { 10000, 10000 };
                double[] jerkPct = { 50, 50 };

                //RapidCode Objects
                MotionController controller = MotionController.Create();



                //Set MotionCount if needed.
                int requiredMotionCount = CalculateMotionCountRequired();
                if (controller.MotionCountGet() < requiredMotionCount)
                    controller.MotionCountSet(requiredMotionCount);

                Axis x = controller.AxisGet(X);
                Axis y = controller.AxisGet(Y);
                Axis z = controller.AxisGet(Z);

                MultiAxis xy = controller.MultiAxisGet(controller.AxisCountGet()); //The first free Motion Supervisor should equal the Axis Count as Motion Supervisors are Zero Ordinate.
                xy.AxisAdd(x);
                xy.AxisAdd(y);

                // Prepare axes
                xy.Abort();
                xy.ClearFaults();
                xy.AmpEnableSet(true);

                // define what type of Motion Hold to use
                xy.MotionHoldTypeSet(RSIMotionHoldType.RSIMotionHoldTypeCUSTOM);

                // find the host address for an Axis' dedicated inputs
                address = z.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeDEDICATED_INPUTS);
                xy.MotionHoldUserAddressSet(address);

                // create the bitmask, by using the HOME input bit number
                mask = 1 << (int)RSIMotorDedicatedIn.RSIMotorDedicatedInHOME;
                xy.MotionHoldUserMaskSet(mask);

                // since we are waiting for the bit to go HIGH, the pattern and mask are equal
                pattern = mask;
                xy.MotionHoldUserPatternSet(pattern);

                // Turn ON Motion Attribute
                xy.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);

                xy.MoveRelative(end_position, velocity, accel, accel, jerkPct);
                xy.MotionDoneWait();

                // Turn OFF Motion Attribute
                xy.MotionAttributeMaskOffSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
