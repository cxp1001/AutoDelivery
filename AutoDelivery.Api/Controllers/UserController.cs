using System.Collections;
using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Service.UserApp;
using Microsoft.AspNetCore.Mvc;
using ShopifyWebApi.Web.Extensions;

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
        [HttpGet("GetAllProducts")]
        public async Task<IEnumerable> GetAllProducts(int pageIndex = 1,
            int pageSize = 20,
            string sort = "ProductName",
            OrderType orderType = OrderType.Asc)
        {
            //var userId = HttpContext.GetCurrentUserId();
            var userId = 4;
            return await _userService.GetAllProducts(userId, new PageWithSortDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType
            });

        }



        /// <summary>
        /// 获取当前用户设置的所有产品分类
        /// </summary>
        /// <returns></returns>
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