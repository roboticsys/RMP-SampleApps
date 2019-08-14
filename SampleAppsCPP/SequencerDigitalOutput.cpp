/// sequencerDigitalOutput.cpp   Fire a Digital Output pulse when an Axis' Actual Position is exceeded
/*  sequencerDigitalOutput.cpp

 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) 
 prior to running this program so that the motor can rotate in a stable manner.

 For any questions regarding this sample code please visit our documentation at www.roboticsys.com


 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.
*/


#include "rsi.h"

#define SEQUENCER			 0
#define TRIGGERS			100
#define BIT_MASK			RSIControlIOMaskUSER0_OUT


using namespace RSI::RapidCode::SynqNet;


void sequencerDigitalOutputMain()
{
	try
	{

		MotionController *controller ;
		Axis		 *axis ;
		long controllerDigitalOutputAddr;
		long axisActualPositionAddr;

		// initialize RSI classes
		controller = MotionController::CreateFromBoard(0);
		axis = controller->AxisGet(0);

		IOPoint  *userOut;

		//userOut->CreateDigitalOutput( controller, 
		// get the host controller address for the controller digital outputs
		controllerDigitalOutputAddr = controller->AddressFromStringGet("SystemData.IO.HostOutput[0]", "c:\\mei\\xmp\\bin\\winnt\\stdmei.map");

		// get the host controller adddress for the axis' actual position
		axisActualPositionAddr = axis->AddressGet(RSIAxisAddressTypeACTUAL_POSITION);


		controller->SequencerEnableSet(SEQUENCER, true);
		
		// append trigger and I/O commands to Sequencer
		for(int i = 0; i < TRIGGERS; i++)
		{
			// wait for Actual Poisition to be exceeded
			controller->CommandWaitLong(SEQUENCER,
												RSICommandOperatorGREATER_OR_EQUAL,
												axisActualPositionAddr,
												(i + 1) * 100 ); // incrementing the "trigger" position
			// turn bit on
			controller->CommandComputeLong(SEQUENCER,							
												RSICommandOperatorOR,			
												controllerDigitalOutputAddr,   // input to computation
												controllerDigitalOutputAddr,   // output from computation written here
												BIT_MASK );							
			// wait a while
			controller->CommandDelay(SEQUENCER, 0.50); //seconds
			
			// turn bit off
			controller->CommandComputeLong(SEQUENCER,							
												RSICommandOperatorAND,			
												controllerDigitalOutputAddr,   // input to computation
												controllerDigitalOutputAddr,   // output from computation
												~(BIT_MASK) );
		
		}

		controller->SequencerStart(0);

		printf("press a key to stop and delete sequencer \n");
		// start the motion on the axis here now
		// wait for key press
		while(controller->OS->KeyGet(RSIWaitPOLL) < 0)
		{
			controller->OS->Sleep(1);
		}

		controller->SequencerStop(0);
		controller->SequencerEnableSet(0, false);

	}
	catch (RsiError *err)
	{
		printf("%s\n", err->text);
	}
}