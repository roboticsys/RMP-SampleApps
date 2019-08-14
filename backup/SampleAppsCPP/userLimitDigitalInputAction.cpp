/*!
@example    userLimitDigitalInputAction.cpp

*  @page       user-limit-digital-input-action-cpp     userLimitDigitalInputAction.cpp

*  @brief        User Limit Digital Input Action sample application.

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
*  @include userLimitDigitalInputAction.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

void UserLimitDigitalInputActionMain()
{
    using namespace RSI::RapidCode;

    const int IO_NODE_NUMBER = 1;// which SqNode to use for I/O?
    const int AXIS_NUMBER = 3;// which Axis number to abort?
    const int USER_LIMIT = 0;// which user limit to use?
    const int CONDITION = 0;// which condition to use (0 or 1)

    const int INPUT_BIT_NUMBER = 7;// which input bit?
// RapidCode interface classes
    IO                    *io;
    IOPoint                *digitalInput;
    // Initialize MotionController class.
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.



        // initialize the I/O class
        io = controller->IOGet(IO_NODE_NUMBER);
        SampleAppsCPP::HelperFunctions::CheckErrors(io);

        digitalInput = IOPoint::CreateDigitalInput(io, INPUT_BIT_NUMBER);

        // configure user limit to evaluate input bit
        controller->UserLimitConditionSet(USER_LIMIT,
            CONDITION,
            RSIUserLimitLogic::RSIUserLimitLogicEQ,
            digitalInput->AddressGet(),
            digitalInput->MaskGet(),
            digitalInput->MaskGet());

        // enable the user limit, generate ESTOP_ABORT action when input is turned on
        controller->UserLimitConfigSet(USER_LIMIT, RSIUserLimitTriggerType::RSIUserLimitTriggerTypeSINGLE_CONDITION, RSIAction::RSIActionE_STOP_ABORT, AXIS_NUMBER, 0.0);

        printf("Waiting for the input bit to go high...\n");
        printf("\nPress Any Key To Exit.\n");

        // wait for user limit to trigger
        while (controller->OS->KeyGet(RSIWaitPOLL) < 0)
        {
            printf("User Limit state is %d\r", controller->UserLimitStateGet(USER_LIMIT));
            controller->OS->Sleep(1);
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

