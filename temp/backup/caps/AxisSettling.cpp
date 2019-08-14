/*! 
*  @example    AxisSettling.cpp

*  @page       axis-settling-cpp AxisSettling.cpp

*  @brief      Axis Settling sample application.

*  @details
*  Configure the following characteristicpp for axis:
<BR>1) Fine Position Tolerance.
<BR>2) Coarse Position Tolerance.
<BR>3) Velocity Tolerance.
<BR>4) Settling Time Tolerance.

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

*  @include AxisSettling.cpp
*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 


static void PrintParameters(Axis *axis)
{
    printf("Fine Position Tolerance: %f  ", axis->PositionToleranceCoarseGet());       // Print fine position tolerance.
    printf("Coarse Position Tolerance: %f  ", axis->PositionToleranceCoarseGet());     // Print coarse position tolerance.
    printf("Velocity Tolerance: %f  ", axis->VelocityToleranceGet());                  // Print velocity tolerance.
    printf("Settling Time: %f   \n", axis->SettlingTimeGet());                         // Print settling time.
}

void AxisSettlingMain()
{
    using namespace RSI::RapidCode;

    const int AXIS_NUMBER = 0;                    // Specify which axis/motor to control.
    const int POSITION_TOLERANCE_FINE = 40;       // Specify the fine position tolerance.
    const int POSITION_TOLERANCE_COARSE = 5;      // Specify the coarse position tolerance.
    const int VELOCITY_TOLERANCE = 50;            // Specify the velocity tolerance.
    const int SETTLING_TIME = 10;                 // Specify the settling time.

    char rmpPath[] = "C:\\RSI\\X.X.X\\";          // Insert the path location of the RMP.rta (usually the RapidSetup folder)  
    // Initialize MotionController class.
    MotionController    *controller = MotionController::CreateFromSoftware(/*rmpPath*/);    // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                                // [Helper Function] Check that the axis has been initialize correctly. 
    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);         // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                       // Initialize the axis->
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                   // [Helper Function] Check that the axis has been initialized correctly.

        printf("Settling Criteria Example\n");
        printf("\nOld Criteria\n");

        PrintParameters(axis);                                               // Print current axis settling parameters.

        axis->PositionToleranceFineSet(POSITION_TOLERANCE_FINE);             // Set fine position tolerance.
        axis->PositionToleranceCoarseSet(POSITION_TOLERANCE_COARSE);         // Set coarse position tolerance.
        axis->VelocityToleranceSet(VELOCITY_TOLERANCE);                      // Set velocity tolerance.
        axis->SettlingTimeSet(SETTLING_TIME);                                // Set settling time.

        printf("\nNew Criteria\n");

        PrintParameters(axis);                                               // Print current axis settling parameters.
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                          // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

