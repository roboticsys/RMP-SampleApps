/*! 
@example    configAmpFault.cpp

*  @page       config-amp-fault-cpp configAmpFault.cpp

*  @brief      Config Amp Fault sample application.

*  @details
The program will configure the amp fault input for a motor.

A motor's amp fault input has three items to configure:

1) Event Action   (any MPIAction such as MPIActionE_STOP)
2) Event Trigger  (a trigger polarity, active HIGH or LOW)
3) Duration       (requires the limit condition to exist for a programmable number of seconds before an event will occur)

The duration described above is a configuration that provides filtering of the amp fault input by requiring the input to remain active for the defined duration.
This will effectively keep the amp fault from activating prematurely due to electrical noise on the amp fault input.

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
*  @include configAmpFault.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;



void configAmpFaultMain()
{
    const int AXIS_NUMBER = 0;
    const int ACTIVE_HIGH = 1;
    const int ACTIVE_LOW = 0;
    const int ENABLED = 1;
    const int DISABLED = 0;
    const int AMP_FAULT_DURATION_TIME = 1; //value in seconds

    // initialize RsiController class
    MotionController *controller = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);

    // initialize RsiAxis class  
    Axis *axis = controller->AxisGet(AXIS_NUMBER);
    SampleAppsCPP::HelperFunctions::CheckErrors(axis);

    axis->AmpEnableSet(false);
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        //To change AMP_FAULT characteristicpp:
        axis->AmpFaultActionSet(RSIActionABORT);
        printf("\n AMP_FAULT Action set to ABORT\n");

        axis->AmpFaultTriggerStateSet(ACTIVE_LOW);
        printf("\n AMP_FAULT Trigger State set to ACTIVE_LOW\n");

        axis->AmpFaultDurationSet(AMP_FAULT_DURATION_TIME);
        printf("\n AMP_FAULT Duration set to 1.0 second\n");
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

