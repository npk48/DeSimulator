﻿using React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public class Passenger : Process
    {
        public Passenger(Simulation Sim, string From = "", string To = "") : base(Sim)
        {
            Id = Counter++;           
            Start = From;
            Destination = To;
            OnOffTime = 1;
            State = PassengerState.Waiting;
        }

        public static int Counter = 0;

        public float Crowdness = 0;

        private int Id;

        private PassengerState _State;
        public PassengerState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
                if (value == PassengerState.Waiting)
                    BaseTime = Simulation.Now;
                if (value == PassengerState.Traveling)
                    WaitingTime = Simulation.Now - BaseTime;
                if (value == PassengerState.Arrived)
                    TravellingTime = Simulation.Now - BaseTime - WaitingTime;
                
                if (value == PassengerState.Arrived)
                {
                    // save all result in a result data class
                    SimResult.BusstopRecords.AddOrUpdate(Start, 1, SimResult.UpdateBusstopRecord);
                    SimResult.PassengerRecords.Add(new PassengerTrackData()
                    {
                        Id = Id,
                        BaseTime = BaseTime / 60.0f,
                        WaitingTime = WaitingTime / 60.0f,
                        TravellingTime = TravellingTime / 60.0f,
                        From = Start,
                        To = Destination,
                        Crowdness = Crowdness
                    });
                    SimResult.TotalWaiting += WaitingTime / 60.0f;
                    SimResult.TotalTravelling += TravellingTime / 60.0f;
                    TestOutput();
                }                 
            }
        }

        public string Start { get; set; }
        public string Destination { get; set; }
        public int OnOffTime { get; private set; }
        public long BaseTime { get; private set; }
        public long WaitingTime { get; private set; }
        public long TravellingTime { get; private set; }

        protected override IEnumerator<Task> GetProcessSteps()
        {
            yield return Delay(OnOffTime);
            switch(State)
            {
                case PassengerState.Waiting:
                    State = PassengerState.Traveling;
                    break;
                case PassengerState.Traveling:
                    State = PassengerState.Arrived;
                    break;
                case PassengerState.Arrived:
                    throw new Exception("Passenger already arrived, cannot process any more");
            }
            yield break;
        }

        private void TestOutput()
        {
            Console.WriteLine("Id: " + Id.ToString());
            Console.WriteLine(" From " + Start + " to " + Destination);
            Console.WriteLine(" Waiting Time: " + (WaitingTime / 60.0f).ToString() + "min");
            Console.WriteLine(" Travelling Time: " + (TravellingTime / 60.0f).ToString() + "min");
        }
    }

    public enum PassengerState
    {
        Waiting,
        Traveling,
        Arrived
    }
}
