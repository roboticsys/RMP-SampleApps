#include "rsi.h"                                    // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 

#include <string>

using std::cout;
using std::endl;

using namespace RSI::RapidCode;
namespace SampleAppsCPP
{

    /*
    static void CheckErrors(RapidCodeObject *rsiObject)
    {
    RsiError *err;
    while(rsiObject->ErrorLogCountGet() > 0)
    {
    err = rsiObject->ErrorLogGet();
    printf("%s\n", err->text);
    }
    }

    static void StartTheNetwork(MotionController *controller)
    {
    // Initialize the Network
    if (controller->NetworkStateGet() != RSINetworkState::RSINetworkStateOPERATIONAL)        // Check if network is started already.
    {
    cout << "Starting Network.." << endl;
    controller->NetworkStart();                                                          // If not. Initialize The Network. (This can also be done from RapidSetup Tool)
    }

    if (controller->NetworkStateGet() != RSINetworkState::RSINetworkStateOPERATIONAL)        // Check if network is started again.
    {
    int messagesToRead = controller->NetworkLogMessageCountGet();                        // Some kind of error starting the network, read the network log messages

    for (int i = 0; i < messagesToRead; i++)
    {
    cout << controller->NetworkLogMessageGet(i) << endl;                                // Print all the messages to help figure out the problem
    }
    cout << "Expected OPERATIONAL state but the network did not get there." << endl;
    //throw new RsiError();                                                             // Uncomment if you want your application to exit when the network isn't operational. (Comment when using phantom axis)
    }
    else                                                                                    // Else, of network is operational.
    {
    cout << "Network Started" << endl;
    }
    }
    */
}
