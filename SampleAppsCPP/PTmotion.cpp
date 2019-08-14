/*! 
*  @example    PTmotion.cpp

*  @page       pt-motion-cpp PTmotion.cpp

*  @brief      PT Simple Motion sample application.

*  @details    This application demonstrates how to use PT Motion.
PT motion is the simplest streaming motion to use because it only requires an array of Positions and Time deltas.
The controller is responsible for figuring out the velocity, and acceleration for each motion segment.

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

*  @include PTmotion.cs
*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 


void PTmotionMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                      // Specify which axis/motor to control.
    const int USER_UNITS = 1048576;                 // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution)  Resolution = 2^20 = 1048576 Counts/MtrRev (set by AKD drive)

    int points = 3;                                 // Specify the total number of streamed points.
    int emptyCount = -1;                            // E-stop generated if there are this number or fewer frames loaded. Use the value "0" to trigger an E-stop when the buffer is empty, or “-1” to not trigger an E-stop. (Typically for PT motion there are two frames per PT point)

    double positions[] = { 2.0, 5.0, 0.0 };         // Specify the positions that you want to reach. (it can be n number)
    double times[] = { 2.0, 3.0, 1.0 };             // Specify the times in which you want to reach each position. (velocity and acceleration is calculated by the RMP)

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                         // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        axis->UserUnitsSet(USER_UNITS);                                         // Specify the counts per Unit.
        axis->PositionSet(0);                                                   // Make sure motor starts at position 0 everytime.
        axis->ErrorLimitTriggerValueSet(1);                                     // Set the position error trigger value
        axis->Abort();                                                          // If there is any motion happening, abort it.
        axis->ClearFaults();                                                    // Clear faults.
        axis->AmpEnableSet(true);                                               // Enable the motor.

        axis->MovePT(RSIMotionType::RSIMotionTypePT,                            // Specify the type of PT Motion that you want to perform. (RSIMotionType.RSIMotionTypePT, RSIMotionType.RSIMotionTypeBSPLINE, RSIMotionType.RSIMotionTypeBSPLINE2)
            positions,                                                          // Specify the positions that you want to reach. (it can be n number)
            times,                                                              // Specify the times in which you want to reach each position. (velocity and acceleration is calculated by the RMP)
            points,                                                             // Specify the total number of streamed points.
            emptyCount,                                                         // E-stop generated if there are this number or fewer frames loaded. Use the value “0” to trigger an E-stop when the buffer is empty, or “-1” to not trigger an E-stop. (Typically for PT motion there are two frames per PT point)
            false,                                                              // Specify whether points are kept, or are not kept.
            true);                                                              // Specify if this is the last MovePT. (If True, this is the final point. If False, more points expected.)

        axis->MotionDoneWait();                                                 // Wait for motion to be completed.
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order. 
}
