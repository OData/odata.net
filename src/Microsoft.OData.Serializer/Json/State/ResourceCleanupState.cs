namespace Microsoft.OData.Serializer;

internal enum ResourceCleanupState : byte
{
    NoCleanupNeeded = 0,
    NeedCleanup,
    Cleanup,
    CleanupComplete
}
