using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace AspNet5BatchingAngularJS.Controllers
{
	[Route("api/[controller]")]
	public class TestClassOneController : Controller
	{
		[HttpGet]
		[Route("")]
		public IEnumerable<TestClassOne> Get()
		{
			return new List<TestClassOne> { new TestClassOne { Id = 1, Description = "hello" } };
		}
	}
}