using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace WpfMouseTrace
{
    public partial class SettingsWindow : Window
    {
        private Settings _settings;

        public SettingsWindow()
        {
            InitializeComponent();
            _settings = Settings.Instance;
            DataContext = _settings;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
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
                    _settings.TrailColorB)
            };

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settings.TrailColorR = colorDialog.Color.R;
                _settings.TrailColorG = colorDialog.Color.G;
                _settings.TrailColorB = colorDialog.Color.B;
            }
        }
    }
}
