/// @example InitializationXML.cs Configure and get an Axis object from an XML settings file
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

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    public class InitializationXML
    {
        const int AXIS_NUMBER = 0;

        private bool CheckErrors(IRapidCodeObject rapidObject)
        {
            bool errors = false;

            while (rapidObject.ErrorLogCountGet() > 0)
            {
                errors = true;
                Console.WriteLine(rapidObject.ErrorLogGet().Message);
            }
            return errors;
        }


        public void Main()
        {
            //RapidCode create methods NEVER throw exceptions but they will log errors
            MotionController controller = MotionController.CreateFromSoftware();
            CheckErrors(controller);

            RapidCodeSystem synqnet = RapidCodeSystem.CreateRapidCodeSystemFromMotionController(controller);

            // get an Axis object that is configured using the specified XML file
            Axis axisX = synqnet.DownloadConfigurationFromXML(AXIS_NUMBER, @"c:\xml\AxisX.xml");
            CheckErrors(axisX);

        }
    }
}
*/