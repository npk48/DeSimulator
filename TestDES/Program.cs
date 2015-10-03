using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using React;
using React.Distribution;
using React.Tasking;
using React.Monitoring;

namespace TestDES
{
    using DeSimulator;
    public class Program
    {
        static void Main(string[] args)
        {
            Simulator Sim = new Simulator();
            Sim.Config = new Config()
            {
                RunningTime = 60 * 60 * 8, // 60s * 30min
                Scheduler = new StaticScheduler(),
                Buses = new Dictionary<int, int>()
                {
                    {0, 2} // line 0 bus * 2
                },
                BusInterval = 60 * 5 // 60s * 5min
            };
            Sim.Start();
            Console.ReadKey();
        }
    }
}
