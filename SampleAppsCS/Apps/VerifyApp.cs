/// @example VerifyApp.cs  This sample code performs a check on the SynqNet Configuration and the configuration of only the 1st node in the system.
/* VerifyApp.cs

 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 VerifyApp.cs: Performs a check on the SynqNet Configuration and the 
 configuration on only the 1st node in the system. Would show better result if the 1st node
 were connected to an axis instead of SLICE I/O.

 For any questions regarding this sample code please visit our documentation at www.roboticsys.com
 or call us at (312)727-0080.

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
    public class VerifyApp
    {
        // RapidCode objects
        MotionController controller;
        Axis axis;

        //constants
        const int AXIS_NUMBER = 0;

        [Test]
        public void Main()
        {
            try
            {
                //Initialize RapidCode Objects
                controller = MotionController.Create();
                axis = controller.AxisGet(AXIS_NUMBER);
                                
                //Disable the amplifier
		        axis.AmpEnableSet(false);

		        Console.WriteLine("Application Verification::::::\n\n");
        		
		        //ALWAYS CHECK
		        // Identifies the network topology
                if (controller.SynqNetNetworkTypeGet() == RSINetworkType.RSINetworkTypeRING)
		        {
                    Console.WriteLine("Network topology:              RING \n\n");
		        }

                if (controller.SynqNetNetworkTypeGet() == RSINetworkType.RSINetworkTypeSTRING)
		        {
                    Console.WriteLine("Network topology:              STRING \n\n");
		        }
        		
		        // SynqNet Node Count - Make sure the number of expected nodes are found during the Discovery phase
                Console.WriteLine("SynqNetNodeCountGet:              " + controller.SynqNetNodeCountGet() + "\n\n");

		        // SqNode Node Type and Option - Make sure the nodes are discovered in the proper order and 
		        // are of the expected hardware TYPE and OPTION
                //Console.WriteLine("SqNodeTypeGet:              " + axis.SqNode.TypeGet() + "\n\n");
                //Console.WriteLine("SqNodeOptionGet:              " + axis.SqNode.OptionGet() + "\n\n");

		        // SqNode FPGA Type - Identifies whether the FPGA image is a Boot or Runtime
                if (axis.SqNode.FPGATypeGet() == (int)RSISqNodeFpgaType.RSISqNodeFpgaTypeRUN_TIME)
		        {
                    Console.WriteLine("This SqNode uses following FPGA image:              Runtime Image\n\n");
		        }
                if (axis.SqNode.FPGATypeGet() == (int)RSISqNodeFpgaType.RSISqNodeFpgaTypeBOOT)
		        {
			        Console.WriteLine("This SqNode uses followsing FPGA image:              Boot Image, factory default\n\n");
		        }	

		        // FPGA VendorDevice - Identifies the FPGA image
                Console.WriteLine("SqNodeFPGAVendorGet:              " + axis.SqNode.FPGAVendorGet() + "\n\n");

		        // SqNode FPGA Version - Make sure the FPGA version is up to date with the software version
                Console.WriteLine("SqNodeFPGAVersionGet:              " + axis.SqNode.FPGAVersionGet() + "\n\n");

		        // SqNode Exact Match - Identifies whether or not the software exactly knows the node type 
		        // and FPGA image combination
		        if(axis.SqNode.ExactMatchGet() == true)
		        {
			        Console.WriteLine("This SqNode exactly matches the software version\n\n");
		        }

		        // SqNode Motor Count and Offset - The number of motors per node and their offsets
                Console.WriteLine("SqNodeMotorCountGet:              " + axis.SqNode.MotorCountGet() + "\n\n" );
                Console.WriteLine("SqNodeMotorOffsetGet:              " + axis.SqNode.MotorOffsetGet() + "\n\n");
                
		        // SqNode Drive Firmware Version - Make sure the drive firmware version is correct for your application
                Console.WriteLine("MPIFirmwareVersionGet:              " + controller.MpiFirmwareVersionGet() + "\n\n");

		        // SqNode Switchld
                Console.WriteLine("SqNodeSwitchIDGet:              " + axis.SqNode.SwitchIDGet() + "\n\n");

		        //OPTION INFORMATION TO CHECK
		        // SqNode Node Name - Text string used to identify the Node Type
                Console.WriteLine("SqNodeNameGet:              " + axis.SqNode.NameGet() + "\n\n");

		        // SqNode Model Number - Identifies the particular node model number.
                Console.WriteLine("SqNodeModelNumberGet:              " + axis.SqNode.ModelNumberGet() + "\n\n");
		        
                // SqNode Serial Number - Identifies the particular node serial number.
                Console.WriteLine("SqNodeSerialNumberGet:              " + axis.SqNode.SerialNumberGet() + "\n\n");

                // SqNode Unique - The Node Type, Option, and Unique values will always uniquely identify 
		        // every SynqNet node
                Console.WriteLine("SqNodeUniqueIDGet:              " + axis.SqNode.UniqueIDGet() + "\n\n");

		        // SqNode Drive Count - Identifies the number of drives on a node
		        Console.WriteLine("SqNodeDriveCountGet:              " + axis.SqNode.DriveCountGet() + "\n\n");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

