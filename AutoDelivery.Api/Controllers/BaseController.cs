using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AutoDelivery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        public JsonSerializerSettings setting = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };
    }
}