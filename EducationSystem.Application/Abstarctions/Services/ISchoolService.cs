using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISchoolService
{
    Task<IReadOnlyList<SchoolResponse>> GetAllAsync();

    Task<SchoolResponse> GetByIdAsync(Guid Id);

    Task<SchoolResponse> CreateAsync(CreateSchoolRequest createDto);

    Task<SchoolResponse> UpdateAsync(UpdateSchoolRequest updateDto);

    Task DeleteAsync(Guid Id);
}