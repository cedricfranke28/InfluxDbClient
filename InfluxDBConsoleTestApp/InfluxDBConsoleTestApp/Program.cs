﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using System.Diagnostics;
using System.Reactive.Concurrency;

namespace InfluxDBConsoleTestApp
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
 
            Console.WriteLine("Starting Application!");
            Console.WriteLine();

            var influxHandler = new InfluxDBHandler();
            influxHandler.Connect();
            
            try
            {
                var count = 0;
                Stopwatch stopwatch = new Stopwatch();
                Console.WriteLine("Read (r) or Write (w)?");
                var input = Console.ReadLine();
                if (input == "w" || input == "R")
                {
                    stopwatch.Start();
                    Console.WriteLine("Store Data to Influx dashboard DB!");
                    count = influxHandler.AddData();
                    stopwatch.Stop();
                }
                else if (input == "r" || input == "R")
                {
                    stopwatch.Start();
                    Console.WriteLine("Read Data from Influx dashboard DB!");
                    influxHandler.getData();
                    stopwatch.Stop();
                }
                

                TimeSpan ts = stopwatch.Elapsed;

                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("RunTime " + elapsedTime);
                Console.WriteLine("Data: " + count.ToString());
                Console.ReadLine();
            } catch(Exception ex)
            {
                Console.WriteLine("Error Writing Data!");
                Console.ReadLine();
            }
        }
    }
}