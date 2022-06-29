using AutoDelivery.Core.Core;
using AutoDelivery.Domain.Secrets;
using AutoDelivery.Domain.Url;
using Microsoft.AspNetCore.Mvc;

namespace AutoDelivery.Api.Controllers
{

    public class AuthController : BaseController
    {
        private readonly ISecrets _secrets;
        private readonly AutoDeliveryContext _dbContex;
        private readonly IApplicationUrls _applicationUrls;
        public AuthController(AutoDeliveryContext dbContex, IApplicationUrls applicationUrls, ISecrets secrets)
        {
            this._applicationUrls = applicationUrls;
            this._dbContex = dbContex;
            this._secrets = secrets;
        }


      

    }
}