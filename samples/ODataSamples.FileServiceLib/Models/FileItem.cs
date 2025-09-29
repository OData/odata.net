using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


}
