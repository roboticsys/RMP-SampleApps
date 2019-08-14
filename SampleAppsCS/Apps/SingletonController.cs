/// @example SingletonController.cs   Avoid multiple calls to create a controller and axes. 
/*  SingletonController.cs
 
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

 This application is designed to demonstrate how to avoid errors when from invalidating 
 RapidCode objects. 
*/

using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;

namespace SampleApplications
{

  public class InvalidController
  {


    public void Main()
    {

      MotionController mc1, mc2;
      mc1 = MotionController.CreateFromSoftware();
      mc1.AxisCountSet(1);

      Axis ax1 =  mc1.AxisGet(0);
      ax1.ActualPositionGet();
      //No error


      mc2 = MotionController.CreateFromSoftware();
      //Invalidates the controller object mc1

      Axis ax2 = null;
      try
      {
        ax2 = mc1.AxisGet(0);
        ax2.ActualPositionGet();
        //Error Expected
      }
      catch (RsiError)
      {
        string s = "Without a Singleton ...\n ";
        RsiError e;
        while (ax2.ErrorLogCountGet() > 0)
        {
          e = ax2.ErrorLogGet();
          s = s + e.Message + " --- " + e.number + ";   ";
        }
        Console.WriteLine(s);
      }

      ax1 = SingletonController.Instance.GetAxis(0);
      ax2 = SingletonController.Instance.GetAxis(0);

      //ax1 & ax2 now reference the same axis using the same instance of the controller
      ax2.ActualPositionGet();
      ax1 = SingletonController.Instance.GetAxis(0);
      ax2.ActualPositionGet();
      ax2 = SingletonController.Instance.GetAxis(0);
      ax2.ActualPositionGet();
      ax1 = SingletonController.Instance.GetAxis(0);
      ax2.ActualPositionGet();
   
    }

    public class SingletonController
    {
      private static SingletonController myController = null;
      private MotionController controller;
      private Dictionary<int, Axis> axisDictionary;

      protected SingletonController()
      {
        axisDictionary = new Dictionary<int, Axis>();
        controller = MotionController.CreateFromSoftware();
        if (CheckErrors(controller))
          throw new Exception("Controller Create Method returned an error");
      }

      public Axis GetAxis(int axisNumber)
      {
        if(!axisDictionary.ContainsKey(axisNumber))
          axisDictionary.Add(axisNumber, controller.AxisGet(axisNumber));
        return axisDictionary[axisNumber];
      }

      public static SingletonController Instance
      {
        get
        {
          if (myController == null || !CheckMotionValid(myController.controller))
          {
            myController = new SingletonController();
            Console.WriteLine("Create new Controller");
          }
          return myController;
        }
      }

      private static bool CheckMotionValid(IRapidCodeObject rapidObject)
      {
        RsiError e;
        while (rapidObject.ErrorLogCountGet() > 0)
        {
            e = rapidObject.ErrorLogGet();
            if (e.number == RSIErrorMessage.RSIAxisMessageAXIS_INVALID || e.number == RSIErrorMessage.RSIMotionMessageMOTION_INVALID)
            {
              return false;
            }
            else
            {
              //Some other error...
            }
        }
        return true;
      }

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
    }
  }
}
