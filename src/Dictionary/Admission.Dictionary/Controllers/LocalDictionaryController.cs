using Admission.Dictionary.Abstractions;
using Admission.Dictionary.Entities;
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
    public async Task<ActionResult<List<EducationProgram>>> GetProgramsAsync()
    {
        var res = await localDictionaryService.GetProgramsAsync();
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
