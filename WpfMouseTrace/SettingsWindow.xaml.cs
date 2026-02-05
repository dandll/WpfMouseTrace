using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace WpfMouseTrace
{
    public partial class SettingsWindow : Window
    {
        private Settings _settings;

        private Settings _settingsBak = null;

        private MainWindow _ownerWindow;

        public SettingsWindow()
        {
            InitializeComponent();
            _settings = Settings.Instance;

            _settingsBak = new Settings();
            _settingsBak.MaxTrailLength = _settings.MaxTrailLength;
            _settingsBak.TrailColorR = _settings.TrailColorR;
            _settingsBak.TrailColorG = _settings.TrailColorG;
            _settingsBak.TrailColorB = _settings.TrailColorB;

            DataContext = _settings;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _ownerWindow = Owner as MainWindow;
        }


        bool isClickOk = false;
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            isClickOk = true;
            _settings.Save();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ColorPreviewBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var colorDialog = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(
                    _settings.TrailColorR,
                    _settings.TrailColorG,
                    _settings.TrailColorB),
                FullOpen = true
            };

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settings.TrailColorR = colorDialog.Color.R;
                _settings.TrailColorG = colorDialog.Color.G;
                _settings.TrailColorB = colorDialog.Color.B;
                
                ApplySettingsToMainWindow();
            }
        }

        private void MaxTrailLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ApplySettingsToMainWindow();
        }

        private void ApplySettingsToMainWindow()
        {
            if (_ownerWindow != null)
            {
                _ownerWindow.TrailCanvas.MaxTrailLength = _settings.MaxTrailLength;
                    
                while (_ownerWindow.TrailCanvas.TrailPoints.Count > _ownerWindow.TrailCanvas.MaxTrailLength)
                {
                    _ownerWindow.TrailCanvas.TrailPoints.Dequeue();
                }
                    
                _ownerWindow.TrailCanvas.TrailColorR = _settings.TrailColorR;
                _ownerWindow.TrailCanvas.TrailColorG = _settings.TrailColorG;
                _ownerWindow.TrailCanvas.TrailColorB = _settings.TrailColorB;
                    
                _ownerWindow.TrailCanvas.InvalidateVisual();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isClickOk)
            {
                _settings.MaxTrailLength = _settingsBak.MaxTrailLength;
                _settings.TrailColorR = _settingsBak.TrailColorR;
                _settings.TrailColorG = _settingsBak.TrailColorG;
                _settings.TrailColorB = _settingsBak.TrailColorB;
                _settings.Save();
            }
        }
    }
}
