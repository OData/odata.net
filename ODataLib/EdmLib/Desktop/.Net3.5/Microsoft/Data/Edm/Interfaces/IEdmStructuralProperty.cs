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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Enumerates the EDM property concurrency modes.
    /// </summary>
    public enum EdmConcurrencyMode
    {
        /// <summary>
        /// Denotes a property that should be used for optimistic concurrency checks.
        /// </summary>
        None,

        /// <summary>
        /// Denotes a property that should not be used for optimistic concurrency checks.
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
