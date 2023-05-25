﻿// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Newtonsoft.Json
// .NET Framework 4.7.1 or greater must be used

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PR_MashineLearning_BikeShareServiceApp;

await InvokeRequestResponseService();

async Task InvokeRequestResponseService()
{
    var handler = new HttpClientHandler()
    {
        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
    };
    using (var client = new HttpClient(handler))
    {
        // Request data goes here
        // The example below assumes JSON formatting which may be updated
        // depending on the format your endpoint expects.
        // More information can be found here:
        // https://docs.microsoft.com/azure/machine-learning/how-to-deploy-advanced-entry-script
        //var requestBody = @"{
        //          ""Inputs"": {
        //            ""data"": [
        //              {
        //                ""instant"": 553,
        //                ""date"": ""2019-07-07T00:00:00.000Z"",
        //                ""season"": 3,
        //                ""yr"": 1,
        //                ""mnth"": 7,
        //                ""weekday"": 6,
        //                ""weathersit"": 1,
        //                ""temp"": 35.328347,
        //                ""atemp"": 40.24565,
        //                ""hum"": 49.2083,
        //                ""windspeed"": 10.958118
        //              }
        //            ]
        //          },
        //          ""GlobalParameters"": 0.0
        //        }";

        BikeShareData bikeShareData = new() {
            Inputs = new Inputs() { Data = new List<Datum>
                {
                    new Datum
                    {
                         Instant = 553,
                         Date = "2019-07-07 00:00:00",
                         Season = 3,
                         Yr = 1,
                         Mnth = 7,
                         Weekday = 6,
                         Weathersit = 1,
                         Temp = 35.328347,
                         Atemp = 40.24565,
                         Hum = 49.2083,
                         Windspeed = 10.958118
                    }
                } 
            }
        };


        client.BaseAddress = new Uri("http://2b5a45f4-4ddb-4172-985a-74d5de6e1477.northeurope.azurecontainer.io/score");
        string requestBody = JsonConvert.SerializeObject(bikeShareData);
        var content = new StringContent(requestBody);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // WARNING: The 'await' statement below can result in a deadlock
        // if you are calling this code from the UI thread of an ASP.Net application.
        // One way to address this would be to call ConfigureAwait(false)
        // so that the execution does not attempt to resume on the original context.
        // For instance, replace code such as:
        //      result = await DoSomeTask()
        // with the following:
        //      result = await DoSomeTask().ConfigureAwait(false)
        HttpResponseMessage response = await client.PostAsync("", content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Result: {0}", result);
            BikeShareResult? bikeShareResult = JsonConvert.DeserializeObject<BikeShareResult>(result);
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"Result : {bikeShareResult?.Results[0]}");
        }
        else
        {
            Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

            // Print the headers - they include the requert ID and the timestamp,
            // which are useful for debugging the failure
            Console.WriteLine(response.Headers.ToString());

            string responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine(responseContent);
        }
    }
}