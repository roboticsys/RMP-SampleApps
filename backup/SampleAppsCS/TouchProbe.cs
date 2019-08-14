/*! 
 *  @example    TouchProbe.cs
 
 *  @page       touch-probe-cs TouchProbe.cs

 *  @brief      Example of Touch Probe for Panasonic Drives
 
 *  @details        
 *  Learn how to use Touch Probe with Panasonic Drives:
        <BR>1) Things you need to know for Panasonics.
        <BR>2) Understand Touch Probe Function (0x60B8 Sub 0 Size 2)
        <BR>3) Understand Touch Probe Status (0x60B9 Sub 0 Size 2)
        <BR>4) Understand Touch Probe captured values (0x60BA-0x60BD Sub 0 Size 4)
 
 *  @pre        This sample code presumes that the user has configured their system prior to running this program.
 
 *  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.
 
 *  @copyright 
	Copyright &copy; 1998-2018 by Robotic Systems Integration, Inc. All rights reserved.
	This software contains proprietary and confidential information of Robotic 
	Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
	in the license agreement under which this software is supplied, disclosure, 
	reproduction, or use with controls other than those provided by RSI or suppliers
	for RSI is strictly prohibited without the prior express written consent of 
	Robotic Systems Integration.
 
 *  @include TouchProbe.cs
 */

using RSI.RapidCode.dotNET;        // Import our RapidCode Library

namespace SampleAppsCS
{
    class TouchProbe
    {
        ////////////////////////////////////////////
        // Things you need to know for Panasonics //
        ////////////////////////////////////////////
        // There are two Touch Probes available.
        // They default to SI5 (EXT1) & SI6 (EXT2) and are configurable.
        // Do not try to capture on both the Positive and Negative Edge of a Signal on the same Touch Probe.
        // Do not configure to the Falling Edge of the Z-phase.
        // If you change configuration of a Touch Probe after you've enabled it, you will need to disable it for changes to take place.
        // Depending on your Configuration you may have have to use PDO or SDO calls.  By default, Touch Probe 1 is available as a cyclically exchanged value.

        ////////////////////////////////////////////////
        // Touch Probe Function (0x60B8 Sub 0 Size 2) //
        ////////////////////////////////////////////////
        //Bit 0 - Enable(1) Touch Probe 1
        //Bit 1 - First Event(0), Continuous(1) [Continuous will overwrite old values on each new signal]
        //Bit 2 - Trigger on EXT1(0), Z-phase(1)
        //Bit 4 - Sample Positive Edge(1) (Store position to 0x60BA Sub 0 Size 4)
        //Bit 5 - Sample Negative Edge(1) (Store position to 0x60BB Sub 0 Size 4)

        //Bit 8 - Enable(1) Touch Probe 2
        //Bit 9 - First Event(0), Continuous(1) [Continuous will overwrite old values on each new signal]
        //Bit 10 - Trigger on EXT2(0), Z-phase(1)
        //Bit 12 - Sample Positive Edge(1) (Store position to 0x60BC Sub 0 Size 4)
        //Bit 13 - Sample Negative Edge(1) (Store position to 0x60BD Sub 0 Size 4)

        //////////////////////////////////////////////
        // Touch Probe Status (0x60B9 Sub 0 Size 2) //
        //////////////////////////////////////////////
        //Bit 0 - Touch Probe 1 Enabled(1)
        //Bit 1 - Positive Edge Value Stored(1)
        //Bit 2 - Negative Edge Value Stored(1)
        //Bit 8 - Touch Probe 2 Enabled(1)
        //Bit 9 - Positive Edge Value Stored(1)
        //Bit 10 - Negative Edge Value Stored(1)

        //PDO Method
        static void CaptureEachIndexPulseDuringMotion(MotionController controller)
        {
            //In this example, we have 1 Panasonic Node with the following exchanged Inputs.  These constants likely will not be those you use.
            const int TOUCH_PROBE_OUTPUT_INDEX = 3; //NetworkOutput #3 - Touch Probe Function
            const int TOUCH_PROBE_STATUS_INDEX = 6; //NetworkInput #6 - Touch Probe Status
            const int TOUCH_PROBE_VALUE_INDEX = 7; //NetworkInput #7 - Touch Probe pos1 pos value

            //We want to Enable the Touch Probe (Bit 0 to 1), Set to Continuous Capture (Bit 1 to 1), Set to Z-phase (Bit 2 to 1), and Sample the positive Edge (Bit 4 to 1)
            //Our Touch Probe Function should be binary xxxx xxxx xx01 x111 -or- 0x17
            //Note if you are making use of Touch Probe 2, some of the above bits will become significant
            const ulong TOUCH_PROBE_ON_EACH_Z_PHASE_COMMAND = 0x17;

            //Enable
            controller.NetworkOutputValueSet(TOUCH_PROBE_OUTPUT_INDEX, TOUCH_PROBE_ON_EACH_Z_PHASE_COMMAND);

            //Evaluate
            ulong currentStatus = controller.NetworkInputValueGet(TOUCH_PROBE_STATUS_INDEX);
            //See above Touch Probe Status Section for details.  Maybe be useful for your diagnostics and assurance that it is ok to use the next value.

            //You'll want to initiate some type of motion so you get a new position every Z-phase

            //After you see currentStatus Bit 1 has high, you know you have your first (of many) Z-Phase.
            ulong lastZPhasePosition = controller.NetworkInputValueGet(TOUCH_PROBE_VALUE_INDEX);
        }

        //SDO Method
        static void CapturePositionOnFallingEdgeOfSI6(MotionController controller, Axis axis)
        {
            //Here we are assuming that you still have Touch Probe Status as PDO entries.  They can be set and read using SDO in similiar fashion if that isn't the case.
            const int TOUCH_PROBE_OUTPUT_INDEX = 3; //NetworkOutput #3 - Touch Probe Function
            const int TOUCH_PROBE_STATUS_INDEX = 6; //NetworkInput #6 - Touch Probe Status

            const int TOUCH_PROBE_2_FALLING_EDGE_VALUE_INDEX = 0x60BD;
            const int TOUCH_PROBE_2_FALLING_EDGE_VALUE_SUB_INDEX = 0x0;
            const int TOUCH_PROBE_2_FALLING_EDGE_VALUE_SIZE = 0x4;

            //We want to Enable the Touch Probe 2 (Bit 8 to 1), Set to First Event (Bit 9 to 0), Set to EXT2 (Bit 10 to 0), and Sample the negative Edge (Bit 13 to 1)
            //Our Touch Probe Function should be binary xx1x x001 xxxx xxxx -or- 0x2100
            //Note if you are making use of Touch Probe 1, some of the above bits will become significant
            const ulong TOUCH_PROBE_2_ON_FIRST_FALLING_EXT2_COMMAND = 0x2100;

            //Enable
            controller.NetworkOutputValueSet(TOUCH_PROBE_OUTPUT_INDEX, TOUCH_PROBE_2_ON_FIRST_FALLING_EXT2_COMMAND);

            //Evaluate
            ulong currentStatus = controller.NetworkInputValueGet(TOUCH_PROBE_STATUS_INDEX);
            //See above Touch Probe Status Section for details.  Maybe be useful for your diagnostics and assurance that it is ok to use the next value.

            //After you've observed currentStatus Bit 10 go high, you know the following value is useful.
            int fallingEdgeExt2 = axis.NetworkNode.ServiceChannelRead(TOUCH_PROBE_2_FALLING_EDGE_VALUE_INDEX, TOUCH_PROBE_2_FALLING_EDGE_VALUE_SUB_INDEX, TOUCH_PROBE_2_FALLING_EDGE_VALUE_SIZE);

        }

        // Main
        static void Main(string[] args)
        {
            // Initialize RapidCode Objects
            MotionController controller = MotionController.CreateFromSoftware(/*@"C:\RSI\X.X.X\"*/);    // Insert the path location of the RMP.rta (usually the RapidSetup folder) 
            SampleAppsCS.HelperFunctions.CheckErrors(controller);                   // [Helper Function] Check that the controller has been initialized correctly.

            SampleAppsCS.HelperFunctions.StartTheNetwork(controller);               // [Helper Function] Initialize the network.
            Axis axis = controller.AxisGet(0);

            CaptureEachIndexPulseDuringMotion(controller);

            CapturePositionOnFallingEdgeOfSI6(controller, axis);
        }
    }
}

