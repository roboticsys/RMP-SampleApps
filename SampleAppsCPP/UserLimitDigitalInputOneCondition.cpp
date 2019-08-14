/*!
 *  @example    OneConditionDigitalInputUserLimit.cs

 *  @page       user-limit-one-condition-cs OneConditionDigitalInputUserLimit.cs

 *  @brief      One Condition Digital Input User Limit sample application.

 *  @details
    This sample code shows how to configure the RMP controller's User Limits to compare an input bit to a specific signal (high signal (1) OR low signal (0)).
    If the (1 condition) pattern matches, then the specified output bit is activated (turns high).

    <BR>In this example we configure a user limit to trigger when our INPUT turns high(1). Once the INPUT turns high(1) then our user limit will set the OUTPUT to high(1).

        The INPUT is specified in UserLimitConditionSet()
        The OUTPUT is specified in UserLimitOutputSet()

    <BR>In this example Beckhoff IO Terminals (Model EL1088 for Inputs and Model EL2008 for outputs) were used to control the Digital IO signals.

    <BR>Make sure to check the correct digital IO signal indexes of your system in: RapidSetup -.
 Tools -.
 NetworkIO

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

 *  @include OneConditionDigitalInputUserLimit.cs
 */

#include "rsi.h"                                    // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

void UserLimitDigitalInputOneConditionMain()
{

    // Constants
    const int NODE_INDEX = 0;
    const int INPUT_INDEX = 0;            // This is the index of the digital input you will use to trigger the user limit.
    const int OUTPUT_INDEX = 0;           // This is the index of the digital output that will go active when the user limit triggers.

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);          // [Helper Function] Initialize the network.

        IOPoint *input0 = IOPoint::CreateDigitalInput(controller->IOGet(NODE_INDEX), INPUT_INDEX);    // Create IOPoint object. Can be used to automatically get the address of a specified node and input index
        IOPoint *output0 = IOPoint::CreateDigitalOutput(controller->IOGet(NODE_INDEX), OUTPUT_INDEX); // Create IOPoint object. Can be used to automatically get the address of a specified node and input index

        //-------- PARAMETERS FOR UserLimitConditionSet --------
        int userLimit = 1;                                                        // Specify which user limit to use.
        int condition = 0;                                                        // Specify which condition to use (0 or 1) ("0" to compare 1 input || "1" to compare 2 inputs)
        RSIUserLimitLogic logic = RSIUserLimitLogic::RSIUserLimitLogicEQ;         // Logic for input value comparison.
        unsigned long inputAddress = input0->AddressGet();                        // Use IOPoint or manually set using controller->NetworkInputAddressGet(INPUT_INDEX); for Beckhoff 10 was the index of 1st input. (To check your IO indexes go to RapidSetup -.

        unsigned long firmwareAddress = controller->FirmwareAddressGet(inputAddress);

        unsigned int test = (unsigned int)input0->MaskGet();
        unsigned int inputMask = (unsigned int)input0->MaskGet();                 // Get the appropriate mask from your IOpoint. OR use 0x00010000 for AKD General IO, 1 for Beckhoff IO Terminals   
        unsigned int limtValue = (unsigned int)input0->MaskGet();                 // Get the appropriate mask from your IOpoint. OR use 0x00010000 for AKD General IO, 1 for Beckhoff IO Terminals   

        // [1] Configure the input's trigger condition.
        controller->UserLimitConditionSet(userLimit,                               // (User Limit Index)   - Specify which user limit to use
            condition,                            // (Condition Number)   - Specify how many inputs you want to compare.
            logic,                                // (Comparison Logic)   - Specify the how the input value(s) will be compared
            inputAddress,                         // (Input Address)      - Specify the address of the input that will be compared.
            inputMask,                            // (Input Mask)         - Specify the bits in an address which need to be used when comparing inputs.
            limtValue);                           // (Limit Value)        - Specify the value to be compared with. 

        //-------- PARAMETERS FOR UserLimitConfigSet --------
        RSIUserLimitTriggerType triggerType = RSIUserLimitTriggerType::RSIUserLimitTriggerTypeSINGLE_CONDITION;
        RSIAction action = RSIAction::RSIActionNONE;
        int axis = 0;
        int duration = 0;

        // [2] Configure and Enable the user limit.
        controller->UserLimitConfigSet(userLimit,                                  // (User Limit Index)   - Specify which user limit to use.
            triggerType,                              // (Trigger Type)       - Specify how your condition should be evalutated.
            action,                                   // (User Limit Action)  - Specify the action you want to cause on the axis when the user limit triggers.
            axis,                                     // (Current Axis)       - Specify the axis that the action (defined above) will occur on.
            duration);                                // (Output Timer)       - Specify the time delay before the action is executed after the User Limit has triggered.


        //-------- PARAMETERS FOR UserLimitOutputSet --------
        unsigned int andMask = (unsigned int)output0->MaskGet();                                   // Get the appropriate mask from your IOpoint. OR use 0x00010000 for AKD General IO, 1 for Beckhoff IO Terminals   
        unsigned int orMask = (unsigned int)output0->MaskGet();                                    // Get the appropriate mask from your IOpoint. OR use 0x00010000 for AKD General IO, 1 for Beckhoff IO Terminals   
        unsigned long outputAddress = output0->AddressGet();                               //Alternatively set manually using: controller->NetworkOutputAddressGet(OUTPUT_INDEX);
        bool enableOutput = true;

        // [3] Configure what the output will be.                                 (Call this method after UserLimitConfigSet if you want an output to be triggered) (The output will only be triggered if the input conditions are TRUE.)
        controller->UserLimitOutputSet(userLimit,                                  // (User Limit Index)   - Specify which user limit to use.
            andMask,                                  // (Logic AND Mask)     - Specify the value that the digital output will be AND-ed with.
            orMask,                                   // (Logic OR Mask)      - Specify the value that the digital output will be OR-ed with.
            outputAddress,                            // (Output Address)     - Specify the digital output address.
            enableOutput);                            // (Enable Output Set)  - If TRUE, the output AND-ing and OR-ing will be executed when the User Limit triggers.

        printf("Waiting for the input bit to go high...\n");

        // Wait for user limit to trigger.
        while (controller->InterruptWait((int)RSIWait::RSIWaitFOREVER) != RSIEventType::RSIEventTypeUSER_LIMIT)
        {
        }

        printf("User Limit %i triggered!", controller->InterruptSourceNumberGet());      // Get the index of the user limit that triggered.

        controller->UserLimitDisable(userLimit);                                         // Disable User Limit.
        printf("\nPress Any Key To Continue");                                           // Allow time to read Console.
        system("pause");                                                                 // Allow time to read Console.
        output0->Set(false);                                                             // Disable User Limit.
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

