using SaaS.Modules.HomePage.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Modules.HomePage;

public interface IHomePageRepository
{
    Task<IEnumerable<HomePageSection>> GetAllAsync();
    Task<HomePageSection> GetByIdAsync(Guid id);
    Task AddAsync(HomePageSection entity);
    Task UpdateAsync(HomePageSection entity);
}
