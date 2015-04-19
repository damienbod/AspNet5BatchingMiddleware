using Microsoft.AspNet.Builder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Owin;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Core;
using System.Threading;

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
				    batchHandler.ProcessBatchAsync(context, CancellationToken.None);

					context.Response.ContentType = "text/plain";
					return context.Response.WriteAsync("Hello, world. template for /api/$batch");
				};
				return next.Invoke();
			});
		}

		
	}



}