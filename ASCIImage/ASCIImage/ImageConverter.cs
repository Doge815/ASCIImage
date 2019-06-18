using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ASCIImage
{
    static class ImageConverter
    {
        private const int char_x = 1;
        private const int char_y = 2;
        //private const string brightness = "@##$||===;;::''''...  ";
        private static Dictionary<float, char> brightness;

        public static string GetImage(string file, int parts = 5)
        {
            if(brightness == null)
            {
                brightness = new Dictionary<float, char>();
                using (StreamReader s = new StreamReader("brightness.txt"))
                {
                    while(!s.EndOfStream)
                    {
                        string input = s.ReadLine();
                        string[] val = input.Split('|');
                        brightness.Add(float.Parse(val[0]), Convert.ToChar(val[1]));
                    }
                }
            }

            Bitmap image = new Bitmap(Image.FromFile(file));
            double[,] values = new double[(int)Math.Ceiling((float)image.Width / char_x / parts), (int)Math.Ceiling((float)image.Height / char_y / parts)];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    values[(int)Math.Floor((float)x / char_x / parts), (int)Math.Floor((float)y / char_y / parts)] += (double)image.GetPixel(x, y).GetBrightness() / parts / parts / char_x / char_y;
                }
                Console.Write($"\rprogress: {Math.Ceiling(100f * x / image.Width)}%");
            }
            Console.WriteLine();
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < (int)Math.Ceiling((float)image.Height / char_y / parts); y++)
            {
                for (int x = 0; x < (int)Math.Ceiling((float)image.Width / char_x / parts); x++)
                {
                    var value = brightness.Keys.OrderBy(v => Math.Abs((double)v - values[x, y])).First();
                    sb.Append(brightness[value]);
                }
                sb.Append(Environment.NewLine);
                Console.Write($"\rprogress: {Math.Ceiling(100f * y / (int)Math.Ceiling((float)image.Height / char_y / parts))}%");
            }
            Console.WriteLine("\nDONE!");
            return sb.ToString();
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
