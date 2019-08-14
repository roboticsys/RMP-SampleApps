/*! 
@example    updateBufferPoints.cpp

*  @page       update-buffer-points-cpp updateBufferPoints.cpp

*  @brief      Update Buffer Points sample application.

*  @details
This application is designed to assist in managing the buffer in streaming motion.
The behavior of continuing streaming motion (with the parameter final=false) will increment the MotionID (retrieved by calling MotionIdExecutingGet()).
It will also reset the motion element ID to 0 (retrieved with MotionElementIdExecutingGet()).

The number sent points needs to be managed, somehow. This sample does so by using a fixed buffer size and keeping at least one buffer associated with a Motion ID queued.
This should be changed as appropiate.

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
*  @include updateBufferPoints.cpp
*/

#include <cassert>
#include "rsi.h"                                    // Import our RapidCode Library. 

using namespace RSI::RapidCode;

void PrintUpdateBufferErrors(RapidCodeObject *rsiClass)
{
    RsiError *err;
    bool hasError = rsiClass->ErrorLogCountGet() > 0;

    while (rsiClass->ErrorLogCountGet() > 0)
    {
        err = rsiClass->ErrorLogGet();

        printf("%s\n", err->text);
    }

    if (hasError)
    {
        exit(1);
    }
}

void PrintUpdateBufferNetworkErrors(MotionController *rsiClass)
{
    char *err;
    bool hasError = rsiClass->NetworkLogMessageCountGet() > 0;

    while (rsiClass->NetworkLogMessageCountGet() > 0)
    {
        err = rsiClass->NetworkLogMessageGet(0);

        printf("%s\n", err);
    }

    if (hasError)
    {
        exit(1);
    }
}

void updateBufferPointsMain()
{
    const double    TIME_SLICE = 0.001;                                // 0.001s = 1ms
    const int        AXIS_COUNT = 1;                                    // number of axes

    const int        REVS = 5;                                        // number of revolutions
    const int        RPS = 1;                                        // revs / sec
    const int        CPS = (int)std::powl(2, 20);                    // encoder counts per rev (set as appropiate)
    const int        TOTAL_POINTS = (int)(REVS / TIME_SLICE / RPS);    // total number of points
    const int        BUFFER_SZ = 100;                                // Number of points to send in a buffer
    const int        EMPTY_CT = 10;                                    // Number of points that remains in the beffer before an e-stop

    // Initizalize the controller from software w/ multiple axes
    MotionController *controller;
    MultiAxis *multiAxis;
    controller = MotionController::CreateFromSoftware();
    PrintUpdateBufferErrors(controller);
    try {

        controller->NetworkStart();
        PrintUpdateBufferNetworkErrors(controller);

        // add an additional axis for the multiaxis supervisor
        controller->MotionCountSet(AXIS_COUNT + 1);

        // create the multiaxis using the ID of the first free axis (0 indexed)
        multiAxis = controller->MultiAxisGet(AXIS_COUNT);
        PrintUpdateBufferErrors(multiAxis);

        // populate the multiaxis
        for (int i = 0; i < AXIS_COUNT; i++)
        {
            Axis *tempAxis;
            tempAxis = controller->AxisGet(i);

            PrintUpdateBufferErrors(tempAxis);

            tempAxis->EStopAbort();
            tempAxis->ClearFaults();
            tempAxis->PositionSet(0);

            tempAxis->UserUnitsSet(CPS);
            multiAxis->AxisAdd(tempAxis);
        }

        // populate the positions and times 
        std::vector<double> positions, times;
        for (int i = 0; i < TOTAL_POINTS; i += AXIS_COUNT)
        {
            positions.push_back(i * TIME_SLICE * RPS);
            times.push_back(TIME_SLICE);
        }


        // prepare the controller (and drive)
        multiAxis->Abort();
        multiAxis->ClearFaults();
        assert(multiAxis->StateGet() == RSIState::RSIStateIDLE);
        multiAxis->AmpEnableSet(true);

        // reset the motion ID to 0
        multiAxis->MovePT(RSIMotionType::RSIMotionTypePT, &positions[0], &times[0], 1, -1, false, true);
        multiAxis->MotionIdSet(0);


        // Establish how to keep track of what blocks have been sent
        int curMotionElementID = 0, curMotionID = 0, finalMotionID = 0;
        int numPointsToSend = BUFFER_SZ;
        int endOfLastSent = 0;

        bool exitCondition = false;


        // Set up a motion hold gate so we can start buffering blocks
        const int motionHoldGate = 3;
        controller->MotionHoldGateSet(motionHoldGate, true);
        multiAxis->MotionHoldGateSet(motionHoldGate);

        for (int i = 0; i < 2; ++i)
        {
            multiAxis->MovePT(RSIMotionType::RSIMotionTypePT, &positions[0] + endOfLastSent * AXIS_COUNT, &times[0] + endOfLastSent, numPointsToSend, EMPTY_CT, false, exitCondition);
            endOfLastSent += numPointsToSend;
            ++finalMotionID;
        }


        // Set up the interrupt frequency period
        controller->SyncInterruptPeriodSet(10); // this generates an interrupt every x cycles of a 1KHz sample rate
        // With our timeslices of 1ms, this is 10*1ms=10ms
        controller->SyncInterruptEnableSet(true);

        controller->MotionHoldGateSet(motionHoldGate, false); // release the hold gate to start moving

        while (!exitCondition)
        {
            int32 sampleRecieved = controller->SyncInterruptWait();    // see above for timing
            curMotionID = multiAxis->MotionIdExecutingGet();        // this takes an unspecified amount of time (non-rtos) so this is going to be called sometime after the interrupt gets handled.
            // There's an additional delay to retrieve the data as well.
            curMotionElementID = multiAxis->MotionElementIdExecutingGet();

            /*
            Each MovePT assigns a new MotionID for each call to a move update (with the bufferred points)
            Working under the assumption that each Buffer gets a new ID, send two (or several) smaller ones
            and when the change in IDs is greater than some number, send a new one
            */

            // change this logic to manage the number of buffered moves (and points) as appropiate
            // generate points as appropiate
            if (std::abs(finalMotionID - curMotionID) < 2)
            {
                // check end condition
                if (TOTAL_POINTS <= (endOfLastSent + BUFFER_SZ))
                {
                    numPointsToSend = TOTAL_POINTS - endOfLastSent; // send the remaining points
                    exitCondition = true;
                }
                multiAxis->MovePT(RSIMotionType::RSIMotionTypePT, &positions[0] + endOfLastSent * AXIS_COUNT, &times[0] + endOfLastSent, numPointsToSend, EMPTY_CT, false, exitCondition);

                printf("MotionID %d\nEnd of Last Sent %d\nElement ID %d\nNum to Send %d\nIs Done %s\n===========================================\n\n",
                    curMotionID, endOfLastSent, curMotionElementID, numPointsToSend, exitCondition ? "yes" : "no");

                endOfLastSent += numPointsToSend;
                ++finalMotionID;
            }
        }
        printf("Updates Done. Waiting to finish motion.\n");
        multiAxis->MotionDoneWait();
        printf("Motion Complete. Final Motion ID: %d\tFinal Element ID %d\n", multiAxis->MotionIdExecutingGet(), multiAxis->MotionElementIdExecutingGet());

        multiAxis->EStopAbort();
    }
    catch (RsiError const& err) {
        printf("\n%s\n", err.text);
    }
    controller->Delete();

}
