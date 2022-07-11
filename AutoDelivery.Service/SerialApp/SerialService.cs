using AutoDelivery.Core;
using AutoDelivery.Core.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using AutoDelivery.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AutoDelivery.Domain.User;
using Newtonsoft.Json;
using AutoDelivery.Domain.Result;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoDelivery.Service.AutoMapper;

namespace AutoDelivery.Service.SerialApp
{
    public class SerialService : ISerialService
    {
        private readonly IRepository<Serial> _serialRepo;
        private readonly AutoDeliveryContext _dbContext;
        private readonly IRepository<UserAccount> _userRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IMapper _mapper;

        public SerialService(IRepository<Serial> serialRepo, IRepository<UserAccount> userRepo, IRepository<Product> productRepo, AutoDeliveryContext dbContext, IMapper mapper)
        {
            this._mapper = mapper;
            this._productRepo = productRepo;
            this._userRepo = userRepo;
            this._serialRepo = serialRepo;
            this._dbContext = dbContext;
        }


        /// <summary>
        /// 拉取当前用户的所有产品-序列号信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IQueryable<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId)
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

            var serialsOfAllProducts = _productRepo.GetQueryable().AsNoTracking().Include(p => p.SerialsInventory).
            Where(p => productIdsOfCurrentUser.Contains(p.Id) && p != null).Select(p => new SerialsInfoList { ProductId = p.Id, ProductName = p.ProductName, SerialInfo = p.SerialsInventory});

            return serialsOfAllProducts;

        }


        /// <summary>
        /// 当前用户的产品总数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CountAllProductsOfCurrentUserAsync(int userId)
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

            var serialsCount = _productRepo.GetQueryable().AsNoTracking().Where(p => productIdsOfCurrentUser.Contains(p.Id)).Count();

            return serialsCount;

        }


        /// <summary>
        /// 拉取当前用户的所有产品-序列号信息,并添加分页和排序
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IQueryable<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId, PageWithSortDto pageWithSortDto)
        {

            // 排序字符
            pageWithSortDto.Sort ??= "ProductName";
            // 分页跳过的页数
            int skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

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

            var serialsOfAllProducts = _productRepo.GetQueryable().AsNoTracking().Include(p => p.SerialsInventory).Where(p => productIdsOfCurrentUser.Contains(p.Id) ).Select(p => new SerialsInfoList { ProductId = p.Id, ProductName = p.ProductName, SerialInfo = p.SerialsInventory });

            return serialsOfAllProducts.OrderBy(pageWithSortDto.Sort, (Convert.ToBoolean(pageWithSortDto.OrderType))).Skip(skip).Take(pageWithSortDto.PageSize);


        }



        // 序列号模糊查找结果数
        public async Task<int> CountSerialsAsync(int userId,
            string name,
            string? serialNumber,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool used)
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
            //获取用户的所有产品ID
            var productsNames = currentUser.Products.Select(p => p.ProductName);
            var serialsOfCurrentUser = _serialRepo.GetQueryable().AsNoTracking().Where(s => productsNames.Contains(s.ProductName));

            // 查询
            var serials = serialsOfCurrentUser.Where(
                m => m.ProductName.ToLower().Contains(name.ToLower()) &&

                 (!string.IsNullOrWhiteSpace(m.SerialNumber) && (!string.IsNullOrWhiteSpace(serialNumber)) && (m.SerialNumber.ToLower().Contains(serialNumber.ToLower()))
                 || string.IsNullOrWhiteSpace(serialNumber)) &&

                (!string.IsNullOrWhiteSpace(m.ActiveKey) && (!string.IsNullOrWhiteSpace(activeKey)) && (m.ActiveKey.ToLower().Contains(activeKey.ToLower()))
                 || string.IsNullOrWhiteSpace(activeKey)) &&

                (!string.IsNullOrWhiteSpace(m.SubActiveKey) && (!string.IsNullOrWhiteSpace(subActiveKey)) && (m.SubActiveKey.ToLower().Contains(subActiveKey.ToLower()))
                 || string.IsNullOrWhiteSpace(subActiveKey)) &&

                (!string.IsNullOrWhiteSpace(m.ActiveLink) && (!string.IsNullOrWhiteSpace(activeLink)) && (m.ActiveLink.ToLower().Contains(activeLink.ToLower()))
                 || string.IsNullOrWhiteSpace(activeLink)) &&

                (m.Used == used)
                );
            var totalSerials = serials.Count();
            return totalSerials;

        }



        /// <summary>
        /// 序列号模糊查找
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productName"></param>
        /// <param name="serialNumber"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <param name="used"></param>
        /// <param name="pageWithSortDto"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SerialDto>> GetSerialDtoAsync(
            int userId,
            string productName,
            string? serialNumber,
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
            //获取用户的所有产品ID
            var productsNames = currentUser.Products.Select(p => p.ProductName);
            var serialsOfCurrentUser = _serialRepo.GetQueryable().AsNoTracking().Where(s => productsNames.Contains(s.ProductName));



            // 查询
            var serials = serialsOfCurrentUser.Where(
                m => m.ProductName.ToLower().Contains(productName.ToLower()) &&

                 (!string.IsNullOrWhiteSpace(m.SerialNumber) && (!string.IsNullOrWhiteSpace(serialNumber)) && (m.SerialNumber.ToLower().Contains(serialNumber.ToLower()))
                 || string.IsNullOrWhiteSpace(serialNumber)) &&

                (!string.IsNullOrWhiteSpace(m.ActiveKey) && (!string.IsNullOrWhiteSpace(activeKey)) && (m.ActiveKey.ToLower().Contains(activeKey.ToLower()))
                 || string.IsNullOrWhiteSpace(activeKey)) &&

                (!string.IsNullOrWhiteSpace(m.SubActiveKey) && (!string.IsNullOrWhiteSpace(subActiveKey)) && (m.SubActiveKey.ToLower().Contains(subActiveKey.ToLower()))
                 || string.IsNullOrWhiteSpace(subActiveKey)) &&

                (!string.IsNullOrWhiteSpace(m.ActiveLink) && (!string.IsNullOrWhiteSpace(activeLink)) && (m.ActiveLink.ToLower().Contains(activeLink.ToLower()))
                 || string.IsNullOrWhiteSpace(activeLink)) &&

                (m.Used == used)
                ).OrderBy(pageWithSortDto.Sort, (Convert.ToBoolean(pageWithSortDto.OrderType))).Skip(skip).Take(pageWithSortDto.PageSize).
                ProjectTo<SerialDto>(_mapper.ConfigurationProvider);


            return await serials.ToListAsync();
        }


        /// <summary>
        ///  用户在下拉列表中选择已有的产品并为其添加序列号
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="sku"></param>
        /// <param name="serialNumber"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <returns></returns>
        public async Task<Serial> AddSerialAsync(int userId, int productId,
            string? serialNumber,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool used = false)
        {
            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);

            // 确定用户是否存在
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

            // 要添加序列号的产品是否存在
            var currentProduct = currentUser.Products.SingleOrDefault(p => p.Id == productId);
            if (currentProduct == null)
            {
                throw new NullReferenceException(
                   JsonConvert.SerializeObject(new
                   {
                       Status = 8,
                       ErrorMessage = "target product is not exist.",
                       Time = DateTimeOffset.Now
                   }));
            }

            // 生成新的序列号实体
            Serial newSerial = new()
            {
                ProductName = currentProduct.ProductName,
                ProductSku = currentProduct.ProductSku,
                SerialNumber = serialNumber,
                ActiveKey = activeKey,
                SubActiveKey = subActiveKey,
                ActiveLink = activeLink,
                Used = used,
                CreatedTime = DateTime.Now
            };


            // 将序列号实体添加到数据库中
            var insertedSerial = await _serialRepo.InsertAsync(newSerial);

            // 将序列号添加到产品的导航属性中
            var product = await _productRepo.GetQueryable().Include(p => p.SerialsInventory).SingleAsync(p => p.Id == productId);
            product.SerialsInventory.Add(newSerial);
            await _productRepo.UpdateAsync(product);



            return insertedSerial;

        }


        /// <summary>
        /// 编辑序列号 ,用户在下拉列表中选择产品并选择序列号进行编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serialNumber"></param>
        /// <param name="activeKey"></param>
        /// <param name="subActiveKey"></param>
        /// <param name="activeLink"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        public async Task<Serial> EditSerialAsync(int userId,
            int productId,
            int serialId,
            string? serialNumber,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool? used = false)
        {

            // 获取当前用户的产品-序列号信息
            var products = await GetAllSerialsOfCurrentUserAsync(userId);

            // 要更改的产品是否存在于当前用户的产品库中
            if (!products.Select(p => p.ProductId).Contains(productId))
            {
                var resultString = new Result()
                {
                    Time = DateTimeOffset.Now,
                    Status = 8,
                    ErrorMessage = "target product is not exist."
                };
                throw new NullReferenceException(
                   JsonConvert.SerializeObject(resultString
                   )
               );
            }

            var serial = await _serialRepo.GetQueryable().FirstOrDefaultAsync(s => s.Id == serialId);
            if (serial != null)
            {
                serial.SerialNumber = serialNumber == null ? serial.SerialNumber : serialNumber;
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
        public async Task<Serial> DeleteSerialAsync(int userId, int productId, int serialId)
        {
            // 获取当前用户的产品-序列号信息
            var products = await GetAllSerialsOfCurrentUserAsync(userId);

            // 要更改的产品是否存在于当前用户的产品库中
            if (!products.Select(p => p.ProductId).Contains(productId))
            {
                var resultString = new Result()
                {
                    Time = DateTimeOffset.Now,
                    Status = 8,
                    ErrorMessage = "target product is not exist."
                };
                throw new NullReferenceException(
                   JsonConvert.SerializeObject(resultString
                   )
               );
            }


            var targetSerial = await _serialRepo.GetQueryable().FirstOrDefaultAsync(s => s.Id == serialId);
            if (targetSerial != null)
            {
                //return await _serialRepo.DeleteAsync(targetSerial);
                var deletedSerial = await _serialRepo.DeleteAsync(targetSerial);
                return deletedSerial;
            }
            else
            {
                return null;
            }


        }
    }


}