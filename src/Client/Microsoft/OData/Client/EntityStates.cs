//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Describes the insert/update/delete state of an entity or link.
    /// </summary>
    /// <remarks>
    /// Deleting an inserted resource will detach it.
    /// After SaveChanges, deleted resources will become detached and Added &amp; Modified resources will become unchanged.
    /// </remarks>
    [System.Flags()] 
    public enum EntityStates
    {
        /// <summary>
        /// The resource is not tracked by the context.
        /// </summary>
        Detached = 0x00000001,

        /// <summary>
        /// The resource is tracked by a context with no changes.
        /// </summary>
        Unchanged = 0x00000002,

        /// <summary>
        /// The resource is tracked by a context for insert.
        /// </summary>
        Added = 0x00000004,

        /// <summary>
        /// The resource is tracked by a context for deletion.
        /// </summary>
        Deleted = 0x00000008,

        /// <summary>
        /// The resource is tracked by a context for update.
        /// </summary>
        Modified = 0x00000010
    }
}
