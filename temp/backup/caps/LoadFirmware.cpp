/*! 
	@example	loadFirmware.cpp

 *  @page       load-firmware-cpp loadFirmware.cpp

 *  @brief      Load Firmware sample application.
 
 *  @details      
	This sample app show how the controller automatically downloads firmware
 
	Note: The flash utility program uses the same code to download firmware to the flash memory.
	
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
 *  @include loadFirmware.cpp 
 */

#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include "stdmpi.h"
//#include "stdmei.h"
//#include "apputil.h"
#include "rsi.h"

#include <conio.h>

using namespace RSI::RapidCode;


void loadFirmwareMain(int argc, char   *argv[])
{
	try
	{
		// initialize MotionController class
		MotionController		*controller ;

		// initialize Axis class  
		controller = MotionController::CreateFromSoftware();	

		/*
		Lines below will only download firmware for the XMP controller for specific 
		MPI version. In this sample application the firmware file is valid only for 
		MPI version 03.02.03.
		If using ZMP controller with MPI version 03.02.03, change the filename.
		*/
		char* filename = "c:\\mei\\xmp\\bin\\XMP561B4.bin\n";
		controller->FirmwareDownload(filename);
		
		printf("\nSuccessfully downloaded firmware with MPI version 03.02.03 for XMP controller \n");

	}
	catch (RsiError const& err)
	{
		printf("%s\n", err.text);
	}
}