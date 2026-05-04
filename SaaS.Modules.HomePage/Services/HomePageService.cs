using SaaS.Modules.HomePage.Entities;
using SaaS.Modules.HomePage.DTOs;
using SaaS.Modules.HomePage;

public class HomePageService
{
    private readonly IHomePageRepository _repository;

    public HomePageService(IHomePageRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HomePageSectionDto>> GetAllAsync()
    {
        var data = await _repository.GetAllAsync();

        return data
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new HomePageSectionDto
            {
                Id = x.Id,
                SectionKey = x.SectionKey,
                Title = x.Title,
                Subtitle = x.Subtitle,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                DisplayOrder = x.DisplayOrder
            });
    }

    public async Task<HomePageSectionDto> CreateAsync(HomePageSectionDto dto)
    {
        var entity = new HomePageSection
        {
            Id = Guid.NewGuid(),
            SectionKey = dto.SectionKey,
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);

        dto.Id = entity.Id;
        return dto;
    }

    public async Task<HomePageSectionDto> UpdateAsync(Guid id, HomePageSectionDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity == null)
            throw new Exception("Section not found");

        entity.Title = dto.Title;
        entity.Subtitle = dto.Subtitle;
        entity.Description = dto.Description;
        entity.ImageUrl = dto.ImageUrl;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);

        return dto;
    }
}