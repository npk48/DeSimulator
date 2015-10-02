using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using React;
using React.Distribution;
using React.Tasking;
using React.Monitoring;
using System.Collections.Concurrent;

namespace DeSimulator
{
    public class Simulator : Simulation
    {
        public Simulator()
        {
            Bus = new Dictionary<int, List<DeSimulator.Bus>>();

            // test only
            Bus.Add(0, new List<DeSimulator.Bus>());
            Bus[0].Add(new Bus(this,0));
            Scheduler = new StaticScheduler();
            CityMap.Init();
        }

        public static long RunTime = 60 * 30; // 60s * 30min

        public static IScheduler Scheduler;
        public Dictionary<int,List<Bus>> Bus { get; private set; }      

        private Normal IntervalRate = new Normal();
        private int Interval
        {
            get
            {
                int r;
                do
                    r = (int)IntervalRate.NextDouble();
                while (r <= 0L);
                return r;
            }
            set
            {
                IntervalRate.Mean = value;
                IntervalRate.StandardDeviation = value;
            }
        }

        private Exponential ArrivalRate = new Exponential();
        private int Arrival
        {
            get
            {
                int r;
                do
                    r = (int)ArrivalRate.NextDouble();
                while (r <= 0L);
                return r;
            }
            set
            {
                ArrivalRate.Mean = value;
            }
        }

        public IEnumerator<Task> Generator(Process Self, object Data)
        {
            // initialize
            foreach(var line in Bus.Values)
                foreach(var bus in line)
                    bus.Activate(null);

            Interval = 60;
            int Step = Interval;
            // loop
            while(Now<RunTime)
            {
                Arrival = 1 + Step / 60; // 1 passenger per 60s
                foreach (var B in CityMap.BusStops.Values)
                {
                    if (B.Name != "Drossen straat") continue; // test only, from DrossenStraat to StationEindhoven
                    int Count = Arrival;
                    for(int i=0; i<Count;i++)
                    {
                        string Destination = "Station Eindhoven";
                        if (!B.Passengers.Keys.ToList().Exists(x => x == Destination))
                            B.Passengers[Destination] = new ConcurrentQueue<Passenger>();
                        B.Passengers[Destination].Enqueue(new Passenger(this, Destination));
                    }
                }
                yield return Self.Delay(Step); // random time 
            }

            // release
            yield break;
        }
    }
}
