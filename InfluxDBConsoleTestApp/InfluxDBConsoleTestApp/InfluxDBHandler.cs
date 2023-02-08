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
        const string bucket = "mcc";

        static string _timestamp;
        static string _value;

        private InfluxDBClient client;

        private string _filePath;
        private string _token;
        private string _org;

        public InfluxDBHandler(string filePath, string token, string org)
        {
            _filePath = filePath;
            _token = token;
            _org = org;
        }

        public async void Connect()
        {
            client = InfluxDBClientFactory.Create("http://127.0.0.1:8086", _token.ToCharArray());            
        }

        public int AddData()
        {
            var counter = 0;
            var c = new CultureInfo("de-DE");
            using (FileStream fs = System.IO.File.Open(this._filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string line;
                sr.ReadLine();
                using (var writeApi = client.GetWriteApi())
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split(';');
                        var point = PointData.Measurement("archive")
                        .Field(parts[0], decimal.Parse(parts[1], c))
                        .Timestamp(DateTime.Parse(parts[2]), WritePrecision.Ns); //2022-08-02 09:27:36.087000000

                        try {
                            writeApi.WritePoint(point, bucket, _org);
                            Console.WriteLine("Wrote Data");
                            counter++;
                        } 
                        catch (Exception e) { }
                        

                    }
                }
            }
            return counter;
        }

        public async void getData()
        {
            var queryApi = client.GetQueryApi();
            
            var query = "from(bucket:\"" + bucket + "\") |> range(start: -30d) |> filter(fn: (r) => r[\"_field\"] == \"SRM.RBG01.Dashboard.ChemicalFiberRope.Display.Axes.HoistUnit.Jerk\")";
            var fluxTables = await queryApi.QueryAsync(query, _org);
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
