using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataPropertyInfo
{
    private string name;

    public string? Name
    {
        get => name;
        set
        {
            ArgumentNullException.ThrowIfNullOrEmpty(value);
            if (!Utf8Name.IsEmpty)
            {
                throw new Exception("Cannot modify property info name");
            }

            Utf8Name = Encoding.UTF8.GetBytes(value);
            name = value;
        }
    }

    public Memory<byte> Utf8Name { get; private set; }
}
