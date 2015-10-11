using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DeSimulator;

namespace DesGui
{
    public class DataViewer
    {
        public ObservableCollection<BusStopViewer> BusStops { get; set; }
        public ObservableCollection<TotalTimeViewer> TotalTimes { get; set; }
        public ObservableCollection<DetailedBusStopViewer> BusStopWaiting { get; set; }
        public ObservableCollection<DetailedBusStopViewer> BusStopTravelling { get; set; }

        public ObservableCollection<SatisficationViewer> Satisfication { get; set; }

        public object _SelectedItem;
        public object SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnSelectedItem(value);
            }
        }

        public delegate void OnSelectedItemDelegate(object Selected);
        public event OnSelectedItemDelegate OnSelectedItem = delegate { };

        public DataViewer()
        {
            BusStops = new ObservableCollection<BusStopViewer>();
            TotalTimes = new ObservableCollection<TotalTimeViewer>();
            BusStopWaiting = new ObservableCollection<DetailedBusStopViewer>();
            BusStopTravelling = new ObservableCollection<DetailedBusStopViewer>();
            Satisfication = new ObservableCollection<SatisficationViewer>();
            OnSelectedItem += OnBusStopSelected;
        }

        private Dictionary<string, int> BusstopRecords;
        private List<PassengerTrackData> PassengerRecords;

        public void Clear()
        {
            BusStops.Clear();
            TotalTimes.Clear();
            BusStopWaiting.Clear();
            BusStopTravelling.Clear();
            Satisfication.Clear();
        }

        private void OnBusStopSelected(object Selected)
        {      
            BusStopViewer B = Selected as BusStopViewer; 
            if (B == null) return;
            
            BusStopWaiting.Clear();
            BusStopTravelling.Clear();
            // update waiting groups
            UpdateDetailedBusStopWaiting(B, 0, 5, "0 - 5 min waiting group");
            UpdateDetailedBusStopWaiting(B, 5, 10, "5 - 10 min waiting group");
            UpdateDetailedBusStopWaiting(B, 10, 15, "10 - 15 min waiting group");
            UpdateDetailedBusStopWaiting(B, 15, 999, "15 - N min waiting group");
            // update travelling time
            UpdateDetailedBusStopTravelling(B, 0, 15, "0 - 15 min travelling group");
            UpdateDetailedBusStopTravelling(B, 15, 30, "15 - 30 min travelling group");
            UpdateDetailedBusStopTravelling(B, 30, 45, "30 - 45 min travelling group");
            UpdateDetailedBusStopTravelling(B, 45, 999, "45 - N min travelling group");


        }

        private void UpdateDetailedBusStopWaiting(BusStopViewer B,float Low,float High,string GroupName)
        {
            var Passengers = from p in PassengerRecords
                             where p.From == B.Stop
                             select p;
            var Group = from p in Passengers
                        where p.WaitingTime >Low && p.WaitingTime<=High
                        select p;
            BusStopWaiting.Add(new DetailedBusStopViewer() { Group = GroupName, Number = Group.Count() }); 
        }

        private void UpdateDetailedBusStopTravelling(BusStopViewer B, float Low, float High, string GroupName)
        {
            var Passengers = from p in PassengerRecords
                             where p.From == B.Stop
                             select p;
            var Group = from p in Passengers
                        where p.TravellingTime > Low && p.TravellingTime <= High
                        select p;
            BusStopTravelling.Add(new DetailedBusStopViewer() { Group = GroupName, Number = Group.Count() });
        }

        public void OutputResult(Dictionary<string, int> BusstopRecords, List<PassengerTrackData> PassengerRecords, float TotalWaiting, float TotalTravelling)
        {
            this.BusstopRecords = BusstopRecords;
            this.PassengerRecords = PassengerRecords;
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                (Action)delegate ()
                {
                    foreach (var k in CityMap.Lines[0])
                    {
                        int n = 0;
                        if (BusstopRecords.Keys.ToList().Exists(x => x == k))
                            n = BusstopRecords[k];
                        BusStops.Add(new BusStopViewer() { Stop = k, Number = n });
                    }
                    TotalTimes.Add(new TotalTimeViewer() { Type = "Waiting at bus stop", Time = TotalWaiting });
                    TotalTimes.Add(new TotalTimeViewer() { Type = "Travelling on bus", Time = TotalTravelling });

                    float GeneralHappiness = 0.0f;
                    foreach(var r in PassengerRecords)
                    {
                        float Crowdness = r.Crowdness;           // 0.0f - 1.0f
                        float WaitingTime = r.WaitingTime > 15.0f ? 1.0f : r.WaitingTime / 15.0f; // 0.0f - 1.0f
                        float TravelingTime = r.TravellingTime > 60.0f ? 1.0f : r.TravellingTime / 60.0f; // 0.0f - 1.0f
                        float Happiness = 100.0f - 100.0f * (WaitingTime * 8.05f + Crowdness * 7.97f + TravelingTime * 7.29f)/(8.05f + 7.97f + 7.29f);
                        GeneralHappiness += Happiness;
                    }
                    GeneralHappiness /= PassengerRecords.Count;
                    Satisfication.Add(new SatisficationViewer() { Percent = GeneralHappiness });
                });
        }

        public void ShowBusline(int Line)
        {
            BusStops.Clear();
            foreach (var k in CityMap.Lines[Line])
            {
                int n = 0;
                if (BusstopRecords.Keys.ToList().Exists(x => x == k))
                    n = BusstopRecords[k];
                BusStops.Add(new BusStopViewer() { Stop = k, Number = n });
            }
        }
    }

    public class BusStopViewer
    {
        public string Stop { get; set; }

        public int Number { get; set; }
    }

    public class TotalTimeViewer
    {
        public string Type { get; set; }
        public float Time { get; set; }
    }

    public class DetailedBusStopViewer
    {
        public string Group { get; set; }
        public int Number { get; set; }
    }

    public class SatisficationViewer
    {
        public string Satisfication
        {
            get
            {
                return "Satisfication";
            }
        }

        public float Percent { get; set; }
    }
}
