namespace Microsoft.OData.Serializer;

internal struct PossibleDuplicateStringChecker()
{
    private const int BitsPerBucket = sizeof(long) * 8;
    private const int NumBuckets = BitsPerBucket * 2;

    // 128 buckets means we'll have false positive/collision probability
    // of about 5% when we have 4 items. We can increase the number of
    // buckets to reduce the probability, esp. if we have more than 4 items
    // to deal with on average.
    // If the number of items is high, e.g. >= 10, we could consider
    // using a bloom filter instead or a standard hash set.
    // This is version is meant to be a fast path for small number of items
    // with low-probability of duplicates. If it fails, the caller should
    // fallback to more robust solution.
    private long _buckets1;
    private long _buckets2;

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

        if (bucket < BitsPerBucket)
        {
            long mask = 1L << bucket;
            if ((_buckets1 & mask) != 0)
            {
                return false; // possible duplicate
            }
            _buckets1 |= mask;
        }
        else
        {
            int offset = bucket - BitsPerBucket;
            long mask = 1L << offset;
            if ((_buckets2 & mask) != 0)
            {
                return false; // possible duplicate
            }
            _buckets2 |= mask;
        }


        return true;
    }
}
