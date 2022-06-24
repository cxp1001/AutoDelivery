using AutoDelivery.Core;
using AutoDelivery.Core.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using AutoDelivery.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ShopifyWebApi.Domain.User;
using Newtonsoft.Json;

namespace AutoDelivery.Service.SerialApp
{
    public class SerialService : ISerialService
    {
        private readonly IRepository<Serial> _serialRepo;
        private readonly AutoDeliveryContext _dbContext;
        private readonly IRepository<UserAccount> _userRepo;
        private readonly IRepository<Product> _productRepo;

        public SerialService(IRepository<Serial> serialRepo, IRepository<UserAccount> userRepo, IRepository<Product> productRepo, AutoDeliveryContext dbContext)
        {
            this._productRepo = productRepo;
            this._userRepo = userRepo;
            this._serialRepo = serialRepo;
            this._dbContext = dbContext;
        }


        public async Task<List<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId)
        {
            // 获取当前用户
            var currentUser = await _userRepo.GetQueryable().AsNoTracking().Include(u => u.Products).SingleOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 2,
                        ErrorMessage = "user is null",
                        Time = DateTimeOffset.Now
                    }
                    )

                );
            }



            var productIdsOfCurrentUser = currentUser.Products.Where(p => p != null).Select(p => p.Id);

            var serialsOfAllProducts = await _productRepo.GetQueryable().AsNoTracking().Include(p => p.SerialsInventory).Where(p => productIdsOfCurrentUser.Contains(p.Id) && p != null).Select(p => new SerialsInfoList { ProductName = p.ProductName, SerialInfo = p.SerialsInventory }).ToListAsync();
            return serialsOfAllProducts;

        }

        /// <summary>
        /// 序列号模糊查询
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serialNum"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <param name="used"></param>
        /// <param name="pageWithSortDto"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Serial>> GetSerialDtoAsync(
            string name,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool used,
            PageWithSortDto pageWithSortDto)
        {
            // 排序字符
            pageWithSortDto.Sort ??= "ProductName";
            // 分页跳过的页数
            int skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            // 查询
            var serials = _serialRepo.GetQueryable().Where(
                m => m.ProductName.ToLower().Contains(name.ToLower()) &&

                 (!string.IsNullOrWhiteSpace(m.SerialNumber) && (!string.IsNullOrWhiteSpace(serialNum)) && (m.SerialNumber.ToLower().Contains(serialNum.ToLower()))
                 || string.IsNullOrWhiteSpace(serialNum)) &&

                (!string.IsNullOrWhiteSpace(m.ActiveKey) && (!string.IsNullOrWhiteSpace(activeKey)) && (m.ActiveKey.ToLower().Contains(activeKey.ToLower()))
                 || string.IsNullOrWhiteSpace(activeKey)) &&

                (!string.IsNullOrWhiteSpace(m.SubActiveKey) && (!string.IsNullOrWhiteSpace(subActiveKey)) && (m.SubActiveKey.ToLower().Contains(subActiveKey.ToLower()))
                 || string.IsNullOrWhiteSpace(subActiveKey)) &&

                (!string.IsNullOrWhiteSpace(m.ActiveLink) && (!string.IsNullOrWhiteSpace(activeLink)) && (m.ActiveLink.ToLower().Contains(activeLink.ToLower()))
                 || string.IsNullOrWhiteSpace(activeLink)) &&

                (m.Used == used)
                ).OrderBy(pageWithSortDto.Sort).Skip(skip).Take(pageWithSortDto.PageSize).AsNoTracking();



            return await serials.ToListAsync();
        }


        /// <summary>
        /// 添加序列号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sku"></param>
        /// <param name="serialNum"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <returns></returns>
        public async Task<Serial> AddSerialAsync(string name,
            string sku,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink)
        {

            Serial newSerial = new()
            {
                ProductName = name,
                ProductSku = sku,
                SerialNumber = serialNum,
                ActiveKey = activeKey,
                SubActiveKey = subActiveKey,
                ActiveLink = activeLink,
                CreatedTime = DateTime.Now
            };


            return await _serialRepo.InsertAsync(newSerial);
        }


        /// <summary>
        /// 编辑序列号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serialNum"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        public async Task<Serial> EditSerialAsync(int id,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool? used = false)
        {
            var serial = await _serialRepo.GetQueryable().FirstOrDefaultAsync(s => s.Id == id);
            if (serial != null)
            {
                serial.SerialNumber = serialNum == null ? serial.SerialNumber : serialNum;
                serial.ActiveKey = activeKey == null ? serial.ActiveKey : activeKey;
                serial.SubActiveKey = subActiveKey == null ? serial.SubActiveKey : subActiveKey;
                serial.ActiveLink = activeLink == null ? serial.ActiveLink : activeLink;
                serial.Used = used == true ? true : false;


                return await _serialRepo.UpdateAsync(serial);

            }
            else
            {

                return null;
            }

        }

        /// <summary>
        /// 删除序列号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSerialAsync(int id)
        {
            var targetSerial = await _serialRepo.GetQueryable().FirstOrDefaultAsync(s => s.Id == id);

            if (targetSerial != null)
            {
                await _serialRepo.DeleteAsync(targetSerial);
                return true;
            }
            else
            {
                return false;
            }

        }
    }


}