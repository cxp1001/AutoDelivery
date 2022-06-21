using AutoDelivery.Core;
using AutoDelivery.Domain;
using AutoDelivery.Service.SerialApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoDelivery.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SerialController : ControllerBase
    {
        private readonly ISerialService _serialService;
        public SerialController(ISerialService serialService)
        {
            this._serialService = serialService;

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
            var res = await _serialService.EditSerialAsync(id,serialNum,activeKey,subActiveKey,activeLink,used);
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
            else{
                return NotFound("serial not existed.");
            }
        }
    }
}