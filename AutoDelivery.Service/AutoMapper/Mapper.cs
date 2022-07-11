using AutoDelivery.Domain;
using AutoMapper;

namespace AutoDelivery.Service.AutoMapper
{
    public class MapperProfile : Profile
    {

        public MapperProfile()
        {
            CreateMap<Serial, SerialDto>().ForMember(dto => dto.SerialId, m => m.MapFrom(s => s.Id));
        }

    }
}