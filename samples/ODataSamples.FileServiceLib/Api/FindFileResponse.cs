using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Api;

public sealed class FindFileResponse(IEnumerable<ISchematizedObject<IFileItemSchema>> items, string skipToken)
{
    public IEnumerable<ISchematizedObject<IFileItemSchema>> Items { get; } = items;
    public string SkipToken { get; } = skipToken;
}
