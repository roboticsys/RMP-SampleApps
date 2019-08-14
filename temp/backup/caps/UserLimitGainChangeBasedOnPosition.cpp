/*! 
@example    userLimitGainChangeBasedOnPosition.cpp

*  @page       user-limit-gain-change-based-on-position-cpp userLimitGainChangeBasedOnPosition.cpp

*  @brief      User Limit Gain Change Based on Position sample application.

*  @details
This sample code shows how to configure the XMP controller's User Limits to compare an input bit to a specific pattern.
If the pattern matches, then the specified output bit is activated and a User Event is generated to the host.
In this case User Limit is created to keep track of the axis Actual Position and when position reaches 7500 counts, the Proportional Gain (Kp) changes to 0.75 times the current value.

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
*  @include userLimitGainChangeBasedOnPosition.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

// which axis to use?
const int AXIS_NUMBER = 0;

// which user limit to use?
const int USER_LIMIT = 0;

// which condition to use (0 or 1)
const int CONDITION = 0;

// which gain table
const int GAIN_TABLE = 0;


void UserLimitGainChangeBasedOnPositionMain()
{
    //RapidCode interface classes
    MotionController   *controller;
    Axis         *axis;

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        // initialize RsiController class
        MotionController *controller = MotionController::CreateFromSoftware();
        SampleAppsCPP::HelperFunctions::CheckErrors(controller);

        // initialize RsiAxis class
        Axis *axis = controller->AxisGet(AXIS_NUMBER);
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);

        // this sample uses Interrupts
        controller->InterruptEnableSet(true);

        // set the command/actual position to zero
        axis->PositionSet(0);

        double Axis0TriggerPosition = 7500.0;
        double ProportionalGain = axis->FilterCoeffGet(RSIFilterGainPIDCoeffGAIN_PROPORTIONAL, GAIN_TABLE) * 0.65;

        // configure user limit to evaluate input bit
        controller->UserLimitConditionSet(USER_LIMIT,
            CONDITION,
            RSIUserLimitLogic::RSIUserLimitLogicGT,
            (long)axis->AddressGet(RSIAxisAddressTypeACTUAL_POSITION),
            Axis0TriggerPosition);

        // configure user limit to set OUTPUT_BIT_MASK high when limit is true
        // 64-bit output not supported
        //controller->UserLimitOutputSet(USER_LIMIT, 
        //                         0,
        //                         (uint64)*((uint64*)&ProportionalGain),
        //                         (long)axis->AddressGet(RSIAxisAddressType::RSIAxisAddressTypeFILTER_GAIN_KP)
        //                         true); 

        // set the configuration
        controller->UserLimitConfigSet(USER_LIMIT, RSIUserLimitTriggerType::RSIUserLimitTriggerTypeSINGLE_CONDITION,
            RSIAction::RSIActionNONE, 0, 0.0);

        axis->MoveVelocity(500, 500);

        printf("Waiting for axis0 to reach position specified....\n");

        // wait for user limit to trigger
        while (controller->InterruptWait(RSIWaitFOREVER) != RSIEventType::RSIEventTypeUSER_LIMIT)
        {
        }

        printf("Axis0 reached specified position. Proportional Gain for axis0 changed!\n");

        // disable User Limit
        controller->UserLimitDisable(USER_LIMIT);

        // stop velocity move
        axis->Stop();

    }
    catch (RsiError const& rsiError)
    {
        printf("Text:  %s\n", rsiError.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    return;
}
