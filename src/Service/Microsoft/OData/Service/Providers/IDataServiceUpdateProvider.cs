//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
