//---------------------------------------------------------------------
// <copyright file="ODataAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>Represents an OData action.</summary>
    [DebuggerDisplay("{Metadata}")]
    public sealed class ODataAction : ODataOperation
    {
    }
}
