using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Aspwebstack;
using System.Linq;

namespace Damienbod.HttpBatching
{
    public class MiddlewareHttpBatchHandler
    {
        private const string MultiPartContentSubtype = "mixed";
        private const string MultiPartMixed = "multipart/mixed";
		private const string MultiPartBatch = "multipart/batch";
		private BatchExecutionOrder _executionOrder;

        public MiddlewareHttpBatchHandler()
        {
            ExecutionOrder = BatchExecutionOrder.Sequential;
            SupportedContentTypes = new List<string>() { MultiPartMixed, MultiPartBatch };
        }

        public BatchExecutionOrder ExecutionOrder
        {
            get
            {
                return _executionOrder;
            }
            set
            {
                if (!Enum.IsDefined(typeof(BatchExecutionOrder), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(BatchExecutionOrder));
                }
                _executionOrder = value;
            }
        }

        public IList<string> SupportedContentTypes { get; private set; }

        public Task<MultipartContent> CreateResponseMessageAsync(IList<HttpResponseMessage> responses, Microsoft.AspNet.Http.HttpContext context, CancellationToken cancellationToken)
        {
            if (responses == null)
            {
                throw Error.ArgumentNull("responses");
            }
            if (context.Request == null)
            {
                throw Error.ArgumentNull("request");
            }

            MultipartContent batchContent = new MultipartContent(MultiPartContentSubtype);

            foreach (HttpResponseMessage batchResponse in responses)
            {
                batchContent.Add(batchResponse.Content);
            }
            return Task.FromResult(batchContent);
		}

		public async Task<MultipartContent> ProcessBatchAsync(Microsoft.AspNet.Http.HttpContext context, CancellationToken cancellationToken)
		{
			if (context.Request == null)
			{
				throw Error.ArgumentNull("request");
			}

			ValidateRequest(context);

			IList<HttpRequestMessage> subRequests = await ParseBatchRequestsAsync(context, cancellationToken);

			try
			{
				IList<HttpResponseMessage> responses = await ExecuteRequestMessagesAsync(subRequests, cancellationToken);
				return await CreateResponseMessageAsync(responses, context, cancellationToken);
			}
			finally
			{
				// TODO
				//foreach (HttpRequestMessage subRequest in subRequests)
				//{
				//	request.RegisterForDispose(subRequest.GetResourcesForDisposal());
				//	request.RegisterForDispose(subRequest);
				//}
			}
		}

		public async Task<IList<HttpResponseMessage>> ExecuteRequestMessagesAsync(IEnumerable<HttpRequestMessage> requests, CancellationToken cancellationToken)
        {
            if (requests == null)
            {
                throw Error.ArgumentNull("requests");
            }

            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

			// TODO this should use the same handler without any network...
			HttpClient httpClient = new HttpClient();
			try
			{
				switch (ExecutionOrder)
				{
					case BatchExecutionOrder.Sequential:
						foreach (HttpRequestMessage request in requests)
						{
							responses.Add(await httpClient.SendAsync(request, cancellationToken));
						}
						break;

					case BatchExecutionOrder.NonSequential:
						responses.AddRange(await Task.WhenAll(requests.Select(request => httpClient.SendAsync(request, cancellationToken))));
						break;
				}
			}
			catch
			{
				foreach (HttpResponseMessage response in responses)
				{
					if (response != null)
					{
						response.Dispose();
					}
				}
				throw;
			}

			return responses;
        }

        public async Task<IList<HttpRequestMessage>> ParseBatchRequestsAsync(Microsoft.AspNet.Http.HttpContext context, CancellationToken cancellationToken)
        {
            if (context.Request == null)
            {
				throw Error.ArgumentNull("request");
            }

			HttpContent content = new StreamContent(context.Request.Body);
			string[] headersArray;
			var tryGetOk = context.Request.Headers.TryGetValue("Content-Type", out headersArray);

            content.Headers.Remove("Content-Type");
			content.Headers.TryAddWithoutValidation("Content-Type", headersArray[0]);
			// Content-Type: multipart/batch; boundary="3a34a745-eb85-4599-9e27-e19691705f51"


			List<HttpRequestMessage> requests = new List<HttpRequestMessage>();
			cancellationToken.ThrowIfCancellationRequested();
			MultipartStreamProvider streamProvider = await content.ReadAsMultipartAsync();
			foreach (HttpContent httpContent in streamProvider.Contents)
			{
				cancellationToken.ThrowIfCancellationRequested();
				HttpRequestMessage innerRequest = await httpContent.ReadAsHttpRequestMessageAsync();

				// TODO these properties are important
				HttpRequestMessage request = new HttpRequestMessage();
				//request.Properties = context.Request
				innerRequest.CopyBatchRequestProperties(request);
				requests.Add(innerRequest);
			}
			return requests;
        }

        public void ValidateRequest(Microsoft.AspNet.Http.HttpContext context)
        {
            if (context.Request == null)
			{
				throw Error.ArgumentNull("request");
			}

            if (context.Request.ContentLength  == null)
			{
				throw new HttpRequestException("BatchRequestMissingContent");
			}

			string[] headersArray;
			var tryGetOk = context.Request.Headers.TryGetValue("Content-Type", out headersArray);
			if (string.IsNullOrEmpty(headersArray[0]))
			{
				throw new HttpRequestException("BatchContentTypeMissing");
			}
			string[] ctypes = headersArray[0].Split(';');
            if (!SupportedContentTypes.Contains(ctypes[0], StringComparer.OrdinalIgnoreCase))
			{
				throw new HttpRequestException("BatchMediaTypeNotSupported");
			}
		}

	}
}