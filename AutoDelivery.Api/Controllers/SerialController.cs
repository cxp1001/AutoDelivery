using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Service.SerialApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifyWebApi.Web.Extensions;

namespace AutoDelivery.Api.Controllers
{
    //[Authorize]

    public class SerialController : BaseController
    {
        private readonly ISerialService _serialService;
        public SerialController(ISerialService serialService)
        {
            this._serialService = serialService;

        }


        [HttpGet("GetAllSerials")]
        public async Task<string> GetAllSerialsAsync()
        {
            //int userId = HttpContext.GetCurrentUserId();
            int userId = 4;

            var listofSerials = await _serialService.GetAllSerialsOfCurrentUserAsync(userId);

            string resString;

            // 当前用户未添加任何产品
            if (!listofSerials.Any())
            {
                resString = JsonConvert.SerializeObject(
                     new Result()
                     {
                         ErrorMessage = "none product",
                         Status = 11,
                         Time = DateTimeOffset.Now,
                         Data = null,
                         ResultCount = 0
                     }
                 );
            }
            else
            {
                // 当前用户已添加的所有产品都没有配置序列号信息
                if (!listofSerials.SelectMany(l => l.SerialInfo).Any())
                {
                    resString = JsonConvert.SerializeObject(
                    new Result()
                    {
                        ErrorMessage = "none serial info of all existed products",
                        Status = 12,
                        Time = DateTimeOffset.Now,
                        Data = listofSerials,
                        ResultCount = listofSerials.Count()
                    }
                );
                }

                // 返回当前用户已添加的产品及对应的序列号信息
                else
                {
                    resString = JsonConvert.SerializeObject(
                        new Result()
                        {
                            ErrorMessage = "Successfully obtained all serial numbers of current user",
                            Status = 13,
                            Time = DateTimeOffset.Now,
                            Data = listofSerials,
                            ResultCount = listofSerials.Count()
                        }
                    );
                }

            }
            return resString;
        }



        [HttpGet]
        public async Task<IEnumerable<Serial>> GetSerialsAsync(string name,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool used,
            int pageIndex = 1,
            int pageSize = 20,
            string sort = "ProductName",
            OrderType orderType = OrderType.Asc
             )
        {
            return await _serialService.GetSerialDtoAsync(name, serialNum, activeKey, subActiveKey, activeLink, used,
            new PageWithSortDto()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType

            });
        }



        [HttpPost]
        public async Task<Serial> AddNewSerialAsync(string name,
            string sku,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink
           )
        {
            return await _serialService.AddSerialAsync(name, sku, serialNum, activeKey, subActiveKey, activeLink);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateSerialAsync(int id,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool? used = false
        )
        {
            var res = await _serialService.EditSerialAsync(id, serialNum, activeKey, subActiveKey, activeLink, used);
            if (res != null)
            {
                return Ok($"Serial of {res.ProductName} has been successfully changed!");
            }
            else
            {
                return NotFound("serial not found.");
            }

        }


        [HttpDelete]
        public async Task<IActionResult> DeleteSerialAsync(int id)
        {
            var res = await _serialService.DeleteSerialAsync(id);
            if (res)
            {
                return Ok("serial has been deleted.");
            }
            else
            {
                return NotFound("serial not existed.");
            }
        }
    }
}