using EducationSystem.Application.Abstarctions.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganisationController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var organisation = _unitOfWork.OrganisationRepo.GetByIdAsync(id);

        if (organisation is not null)
            return Ok(organisation);

        return NotFound("sorry organisation not found ");
    }
};