using System;
using System.Drawing;
using System.Text;

namespace ASCIImage
{
    static class ImageConverter
    {
        private const int char_x = 1;
        private const int char_y = 2;
        private const string brightness = "@##$||===;;::''''...  ";

        public static string GetImage(string file, int parts = 5)
        {
            Bitmap image = new Bitmap(Image.FromFile(file));
            decimal[,] values = new decimal[(int)Math.Ceiling((float)image.Width / char_x / parts), (int)Math.Ceiling((float)image.Height / char_y / parts)];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    values[(int)Math.Floor((float)x / char_x / parts), (int)Math.Floor((float)y / char_y / parts)] += (decimal)image.GetPixel(x, y).GetBrightness() / parts / parts / char_x / char_y;
                }
                Console.Write($"\rprogress: {Math.Ceiling(100f * x / image.Width)}%");
            }
            Console.WriteLine();
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < (int)Math.Ceiling((float)image.Height / char_y / parts); y++)
            {
                for (int x = 0; x < (int)Math.Ceiling((float)image.Width / char_x / parts); x++)
                {
                    sb.Append(brightness[(int)((float)values[x, y] * (brightness.Length-1))]);
                }
                sb.Append(Environment.NewLine);
                Console.Write($"\rprogress: {Math.Ceiling(100f * y / (int)Math.Ceiling((float)image.Height / char_y / parts))}%");
            }
            Console.WriteLine("\nDONE!");
            return sb.ToString();
        }
    }
}
