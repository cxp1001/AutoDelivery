using AutoDelivery.Core;
using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;
using AutoDelivery.Service.AutoMapper;

namespace AutoDelivery.Service.SerialApp
{
    public interface ISerialService : IDependency
    {
        Task<Serial> AddSerialAsync(int userId, int productId, string? serialNumber, string? activeKey, string? subActiveKey, string? activeLink, bool used = false);
        Task<int> CountAllProductsOfCurrentUserAsync(int userId);
        Task<int> CountSerialsAsync(int userId, string name, string? serialNumber, string? activeKey, string? subActiveKey, string? activeLink, bool used);
        Task<Serial> DeleteSerialAsync(int userId, int productId, int serialId);
        Task<Serial> EditSerialAsync(int userId, int productId, int serialId, string? serialNumber, string? activeKey, string? subActiveKey, string? activeLink, bool? used = false);
        Task<IQueryable<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId);
        Task<IQueryable<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId, PageWithSortDto pageWithSortDto);
        Task<IEnumerable<SerialDto>> GetSerialDtoAsync(int userId, string productName, string? serialNumber, string? activeKey, string? subActiveKey, string? activeLink, bool used, PageWithSortDto pageWithSortDto);
    }
}