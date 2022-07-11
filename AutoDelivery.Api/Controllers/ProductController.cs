using System.Security.Cryptography.X509Certificates;
using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Result;
using AutoDelivery.Service.ProductApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AutoDelivery.Api.Extensions;
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
            OrderType orderType = OrderType.Asc,
            int userId = 4)
        {

            // 获取用户的Id
            //var userId = HttpContext.GetCurrentUserId();


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
            var totalCount = await _productService.CountProductsAsync(userId,
                productCategory, productName, productSku, maker);

            if (totalCount != 0)
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
                result.TotalCount = totalCount;

            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                result.Data = null;
                result.Time = DateTimeOffset.Now;
                result.Status = 1;
                result.ErrorMessage = "Products is null (No product results found matching your query)";

            }

            return JsonConvert.SerializeObject(result, setting);

        }


        /// <summary>
        /// 用户添加产品
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="maker"></param>
        /// <param name="mainName"></param>
        /// <param name="subName"></param>
        /// <param name="productEdition"></param>
        /// <param name="productVersion"></param>
        /// <param name="productCommonName"></param>
        /// <param name="productSku"></param>
        /// <param name="productDetails"></param>
        /// <param name="productCategory"></param>
        /// <param name="hasActiveKey"></param>
        /// <param name="hasSubActiveKey"></param>
        /// <param name="hasActiveLink"></param>
        /// <param name="hasSerialNum"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "添加产品")]
        [HttpPost]

        public async Task<IActionResult> AddProductAsync(string productName,
            string maker,
            string? mainName,
            string? subName,
            string? productEdition,
            string? productVersion,
            string? productCommonName,
            string productSku,
            string? productDetails,
            string? productCategory,
            bool? hasActiveKey,
            bool? hasSubActiveKey,
            bool? hasActiveLink,
            bool? hasSerialNum,
            bool isAvailable = false, int userId = 4)
        {
            // 获取用户的Id
            // var userId = HttpContext.GetCurrentUserId();


            var insertedProduct = await _productService.AddProductAsync(userId, productName, maker, mainName, subName, productEdition, productVersion, productCommonName, productSku, productDetails, productCategory, hasActiveKey, hasSubActiveKey, hasActiveLink, hasSerialNum, isAvailable);

            if (insertedProduct != null)
            {
                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 3,
                    ErrorMessage = $"Product {insertedProduct.ProductName} added successfully",
                    Time = DateTimeOffset.Now
                }, setting);

                return Ok(goodResString);
            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 4,
                    ErrorMessage = $"Failed to add product {productName}",
                    Time = DateTimeOffset.Now
                }, setting);
                return BadRequest(badResString);
            }
        }


        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productName"></param>
        /// <param name="maker"></param>
        /// <param name="mainName"></param>
        /// <param name="subName"></param>
        /// <param name="productEdition"></param>
        /// <param name="productVersion"></param>
        /// <param name="productCommonName"></param>
        /// <param name="productSku"></param>
        /// <param name="productDetails"></param>
        /// <param name="productCategory"></param>
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
            string? productName,
            string? maker,
            string? mainName,
            string? subName,
            string? productEdition,
            string? productVersion,
            string? productCommonName,
            string? productSku,
            string? productDetails,
            string? productCategory,
            int? categoryId,
            bool? hasActiveKey,
            bool? hasSubActiveKey,
            bool? hasActiveLink,
            bool? hasSerialNum,
            bool IsAvailable,
            int userId=4)
        {
            // 获取用户的Id
            // var userId = HttpContext.GetCurrentUserId();
           

            var updatedProduct = await _productService.EditProductAsync(userId, id, productName, maker, mainName, subName, productEdition, productVersion, productCommonName, productSku, productDetails, productCategory, categoryId,
            hasActiveKey, hasSubActiveKey, hasActiveLink, hasSerialNum, IsAvailable);
            if (updatedProduct != null)
            {
                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 6,
                    ErrorMessage = $"Product {updatedProduct.ProductName}'s information updated successfully",
                    Time = DateTimeOffset.Now
                }, setting);

                return Ok(goodResString);

            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 7,
                    ErrorMessage = $"The update of the product {updatedProduct.ProductName}'s information was unsuccessful",
                    Time = DateTimeOffset.Now
                }, setting);

                return Ok(badResString);
            }
        }


        [HttpDelete]
        [SwaggerOperation(Summary = "删除产品")]
        public async Task<ActionResult> DeleteProductAsync(int userId,int id)
        {

            // 获取用户的Id
            // var userId = HttpContext.GetCurrentUserId();
           

            var deletedProduct = await _productService.DeleteProductAsync(userId, id);

            if (deletedProduct != null)
            {
                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 9,
                    ErrorMessage = $"Product {deletedProduct.ProductName}'s information deleted successfully",
                    Time = DateTimeOffset.Now
                }, setting);

                return Ok(goodResString);

            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 10,
                    ErrorMessage = $"The delete of the product was unsuccessful",
                    Time = DateTimeOffset.Now
                }, setting);

                return Ok(badResString);
            }


        }




    }



}