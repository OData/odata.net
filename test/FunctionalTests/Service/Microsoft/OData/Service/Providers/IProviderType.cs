//---------------------------------------------------------------------
// <copyright file="IProviderType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Implemented by a class that encapsulates a data service provider's metadata representation of a type.
    /// </summary>
    internal interface IProviderType
    {
        /// <summary>
        /// Returns the members declared on this type only, not including any inherited members.
        /// </summary>
        IEnumerable<IProviderMember> Members { get; }

        /// <summary>
        /// Name of the type without its namespace
        /// </summary>
        string Name { get; }
    }
}
