using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSimulator
{
    public class DynamicScheduler : IScheduler
    {
        public void Update(int BusLine, ref List<string> Destination, ref int ExpectedTime, ref Dictionary<string, Queue<Passenger>> Passengers)
        {
            // if start
            if (Destination.Count == 0)
            {
                Destination = new List<string>(CityMap.Lines[BusLine]);
                ExpectedTime = 0;
                return;
            }

            // check if there's shortcut available
            string CurrentBusStop = Destination[0];
            List<Route> SelectedRoutes = new List<Route>();
            Route[] Routes = CheckAvailableRoutes(BusLine, CurrentBusStop);
            foreach (var r in Routes)
            {
                bool CanSkip = true;

                // verify if people want to go to the skipped bus stop
                string[] Skipped = CheckSkippedBusStops(BusLine, CurrentBusStop, r);               
                foreach(var s in Skipped)
                {
                    if(Passengers.Keys.ToList().Exists(x => x==s))
                    {
                        if (Passengers[s].Count != 0)
                            CanSkip = false;
                    }
                }

                // verify if people want to get on from the skipped bus stop
                foreach(var s in Skipped)
                {
                    if (CheckSkippedBusStopsGetOn(BusLine, CurrentBusStop, s, r))
                        CanSkip = false;
                }

                if (CanSkip)
                    SelectedRoutes.Add(r);
            }

            // choose the route with min expected time
            // if reached end
            if (SelectedRoutes.Count() == 0)
            {
                ExpectedTime = 0;
                Destination = new List<string>(CityMap.Lines[BusLine]);
                return;
            }
            var SelectedRoute = SelectedRoutes.OrderBy(x => x.ExpectedTime).First();

            // build Destination list
            Destination = BuildDestinationList(BusLine,CurrentBusStop, SelectedRoute);

            ExpectedTime = (int)(SelectedRoute.ExpectedTime * 60);

            

        }

        private Route[] CheckAvailableRoutes(int BusLine,string CurrentBusStop)
        {
            // test only select only one route

            //Route ro;
            //if (CurrentBusStop == CityMap.Lines[BusLine].Last())
            //    return new Route[] { };
            //CityMap.Navigate(CurrentBusStop, CityMap.Lines[BusLine][Array.IndexOf(CityMap.Lines[BusLine],CurrentBusStop)+1], out ro);
            //return new Route[] { ro };

            //
            string[] AllBusStops = CityMap.Lines[BusLine];
            var RemainStops = AllBusStops.Skip(Array.IndexOf(AllBusStops, CurrentBusStop)+1);

            var Routes = from r in CityMap.Routes
                         where (r.BusStopA == CurrentBusStop && RemainStops.ToList().Exists(x => x == r.BusStopB)) 
                         || (r.BusStopB == CurrentBusStop && RemainStops.ToList().Exists(x => x == r.BusStopA))
                         select r;
            return Routes.ToArray();
        }

        private string[] CheckSkippedBusStops(int BusLine,string CurrentBusStop, Route Route)
        {
            string[] AllBusStops = CityMap.Lines[BusLine];
            int CurrentPosition = Array.IndexOf(AllBusStops, CurrentBusStop)+1;            
            int DestinationPosition = Array.IndexOf(AllBusStops, Route.BusStopA == CurrentBusStop ? Route.BusStopB : Route.BusStopA)+1;
            int Count = DestinationPosition - CurrentPosition -1;
            string[] SkippedStops = AllBusStops.Skip(CurrentPosition).Take(Count).ToArray();
            return SkippedStops;
        }

        private bool CheckSkippedBusStopsGetOn(int BusLine,string CurrentBusStop,string Skipped, Route Route)
        {
            string[] AllBusStops = CityMap.Lines[BusLine];
            int DestinationPosition = Array.IndexOf(AllBusStops, Route.BusStopA == CurrentBusStop ? Route.BusStopB : Route.BusStopA) + 1;
            string[] RemainStops = AllBusStops.Skip(DestinationPosition).ToArray();

            bool GetOn = false;
            BusStop SkippedBusStop = CityMap.BusStops[Skipped];
            foreach(var s in RemainStops)
            {
                if (SkippedBusStop.Passengers.Keys.ToList().Exists(x => x == s))
                {
                    if (SkippedBusStop.Passengers[s].Count != 0)
                        GetOn = true;
                }
            }

            return GetOn;
        }

        private List<string> BuildDestinationList(int BusLine, string CurrentBusStop,Route Route)
        {
            List<string> Destinations = new List<string>();

            // add remaining bus stops after route destination include destination
            string[] AllBusStops = CityMap.Lines[BusLine];
            int DestinationPosition = Array.IndexOf(AllBusStops, Route.BusStopA == CurrentBusStop ? Route.BusStopB : Route.BusStopA);
            var RemainStops = AllBusStops.Skip(DestinationPosition);

            Destinations.AddRange(RemainStops);

            return Destinations;
        }
    }
}
