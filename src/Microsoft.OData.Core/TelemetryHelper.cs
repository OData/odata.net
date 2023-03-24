//---------------------------------------------------------------------
// <copyright file="TelemetryHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
    using Activity = System.Diagnostics.Activity;
    using ActivitySource = System.Diagnostics.ActivitySource;
#else
    using System;
#endif

    internal static class TelemetryHelper
    {
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
        public static readonly ActivitySource ODataTelemetrySource = new ActivitySource("Microsoft.OData.Core");
#endif

        public static Activity StartActivity(string displayName)
        {
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
            return ODataTelemetrySource.StartActivity(displayName);
#else
            return null;
#endif
        }

        public static void StopActivity()
        {
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
            Activity.Current?.Stop();
#endif
        }

        public static void StopAndDisposeActivity()
        {
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
            Activity.Current?.Dispose(); // Stops and disposes activity
#endif
        }
    }

#if !(NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER)
    /// <summary>
    /// Dummy class created to make it possible for the methods in <see cref="TelemetryHelper"/>
    /// class to work seamlessly across frameworks that support open telemetry and those that don't.
    /// </summary>
    internal class Activity : IDisposable
    {
        private Activity() { } // To inhibit class from being initialized

        public Activity AddTag(object tag, object value) => throw new NotImplementedException();

        public Activity AddEvent(object @event) => throw new NotImplementedException();

        public void Dispose() { }
    }
#endif
}
