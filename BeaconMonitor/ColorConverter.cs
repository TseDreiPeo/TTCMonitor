using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BeaconMonitor
{
    public class ColourConverter : IValueConverter
    {
        private Color MinCol = (Color)ColorConverter.ConvertFromString("#A000FF00");
        private Double MinVal = 0.0;
        private Color MaxCol = (Color)ColorConverter.ConvertFromString("#A0FF0000");
        private Double MaxVal = 100.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object retVal = "Yellow";
            Double dVal;
            Random rd = new Random();
            if (Double.TryParse(value.ToString(), out dVal))
            {
                retVal = GetGradient(MinCol, MaxCol, dVal).ToString();
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public Color GetGradient(Color start, Color end, double step, int steps = 100)
        {
            int stepA = ((end.A - start.A) / (steps - 1));
            int stepR = ((end.R - start.R) / (steps - 1));
            int stepG = ((end.G - start.G) / (steps - 1));
            int stepB = ((end.B - start.B) / (steps - 1));

            Color retVal = Color.FromArgb((byte)(start.A + (stepA * step)),
                                   (byte)(start.R + (stepR * step)),
                                   (byte)(start.G + (stepG * step)),
                                   (byte)(start.B + (stepB * step)));

            return retVal;
        }
    }
}
