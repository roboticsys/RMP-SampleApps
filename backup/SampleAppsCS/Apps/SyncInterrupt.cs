/// @example SyncInterrupt.cs   MotionController will generate a periodic PCI interrupt
using System;
using System.Collections.Generic;
using System.Text;
using RSI.RapidCode.dotNET;
using RSI.RapidCode.dotNET.Enums;
using System.Threading;

namespace SampleApplications
{
    public class TimeChecker
    {
        const int MS_PER_SECOND = 1000;

        double frequency,  current, previous, delta, deltaTime, minTime, maxTime;
        string name;

        public TimeChecker(string name)
        {
            minTime = double.MaxValue;
            maxTime = double.MinValue;
            this.name = name;
        }

        public void CalcUpdate(double currentTime, int loops)
        {
            current = currentTime;
            delta = current - previous;
            previous = current;
            deltaTime = (double)(delta * (double)(1 / (double)frequency)) * MS_PER_SECOND;

            if (loops > 2)  // can't check min/max until it has run through more than once
            {
                if (deltaTime > maxTime)
                {
                    maxTime = deltaTime;
                }
                if (deltaTime < minTime)
                {
                    minTime = deltaTime;
                }
            }
        }

        public void FrequencySet(double frequency)
        {
            this.frequency = frequency;
        }

        public string ResultsGet()
        {
            return String.Format(name + " delta {0:0.00} min {1:0.00} max {2:0.00} ", deltaTime, minTime, maxTime);
        }
    }



    class SyncInterrupt
    {
        const int INTERRUPT_EVERY_N_SAMPLES = 5;   // have the controller generate an interrupt after this many sample periods
        const int PRINT_EVERY_N_INTERRUPTS = 100;
        const ThreadPriority PRIORITY = ThreadPriority.Highest;

        MotionController mc;
        Thread fastThread;
        bool done = false;

        TimeChecker performanceTimeChecker;
        TimeChecker rmpSampleTimeChecker;
        int loopCount = 0;


        public void FixtureSetup()
        {
            mc = MotionController.Create();
            mc.SyncInterruptPeriodSet(INTERRUPT_EVERY_N_SAMPLES);
            fastThread = new Thread(SyncInterruptLoop);
            fastThread.Priority = PRIORITY;
        }


        public void FixtureTearDown()
        {
            Kill();
        }

        public void SyncInterruptLoop()
        {
            performanceTimeChecker = new TimeChecker("Performance Timer");
            rmpSampleTimeChecker = new TimeChecker("RMP Sample Timer");

            performanceTimeChecker.FrequencySet(mc.OS.PerformanceTimerFrequencyGet());
            rmpSampleTimeChecker.FrequencySet(mc.SampleRateGet());  

            mc.SyncInterruptEnableSet(true);
            while (!done)
            {
                int interruptTimeRMP = mc.SyncInterruptWait();

                // your algorithms go here

                // see how long it's been since last interrupt
                performanceTimeChecker.CalcUpdate(mc.OS.PerformanceTimerCountGet(), loopCount);
                rmpSampleTimeChecker.CalcUpdate(interruptTimeRMP, loopCount);

                if (loopCount % PRINT_EVERY_N_INTERRUPTS == 0) // don't print every time
                {
                    Console.Write("Iteration " + loopCount + " ");
                    Console.Write(performanceTimeChecker.ResultsGet());
                    Console.WriteLine(rmpSampleTimeChecker.ResultsGet());
                }
                loopCount++;
            }
        }
 


        public void Start()
        {
            fastThread.Start();

            // sleep a while so we can see the console writes from the new thread
            mc.OS.Sleep(86400 * 1000 );  // sleep a day
            //mc.OS.Sleep(5000);
        }


        public void Kill()
        {
            done = true;
            mc.SyncInterruptEnableSet(false);
            fastThread.Abort();
        }
    }
}

