/*! 
*  @example    PTmotionWhileStopping.cs

*  @page       pt-motion-while-stopping-cs PTmotionWhileStopping.cs

*  @brief      PT Motion While Stopping sample application.

*  @details    This application demonstrates that different actions that you can take after stopping a PT Streaming Motion.

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

*  @include PTmotionWhileStopping.cs
*


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void PTmotionWhileStoppingMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;         // Specify which axis/motor to control.
    const int POINTS = 1;              // Specify how many points per streaming motion method call.
    const int USER_UNITS = 1048576;    // Specify your counts per unit / user units.           (the motor used in this sample app has 1048576 encoder pulses per revolution)       

    double first[] = { 10.0 };         // specify the array with position(s) that will go in the first MovePT call.
    double second[] = { 20.0 };        // specify the array with position(s) that will go in the second MovePT call
    double third[] = { 7.0 };          // specify the array with position(s) that will go in the third MovePT call

    double time1[] = { 5.0 };          // specify the array with time(s) that will go in the first MovePT call.
    double time2[] = { 2.0 };          // specify the array with time(s) that will go in the second MovePT call.
    double time3[] = { 5.0 };          // specify the array with time(s) that will go in the third MovePT call.

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);           // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                         // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                     // [Helper Function] Check that the axis has been initialize correctly.

        axis->UserUnitsSet(USER_UNITS);
        axis->ErrorLimitTriggerValueSet(1);                                                         // Specify the position error limit trigger. (Learn more about this on our support page)
        axis->PositionSet(0);

        axis->Abort();                                                                              // If there is any motion happening, abort it.
        axis->ClearFaults();                                                                        // Clear faults.>
        axis->AmpEnableSet(true);                                                                   // Enable the motor.

        axis->MovePT(RSIMotionType::RSIMotionTypePT, first, time1, POINTS, -1, false, false);       // Start motion and stream a point.
        axis->MovePT(RSIMotionType::RSIMotionTypePT, second, time2, POINTS, -1, false, false);      // Append a point.

        // After you stop a streaming motion, you can do 2 things. 
        // 1. You can either RESUME motion and continue to append. 
        // 2. Or you can DISCARD the points before the stop and start new motion.

        axis->Stop();                                                                               // Calling Stop() after streaming motion will put you in an STOPPED state.

        //axis->EStop();                                                                            // Calling EStop() after streaming motion will put you in an ERROR state. 
        // Therefore, you must ClearFaults() before taking any action.

        //axis->Abort();                                                                            // Calling Abort() after streaming motion will put you in an ERROR state and trigger an AmpEnable(false). 
        // Therefore, you must call AmpEnable(true) before taking action.

        axis->MotionDoneWait();                                                                     // Wait for axis to come to a stop.
        axis->Resume();                                                                             // Resume() is the key here. 
        // If you include Resume() you will append all next streaming points to the old ones.
        // If you do not include Resume() you will discard all previous points and start with new motion with the new points.

        axis->MovePT(RSIMotionType::RSIMotionTypePT, third, time3, POINTS, -1, false, true);        // Depending on whether you include Resume() or not this point will be appended or will be starting motion.
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);                          // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

