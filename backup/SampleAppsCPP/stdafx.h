// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#ifndef _WIN32_WINNT        // Allow use of features specific to Windows XP or later.                   
#define _WIN32_WINNT 0x0501    // Change this to the appropriate value to target other versions of Windows.
#endif                        

#include <stdio.h>
#include <tchar.h>

void AbsoluteMotionMain();
void AxisSettlingMain();
void AxisStatusMain();
void CammingMain();
void captureMain();
void configAmpFaultMain();
void controllerInterruptsMain();
void custom97Main();
void customHomeMain();
void DedicatedIOMain();
void driveMonitorMain();
void ErrorLogMain();
void FeedRateMain();
void FinalVelocityMain();
void GearingMain();
void homeMain();
void HardwareLimitsMain();
void HomeToNegativeLimitMain();
void HomingWithAKDdriveMain();
void IOwithAKDMain();
void MotionHoldReleasedByPositionMain();
void MotionHoldReleasedByDigitalInputMain();
void MotionHoldReleasedByDigitalInputMain();
void MotionHoldReleasedBySoftwareAddressMain();
void multiaxisMotionMain();
void memoryMain();
void pathMotionMain();
void PTmotionMain();
void PVTmotionMain();
void PVTmotionMultiAxisMain();
void PhantomAxisMain();
void PointToPointMultiaxisMotionMain();
void PTmotionWhileStoppingMain();
void RelativeMotionMain();
void RecorderMain();
void settleCriteriaMain();
void StopRateMain();
void streamingMotionBufferManagementMain();
void syncInterruptMain();
void SCurveMotionMain();
void SetUserUnitsMain();
void SingleAxisSyncOutputsMain();
void VelocitySetByAnalogInputValueMain();
void UserLimitDigitalInputActionMain();
void UserLimitGainChangeBasedOnPositionMain();
void UserLimitPositionOneConditionMain();
void UserLimitStateActionMain();

void UserLimitPositionOneConditionMain();
void UserLimitDigitalInputOneConditionMain();

// TODO: reference additional headers your program requires here
