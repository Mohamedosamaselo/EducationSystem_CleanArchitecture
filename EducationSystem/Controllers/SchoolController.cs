using EducationSystem.Application.Abstarctions.Services;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class SchoolController(ISchoolService schoolService) : ControllerBase
{
    private readonly ISchoolService _schoolService = schoolService;


    /// <summary>
    /// Allows Users to getAll 
    /// </summary>
    /// <param name="id"></param>
    /// <returns> return all schools from DB </returns>

    [HttpGet]
    public async Task<IActionResult> GetById(Guid id)
    {
        var school = await _schoolService.GetByIdAsync(id);
        return Ok(school);
    }


}