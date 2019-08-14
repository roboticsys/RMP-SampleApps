/// @example RapidENI.cs   Discover connected devices and generate an ENI file, handle status messages
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
using RSI.RapidCode.dotNET.RapidENILib;

namespace SampleApplications
{

    public class GenerateENIFile
    {
        public const string ENIFileName = "EtherCAT.xml";

 
        public void Main()
        {
            try
            {
                MotionController controller = MotionController.CreateFromSoftware();

                //Optional handling of status updates
                RapidENI.RapidENIStatusChanged += new RapidENIStatusChangedEventHandler(RapidENI_RapidENIStatusChanged);
                
                RapidENIResult res = RapidENI.DiscoverTopology(controller);
                if (res != RapidENIResult.Success)
                   throw new Exception("Failed to Discover Topology, unable to generate file");

                res = RapidENI.GenerateENIFile(controller);
                if (res != RapidENIResult.Success || !System.IO.File.Exists(ENIFileName))
                    throw new Exception("Failed to generate new ENI File");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        void RapidENI_RapidENIStatusChanged(RapidENIStatus status)
        {
            switch (status)
            {
                case RapidENIStatus.ProbingNetwork:
                case RapidENIStatus.ProbeFailed:
                case RapidENIStatus.TopologyDiscovered:
                case RapidENIStatus.DeletingENIFile:
                case RapidENIStatus.GeneratingFile:
                case RapidENIStatus.MissingESIFiles:
                case RapidENIStatus.ENIGenerated:
                case RapidENIStatus.Canceled:
                    Console.WriteLine("Status Update from RapidENI: " + status.ToString());
                    break;
            }

        }
    }
}
*/
