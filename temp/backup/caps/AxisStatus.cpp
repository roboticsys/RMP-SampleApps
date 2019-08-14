/*! 
*  @example    AxisStatus.cpp

*  @page       axis-status-cpp AxisStatus.cpp

*  @brief      Axis Status sample application.

*  @details
*  Learn how to use:
<BR>1) axis->StateGet
<BR>2) axis->SourceGet
<BR>3) axis->SourceNameGet
<BR>4) axis->StatusBitGet

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

*  @include AxisStatus.cpp
*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
#include <string>

using namespace RSI::RapidCode;

void PrintSource(Axis* axis, RSISource source)
{
    printf("\nThe source of the axis error is: %s ", axis->SourceNameGet(source));                  // SourceNameGet will return the first status bit which is set on an Axis or Multiaxis->
}

void PrintState(RSIState state)
{
    printf("\nYour Axis is in state: %i", state);
}

void AxisStatusMain()
{
    // Constants
    const int AXIS_NUMBER = 0;                // Specify which axis/motor to control.

    char rmpPath[] = "C:\\RSI\\X.X.X\\";
    // Initialize MotionController class.
    MotionController *controller = MotionController::CreateFromSoftware(/*rmpPath*/);
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);             // [Helper Function] Check that the controller has been initialized correctly.
    SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);         // [Helper Function] Initialize the network.

    Axis *axis = controller->AxisGet(AXIS_NUMBER);                                                 // Initialize the axis->
    SampleAppsCPP::HelperFunctions::CheckErrors(axis);             // [Helper Function] Check that the controller has been initialized correctly.

    try
    {
        // CHECK AXIS STATE
        RSIState state = axis->StateGet();                                                   // StateGet will return RSIState enum name of the current state of the Axis or Multiaxis-> (Ex: RSIStateERROR)

        RSISource source;                                                                   // Declare a RSISource variable.

        switch (state)
        {
        case RSIState::RSIStateIDLE:
        case RSIState::RSIStateMOVING:
            PrintState(state);
            break;
        case RSIState::RSIStateERROR:
        case RSIState::RSIStateSTOPPING_ERROR:
        case RSIState::RSIStateSTOPPED:
        case RSIState::RSIStateSTOPPING:
            source = axis->SourceGet();                                                  // SourceGet will return the RSISource enum name of the first status bit that is active. (Ex: RSISourceAMP_FAULT)
            PrintState(state);
            PrintSource(axis, source);
            break;
        default:
            printf("");
            break;
        }

        // or USE STATUS BIT GET

        bool isAmpFault_Active = axis->StatusBitGet(RSIEventType::RSIEventTypeAMP_FAULT);                   // StatusBitGet returns the state of a status bit, true or false.
        bool isPositionErrorLimitActive = axis->StatusBitGet(RSIEventType::RSIEventTypeLIMIT_ERROR);
        bool isHWNegativeLimitActive = axis->StatusBitGet(RSIEventType::RSIEventTypeLIMIT_HW_NEG);
        bool isHWPostiveLimitActive = axis->StatusBitGet(RSIEventType::RSIEventTypeLIMIT_HW_POS);           // This can be done for all RSIEventTypes

    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);                             // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


