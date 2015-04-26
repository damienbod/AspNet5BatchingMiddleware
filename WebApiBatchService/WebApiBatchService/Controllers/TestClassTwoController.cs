using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiBatchService.Controllers
{
	using System.Web.Http;

	[RoutePrefix("api/testclasstwo")]
	public class TestClassTwoController : ApiController
	{
		[HttpGet]
		[Route("")]
		public IEnumerable<TestClassTwo> Get()
		{
			return new List<TestClassTwo> { new TestClassTwo { Id = 1, News = "two" } };
		}
	}
}