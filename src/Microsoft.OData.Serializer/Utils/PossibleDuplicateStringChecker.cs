namespace Microsoft.OData.Serializer.Utils;

internal struct PossibleDuplicateStringChecker()
{
    const int NumBuckets = sizeof(ulong);
    private ulong _buckets;

    /// <summary>
    /// Attempts to add the item to the duplicate checker.
    /// Return true if the item was added. This guarantees the item is unique.
    /// Returns false if the item is possibly a duplicate (may return false positives).
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryAdd(string item)
    {
        int hash = item.GetHashCode() & 0x7FFFFFFF; // avoid negative hash codes otherwise we'll get negative modulo
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
