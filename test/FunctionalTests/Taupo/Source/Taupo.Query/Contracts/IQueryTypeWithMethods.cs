//---------------------------------------------------------------------
// <copyright file="IQueryTypeWithMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Query type with methods that can be called in a query
    /// </summary>
    public interface IQueryTypeWithMethods
    {
        /// <summary>
        /// Gets the collection of type methods.
        /// </summary>
        IList<Function> Methods { get; }
    }
}
