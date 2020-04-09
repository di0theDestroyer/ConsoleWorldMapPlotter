using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class AsyncConsoleWriter
    {
        private static BlockingCollection<Tuple<string, int, int>> writes =
            new BlockingCollection<Tuple<string, int, int>>();

        static AsyncConsoleWriter()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var toWrite = writes.Take();

                    Console.SetCursorPosition(toWrite.Item2, toWrite.Item3);

                    // change aircraft icon red
                    if (toWrite.Item1 == WorldMapPlotter.AIRCRAFT_ICON)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.Write(toWrite.Item1);

                        Console.ResetColor();

                        continue;
                    }

                    // change mapping metadatatable stuff cyan
                    if (toWrite.Item1.Contains(":"))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;

                        Console.Write(toWrite.Item1);

                        Console.ResetColor();

                        continue;
                    }

                    Console.Write(toWrite.Item1);
                }

            });
        }

        public static void Write(Tuple<string, int, int> value)
        {
            writes.Add(value);
        }

    }
}
