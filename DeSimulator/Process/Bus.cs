﻿using React;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public class Bus : Process
    {
        public Bus(Simulation Sim, int Id = 0, int Max = 999) : base(Sim)
        {
            Identity = Id;
            MaxPassengers = Max;
            Passengers = new Dictionary<string, Queue<Passenger>>();
            Destination = new List<string>();
        }

        public int Identity { get; set; }

        private int ExpectedTime;
        private List<string> Destination;
        private Dictionary<string, Queue<Passenger>> Passengers;
        private int MaxPassengers = 999;

        protected override IEnumerator<Task> GetProcessSteps()
        {
            while(Now < Simulator.RunTime)
            {
                // drive to destination
                Simulator.Scheduler.Update(Identity, ref Destination, ref ExpectedTime, ref Passengers);
                yield return Delay(ExpectedTime); // + random traffic jam

                // pick passengers
                int PassengerCount = 0;
                foreach (var queue in Passengers.Values)
                    PassengerCount += queue.Count;

                BusStop Arrived = CityMap.BusStops[Destination[0]];                
                foreach(var dest in Destination)
                {
                    if(!Passengers.Keys.ToList().Exists(x => x == dest))
                        Passengers[dest] = new Queue<Passenger>();
                    ConcurrentQueue<Passenger> queue;
                    if(Arrived.Passengers.TryGetValue(dest, out queue))
                    {
                        Passenger passenger;
                        while(queue.Count != 0)
                        {
                            if (queue.TryDequeue(out passenger))
                            {
                                yield return passenger;
                                if (PassengerCount > MaxPassengers)
                                    passenger.Crowdness = 1.0f;
                                else
                                    passenger.Crowdness = PassengerCount / (MaxPassengers * 1.0f);
                                Passengers[dest].Enqueue(passenger);
                            }
                        }                       
                    }
                }

                // off passengers
                Queue<Passenger> Off;
                if(Passengers.TryGetValue(Destination[0], out Off))
                {
                    while (Off.Count != 0)
                    {
                        Passenger passenger = Off.Dequeue();
                        yield return passenger;
                    }
                        
                }

                
                foreach (var queue in Passengers.Values)
                {
                    foreach (var p in queue)
                    {
                        float Max = MaxPassengers;
                        if (PassengerCount > Max)
                            p.Crowdness = 1.0f;
                        else
                            p.Crowdness = PassengerCount / Max;
                    }
                }
            }
            yield break;
        }
    }
}
