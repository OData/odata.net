//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
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
