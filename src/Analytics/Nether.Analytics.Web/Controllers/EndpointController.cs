using System;
using Microsoft.AspNetCore.Mvc;
using Nether.Analytics.Web.Models;
using Nether.Analytics.Web.Utilities;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Analytics.Web.Controllers
{
    [Route("api/endpoint")]
    public class EndpointController : Controller
    {
        // GET: api/endpoint
        [HttpGet]
        public ActionResult Get()
        {
            //TODO: Pick up the hardcoded information below from configuration as soon as we have that implemented
            var keyName = "test";
            var sharedAccessKey = "w8UXwPyDp6a0oxbeUyFoy6HEUOzJ0cYnVjt7muyzps4=";
            var resource = "https://netheranalytics-ns.servicebus.windows.net/gameevents/messages";
            var timeSpan = TimeSpan.FromHours(24);

            var validUntilUtc = DateTime.UtcNow + timeSpan;

            var authorization = SharedAccessSignatureTokenProviderEx.GetSharedAccessSignature(
                keyName,
                sharedAccessKey,
                resource,
                timeSpan);

            var result = new EndpointGetResponseModel()
            {
                HttpVerb = "POST",
                Url = resource,
                ContentType = "application/json",
                Authorization = authorization,
                ValidUntilUtc = validUntilUtc
            };

            return Ok(result);
        }
    }
}
