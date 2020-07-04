using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ASCIImage
{
    static class ImageConverter
    {
        private const int char_x = 1;
        private const int char_y = 2;
        private static Dictionary<int, char> brightness;

        public static string GetStringyImage(string file, int parts, Action<double>SetValue)
        {
            if (brightness == null)
            {
                brightness = new Dictionary<int, char>();
                string inp = Properties.Resources.brightness;
                string[] pairs = inp.Split(new []{ "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string pair in pairs)
                {
                    var a = pair[^1];
                    var b = pair[0..^1];
                    brightness.Add(int.Parse(b), Convert.ToChar(a));
                }
            }
            SetValue(0);
            Bitmap image = new Bitmap(System.Drawing.Image.FromFile(file));
            int[,] values = new int[(int)Math.Ceiling((float)image.Width / char_x / parts),
                (int)Math.Ceiling((float)image.Height / char_y / parts)];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    values[(int)Math.Floor((float)x / char_x / parts),
                        (int)Math.Floor((float)y / char_y / parts)] +=
                        (int)(image.GetPixel(x, y).GetBrightness() / parts / parts / char_x / char_y*100);
                }
                SetValue(Math.Ceiling(100d * x / image.Width));
            }
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < (int)Math.Ceiling((float)image.Height / char_y / parts); y++)
            {
                for (int x = 0; x < (int)Math.Ceiling((float)image.Width / char_x / parts); x++)
                {
                    int a = values[x, y];
                    int c = 100 / (brightness.Count - 1);
                    int d = c * (int)Math.Round((decimal)a / c);
                    char value = brightness[d];
                    sb.Append(value);
                }
                sb.Append(Environment.NewLine);
                SetValue(Math.Ceiling(100d * y * char_y * parts / image.Height));
                System.Diagnostics.Debug.Print((Math.Ceiling(100d * y * char_y * parts / image.Height).ToString()));
            }
            SetValue(100);
            return sb.ToString();
        }

        public static System.Drawing.Image GetImagyImage(string text, int size, Action<double> SetValue)
        {
            throw new NotImplementedException();
        }
    }
}
