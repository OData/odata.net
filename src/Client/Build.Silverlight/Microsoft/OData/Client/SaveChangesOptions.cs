//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    /// options when saving changes
    /// </summary>
    [Flags]
    public enum SaveChangesOptions
    {
        /// <summary>default option, using multiple requests to the server stopping on the first failure</summary>
        None = 0,

        /// <summary>save the changes in a single changeset in a batch request.</summary>
        BatchWithSingleChangeset = 1,

        /// <summary>save all the changes using multiple requests</summary>
        ContinueOnError = 2,

        /// <summary>Use replace semantics when doing update.</summary>
        ReplaceOnUpdate = 4,

        /// <summary>save each change independently in a batch request.</summary>
        BatchWithIndependentOperations = 16,

        /// <summary>
        /// Use partial payload when doing post.
        /// Note it can only be used when using <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" />
        /// </summary>
        PostOnlySetProperties = 8
    }
}
