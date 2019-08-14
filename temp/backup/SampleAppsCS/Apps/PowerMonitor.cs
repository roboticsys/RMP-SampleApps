/// @example PowerMonitor.cs  
/*  
 Copyright(c) 1998-2015 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.
  
 This sample application demonstrates how to estimate the power usage of the 
 servo axes.

 For any questions regarding this sample code please visit www.roboticsys.com.
 ==================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{
    [TestFixture]
    class PowerMonitor
    {
        const int CONTROLLER = 0;
        const int AXIS_X = 0;
        const double DRIVE_EFF = 1.1;  //Power efficency of drive
        const double MOTOR_RES_16 = 65536;  //Motor Resolution = counts per 1 motor Rev

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

        [Test]
        public void Main()
        {
            try
            {
                //Create RapidCode objects
                MotionController controller = MotionController.CreateFromBoard(CONTROLLER);
                CheckCreationErrors(controller);
                
                Axis axisX = controller.AxisGet(AXIS_X);
                CheckCreationErrors(axisX);

                axisX.Abort();
                axisX.ClearFaults();
                Assert.That(axisX.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));

                //Setup User Units 
                axisX.UserUnitsSet(MOTOR_RES_16); //Motor Revs
                
                //Set Position to 0
                axisX.PositionSet(0);
               
                //Enable and command motion
                axisX.AmpEnableSet(true);
                axisX.MoveTrapezoidal(10, 10, 100, 100);

                while (axisX.ActualPositionGet() < 1) { }
                double Icmd = axisX.DriveParamGet(RSIKollmorgenAKDParam.RSIKollmorgenAKDParamIL_CMD);  
                double Vbus = axisX.DriveParamGet(RSIKollmorgenAKDParam.RSIKollmorgenAKDParamVBUS_VALUE); 
                Icmd = Icmd / 1000; //convert mA to A
                Vbus = Vbus / 1000; //convert mV to V

                double DCPower = Icmd * Vbus;
                double ACPower = DCPower * DRIVE_EFF;

                Console.WriteLine("AxisX Current Command (Amps) = " + Icmd);
                Console.WriteLine("AxisX Bus voltage (Volts) = " + Vbus);
                Console.WriteLine("AxisX DC Power (Watts) = " + DCPower);
                Console.WriteLine("AxisX AC Power (Watts) = " + ACPower);

                axisX.MotionDoneWait();

                axisX.Abort();
                
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}