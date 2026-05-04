using SaaS.Modules.HomePage.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Modules.HomePage.Services
{
    public interface IHomePageService
    {
        Task<List<HomePageSectionDto>> GetAllAsync();
        Task<HomePageSectionDto> CreateAsync(HomePageSectionDto dto);
        Task<HomePageSectionDto> UpdateAsync(Guid id, HomePageSectionDto dto);
    }
}
