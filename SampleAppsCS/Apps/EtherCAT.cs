/// @example EtherCAT.cs   Show initialization and usage for EtherCAT systems
/*  
 Copyright(c) 1998-2014 by Robotic Systems Integration, Inc. All rights reserved.
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

    public class EtherCAT
    {
        private const int AXIS_NUMBER = 1;

        private MotionController controller;
        private Axis akdAxis;


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
            controller = MotionController.CreateFromSoftware();
            CheckErrors(controller);

            if (controller.NetworkStateGet() != RSINetworkState.RSINetworkStateOPERATIONAL)
            {
                controller.NetworkStart();  // you must manually start the network
            }


            // check to see if it started
            if (controller.NetworkStateGet() != RSINetworkState.RSINetworkStateOPERATIONAL)
            {
                // some kind of error starting the network, read the network log messages
                int messagesToRead = controller.NetworkLogMessageCountGet();
                for (int i = 0; i < messagesToRead; i++)
                {
                    Console.WriteLine(controller.NetworkLogMessageGet(i));  // print all the messages to help figure out the problem
                }

                //Assert.Fail("Expected OPERATIONAL state but the network did not get there.");
            }



            // show the list of all the raw network PDO inputs and outputs
            int networkInputCount = controller.NetworkInputCountGet();
            for (int i = 0; i < networkInputCount; i++)
            {
                Console.WriteLine("Input " + i + ": " + controller.NetworkInputNameGet(i));
            }
            int networkOutputCount = controller.NetworkOutputCountGet();
            for (int i = 0; i < networkOutputCount; i++)
            {
                Console.WriteLine("Output " + i + ": " + controller.NetworkOutputNameGet(i));
            }


            int nodeCount = controller.NetworkNodeCountGet();
            //Assert.That(nodeCount, Is.Not.EqualTo(0), "We are expecting to find some nodes.");


            for (int i = 0; i < nodeCount; i++)
            {
                // first, try getting each node as an IO node, so we can get some basic node info
                IO io = controller.IOGet(i);
                CheckErrors(io);  // creation methods don't throw errors

                Console.WriteLine("Node " + i + " Name: " + io.NetworkNode.NameGet());
            }


            // we should have at least one Axis on the controller (determined by the nodes found on the network)
            //Assert.That(controller.AxisCountGet(), Is.GreaterThan(1), "this sample expects there to be at least one Axis");

            akdAxis = controller.AxisGet(AXIS_NUMBER);
            CheckErrors(akdAxis);  // creation methods don't throw errors

            //Assert.That(akdAxis.NetworkNode.TypeGet(), Is.EqualTo(RSINodeType.RSINodeTypeKOLLMORGEN_AKD), "this test is expecting that this is an AKD axis.");

        }
    }
}
