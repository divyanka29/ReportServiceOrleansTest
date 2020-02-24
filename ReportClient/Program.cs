using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using ReportData;
using ReportGenericService;
using System;
using System.Collections.Generic;
using System.Fabric.Query;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportClient
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoClientWork(client, new GEReport("GE", "1"));
                    await DoClientWork(client, new PersonReport("Person"));
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        //private static async Task<IClusterClient> ConnectClient()
        //{
        //    var userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        //    var userSID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        //    var port = int.Parse(userSID.Split("-".ToCharArray()).Last()) % 10000;

        //    IClusterClient client;
        //    client = new ClientBuilder()
        //        .UseLocalhostClustering(
        //                //20000 + port,
        //                30000 + port,
        //                //null,
        //                userName,
        //                userName)
        //        .Configure<ClusterOptions>(options =>
        //        {
        //            options.ClusterId = "dev";
        //            options.ServiceId = "OrleansBasics";
        //        })
        //        .ConfigureLogging(logging => logging.AddConsole())
        //        .Build();

        //    await client.Connect();
        //    Console.WriteLine("Client successfully connected to silo host \n");
        //    return client;
        //}

        private static async Task<IClusterClient> ConnectClient()
        {
            var userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            var userSID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
            var port = 20;

            var serviceName = new Uri("fabric:/ReportService/Stateless1").ToString();

            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering(
                        30000 + port,
                        "other service",
                        userName)
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork<TReport>(IClusterClient client, TReport b) where TReport : ReportBase
        {
            // example of calling grains from the initialized client

            var friend = client.GetGrain<IReportGenericService<TReport>>(0);
            //var friend = client.GetGrain<IReportBaseService>(0);

            await friend.AddReport(b);
            await friend.Execute(b);
        }
    }
}
