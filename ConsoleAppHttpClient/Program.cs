using System;
using System.Net.Http;

namespace ConsoleAppHttpClient
{
    public class Program
    {
        public void Main(string[] args)
        {
            Console.WriteLine("Hello World");

			var client = new HttpClient();
			var batchRequest = new HttpRequestMessage(
				HttpMethod.Post,
				"http://localhost/api/batch"
			);

			var batchContent = new MultipartContent("batch");
			batchRequest.Content = batchContent;

			batchContent.Add(
				new HttpMessageContent(
					new HttpRequestMessage(
						HttpMethod.Get,
						"http://localhost/api/values"
					)
				)
			);

			batchContent.Add(
				new HttpMessageContent(
					new HttpRequestMessage(
						HttpMethod.Get,
						"http://localhost/foo/bar"
					)
				)
			);

			batchContent.Add(
				new HttpMessageContent(
					new HttpRequestMessage(
						HttpMethod.Get,
						"http://localhost/api/values/1"
					)
				)
			);

			Console.ReadLine();
        }
    }
}
