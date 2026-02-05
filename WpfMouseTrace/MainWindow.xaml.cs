using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace WpfMouseTrace
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        private DispatcherTimer _timer;
        private NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            //最大化显示
            this.WindowState = WindowState.Maximized;

            // 设置窗口为鼠标穿透（点击穿透到底层窗口）
            Loaded += (s, e) =>
            {
                var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_EXSTYLE,
                    ((int)GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TRANSPARENT));
                
                InitializeNotifyIcon();
                ApplySettings();
            };

            // 启动定时器（每33ms ≈ 30fps，降低内存占用）
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(33);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void InitializeNotifyIcon()
        {
            var iconStream = System.Windows.Application.GetResourceStream(
                new Uri("pack://application:,,,/Resources/favicon(1).ico"))?.Stream;

            _notifyIcon = new NotifyIcon
            {
                Icon = iconStream != null ? new System.Drawing.Icon(iconStream) : SystemIcons.Application,
                Text = "鼠标轨迹追踪",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            var settingsItem = new ToolStripMenuItem("设置");
            settingsItem.Click += (s, e) => OpenSettings();
            
            var exitItem = new ToolStripMenuItem("退出");
            exitItem.Click += (s, e) => ExitApplication();

            contextMenu.Items.AddRange(new ToolStripItem[] { settingsItem, exitItem });
            _notifyIcon.ContextMenuStrip = contextMenu;

            _notifyIcon.DoubleClick += (s, e) => OpenSettings();
        }

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = this
            };

            if (settingsWindow.ShowDialog() == true)
            {
                ApplySettings();
            }
            else
            {
                ApplySettings();
            }
        }

        private void ApplySettings()
        {
            var settings = Settings.Instance;

            settings.ReLoad();

            TrailCanvas.MaxTrailLength = settings.MaxTrailLength;
            
            while (TrailCanvas.TrailPoints.Count > TrailCanvas.MaxTrailLength)
            {
                TrailCanvas.TrailPoints.Dequeue();
            }
            
            TrailCanvas.TrailColorR = settings.TrailColorR;
            TrailCanvas.TrailColorG = settings.TrailColorG;
            TrailCanvas.TrailColorB = settings.TrailColorB;
        }

        private void ExitApplication()
        {
            _notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnClosing(e);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (GetCursorPos(out POINT pt))
            {
                var point = PointFromScreen(new System.Windows.Point(pt.X, pt.Y));

                TrailCanvas.TrailPoints.Enqueue(point);
                if (TrailCanvas.TrailPoints.Count > TrailCanvas.MaxTrailLength)
                    TrailCanvas.TrailPoints.Dequeue();

                TrailCanvas.InvalidateVisual();
            }
        }
    }
}
