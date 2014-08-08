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
    using System;

    /// <summary>
    /// Provides values to describe the kind of thing targetted by a 
    /// client request.
    /// </summary>
    [Flags]
    public enum ServiceOperationRights
    {
        /// <summary>Specifies no rights on this service operation.</summary>
        None = 0,

        /// <summary>Specifies the right to read one resource per request.</summary>
        ReadSingle = 1,

        /// <summary>Specifies the right to read multiple resources per request.</summary>
        ReadMultiple = 2,

        /// <summary>Specifies the right to read single or multiple resources in a single request.</summary>
        AllRead = ReadSingle | ReadMultiple,

        /// <summary>Specifies all rights to the service operation.</summary>
        All = AllRead,

        /// <summary>Actually use ServiceOperationRights, not EntitySetRights for the service operation.</summary>
        OverrideEntitySetRights = 4,
    }
}
