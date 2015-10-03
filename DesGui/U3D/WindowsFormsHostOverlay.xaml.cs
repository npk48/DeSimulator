using System;
using System.Windows;
using System.Windows.Forms;


namespace DesGui
{
    /// <summary>
    /// WindowsFormsHostOverlay.xaml 的交互逻辑
    /// </summary>
    public partial class WindowsFormsHostOverlay : Window
    {
        FrameworkElement t;
        public WindowsFormsHostOverlay(FrameworkElement target, Control child)
        {
            InitializeComponent();

            t = target;
            wfh.Child = child;

            Owner = GetWindow(t);
            

            Owner.LocationChanged += new EventHandler(PositionAndResize);
            t.LayoutUpdated += new EventHandler(PositionAndResize);
            PositionAndResize(null, null);
            Show();
            if (Owner.IsVisible)
                Show();
            else
                Owner.IsVisibleChanged += delegate
                {
                    if (Owner.IsVisible)
                        Show();
                };

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Owner.LocationChanged -= new EventHandler(PositionAndResize);
            t.LayoutUpdated -= new EventHandler(PositionAndResize);
        }

        void PositionAndResize(object sender, EventArgs e)
        {
            Point p = t.PointToScreen(new Point());
            Left = p.X;
            Top = p.Y;

            Height = t.ActualHeight;
            Width = t.ActualWidth;
        }

    }
}
