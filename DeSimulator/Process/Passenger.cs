﻿using React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public class Passenger : Process
    {
        public Passenger(Simulation Sim, string Dest = "") : base(Sim)
        {
            Id = Counter++;
            State = PassengerState.Waiting;
            Destination = Dest;
            OnOffTime = 1;           
        }

        private static int Counter = 0;

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
                    TestOutput();
            }
        }
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
            Console.WriteLine(
                "Id: " + Id.ToString() +
                " Starting time: " + (BaseTime/60.0f).ToString() +
                " Wait time: " + (WaitingTime/60.0f).ToString() +
                " Travel time: " + (TravellingTime/60.0f).ToString() +
                " Total : " + Counter.ToString()
                );
        }
    }

    public enum PassengerState
    {
        Waiting,
        Traveling,
        Arrived
    }
}