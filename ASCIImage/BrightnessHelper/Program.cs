using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightnessHelper
{
    static class Program
    {
        static void Main(string[] args)
        {
            var chars = Enumerable.Range('\x1', 127).Select(x => Convert.ToChar(x)).Where(x => !char.IsControl(x)).ToList();
            Dictionary<char, decimal> brightness = new Dictionary<char, decimal>();
            Bitmap image = new Bitmap(100, 200);
            Graphics graphic = Graphics.FromImage(image);
            Font font = new Font(new FontFamily("DejaVu Sans Mono"), 12);
            SolidBrush brush = new SolidBrush(Color.Black);
            PointF point = new PointF(1, 1);

            foreach (char c in chars)
            {
                graphic.Clear(Color.White);
                graphic.DrawString(c.ToString(), font, brush, point);
                SizeF ss = graphic.MeasureString(c.ToString(), font);
                int width = (int)ss.Width;
                int height = (int)ss.Height;
                decimal charBrightness = 0;
                for (int x = (int)point.X; x < width + (int)point.X; x++)
                    for (int y = (int)point.Y; y < height + (int)point.Y; y++)
                        charBrightness += (decimal)image.GetPixel(x, y).GetBrightness();
                charBrightness = charBrightness / width / height * 100;
                brightness.Add(c, charBrightness);
            }

            var minBright = brightness.Values.AsEnumerable().Min();
            Dictionary<char, decimal> relativeBrightness = new Dictionary<char, decimal>();
            foreach (char c in brightness.Keys)
                relativeBrightness.Add(c, (brightness[c] - minBright) *100 / (100-minBright));

            Dictionary<decimal, char> bestChars = new Dictionary<decimal, char>();
            for (int i = 0; i <= 100; i += 5)
            {
                var u = relativeBrightness.MinBy(x => Math.Abs(i - x.Value)).Item1;
                bestChars.Add(i, u.Key);
                relativeBrightness.Remove(u.Key);
            }

            using (StreamWriter writer = new StreamWriter("brightness.txt", false, Encoding.Unicode))
            {
                foreach (decimal bright in bestChars.Keys)
                {
                    writer.WriteLine($"{bright}{bestChars[bright]}");
                }
                writer.Close();
            }
        }

        public static (T1, T2) MinBy<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> selector)
            where T2 : IComparable<T2>
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext()) throw new ArgumentOutOfRangeException(nameof(source) + " is empty");
            (T1, T2) min = (enumerator.Current, selector(enumerator.Current));

            while (enumerator.MoveNext())
            {
                (T1, T2) current = (enumerator.Current, selector(enumerator.Current));
                if (min.Item2.CompareTo(current.Item2) > 0) min = current;
            }

            return min;
        }
    }
}