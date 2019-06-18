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
            string file = "";
            try
            {
                file = args[0];
            }
            catch
            {
                Console.WriteLine($"Insert a file path:");
                file = AutoComplete.GetPath();
            }
                
            while (!File.Exists(file))
            {
                Console.WriteLine();
                Console.WriteLine($"'{file}' doesn't exists. Insert a file path:");
                file = AutoComplete.GetPath();
            }
            Console.WriteLine();
            //try
            //{
                using (StreamWriter s = new StreamWriter(file + ".txt", false))
                {
                    s.Write(ImageConverter.GetImage(file, 5));
                    s.Close();
                }
            //}
#if false
            catch
            {
                try { File.Delete(file + ".txt"); } catch { }
                Console.WriteLine("something happened!");
            }
#endif
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        
    }
}
