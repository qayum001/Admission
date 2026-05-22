using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities.Dictionary;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Managers.Commands;

public sealed record SetManagerFacultyCommand(Guid ManagerId, Guid? FacultyId) : ICommand;

public sealed class SetManagerFacultyCommandHandler(
    IRepository repository,
    ILocalDictionaryService localDictionaryService)
    : ICommandHandler<SetManagerFacultyCommand>
{
    public async Task HandleAsync(SetManagerFacultyCommand message, CancellationToken cancellationToken = default)
    {
        var manager = await repository.Managers
            .FirstOrDefaultAsync(m => m.Id == message.ManagerId, cancellationToken);

        if (manager is null)
            throw new NotFoundException("Manager not found");

        if (message.FacultyId.HasValue)
        {
            var faculty = await GetOrCreateFacultyAsync(message.FacultyId.Value, cancellationToken);
            manager.SetFaculty(faculty);
        }
        else
        {
            manager.ClearFaculty();
        }

        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Faculty> GetOrCreateFacultyAsync(Guid facultyId, CancellationToken ct)
    {
        var local = await repository.Faculties
            .FirstOrDefaultAsync(f => f.Id == facultyId, ct);

        if (local is not null)
            return local;

        var imported = await localDictionaryService.GetFacultiesAsync(ct);
        var dto = imported.FirstOrDefault(f => f.Id == facultyId)
            ?? throw new NotFoundException("Faculty not found in dictionary");

        var faculty = dto.ToFaculty();
        repository.Faculties.Add(faculty);
        return faculty;
    }
}
