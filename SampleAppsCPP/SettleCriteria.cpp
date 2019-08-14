/*! 
@example    settleCriteria.cpp

*  @page       settle-criteria-cpp settleCriteria.cpp

*  @brief      Settle Criteria sample application.

*  @details
Configure the following characteristicpp for axis:

1) Fine Position Tolerance
2) Coarse Position Tolerance
3) Velocity Tolerance
4) Settling Time Tolerance

Once you execute this sample application, changes in values can be verified from RapidSetup in the "Position and Trajectory Status."

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
*  @include settleCriteria.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

const int AXIS_NUMBER = 0;
const int POSITION_TOLERANCE_FINE = 200;
const int POSITION_TOLERANCE_COARSE = 300;
const int VELOCITY_TOLERANCE = 12000;
const int SETTLING_TIME = 5;


void PrintResult(Axis *axis, char * Vals)
{
    printf("\r%s", Vals);
    printf("\nPosition Tolerance Fine  :   %lf", axis->PositionToleranceFineGet());
    printf("\nPosition Tolerance Coarse:   %lf", axis->PositionToleranceCoarseGet());
    printf("\nVelocity Tolerance       :   %lf", axis->VelocityToleranceGet());
    printf("\nSettling Time            :   %lf\n\n", axis->SettlingTimeGet());
}


void settleCriteriaMain()
{

    MotionController        *controller;
    Axis                *axis;
    try
    {
        // Initialize MotionController class.
        controller = MotionController::CreateFromSoftware();
        SampleAppsCPP::HelperFunctions::CheckErrors(controller);

        // initialize Axis class  
        axis = controller->AxisGet(AXIS_NUMBER);
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);

        axis->AmpEnableSet(false);

        PrintResult(axis, "OLD VALUES \n------------");

        //setting new values to Pos_Tol_Fine, Pos_Tol_Coarse, Vel_Tol, Settling_Time 
        axis->PositionToleranceFineSet(POSITION_TOLERANCE_FINE);
        axis->PositionToleranceCoarseSet(POSITION_TOLERANCE_COARSE);
        axis->VelocityToleranceSet(VELOCITY_TOLERANCE);
        axis->SettlingTimeSet(SETTLING_TIME);

        PrintResult(axis, "NEW Values \n------------");
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}
