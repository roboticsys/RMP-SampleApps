/*! 
@example    gantry.cpp

*  @page       gantry-cpp gantry.cpp

*  @brief      Gantry sample application.

*  @details    This sample will configure two parallel axes (X1, X2) connected to the same load, and configure them so that X1 will be used to command linear motion, and X2 will be used to command yaw motion.

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
*  @include gantry.cpp


*/

#include "rsi.h" // Import our RapidCode Library. 
#include "HelperFunctions.h"                        // Import our SampleApp helper functions. 
using namespace RSI::RapidCode;

#pragma region GantryDeclares
MotionController *mc;;
Axis *linearAxis;
Axis *yawAxis;

int linearAxisNumber = 0;
int yawAxisNumber = 1;

int defaultEncoderNumerator = 0;   // 0 disables
int defaultEncoderDenominator = 0;
int gantryEncoderNumerator = 1;   // we want 1/2 
int gantryEncoderDenominator = 2;

double x1PrimaryCoeff = 1.0;
double x2PrimaryCoeff = 1.0;
double x1SecondaryCoeff = 1.0;
double x2SecondaryCoeff = -1.0;
double defaultPrimaryCoeff = 1.0;
double defaultSecondaryCoeff = 0.0;

uint64 x1EncoderAddress;
uint64 x2EncoderAddress;
uint64 x1FilterPrimaryPointerAddress;
uint64 x1FilterSecondaryPointerAddress;
uint64 x2FilterPrimaryPointerAddress;
uint64 x2FilterSecondaryPointerAddress;
uint64 x1FilterPrimaryCoefficientAddress;
uint64 x1FilterSecondaryCoefficientAddress;
uint64 x2FilterPrimaryCoefficientAddress;
uint64 x2FilterSecondaryCoefficientAddress;
uint64 x1AxisLinkAddress;
uint64 x2AxisLinkAddress;
#pragma endregion

#pragma region GantryMethods
void ReadAddressesFromMotionController()
{
    x1EncoderAddress = linearAxis->AddressGet(RSIAxisAddressTypeENCODER_PRIMARY);
    x2EncoderAddress = yawAxis->AddressGet(RSIAxisAddressTypeENCODER_PRIMARY);
    x1FilterPrimaryPointerAddress = linearAxis->AddressGet(RSIAxisAddressTypeFILTER_PRIMARY_POINTER);
    x1FilterSecondaryPointerAddress = linearAxis->AddressGet(RSIAxisAddressTypeFILTER_SECONDARY_POINTER);
    x2FilterPrimaryPointerAddress = yawAxis->AddressGet(RSIAxisAddressTypeFILTER_PRIMARY_POINTER);
    x2FilterSecondaryPointerAddress = yawAxis->AddressGet(RSIAxisAddressTypeFILTER_SECONDARY_POINTER);
    x1FilterPrimaryCoefficientAddress = linearAxis->AddressGet(RSIAxisAddressTypeFILTER_PRIMARY_COEFF);
    x1FilterSecondaryCoefficientAddress = linearAxis->AddressGet(RSIAxisAddressTypeFILTER_SECONDARY_COEFF);
    x2FilterPrimaryCoefficientAddress = yawAxis->AddressGet(RSIAxisAddressTypeFILTER_PRIMARY_COEFF);
    x2FilterSecondaryCoefficientAddress = yawAxis->AddressGet(RSIAxisAddressTypeFILTER_SECONDARY_COEFF);
    x1AxisLinkAddress = linearAxis->AddressGet(RSIAxisAddressTypeAXIS_LINK);
    x2AxisLinkAddress = yawAxis->AddressGet(RSIAxisAddressTypeAXIS_LINK);
}

void SetupEncoderMixing(bool enableGantry)
{
    if (enableGantry)
    {
        // first scale encoders by half
        linearAxis->EncoderRatioSet(RSIMotorFeedbackPRIMARY, gantryEncoderNumerator, gantryEncoderDenominator);
        yawAxis->EncoderRatioSet(RSIMotorFeedbackPRIMARY, gantryEncoderNumerator, gantryEncoderDenominator);
        mc->OS->Sleep(10);
        // mix encoders (add on linear, subtract for yaw)
        linearAxis->FeedbackPointerSet(RSIAxisPositionInputFIRST, x1EncoderAddress);
        linearAxis->FeedbackPointerSet(RSIAxisPositionInputSECOND, x2EncoderAddress);
        linearAxis->GantryTypeSet(RSIAxisGantryTypeADD);
        yawAxis->FeedbackPointerSet(RSIAxisPositionInputFIRST, x1EncoderAddress);
        yawAxis->FeedbackPointerSet(RSIAxisPositionInputSECOND, x2EncoderAddress);
        yawAxis->GantryTypeSet(RSIAxisGantryTypeSUBTRACT);
    }
    else
    {
        linearAxis->EncoderRatioSet(RSIMotorFeedbackPRIMARY, defaultEncoderNumerator, defaultEncoderDenominator);
        yawAxis->EncoderRatioSet(RSIMotorFeedbackPRIMARY, defaultEncoderNumerator, defaultEncoderDenominator);
        mc->OS->Sleep(10);
        linearAxis->FeedbackPointerSet(RSIAxisPositionInputFIRST, x1EncoderAddress);
        linearAxis->FeedbackPointerSet(RSIAxisPositionInputSECOND, x1EncoderAddress);
        linearAxis->GantryTypeSet(RSIAxisGantryTypeNONE);
        yawAxis->FeedbackPointerSet(RSIAxisPositionInputFIRST, x2EncoderAddress);
        yawAxis->FeedbackPointerSet(RSIAxisPositionInputSECOND, x2EncoderAddress);
        yawAxis->GantryTypeSet(RSIAxisGantryTypeNONE);
    }
}
void SetupFilterMixing(bool enableGantry)
{
    if (enableGantry)
    {
        // mix X1 filter
        mc->MemorySet(x1FilterPrimaryPointerAddress, mc->FirmwareAddressGet(x1AxisLinkAddress));
        mc->MemorySet(x1FilterSecondaryPointerAddress, mc->FirmwareAddressGet(x2AxisLinkAddress));
        // mix X2 filter
        mc->MemorySet(x2FilterPrimaryPointerAddress, mc->FirmwareAddressGet(x1AxisLinkAddress));
        mc->MemorySet(x2FilterSecondaryPointerAddress, mc->FirmwareAddressGet(x2AxisLinkAddress));

        // setup X1 filter mixing coefficients
        mc->MemoryDoubleSet(x1FilterPrimaryCoefficientAddress, x1PrimaryCoeff);
        mc->MemoryDoubleSet(x1FilterSecondaryCoefficientAddress, x1SecondaryCoeff);
        // setup X2 filter mixing coefficients
        mc->MemoryDoubleSet(x2FilterPrimaryCoefficientAddress, x2PrimaryCoeff);
        mc->MemoryDoubleSet(x2FilterSecondaryCoefficientAddress, x2SecondaryCoeff);
    }
    else
    {
        // unmix X1 filter
        mc->MemorySet(x1FilterPrimaryPointerAddress, mc->FirmwareAddressGet(x1AxisLinkAddress));
        mc->MemorySet(x1FilterSecondaryPointerAddress, mc->FirmwareAddressGet(x1AxisLinkAddress));
        // unmix X2 filter
        mc->MemorySet(x2FilterPrimaryPointerAddress, mc->FirmwareAddressGet(x2AxisLinkAddress));
        mc->MemorySet(x2FilterSecondaryPointerAddress, mc->FirmwareAddressGet(x2AxisLinkAddress));
        // setup X1 filter defult coefficients
        mc->MemoryDoubleSet(x1FilterPrimaryCoefficientAddress, defaultPrimaryCoeff);
        mc->MemoryDoubleSet(x1FilterSecondaryCoefficientAddress, defaultSecondaryCoeff);
        // setup X2 filter default coefficients
        mc->MemoryDoubleSet(x2FilterPrimaryCoefficientAddress, defaultPrimaryCoeff);
        mc->MemoryDoubleSet(x2FilterSecondaryCoefficientAddress, defaultSecondaryCoeff);
    }
}
void GantryEnable(bool enable)
{
    ReadAddressesFromMotionController();
    linearAxis->Abort();
    yawAxis->Abort();
    SetupEncoderMixing(enable);
    SetupFilterMixing(enable);
    linearAxis->ClearFaults();
    linearAxis->AmpEnableSet(true);
    yawAxis->ClearFaults();
    yawAxis->AmpEnableSet(true);
}
#pragma endregion

void gantryMain()
{

    // Create and initialize MotionController
    mc = MotionController::CreateFromSoftware();
    SampleAppsCPP::HelperFunctions::CheckErrors(mc);

    // Get Axis X0 and X1 respectively.
    linearAxis = mc->AxisGet(linearAxisNumber);
    SampleAppsCPP::HelperFunctions::CheckErrors(linearAxis);

    yawAxis = mc->AxisGet(yawAxisNumber);
    SampleAppsCPP::HelperFunctions::CheckErrors(yawAxis);

    //Only need once
    ReadAddressesFromMotionController();
    //Enable when desired.
    GantryEnable(true);

    //////////////////////
    ///Normal Operation///
    //////////////////////

    //Disable when finished.
    GantryEnable(false);



}