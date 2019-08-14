/*! 
@example    multiaxisMotion.cpp

*  @page       multi-axis-motion-cpp multiaxisMotion.cpp

*  @brief      Multi-Axis Motion sample application.

*  @details
We have created arrays for start_position, end_position, velocity, accel, decel and jerkPct and used them in function called MoveSCurve which commands motion for both the axis->

Syncpptart commands both motors to start moving at the same time towards their respective end_positions.
SyncEnd commands both the motors to end their motion at the same time when moving towards start_positions.

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
*  @include multiaxisMotion.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
#include <conio.h>

using namespace RSI::RapidCode;

const int AXIS_X = 0;
const int AXIS_Y = 1;



const int AXIS_COUNT = (2);
long axisNumber[AXIS_COUNT] = { 0, 1, };

double start_position[2] = { 0 , 0 };
double end_position[2] = { 2000 , 6000 };
double velocity[2] = { 1000, 1000 };
double accel[2] = { 10000, 10000 };
double decel[2] = { 10000, 10000 };
double jerkPct[2] = { 0, 0 };

long index;


/*This function will print all the results to the screen*/
void PrintResult(Axis *axisX, Axis *axisY)
{
    printf("Motion Done \n AxisX position->  Commanded: %f  \tActual: %f\n",
        axisX->CommandPositionGet(),
        axisX->ActualPositionGet());

    printf(" AxisY position->  Commanded: %f  \tActual: %f\n",
        axisY->CommandPositionGet(),
        axisY->ActualPositionGet());
}


void multiaxisMotionMain()
{

    // Create and initialize RsiController class 
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);
    try
    {
        // enable one MotionSupervisor for the MultiAxis
        controller->MotionCountSet(controller->AxisCountGet() + 1);

        // Get Axis X and Y respectively.
        Axis *axisX = controller->AxisGet(AXIS_X);
        SampleAppsCPP::HelperFunctions::CheckErrors(axisX);
        Axis *axisY = controller->AxisGet(AXIS_Y);
        SampleAppsCPP::HelperFunctions::CheckErrors(axisY);


        // Initialize a MultiAxis, using the last MotionSupervisor.
        MultiAxis *multiAxisXY = controller->MultiAxisGet(controller->MotionCountGet() - 1);
        SampleAppsCPP::HelperFunctions::CheckErrors(multiAxisXY);
        multiAxisXY->AxisAdd(axisX);
        multiAxisXY->AxisAdd(axisY);

        // make sure all axes are enabled and ready
        multiAxisXY->ClearFaults();
        multiAxisXY->AmpEnableSet(true);

        while (controller->OS->KeyGet(RSIWaitPOLL) < 0)
        {
            // Set SYNC_START motion attribute mask.
            multiAxisXY->MotionAttributeMaskOnSet(RSIMotionAttrMask::RSIMotionAttrMaskSYNC_START);
            printf("\nMotionStart...");

            // Commanding motion using Syncpptart to start motion for both the axis at the same time.
            multiAxisXY->MoveSCurve(end_position, velocity, accel, decel, jerkPct);
            multiAxisXY->MotionDoneWait();

            // Calling function created on top.
            PrintResult(axisX, axisY);

            // Set SYNC_END motion attribute mask
            multiAxisXY->MotionAttributeMaskOnSet(RSIMotionAttrMask::RSIMotionAttrMaskSYNC_END);
            printf("\nMotionStart...");

            // Commanding motion using SyncEnd to end motion for both the axis at the same time.
            multiAxisXY->MoveSCurve(start_position, velocity, accel, decel, jerkPct);

            // while loop to keep motor spinning while motion not completed.
            multiAxisXY->MotionDoneWait();
            PrintResult(axisX, axisY);
        }

        printf("\nTrapezoidal Motion Done\n");

    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();

}