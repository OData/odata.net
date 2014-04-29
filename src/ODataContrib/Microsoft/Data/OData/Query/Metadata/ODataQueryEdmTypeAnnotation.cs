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
    #region Namespaces
    using System;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// In-memory annotation class to associate CLR instance types with 
    /// (non-primitive) EDM types.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Camel-casing in class name.")]
    public sealed class ODataQueryEdmTypeAnnotation
    {
        /// <summary>
        /// true if reflection over the instance type is supported; otherwise false.
        /// </summary>
        public bool CanReflectOnInstanceType
        {
            get;
            set;
        }

        /// <summary>
        /// The instance type represented by this annotation.
        /// </summary>
        public Type InstanceType
        {
            get;
            set;
        }
    }
}
