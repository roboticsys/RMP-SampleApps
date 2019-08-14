/// Use a periodic interrupt from the controller.  Requires RTOS and Custom97 firmware.
/*  custom97.cpp

 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 For any questions regarding this sample code please visit our documentation at www.roboticsys.com
 or call us at (312) 541-2600.

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.
*/

#include "rsi.h"

using namespace RSI::RapidCode::SynqNet;

//configurable
#define SYNC_PERIOD				(1)		// interrupt every SynqNet/MotionController sample
#define FIRMWARE_WAIT_TIME		(12000)  // 25 nanosecond cycles to delay the firmware foreground -- "tune" this value using VM3 and HostProcessTime
#define DATA_READ_COUNT			(3)		// how many Rincon values to read
#define DATA_WRITE_COUNT		(3)		// how many values to write to the Rincon buffer


// hard-coded addresses from VM3  -- these are the addresses we want to read/write in the SynqNet buffer
#define ENCODER_X_ADDR			(0x1000180)
#define ENCODER_Y_ADDR			(0x1000220)
#define ENCODER_Z_ADDR			(0x1000260)

#define TORQUE_X_ADDR			(0x1000184)
#define TORQUE_Y_ADDR			(0x1000224)
#define TORQUE_Z_ADDR			(0x1000264)

//constants
#define MS_PER_SECOND			(1000.0)



// some global data for the sample code
MotionController *controller;

long			currentCounter = 0;
long			previousCounter = 0;
long			deltaSamples = 0;
long			iterations = 0;
unsigned long	cpuFreq = 0;
double			deltaTime = 0.0;
double			minTime = 1000000.0;
double			maxTime = 0.0;


// define your own structures for storing the Custom97 data -- up to 64 32-bit values each
typedef struct {
	long encoderX;
	long encoderY;
	long encoderZ;
} ReadData;


typedef struct {
	long torqueX;
	long torqueY;
	long torqueZ;
} WriteData;

long readSize;
long writeSize; 

ReadData *readData;
WriteData *writeData;




void CheckHostProcessTimeStatus()
{

	// did we take too long processing the previous interrupt?
	if(controller->SyncInterruptHostProcessStatusBitGet() == true)
	{
		printf("\n\n Oops, we took too long processing the last interrupt. \n");

		// clear the Host Process Status Bit
		controller->SyncInterruptHostProcessStatusClear();
	}
}


void PrintTimingInfo()
{
	currentCounter = controller->OS->PerformanceTimerCountGet();
	deltaSamples = currentCounter - previousCounter;
	previousCounter = currentCounter;

	deltaTime = (double)( deltaSamples * (double)(1/(double)cpuFreq)) * MS_PER_SECOND;

	if(iterations > 1) // ignore first time through
	{
		if(deltaTime > maxTime)
		{	  
		  maxTime = deltaTime;
		}
		if(deltaTime < minTime)
		{
		  minTime = deltaTime;
		}
		printf("IRQ %ld: %3.3lf ms  Min: %3.3lf  Max: %3.3lf \r", iterations, deltaTime, minTime, maxTime );
	}
}


void SetupCustom97()
{
	// Show the Custom97 version
	printf("Custom97 version is: %ld\n ", controller->Custom97VersionGet());

	// "stretch" the controller firmware's foreground cycle
	controller->Custom97WaitTimeSet(FIRMWARE_WAIT_TIME);
	controller->OS->Sleep(1); // wait a millisecond (multiple samples) for this to take effect

	// setup read Ptrs
	controller->Custom97ReadPtrSet(0, ENCODER_X_ADDR);   // from VM3
	controller->Custom97ReadPtrSet(1, ENCODER_Y_ADDR);   // from VM3
	controller->Custom97ReadPtrSet(2, ENCODER_Z_ADDR);   
    
	// once the read count > 0, the feature is enabled  (must have Ptrs setup already)
	controller->Custom97ReadCountSet(DATA_READ_COUNT);

    // setup write Ptrs
	controller->Custom97WritePtrSet(0, TORQUE_X_ADDR);  // will write to torque outputs(ZMP)
    controller->Custom97WritePtrSet(1, TORQUE_Y_ADDR); 
    controller->Custom97WritePtrSet(2, TORQUE_Z_ADDR);  

	// once the write count > 0, the feature is enabled
    printf("Setting Write count");
    controller->Custom97WriteCountSet(DATA_WRITE_COUNT);

	// initialize our data structures
	readData = new ReadData;
	writeData = new WriteData;

	readSize = sizeof(ReadData);
	writeSize = sizeof(WriteData);
}


void DisableCustom97()
{
	// disable reading and writing
	controller->Custom97ReadCountSet(0);
	controller->Custom97WriteCountSet(0);

	delete readData;
	delete writeData;
}


void custom97Main()
{
	try
	{
		// create and initialize MotionController class (PCI board)
		controller = MotionController::CreateFromBoard(0);	

		// disable the service thread if using the controller Sync interrupt
		controller->ServiceThreadEnableSet(false);

		// configure the Custom97 read/write buffers
		SetupCustom97();

		// Get CPU frequency from Operating System performance counter
        cpuFreq = controller->OS->PerformanceTimerFrequencyGet();
		printf("CPU Frequency is: %u Hz\n", cpuFreq);

		// See how much total time is available for Sync interrupt processing (before SynqNet buffer is DMA'd)
		printf("Host will have %ld microseconds to process data.\n", controller->SyncInterruptHostProcessTimeGet() );

		// configure a Sync interrupt for every sample
		controller->SyncInterruptPeriodSet(SYNC_PERIOD);
		
		// enable controller interrupts
		controller->SyncInterruptEnableSet(true);
		
		printf("Press a key to exit the Sync Interrupt processing loop...\n");
		while( controller->OS->KeyGet(RSIWaitPOLL) < 0)
		{
			// wait for the controller's Sync interrupt
			controller->SyncInterruptWait();

			// see if we exceeded our processing time during the previous interrupt
			CheckHostProcessTimeStatus();

			// see how long it's been since last interrupt
			PrintTimingInfo();

			// tell the controller firmware that we are going to do some calculations
			controller->SyncInterruptHostProcessFlagSet(true);

			// read SynqNet data from Custom97 read buffer
			controller->Custom97ReadDataGet(readData, readSize);
	
			//
			// Your real-time calculations here
			//

			// create some dummy data
			writeData->torqueX = 0;
			writeData->torqueY = 1;
			writeData->torqueZ = 2;

			// write our datat to Custom97 write buffer
			controller->Custom97WriteDataSet(writeData, writeSize);

			// tell the controller firmware that we have finished our calculations
			controller->SyncInterruptHostProcessFlagSet(false);

			iterations++;  // used for printing info
		}

		// turn off Custom97 features
		DisableCustom97();

		// turn off Sync Interrupt
		controller->SyncInterruptEnableSet(false);
	}
	catch (RsiError *err)
	{
		printf("%s\n", err->text);
	}

	printf("Press a key to exit.\n");
	while( controller->OS->KeyGet(RSIWaitPOLL) < 0)
	{
		controller->OS->Sleep(100); //ms
	}
}

