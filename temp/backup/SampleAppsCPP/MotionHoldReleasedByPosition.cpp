/*! 
*  @example    MotionHoldReleasedByPosition.cpp

*  @page       motion-hold-released-by-position-cpp MotionHoldReleasedByPosition.cpp

*  @brief      Motion Hold Move Released By Position sample application.

*  @details    This sample code is done in AKD Drive with one Actual Axis and one Phantom axis-> It can be applied to two Phatom Axis or two Actual Axis with the slight changes of code which is guided in comment.

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
* @include MotionHoldReleasedByPosition.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
#include <thread>                                   // std::this_thread::sleep_for

void MotionHoldReleasedByPositionMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int       HOLDING_AXIS_INDEX = 1;    // Specify which axis/motor to control.
    const int       MOVING_AXIS_INDEX = 0;     // Specify which axis/motor to control.
    const double    TRIGGER_POS = 5;           // Specify the position that will be evaluted on triggering/releasing motion
    const int       USER_UNITS = 1048576;      // Specify USER UNITS

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.
        controller->AxisCountSet(2);                                            // Set the number of axis being used. A phantom axis will be created if for any axis not on the network. You may need to refresh rapid setup to see the phantom axis.

        // initialize Axis class  
        Axis *holdingAxis = controller->AxisGet(HOLDING_AXIS_INDEX);            // Initialize master Class. (Use RapidSetup Tool to see what is your axis number)
        Axis *movingAxis = controller->AxisGet(MOVING_AXIS_INDEX);              // Initialize slave Class.

        SampleAppsCPP::HelperFunctions::CheckErrors(holdingAxis);               // [Helper Function] Check that the master axis has been initialize correctly.
        SampleAppsCPP::HelperFunctions::CheckErrors(movingAxis);                // [Helper Function] Check that the slave axis has been initialize correctly.

        // GET HOLD AXIS READY
        holdingAxis->UserUnitsSet(USER_UNITS);                                           // Specify the counts per Unit.
        holdingAxis->PositionSet(0);                                                     // Make sure motor starts at position 0 everytime.
        holdingAxis->ErrorLimitTriggerValueSet(1000);                                    // Specify the position error limit trigger. (Learn more about this on our support page)
        holdingAxis->Abort();                                                            // If there is any motion happening, abort it.
        holdingAxis->ClearFaults();                                                      // Clear faults.>
        holdingAxis->AmpEnableSet(true);                                                 // Enable the motor.

        // GET MOVE AXIS READY
        movingAxis->ErrorLimitActionSet(RSIAction::RSIActionNONE);                       // If NOT using a Phantom Axis, switch to RSIActionABORT
        movingAxis->UserUnitsSet(USER_UNITS);                                            // Specify the counts per Unit.
        movingAxis->PositionSet(0);                                                      // Make sure motor starts at position 0 everytime.
        movingAxis->ErrorLimitTriggerValueSet(1000);                                     // Specify the position error limit trigger. (Learn more about this on our support page)
        movingAxis->Abort();                                                             // If there is any motion happening, abort it.
        movingAxis->ClearFaults();                                                       // Clear faults.>
        movingAxis->AmpEnableSet(true);                                                  // Enable the motor.

        // SET UP MOTION HOLD                                                                        // Condition/Configuration to the Axis(movingAxis) that will hold Motion and its Position that will trigger/release motion
        holdingAxis->MotionHoldTypeSet(RSIMotionHoldType::RSIMotionHoldTypeAXIS_COMMAND_POSITION);   // Use RSIMotionHoldTypeAXIS_ACTUAL_POSITION if it is not Phantom axis->
        holdingAxis->MotionHoldAxisNumberSet(movingAxis->NumberGet());                               // Specify motion hold to the Axis(movingAxis) whose position will hold the motion of holdingAxis->
        holdingAxis->MotionHoldAxisPositionSet(USER_UNITS * TRIGGER_POS);                            // Specify motion hold position which is the movingAxis's position(need to multiply with USER_UNITS to get correct position value) to trigger/release the motion of holdingAxis->
        holdingAxis->MotionHoldAxisLogicSet(RSIUserLimitLogic::RSIUserLimitLogicGE);                 // Specify the logic condition that will be evaluated to trigger/release motion based on the SET POSITION(USER_UNITS * TRIGGER_POS).In this case, GT(Greater than or Equal to) motion hold position limit to release

        // SET MOTION HOLD ON
        holdingAxis->MotionAttributeMaskOnSet(RSIMotionAttrMask::RSIMotionAttrMaskHOLD);             // Set the HOLD motion attribute mask ON. (This initializes the HOLD on a particular motion)
        holdingAxis->MoveRelative(10);                                                               // Command simple relative motion(This motion will be hold by condition above until movingAxis's Position passes motion hold position limit)

        // Release MOTION HOLD
        movingAxis->MoveRelative(10);                                             // Move movingAxis->MovingAxis's position reaches its MotionHold Position limit(in this case, limit is 5). It will trigger/release motion on holdingAxis (holidingAxis will move relatively 10).


        //movingAxis->MotionDoneWait();                                           // Wait for motion to be completed. Uncomment when not using phantom axis.
        //holdingAxis->MotionDoneWait();                                          // Wait for motion to be completed. Uncomment when not using phantom axis.
        std::this_thread::sleep_for(std::chrono::seconds(1));                     // Allow time motion to be completed. Use MotionDoneWait() when not using phantom axis

        // Abort and Clear Faults 
        holdingAxis->Abort();
        holdingAxis->ClearFaults();
        movingAxis->Abort();
        movingAxis->ClearFaults();
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                               // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

