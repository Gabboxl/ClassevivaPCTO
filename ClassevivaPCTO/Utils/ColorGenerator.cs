using System;
using System.Drawing;

namespace ClassevivaPCTO.Utils
{
    internal class ColorGenerator
    {
        private static double goldenRatio = 0.618033988749895;

        private double hue;
        private double saturation;
        private double lightness;

        public ColorGenerator(double hue, double saturation, double lightness)
        {
            this.hue = hue;
            this.saturation = saturation;
            this.lightness = lightness;
        }

        public Color GetColor(int number)
        {
            double hue = (this.hue + goldenRatio * number) % 1.0;
            double saturation = this.saturation;
            double lightness = this.lightness;
            return FromHsl(hue, saturation, lightness);
        }

        private static Color FromHsl(double hue, double saturation, double lightness)
        {
            if (saturation == 0)
            {
                int gray = (int)Math.Round(lightness * 255);
                return Color.FromArgb(gray, gray, gray);
            }

            double q = lightness < 0.5 ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation;
            double p = 2 * lightness - q;

            double r = HueToRgb(p, q, hue + 1.0 / 3.0);
            double g = HueToRgb(p, q, hue);
            double b = HueToRgb(p, q, hue - 1.0 / 3.0);

            return Color.FromArgb((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
            return p;
        }
    }
}