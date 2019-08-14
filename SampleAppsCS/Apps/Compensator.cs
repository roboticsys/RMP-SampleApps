/// @example Compensator.cs   Configure and load a two dimensional Compensator table
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
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

    public class Compensator
    {
        MotionController controller;

        const int COMPENSATOR_COUNT             = 1;  // how many compensators to enable?
        const int COMPENSATOR_NUMBER            = 0;
        const int COMPENSATOR_DIMENSION         = 2;

        const int AXIS_X_NUMBER                 = 1;
        const double AXIS_X_RANGE_START         = 0.0;
        const double AXIS_X_RANGE_END           = 200000;
        const double AXIS_X_POINT_DELTA         = 50000;

        const int AXIS_Y_NUMBER                 = 0;
        const double AXIS_Y_RANGE_START         = 25000;
        const double AXIS_Y_RANGE_END           = 225000;
        const double AXIS_Y_POINT_DELTA         = 10000;

        const int AXIS_Z_NUMBER                 = 2;

        const int X_DIM  = (int)((int)(AXIS_X_RANGE_END-AXIS_X_RANGE_START)/AXIS_X_POINT_DELTA + 1);
        const int Y_DIM  = (int)((int)(AXIS_Y_RANGE_END-AXIS_Y_RANGE_START)/AXIS_Y_POINT_DELTA + 1);


        private bool CheckErrors(IRapidCodeObject rapidObject)
        {
            bool errors = false;

            while (rapidObject.ErrorLogCountGet() > 0)
            {
                errors = true;
                Console.WriteLine(rapidObject.ErrorLogGet().Message);
            }
            return errors;
        }

        private void ConfigureAndLoadCompensator()
        {
            controller.CompensatorTwoDimensionConfigSet(COMPENSATOR_NUMBER,
                AXIS_X_NUMBER, AXIS_X_RANGE_START, AXIS_X_RANGE_END, AXIS_X_POINT_DELTA,
                AXIS_Y_NUMBER, AXIS_Y_RANGE_START, AXIS_Y_RANGE_END, AXIS_Y_POINT_DELTA,
                AXIS_Z_NUMBER);

            double[] compensatorTable = new double[Y_DIM * X_DIM] {
                           0,      0,      0,      0,      0  ,
                           100,    200,    -200,   -100,   0  ,
                           200,    400,    -400,   -200,   0   ,
                           300,    600,    -600,   -300,   0   ,
                           400,    800,    -800,   -400,   0   ,
                           500,    1000,   -1000,  -500,   0   ,
                           600,    1200,   -1200,  -600,   0   ,
                           700,    1400,   -1400,  -700,   0   ,
                           800,    1600,   -1600,  -800,   0   ,
                           900,    1800,   -1800,  -900,   0   ,
                           1000,   2000,   -2000,  -1000,  0   ,
                           900,    1800,   -1800,  -900,   0   ,
                           800,    1600,   -1600,  -800,   0   ,
                           700,    1400,   -1400,  -700,   0   ,
                           600,    1200,   -1200,  -600,   0   ,
                           500,    1000,   -1000,  -500,   0   ,
                           400,    800,    -800,   -400,   0   ,
                           300,    600,    -600,   -300,   0   ,
                           200,    400,    -400,   -200,   0   ,
                           100,    200,    -200,   -100,   0   ,
                           0,      0,      0,      0,      0   ,
                    };

            controller.CompensatorTableSet(COMPENSATOR_NUMBER, compensatorTable);
        }


        public void Main()
        {
            //RapidCode create methods NEVER throw exceptions but they will log errors
            controller = MotionController.Create();
            CheckErrors(controller);

            // do this during initialization, before creating Axis objects, etc.
            controller.CompensatorCountSet(COMPENSATOR_COUNT);
            controller.CompensatorPointCountSet(COMPENSATOR_NUMBER, (int)(X_DIM * Y_DIM));


            // configure and load a compensation table
            ConfigureAndLoadCompensator();
        }
    }
}
