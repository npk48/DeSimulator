using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public class StaticScheduler : IScheduler
    {
        public void Update(ref List<string> Destination, ref int ExpectedTime, ref Dictionary<string, Queue<Passenger>> Passengers)
        {
            if(Destination.Count == 0)
                Destination = new List<string>(CityMap.Lines[1]);

            Shift(ref Destination);

            Route r;
            if (CityMap.Navigate(Destination[0], Destination[1], out r))
            {
                ExpectedTime = (int)(r.ExpectedTime * 60);
            }
            else
            {
                // reset to start point
                ExpectedTime = 0;
            }
        }

        private void Shift(ref List<string> Destination)
        {
            string first = Destination[0];
            for (int i = 1; i < Destination.Count; i++)
                Destination[i - 1] = Destination[i];
            Destination[Destination.Count - 1] = first;
        }
    }
}
