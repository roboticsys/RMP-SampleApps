/// @example IOPoint.cs  RapidCode API Sample Application\n
/// Explore the IOPoint interface
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

    public class IOPointApp
    {
        const int IO_NODE_NUMBER = 0;
        const int SPINDLE_OUTPUT = 0;                        // bit number
        const int COOLANT_INPUT = 1;                        // bit number
        
        public void Main()
        {
            try
            {
                //RapidCode Objects
                MotionController controller = MotionController.Create();
                IO io = controller.IOGet(IO_NODE_NUMBER);
                IOPoint spindle = IOPoint.CreateDigitalOutput(io, SPINDLE_OUTPUT);
                IOPoint coolant = IOPoint.CreateDigitalInput(io, COOLANT_INPUT);

                // turn on Spindle
                spindle.Set(true);

                // verify the Spindle is on
                //Assert.That(spindle.Get(), Is.True, "Spindle should be on!");

                // see if Coolant is on 
                Console.WriteLine("Coolant is " + coolant.Get().ToString());

                // verifty that Spindle is an output
                //Assert.That(spindle.IsOutput(), Is.True, "Spindle should be an Output!");

                // verify that Coolant is Digital
                //Assert.That(coolant.IsDigital(), Is.True, "Coolant should be digital, not analog!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
