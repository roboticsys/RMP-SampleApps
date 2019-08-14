/*! 
@example    syncInterrupt.cpp

*  @page       sync-interrupt-cpp syncInterrupt.cpp

*  @brief      Sync Interrupt sample application.

*  @details    Use a periodic interrupt from the controller. (Requires RTOS)

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
*  @include syncInterrupt.cpp
*/

#include <windows.h>    // this is only needed if you try to set the windows thread priority#include "rsi.h"                                    // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

//configurable
const int SYNC_PERIOD = (1);        // interrupt every SynqNet/MotionController sample
const int AXIS_COUNT = (6);  // how many axes will we process each sample

//constants
const double MS_PER_SECOND = (1000.0);


void syncInterruptMain()
{

    Axis            *axes[AXIS_COUNT];
    double            encoderPositions[AXIS_COUNT];
    short            torqueOutputs[AXIS_COUNT] = { 0, 0, 0, 0, 0, 0 };
    long            currentCounter = 0;
    long            previousCounter = 0;
    long            deltaSamples = 0;
    long            iterations = 0;
    unsigned long    cpuFreq = 0;
    double            deltaTime = 0.0;
    double            minTime = 1000000.0;
    double            maxTime = 0.0;

    long i = 0;
    long errorCount = 0;
    // create and Initialize MotionController class. (PCI board)
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);
    try
    {



        // get Axis objects from MotionController
        for (i = 0; i < AXIS_COUNT; i++)
        {
            axes[i] = controller->AxisGet(i);
            SampleAppsCPP::HelperFunctions::CheckErrors(axes[i]);
        }

        // disable the service thread if using the controller Sync interrupt
        controller->ServiceThreadEnableSet(false);

        // Get CPU frequency from Operating System performance counter
        cpuFreq = controller->OS->PerformanceTimerFrequencyGet();
        printf("CPU Frequency is: %u Hz\n", cpuFreq);

        // See how much total time is available for Sync interrupt processing (before SynqNet buffer is DMA'd)
        printf("Host will have %ld microseconds to process data.\n", controller->SyncInterruptHostProcessTimeGet());

        //
        // it will be required to set a high thread priority here for Windows... you really should have an RTOS
        //
        SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL);

        // configure a Sync interrupt for every sample
        controller->SyncInterruptPeriodSet(SYNC_PERIOD);

        // enable controller interrupts
        controller->SyncInterruptEnableSet(true);

        // wait for someone to press a key 
        while (controller->OS->KeyGet(RSIWaitPOLL) < 0)
        {
            // wait for the controller's Sync interrupt
            controller->SyncInterruptWait();

            // see if we exceeded our processing time during the previous interrupt
            // did we take too long processing the previous interrupt?
            if (controller->SyncInterruptHostProcessStatusBitGet() == true)
            {
                printf("\n\n Oops, we took too long processing the last interrupt. \n");

                // clear the Host Process Status Bit
                controller->SyncInterruptHostProcessStatusClear();
            }

            // see how long it's been since last interrupt
            currentCounter = controller->OS->PerformanceTimerCountGet();
            deltaSamples = currentCounter - previousCounter;
            previousCounter = currentCounter;
            deltaTime = (double)(deltaSamples * (double)(1 / (double)cpuFreq)) * MS_PER_SECOND;
            if (iterations > 1) // ignore first time through
            {
                if (deltaTime > maxTime)
                {
                    maxTime = deltaTime;
                }
                if (deltaTime < minTime)
                {
                    minTime = deltaTime;
                }
                printf("IRQ %ld: %3.3lf ms  Min: %3.3lf  Max: %3.3lf \n", iterations, deltaTime, minTime, maxTime);
            }

            // tell the controller firmware that we are going to do some calculations
            controller->SyncInterruptHostProcessFlagSet(true);


            // Get Encoder Positions
            for (int i = 0; i < AXIS_COUNT; i++)
            {
                encoderPositions[i] = axes[i]->EncoderPositionGet(RSIMotorFeedbackPRIMARY);
            }
            //
            // do calculations here
            //
            // Set Torque Outputs
            for (int i = 0; i < AXIS_COUNT; i++)
            {
                axes[i]->FilterCoeffSet(RSIFilterGainPIDCoeffOUTPUT_OFFSET, 0, torqueOutputs[i]);   // gain table 0
            }



            // tell the controller firmware that we have finished our calculations
            controller->SyncInterruptHostProcessFlagSet(false);

            iterations++;  // used for printing info
        }

        // turn off Sync Interrupt
        controller->SyncInterruptEnableSet(false);
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }

    printf("Press a key to exit.\n");
    while (controller->OS->KeyGet(RSIWaitPOLL) < 0)
    {
        controller->OS->Sleep(100); //ms
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}



