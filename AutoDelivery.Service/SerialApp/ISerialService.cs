using AutoDelivery.Core;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.SerialApp
{
    public interface ISerialService : IocTag
    {
          Task<Serial> AddSerialAsync(int userId, int productId, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool used = false);
        Task<bool> DeleteSerialAsync(int userId, int id);
        Task<Serial> EditSerialAsync(int userId, int id, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool? used = false);
        Task<IQueryable<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId);
        Task<IEnumerable<Serial>> GetSerialDtoAsync(int userId, string name, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool used, PageWithSortDto pageWithSortDto);
     }
}