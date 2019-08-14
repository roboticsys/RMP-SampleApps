/*! 
@example    customHome.cpp

*  @page       custom-home-cpp customHome.cpp

*  @brief      Custom Home sample application.

*  @details
This sample app allows the user to trigger his home off a custom input.

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
*  @include customHome.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

// user defined options
const int AXIS_NUMBER = 0;
const int CAPTURE_SOURCE = RSIMotorDedicatedInHOME;

const int HOME_LIMIT_NETWORK_INDEX = 99; //You may want to discover this rather than hard code it.
const int HOME_LIMIT_SIG_BIT = 0;

/* Motion Parameters */
const int VELOCITY = 5000;
const int ACCELERATION = 80000;
const int DECELERATION = 80000;
const int POSITION = 0;


void customHomeMain()
{

    // Initialize MotionController class.
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);

    // initialize Axis class  
    Axis *axis = controller->AxisGet(AXIS_NUMBER);
    SampleAppsCPP::HelperFunctions::CheckErrors(axis);

    uint64 homeLimitAddress = controller->NetworkInputAddressGet(HOME_LIMIT_NETWORK_INDEX);
    axis->HomeLimitCustomConfigSet(homeLimitAddress, HOME_LIMIT_SIG_BIT);
    axis->HomeActionSet(RSIActionSTOP);
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        // Ready Axis
        axis->Abort();
        axis->ClearFaults();
        axis->AmpEnableSet(true);

        printf("Looking for Home...\n\n");

        // commanding a velocity move.  This program assumes that the Custom Home will trigger at some point.
        axis->MoveVelocity(VELOCITY, ACCELERATION);

        // wait (sleep) until motion done interrupt occurs
        while (axis->InterruptWait(RSIWaitFOREVER) != RSIEventTypeMOTION_DONE) {}

        if ((controller->NetworkInputValueGet(HOME_LIMIT_NETWORK_INDEX) & HOME_LIMIT_SIG_BIT) > 0) //On Home Limit
        {
            axis->HomeStateSet(true);
        }
        else
        {
            //Evaluate why we aren't on custom home.
        }

        // setup Home Action (the home action will not trigger)
        axis->HomeActionSet(RSIActionNONE);

        axis->ClearFaults();
        axis->AmpEnableSet(false);
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

