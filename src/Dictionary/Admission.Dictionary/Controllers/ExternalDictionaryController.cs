using Admission.Dictionary.Abstractions;
using Dictionary.Integration;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Dictionary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalDictionaryController(IDictionaryService dictionaryService) : ControllerBase
{
    [HttpGet("education_levels")]
    public async Task<ActionResult<ICollection<EducationLevelModel>>> GetEducationLevels()
    {
        var educationLevels = await dictionaryService.GetEducationLevelsAsync();
        return Ok(educationLevels);
    }

    [HttpGet("document_types")]
    public async Task<ActionResult<ICollection<EducationDocumentTypeModel>>> GetEducationDocumentTypes()
    {
        var documentTypes = await dictionaryService.GetDocumentTypesAsync();
        return Ok(documentTypes);
    }
    
    [HttpGet("faculties")]
    public async Task<ActionResult<ICollection<FacultyModel>>> GetFacultiesAsync()
    {
        var faculties = await dictionaryService.GetFacultiesAsync();
        return Ok(faculties);
    }

    [HttpGet("programs")]
    public async Task<ActionResult<ProgramPagedListModel>> GetPagedList(int page = 1, int pageSize = 5)
    {
        var pageList = await dictionaryService.GetProgramsPageAsync(page, pageSize);
        return Ok(pageList);
    }
}