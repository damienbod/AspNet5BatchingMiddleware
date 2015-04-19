using Microsoft.AspNet.Builder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Owin;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Core;
using System.Threading;
using System.Net.Http;

namespace Damienbod.HttpBatching
{

	public static class IApplicationBuilderExtensions
	{
		public static void UseBatchOwinMiddleware(this IApplicationBuilder app)
		{
			app.Use((context, next) =>
			{
				if (context.Request.Path.Value == "/api/$batch")
				{
					MiddlewareHttpBatchHandler batchHandler = new MiddlewareHttpBatchHandler();

					var result = GetBatch(() => batchHandler.ProcessBatchAsync(context, CancellationToken.None));

					context.Response.ContentType = "multipart/batch";
                    return context.Response.WriteAsync(result.ReadAsStringAsync().Result);
				};
				return next.Invoke();
			});
		}

		public static MultipartContent GetBatch(Func<Task<MultipartContent>> method)
		{
			try
			{
				Task<MultipartContent> task = Task.Run(() => method.Invoke());
				task.Wait();
				return task.Result;
			}
			catch (AggregateException ae)
			{
				// TODO
			}

			return null;
		}


	}



}