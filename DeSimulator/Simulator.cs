using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using React;
using React.Distribution;
using React.Tasking;
using React.Monitoring;
using System.Collections.Concurrent;
using System.Threading;

namespace DeSimulator
{
    public class Simulator : Simulation
    {
        public Simulator()
        {
                    
        }

        ~Simulator()
        {
            if (Backend != null)
                Backend.Join();
        }

        private Thread Backend;

        public delegate void UpdateBusStopViewerDelegate(string[] Name, int[] Number);
        public delegate void UpdateTotalTimeViewerDelegate(float Waiting, float Travelling);
        public static UpdateBusStopViewerDelegate UpdateBusStopViewerHandler;
        public static UpdateTotalTimeViewerDelegate UpdateTotalTimeViewerHandler;

        public void Start()
        {
            //Run(new Process(this, Generator)); return;
            if (Backend != null)
            {
                Console.Clear();
                Backend.Join();
            }
                
                         
            Backend = new Thread(
                new ThreadStart(
                    new Action(() => 
                    {
                        Run(new Process(this, Generator));
                    })));
            Backend.Start();
        }

        private void Init()
        {
            CityMap.Init();
            Bus = new Dictionary<int, List<Bus>>();
            foreach (int line in Config.Buses.Keys)
            {
                Bus.Add(line, new List<Bus>());
                for (int i = 0; i < Config.Buses[line]; i++)
                    Bus[line].Add(new Bus(this, line));
            }
            Scheduler = Config.Scheduler;
            RunTime = Config.RunningTime;
            Passenger.Travelling = Passenger.Waiting = Passenger.Counter = 0;
        }

        private Config _Config;
        public Config Config
        {
            get
            {
                return _Config;
            }
            set
            {
                _Config = value;
                Init();
            }
        }

        public static long RunTime;

        public static IScheduler Scheduler;
        public Dictionary<int,List<Bus>> Bus { get; private set; }      

        private Normal IntervalRate = new Normal();
        private int Interval
        {
            get
            {
                double r;
                do
                    r = IntervalRate.NextDouble();
                while (r < 1L);
                return (int)r;
            }
        }

        private Exponential ArrivalRate = new Exponential();
        private int Arrival
        {
            get
            {
                double r;
                do
                    r = ArrivalRate.NextDouble();
                while (r <= 0L);
                return (int)r;
            }
        }

        private IEnumerator<Task> Generator(Process Self, object Data)
        {
            // initialize for data view
            Dictionary<string, int> BusStopPassengers = new Dictionary<string, int>();
            foreach (var b in CityMap.BusStops.Keys)
                BusStopPassengers.Add(b, 0);

            // initialize
            foreach (var line in Bus.Values)
            {
                long Offset = 0;
                foreach (var bus in line)
                {
                    bus.Activate(null,Offset);
                    Offset += Config.BusInterval;
                }     
            }

            IntervalRate.Mean = 60; // 60s as random step time expectation
            IntervalRate.StandardDeviation = 30; // +- 30s around 60s           

            var rd = new Random();
            // loop
            while(Now<RunTime)
            {
                int Step = Interval;
                ArrivalRate.Mean = 1 * Step / 60.0; // 1 passenger per 60s
                foreach (var B in CityMap.BusStops.Values)
                {
                    int Count = Arrival;
                    BusStopPassengers[B.Name] += Count;
                    for (int i=0; i<Count;i++)
                    {
                        var Destinations = (from d in CityMap.TestLine
                                            where d != B.Name
                                            select d).ToList();
                        string Destination = Destinations[rd.Next(0, Destinations.Count)];
                        if (!B.Passengers.Keys.ToList().Exists(x => x == Destination))
                            B.Passengers[Destination] = new ConcurrentQueue<Passenger>();
                        B.Passengers[Destination].Enqueue(new Passenger(this, B.Name, Destination));
                    }
                }
                yield return Self.Delay(Step); // random time 
            }


            //string[] Names = CityMap.BusStops.Keys.ToArray();
            //int[] Values = new int[Names.Length];
            //int Index = 0;
            //foreach (var b in CityMap.BusStops.Values)
            //{
            //    int Count = 0;
            //    foreach (var q in b.Passengers.Values)
            //    {
            //        Count += q.Count;
            //    }
            //    Names[Index] = b.Name;
            //    Values[Index] = Count;
            //    Index++;
            //}
            UpdateBusStopViewerHandler(BusStopPassengers.Keys.ToArray(), BusStopPassengers.Values.ToArray());
            UpdateTotalTimeViewerHandler(Passenger.Waiting, Passenger.Travelling);
            // release
            yield break;
        }
    }
}
