using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeSimulator;

namespace DesGui
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        public Config Config = new Config()
        {
            RunningTime = 60 * 360,
            Scheduler = new StaticScheduler(),
            Buses = new Dictionary<int, int>()
                {
                    {0, 1 }, // line 0 bus * 1
                    {1, 1 }  // line 1 bus * 1
                },
            MaxPassenger = 50,
            BusInterval = 60 * 5 // 60s * 5min
        };

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            string Selected = ComboBox_Busline.SelectedItem as string;
            Config.Buses[int.Parse(Selected)] = int.Parse(TextBox_BusNumber.Text);

            Config.BusInterval = 60 * int.Parse(TextBox_Interval.Text);
            Selected = ComboBox_Scheduler.SelectedItem as string;
            if (Selected == "Static")
                Config.Scheduler = new StaticScheduler();
            //else if (Selected == "Dynamic")
            //    Config.Scheduler = new DynamicScheduler();
            Config.MaxPassenger = int.Parse(TextBox_MaxPassenger.Text);
            Config.RunningTime = 60 * int.Parse(TextBox_SimTime.Text);
        }

        private void ComboBox_Busline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            ComboBox_Busline.DataContext = box.SelectedItem;
            if (e.RemovedItems.Count == 0) return;
            string Selected = e.RemovedItems[0] as string;
            Config.Buses[int.Parse(Selected)] = int.Parse(TextBox_BusNumber.Text);

            Selected = ComboBox_Busline.SelectedItem as string;
            TextBox_BusNumber.Text = Config.Buses[int.Parse(Selected)].ToString();
        }

        private void ComboBox_Busline_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> items = new List<string>();
            items.Add("0");
            items.Add("1");
            ComboBox_Busline.ItemsSource = items;
        }

        private void ComboBox_Scheduler_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> items = new List<string>();
            items.Add("Static");
            items.Add("Dynamic");
            ComboBox_Scheduler.ItemsSource = items;            
        }

        private void ComboBox_Scheduler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            ComboBox_Scheduler.DataContext = box.SelectedItem;
        }
    }
}
