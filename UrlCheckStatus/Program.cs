using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace UrlCheckStatus
{
    internal class Program
    {
        public static void Main(string[] urls)
        {
            int number = urls.Length;
            bool[] statuses = Enumerable.Repeat(true, number).ToArray();

            while (statuses.Any(x => x))
            {
                string[] responses = GetResponses(urls, statuses, number);
                for (int i = 0; i < responses.Length; i++)
                {
                    if (statuses[i])
                    {
                        statuses[i] = !IsValid(responses[i]);
                    }
                }
            }
        }

        private static string[] GetResponses(string[] urls, bool[] statuses, int number)
        {
            if (number > urls.Length)
            {
                number = urls.Length;
            }

            Task<string>[] taskList = new Task<string>[number];
            for (int i = 0; i < number; i++)
            {
                taskList[i] = statuses[i]
                    ? GetResponseAsync(urls[i])
                    : Task.FromResult("");
            }

            var allTask = Task.WhenAll(taskList);
            return allTask.Result;
        }

        private static async Task<string> GetResponseAsync(string url)
        {
            HttpClient client = new HttpClient();
            try
            {
                var response = await client.PostAsync(url, null);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static bool IsValid(string response)
        {
            return response.StartsWith("<!DOCTYPE html>");
        }
    }
}