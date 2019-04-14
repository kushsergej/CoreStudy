using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleHttpClient
{
    public class Program
    {
        // Check port # in the following line:
        private static readonly string path = "https://localhost:44396/api";
        

        public static async Task Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Console.WriteLine($"--- Start ---");

                    HttpResponseMessage response = await client.GetAsync(path);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Exception Caught!");
                    Console.WriteLine($"Message :{ex.Message}");
                }
                finally
                {
                    Console.WriteLine($"--- Finish ---");
                    Console.ReadLine();
                }
            }
        }
    }
}