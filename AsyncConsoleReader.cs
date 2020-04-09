using System;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class AsyncConsoleReader
    {
        public static Task<string> ReadLine()
        {
            return Task.Run(() =>
            {
                var userInput = Console.ReadLine();

                // make cursor invisible after input, so we can rehome it in calling logic
                Console.CursorVisible = false;

                return userInput;
            });
        }
    }
}
