/*! 
*  @example    SetUserUnits.cpp

*  @page       set-user-unit-cs SetUserUnits.cs

*  @brief      Set User Units sample application.

*  @details    This application demonstrates how to set User Units.

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

*  @include SetUserUnits.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void SetUserUnitsMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int    AXIS_NUMBER = 0;         // Specify which axis/motor to control.
    const int    USER_UNITS = 1048576;    // Specify your counts per unit/user units.             (the motor used in this sample app has 1048576 encoder pulses per revolution) ("user unit = 1" means it will do one count out of the 1048576)  

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        axis->UserUnitsSet(1);                                                  // SET YOUR USER UNITS!
        axis->ErrorLimitTriggerValueSet(1);                                     // Specify the position error limit trigger. (Learn more about this on our support page)
        axis->PositionSet(0);                                                   // Make sure motor starts at position 0 everytime.

        double currentUserUnits = axis->UserUnitsGet();                         // Verify that your user units were changed!

        printf("The axis current user unit is: %f\n", currentUserUnits);

        axis->UserUnitsSet(USER_UNITS);                                         // SET YOUR USER UNITS!
        axis->ErrorLimitTriggerValueSet(1);                                     // Specify the position error limit trigger. (Learn more about this on our support page)
        axis->PositionSet(0);                                                   // Make sure motor starts at position 0 everytime.

        currentUserUnits = axis->UserUnitsGet();                                // Verify that your user units were changed!

        printf("The axis current user unit is: %f\n", currentUserUnits);
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


