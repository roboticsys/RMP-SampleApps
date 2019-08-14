/*! 
 *  @example    VelocitySetByAnalogInputValue.cs
 
 *  @page       velocity-set-by-analog-input-value-cs VelocitySetByAnalogInputValue.cs

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
 
 *  @include VelocitySetByAnalogInputValue.cs
 */

using RSI.RapidCode.dotNET;
using System;

namespace SampleAppsCS
{
    class VelocitySetByAnalogInputValue
    {
        static void Main(string[] args)
        {
            // GLOBAL VARIABLES
            double analogMaxValue = 65536;     // The analog inputs used are 16 bit wide.
            double currentVelocity = 0;        // Used to update velocity based on analog input value.
            double analogCurrentValue = 0;     // Used to store current analog input value.
            double analogValuePrecentage = 0;  // analogCurrentValue/anlogMaxValue.
            double velocityAbsoluteSum = 0;    // Absolute sum of min and max velocities.

            // CONSTANTS
            const int AXIS_NUMBER = 0;         // Specify which axis/motor to control.
            const int NODE_NUMBER = 0;         // Specify which IO Terminal/Node to control. 0 for RSI AKD DemoBench
            const int ANALOG_INPUT_0 = 0;      // Specify which Analog Input to read.
            const int MIN_VEL = -10;           // Specify Min Velocity.
            const int MAX_VEL = 10;            // Specify Max Velocity.
            const int ACC = 100;               // Specify Acceleration/Deceleration value.
            const int USER_UNITS = 1048576;    // Specify your counts per unit / user units.  (the motor used in this sample app has 1048576 encoder pulses per revolution)  


            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                           // [Helper Function] Check that the controller has been initialized correctly.
            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                       // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                  // Initialize the axis.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                 // [Helper Function] Check that the axis has been initialized correctly.

            IO ioNode = controller.IOGet(NODE_NUMBER);                                    // Initialize the axis.
            SampleAppsCS.HelperFunctions.CheckErrors(ioNode);                               // [Helper Function] Check that the axis has been initialized correctly.

            try
            {
                // CONSOLE OUT
                Console.WriteLine("Velocity Move Initialized.");
                Console.WriteLine("Max Speed = " + MAX_VEL);
                Console.WriteLine("Min Speed = " + MIN_VEL);
                Console.WriteLine("\nPress SPACEBAR to exit.");

                // READY AXIS
                axis.UserUnitsSet(USER_UNITS);                                              // Specify the counts per Unit.
                axis.ErrorLimitTriggerValueSet(1);                                          // Specify the position error limit trigger. (Learn more about this on our support page)
                axis.PositionSet(0);                                                        // Make sure motor starts at position 0 everytime.
                axis.DefaultAccelerationSet(ACC);                                           // Set Acceleration.                                   
                axis.DefaultDecelerationSet(ACC);                                           // Set Deceleration.
                axis.Abort();                                                               // If there is any motion happening, abort it.
                axis.ClearFaults();                                                         // Clear faults.
                axis.AmpEnableSet(true);                                                    // Enable the motor.

                do
                {
                    while (!Console.KeyAvailable)
                    {

                        velocityAbsoluteSum = Math.Abs(MIN_VEL) + Math.Abs(MAX_VEL);
                        analogCurrentValue = (double)ioNode.AnalogInGet(ANALOG_INPUT_0);                           // Get analog in value.
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

                        axis.MoveVelocity(currentVelocity);                                                             // Update Velocity.

                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Spacebar);                                             // If the Spacebar key is pressed, exit.

                axis.Abort();                                                                                           // Abort Motion.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                           // If there are any exceptions/issues this will be printed out.
            }
        }
    }
}
