using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using WebApi.ApiClients;
using WebApi.Models;

namespace WebApi.HostedServices
{
    public class ScrapingJob : IJob
    {
        // private readonly HttpClient httpClient;
        // public ScrapingJob(HttpClient httpClient) =>
        //         this.httpClient = httpClient;

        public async Task Execute(IJobExecutionContext context)
        {
            var httpClient = new HttpClient();
            Common.Logs($"ScrapingJob at " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), "ScrapingJob" + DateTime.Now.ToString("hhmmss"));
            Vendor1ApiClient vendorApiClient = new Vendor1ApiClient(httpClient);
            try
            {
                await vendorApiClient.GetEvents(new CancellationToken());
            }
            catch (AggregateException e)
            {
                throw;
            }            
        }
    }
}