/*! 
@example    pvtMotion.cpp

*  @page       pvt-motion-cpp pvtMotion.cpp

*  @brief      PVT Motion sample application.

*  @details    This application is designed to demonstrate commanding a Quad Circle movement on a two axis system.

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
*  @include pvtMotion.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void PVTmotionMultiAxisMain()
{
    using namespace RSI::RapidCode;

    const int AXIS_X = (0);
    const int AXIS_Y = (1);
    const int POINTS = (3000);  //total points used
    const int AXIS_COUNT = (2);    //two axis computation (X & Y)
    const double TIME_SLICE = (0.01); //each point processed within 10ms
    const int USER_UNITS = 1;      // Specify USER UNITS


    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);       // [Helper Function] Initialize the network.
        controller->AxisCountSet(2);                                       // Set the number of axis being used. A phantom axis will be created if for any axis not on the network. You may need to refresh rapid setup to see the phantom axis.

        MultiAxis               *multiAxisXY;
        Axis *axisX = controller->AxisGet(AXIS_X);                         // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        Axis *axisY = controller->AxisGet(AXIS_Y);                         // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axisX);                // [Helper Function] Check that the axis has been initialize correctly.
        SampleAppsCPP::HelperFunctions::CheckErrors(axisY);                // [Helper Function] Check that the axis has been initialize correctly.

        axisX->UserUnitsSet(USER_UNITS);                                                         // Specify the counts per Unit.
        axisY->UserUnitsSet(USER_UNITS);                                                         // Specify the counts per Unit.

        // enable one MotionSupervisor for the MultiAxis
        //controller->MotionCountSet(controller->AxisCountGet() + 1);

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

        long radius = 1000;                     //radius of circle

        double PI = 3.14159265358979323;        //variable used in equation to convert degrees to radians

        double degrees = 90.00 / (POINTS);      //angle between each point. Used in the equation below.

        double position[POINTS * AXIS_COUNT];   //defining size of position array
        double vel[POINTS * AXIS_COUNT];        //defining size of velocity array
        double time[POINTS];                    //defining size of time array

        ////////////////////Calculations to Auto Generate Position & Time Array////////////////////////////////
        for (int i = 0, x = 0, y = 1; i < POINTS; i++)
        {
            position[x] = (radius*cos((i + 1)*degrees*PI / 180.0));
            position[y] = (radius*sin((i + 1)*degrees*PI / 180.0));

            x = x + 2;
            y = y + 2;

            time[i] = TIME_SLICE;
        }

        ////////////////////Calculations to Auto Generate Velocity Array////////////////////////////////
        for (int i = 0, x = 0, y = 1; i < (POINTS - 1); i++)
        {
            vel[x] = (position[x + 2] - position[x]) / TIME_SLICE;
            vel[y] = (position[y + 2] - position[y]) / TIME_SLICE;

            x = x + 2;
            y = y + 2;
        }

        //Final two points (X Axis Final Vel, Y Axis Final Vel) need to be set to 0.
        vel[(POINTS * 2) - 2] = 0;  //X Axis
        vel[(POINTS * 2) - 1] = 0;  //Y Axis

        multiAxisXY->Abort();
        multiAxisXY->ClearFaults();
        multiAxisXY->AmpEnableSet(true);

        axisX->PositionSet(radius);
        axisY->PositionSet(0);

        multiAxisXY->MovePVT(position, vel, time, POINTS, -1, false, true);
        multiAxisXY->MotionDoneWait();
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


