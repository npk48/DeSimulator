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
            //throw new NotImplementedException();
            if (Destination.Count <= 1)
            {
                var d = new string[]
                {
                    "Bronziet",
                    "Smaragd",
                    "Hondsruglaan",
                    "Paasberglaan",
                    "Grebbeberglaan",
                    "Deneerdbrand",
                    "Noordzeelaan",
                    "Lohengrinlaan",
                    "Koning Arthurlaan",
                    "WC Woensel",
                    "Catharina Zh oost",
                    "Generaal Hardenbergpad",
                    "Drossen straat",
                    "Generaal Coenderslaan",
                    "Peppelrode",
                    "MMC Eindhoven",
                    "Rachelsmolen",
                    "Station Eindhoven"
                };
                Destination = new List<string>(d);
            }
            else
            {
                Destination.RemoveAt(0);
                if(Destination.Count != 1)
                {
                    Route r;
                    if (CityMap.Navigate(Destination[0], Destination[1], out r))
                    {
                        ExpectedTime = (int)(r.ExpectedTime * 60);
                    }
                }
                
            }
            
        }
    }
}
