//---------------------------------------------------------------------
// <copyright file="ODataAnnotatedError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Test OM representing an error payload.
    /// </summary>
    public sealed class ODataAnnotatedError
    {
        /// <summary>The <see cref="ODataError"/> instance.</summary>
        public ODataError Error { get; set; }

        /// <summary>true to include debug information; otherwise false.</summary>
        public bool IncludeDebugInformation { get; set; }
    }
}
