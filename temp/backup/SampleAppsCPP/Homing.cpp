/*! 
 *  @page       home-cpp home.cpp

 *  @brief      Home sample application.
 
 *  @details        
	This sample app performs a simple homing routine that triggers home off an input pulse, captures the hardware position, sets the origin and then moves back to that home position.

	The home method used in this sample code (RSIHomeMethodNEGATIVE_LIMIT) is one of the 35 homing routines available in our homing documenation.
	
 *  @pre        This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) prior to running this program so that the motor can rotate in a stable manner.
 
 *  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.
 
 *  @copyright 
	Copyright &copy; 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
	This software contains proprietary and confidential information of Robotic 
	Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
	in the license agreement under which this software is supplied, disclosure, 
	reproduction, or use with controls other than those provided by RSI or suppliers
	for RSI is strictly prohibited without the prior express written consent of 
	Robotic Systems Integration.
 *
 *  @include home.cpp 
 */

#include "rsi.h"
using namespace RSI::RapidCode;

int main()
{
	try
	{
		#define AXIS_NUMBER							0
		#define VELOCITY							8000
		#define ACCELERATION						6000
		#define DECELERATION						6000

		//Only used if you set max Travel distances for Various homing stages.
		#define MAX_TRAVEL_TO_SWITCH				10000
		#define MAX_TRAVEL_TO_EDGE					100
		#define MAX_TRAVEL_TO_INDEX					2000

		MotionController	*controller ;
		Axis				*axis ;

		///////////////////////////////////////////////////////////////
		// Once Per Application Initialization 
		//controller = MotionController::CreateFromSoftware(); //SynqNet
		// -or- Not both.
		controller = MotionController::CreateFromSoftware(); //RMP
 
		axis = controller->AxisGet(AXIS_NUMBER);

		axis->TraceFileSet("dummy.txt");
		axis->TraceMaskOnSet(RSITrace::RSITraceCUSTOM);
		///////////////////////////////////////////////////////////////

		// These is unlikely to change.  If it will not.  Put it in Initialization above.
		axis->HomeMethodSet(RSIHomeMethodNEGATIVE_LIMIT);
		axis->HomeOffsetSet(0.0);
		axis->HomeVelocitySet(VELOCITY);
		axis->HomeSlowVelocitySet(VELOCITY / 10);  // Used for final move to index, if selected method does so.
		axis->HomeAccelerationSet(ACCELERATION);
		axis->HomeDecelerationSet(ACCELERATION);

		// Optional Configuration for Max Travel.
		axis->HomeTravelDistanceSet(RSIHomeStageSTAGE_ONE, MAX_TRAVEL_TO_SWITCH);
		axis->HomeTravelDistanceSet(RSIHomeStageSTAGE_TWO, MAX_TRAVEL_TO_EDGE);
		axis->HomeTravelDistanceSet(RSIHomeStageSTAGE_THREE, MAX_TRAVEL_TO_INDEX);

		// May or may not be constantly enabled.
		axis->HomeActionSet(RSIActionSTOP);

		// Ready Axis.
		axis->ClearFaults();
		axis->AmpEnableSet(true);

		// Begin Homing.
		axis->Home();

		if(axis->HomeStateGet() == true)
		{
			printf("Homing successful\n");
		}
		axis->ClearFaults();
		axis->TraceFileClose();
	}
	catch (RsiError const& err)
	{
		printf("%s\n", err.text);
	}
}