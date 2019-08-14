/*! 
@example    customEtherCATHome.cpp

*  @page       custom-ethercat-home-cpp customEtherCATHome.cpp

*  @brief      Custom EtherCAT Home sample application.

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
*  @include customEtherCATHome.cpp
*/

#include "rsi.h"                                    // Import our RapidCode Library. 

using namespace RSI::RapidCode;

// user defined options
const int AXIS_NUMBER = 1;        // which axis?
const int NETWORK_INPUT_INDEX = 22;        // which network PDO input?
const int HOME_BIT_INDEX = 0;        // home input is which bit index?  (0-based)

const int VELOCITY = 80000;
const int ACCELERATION = 600000;
const int DECELERATION = 600000;



void PrintRapidCodeErrors(RapidCodeObject *rsiClass)
{
    RsiError *err;
    bool hasErrors = false;

    while (rsiClass->ErrorLogCountGet() > 0)
    {
        err = rsiClass->ErrorLogGet();
        printf("%s\n", err->text);
        hasErrors = true;
    }
    if (hasErrors)
        exit(1);
}


void customEtherCATHomeMain()
{

    uint64 networkAddress;
    char* inputName;



    // create MotionController class
    MotionController *controller = MotionController::CreateFromSoftware();
    PrintRapidCodeErrors(controller);

    // create Axis class  
    Axis *axis = controller->AxisGet(AXIS_NUMBER);
    PrintRapidCodeErrors(axis);


    try
    {
        axis->Abort();
        axis->ClearFaults();
        axis->AmpEnableSet(true);

        if (axis->StateGet() != RSIState::RSIStateIDLE)
        {
            printf("Error, should be in IDLE state.\n");
            exit(1);
        }

        inputName = controller->NetworkInputNameGet(NETWORK_INPUT_INDEX);
        printf("Input name is %s \n", inputName);
        networkAddress = controller->NetworkInputAddressGet(NETWORK_INPUT_INDEX);

        axis->HomeLimitCustomConfigSet(networkAddress, HOME_BIT_INDEX);



        axis->HomeActionSet(RSIActionSTOP);
        axis->HomeMethodSet(RSIHomeMethod::RSIHomeMethodRISING_HOME);
        axis->HomeVelocitySet(VELOCITY);
        axis->HomeSlowVelocitySet(VELOCITY);
        axis->HomeAccelerationSet(ACCELERATION);
        axis->HomeDecelerationSet(ACCELERATION);
        axis->HomeOffsetSet(0.0);
        axis->Home();

        if (axis->HomeStateGet() == true)
        {
            printf(" Homing successful\n");
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

