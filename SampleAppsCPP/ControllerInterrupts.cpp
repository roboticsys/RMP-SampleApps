/*! 
@example    controllerInterrupts.cpp

*  @page       controller-interrupts-cpp controllerInterrupts.cpp

*  @brief      Controller Interrupts sample application.

*  @details

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
*  @include controllerInterrupts.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

void controllerInterruptsMain()
{
    const int TIMEOUT = (5000);  //ms


    long interruptType;

    // Initialize Controller, enable interrupts
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);

    controller->InterruptEnableSet(true);
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        while (controller->OS->KeyGet(RSIWaitPOLL) < 0)
        {
            // add code here to generate interrupts (move axes, etc.)

            // wait for an interrupt
            interruptType = controller->InterruptWait(TIMEOUT);

            if (interruptType != RSIEventTypeTIMEOUT)
            {
                printf("IRQ %ld\n", interruptType);
                printf("%s\n", controller->InterruptNameGet());
                printf("InterruptSourceNumber = %ld\n", controller->InterruptSourceNumberGet());
                printf("InterruptSampleTimer = %ld\n", controller->InterruptSampleTimeGet());
                printf("\n");
            }
            else
            {
                printf("Timeout waiting for interrupts...\n");
            }
        }
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

