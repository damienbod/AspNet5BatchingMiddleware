using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiBatchService.Controllers
{
	using System.Web.Http;

	[RoutePrefix("api/testclassone")]
	public class TestClassOneController : ApiController
	{
		[HttpGet]
		[Route("")]
		public IEnumerable<TestClassOne> Get()
		{
			return new List<TestClassOne>
						{
							new TestClassOne { Id = 1, Description = "hello" },
							 new TestClassOne { Id = 2, Description = "hello from two" }
						};
		}
	}
}