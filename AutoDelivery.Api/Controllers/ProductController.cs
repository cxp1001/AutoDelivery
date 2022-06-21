using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Service.ProductApp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopifyWebApi.Web.Extensions;

namespace AutoDelivery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            this._productService = productService;

        }

       


        /// <summary>
        /// 搜索产品
        /// </summary>
        /// <param name="productCategory"></param>
        /// <param name="productName"></param>
        /// <param name="productSku"></param>
        /// <param name="maker"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        [HttpGet("GetProducts")]
        public async Task<IEnumerable<Product>> GetProductsAsync(string? productCategory,
            string productName,
            string? productSku,
            string? maker,
             int pageIndex = 1,
            int pageSize = 20,
            string sort = "ProductName",
            OrderType orderType = OrderType.Asc)
        {
            return await _productService.GetProductDtoAsync(productCategory, productName, productSku, maker,
            new PageWithSortDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType
            });
        }


        // 获取所有分类
        [HttpGet("GetCategories")]
        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _productService.GetProductCategoriesAsync();
        }


        [HttpPost]
        public async Task<IActionResult> AddProductAsync(string name,
            string maker,
            string? mainName,
            string? subName,
            string? edition,
            string? version,
            string? commonName,
            string sku,
            string? detail,
            string? category,
            bool? hasActiveKey,
            bool? hasSubActiveKey,
            bool? hasActiveLink,
            bool? hasSerialNum,
            bool isAvailable = false)
        {
            var res = await _productService.AddProductAsync(name, maker, mainName, subName, edition, version, commonName, sku, detail, category, hasActiveKey, hasSubActiveKey, hasActiveLink, hasSerialNum, isAvailable);
            if (res != null)
            {
                return Ok("add product successful");
            }
            else
            {
                return BadRequest("error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync(int id, string? name)
        {
            var res = await _productService.EditProductAsync(id, name);
            if (res != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}