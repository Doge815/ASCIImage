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
            AutoComplete.GetPath();
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
                Console.WriteLine("something happened!");
            }
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        
    }
}
