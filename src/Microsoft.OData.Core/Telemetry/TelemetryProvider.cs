//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValidationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_0_OR_GREATER
using System.Diagnostics;
#endif
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.Telemetry
{
    /// <summary>
    /// Telemetry provider.
    /// </summary>
    internal static class TelemetryProvider
    {
#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        private static readonly ActivitySource source = new ActivitySource("Microsoft.OData.Core");

        private const string VALIDATION_ERROR = "Validation Error";
        private const string TAG_VALIDATION_TYPE = "Validation Type";
        private const string TAG_MESSAGE = "Message";

        public static void AddValidationEvent(string message)
        {
            using (Activity activity = source.StartActivity(VALIDATION_ERROR))
            {
                activity?.SetTag(TAG_VALIDATION_TYPE, "Writer Validation");
                activity?.SetTag(TAG_MESSAGE, message);
            }
        }
#else
        public static void AddValidationEvent(string message)
        {
            // Do nothing
        }
#endif
    }
}

