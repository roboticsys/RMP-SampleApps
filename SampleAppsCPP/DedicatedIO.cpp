/*! 
*  @example    DedicatedIO.cpp

*  @page       dedicated-io-cpp DedicatedIO.cpp

*  @brief      Dedicated IO sample application.

*  @details    This application demonstrates how to access Dedicated IO. See the Dedicated I/O topic page for additional information.

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

*  @include DedicatedIO.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

void DedicatedIOMain()
{
    // Constants
    const int AXIS_NUMBER = 0;                                                  // Specify the axis that will be used.

    printf("Dedicated Inputs:");
    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                            // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.
        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // initialize Axis class  
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        printf("Axis %i:\n", AXIS_NUMBER);
        // Retrieve dedicated inputs with generic and specific function.
        printf("RSIMotorDedicatedInLIMIT_HW_NEG: %i and %i\n",
            axis->DedicatedInGet(RSIMotorDedicatedIn::RSIMotorDedicatedInLIMIT_HW_NEG),
            axis->NegativeLimitGet());

        printf("RSIMotorDedicatedInLIMIT_HW_POS: %i and %i\n",
            axis->DedicatedInGet(RSIMotorDedicatedIn::RSIMotorDedicatedInLIMIT_HW_POS),
            axis->PositiveLimitGet());

        printf("RSIMotorDedicatedInHOME: %i and %i\n",
            axis->DedicatedInGet(RSIMotorDedicatedIn::RSIMotorDedicatedInHOME),
            axis->HomeSwitchGet());

        printf("RSIMotorDedicatedInAMP_FAULT: %i and %i\n",
            axis->DedicatedInGet(RSIMotorDedicatedIn::RSIMotorDedicatedInAMP_FAULT),
            axis->AmpFaultGet());

        printf("RSIMotorDedicatedInAMP_ACTIVE: %i and %i\n",
            axis->DedicatedInGet(RSIMotorDedicatedIn::RSIMotorDedicatedInAMP_ACTIVE),
            axis->AmpEnableGet());
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                                  // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}
