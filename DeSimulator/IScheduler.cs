using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public interface IScheduler
    {
        void Update(int BusLine,ref List<string> Destination, ref int ExpectedTime, ref Dictionary<string, Queue<Passenger>> Passengers);
    }
}
