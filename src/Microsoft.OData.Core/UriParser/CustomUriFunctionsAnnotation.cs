//---------------------------------------------------------------------
// <copyright file="CustomUriFunctionsAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Concurrent;
    using Microsoft.OData.UriParser;

    #endregion Namespaces

    /// <summary>
    /// Represents an annotation for custom URI functions.
    /// </summary>
    internal sealed class CustomUriFunctionsAnnotation
    {
        /// <summary>
        /// Container for custom URI functions and their signatures with return types.
        /// </summary>
        public ConcurrentDictionary<string, FunctionSignatureWithReturnType[]> CustomUriFunctions { get; }
            = new ConcurrentDictionary<string, FunctionSignatureWithReturnType[]>(StringComparer.Ordinal);
    }
}
