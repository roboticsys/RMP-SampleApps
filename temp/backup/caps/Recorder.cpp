/*! 
@example    Recorder.cpp

*  @page       Recorder-cpp Recorder.cpp

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
*
*  @include Recorder.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void RecorderMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                    // Specify which axis/motor to control.
    const int VALUES_PER_RECORD = 2;              // How many values to store in each record.  
    const int RECORD_PERIOD_SAMPLES = 1;          // How often to record data. (samples between consecutive records)
    const int RECORD_TIME = 5000;                 // How long to record. (in milliseconds)

    uint64 axis0ActualPositionAddr;
    uint64 axis0CommandVelocityAddr;
    uint64 axis1ActualPositionAddr;

    int32 *recordDataPtr;
    int32 recordData[VALUES_PER_RECORD];

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        // configure Recorder to record every 'n' samples
        controller->RecorderPeriodSet(RECORD_PERIOD_SAMPLES);

        // do not use a circular buffer
        controller->RecorderCircularBufferSet(false);

        // configure the number of values for each record
        controller->RecorderDataCountSet(VALUES_PER_RECORD);

        // get the host controller addresses for the values we want to record
        axis0ActualPositionAddr = controller->AxisGet(0)->AddressGet(RSIAxisAddressType::RSIAxisAddressTypeACTUAL_POSITION);
        axis0CommandVelocityAddr = controller->AxisGet(0)->AddressGet(RSIAxisAddressType::RSIAxisAddressTypeCOMMAND_VELOCITY);
        axis1ActualPositionAddr = controller->AxisGet(1)->AddressGet(RSIAxisAddressType::RSIAxisAddressTypeACTUAL_POSITION);

        // configure the recoder to record values from these addresses
        controller->RecorderDataAddressSet(0, axis0ActualPositionAddr);
        controller->RecorderDataAddressSet(1, axis0CommandVelocityAddr);
        controller->RecorderDataAddressSet(2, axis1ActualPositionAddr);

        // start recording
        controller->RecorderStart();

        // put this thread to sleep for some milliseconds
        controller->OS->Sleep(RECORD_TIME);

        // stop recording
        controller->RecorderStop();

        // find out how many records were recorded
        long recordsAvailable = controller->RecorderRecordCountGet();
        printf("There are %ld Records available.\n", recordsAvailable);

        // print all the records
        for (long i = 0; i < recordsAvailable; i++)
        {
            // get the pointer to the record data
            recordDataPtr = controller->RecorderRecordDataGet();

            // copy the recorded data into an array
            memcpy(&recordData, recordDataPtr, sizeof(recordData));

            // print first data value 
            printf("Record %ld: Axis 0 ActPos: %lf ", i, recordData[0]);

            // print second data value 
            printf("Axis 0 CmdVel: %lf ", recordData[1]);

            // print third data value 
            printf("Axis 1 ActPos: %lf\n", recordData[2]);
        }

    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

