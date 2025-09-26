using ODataSamples.FileServiceLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Serialization;

public class IdPropertySerializer(string rootUrl)
{
    public Uri GetODataId(SchematizedFileItem fileItem)
    {
        var id = fileItem.Id;
        return new Uri($"{rootUrl}/Files({id})", UriKind.Absolute);
    }
}
