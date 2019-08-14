/*! 
@example    syncOutputWithMotion.cpp

*  @page       sync-output-with-motion-cpp syncOutputWithMotion.cpp

*  @brief      Sync Output With Motion sample application.

*  @details    Synchronize setting outputs with motion element ID's.

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
*  @include syncOutputWithMotion.cpp
*/

#include <cassert>
#include "rsi.h"                                    // Import our RapidCode Library. 

using namespace RSI::RapidCode;

void PrintStreamingOutputErrors(RapidCodeObject *rsiClass)
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

void PrintStreamingOutputNetworkErrors(MotionController *rsiClass)
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

int syncOutputWithMotionMain(int argc, char *argv[])
{
    const double    TIME_SLICE = 0.01;                                // 0.01s = 10ms
    const int        AXIS_COUNT = 1;                                    // number of axes

    const int        REVS = 4;                                        // number of revolutions
    const int        RPS = 1;                                        // revs / sec
    const int        CPS = (int)std::powl(2, 20);                    // encoder counts per rev (set as appropiate)
    const int        TOTAL_POINTS = (int)(REVS / TIME_SLICE / RPS);    // total number of points
    const int        EMPTY_CT = -1;                                    // Number of points that remains in the buffer before an e-stop


    // Initizalize the controller from software w/ multiple axes

    MultiAxis *multiAxis;        MotionController *controller = MotionController::CreateFromSoftware();

    try {
        PrintStreamingOutputErrors(controller);

        controller->NetworkStart();
        PrintStreamingOutputNetworkErrors(controller);

        // add an additional axis for the multiaxis supervisor
        controller->MotionCountSet(AXIS_COUNT + 1);

        // create the multiaxis using the ID of the first free axis (0 indexed)
        multiAxis = controller->MultiAxisGet(AXIS_COUNT);
        PrintStreamingOutputErrors(multiAxis);

        // populate the multiaxis
        for (int i = 0; i < AXIS_COUNT; i++)
        {
            Axis *tempAxis;
            tempAxis = controller->AxisGet(i);

            PrintStreamingOutputErrors(tempAxis);

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


        // set up the inputs
        IOPoint *output0 = IOPoint::CreateDigitalOutput(multiAxis->AxisGet(0), RSIMotorGeneralIo0);
        // ensure the digital out is set low
        output0->Set(0);


        // The motion element ID at which to set the output
        int outPutEnableID = TOTAL_POINTS / 2;


        // enable streaming output
        multiAxis->StreamingOutputsEnableSet(true);


        // Enable the streaming output (The following are functionally equivalent)
        //multiAxis->StreamingOutputAdd(output0->MaskGet(), 0, output0->AddressGet(), outPutEnableID);
        multiAxis->StreamingOutputAdd(output0, true, outPutEnableID);

        // Disable the output (The following are functionally equivalent)
        //multiAxis->StreamingOutputAdd(0, output0->MaskGet(), output0->AddressGet(), outPutEnableID);
        //multiAxis->StreamingOutputAdd(output0, false, outPutEnableID);


        multiAxis->MovePT(RSIMotionType::RSIMotionTypePT, &positions[0], &times[0], TOTAL_POINTS, EMPTY_CT, false, true);

        printf("Motion started. Waiting to complete.\n");
        multiAxis->MotionDoneWait();
        printf("Motion Complete. The outputs should have been set\n");

        multiAxis->StreamingOutputsClear();  // cleanup for next run

        multiAxis->EStopAbort();

        controller->Delete();
    }
    catch (RsiError const& err) {
        printf("\n%s\n", err.text);
        argc = argc;
        return 1;
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    return 0;
}
