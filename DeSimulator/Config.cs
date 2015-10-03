using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public struct Config
    {
        public long RunningTime;
        public IScheduler Scheduler;
        public Dictionary<int, int> Buses; // line:#bus
        public long BusInterval;
    }
}
