using System;
#if NETCOREAPP3_1_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endif

namespace Microsoft.OData
{
    internal static class Telemetry
    {
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
        private static readonly Assembly Assembly = typeof(Telemetry).Assembly;
        private static readonly AssemblyName AssemblyName = Assembly.GetName();

        public static ActivitySource ActivitySource = new ActivitySource(AssemblyName.Name, AssemblyName.Version.ToString());

        public static void LogException(Exception exception)
        {
            Activity activity = Activity.Current;
            if (activity == null)
            {
                return;
            }

            ActivityTagsCollection tags = new ActivityTagsCollection();
            tags.Add("Message", exception.Message);
            ActivityEvent exEvent = new ActivityEvent("Exception", default, tags);

            activity.AddEvent(exEvent);
        }
#else
        public static void LogException(Exception exception) {}
#endif

        public static void EndCurrentActivity()
        {
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
            Activity.Current?.Stop();
#endif
        }
    }
}
