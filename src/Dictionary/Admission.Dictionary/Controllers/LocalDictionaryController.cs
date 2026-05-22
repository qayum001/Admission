using Admission.Dictionary.Abstractions;
using Admission.Domain.Entities.Dictionary;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Dictionary.Controllers;

[ApiController]
[Route("api/local_dictionary")]
public class LocalDictionaryController(ILocalDictionaryService localDictionaryService) : ControllerBase
{
    [HttpGet("education_levels")]
    public async Task<ActionResult<List<EducationLevel>>> GetEducationLevelsAsync()
    {
        var res = await localDictionaryService.GetEducationLevelsAsync();
        if (res.Count > 0)
            return Ok(res);

        return NoContent();
    }

    [HttpGet("document_types")]
    public async Task<ActionResult<List<EducationDocumentType>>> GetDocumentTypesAsync()
    {
        var res = await localDictionaryService.GetDocumentTypesAsync();
        if (res.Count > 0)
            return Ok(res);

        return NoContent();
    }

    [HttpGet("faculties")]
    public async Task<ActionResult<List<Faculty>>> GetFacultiesAsync()
    {
        var res = await localDictionaryService.GetFacultiesAsync();

        if (res.Count > 0)
            return Ok(res);

        return NoContent();
    }

    [HttpGet("programs")]
    public async Task<ActionResult<List<EducationProgram>>> GetProgramsAsync(
        [FromQuery] Guid? facultyId = null,
        [FromQuery] int? educationLevelId = null,
        [FromQuery] string? language = null,
        [FromQuery] string? educationForm = null,
        [FromQuery] string? nameOrCode = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var res = await localDictionaryService.GetProgramsAsync(
            facultyId, educationLevelId, language, educationForm, nameOrCode, page, pageSize);

        if (res.Count > 0)
            return Ok(res);

        return NoContent();
    }

    [HttpGet("faculties/{facultyId:guid}/programs")]
    public async Task<ActionResult<List<EducationProgram>>> GetFacultyProgramsAsync(Guid facultyId)
    {
        var res = await localDictionaryService.GetProgramsByFacultyAsync(facultyId);
        if (res.Count > 0)
            return Ok(res);

        return NoContent();
    }
}
