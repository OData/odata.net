//---------------------------------------------------------------------
// <copyright file="TelemetryProvider.cs" company="Microsoft">
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
        private const string VALIDATION_ERROR = "Validation Error";
        private const string TAG_VALIDATION_TYPE = "Validation Type";
        private const string TAG_MESSAGE = "Message";
        private const string TAG_WRITER_VALIDATION = "Writer validation";
        //private const string TAG_READER_VALIDATION = "Reader validation";
        //private const string TAG_URL_VALIDATION = "Url validation";

        public static void AddValidationEvent(string message)
        {
            Activity activity = Activity.Current;

            if (activity == null)
            {
                return;
            }

            ActivityTagsCollection tags = new ActivityTagsCollection();
            tags.Add(TAG_VALIDATION_TYPE, TAG_WRITER_VALIDATION); // TODO: Use enums in validation types.
            tags.Add(TAG_MESSAGE, message);
            ActivityEvent exEvent = new ActivityEvent(VALIDATION_ERROR, default, tags);
            activity.AddEvent(exEvent);
        }
#else
        public static void AddValidationEvent(string message)
        {
            // Do nothing
        }
#endif
    }
}

