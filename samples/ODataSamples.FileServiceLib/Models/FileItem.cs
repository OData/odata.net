using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;

namespace ODataSamples.FileServiceLib.Models;

public class FileItem(IFileItemSchema schema, IDictionary<IPropertyDefinition, object?> data) : ISchematizedObject<IFileItemSchema>
{
    public IFileItemSchema Schema => schema;

    public IDictionary<IPropertyDefinition, object?> Data => data;

    public string Id
    {
        get => (string)this.Data[this.Schema.Id]!;
        set => this.Data[this.Schema.Id] = value;
    }

    public string? FileExtension
    {
        get => (string?)this.Data[this.Schema.FileExtension];
        set => this.Data[this.Schema.FileExtension] = value;
    }

    public long? FileSize
    {
        get => (long?)this.Data[this.Schema.FileSize];
        set => this.Data[this.Schema.FileSize] = value;
    }

    public int? Version
    {
        get => (int?)this.Data[this.Schema.Version];
        set => this.Data[this.Schema.Version] = value;
    }

    public bool? IsProtected
    {
        get => (bool?)this.Data[this.Schema.IsProtected];
        set => this.Data[this.Schema.IsProtected] = value;
    }

    public string? FileName
    {
        get => (string?)this.Data[this.Schema.FileName];
        set => this.Data[this.Schema.FileName] = value;
    }

    public string? Description
    {
        get => (string?)this.Data[this.Schema.Description];
        set => this.Data[this.Schema.Description] = value;
    }

    public Guid? ExternalId
    {
        get => (Guid?)this.Data[this.Schema.ExternalId];
        set => this.Data[this.Schema.ExternalId] = value;
    }

    public ICollection<string>? Tags
    {
        get => (ICollection<string>?)this.Data[this.Schema.Tags];
        set => this.Data[this.Schema.Tags] = value;
    }

    public FileContent? FileContent
    {
        get => (FileContent?)this.Data[this.Schema.FileContent];
        set => this.Data[this.Schema.FileContent] = value;
    }

    public IOpenPropertyValue? AllExtensions
    {
        get => (IOpenPropertyValue?)this.Data[this.Schema.AllExtensions];
        set => this.Data[this.Schema.AllExtensions] = value;
    }

    public IOpenPropertyValue? ItemProperties
    {
        get => (IOpenPropertyValue?)this.Data[this.Schema.ItemProperties];
        set => this.Data[this.Schema.ItemProperties] = value;
    }

    public DateTimeOffset? CreatedAt
    {
        get => (DateTimeOffset?)this.Data[this.Schema.CreatedAt];
        set => this.Data[this.Schema.CreatedAt] = value;
    }

    public AccessControlList? EntityACL
    {
        get => (AccessControlList?)this.Data[this.Schema.EntityACL];
        set => this.Data[this.Schema.EntityACL] = value;
    }

    public byte[]? BinaryData
    {
        get => (byte[]?)this.Data[this.Schema.BinaryData];
        set => this.Data[this.Schema.BinaryData] = value;
    }

    public byte[]? ByteCollection
    {
        get => (byte[]?)this.Data[this.Schema.ByteCollection];
        set => this.Data[this.Schema.ByteCollection] = value;
    }

    public IEnumerable<ActivityStat>? ActivityStats
    {
        get => (IEnumerable<ActivityStat>?)this.Data[this.Schema.ActivityStats];
        set => this.Data[this.Schema.ActivityStats] = value;
    }
}
