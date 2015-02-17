//---------------------------------------------------------------------
// <copyright file="QueryAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;

    /// <summary>
    /// Abstract class to represent query-specific annotations (empty for now)
    /// </summary>
    public abstract class QueryAnnotation : IEquatable<QueryAnnotation>
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The clone</returns>
        public abstract QueryAnnotation Clone();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public abstract bool Equals(QueryAnnotation other);
    }
}
