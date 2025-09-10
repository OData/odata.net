using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Utils;

internal struct PossibleDuplicateStringChecker(StringComparison comparison = default)
{
    const int NumBuckets = sizeof(ulong);
    private ulong _buckets;

    public bool TryAdd(string item)
    {
        int hash = item.GetHashCode(comparison) & 0x7FFFFFFF; // avoid negative hash codes
        int bucket = hash % NumBuckets;
        ulong mask = 1UL << bucket;

        if ((_buckets & mask) != 0)
        {
            return false; // possible duplicate
        }

        _buckets |= mask;

        return true;
    }
}
