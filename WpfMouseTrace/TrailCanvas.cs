using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfMouseTrace
{
    public class TrailCanvas : Canvas
    {
        public Queue<Point> TrailPoints { get; set; } = new Queue<Point>();
        private int _maxTrailLength = 20;
        public int MaxTrailLength
        {
            get => _maxTrailLength;
            set
            {
                if (_maxTrailLength != value)
                {
                    _maxTrailLength = value;
                    _pointsCache = new Point[_maxTrailLength];
                }
            }
        }
        public byte TrailColorR = 0;
        public byte TrailColorG = 100;
        public byte TrailColorB = 255;

        private Point[] _pointsCache = new Point[20];

        public TrailCanvas()
        {
            _pointsCache = new Point[MaxTrailLength];
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            int count = TrailPoints.Count;
            if (count < 2) return;

            TrailPoints.CopyTo(_pointsCache, 0);

            for (int i = 0; i < count - 1; i++)
            {
                double alpha = (i / (double)count);
                byte alphaByte = (byte)(alpha * 200);

                //using (var brush = new SolidColorBrush(Color.FromArgb(alphaByte, TrailColorR, TrailColorG, TrailColorB)))
                var brush = new SolidColorBrush(Color.FromArgb(alphaByte, TrailColorR, TrailColorG, TrailColorB));
                {
                    double thickness = 8 * alpha + 2;
                    var pen = new Pen(brush, thickness);
                    drawingContext.DrawLine(pen, _pointsCache[i], _pointsCache[i + 1]);
                }
            }
        }
    }
}
