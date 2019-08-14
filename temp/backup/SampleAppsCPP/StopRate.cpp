/*! 
@example    stoprate.cpp

*  @page       stop-rate-cpp stoprate.cpp

*  @brief      Stop Rate sample application.

*  @details
This sample code demonstrates how to configure the RSIActionSTOP and RSIActionE_STOP deceleration rates for a given axis->

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
*  @include stoprate.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
void StopRateMain()
{
    using namespace RSI::RapidCode;

    const int AXIS_NUMBER = 0;

    //Define STOP and ESTOP rates in seconds. 
    //The value entered here will be the new STOP and ESTOP rates (second).
    const float STOP_RATE_DEFAULT = ((float)1.0);  //seconds
    const float ESTOP_RATE_DEFAULT = ((float)0.050); //seconds

    /* Command line arguments and defaults */
    float   stopRate = STOP_RATE_DEFAULT;
    float   eStopRate = ESTOP_RATE_DEFAULT;

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);           // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                         // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                     // [Helper Function] Check that the axis has been initialize correctly.

        //display old STOP and ESTOP rates.
        printf("OLD: StopRate = %f   eStopRate = %f\n",
            axis->StopTimeGet(),
            axis->EStopTimeGet());

        //Replacing new values of STOP and ESTOP rates(seconds). Values set above.
        axis->StopTimeSet(stopRate);
        axis->EStopTimeSet(eStopRate);

        //display new STOP and ESTOP rates.
        printf("\nNEW: StopRate = %f   eStopRate = %f\n\n",
            axis->StopTimeGet(),
            axis->EStopTimeGet());
    }
    catch (RsiError const& err)
    {
        printf("\n%s\n", err.text);
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}

