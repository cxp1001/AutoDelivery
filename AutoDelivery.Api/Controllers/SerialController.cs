using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Result;
using AutoDelivery.Service.SerialApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AutoDelivery.Api.Extensions;
using Swashbuckle.AspNetCore.Annotations;

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

        /// <summary>
        /// 获取当前用户的所有产品和对应的序列号集合
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation(Summary = "获取当前用户的所有产品和对应的序列号集合")]
        [HttpGet("GetAllSerials")]
        public async Task<string> GetAllSerialsAsync()
        {
            //int userId = HttpContext.GetCurrentUserId();
            int userId = 6;

            var listofSerials = await _serialService.GetAllSerialsOfCurrentUserAsync(userId);
            var totalSerials = listofSerials.SelectMany(l => l.SerialInfo).Count();

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
                     },setting
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
                        

                },setting
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
                            ResultCount = listofSerials.Count(),
                            TotalCount = totalSerials
                        },setting
                    );
                }

            }
            return resString;
        } 


        /// <summary>
        /// 根据用户输入的信息模糊查找满足条件的序列号
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="serialNum"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <param name="used"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation(Summary = "根据用户输入的信息模糊查找满足条件的序列号")]
        public async Task<string> GetSerialsAsync(string productName,
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

            //int userId = HttpContext.GetCurrentUserId();
            int userId = 6;

            var serials = await _serialService.GetSerialDtoAsync(userId, productName, serialNum, activeKey, subActiveKey, activeLink, used,
            new PageWithSortDto()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Sort = sort,
                OrderType = orderType

            });

            var totalSerials = await _serialService.CountSerialsAsync(userId, productName, serialNum, activeKey, subActiveKey, activeLink, used);

            string resString;
            // 查询结果为空
            if (!serials.Any())
            {
                resString = JsonConvert.SerializeObject(
                   new Result()
                   {
                       ErrorMessage = "none serial info of all existed products",
                       Status = 14,
                       Time = DateTimeOffset.Now,
                       Data = null,
                       ResultCount = 0
                   },setting
                );
            }
            else
            {
                resString = JsonConvert.SerializeObject(
                  new Result()
                  {
                      ErrorMessage = "search of serials successful",
                      Status = 15,
                      Time = DateTimeOffset.Now,
                      Data = serials,
                      ResultCount = serials.Count(),
                      TotalCount = totalSerials
            },setting
                );
            }

            return resString;
        }


        /// <summary>
        /// 添加序列号
        /// </summary>
        /// <param name="productId">通过用户在下拉列表中选择的产品来获取当前产品的Id</param>
        /// <param name="serialNum"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(Summary = "添加序列号")]
        public async Task<string> AddNewSerialAsync(int productId,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool used = false
           )
        {
            //int userId = HttpContext.GetCurrentUserId();
            int userId = 4;

            var serial = await _serialService.AddSerialAsync(userId, productId, serialNum, activeKey, subActiveKey, activeLink, used);
            string resString;

            if (serial == null)
            {
                resString = JsonConvert.SerializeObject(
                   new Result()
                   {
                       ErrorMessage = $"add serial failed",
                       Status = 16,
                       Time = DateTimeOffset.Now,
                       Data = null,
                       ResultCount = 0
                   },setting
                );
            }
            else
            {
                resString = JsonConvert.SerializeObject(
                  new Result()
                  {
                      ErrorMessage = "add serial successful",
                      Status = 17,
                      Time = DateTimeOffset.Now,
                      Data = serial,
                      ResultCount = 1
                  },setting
                );
            }

            return resString;
        }


        /// <summary>
        /// 编辑序列号信息
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="serialId"></param>
        /// <param name="serialNum"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation(Summary = "编辑用户选择的产品的序列号")]
        public async Task<IActionResult> UpdateSerialAsync(int productId,
            int serialId,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool? used = false
        )
        {
            //int userId = HttpContext.GetCurrentUserId();
            int userId = 4;



            var updatedSerial = await _serialService.EditSerialAsync(userId, productId, serialId, serialNum, activeKey, subActiveKey, activeLink, used);
            if (updatedSerial != null)
            {

                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 17,
                    ErrorMessage = $"Serial of {updatedSerial.ProductName} updated successful",
                    Time = DateTimeOffset.Now
                },setting);

                return Ok(goodResString);
            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 18,
                    ErrorMessage = $"Serial of {updatedSerial.ProductName} updated failed",
                    Time = DateTimeOffset.Now
                },setting);

                return BadRequest(badResString);
            }

        }

        /// <summary>
        /// 删除序列号
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="serialId"></param>
        /// <returns></returns>
        [HttpDelete]
        [SwaggerOperation(Summary = "删除序列号")]
        public async Task<IActionResult> DeleteSerialAsync(int productId,
            int serialId)
        {
            //int userId = HttpContext.GetCurrentUserId();
            int userId = 4;

            var deletedSerial = await _serialService.DeleteSerialAsync(userId, productId, serialId);
            if (deletedSerial != null)
            {
                var goodResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 19,
                    ErrorMessage = $"Serial of {deletedSerial.ProductName} deleted successful",
                    Time = DateTimeOffset.Now
                },setting);

                return Ok(goodResString);
            }
            else
            {
                var badResString = JsonConvert.SerializeObject(new Result
                {
                    Status = 20,
                    ErrorMessage = $"Failed to delete serial of {deletedSerial.ProductName}.",
                    Time = DateTimeOffset.Now
                },setting);

                return BadRequest(badResString);
            }
        }
    }
}