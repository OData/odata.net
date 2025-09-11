using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Utils;

/// <summary>
/// Small inline zero-allocation that stores up to 10 strings.
/// 
/// </summary>
internal struct InlineStringList10
{
    private InlineStringArray10 _array;

    public int Length { get; private set; }

    public readonly string this[int index] => _array[index];

    public bool TryAdd(string item)
    {
        if (Length == InlineStringArray10.Capacity)
        {
            return false;
        }

        _array[Length++] = item;
        return true;
    }

    [InlineArray(Capacity)]
    internal struct InlineStringArray10
    {
        internal const int Capacity = 10;
        private string _item;
    }
}


