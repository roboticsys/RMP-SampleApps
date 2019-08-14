/*! 
@example    template.cpp

*  @page       template-cpp template.cpp

*  @brief      Template sample application.

*  @details
This application is designed to demonstrate simple Controller, Axis, MultiAxis, and I/O declaration. It can be a handy template for starting a RapidCode application.

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
*  @include template.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

const int AXIS_X = (0);
const int AXIS_Y = (1);
const int IO_NODE = (7);


void templateMain(int argc, char   *argv[])
{


    Axis                *axisX;
    Axis                *axisY;
    MultiAxis              *multiAxisXY;
    IO                    *io;
    // Create and initialize RsiController class
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);
    try
    {



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

        // Initialize an IO.
        io = controller->IOGet(IO_NODE);

        // Insert your application code here.

    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

