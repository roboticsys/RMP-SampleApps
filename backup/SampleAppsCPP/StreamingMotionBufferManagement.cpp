/*! 
@example    StreamingMotionBufferManagement.cpp

*  @page       streaming-motion-buffer-management-cpp StreamingMotionBufferManagement.cpp

*  @brief      Streaming Motion Buffer Management sample application.

*  @details
This sample code is built for the RMP but the concepts should apply to all RapidSoftware Applications.
It gives a very simple initialization of an RMP MotionController.  It only prints out errors rather than handling them.

This application assumes that you will be generating points which have times of exactly 1 sample duration.
If you want to generate points of size X samples, be sure to divide initial Sample Rate Deduction by X.

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
*  @include StreamingMotionBufferManagement.cpp
*/

#include "rsi.h"                                    // Import our RapidCode Library. 

#include <process.h>

using namespace RSI::RapidCode;

const int AXIS_COUNT = 2;

const int DESIRED_POINTS = 50; //We'd like to keep a buffer of 50 points.
const int SYNC_PERIOD = 10; //Every 10 samples.

int32 lastSample = 0;

#pragma region Fixed Size Implementation
//Some applications might like to send block of say 10 points.
//This means they might end up with anywhere from DESIRED_POINTS to DESIRED_POINTS + (POINT_BLOCK_COUNT - 1) in points at the time of Motion Call.
//We want to adjust desired points and keep track of the extra for future calculations. 
//You don't need to do it this way unless you've a reason to.
//If you are using fixed size blocks, you might just want to just use MotionIdGet() and ExecutingMotionIdGet() to determine the number of blocks to send.
const int POINT_BLOCK_COUNT = 10;
int32 extraPointsSentToEvenOutBlock = 0;

int32 AdjustPointsNeededToBlockCount(int32 initialCount)
{
    int32 calculatedCount = 0;
    int32 adjustedInitialCount = initialCount - extraPointsSentToEvenOutBlock;

    while ((adjustedInitialCount > 0) && (adjustedInitialCount > calculatedCount))
    {
        calculatedCount += POINT_BLOCK_COUNT;
    }

    //Adjust ExtraPointsSent so it can be used in future cycles.
    extraPointsSentToEvenOutBlock += calculatedCount - initialCount;

    return calculatedCount;
}
#pragma endregion


volatile int _continueMonitoring = true;
//In this example the expected passed Object is a Controller Object.
void monitoringThread(void* tmp)
{
    MotionController* controller = (MotionController*)tmp;

    //Configure Sync Interrupts here.
    controller->SyncInterruptPeriodSet(SYNC_PERIOD);
    controller->SyncInterruptEnableSet(true);

    while (_continueMonitoring)
    {
        //SyncInterruptWait will wait until the next Interrupt generated by the controller.  
        //This is the best way to periodically trigger your application while getting useful information about jitter.
        int32 sampleRecieved = controller->SyncInterruptWait();
        int32 pointsNeeded = sampleRecieved - lastSample;

        //Some applications may want to see specific block counts.  You likely don't need to do this.  See region (Fixed Size Implementation) Notes above.
        pointsNeeded = AdjustPointsNeededToBlockCount(pointsNeeded);

        //Note this application doesn't do the following as it is intended to help organization rather than preform motion.  Please see other examples for that.
        //Generate pointsNeeded Points.
        //Potentially Command Move with generated Points -or- signal thread to do so -or- any number of things.

        //Save sample for next calculation.
        lastSample = sampleRecieved;

        //One possible point lookup method might be to keep a rotating buffer of points which you use SampleRate Modulus BufferSize for setting/lookup.
        //SampleRateGet() Modulus BufferSize would be your key for current point.
    }
}

//Print out any errors on the associated Object.
void checkErrors(RapidCodeObject *object)
{
    while (object->ErrorLogCountGet() > 0)
    {
        printf("Error Found: %s\n", object->ErrorLogGet()->text);
    }
}

void streamingMotionBufferManagementMain()
{
    // Initialize MotionController.  An example path to RapidCode diretory provided.
    // NOTE: Use CreateFromBoard(#) and skip NetworkStart() for SynqNet.
    MotionController *controller = MotionController::CreateFromSoftware(/*rmpPath*/);;// .XX\\");
    checkErrors(controller);

    try
    {


        // Initialize the Network.
        controller->NetworkStart();
        checkErrors(controller);

        //Create Monitoring Thread.
        _beginthread(monitoringThread, 0, controller);

        //Wait for some exit condition
        bool exitCondition = false;
        while (!exitCondition)
        {
            //In theory, you application will be doing a lot in here.  Note this application has no way to trigger exitCondition and will run forever.
        }
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.

    //Exit Gracefully
    _continueMonitoring = false;
    //Wait an appropriate time to get from the top of the Monitoring Loop to the bottom of it.
}