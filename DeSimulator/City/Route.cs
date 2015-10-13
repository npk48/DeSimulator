using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public struct Route
    {
        public Route(string BusStopA, string BusStopB, float Distance,float ExpectedTime,float Crowdness = 0.0f)
        {
            this.BusStopA = BusStopA;
            this.BusStopB = BusStopB;
            this.Distance = Distance;
            this.ExpectedTime = ExpectedTime;
            this.Crowdness = Crowdness;
        }

        public string BusStopA { get; set; }

        public string BusStopB { get; set; }

        public float Distance { get; private set; }

        public float ExpectedTime { get; private set; }

        public float Crowdness { get; set; }
    }
}
