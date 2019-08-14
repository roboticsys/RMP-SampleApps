/*! 
*  @example    Camming.cpp

*  @page       camming-cpp Camming.cpp

*  @brief      Camming sample application.

*  @details
*  This sample application allows you to command a nonlinear coordinated motion betweentwo axes.
<BR>Master Axis: this axis/motor may or may not be controlled by the motion controller.
<BR>Slave Axis: the motion controller controls the position of this axis/motor as a function of the position of the master axis->

<BR>NOTE: User Units must be set to 1!

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

*  @include Camming.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void CammingMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int USER_UNITS = 1048576;             // Specify your counts per unit / user units.                       (the motor used in this sample app has 1048576 encoder pulses per revolution) ("user unit = 1" means it will do one count out of the 1048576)   
    const int MASTER_AXIS_NUMBER = 0;           // Specify which is your master axis/motor.
    const int SLAVE_AXIS_NUMBER = 1;            // Specify which is your slave axis/motor.
    const double MASTER_VELOCITY = 2;           // Specify your master's axis velocity.   -   units: Units/Sec      (it will do 1 counts / (1/104857) of a revolution every 1 second.)
    const double MASTER_ACCELERATION = 10;      // Specify your master's acceleration.    -   units: Units/Sec^2


    // Commanded Positions
    //NOTE for caming positions are in ticks not user units. You will need to multiply by you USER_UNITS
    double masterDistances[] = { 5 * USER_UNITS, 20 * USER_UNITS, 15 * USER_UNITS };           // This is the RELATIVE master distance, every n revolutions it changes its slave position. (This is the x-axis)
    double slavePositions[] = { 10 * USER_UNITS, 20 * USER_UNITS, 10 * USER_UNITS };          // This is the final ABSOLUTE slave position, for every 5 revolutions one of this positions gets applied. (This is the y-axis)

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                            // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);        // [Helper Function] Initialize the network.
        controller->AxisCountSet(2);                                        // Set the number of axis being used. A phantom axis will be created if for any axis not on the network. You may need to refresh rapid setup to see the phantom axis.

        // initialize Axis class  
        Axis *master = controller->AxisGet(MASTER_AXIS_NUMBER);                // Initialize master Class. (Use RapidSetup Tool to see what is your axis number)
        Axis *slave = controller->AxisGet(SLAVE_AXIS_NUMBER);                // Initialize slave Class.
        SampleAppsCPP::HelperFunctions::CheckErrors(master);                // [Helper Function] Check that the master axis has been initialize correctly.
        SampleAppsCPP::HelperFunctions::CheckErrors(slave);                    // [Helper Function] Check that the slave axis has been initialize correctly.

        master->UserUnitsSet(USER_UNITS);                                   // Specify the counts per Unit.
        master->ErrorLimitTriggerValueSet(1000);                            // Specify the position error limit trigger. (Learn more about this on our support page)
        slave->UserUnitsSet(USER_UNITS);                                    // Specify the counts per Unit.
        slave->ErrorLimitTriggerValueSet(1000);                             // Specify the position error limit trigger. (Learn more about this on our support page)

        master->Abort();                                                    // If there is any motion happening, abort it.
        slave->Abort();

        master->ClearFaults();                                              // Clear faults.>
        slave->ClearFaults();

        master->AmpEnableSet(true);                                         // Enable the motor.
        slave->AmpEnableSet(true);

        master->PositionSet(0);                                             // this negates homing, so only do it in test/sample code.
        slave->PositionSet(0);

        // Command motion on the slave before the master starts
        slave->MoveCamLinear(master->NumberGet(),
            RSIAxisMasterType::RSIAxisMasterTypeAXIS_COMMAND_POSITION,       // This specified to use the actual position of the master axis->
            masterDistances,
            slavePositions,
            sizeof(masterDistances) / sizeof(double));

        master->MoveVelocity(MASTER_VELOCITY, MASTER_ACCELERATION);          // Command a constant velocity on the master axis, slave will follow.

        master->MotionDoneWait();                                             // Wait for the cam motion to complete

        master->Stop();                                                      // Stop the master

        master->AmpEnableSet(false);                                         // Disable the motor.
        slave->AmpEnableSet(false);

        printf("\nTest Complete\n");
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                               // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

