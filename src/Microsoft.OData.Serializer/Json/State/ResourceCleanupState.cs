namespace Microsoft.OData.Serializer.Json.State;

internal enum ResourceCleanupState : byte
{
    NoCleanupNeeded = 0,
    NeedCleanup,
    Cleanup,
    CleanupComplete
}
