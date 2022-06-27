using System.Collections;
using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Result;
using AutoDelivery.Service.UserApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifyWebApi.Web.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace AutoDelivery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }


        // 获取当前用户的所有产品
        [SwaggerOperation(Summary = "拉取当前用户的所有产品")]
        [HttpGet("GetAllProducts")]
        public async Task<IEnumerable> GetAllProductsAsync(int pageIndex = 1,
          int pageSize = 20,
          string sort = "ProductName",
          OrderType orderType = OrderType.Asc)
        {
            //var userId = HttpContext.GetCurrentUserId();
            var userId = 10;
            var res = await _userService.GetAllProductsAsync(userId, new PageWithSortDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType
            });

            Result result = new();
            if (!res.Any())
            {
                
                result.Time = DateTimeOffset.Now;
                result.Status = 11;
                result.ErrorMessage = "none product";
            }
            else
            {
                result.Time = DateTimeOffset.Now;
                result.Data = res;
                result.Status = 21;
                result.ErrorMessage = "Get all products of current user successful.";
                result.ResultCount = res.Count();
            }

            return JsonConvert.SerializeObject(result);

        }



        /// <summary>
        /// 获取当前用户设置的所有产品分类
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation(Summary = "拉取当前用户设置的所有产品分类")]
        [HttpGet("GetCategories")]
        public async Task<IEnumerable> GetCategoriesAsync(int pageIndex = 1,
          int pageSize = 20,
          string sort = "ProductName",
          OrderType orderType = OrderType.Asc)
        {
            //var userId = HttpContext.GetCurrentUserId();
            var userId = 4;


            return await _userService.GetProductCategoriesAsync(userId, new PageWithSortDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType
            });
        }
    }
}