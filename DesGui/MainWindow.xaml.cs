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
        }

        private void InitConsole()
        {
            Logger Log = Logger.Instance;
            Console.SetOut(Log);
            DataContext = Log.Content;
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
            Sim.Config = new Config()
            {
                RunningTime = 60 * 60 * 8, // 60s * 30min
                Scheduler = new StaticScheduler(),
                Buses = new Dictionary<int, int>()
                {
                    {0, 2} // line 0 bus * 2
                },
                BusInterval = 60 * 5 // 60s * 5min
            };
            Sim.Start();
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
