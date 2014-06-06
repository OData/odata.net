//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Use this type to describe the kind of results returned by a service
    /// operation.
    /// </summary>
    public enum ServiceOperationResultKind
    {
        /// <summary>
        /// A single direct value which cannot be further composed.
        /// </summary>
        DirectValue,

        /// <summary>
        /// An enumeration of values which cannot be further composed.
        /// </summary>
        Enumeration,

        /// <summary>
        /// A queryable object which returns multiple elements.
        /// </summary>
        QueryWithMultipleResults,

        /// <summary>
        /// A queryable object which returns a single element.
        /// </summary>
        QueryWithSingleResult,

        /// <summary>
        /// No result return.
        /// </summary>
        Void,
    }
}
