using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ASCIImage
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            using (StreamWriter s = new StreamWriter(args[0]+".txt",false))
            {
                s.Write(new ImageConverter(args[0]).GetImage());
                s.Close();
            }
        }
    }

    class ImageConverter
    {
        private const int char_x = 1;
        private const int char_y = 2;
        private const int parts = 5;
        private const string bright = "@##$===;:::''''... ";
        private Bitmap i { get; set; }
        private decimal[,] val { get; set; }

        public ImageConverter(string args) => i = new Bitmap(Image.FromFile(args));
        public string GetImage()
        {
            val = new decimal[(int)Math.Ceiling((float)i.Width / char_x / parts), (int)Math.Ceiling((float)i.Height / char_y / parts)];
            for (int x = 0; x < i.Width; x++)
            {
                for (int y = 0; y < i.Height; y++)
                {
                    val[(int)Math.Floor((float)x / char_x / parts), (int)Math.Floor((float)y / char_y / parts)] += (decimal)i.GetPixel(x, y).GetBrightness()/parts/parts/char_x/char_y; 
                }
                Console.Write($"\rprogress: {Math.Ceiling(100f*x/i.Width)}%");
            }
            Console.WriteLine("\nWait a sec...");
            string output = string.Empty;
            for (int y = 0; y < (int)Math.Ceiling((float)i.Height / char_y / parts); y++)
            {
                for (int x = 0; x < (int)Math.Ceiling((float)i.Width / char_x / parts); x++)
                {
                    output += bright[(int)((float)val[x, y] * bright.Length)];
                }
                output += "\n";
            }
            Console.WriteLine("DONE!");
            return output;
        }
    }
}
