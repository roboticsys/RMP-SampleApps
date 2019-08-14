/*!
@example    userLimitStateAction.cpp

*  @page       user-limit-state-action-cpp userLimitStateAction.cpp

*  @brief      User Limit State Action sample application.

*  @details
This sample code shows how to configure the XMP controller's User Limits to compare an input bit to a specific pattern.
If the pattern matches, then the specified output bit is activated and a User Event is generated to the host.

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
*  @include userLimitStateAction.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

// which axis to E-STOP_ABORT?
const int AXIS_NUMBER = 0;

// which axis will we monitor
const int TRIGGER_AXIS_NUMBER = 1;
// which user limit to use?
const int USER_LIMIT = 0;

// which condition to use (0 or 1)
const int CONDITION = 0;

#define ACTION                        RSIActionE_STOP_ABORT

void UserLimitStateActionMain()
{
    // RapidCode interface classes
    MotionController   *controller;
    Axis            *axis;
    Axis         *triggerAxis;

    uint64  axisStatusAddress;
    int32  axisStatusMask;

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        // Initialize MotionController class.
        MotionController *controller = MotionController::CreateFromSoftware();
        SampleAppsCPP::HelperFunctions::CheckErrors(controller);

        // initialize Rsi Axis class
        Axis *axis = controller->AxisGet(AXIS_NUMBER);
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);


        // initialize Rsi Axis class (we will be waiting for this axis' state to become an error)
        triggerAxis = controller->AxisGet(TRIGGER_AXIS_NUMBER);
        SampleAppsCPP::HelperFunctions::CheckErrors(triggerAxis);


        // get the trigger axis' status address
        axisStatusAddress = triggerAxis->AddressGet(RSIAxisAddressTypeSTATUS);

        // ESTOP bit mask
        axisStatusMask = RSIFirmwareStatusESTOP;


        // configure user limit to evaluate another axis' status
        controller->UserLimitConditionSet(USER_LIMIT,
            CONDITION,
            RSIUserLimitLogic::RSIUserLimitLogicEQ,
            (long)axisStatusAddress,
            axisStatusMask,
            axisStatusMask
        );

        // enable the user limit, generate ESTOP_ABORT action when ESTOP occurs on triggerAxis
        controller->UserLimitConfigSet(USER_LIMIT, RSIUserLimitTriggerType::RSIUserLimitTriggerTypeSINGLE_CONDITION,
            ACTION, AXIS_NUMBER, 0.0);

        printf("Waiting for the triggerAxis to have an ESTOP...\n");
        printf("\nPress Any Key To Exit.\n");

        // wait for user limit to trigger
        while (controller->OS->KeyGet(RSIWaitPOLL) < 0)
        {
            printf("User Limit state is %d\r", controller->UserLimitStateGet(USER_LIMIT));
            controller->OS->Sleep(10);
        }


        // disable User Limit
        controller->UserLimitDisable(USER_LIMIT);

    }
    catch (RsiError const& rsiError)
    {
        printf("Text:  %s\n", rsiError.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

