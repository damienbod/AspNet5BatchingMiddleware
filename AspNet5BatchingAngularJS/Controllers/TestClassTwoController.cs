using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace AspNet5BatchingAngularJS.Controllers
{
	[Route("api/[controller]")]
	public class TestClassTwoController : Controller
	{
		[HttpGet]
		[Route("")]
		public IEnumerable<TestClassTwo> Get()
		{
			return new List<TestClassTwo> { new TestClassTwo { Id = 1, News = "two" } };
		}
	}
}