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
            var chars = Enumerable.Range(char.MinValue, char.MaxValue - char.MinValue).Select(x => Convert.ToChar(x)).Where(x => !char.IsControl(x)).ToList();
            chars = Enumerable.Range('\x1', 127).Select(x => Convert.ToChar(x)).Where(x => !char.IsControl(x)).ToList();
            Dictionary<char, float>       b = new Dictionary<char, float>();
            Bitmap                       bb = new Bitmap(100, 200);
            Graphics                      g = Graphics.FromImage(bb);
            Font                          f = new Font(new FontFamily("DejaVu Sans Mono"), 12);
            SolidBrush                    s = new SolidBrush(Color.Black);
            PointF                        p = new PointF(1, 1);
            
            foreach(char c in chars)
            {
                g.Clear(Color.White);
                g.DrawString(c.ToString(), f, s, p);
                SizeF ss = g.MeasureString(c.ToString(), f);
                int w = (int)ss.Width;
                int h = (int)ss.Height;
                float bbb = 0;
                for(int x = (int)p.X; x < w+(int)p.X; x++)
                for(int y = (int)p.Y; y < h+(int)p.Y; y++)
                    bbb += bb.GetPixel(x, y).GetBrightness();
                bbb = bbb / w / h;
                b.Add(c, bbb);
            }
            Dictionary<float, char> bbbb = new Dictionary<float, char>();
            for(float i = 0; i <= 2; i+= 0.05f)
            {
                var u = b.MinBy(x => Math.Abs(i - x.Value)).Item1;
                bbbb.Add(i, u.Key);
                b.Remove(u.Key);
            }

            using (StreamWriter sss = new StreamWriter("brightness.txt", false, Encoding.Unicode))
            {
                foreach(float ff in bbbb.Keys)
                {
                    sss.WriteLine($"{ff}|{bbbb[ff]}");
                }
                sss.Close();
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
