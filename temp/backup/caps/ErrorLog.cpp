/*! 
@example    ErrorLog.cpp

*  @page       error-log-cpp ErrorLog.cpp

*  @brief      Error Log sample application.

*  @details    This sample application demonstrates how to check errors on creation of RapidCode Objects and catch/throw exceptions througout a program.

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
*  @include ErrorLog.cpp
*/

#include "rsi.h"                                    // Import our RapidCode Library. 
using namespace RSI::RapidCode;

void PrintErrors(RapidCodeObject *rsiClass)         //Helper Function to print all errors of a given RapidCodeObject
{
    RsiError *err;
    while (rsiClass->ErrorLogCountGet() > 0)
    {
        err = rsiClass->ErrorLogGet();
        printf("%s\n", err->text);
    }
}

void ErrorLogMain()
{
    Axis                *axisX;
    const int AXIS_X = (2);

    // Insert the path location of the RMP.rta (usually the RapidSetup folder)  
    char rmpPath[] = "C:\\RSI\\X.X.X\\";

    // Initialize MotionController class.
    MotionController *controller = MotionController::CreateFromSoftware(/*rmpPath*/);
    PrintErrors(controller);

    // initialize Axis X 
    axisX = controller->AxisGet(AXIS_X);
    PrintErrors(axisX);

    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");

}