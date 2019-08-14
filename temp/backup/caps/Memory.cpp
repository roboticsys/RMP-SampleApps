/*!  
@example    memory.cpp

*  @page       memory-cpp memory.cpp

*  @brief      Memory sample application.

*  @details    This sampple app demonstrates how to get/set controller memory.    

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
*  @include memory.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;


void memoryMain()
{


Axis                *axis ;
uint64 addr;

// Initialize MotionController class. (PCI board)
MotionController *controller = MotionController::CreateFromSoftware();    
SampleAppsCPP::HelperFunctions::CheckErrors(controller);

// initialize Axis (0)
axis = controller->AxisGet(0);
SampleAppsCPP::HelperFunctions::CheckErrors(axis);

try
{
// get a controller host address for axis memory
addr = axis->AddressGet(RSIAxisAddressTypeACTUAL_POSITION);
printf("Axis Host address is 0x%x   Firmware Address is 0x%x\n", addr, controller->FirmwareAddressGet(addr));
printf("Value is %lf\n", controller->MemoryDoubleGet(addr)); // ACTUAL_POSITION is a 64-bit double

}
catch (RsiError const& err)
{
printf("\n%s\n", err.text);
}
controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
system("pause");                                        // Allow time to read Console.
}

