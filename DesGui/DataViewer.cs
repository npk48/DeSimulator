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
        public object SelectedItem { get; set; }
        public DataViewer()
        {
            BusStops = new ObservableCollection<BusStopViewer>();
            TotalTimes = new ObservableCollection<TotalTimeViewer>();
        }

        public void Clear()
        {
            BusStops.Clear();
        }

        public void OutputResult(Dictionary<string, int> BusstopRecords, List<PassengerTrackData> PassengerRecords, float TotalWaiting, float TotalTravelling)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                (Action)delegate ()
                {
                    foreach(var k in BusstopRecords.Keys)
                    {
                        BusStops.Add(new BusStopViewer() { Stop = k, Number = BusstopRecords[k] });
                    }
                    TotalTimes.Add(new TotalTimeViewer() { Type = "Waiting at bus stop", Time = TotalWaiting });
                    TotalTimes.Add(new TotalTimeViewer() { Type = "Travelling on bus", Time = TotalTravelling });

                });
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
}
