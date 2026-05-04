using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.HomePage.DTOs;

[ApiController]
[Route("api/homepage")]
public class HomePageController : ControllerBase
{
    private readonly HomePageService _service;

    public HomePageController(HomePageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(HomePageSectionDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, HomePageSectionDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(result);
    }
}