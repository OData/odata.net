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

namespace System.Data.Services
{
    /// <summary>Describes an action performed on a resource.</summary>
    /// <remarks>
    /// This enumeration has been patterned after the DataRowAction
    /// (http://msdn2.microsoft.com/en-us/library/system.data.datarowaction.aspx)
    /// enumeration (with a few less values).
    /// </remarks>
    [System.Flags]
    public enum UpdateOperations
    {
        /// <summary>The resource has not changed.</summary>
        None = 0x00,

        /// <summary>The resource has been added to a container.</summary>
        Add = 0x01,

        /// <summary>The resource has changed.</summary>
        Change = 0x02,

        /// <summary>The resource has been deleted from a container.</summary>
        Delete = 0x04,
    }
}
