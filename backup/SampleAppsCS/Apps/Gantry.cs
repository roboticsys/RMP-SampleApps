/// @example Gantry.cs   Configure a gantry which has two motors, two encoders.
/* 
 Copyright(c) 1998-2009 by Robotic Systems Integration, Inc. All rights reserved.
 This software contains proprietary and confidential information of Robotic 
 Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
 in the license agreement under which this software is supplied, disclosure, 
 reproduction, or use with controls other than those provided by RSI or suppliers
 for RSI is strictly prohibited without the prior express written consent of 
 Robotic Systems Integration.

 Warning!  This is a sample program to assist in the integration of your motion 
 controller with your application.  It may not contain all of the logic and safety
 features that your application requires.

 For any questions regarding this sample code please visit www.roboticsys.com.
 ==================================================================================

 * 
 * This sample will configure two parallel axes (X1, X2) connected to the same load, 
 * and configure them so that X1 will be used to command linear motion, and X2 will 
 * be used to command yaw motion.
 * 
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    public class Gantry
    {
        // RapidCode objects
        MotionController mc;
        Axis linearAxis;
        Axis yawAxis;

        // constants        
        const int X1 = 5;  // axis number
        const int X2 = 6;  // axis number

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

        // stuff we'll need
        UInt64 x1EncoderAddress;
        UInt64 x2EncoderAddress;
        UInt64 x1FilterPrimaryPointerAddress;
        UInt64 x1FilterSecondaryPointerAddress;
        UInt64 x2FilterPrimaryPointerAddress;
        UInt64 x2FilterSecondaryPointerAddress;
        UInt64 x1FilterPrimaryCoefficientAddress;
        UInt64 x1FilterSecondaryCoefficientAddress;
        UInt64 x2FilterPrimaryCoefficientAddress;
        UInt64 x2FilterSecondaryCoefficientAddress;
        UInt64 x1AxisLinkAddress;
        UInt64 x2AxisLinkAddress;

        public void TurnOnGantry()
        {
            GantryEnable(true);
        }


        public void TurnOffGantry()
        {
            GantryEnable(false);
        }


        private void GantryEnable(bool enable)
        {
            mc = MotionController.Create();
            linearAxis = mc.AxisGet(X1);
            yawAxis = mc.AxisGet(X2);
            ReadAddressesFromMotionController();

            linearAxis.Abort();
            yawAxis.Abort();

            SetupEncoderMixing(enable);
            SetupFilterMixing(enable);

            linearAxis.ClearFaults();
            linearAxis.AmpEnableSet(true);
            yawAxis.ClearFaults();
            yawAxis.AmpEnableSet(true);

        }

        private void ReadAddressesFromMotionController()
        {
            x1EncoderAddress = linearAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeENCODER_PRIMARY);
            x2EncoderAddress = yawAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeENCODER_PRIMARY);
            x1FilterPrimaryPointerAddress = linearAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_PRIMARY_POINTER);
            x1FilterSecondaryPointerAddress = linearAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_SECONDARY_POINTER);
            x2FilterPrimaryPointerAddress = yawAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_PRIMARY_POINTER);
            x2FilterSecondaryPointerAddress = yawAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_SECONDARY_POINTER);
            x1FilterPrimaryCoefficientAddress = linearAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_PRIMARY_COEFF);
            x1FilterSecondaryCoefficientAddress = linearAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_SECONDARY_COEFF);
            x2FilterPrimaryCoefficientAddress = yawAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_SECONDARY_COEFF);
            x2FilterSecondaryCoefficientAddress = yawAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeFILTER_SECONDARY_COEFF);

            x1AxisLinkAddress = linearAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeAXIS_LINK);
            x2AxisLinkAddress = yawAxis.AddressGet(RSIAxisAddressType.RSIAxisAddressTypeAXIS_LINK);
        }

        private void SetupEncoderMixing(bool enableGantry)
        {
            if (enableGantry)
            {
                // first scale encoders by half
                linearAxis.EncoderRatioSet(RSIMotorFeedback.RSIMotorFeedbackPRIMARY, gantryEncoderNumerator, gantryEncoderDenominator);
                yawAxis.EncoderRatioSet(RSIMotorFeedback.RSIMotorFeedbackPRIMARY,  gantryEncoderNumerator, gantryEncoderDenominator);

                mc.OS.Sleep(10);

                // mix encoders (add on linear, subtract for yaw)
                linearAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputFIRST, x1EncoderAddress);
                linearAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputSECOND, x2EncoderAddress);
                linearAxis.GantryTypeSet(RSIAxisGantryType.RSIAxisGantryTypeADD);

                yawAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputFIRST, x1EncoderAddress);
                yawAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputSECOND, x2EncoderAddress);
                yawAxis.GantryTypeSet(RSIAxisGantryType.RSIAxisGantryTypeSUBTRACT);
            }

            else
            {
                linearAxis.EncoderRatioSet(RSIMotorFeedback.RSIMotorFeedbackPRIMARY, defaultEncoderNumerator, defaultEncoderDenominator);
                yawAxis.EncoderRatioSet(RSIMotorFeedback.RSIMotorFeedbackPRIMARY, defaultEncoderNumerator, defaultEncoderDenominator);

                mc.OS.Sleep(10);

                linearAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputFIRST, x1EncoderAddress);
                linearAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputSECOND, x1EncoderAddress);
                linearAxis.GantryTypeSet(RSIAxisGantryType.RSIAxisGantryTypeNONE);

                yawAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputFIRST, x2EncoderAddress);
                yawAxis.FeedbackPointerSet(RSIAxisPositionInput.RSIAxisPositionInputSECOND, x2EncoderAddress);
                yawAxis.GantryTypeSet(RSIAxisGantryType.RSIAxisGantryTypeNONE);
            }
        }

        private void SetupFilterMixing(bool enableGantry)
        {
            if (enableGantry)
            {
                // mix X1 filter
                mc.MemorySet(x1FilterPrimaryPointerAddress, (int)mc.FirmwareAddressGet(x1AxisLinkAddress));
                mc.MemorySet(x1FilterSecondaryPointerAddress, (int)mc.FirmwareAddressGet(x2AxisLinkAddress));

                // mix X2 filter
                mc.MemorySet(x2FilterPrimaryPointerAddress, (int)mc.FirmwareAddressGet(x1AxisLinkAddress));
                mc.MemorySet(x2FilterSecondaryPointerAddress, (int)mc.FirmwareAddressGet(x2AxisLinkAddress));

                // setup X1 filter mixing coefficients
                mc.MemoryDoubleSet(x1FilterPrimaryCoefficientAddress, x1PrimaryCoeff);
                mc.MemoryDoubleSet(x1FilterSecondaryCoefficientAddress, x1SecondaryCoeff);

                // setup X2 filter mixing coefficients
                mc.MemoryDoubleSet(x2FilterPrimaryCoefficientAddress, x2PrimaryCoeff);
                mc.MemoryDoubleSet(x2FilterSecondaryCoefficientAddress, x2SecondaryCoeff);

            }

            else
            {
                // unmix X1 filter
                mc.MemorySet(x1FilterPrimaryPointerAddress, (int)mc.FirmwareAddressGet(x1AxisLinkAddress));
                mc.MemorySet(x1FilterSecondaryPointerAddress, (int)mc.FirmwareAddressGet(x1AxisLinkAddress));

                // unmix X2 filter
                mc.MemorySet(x2FilterPrimaryPointerAddress, (int)mc.FirmwareAddressGet(x2AxisLinkAddress));
                mc.MemorySet(x2FilterSecondaryPointerAddress, (int)mc.FirmwareAddressGet(x2AxisLinkAddress));

                // setup X1 filter defult coefficients
                mc.MemoryDoubleSet(x1FilterPrimaryCoefficientAddress, defaultPrimaryCoeff);
                mc.MemoryDoubleSet(x1FilterSecondaryCoefficientAddress, defaultSecondaryCoeff);

                // setup X2 filter default coefficients
                mc.MemoryDoubleSet(x2FilterPrimaryCoefficientAddress, defaultPrimaryCoeff);
                mc.MemoryDoubleSet(x2FilterSecondaryCoefficientAddress, defaultSecondaryCoeff);

            }
        }

    }
}
