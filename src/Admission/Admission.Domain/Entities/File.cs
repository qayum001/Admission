using Admission.Domain.Abstractions;

namespace Admission.Domain.Entities;

public class File : BaseEntity
{
    private File()
    {
        Key = string.Empty;
    }

    public File(Guid id, string key) : base(id)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("File key cannot be empty.", nameof(key));

        Key = key;
    }

    public string Key { get; private set; }
    
    public void UpdateKey(string newKey)
    {
        if (string.IsNullOrWhiteSpace(newKey))
            throw new ArgumentException("File key cannot be empty.", nameof(newKey));

        Key = newKey;
    }
}
