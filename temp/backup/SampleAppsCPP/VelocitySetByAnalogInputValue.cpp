/*! 
*  @example    VelocitySetByAnalogInputValue.cpp

*  @page       velocity-set-by-analog-input-value-cpp VelocitySetByAnalogInputValue.cpp

*  @brief      Velocity Set by Analog Input Value Sample Application.

*  @details
*  This Sample App was created using the EL3002 Analog Input module. Please note that some variable values might changed based on your analog input module.
*  The functionality used with many other Analog Input modules, not only Beckhoff's.
*
*  Learn more about the EL3XXX analog input modules from beckhoff. (https://www.beckhoff.com/english/ethercat/analog.htm)

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

*  @include VelocitySetByAnalogInputValue.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
#include <cmath>        // std::abs

void VelocitySetByAnalogInputValueMain()
{
    using namespace RSI::RapidCode;

    IO      *ioNode;                      // Declare what 'io' is.
    double  analogMaxValue = 65536;       // The analog inputs used are 16 bit wide.
    double  currentVelocity = 0;          // Used to update velocity based on analog input value.
    double  analogCurrentValue = 0;       // Used to store current analog input value.
    double  analogValuePrecentage = 0;    // analogCurrentValue/anlogMaxValue.
    double  velocityAbsoluteSum = 0;      // Absolute sum of min and max velocities.


    // CONSTANTS
    const int AXIS_NUMBER = 0;            // Specify which axis/motor to control.
    const int NODE_NUMBER = 0;            // Specify which IO Terminal/Node to control. 0 for RSI AKD DemoBench
    const int ANALOG_INPUT_0 = 0;         // Specify which Analog Input to read.
    const int MIN_VEL = -10;              // Specify Min Velocity.
    const int MAX_VEL = 10;               // Specify Max Velocity.
    const int ACC = 100;                  // Specify Acceleration/Deceleration value.
    const int USER_UNITS = 1048576;       // Specify your counts per unit / user units.  (the motor used in this sample app has 1048576 encoder pulses per revolution)  

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);            // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                          // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                      // [Helper Function] Check that the axis has been initialize correctly.

        // CONSOLE OUT
        printf("\nVelocity Move Initialized.");
        printf("\nMax Speed = %d ", MAX_VEL);
        printf("\nMin Speed = %d ", MIN_VEL);
        printf("\n\nPress ENTER to update analog reading");
        printf("\nPress SPACEBAR+ENTER to exit.");

        // READY AXIS
        axis->UserUnitsSet(USER_UNITS);                                         // Specify the counts per Unit.
        axis->ErrorLimitTriggerValueSet(1);                                     // Specify the position error limit trigger. (Learn more about this on our support page)
        axis->PositionSet(0);                                                   // Make sure motor starts at position 0 everytime.
        axis->DefaultAccelerationSet(ACC);                                      // Set Acceleration.                                   
        axis->DefaultDecelerationSet(ACC);                                      // Set Deceleration.
        axis->Abort();                                                          // If there is any motion happening, abort it.
        axis->ClearFaults();                                                    // Clear faults.>
        axis->AmpEnableSet(true);                                               // Enable the motor.

        char input = 0;

        while (1) {
            input = getchar();

            velocityAbsoluteSum = std::abs(MIN_VEL) + std::abs(MAX_VEL);
            analogCurrentValue = (double)ioNode->AnalogInGet(ANALOG_INPUT_0);                             // Get analog in value.
            analogValuePrecentage = analogCurrentValue / analogMaxValue;                                  // Get percentage of analog voltage.

            /*
            *  REPRESENTATION OF ANALOG INPUT VALUE BASED ON DIGITAL OUTPUT VOLTAGE
            *
            *  AI Value     -->   0 ............ 32,769 ............ 65,536
            *  DO Voltage   -->   0 ........8 9 10   -10 -9 -8...... 0
            */

            if (analogValuePrecentage * 100 > 99 || analogValuePrecentage * 100 < 1)                        // If the Analog Input value is close to 0 or 65,536 then velocity is ZERO.
                currentVelocity = 0;

            else if (analogValuePrecentage * 100 < 50)                                                      // If the Analog Input value is less than 50% of 65,536 then velocity varies from 0 to 10.
                currentVelocity = velocityAbsoluteSum * analogValuePrecentage;

            else                                                                                            // If the Analog Input value is greater than 50% of 65,536 then velocity varies from 0 to -10.
                currentVelocity = -velocityAbsoluteSum + (velocityAbsoluteSum * analogValuePrecentage);

            axis->MoveVelocity(currentVelocity);                                                            // Update Velocity.

            if (input == ' ') {
                break;
            }
        }
        axis->Abort();                                                                                      // Abort Motion.
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                                  // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


