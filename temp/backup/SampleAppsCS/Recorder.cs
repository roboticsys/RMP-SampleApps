/*! 
 *  @example    Recorder.cs

 *  @page       recorder-cs	Recorder.cs
 
 *  @brief      Recorder sample application.
 
 *  @details    In this sample app we show you how easy it is to track multiple drive parameters with a Recorder. 
 
 *  @pre        This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) prior to running this program so that the motor can rotate in a stable manner.
 
 *  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.
 
 *  @copyright 
	Copyright &copy; 1998-2019 by Robotic Systems Integration, Inc. All rights reserved.
	This software contains proprietary and confidential information of Robotic 
	Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
	in the license agreement under which this software is supplied, disclosure, 
	reproduction, or use with controls other than those provided by RSI or suppliers
	for RSI is strictly prohibited without the prior express written consent of 
	Robotic Systems Integration.
 
 *  @include Recorder.cs
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using System;


namespace SampleAppsCS
{
    class Recorder
    {
        static void Main(string[] args)
        {
            // Constants
            const int VALUES_PER_RECORD = 2;          // How many values to store in each record.  
            const int RECORD_PERIOD_SAMPLES = 1;      // How often to record data. (samples between consecutive records)
            const int RECORD_TIME = 5000;             // How long to record. (in milliseconds)

            ulong axis0ActualPositionAddress;         // It will be used to get the network address of Axis 0 - Actual Position.
            ulong ioAddress;                          // It will be used to get the network address of Drive 0 - Digital Inputs.

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                       // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                                   // [Helper Function] Initialize the network.

            try
            {
                // Get the host controller addresses for the values we want to record
                axis0ActualPositionAddress = controller.NetworkInputAddressGet(0);                      // Get the host address of axis 0 actual position.  (Use RapidSetup -.
                
                ioAddress = controller.NetworkInputAddressGet(3);                                       // Get the host address for the drive's inputs.     (Use RapidSetup -.
                               
                //uint firmwareioAddress    = controller.FirmwareAddressGet(ioAddress);           
                
                if (controller.RecorderEnabledGet() == true)                                            // Check if the recorder is running already. If it is, stop it.
                {
                    controller.RecorderStop();                                                          // Stop recording.
                    controller.RecorderReset();                                                         // Reset controller.
                }

                controller.RecorderCountSet(1);                                                         // Number of processed Recorders in the controller.
                controller.RecorderPeriodSet(RECORD_PERIOD_SAMPLES);                                    // Configure recorder to record every 'n' samples (Every 'n' ms)
                controller.RecorderCircularBufferSet(false);                                            // Do not use a circular buffer
                controller.RecorderDataCountSet(VALUES_PER_RECORD);                                     // Configure the number of values for each record. (how many things to record)
                controller.RecorderDataAddressSet(0, axis0ActualPositionAddress);                       // Configure the recoder to record values from this address (1st recorded address)
                controller.RecorderDataAddressSet(1, ioAddress);                                        // Configure the recoder to record values from this address (2nd recorded address)

                controller.RecorderStart();                                                             // Start recording.
                controller.OS.Sleep(RECORD_TIME);                                                       // Put this thread to sleep for some milliseconds.
                
                int recordsAvailable = controller.RecorderRecordCountGet();                             // find out how many records were recorded. (Get the number of records that are stored and ready for reading)

                Console.WriteLine("There are {0} Records available.\n", recordsAvailable);              // Print the number of records recorded.

                for (int i = 0; i < recordsAvailable; i++)                                              // Check every record recorded.
                {
                    controller.RecorderRecordDataRetrieve();                                            // Retrieve one record of recorded data.

                    UInt32 positionRecord = (UInt32)controller.RecorderRecordDataValueGet(0);           // Get the value from the 1st spicified address.
                    var digitalInputsValue = controller.RecorderRecordDataValueGet(1);                            // Get the value from the 2nd spicified address.

                    if ((digitalInputsValue & 0x00010000) == 0x00010000)                                // We are checking to see if the digital input 1 bit changes. If it does then we record the position when that input went high. 
                    {
                        i = recordsAvailable;                                                           // break from loop.
                        Console.WriteLine("Your encoder position was: " + positionRecord + " when the input triggered.");
                    }
                }

                controller.RecorderStop();                                                              // Stop recording.
                controller.RecorderReset();                                                             // Reset controller.
            }
            catch (RsiError e)
            {
                Console.WriteLine(e.Message);                                                           // If there are any exceptions/issues this will be printed out.
            }
            Console.WriteLine("\nPress Any Key To Exit");                                           // Allow time to read Console.
            Console.ReadKey();
        }
    }
}

