/*! 
*  @example    UserLimitOneCondition.cpp

*  @page       user-limit-one-condition-cpp UserLimitOneCondition.cpp

*  @brief      One Condition Position User Limit sample application.

*  @details
This sample code shows how to configure the RMP controller's User Limits to compare an encoder position to a specific encoder position value.
If the (1 condition) pattern matches, then the specified output bit is activated (turns high) and the axis will cause an ABORT at the specified position.

<BR>In this example we configure a user limit to trigger when our axis reaches a specific position.
Once the specific position has been reached then our user limit will set the OUTPUT to high(1) as well as command an ABORT on the current motion.

<BR>The ENCODER VALUE is specified in UserLimitConditionSet()
<BR>The ABORT is specified in UserLimitConfigSet()
<BR>The OUTPUT is specified in UserLimitOutputSet()

<BR>In this example Beckhoff IO Terminals (Model EL1088 for Inputs and Model EL2008 for outputs) were used to control the Digital IO signals.

<BR>Make sure to check the correct digital IO signal indexes of your system in: RapidSetup --> Tools --> NetworkIO

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

*  @include UserLimitOneCondition.cpp
*/

#include "rsi.h"                                    // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

void UserLimitPositionOneConditionMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                      // Specify which axis/motor to control.
    const int POSITION = 5;                         // Specify the position to travel to.
    const int USER_UNITS = 1048576;                 // Specify your counts per unit / user units.           (the motor used in this sample app has 1048576 encoder pulses per revolution)      
    const int VELOCITY = 1;                         // Specify your velocity.       -   units: Units/Sec    (it will do 1048576 counts/1 revolution every 1 second.)
    const int ACCELERATION = 10;                    // Specify your acceleration.   -   units: Units/Sec^2
    const int DECELERATION = 10;                    // Specify your deceleration.   -   units: Units/Sec^2const int POSITION_INDEX = 0;                                               // This is the index of the input you will use to trigger the user limit.
    const int POSITION_INDEX = 0;                   // This is the index of the axis actual position that will go active when the user limit triggers.
    const int OUTPUT_INDEX = 0;                     // This is the index of the digital output that will go active when the user limit triggers.
    const int NODE_INDEX = 0;                       // The EtherCAT Node we will be communicating with

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);          // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                        // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                    // [Helper Function] Check that the axis has been initialize correctly.

        IOPoint *output0 = IOPoint::CreateDigitalOutput(controller->IOGet(NODE_INDEX), OUTPUT_INDEX); // Create an IOPoint class for a specified node and input index. Automatically gets memory address. 

        controller->UserLimitCountSet(2);                                     // Set the amount of UserLimits that you want to use. (every connected axis will automatically get 1 user limit)
        controller->InterruptEnableSet(true);                                 // Enable User Limit Interrupts. (When a user limit input comparison is met, and interrupt is triggered)

        //-------- PARAMETERS FOR UserLimitConditionSet --------
        int     userLimit = 1;                                                // Specify which user limit to use.
        int     condition = 0;                                                // Specify which condition to use (0 or 1) ("0" to compare 1 input || "1" to compare 2 inputs)
        RSIUserLimitLogic logic = RSIUserLimitLogic::RSIUserLimitLogicGE;                            // Logic for input value comparison. (Greater or equal)
        unsigned long   inputAddress = controller->NetworkInputAddressGet(POSITION_INDEX);           // 0 was the index of tha axis' Position Actual Value. (To check your IO indexes go to RapidSetup --> Tools --> NetworkIO) (This is the windows address)
        unsigned int     inputMask = 0XFFFFFFFF;
        unsigned int     limitValue = (unsigned int)controller->NetworkInputValueGet(POSITION_INDEX) + (1048576 * POSITION);     // Get the current encoder value, then add to it your axis encoder unit count times the number of revolutions you want your motor to do before stopping.

        // [1] Configure the input's trigger condition.
        controller->UserLimitConditionSet(userLimit,                                                      // (User Limit Index)   - Specify which user limit to use
            condition,                                                      // (Condition Number)   - Specify how many inputs you want to compare.
            logic,                                                          // (Comparison Logic)   - Specify the how the input value(s) will be compared
            inputAddress,                                                   // (Input Address)      - Specify the address of the input that will be compared.
            inputMask,                                                      // (Input Mask)         - Specify the bits in an address which need to be used when comparing inputs.
            limitValue);                                                    // (Limit Value)        - Specify the value to be compared with. 

        //-------- PARAMETERS FOR UserLimitConfigSet --------
        RSIUserLimitTriggerType triggerType = RSIUserLimitTriggerType::RSIUserLimitTriggerTypeSINGLE_CONDITION;
        RSIAction               action = RSIAction::RSIActionABORT;                                     // Abort move when user limit triggers.
        int                     duration = 0;

        // [2] Configure and Enable the user limit.
        controller->UserLimitConfigSet(userLimit,                                                          // (User Limit Index)   - Specify which user limit to use.
            triggerType,                                                        // (Trigger Type)       - Specify how your condition should be evalutated.
            action,                                                             // (User Limit Action)  - Specify the action you want to cause on the axis when the user limit triggers.
            AXIS_NUMBER,                                                        // (Current Axis)       - Specify the axis that the action (defined above) will occur on.
            duration);                                                          // (Output Timer)       - Specify the time delay before the action is executed after the User Limit has triggered.

        //-------- PARAMETERS FOR UserLimitOutputSet --------
        unsigned int andMask = output0->MaskGet();                              // Get the appropriate mask from your IOpoint. OR use 0x00010000 for AKD General IO, 1 for Beckhoff IO Terminals   
        unsigned int     orMask = output0->MaskGet();                           // Get the appropriate mask from your IOpoint. OR use 0x00010000 for AKD General IO, 1 for Beckhoff IO Terminals   
        unsigned long   outputAddress = output0->AddressGet();                  //Alternatively set manually using: controller->NetworkOutputAddressGet(OUTPUT_INDEX);
        bool    enableOutput = true;

        // [3] Configure what the output will be.                                                              (Call this method after UserLimitConfigSet if you want an output to be triggered) (The output will only be triggered if the input conditions are TRUE.)
        controller->UserLimitOutputSet(userLimit,                                                          // (User Limit Index)   - Specify which user limit to use.
            andMask,                                                            // (Logic AND Mask)     - Specify the value that the digital output will be AND-ed with.
            orMask,                                                             // (Logic OR Mask)      - Specify the value that the digital output will be OR-ed with.
            outputAddress,                                                      // (Output Address)     - Specify the digital output address.
            enableOutput);                                                      // (Enable Output Set)  - If TRUE, the output AND-ing and OR-ing will be executed when the User Limit triggers.

        printf("Waiting for axis to reach specified position\n");

        axis->UserUnitsSet(USER_UNITS);                                                                      // Specify the counts per Unit.
        axis->ErrorLimitTriggerValueSet(1);                                                                  // Specify the position error limit trigger. (Learn more about this on our support page)
        axis->PositionSet(0);                                                                                // Make sure motor starts at position 0 everytime.
        axis->Abort();                                                                                       // If there is any motion happening, abort it.
        axis->ClearFaults();                                                                                 // Clear faults.>
        axis->AmpEnableSet(true);                                                                            // Enable the motor.

        axis->MoveTrapezoidal(POSITION, VELOCITY, ACCELERATION, DECELERATION);                               // Command simple trapezoidal motion.

        // Wait for user limit to trigger.
        while (controller->InterruptWait((int)RSIWait::RSIWaitFOREVER) != RSIEventType::RSIEventTypeUSER_LIMIT)
        {
        }

        printf("User Limit %i triggered!\n", controller->InterruptSourceNumberGet());                        // Get the index of the user limit that triggered.


        printf("User Limit %i triggered!\n", controller->InterruptSourceNumberGet());                        // Get the index of the user limit that triggered.

        axis->AmpEnableSet(false);                                                                           // Disable the motor.
        controller->UserLimitDisable(userLimit);                                                             // Disable User Limit.
        system("pause");                                                                                     // Allow time to read Console.
        output0->Set(false);                                                                                 // Set output low so program can run again

    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


