using System;

namespace HttpBatchClient
{
	using System.IO;
	using System.Net.Http;

	class Program
	{
		static void Main(string[] args)
		{
			var client = new HttpClient();
			var batchRequest = new HttpRequestMessage(
				HttpMethod.Post,
				"http://localhost:52857/api/$batch"
			);

			var batchContent = new MultipartContent("batch");
			batchRequest.Content = batchContent;

			batchContent.Add(
				new HttpMessageContent(
					new HttpRequestMessage(
						HttpMethod.Get,
						"http://localhost:52857/api/TestClassOne"
					)
				)
			);

			batchContent.Add(
				new HttpMessageContent(
					new HttpRequestMessage(
						HttpMethod.Get,
						"http://localhost:52857/api/TestClassTwo"
					)
				)
			);

			using (Stream stdout = Console.OpenStandardOutput())
			{
				Console.WriteLine("<<< REQUEST >>>");
				Console.WriteLine();
				Console.WriteLine(batchRequest);
				Console.WriteLine();
				batchContent.CopyToAsync(stdout).Wait();
				Console.WriteLine();

				var batchResponse = client.SendAsync(batchRequest).Result;

				Console.WriteLine("<<< RESPONSE >>>");
				Console.WriteLine();
				Console.WriteLine(batchResponse);
				Console.WriteLine();
				batchResponse.Content.CopyToAsync(stdout).Wait();
				Console.WriteLine();
				Console.WriteLine();
			}

			Console.ReadLine();

		}
	}
}
