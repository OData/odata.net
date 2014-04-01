//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Enumerates the EDM property concurrency modes.
    /// </summary>
    public enum EdmConcurrencyMode
    {
        /// <summary>
        /// Denotes a property that should not be used for optimistic concurrency checks.
        /// </summary>
        None = 0,

        /// <summary>
        /// Denotes a property that should be used for optimistic concurrency checks.
        /// </summary>
        Fixed
    }

    /// <summary>
    /// Represents an EDM structural (i.e. non-navigation) property.
    /// </summary>
    public interface IEdmStructuralProperty : IEdmProperty
    {
        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        string DefaultValueString { get; }

        /// <summary>
        /// Gets the concurrency mode of this property.
        /// </summary>
        EdmConcurrencyMode ConcurrencyMode { get; }
    }
}
