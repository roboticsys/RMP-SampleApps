/*! 
@example    pathMotion.cpp

*  @page       path-motion-cpp pathMotion.cpp

*  @brief      Path Motion sample application.

*  @details
This sample app will command a path motion for two axis with different points and desired velocity, acceleration and deceleration specified by user.

Different positions have to be secified within 'PathListStart' and 'PathListEnd' using function 'PathLineAdd' where the coordinates are declared in line_A, line_B and etc.

Function 'PathRatioSet(ratio)' is used to define different ratios between the two different drive's encoder resolution. As the name 'PathMotionStart' suggests will simply execute motion for axes.

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
*  @include pathMotion.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

void pathMotionMain()
{


    Axis                    *axisX;
    Axis                    *axisY;
    MultiAxis                  *multiAxisXY;

    const int AXIS_X = 0;
    const int AXIS_Y = 1;

    double line_A[2] = { 0, 1000 };
    double line_B[2] = { 1000,1000 };
    double line_C[2] = { 1000, 0 };
    double line_D[2] = { 0, 0 };

    double arc_center[2] = { 1000,1000 };
    // Create and initialize RsiController class 
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.



        // enable one MotionSupervisor for the MultiAxis
        controller->MotionCountSet(controller->AxisCountGet() + 1);

        // Get Axis X and Y respectively.
        axisX = controller->AxisGet(AXIS_X);
        SampleAppsCPP::HelperFunctions::CheckErrors(axisX);

        axisY = controller->AxisGet(AXIS_Y);
        SampleAppsCPP::HelperFunctions::CheckErrors(axisY);

        // Initialize a MultiAxis, using the last MotionSupervisor.
        multiAxisXY = controller->MultiAxisGet(controller->MotionCountGet() - 1);
        SampleAppsCPP::HelperFunctions::CheckErrors(multiAxisXY);
        multiAxisXY->AxisAdd(axisX);
        multiAxisXY->AxisAdd(axisY);

        // make sure all axes are enabled and ready
        multiAxisXY->ClearFaults();
        multiAxisXY->AmpEnableSet(true);

        // set the trajectory info
        multiAxisXY->VectorVelocitySet(1000.0);
        multiAxisXY->VectorAccelerationSet(10000.0);
        multiAxisXY->VectorDecelerationSet(10000.0);

        // start path list
        double start_positions[2] = { axisX->CommandPositionGet(),axisY->CommandPositionGet() };
        multiAxisXY->PathListStart(start_positions);

        // turn on blending (smooth corners)
        multiAxisXY->PathBlendSet(true);

        // an X-Y circle
        multiAxisXY->PathArcAdd(arc_center, 360.0);

        // a rectangle
        multiAxisXY->PathLineAdd(line_A);
        multiAxisXY->PathLineAdd(line_B);
        multiAxisXY->PathLineAdd(line_C);
        multiAxisXY->PathLineAdd(line_D);

        // end path list
        multiAxisXY->PathListEnd();

        // execute the motion
        multiAxisXY->PathMotionStart();

        // wait for motion to complete
        multiAxisXY->MotionDoneWait();

    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

