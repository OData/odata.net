using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.State;

internal enum ResourceCleanupState : byte
{
    NoCleanupNeeded = 0,
    NeedCleanup,
    Cleanup,
    CleanupComplete
}
