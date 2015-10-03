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

        public void UpdateBusStopViewer(string[] Name,int[] Number)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                (Action)delegate ()
                {
                    for(int i=0; i<Name.Length; i++)
                    {
                        BusStops.Add(new BusStopViewer() { Stop = Name[i],Number=Number[i] });
                    }
                });          
        }

        public void UpdateTotalTimeViewer(float Waiting, float Travelling)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                (Action)delegate ()
                {
                    TotalTimes.Add(new TotalTimeViewer() { Type = "Waiting at bus stop", Time = Waiting });
                    TotalTimes.Add(new TotalTimeViewer() { Type = "Travelling on bus", Time = Travelling });
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
