using Microsoft.AspNetCore.Mvc;

namespace AutoDelivery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        public class Result
        {
            public int Status { get; set; }
            public string ErrorMessage { get; set; }
            public DateTimeOffset Time { get; set; }
            public Object Data { get; set; } = null;
            public int ResultCount { get; set; } = 0;

        }
    }
}