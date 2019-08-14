

#include "rsi.h"                                    // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

void IOwithAKDMain()
{
    using namespace RSI::RapidCode;

    // Constants
    const int AXIS_NUMBER = 0;                      // Specify which axis/motor to control.
    const int NODE_INDEX = 0;                       // The EtherCAT Node we will be communicating with
    const int OUTPUT_INDEX = 0;

    char rmpPath[] = "C:\\RSI\\X.X.X\\";            // Insert the path location of the RMP.rta (usually the RapidSetup folder)
    // Initialize MotionController class.
    MotionController   *controller = MotionController::CreateFromSoftware(/*rmpPath*/);   // NOTICE: Uncomment "rmpPath" if project directory is different than rapid setup directory.
    SampleAppsCPP::HelperFunctions::CheckErrors(controller);                              // [Helper Function] Check that the axis has been initialize correctly. 

    try
    {
        SampleAppsCPP::HelperFunctions::StartTheNetwork(controller);          // [Helper Function] Initialize the network.

        Axis *axis = controller->AxisGet(AXIS_NUMBER);                        // Initialize Axis Class. (Use RapidSetup Tool to see what is your axis number)
        SampleAppsCPP::HelperFunctions::CheckErrors(axis);                    // [Helper Function] Check that the axis has been initialize correctly.

      
        IOPoint *output0 = IOPoint::CreateDigitalOutput(controller->IOGet(NODE_INDEX), OUTPUT_INDEX); // Automatically gets the memory index of a specified node and input index
       
        system("pause");
        output0->Set(0);
        system("pause");
        output0->Set(1);
        system("pause");
        output0->Set(0);
        system("pause");
        output0->Set(1);
    }
    catch (RsiError const& err)
    {
        printf("%s\n", err.text);                                              // If there are any exceptions/issues this will be printed out.
    }
    controller->Delete();                                   // Delete the controller as the program exits to ensure memory is deallocated in the correct order.
    system("pause");                                        // Allow time to read Console.
}


