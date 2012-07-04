//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.Metadata
{
    /// <summary>
    /// Use this type to describe the kind of results returned by a service
    /// operation.
    /// </summary>
    public enum ODataServiceOperationResultKind
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
