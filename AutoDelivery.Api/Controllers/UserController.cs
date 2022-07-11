using System.Collections;
using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Result;
using AutoDelivery.Service.UserApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AutoDelivery.Api.Extensions;
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
        public async Task<IEnumerable> GetAllProductsAsync(int userId=4,int pageIndex = 1,
          int pageSize = 20,
          string sort = "ProductName",
          OrderType orderType = OrderType.Asc)
        {
            //var userId = HttpContext.GetCurrentUserId();
         
            var res = await _userService.GetAllProductsAsync(userId, new PageWithSortDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType
            });

            var totalCount = await _userService.CountAllProductsAsync(userId);
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
                result.TotalCount = totalCount;
            }

            return JsonConvert.SerializeObject(result, setting);

        }



        /// <summary>
        /// 获取当前用户设置的所有产品分类
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation(Summary = "拉取当前用户设置的所有产品分类信息")]
        [HttpGet("GetCategories")]
        public async Task<string> GetCategoriesAsync(int userId=4,int pageIndex = 1,
          int pageSize = 20,
          string sort = "ProductName",
          OrderType orderType = OrderType.Asc)
        {
            //var userId = HttpContext.GetCurrentUserId();
     


            var categoies = await _userService.GetProductCategoriesAsync(userId, new PageWithSortDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType
            });

            var totalCategories = await _userService.CountCategories(userId);

            string resString;
            if (categoies.Any())
            {
                resString = JsonConvert.SerializeObject(new Result()
                {
                    Data = categoies,
                    Time = DateTimeOffset.Now,
                    ErrorMessage = "pull categories successful",
                    Status = 22,
                    ResultCount = categoies.Count(),
                    TotalCount = totalCategories

                }, setting);
            }
            else
            {
                resString = JsonConvert.SerializeObject(new Result()
                {

                    Time = DateTimeOffset.Now,
                    ErrorMessage = "none category",
                    Status = 23,
                }, setting);
            }

            HttpContext.Response.StatusCode = 200;
            return resString;

        }
    }
}