using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiBatchService.Controllers
{
    using System.Web.Http;
    using System.Web.Http.Cors;

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/testclasstwo")]
    public class TestClassTwoController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IEnumerable<TestClassTwo> Get()
        {
            return new List<TestClassTwo>
                        {
                            new TestClassTwo { Id = 1, News = "news one" },
                            new TestClassTwo { Id = 2, News = "news for two" },
                            new TestClassTwo { Id = 3, News = "news for three" },
                            new TestClassTwo { Id = 4, News = "news for four" }
                        };
        }
    }
}