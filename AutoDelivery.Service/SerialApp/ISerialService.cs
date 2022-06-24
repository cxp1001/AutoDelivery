using AutoDelivery.Core;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.SerialApp
{
    public interface ISerialService : IocTag
    {
        Task<Serial> AddSerialAsync(string name, string sku, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink);
        Task<bool> DeleteSerialAsync(int id);
        Task<Serial> EditSerialAsync(int id, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool? used = false);
        Task<List<SerialsInfoList>> GetAllSerialsOfCurrentUserAsync(int userId);
        Task<IEnumerable<Serial>> GetSerialDtoAsync(string name, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool used, PageWithSortDto pageWithSortDto);

    }
}