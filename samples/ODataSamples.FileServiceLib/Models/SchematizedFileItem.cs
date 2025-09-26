using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

public class SchematizedFileItem : ISchematizedObject<IFileItemSchema>
{
    public IFileItemSchema Schema => throw new NotImplementedException();

    public IDictionary<IPropertyDefinition, object?> Data => throw new NotImplementedException();

    public string Id
    {
        get => (string)this.Data[this.Schema.Id]!;
        set => this.Data[this.Schema.Id] = value;
    }

    public string GetEtag()
            {
        if (this.Data.TryGetValue(this.Schema.Version, out var version) && version is string versionString)
        {
            return $"W/\"{versionString}\"";
        }

        return string.Empty;
    }
}
