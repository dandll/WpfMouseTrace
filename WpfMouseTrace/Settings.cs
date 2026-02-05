using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Media;

namespace WpfMouseTrace
{
    [DataContract]
    public class Settings : INotifyPropertyChanged
    {
        private static Settings _instance;
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WpfMouseTrace",
            "settings.json");

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Load();
                }
                return _instance;
            }
        }

        private int _maxTrailLength = 20;
        private byte _trailColorR = 0;
        private byte _trailColorG = 100;
        private byte _trailColorB = 255;

        [DataMember]
        public int MaxTrailLength
        {
            get => _maxTrailLength;
            set
            {
                if (_maxTrailLength != value)
                {
                    _maxTrailLength = value;
                    OnPropertyChanged(nameof(MaxTrailLength));
                }
            }
        }

        [DataMember]
        public byte TrailColorR
        {
            get => _trailColorR;
            set
            {
                if (_trailColorR != value)
                {
                    _trailColorR = value;
                    OnPropertyChanged(nameof(TrailColorR));
                }
            }
        }

        [DataMember]
        public byte TrailColorG
        {
            get => _trailColorG;
            set
            {
                if (_trailColorG != value)
                {
                    _trailColorG = value;
                    OnPropertyChanged(nameof(TrailColorG));
                }
            }
        }

        [DataMember]
        public byte TrailColorB
        {
            get => _trailColorB;
            set
            {
                if (_trailColorB != value)
                {
                    _trailColorB = value;
                    OnPropertyChanged(nameof(TrailColorB));
                    OnPropertyChanged(nameof(TrailColor));
                }
            }
        }

        public Color TrailColor => Color.FromRgb(TrailColorR, TrailColorG, TrailColorB);

        public void SetColor(Color color)
        {
            TrailColorR = color.R;
            TrailColorG = color.G;
            TrailColorB = color.B;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var serializer = new DataContractJsonSerializer(typeof(Settings));
                using (var stream = new FileStream(SettingsFilePath, FileMode.Create))
                {
                    serializer.WriteObject(stream, this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        public void ReLoad() 
        {
            _instance = Load();
        }

        private static Settings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var serializer = new DataContractJsonSerializer(typeof(Settings));
                    using (var stream = new FileStream(SettingsFilePath, FileMode.Open))
                    {
                        return (Settings)serializer.ReadObject(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
            }

            return new Settings();
        }
    }
}
