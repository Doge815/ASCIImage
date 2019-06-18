using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ASCIImage
{
    static class AutoComplete
    {
        public static string GetPath()
        {
            void ClearCurrentLine()
            {
                var currentLine = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLine);
            }

            var builder = new StringBuilder();
            var input = Console.ReadKey(intercept: true);
            int index = 0;
            string old = string.Empty;

            while (input.Key != ConsoleKey.Enter)
            {
                var currentInput = builder.ToString();
                if (input.Key == ConsoleKey.Tab)
                {
                    try
                    {
                        if (index == -1)
                        {
                            index = 0;
                            old = builder.ToString();
                        }
                        List<string> data = new List<string>();
                        string[] conten = old.Split('\\');
                        string direc = string.Join("\\", conten.Take(conten.Length - 1)) + "\\";

                        data = Directory.GetFileSystemEntries(direc).Where((f => !(new FileInfo(f).Attributes.HasFlag(FileAttributes.Hidden) || new FileInfo(f).Attributes.HasFlag(FileAttributes.System)))).ToList();
                        var m = data.Where(item => item != old && item.StartsWith(old, true, CultureInfo.InvariantCulture)).ToList();
                        var match = m.ElementAt(index++);
                        if (index == m.Count) index = 0;

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
                    catch { }
                }
                else
                {
                    index = -1;
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

                input = Console.ReadKey(intercept: true);
            }
            Console.Write(input.KeyChar);
            return builder.ToString();
        }
    }
}
