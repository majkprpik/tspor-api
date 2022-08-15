using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace WebApi.ApiClients
{
    public class Vendor1ApiClient
    {
        private string ApiURL = "http://www.osijek031.com/";
        private readonly HttpClient httpClient;

        public Vendor1ApiClient(HttpClient httpClient) =>
                this.httpClient = httpClient;

        public async Task<bool> GetEvents(CancellationToken cancellationToken)
        {
            try
            {
                var events = await this.httpClient.GetStringAsync(
                    ApiURL, cancellationToken);

                // if (events is { Body.Count: 0 } or { Success: false })
                // {
                //     // consider creating custom exceptions for situations like this
                //     throw new InvalidOperationException("This API is no joke.");
                // }

                // XmlDocument xmlDoc = new XmlDocument();
                // xmlDoc.LoadXml(events);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(events);

                var name = htmlDoc.DocumentNode
                    .SelectNodes("//table[@id='main_calendar']")
                    .First();
                    // .Attributes["value"].Value;

                System.Console.WriteLine(events);    
            }
            catch (System.Exception e)
            {
                
                throw e;
            }
            return true;
        }
    }
}