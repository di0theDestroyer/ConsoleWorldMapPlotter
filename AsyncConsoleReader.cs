using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class AsyncConsoleReader
    {
        public static Task<string> ReadLine()
        {
            return Task.Run(() => Console.ReadLine());
        }
    }
}
