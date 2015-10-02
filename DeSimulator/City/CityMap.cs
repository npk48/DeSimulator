﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSimulator
{
    public static class CityMap
    {
        public static Dictionary<string, BusStop> BusStops = new Dictionary<string, BusStop>();
        public static Route[] Routes =
        {
            new Route("Bronziet",              "Smaragd",               350.0f,  1.0f),
            new Route("Bronziet",              "Paasberglaan",          700.0f,  2.0f),
            new Route("Smaragd",               "Deneerdbrand",          1200.0f, 3.0f),
            new Route("Smaragd",               "Hondsruglaan",          500.0f,  1.0f),
            new Route("Smaragd",               "WC Woensel",            2300.0f, 6.0f),
            new Route("Hondsruglaan",          "Paasberglaan",          300.0f,  1.0f),
            new Route("Hondsruglaan",          "WC Woensel",            2400.0f, 6.0f),
            new Route("Paasberglaan",          "Grebbeberglaan",        290.0f,  1.0f),
            new Route("Grebbeberglaan",        "WC Woensel",            2500.0f, 6.0f),
            new Route("Grebbeberglaan",        "Deneerdbrand",          300.0f,  1.0f),
            new Route("Deneerdbrand",          "Noordzeelaan",          350.0f,  1.0f),
            new Route("Noordzeelaan",          "Lohengrinlaan",         500.0f,  2.0f),
            new Route("Lohengrinlaan",         "Koning Arthurlaan",     350.0f,  1.0f),
            new Route("Koning Arthurlaan",     "WC Woensel",            350.0f,  1.0f),
            new Route("WC Woensel",            "Catharina Zh oost",     400.0f,  1.0f),
            new Route("Catharina Zh oost",     "Generaal Hardenbergpad",700.0f,  2.0f),
            new Route("Generaal Hardenbergpad","Drossen straat",        400.0f,  1.0f),
            new Route("Drossen straat",        "Generaal Coenderslaan", 240.0f,  1.0f),
            new Route("Generaal Coenderslaan", "Rachelsmolen",          850.0f,  2.0f),
            new Route("Generaal Coenderslaan", "Peppelrode",            750.0f,  2.0f),
            new Route("Peppelrode",            "MMC Eindhoven",         190.0f,  0.5f),
            new Route("MMC Eindhoven",         "Rachelsmolen",          600.0f,  3.0f),
            new Route("Rachelsmolen",          "Station Eindhoven",     1100.0f, 5.0f)
        };

        public static void Init()
        {
            foreach(var r in Routes)
            {
                BusStop t;
                if (!BusStops.TryGetValue(r.BusStopA, out t))
                    BusStops.Add(r.BusStopA, new BusStop(r.BusStopA));
                if (!BusStops.TryGetValue(r.BusStopB, out t))
                    BusStops.Add(r.BusStopB, new BusStop(r.BusStopB));
            }
        }

        public static bool Navigate(string From,string To,out Route Result)
        {
            Result = new Route();
            var result = from r in Routes
                         where (r.BusStopA == From && r.BusStopB == To) || (r.BusStopA == To && r.BusStopB == From)
                         select r;
            if (result.Count() == 0)
                return false;
            Result = result.First();
            return true;
        }
    }
}
