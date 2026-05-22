using Admission.Domain.Abstractions;

namespace Admission.Domain.Entities;

public class Document(Guid id) : BaseEntity(id)
{
    public File? File { get; set; }
    public void SetFile(File? file)
    {
        File = file;
    }
}