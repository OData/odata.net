//---------------------------------------------------------------------
// <copyright file="IDataServiceUpdateProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface declares the methods required for passing
    /// etag values to the provider.
    /// </summary>
    public interface IDataServiceUpdateProvider : IUpdatable
    {
        /// <summary>Supplies the eTag value for the given entity resource.</summary>
        /// <param name="resourceCookie">Cookie that represents the resource.</param>
        /// <param name="checkForEquality">A <see cref="T:System.Boolean" /> that is true when property values must be compared for equality; false when property values must be compared for inequality.</param>
        /// <param name="concurrencyValues">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> list of the eTag property names and corresponding values.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "need to pass in a collection of key value pair")]
        void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues);
    }
}
