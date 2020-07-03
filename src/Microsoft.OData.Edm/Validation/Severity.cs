//---------------------------------------------------------------------
// <copyright file="Severity.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// The severity kinds.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Default / Undefined error Level
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Informational Message
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error
        /// </summary>
        Error = 3,
    }
}
