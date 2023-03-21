//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValidationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.Telemetry
{
    /// <summary>
    /// Telemetry provider.
    /// </summary>
    public static class TelemetryProvider
    {
        public static readonly ActivitySource ActivitySourceODataCore = new ActivitySource("Microsoft.OData.Core");
        public static readonly ActivitySource ActivitySourceEdmLib = new ActivitySource("Microsoft.OData.Edm");
    }
}
#endif
