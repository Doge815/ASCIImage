using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ASCIImage
{
    class Program
    {
        static void Main(string[] args)
        {
            getpath();
            string file = "";
            try
            {
                file = args[0];
            }
            catch
            {
                Console.Write($"Insert a file path: ");
                file = Console.ReadLine();
            }
                
            while (!File.Exists(file))
            {
                Console.Write($"'{file}' doesn't exists. Insert a file path: ");
                file = Console.ReadLine();
            }
            try
            {
                using (StreamWriter s = new StreamWriter(file + ".txt", false))
                {
                    s.Write(ImageConverter.GetImage(file, 1));
                    s.Close();
                }
            }
            catch
            {
                try { File.Delete(file + ".txt"); } catch { }
                Console.WriteLine("something happend!");
            }
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static void getpath()
        {
            void ClearCurrentLine()
            {
                var currentLine = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLine);
            }

            var builder = new StringBuilder();
            var input = Console.ReadKey(intercept:true);

            while (input.Key != ConsoleKey.Enter)
            {
                var currentInput = builder.ToString();
                if (input.Key == ConsoleKey.Tab)
                {
                    List<string> data = new List<string>();
                    string raw = builder.ToString();
                    string direc = string.Join("\\", raw.Split('\\').Reverse().Skip(1).Reverse()) + "\\";
                    data = Directory.GetFiles(direc).ToList();
                    data.AddRange(Directory.GetDirectories(direc));
                    var match = data.FirstOrDefault(item => item != currentInput && item.StartsWith(currentInput, true, CultureInfo.InvariantCulture));
                    if (string.IsNullOrEmpty(match))
                    {
                        input = Console.ReadKey(intercept: true);
                        continue;
                    }

                    ClearCurrentLine();
                    builder.Clear();

                    Console.Write(match);
                    builder.Append(match);
                }
                else
                {
                    if (input.Key == ConsoleKey.Backspace && currentInput.Length > 0)
                    {
                        builder.Remove(builder.Length - 1, 1);
                        ClearCurrentLine();

                        currentInput = currentInput.Remove(currentInput.Length - 1);
                        Console.Write(currentInput);
                    }
                    else
                    {
                        var key = input.KeyChar;
                        builder.Append(key);
                        Console.Write(key);
                    }
                }

                input = Console.ReadKey(intercept:true);
            }
            Console.Write(input.KeyChar);
        }
    }

    class ImageConverter
    {
        private const int char_x = 1;
        private const int char_y = 2;
        private const string brightness = "@##$||===;;::''''...  ";

        private ImageConverter() { }
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
                    sb.Append(brightness[(int)((float)values[x, y] * brightness.Length)]);
                }
                sb.Append(Environment.NewLine);
                Console.Write($"\rprogress: {Math.Ceiling(100f * y / (int)Math.Ceiling((float)image.Height / char_y / parts))}%");
            }
            Console.WriteLine("\nDONE!");
            return sb.ToString();
        }
    }
}
