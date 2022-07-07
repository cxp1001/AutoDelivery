using AutoDelivery.Core;
using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.SerialApp
{
    public interface ISerialService : IDependency
    {
        Task<Serial> AddSerialAsync(int userId, int productId, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool used = false);
        Task<Serial> DeleteSerialAsync(int userId, int productId, int serialId);
        Task<Serial> EditSerialAsync(int userId, int productId, int serialId, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool? used = false);
        Task<IQueryable<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId);
        Task<IEnumerable<Serial>> GetSerialDtoAsync(int userId, string name, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool used, PageWithSortDto pageWithSortDto);
        Task<int> CountSerialsAsync(int userId,
            string name,
            string? serialNum,
            string? activeKey,
            string? subActiveKey,
            string? activeLink,
            bool used);
    }
}