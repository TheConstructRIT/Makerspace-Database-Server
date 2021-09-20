using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Threading.Tasks;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Configuration;
using Construct.Core.Server;
using Newtonsoft.Json;

namespace Construct.Base.Test.Integration.Base
{
    public class BaseIntegrationTest : BaseSqliteTest
    {
        /// <summary>
        /// Starts the program to test.
        /// </summary>
        /// <typeparam name="T">Program type to start.</typeparam>
        public void StartProgram<T>() where T : class
        {
            // Set the ports.
            foreach (var name in ConstructConfiguration.Configuration.Ports.Keys)
            {
                var tcpListener = new TcpListener(IPAddress.Loopback, 0);
                tcpListener.Start();
                ConstructConfiguration.Configuration.Ports[name] = ((IPEndPoint) tcpListener.LocalEndpoint).Port;
                tcpListener.Stop();
            }
            
            // Write the configuration.
            ConstructConfiguration.Configuration.SaveAsync().Wait();
            
            // Start the program.
            var _ = Task.Run(() =>
            {
                typeof(T).GetMethod("Main")?.Invoke(null, new object[] {Array.Empty<string>()});
            });
            while (Startup.LifeTime == null)
            {
                Task.Delay(50).Wait();
            }
        }

        /// <summary>
        /// Stops the program.
        /// </summary>
        public void StopProgram()
        {
            ServerProgram.Stop();
        }

        /// <summary>
        /// Sends a GET request.
        /// </summary>
        /// <param name="host">Host service to send.</param>
        /// <param name="path">Path to send.</param>
        /// <typeparam name="T">Type of the response.</typeparam>
        /// <returns>The response of the request.</returns>
        public (T, HttpStatusCode) Get<T>(string host, string path)
        {
            var client = new HttpClient();
            var url = "http://localhost:" + ConstructConfiguration.Configuration.Ports[host] + path;
            var response = client.GetAsync(url).Result;
            return (JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result), response.StatusCode);
        }
        
        /// <summary>
        /// Sends a POST request.
        /// </summary>
        /// <param name="host">Host service to send.</param>
        /// <param name="path">Path to send.</param>
        /// <param name="data">Data to send.</param>
        /// <typeparam name="T">Type of the response.</typeparam>
        /// <returns>The response of the request.</returns>
        public (T, HttpStatusCode) Post<T>(string host, string path, object data)
        {
            var client = new HttpClient();
            var url = "http://localhost:" + ConstructConfiguration.Configuration.Ports[host] + path;
            var response = client.PostAsync(url, JsonContent.Create(data)).Result;
            return (JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result), response.StatusCode);
        }
    }
}