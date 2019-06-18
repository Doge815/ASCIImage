using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightnessHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var chars = Enumerable.Range(char.MinValue, char.MaxValue - char.MinValue).Select(x => Convert.ToChar(x)).Where(x => !char.IsControl(x));
            Dictionary<char, float>       b = new Dictionary<char, float>();
            Bitmap                       bb = new Bitmap(100, 200);
            Graphics                      g = Graphics.FromImage(bb);
            Font                          f = new Font(new FontFamily("Lucida Sans Typewriter"), 12);
            SolidBrush                    s = new SolidBrush(Color.Black);
            PointF                        p = new PointF(1, 1);
            
            foreach(char c in chars)
            {
                g.Clear(Color.White);
                g.DrawString(c.ToString(), f, s, p);
                SizeF ss = g.MeasureString(c.ToString(), f);
                float w = ss.Width;
                float h = ss.Height;
                float bbb = 0;
                for(int x = (int)p.X; x < w+p.X; x++)
                for(int y = (int)p.Y; y < h+p.Y; y++)
                    bbb += bb.GetPixel(x, y).GetBrightness();
                bbb = bbb / w / h;
                b.Add(c, bbb);
            }
            using (StreamWriter sss = new StreamWriter())
            {

            }
        }
    }
}
