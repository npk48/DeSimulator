using React;
using React.Tasking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public class BusStop
    {
        public BusStop(string Name)
        {
            this.Name = Name;
            Passengers = new ConcurrentDictionary<string, ConcurrentQueue<Passenger>>();
        }

        public string Name { get; private set; }


        public ConcurrentDictionary<string,ConcurrentQueue<Passenger>> Passengers { get; private set; }
    }
}
