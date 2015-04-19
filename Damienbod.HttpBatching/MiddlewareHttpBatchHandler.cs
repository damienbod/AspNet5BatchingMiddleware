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
        private BatchExecutionOrder _executionOrder;

        public MiddlewareHttpBatchHandler()
        {
            ExecutionOrder = BatchExecutionOrder.Sequential;
            SupportedContentTypes = new List<string>() { MultiPartMixed };
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

			//ValidateRequest(context.Request);

			IList<HttpRequestMessage> subRequests = await ParseBatchRequestsAsync(context, cancellationToken);

			try
			{
				IList<HttpResponseMessage> responses = await ExecuteRequestMessagesAsync(subRequests, cancellationToken);
				return await CreateResponseMessageAsync(responses, context, cancellationToken);
			}
			finally
			{
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
			var valuecc = context.Request.Headers.TryGetValue("Content-Type", out headersArray);

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
				//innerRequest.CopyBatchRequestProperties(context.Request);
				requests.Add(innerRequest);
			}
			return requests;
        }

        public void ValidateRequest(HttpRequestMessage request)
        {
            if (request == null)
			{
				throw Error.ArgumentNull("request");
			}

			// TODO
			//if (request.Content == null)
			//{
			//	throw new HttpResponseException(request.CreateErrorResponse(
			//		HttpStatusCode.BadRequest,
			//		SRResources.BatchRequestMissingContent));
			//}

			//MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;
			//if (contentType == null)
			//{
			//	throw new HttpResponseException(request.CreateErrorResponse(
			//		HttpStatusCode.BadRequest,
			//		SRResources.BatchContentTypeMissing));
			//}

			//if (!SupportedContentTypes.Contains(contentType.MediaType, StringComparer.OrdinalIgnoreCase))
			//{
			//	throw new HttpResponseException(request.CreateErrorResponse(
			//		HttpStatusCode.BadRequest,
			//		Error.Format(SRResources.BatchMediaTypeNotSupported, contentType.MediaType)));
			//}
		}

	}
}