using AutoMapper;
using SaaS.Modules.Drivers.DTOs;
using SaaS.Modules.Drivers.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaaS.Modules.Drivers.Mappings
{
    public sealed class DriverProfile : Profile
    {
        public DriverProfile()
        {
            // CreateDriverDto → Driver entity
            CreateMap<CreateDriverDto, Driver>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(_ => 0m))
                .ForMember(dest => dest.TripsCompleted, opt => opt.MapFrom(_ => 0));

            // Driver entity → DriverResponseDto
            CreateMap<Driver, DriverResponseDto>();

            // UpdateDriverDto → Driver entity (only non-null properties)
            CreateMap<UpdateDriverDto, Driver>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
