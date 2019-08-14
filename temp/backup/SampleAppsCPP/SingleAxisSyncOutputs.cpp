/*! 
*  @example    SingleAxisSyncOutputs.cpp

*  @page       single-axis-sync-outputs-cpp SingleAxisSyncOutputs.cpp

*  @brief      Single Axis Sync Outputs sample application.

*  @details    This sample application will show you a basic demonstartion on how to set up Sync Outputs, so that you can easily change any IO’s state based on a specified point index (or ElmentID) on your steaming motion.

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

*  @include SingleAxisSyncOutputs.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void SingleAxisSyncOutputsMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int NODE_INDEX = 0;                       // Specify which EtherCat Node will be used
    const int AXIS_NUMBER = 0;                      // Specify which axis/motor to control.
    const int USER_UNITS = 1048576;                 // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution)       
    const int TOTAL_POINTS = 4;                     // total number of points
    const int EMPTY_CT = -1;                        // Number of points that remains in the buffer before an e-stop
    const int OUTPUT_INDEX = 0;

    double positions[] = { 1.0, 2.0, 3.0, 4.0 };    // These will be the streaming motion 5 positions.
    double times[] = { 0.5, 1.0, 2.0, 4.0 };        // These will be the streaming motion 5 positions' time. 
    int outputEnableID = 2;                         // The motion element ID at which to set the output
    int outputDisableID = 3;                        // The motion element ID at which to set the output

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        axis->PositionSet(0);                                                   // Make sure motor starts at position 0 everytime.
        axis->UserUnitsSet(USER_UNITS);                                         // Change your user units.
        axis->Abort();                                                          // If there is any motion happening, abort it.
        axis->ClearFaults();                                                    // Clear faults.>
        axis->AmpEnableSet(true);                                               // Enable the motor.

        // Set up the inputs
        // Ensure the digital out is set low.
        //IOPoint *output0 = IOPoint::CreateDigitalOutput(axis, RSIMotorGeneralIo.RSIMotorGeneralIo16);      // Retrieve DOUT 1, Method 1: requires you know the io adress in memory, slightly faster
        IOPoint *output0 = IOPoint::CreateDigitalOutput(controller->IOGet(NODE_INDEX), OUTPUT_INDEX);        // Retrieve DOUT 1  Method 2: only need to know node index
        output0->Set(false);

        // Set up Sync Outputs
        axis->StreamingOutputsEnableSet(true);                                                               // Enable streaming output.

        // ENABLE the Sync Output(s)
        axis->StreamingOutputAdd(output0, true, outputEnableID);                                             // This will turn DOUT1 High when the streaming motion reaches its 3rd motion point.
        axis->StreamingOutputAdd(output0, false, outputDisableID);                                           // This will turn DOUT1 Low when the streaming motion reaches its 4th motion point.

        // DISABLE the Sync Output(s)
        //axis->StreamingOutputAdd(output0, false, outPutEnableID);

        axis->MovePT(RSIMotionType::RSIMotionTypePT, positions, times, TOTAL_POINTS, EMPTY_CT, false, true);  // Start Streaming Motion
        printf("Motion started. Waiting to complete.\n");

        axis->MotionDoneWait();                                                                              // What for Streaming Motion to be done.
        printf("Motion Complete. The outputs should have been set\n");

        axis->StreamingOutputsEnableSet(false);                                                              // Disable Sync Outputs.
        axis->AmpEnableSet(false);                                                                           // Disable the motor.

    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


