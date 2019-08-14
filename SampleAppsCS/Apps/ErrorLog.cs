/// @example ErrorLog.cs  
/*  
 Copyright(c) 1998-2013 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.
  
 This sample application demonstrates how to check errors on creation of 
 RapidCode Objects and catch exceptions througout a program.

 For any questions regarding this sample code please visit www.roboticsys.com.
 ==================================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using RSI.RapidCode.SynqNet.dotNET;
using RSI.RapidCode.SynqNet.dotNET.Enums;

namespace SampleApplications
{
    [TestFixture]
    class ErrorLog
    {
        const int AXIS_X = 0;
        const int AXIS_Y = 1;
        const int MULTIAXIS_XY = 2;
        const int CONTROLLER = 0;

        // RapidCode creation methods don't throw exceptions, they log errors. CreateFromBoard(), AxisGet(), MultiAxisGet(), etc.
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
            //Use a Try/Catch Statement to catch and print RapidCode exceptions
            try  
            {
                //Create RapidCode objects
                MotionController controller;
                Axis axisX;
                Axis axisY;
                MultiAxis multiAxisXY;
                
                //Initialize controller object
                controller = MotionController.CreateFromBoard(CONTROLLER);
                CheckCreationErrors(controller);

                //Initialize axis objects
                axisX = controller.AxisGet(AXIS_X);
                CheckCreationErrors(axisX);
                axisY = controller.AxisGet(AXIS_Y);
                CheckCreationErrors(axisY);
                
                //Initialize MultiAxis object
                multiAxisXY = controller.MultiAxisGet(MULTIAXIS_XY);
                CheckCreationErrors(multiAxisXY);
                multiAxisXY.AxisAdd(axisX);
                multiAxisXY.AxisAdd(axisY);
                
  
                /*********************************************************************
                 * 
                 * INSERT YOUR APPLICATION CODE HERE
                 * 
                 * 
                 * 
                 * Sample of using Assert.That within your application: 
                 *   axisX.ClearFaults();
                 *   Assert.That(axisX.StateGet(), Is.EqualTo(RSIState.RSIStateIDLE));
                 *   axisX.AmpEnableSet(true);
                 * ********************************************************************/
                                
                
            }

            catch (RsiError err)
            {
                Console.WriteLine(err.text);
            }
        }
    }
}