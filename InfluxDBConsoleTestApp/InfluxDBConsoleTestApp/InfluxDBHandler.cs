using InfluxDB.Client;
using InfluxDB.Client.Core;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using InfluxDB.Client.Flux;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Cache;

namespace InfluxDBConsoleTestApp
{
    internal class InfluxDBHandler
    {
        //Docker: yi4RB9gXMUzRRTfVdBDndgRijpMEjRvrSjB2f0C7JNLcd4L18FA5SWVwOJKElViiSc4twUB_csWSLjO-axW9Bg==
        //Windows: 6AMYuigbcNO3E3lPW0NZSbAfTagor2LCRRVqidKqau_4ynS5EjAWS4sHbHaiBiejWsR-rEIvxo_d0vFDgy8eog==
        const string token = "6AMYuigbcNO3E3lPW0NZSbAfTagor2LCRRVqidKqau_4ynS5EjAWS4sHbHaiBiejWsR-rEIvxo_d0vFDgy8eog==";
        const string bucket = "test2";
        const string org = "Kardex Mlog";

        static string _timestamp;
        static string _value;

        private InfluxDBClient client;

        public async void Connect()
        {
            client = InfluxDBClientFactory.Create("http://localhost:8086", token.ToCharArray());
        }

        public int AddData()
        {
            var counter = 0;
            var c = new CultureInfo("de-DE");
            string filePath = "D:\\Mlog\\AA_EM-F\\Dashboard\\InfluxDB\\2022-07-01_2023-02-06_export.csv";
            using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string line;
                sr.ReadLine();
                using (var writeApi = client.GetWriteApi())
                {  
                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split(';');
                        var point = PointData.Measurement("HRL1")
                        .Tag("Tag", parts[0])
                        .Field(parts[0], decimal.Parse(parts[1], c))
                        .Timestamp(DateTime.Parse(parts[2]), WritePrecision.Ns); //2022-08-02 09:27:36.087000000

                        writeApi.WritePoint(point, bucket, org);
                        Console.WriteLine("Wrote Data");
                        counter++;

                    }
                }
            }
            return counter;
        }

        public async void getData()
        {
            var queryApi = client.GetQueryApi();
            
            var query = "from(bucket:\"" + bucket + "\") |> range(start: -30d) |> filter(fn: (r) => r[\"_field\"] == \"SRM.RBG01.Dashboard.ChemicalFiberRope.Display.Axes.HoistUnit.Jerk\")";
            var fluxTables = await queryApi.QueryAsync(query, org);
            fluxTables.ForEach(table =>
            {
                table.Records.ForEach(record =>
                {
                    Console.WriteLine($"{record.GetTime()}: {record.GetValueByKey("_value")}: {record.GetValueByKey("_field")}");
                });
            });
            
            
        }
    }
}
