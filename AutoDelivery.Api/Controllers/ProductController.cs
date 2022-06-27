using System.Security.Cryptography.X509Certificates;
using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Result;
using AutoDelivery.Service.ProductApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifyWebApi.Web.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace AutoDelivery.Api.Controllers
{
    //[Authorize]
    public class ProductController : BaseController
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
        [SwaggerOperation(Summary = "模糊搜索产品")]

        public async Task<string> GetProductsAsync(string? productCategory,
            string? productName,
            string? productSku,
            string? maker,
             int pageIndex = 1,
            int pageSize = 20,
            string sort = "ProductName",
            OrderType orderType = OrderType.Asc)
        {

            // 获取用户的Id
            // var userId = HttpContext.GetCurrentUserId();
            var userId = 5;

            var productsDto = await _productService.GetProductDtoAsync(userId,
                productCategory, productName, productSku, maker,
                new PageWithSortDto
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Sort = sort,
                    OrderType = orderType
                });

            var productCount = productsDto.Count();
            Result result = new();

            if (productCount != 0)
            {
                HttpContext.Response.StatusCode = 200;
                result.Data = new
                {
                    Products = productsDto.ToList()
                };
                result.Time = DateTimeOffset.Now;
                result.Status = 0;
                result.ErrorMessage = "Ok";
                result.ResultCount = productCount;

            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                result.Data = null;
                result.Time = DateTimeOffset.Now;
                result.Status = 1;
                result.ErrorMessage = "Products is null (No product results found matching your query)";

            }

            return JsonConvert.SerializeObject(result);

        }


        /// <summary>
        /// 用户添加产品
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maker"></param>
        /// <param name="mainName"></param>
        /// <param name="subName"></param>
        /// <param name="edition"></param>
        /// <param name="version"></param>
        /// <param name="commonName"></param>
        /// <param name="sku"></param>
        /// <param name="detail"></param>
        /// <param name="category"></param>
        /// <param name="hasActiveKey"></param>
        /// <param name="hasSubActiveKey"></param>
        /// <param name="hasActiveLink"></param>
        /// <param name="hasSerialNum"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "添加产品")]
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
            // 获取用户的Id
            // var userId = HttpContext.GetCurrentUserId();
            var userId = 4;

            var insertedProduct = await _productService.AddProductAsync(userId, name, maker, mainName, subName, edition, version, commonName, sku, detail, category, hasActiveKey, hasSubActiveKey, hasActiveLink, hasSerialNum, isAvailable);

            if (insertedProduct != null)
            {
                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 3,
                    ErrorMessage = $"Product {insertedProduct.ProductName} added successfully",
                    Time = DateTimeOffset.Now
                });

                return Ok(goodResString);
            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 4,
                    ErrorMessage = $"Failed to add product {name}",
                    Time = DateTimeOffset.Now
                });
                return BadRequest(badResString);
            }
        }


        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="maker"></param>
        /// <param name="mainName"></param>
        /// <param name="subName"></param>
        /// <param name="edition"></param>
        /// <param name="version"></param>
        /// <param name="commonName"></param>
        /// <param name="sku"></param>
        /// <param name="detail"></param>
        /// <param name="category"></param>
        /// <param name="categoryId"></param>
        /// <param name="hasActiveKey"></param>
        /// <param name="hasSubActiveKey"></param>
        /// <param name="hasActiveLink"></param>
        /// <param name="hasSerialNum"></param>
        /// <param name="IsAvailable"></param>
        /// <returns></returns>

        [HttpPut]
        [SwaggerOperation(Summary = "更改产品信息")]
        public async Task<IActionResult> UpdateProductAsync(
            int id,
            string? name,
            string? maker,
            string? mainName,
            string? subName,
            string? edition,
            string? version,
            string? commonName,
            string? sku,
            string? detail,
            string? category,
            int? categoryId,
            bool? hasActiveKey,
            bool? hasSubActiveKey,
            bool? hasActiveLink,
            bool? hasSerialNum,
            bool IsAvailable)
        {
            // 获取用户的Id
            // var userId = HttpContext.GetCurrentUserId();
            var userId = 4;

            var updatedProduct = await _productService.EditProductAsync(userId, id, name, maker, mainName, subName, edition, version, commonName, sku, detail, category, categoryId,
            hasActiveKey, hasSubActiveKey, hasActiveLink, hasSerialNum, IsAvailable);
            if (updatedProduct != null)
            {
                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 6,
                    ErrorMessage = $"Product {updatedProduct.ProductName}'s information updated successfully",
                    Time = DateTimeOffset.Now
                });

                return Ok(goodResString);

            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 7,
                    ErrorMessage = $"The update of the product {updatedProduct.ProductName}'s information was unsuccessful",
                    Time = DateTimeOffset.Now
                });

                return Ok(badResString);
            }
        }


        [HttpDelete]
        [SwaggerOperation(Summary = "删除产品")]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {

            // 获取用户的Id
            // var userId = HttpContext.GetCurrentUserId();
            var userId = 4;

            var deletedProduct = await _productService.DeleteProductAsync(userId, id);

            if (deletedProduct != null)
            {
                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 9,
                    ErrorMessage = $"Product {deletedProduct.ProductName}'s information deleted successfully",
                    Time = DateTimeOffset.Now
                });

                return Ok(goodResString);

            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 10,
                    ErrorMessage = $"The delete of the product was unsuccessful",
                    Time = DateTimeOffset.Now
                });

                return Ok(badResString);
            }


        }




    }



}