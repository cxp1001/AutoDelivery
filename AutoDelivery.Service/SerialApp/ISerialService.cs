using AutoDelivery.Core;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.SerialApp
{
    public interface ISerialService : IocTag
    {
        Task<Serial> AddSerialAsync(string name, string sku, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink);
        Task<Serial> EditSerialAsync(int id, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool? used = false);
        Task<IEnumerable<Serial>> GetSerialDtoAsync(string name, string? serialNum, string? activeKey, string? subActiveKey, string? activeLink, bool used, PageWithSortDto pageWithSortDto);

        Task<bool> DeleteSerialAsync(int id);
    }
}