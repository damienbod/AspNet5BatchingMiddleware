using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApiBatchService
{
    using System.Web.Http.Batch;
    using System.Web.Http.Cors;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors(new EnableCorsAttribute("*","*", "*"));

            // Web API configuration and services
            config.Routes.MapHttpBatchRoute(
               routeName: "batch",
               routeTemplate: "api/batch",
               batchHandler: new DefaultHttpBatchHandler(GlobalConfiguration.DefaultServer));

            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           
        }
    }
}
