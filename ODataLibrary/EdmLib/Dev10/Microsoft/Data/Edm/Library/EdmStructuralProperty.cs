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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM structural (i.e. non-navigation) property.
    /// </summary>
    public class EdmStructuralProperty : EdmProperty, IEdmStructuralProperty
    {
        private readonly string defaultValue;
        private readonly EdmConcurrencyMode concurrencyMode;

        /// <summary>
        /// Initializes a new instance of the EdmStructuralProperty class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">The default value of this property.</param>
        /// <param name="concurrencyMode">The concurrency mode of this property.</param>
        public EdmStructuralProperty(
            IEdmStructuredType declaringType,
            string name,
            IEdmTypeReference type,
            string defaultValue,
            EdmConcurrencyMode concurrencyMode)
            : base(declaringType, name, type)
        {
            this.defaultValue = defaultValue;
            this.concurrencyMode = concurrencyMode;
        }

        /// <summary>
        /// Initializes a new instance of the EdmStructuralProperty class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        public EdmStructuralProperty(IEdmStructuredType declaringType)
            : base(declaringType)
        {
            this.defaultValue = null;
            this.concurrencyMode = EdmConcurrencyMode.None;
        }

        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        public string DefaultValue
        {
            get { return this.defaultValue; }
        }

        /// <summary>
        /// Gets the concurrency mode of this property.
        /// </summary>
        public EdmConcurrencyMode ConcurrencyMode
        {
            get { return this.concurrencyMode; }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public override EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Structural; }
        }
    }
}
