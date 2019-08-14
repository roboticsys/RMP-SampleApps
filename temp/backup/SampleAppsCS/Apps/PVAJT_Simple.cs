/// @example PVAJT_Simple.cs   Calculate a simple point-to-point jerk limited profile and use MovePVAJT. 
/// \image html PVAJT_Simple.jpg "Resulting plot of Velocity, Acceleration and Jerk"
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
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    public class PVAJT_Simple
    {
        const double START_POSITION = 0.0;
        const double COUNTS_PER_CM = 1000.0;
        const int X = 0;  // axis number


        MotionController controller;
        Axis axis;

        // lists/arrays for storing the PVAJT
        List<double> jerks = new List<double>();
        List<double> accelerations = new List<double>();
        List<double> velocities = new List<double>();
        List<double> positions = new List<double>();
        List<double> times = new List<double>();

        // used to update PVAJT arrays
        double currentAcceleration = 0.0;
        double currentVelocity = 0.0;
        double currentPosition = START_POSITION;

        double samplePeriod;



        public void Setup()
        {
            controller = MotionController.Create();
            axis = controller.AxisGet(X);
            axis.PositionSet(START_POSITION);
            axis.ClearFaults();
            axis.AmpEnableSet(true);

            // calculate samplePeriod
            samplePeriod = 1.0 / controller.SampleRateGet();
        }


        // returns a value in seconds, but truncated to whole sample periods (0.0005, 0.001, 0.0015, etc.)
        private double ConvertTimeToWholeSamplePeriods(double seconds)
        {
            if (seconds < samplePeriod)
                return 0.0;

            return Math.Floor(seconds / samplePeriod) * samplePeriod;
        }


        private void UpdatePVAJTArrays(double time, double jerk)
        {
            //add values to arrays
            jerks.Add(jerk);
            accelerations.Add(currentAcceleration);
            velocities.Add(currentVelocity);
            positions.Add(currentPosition);
            times.Add(time);

            // update pos,vel,acc using standard kinematic equations, so they'll be ready next time they are added to the arrays
            currentPosition += currentVelocity * time + (0.5 * currentAcceleration * time * time) + (1.0 / 6.0 * jerk * time * time * time);
            currentVelocity += currentAcceleration * time + (0.5 * jerk * time * time);
            currentAcceleration += jerk * time;
        }

        // units are counts, counts/second, counts/second^2, counts/second^3)
        private void CalculatePVAJTForStraightLine(double deltaPosition, double maxVelocity, double maxAcceleration, double maxJerk)
        {
            double accelTime = 0.0;
            double velocityTime = 0.0;
            double jerkTime = 0.0;
            double positionDeltaDuringJerk = 0.0;
            long direction = 0;

            if (deltaPosition < 0)
            {
                direction = 1;
                deltaPosition = -deltaPosition;
            }

            // Calculate the accel time.  If the accel time is negative, then the target
            //   accel must be adjusted so that ta = 0.  This will preserve the specified
            //    velocity.  

            if (maxAcceleration * maxAcceleration / maxJerk > maxVelocity)
            {
                accelTime = 0.0;
                maxAcceleration = Math.Sqrt(maxJerk * maxVelocity);
            }
            else
                accelTime = ((maxVelocity - maxAcceleration * maxAcceleration / maxJerk) / maxAcceleration);

            // Calculate the distance during the accel and jerk profile portion.
            positionDeltaDuringJerk = (2.0 * (maxAcceleration * maxAcceleration * maxAcceleration) / (maxJerk * maxJerk))
                        + ((3.0 * maxAcceleration * maxAcceleration * accelTime) / maxJerk)
                        + (maxAcceleration * accelTime * accelTime);

            if (positionDeltaDuringJerk > deltaPosition)		// Is the distance during jerk and accel too far? 
            {
                velocityTime = 0.0; 			// Remove the constant velocity profile 
                if (accelTime > 0.0)
                {
                    double x;
                    x = Math.Sqrt(1 + (4 * deltaPosition * maxJerk * maxJerk / (maxAcceleration * maxAcceleration * maxAcceleration)));

                    if (x > 3.0)
                    {
                        jerkTime = maxAcceleration / maxJerk;
                        accelTime = (0.5 * jerkTime) * (x - 3.0);
                    }
                    else
                        accelTime = 0.0;
                }
                if (accelTime < 1.0)
                {
                    accelTime = 0.0;
                    jerkTime = Math.Pow((deltaPosition / (2.0 * maxJerk)), .3333333);
                }
            }
            else
            {
                velocityTime = (deltaPosition - positionDeltaDuringJerk) / maxVelocity;
                jerkTime = maxAcceleration / maxJerk;
            }

            if (jerkTime < 0.0)
            {
                throw new Exception("Can't have negative jerk time, illegal parameter.");
            }

            // Reverse calculate to eliminate roundoff errors.  Since the controller uses 
            //    whole time periods (no fractional time periods) to calculate the
            //    trajectory, the jerk, accel, or vel, may need to be adjusted to
            //    guarantee the accuracy of the final position.
            jerkTime = ConvertTimeToWholeSamplePeriods(jerkTime);
            maxAcceleration = jerkTime * maxJerk;

            if (accelTime != 0.0)		// Is there an accel portion? 
            {
                if (velocityTime == 0.0)
                {
                    double x;
                    x = Math.Sqrt(1 + (4 * deltaPosition * maxJerk * maxJerk / (maxAcceleration * maxAcceleration * maxAcceleration)));
                    accelTime = (0.5 * maxAcceleration / maxJerk) * (x - 3.0);		// Recalculate accelTime
                }
                else
                    accelTime = ((maxVelocity - maxAcceleration * maxAcceleration / maxJerk) / maxAcceleration);	// Recalculate accelTime
                accelTime = ConvertTimeToWholeSamplePeriods(accelTime);
            }

            if (velocityTime != 0.0) 		/* Is there a vel portion? */
            {
                positionDeltaDuringJerk = (2.0 * (maxAcceleration * maxAcceleration * maxAcceleration) / (maxJerk * maxJerk))
                            + ((3.0 * maxAcceleration * maxAcceleration * accelTime) / maxJerk)
                            + (maxAcceleration * accelTime * accelTime);
                maxVelocity = accelTime * maxAcceleration + (maxAcceleration * maxAcceleration / maxJerk);	/* Recalculate vel */
                velocityTime = (deltaPosition - positionDeltaDuringJerk) / maxVelocity;				/* Recalculate tv */
                velocityTime = ConvertTimeToWholeSamplePeriods(velocityTime);
            }

            maxJerk = deltaPosition / (jerkTime * (jerkTime + accelTime) * (2 * jerkTime + accelTime + velocityTime));

            if (direction > 0)
                maxJerk = -maxJerk;


            // first jerk
            UpdatePVAJTArrays(jerkTime, maxJerk);

            // add constant acceleration if needed
            if (accelTime > 0.0)
                UpdatePVAJTArrays(accelTime, 0);

            // jerk to constant (or maximum) velocity
            UpdatePVAJTArrays(jerkTime, -maxJerk);

            // add constant velocity, if needed
            if (velocityTime > 0.0)
                UpdatePVAJTArrays(velocityTime, 0);

            // first jerk during decel
            UpdatePVAJTArrays(jerkTime, -maxJerk);

            /// add constant deceleration if needed
            if (accelTime > 0.0)
                UpdatePVAJTArrays(accelTime, 0);

            // add final jerk should leave us at zero
            UpdatePVAJTArrays(jerkTime, maxJerk);


            // add final position to the end of the arrays
            jerks.Add(0.0);
            accelerations.Add(0.0);
            velocities.Add(0.0);
            positions.Add(deltaPosition);
            times.Add(samplePeriod);

        }



        public void StraightLine()
        {
            // calculate array values for single-axis stright line
            CalculatePVAJTForStraightLine(25 * COUNTS_PER_CM, 40 * COUNTS_PER_CM, 300 * COUNTS_PER_CM, 8000 * COUNTS_PER_CM);

            // use RapidCode PVAJT motion
            axis.MovePVAJT(positions.ToArray(), velocities.ToArray(), accelerations.ToArray(), jerks.ToArray(), times.ToArray(), positions.Count, -1, false, true);
        }
    }
}




