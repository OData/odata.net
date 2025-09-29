using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Api;

public sealed class FindFileResponse(IEnumerable<ISchematizedObject<IFileItemSchema>> items, string skipToken, int? count = null)
    // For now, you need to inherit IEnumerable<T> or other collection interfaces for the type to be treated like a collection
    : IEnumerable<ISchematizedObject<IFileItemSchema>>
{
    public IEnumerable<ISchematizedObject<IFileItemSchema>> Items { get; } = items;
    public string SkipToken { get; } = skipToken;
    public int? Count { get; } = count;

    public IEnumerator<ISchematizedObject<IFileItemSchema>> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
