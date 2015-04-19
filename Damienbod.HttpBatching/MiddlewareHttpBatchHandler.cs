using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Aspwebstack;

namespace Damienbod.HttpBatching
{
    /// <summary>
    /// Default implementation of <see cref="HttpBatchHandler"/> that encodes the HTTP request/response messages as MIME multipart.
    /// </summary>
    /// <remarks>
    /// By default, it buffers the HTTP request messages in memory during parsing.
    /// </remarks>
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

        public Task<HttpResponseMessage> CreateResponseMessageAsync(IList<HttpResponseMessage> responses, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (responses == null)
            {
                throw Error.ArgumentNull("responses");
            }
            if (request == null)
            {
				// TODO
                //throw Error.ArgumentNull("request");
            }

            MultipartContent batchContent = new MultipartContent(MultiPartContentSubtype);

            foreach (HttpResponseMessage batchResponse in responses)
            {
                batchContent.Add(batchResponse.Content);
            }

			// TODO add headers
			HttpResponseMessage response = new HttpResponseMessage();
            response.Content = batchContent;
            return Task.FromResult(response);
        }

		//public async Task<HttpResponseMessage> ProcessBatchAsync(HttpContext context, CancellationToken cancellationToken)
		//{
		//	// TEST
		//	//byte[] result;
		//	//using (var stream = new MemoryStream())
		//	//{
		//	//	context.Request.Body.CopyTo(stream);
		//	//	result = stream.ToArray();


		//	//	// TODO: do something with the result
		//	//}

		//	//var str = System.Text.Encoding.Default.GetString(result);

		//	IList<HttpRequestMessage> subRequests = new List<HttpRequestMessage>();
  //          try
		//	{
		//		IList<HttpResponseMessage> responses = await ExecuteRequestMessagesAsync(subRequests, cancellationToken);
		//		return await CreateResponseMessageAsync(responses, null, cancellationToken);
		//	}
		//	catch(Exception e)
		//	{
		//		string err = e.Message;
		//	}

		//	return null;
  //      }

		public async Task<HttpResponseMessage> ProcessBatchAsync(Microsoft.AspNet.Http.HttpContext context, CancellationToken cancellationToken)
		{
			if (context.Request == null)
			{
				// TODO
				throw Error.ArgumentNull("request");
			}

			//ValidateRequest(context.Request);

			IList<HttpRequestMessage> subRequests = await ParseBatchRequestsAsync(context, cancellationToken);

			try
			{
				IList<HttpResponseMessage> responses = await ExecuteRequestMessagesAsync(subRequests, cancellationToken);
				return null;
				//return await CreateResponseMessageAsync(responses, context.Request, cancellationToken);
			}
			finally
			{
				//foreach (HttpRequestMessage subRequest in subRequests)
				//{
				//    request.RegisterForDispose(subRequest.GetResourcesForDisposal());
				//    request.RegisterForDispose(subRequest);
				//}
			}
		}

		public async Task<IList<HttpResponseMessage>> ExecuteRequestMessagesAsync(IEnumerable<HttpRequestMessage> requests, CancellationToken cancellationToken)
        {
            if (requests == null)
            {
				// TODO
                //throw Error.ArgumentNull("requests");
            }

            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

			// TODO
            //try
            //{
            //    switch (ExecutionOrder)
            //    {
            //        case BatchExecutionOrder.Sequential:
            //            foreach (HttpRequestMessage request in requests)
            //            {
            //                responses.Add(await Invoker.SendAsync(request, cancellationToken));
            //            }
            //            break;

            //        case BatchExecutionOrder.NonSequential:
            //            responses.AddRange(await Task.WhenAll(requests.Select(request => Invoker.SendAsync(request, cancellationToken))));
            //            break;
            //    }
            //}
            //catch
            //{
            //    foreach (HttpResponseMessage response in responses)
            //    {
            //        if (response != null)
            //        {
            //            response.Dispose();
            //        }
            //    }
            //    throw;
            //}

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
				
				//cancellationToken.ThrowIfCancellationRequested();
				//HttpRequestMessage innerRequest = await httpContent.ReadAsHttpRequestMessageAsync();
				//innerRequest.CopyBatchRequestProperties(request);
				//requests.Add(innerRequest);
			}
			return requests;
        }

        public void ValidateRequest(HttpRequestMessage request)
        {
			// TODO
            //if (request == null)
            //{
            //    throw Error.ArgumentNull("request");
            //}

            //if (request.Content == null)
            //{
            //    throw new HttpResponseException(request.CreateErrorResponse(
            //        HttpStatusCode.BadRequest,
            //        SRResources.BatchRequestMissingContent));
            //}

            //MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;
            //if (contentType == null)
            //{
            //    throw new HttpResponseException(request.CreateErrorResponse(
            //        HttpStatusCode.BadRequest,
            //        SRResources.BatchContentTypeMissing));
            //}

            //if (!SupportedContentTypes.Contains(contentType.MediaType, StringComparer.OrdinalIgnoreCase))
            //{
            //    throw new HttpResponseException(request.CreateErrorResponse(
            //        HttpStatusCode.BadRequest,
            //        Error.Format(SRResources.BatchMediaTypeNotSupported, contentType.MediaType)));
            //}
        }

	}
}