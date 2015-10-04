using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSimulator
{
    public static class SimResult
    {
        public static void Init()
        {
            BusstopRecords = new ConcurrentDictionary<string, int>();
            PassengerRecords = new List<PassengerTrackData>();
            TotalWaiting = 0.0f;
            TotalTravelling = 0.0f;
        }

        public static ConcurrentDictionary<string, int> BusstopRecords;
        public static List<PassengerTrackData> PassengerRecords;
        public static float TotalWaiting;
        public static float TotalTravelling;

        public static int UpdateBusstopRecord(string Name,int Value)
        {
            return BusstopRecords[Name] + 1;
        }
    }

    public struct PassengerTrackData
    {
        public float BaseTime;
        public float WaitingTime;
        public float TravellingTime;
        public string From;
        public string To;
        public int Id;
    }
}
