/*! 
@example    home.cpp

*  @page       home-cpp home.cpp

*  @brief      Home sample application.

*  @details
This sample app performs a simple homing routine that triggers home off an input pulse, captures the hardware position, sets the origin and then moves back to that home position.

The home method used in this sample code (RSIHomeMethodNEGATIVE_LIMIT) is one of the 35 homing routines available in our homing documenation.

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
*  @include home.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

const int AXIS_NUMBER = 0;
const int VELOCITY = 8000;
const int ACCELERATION = 6000;
const int DECELERATION = 6000;

//Only used if you set max Travel distances for Various homing stages.
const int MAX_TRAVEL_TO_SWITCH = 10000;
const int MAX_TRAVEL_TO_EDGE = 100;
const int MAX_TRAVEL_TO_INDEX = 2000;

//If you wish to create a log file.  Set this to 1.  This is completely optional.
const int LOG_HOMING = 0;


Axis                *axis;

void homeMain()
{
    MotionController *controller = MotionController::CreateFromSoftware(); //RMP
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        ///////////////////////////////////////////////////////////////
        // Once Per Application Initialization 
        //MotionController *controller = MotionController::CreateFromSoftware(/*rmpPath*/);
        // -or- Not both.


        axis = controller->AxisGet(AXIS_NUMBER);
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);
        ///////////////////////////////////////////////////////////////

        // These is unlikely to change.  If it will not.  Put it in Initialization above.
        axis->HomeMethodSet(RSIHomeMethodNEGATIVE_LIMIT);
        axis->HomeOffsetSet(0.0);
        axis->HomeVelocitySet(VELOCITY);
        axis->HomeSlowVelocitySet(VELOCITY / 10);  // Used for final move to index, if selected method does so.
        axis->HomeAccelerationSet(ACCELERATION);
        axis->HomeDecelerationSet(ACCELERATION);

        // Optional Configuration for Max Travel.
        axis->HomeTravelDistanceSet(RSIHomeStageSTAGE_ONE, MAX_TRAVEL_TO_SWITCH);
        axis->HomeTravelDistanceSet(RSIHomeStageSTAGE_TWO, MAX_TRAVEL_TO_EDGE);
        axis->HomeTravelDistanceSet(RSIHomeStageSTAGE_THREE, MAX_TRAVEL_TO_INDEX);

        // May or may not be constantly enabled.
        axis->HomeActionSet(RSIActionSTOP);

        // Ready axis->
        axis->ClearFaults();
        axis->AmpEnableSet(true);

        // Log Homing if you set it to true above.
        if (LOG_HOMING)
        {
            axis->TraceFileSet("RapidCodeHomeTrace.txt"); // Change the file name to whatever you want.
            axis->TraceMaskOffSet(RSITraceALL);
            axis->Trace(true);
        }

        // Begin Homing.
        axis->Home();

        // Close Out Logging.
        if (LOG_HOMING)
        {
            axis->Trace(false);
            axis->TraceFileClose();
        }

        if (axis->HomeStateGet() == true)
        {
            printf("Homing successful\n");
        }
        axis->ClearFaults();
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

