/*! 
 *  @example    PVAJTmotion.cs

 *  @page       pvajt-motion-cs PVAJTmotion.cs
 
 *  @brief      PVAJT Simple Motion sample application.
 
 *  @details    This application demonstrates how to use PVAJT Motion.
 
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
 
 *  @include PVAJTmotion.cs
    
 */

using System;
using RSI.RapidCode.dotNET;         // Import our RapidCode Library.
using RSI.RapidCode.dotNET.Enums;

namespace SampleAppsCS
{
    class PVAJTmotion
    {
        static void Main(string[] args)
        {

            // Constants
            const int AXIS_NUMBER = 0;                              // Specify which axis/motor to control.

            int points      = 3;                                    // Specify the total number of streamed points. (Very Important!)
            int emptyCount  = -1;                                   // E-stop generated if there aren't at least this many points loaded. ("-1" if you don't wish to trigger an E-Stop)

            double[] positions      = { 4.0, 8.0, 16.0 };           // Specify the positions that you want to reach. (it can be n number)
            double[] velocities     = { 1.0, 2.0, 4.0 };            // Specify the velocities that you want to use to reach your position.
            double[] accelerations  = { 1, 1, 1 };                  // Specify the accelerations that you want to use to reach your position.
            double[] jerks          = { 50, 50, 50 };               // Specify the jerk that you want to use in each move.
            double[] times          = { 4.0, 2, 1 };                // Specify the times in which you want to reach each position. (velocity and acceleration is calculated by the RMP)
// Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                                   // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);                               // [Helper Function] Initialize the network.

            Axis axis = controller.AxisGet(AXIS_NUMBER);                                                 // Initialize the axis.
            SampleAppsCS.HelperFunctions.CheckErrors(axis);                                         // [Helper Function] Check that the axis has been initialized correctly.

            try
            {

                axis.Abort();
                axis.ClearFaults();
                axis.AmpEnableSet(true);
                axis.PositionSet(0);
                axis.UserUnitsSet(1048576);

                axis.MovePVAJT(positions,                                                       // Specify the type of PT Motion that you want to perform. (RSIMotionType.RSIMotionTypePT, RSIMotionType.RSIMotionTypeBSPLINE, RSIMotionType.RSIMotionTypeBSPLINE2)
                            velocities,                                                         // Specify the positions that you want to reach. (it can be n number)
                            accelerations,                                                      // Specify the accelerations that you want to use during every move. 
                            jerks,                                                              // Specify the jerk that you want to use during every move.
                            times,                                                              // Specify the times in which you want to reach each position. (velocity and acceleration is calculated by the RMP)
                            points,                                                             // Specify the total number of streamed points.
                            emptyCount,                                                         // E-stop generated if there are this number or fewer frames loaded. Use the value “0” to trigger an E-stop when the buffer is empty, or “-1” to not trigger an E-stop. (Typically for PT motion there are two frames per PT point)
                            false,                                                              // Specify whether points are kept, or are not kept.
                            true);                                                              // Specify if this is the last MovePT. (If True, this is the final point. If False, more points expected.)

                axis.MotionDoneWait();                                                          // Wait for motion to be completed.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                                                   // If there are any exceptions/issues this will be printed out.
            }
        }
    }
}
