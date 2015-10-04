using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeSimulator;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

namespace DesGui
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitConsole();
            InitCharts();
        }

        private void InitConsole()
        {
            Logger Log = Logger.Instance;
            Console.SetOut(Log);
            Scroller.DataContext = Log.Content;
            Loaded += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
            {
                InputBlock.KeyDown += new KeyEventHandler(delegate (object _sender, KeyEventArgs _e)
                {
                    if (_e.Key == Key.Enter)
                    {
                        Log.RunCommand(InputBlock.Text);
                        InputBlock.Text = "";
                        InputBlock.Focus();
                    }
                });
                Log.Updated += new Logger.UpdatedEventHandler(delegate (string value)
                {
                    Scroller.ScrollToBottom();
                    if (Log.Content.Output.Count > 500)
                        Log.Content.Output.RemoveAt(0);
                });
            });
            Log.ExecuteCommand += ExecuteCommand;           
        }

        public void InitCharts()
        {
            DataView = new DataViewer();
            SimRegion.DataContext = DataView;
            Simulator.UpdateBusStopViewerHandler = DataView.UpdateBusStopViewer;
            Simulator.UpdateTotalTimeViewerHandler = DataView.UpdateTotalTimeViewer;
        }

        private DataViewer DataView;

        private void ExecuteCommand(string Value)
        {
            
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Launch_Click(object sender, RoutedEventArgs e)
        {
            Simulator Sim = new Simulator();
            DataView.Clear();
            Sim.Config = new Config()
            {
                RunningTime = 60 * int.Parse(TextBox_SimTime.Text),
                Scheduler = new StaticScheduler(),
                Buses = new Dictionary<int, int>()
                {
                    {0, 1} // line 0 bus * 2
                },
                BusInterval = 60 * 5 // 60s * 5min
            };                  

            Sim.Start();         
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (Settings_Form.Visibility == Visibility.Visible)
                Settings_Form.Visibility = Visibility.Hidden;
            else
                Settings_Form.Visibility = Visibility.Visible;
        }

        private void CheckBox_Log_Checked(object sender, RoutedEventArgs e)
        {
            Scroller.Visibility = Visibility.Visible;
            InputBlock.Visibility = Visibility.Visible;
            Logger.Instance.Enable = true;
        }

        private void CheckBox_Log_Unchecked(object sender, RoutedEventArgs e)
        {
            Scroller.Visibility = Visibility.Hidden;
            InputBlock.Visibility = Visibility.Hidden;
            Logger.Instance.Enable = false;
        }

        private void SimRegionScroll(object sender, MouseWheelEventArgs e)
        {
            if (Monitor.TryEnter(locker))
            {
                try
                {
                    if(!AnimatedScrollViewer.InAnimation)
                    {
                        AnimatedScrollViewer.InAnimation = true;
                        ScrollViewer S = sender as ScrollViewer;
                        if (e.Delta > 0)
                            S.AnimatedPageLeft();
                        else
                            S.AnimatedPageRight();
                    }               
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
            

            e.Handled = true;
        }

        private object locker = new object();
    }

    public static class AnimatedScrollViewer
    {
        public static bool InAnimation = false;

        private static int Page = 0;

        public static int PageNumber = 2;

        public static double PageWidth = 640;

        public static double Duration = 250; // ms

        public static int Interval = 10; // ms

        public static void AnimatedPageRight(this ScrollViewer S)
        {
            if (Page >= PageNumber-1)
            {
                InAnimation = false;
                return;
            }
            double CurrentOffset = Page * PageWidth;
            double TargetOffset = Page * PageWidth + PageWidth;
            double Step = PageWidth / Duration;
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(Interval);
            int TimeOffset = 0;
            Timer.Tick += (s, e) =>
            {
                TimeOffset += Interval;
                S.ScrollToHorizontalOffset(CurrentOffset + TimeOffset * Step);
                if(TimeOffset>=Duration)
                {
                    S.ScrollToHorizontalOffset(TargetOffset);
                    Timer.Stop();
                    InAnimation = false;
                }                             
            };
            Timer.Start();
            Page++;
        }

        public static void AnimatedPageLeft(this ScrollViewer S)
        {
            if (Page <=0 )
            {
                InAnimation = false;
                return;
            }
            double CurrentOffset = Page * PageWidth;
            double TargetOffset = Page * PageWidth - PageWidth;
            double Step = PageWidth / Duration;
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(Interval);
            int TimeOffset = 0;
            Timer.Tick += (s, e) =>
            {
                TimeOffset += Interval;
                S.ScrollToHorizontalOffset(CurrentOffset - TimeOffset * Step);
                if (TimeOffset >= Duration)
                {
                    S.ScrollToHorizontalOffset(TargetOffset);
                    Timer.Stop();
                    InAnimation = false;
                }
            };
            Timer.Start();
            Page--;
        }
    }
}
