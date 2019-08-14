/*! 
 *  @example    MotionHoldReleasedByDigitalInput.cs
 
 *  @page       motion-hold-released-by-digital-input-cs MotionHoldReleasedByDigitalInput.cs

 *  @brief      Motion Hold Move Released By Digital Input sample application.

 *  @details    This sample code is done in AKD Drive with Digital IO Inputs switches. A digital input switch triggers to release the HOLD set on the specified Motion. This functionality is available for all Drives but some changes to the sample app may be required.
 
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
 * @include MotionHoldReleasedByDigitalInput.cs
 */

using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;
using System;

namespace SampleAppsCS
{
    class MotionHoldReleasedByDigitalInput
    {
        static void Main(string[] args)
        {

            // Constants
            const int DIGITAL_INPUTS_PDO_INDEX = 3;     // Specify the pdo inputs index that represent digital inputs.
            const int AXIS_INDEX = 0;                   // Specify which axis/motor to control.
            const int USER_UNITS = 1048576;      // Specify USER UNITS

            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialize correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_INDEX);                                                  // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialize correctly.


            try
            {
                // GET AXIS READY
                axis.UserUnitsSet(USER_UNITS);                                                         // Specify the counts per Unit.
                axis.PositionSet(0);                                                                // Make sure motor starts at position 0 everytime.
                axis.ErrorLimitActionSet(RSIAction.RSIActionNONE);                                  // Uncomment when using Phantom Axes.
                axis.Abort();                                                                       // If there is any motion happening, abort it.
                axis.ClearFaults();                                                                 // Clear faults.
                axis.AmpEnableSet(true);                                                            // Enable the motor.


                // SET MOTION HOLD
                ulong inputAddress = controller.NetworkInputAddressGet(DIGITAL_INPUTS_PDO_INDEX);   // Get host address using the PDO Input Index of Digital Inputs.

                axis.MotionHoldTypeSet(RSIMotionHoldType.RSIMotionHoldTypeCUSTOM);                  // Use TypeCUSTOM to hold execution based on a particular bit turning ON or OFF.
                axis.MotionHoldUserAddressSet(inputAddress);                                        // Specify the digital inputs host address. This address' value will be used to evaluate the motion hold condition.
                axis.MotionHoldUserMaskSet(0x20000);                                                // Specify the bit you want to mask/watch from the MotionHoldUserAddressSet' address value (this evaluates using a logic AND)
                axis.MotionHoldUserPatternSet(0x20000);                                             // Specify the bit value that will release the motion hold. (When this value is met, motion hold will be released)

                axis.MotionAttributeMaskOnSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);             // Set the HOLD motion attribute mask ON. (This initializes the HOLD on a particular motion)

                axis.MoveRelative(10);                                                              // Command simple relative motion. (This motion will be HOLD by the condition above)
                axis.MotionDoneWait();                                                              // Wait for Motion to be completed.

                axis.MoveRelative(10);                                                              // If motion attribute mask off has not been set, this motion will have same HOLD condition as previous move.
                axis.MotionDoneWait();                                                              // Wait for Motion to be completed.

                axis.MotionAttributeMaskOffSet(RSIMotionAttrMask.RSIMotionAttrMaskHOLD);            // Set the HOLD motion attribute mask OFF. (This will clear any motion HOLDS that were set on this Axis)

                axis.MoveRelative(10);                                                              // This motion will have no HOLD since the previous line has set the motion attribute mask OFF.
                axis.MotionDoneWait();                                                              // Wait for Motion to be completed.

                // Abort and Clear Faults 
                axis.Abort();
                axis.ClearFaults();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                       // If there are any exceptions/issues this will be printed out.
            }
            Console.WriteLine("\nPress Any Key To Exit");                                         // Allow time to read Console.
            Console.ReadKey();
        }
    }
}
