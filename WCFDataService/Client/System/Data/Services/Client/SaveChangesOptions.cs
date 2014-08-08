//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Data.Services.Client
{
    /// <summary>
    /// options when saving changes
    /// </summary>
    [Flags]
    public enum SaveChangesOptions
    {
        /// <summary>default option, using multiple requests to the server stopping on the first failure</summary>
        None = 0,

        /// <summary>save the changes in a single changeset in a batch request.</summary>
        Batch = 1,

        /// <summary>save all the changes using multiple requests</summary>
        ContinueOnError = 2,

        /// <summary>Use replace semantics when doing update.</summary>
        ReplaceOnUpdate = 4,

        /// <summary>Use PATCH verb when doing update (retains the merge semantics).</summary>
        PatchOnUpdate = 8,

        /// <summary>save each change independently in a batch request.</summary>
        BatchWithIndependentOperations = 16,
    }    
}

