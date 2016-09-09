using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nether.Analytics.Web.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Analytics.Web.Controllers
{
    [Route("api/endpoint")]
    public class EndpointController : Controller
    {
        // GET: api/endpoint
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            throw new NotImplementedException();

            return Ok(new EndpointResponseModel());
        }
    }
}
