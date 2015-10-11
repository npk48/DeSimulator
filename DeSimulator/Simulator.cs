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

        public delegate void OutputResult(Dictionary<string,int> BusstopRecords, List<PassengerTrackData> PassengerRecords, float TotalWaiting, float TotalTravelling);
        public static OutputResult OutputResultHandler;

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
                    Bus[line].Add(new Bus(this, line,Config.MaxPassenger));
            }
            Scheduler = Config.Scheduler;
            RunTime = Config.RunningTime;
            Passenger.Counter = 0;
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
            // initialize
            SimResult.Init();
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
                    for (int i=0; i<Count;i++)
                    {

                        string[] SelectedLine = { "" };
                        SelectedLine = CityMap.Lines[1];
                        do
                        {
                            SelectedLine = CityMap.Lines[rd.Next(0, CityMap.Lines.Keys.Count)];
                            //SelectedLine = CityMap.Lines[1];
                        } while (!SelectedLine.ToList().Exists(x => x == B.Name));

                        var Destinations = (from d in SelectedLine
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

            OutputResultHandler(
                SimResult.BusstopRecords.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value),
                SimResult.PassengerRecords.ToList(),
                SimResult.TotalWaiting,
                SimResult.TotalTravelling
                );
            // release
            yield break;
        }
    }
}
