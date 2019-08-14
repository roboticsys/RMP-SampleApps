/*! 
*  @example    Gearing.cpp

*  @page       gearing-cpp Gearing.cpp

*  @brief      Gearing sample application.

*  @details
Axis gearing on the XMP is based off of a Axis, RSIAxisMasterType, numerator, and denominator.
<BR>The Axis points to a master axis to gear to.
<BR>The RSIAxisMasterType specifies what feedback source to gear to. The ratio between the lead and follower axes is set by a ratio of two longs -- a numerator and a denominator.

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

*  @include Gearing.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

void GearingMain()
{
    // RapidCode objects
    // Declare what 'controller' is
    Axis *master;                            // Declare what 'master' is
    Axis *slave;                             // Declare what 'slave' is

    // Constants
    const int    MASTER_AXIS = 0;            // Specify which axis will be the master axis
    const int    SLAVE_AXIS = 1;             // Specify which axis will be the slave axis
    const int    USER_UNITS = 1048576;       // Specify your counts per unit / user units.   (the motor used in this sample app has 1048576 encoder pulses per revolution) 
    const int    POSITION1 = 50;             // Specify the position to travel to.
    const int    POSITION2 = 0;              // Specify the position to travel to.
    const double VELOCITY = 10;              // Specify your velocity - units: units/sec     (it will do (1048576*10)counts/10-revolutions every 1 second.)
    const double ACCELERATION = 100;         // Specify your acceleration - units: units/sec^2
    const double DECELERATION = 100;         // Specify your deceleration - units: units/sec^2
    const double JERK_PERCENT = 50;          // Specify your jerk percentage
    
    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);                      // [Helper Function] Initialize the network.
        controller->AxisCountSet(2);                                                      // Set the number of axis being used. A phantom axis will be created if for any axis not on the network. You may need to refresh rapid setup to see the phantom axis.

        Axis *master = controller->AxisGet(MASTER_AXIS);                                  // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        Axis *slave = controller->AxisGet(SLAVE_AXIS);                                    // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(master);                              // [Helper Function] Check that the axis has been initialize correctly.
        SampleAppsCPP::HelperFunctions::CheckErrors(slave);                               // [Helper Function] Check that the axis has been initialize correctly.

        master->UserUnitsSet(USER_UNITS);                                                 // Specify the counts per Unit.
        master->ErrorLimitTriggerValueSet(1);                                             // Specify the position error limit trigger. (Learn more about this on our support page)
        slave->UserUnitsSet(USER_UNITS);
        slave->ErrorLimitTriggerValueSet(1);
        slave->ErrorLimitActionSet(RSIAction::RSIActionNONE);                             // If NOT using a Phantom Axis, switch to RSIActionABORT

        master->Abort();                                                                  // If there is any motion happening, abort it.
        slave->Abort();

        master->ClearFaults();                                                            // Clear faults.>
        slave->ClearFaults();

        master->AmpEnableSet(true);                                                       // Enable the motor.
        slave->AmpEnableSet(true);

        master->PositionSet(0);                                                           // Zero the position (in case the program is run multiple times)
        slave->PositionSet(0);                                                            // this negates homing, so only do it in test/sample code.

        printf("Gearing Example");

        int numerator = 2;                                                                // Specify the numerator of the gearing ratio.
        int denominator = 1;                                                              // Specify the denominator of the gearing ratio.

        // Configure the 'slave' axis to be a slave to the 'master' axis at a ratio of 2:1, that is,
        // for every rotation of the master axis, the slave axis will rotate twice.
        slave->GearingEnable(master,
            RSIAxisMasterType::RSIAxisMasterTypeAXIS_COMMAND_POSITION,    // If NOT using a Phantom Axis, switch to RSIAxisMasterTypeAXIS_ACTUAL_POSITION
            numerator,
            denominator);

        printf("\nTesting for Gearing Ratio %i/%i\n", numerator, denominator);

        master->MoveSCurve(POSITION1,
            VELOCITY,
            ACCELERATION,
            DECELERATION,
            JERK_PERCENT);                                                              // Perform a S-curve motion on the master axis->

        master->MotionDoneWait();                                                       // Wait for motion to finish.

        // Test changing the gearing ratio to -1:1, that is,
        // for every rotation of the master axis, the slave axis will rotate once in the opposite direction
        numerator = -1;
        denominator = 1;

        slave->GearingRatioChange(numerator, denominator);                              // Change the electronic gearing ratio

        printf("\nTesting for Gearing Ratio %i/%i\n", numerator, denominator);

        master->MoveSCurve(POSITION2,                                                   // New Position.
            VELOCITY,
            ACCELERATION,
            DECELERATION,
            JERK_PERCENT);                                                              // Perform a S-curve motion on the master axis again.

        master->MotionDoneWait();                                                       // Wait for motion to finish.

        printf("\nDisable Gearing\n");

        slave->GearingDisable();                                                        // Disable gearing on the slave->
        master->AmpEnableSet(true);                                                     // Disable the motor.
        slave->AmpEnableSet(true);

        printf("\nTest Complete\n");
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                                  // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

