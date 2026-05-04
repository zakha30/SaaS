using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Infrastructure.Repositories;
using SaaS.Modules.HomePage;
using SaaS.Modules.HomePage.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class HomePageRepository : IHomePageRepository
{
    private readonly AppDbContext _context;

    public HomePageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HomePageSection>> GetAllAsync()
    {
        return await _context.HomePageSections.ToListAsync();
    }

    public async Task<HomePageSection> GetByIdAsync(Guid id)
    {
        return await _context.HomePageSections.FindAsync(id);
    }

    public async Task AddAsync(HomePageSection entity)
    {
        _context.HomePageSections.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(HomePageSection entity)
    {
        _context.HomePageSections.Update(entity);
        await _context.SaveChangesAsync();
    }
}