using Admission.Dictionary.Abstractions;
using Admission.Dictionary.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Dictionary.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController (IImportService importService) : ControllerBase
{
    [HttpPost("education_levels")]
    public async Task<ActionResult<IEnumerable<ImportedEducationLevelDto>>> ImportEducationLevelsAsync([FromBody] List<int> educationLevelsIds)
    {
        var res = await importService.ImportEducationLevels(educationLevelsIds);
        if (res.Count > 0)
            return StatusCode(StatusCodes.Status201Created, res);

        return Ok("Looks like education levels already were imported");
    }

    [HttpPost("document_types")]
    public async Task<ActionResult<IEnumerable<ImportedEducationDocumentTypeDto>>> ImportDocumentTypesAsync([FromBody] List<Guid> documentTypesIds)
    {
        var res = await importService.ImportDocumentTypes(documentTypesIds);
        if (res.Count > 0)
            return StatusCode(StatusCodes.Status201Created, res);

        return Ok("Looks like document types already were imported");
    }

    [HttpPost("faculties")]
    public async Task<ActionResult<IEnumerable<ImportedFacultyDto>>> ImportFacultiesAsync([FromBody] List<Guid> facultiesIds)
    {
        var res = await importService.ImportFaculties(facultiesIds);
        if (res.Count > 0) 
            return StatusCode(StatusCodes.Status201Created, res);

        return Ok("Looks like faculties already were imported");
    }

    [HttpPost("programs")]
    public async Task<ActionResult<IEnumerable<ImportedProgramDto>>> ImportProgramsAsync([FromBody] List<ImportProgramsParams> importProgramsParams)
    {
        var res = await importService.ImportPrograms(importProgramsParams);
        if (res.Count > 0)
            return StatusCode(StatusCodes.Status201Created, res);

        return Ok("Looks like programs already were imported");
    }
}
