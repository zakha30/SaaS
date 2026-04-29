using AutoMapper;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Entities;

namespace SaaS.Infrastructure.Modules.Fleet.Mappings;

public sealed class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        CreateMap<CreateVehicleDto, Vehicle>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => VehicleStatus.Available))
            .ForMember(dest => dest.MembershipTier, opt => opt.MapFrom(src => MembershipTierConstants.Free))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<Vehicle, VehicleResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.MembershipTier, opt => opt.MapFrom(src => src.MembershipTier))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));

        CreateMap<UpdateVehicleDto, Vehicle>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
