using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public static class PlaneDataProvider_Example
    {
        public static void Go()
        {
            PlaneDataProvider dataProvider = new PlaneDataProvider();
            CancellationTokenSource cts = new CancellationTokenSource();
            var task = dataProvider.StartAsync(cts.Token);

            while (!dataProvider.Initialized)
                System.Threading.Thread.Sleep(1000);

            var callSignNameList = dataProvider.GetCallSignNames();
            Console.WriteLine($"\n Call Sign Count: {callSignNameList.Count}");
            var callSignName = callSignNameList.First();
            Console.WriteLine($"\n Example Call Sign : {callSignName}\n");

            // Latest Value Query
            Console.WriteLine("####################################");
            Console.WriteLine("\n Value Query Example: Latest \n");
            Console.WriteLine("####################################");
            var dic1 = dataProvider.GetLatestPlaneData(callSignName);
            foreach (var kvp in dic1)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
            Console.WriteLine("\n");

            // Value Query At Time
            Console.WriteLine("##########################################");
            Console.WriteLine("\n Value Query Example: At specific Time \n");
            Console.WriteLine("##########################################");
            var dic2 = dataProvider.GetPlaneDataAtTime(callSignName, "*-1h");

            foreach (var kvp in dic2)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
            Console.WriteLine("\n");

            // Time Range Query
            Console.WriteLine("####################################");
            Console.WriteLine("\n Value Query Example: Time Range \n");
            Console.WriteLine("####################################");
            var dic3 = dataProvider.GetPlaneDataInInterval(callSignName, "*-1h", "*");

            foreach (var kvp in dic3)
            {
                Console.WriteLine($"\n TimeStamp Key: {kvp.Key}");
                foreach (var subKvp in kvp.Value)
                {
                    Console.WriteLine($"Key: {subKvp.Key}, Value: {subKvp.Value}");
                }
            }
            Console.WriteLine("\n");

            cts.Cancel();
            Task.WaitAny(task);

            Console.WriteLine("Press entery to finish...");
            Console.ReadKey();
        }
    }
}
