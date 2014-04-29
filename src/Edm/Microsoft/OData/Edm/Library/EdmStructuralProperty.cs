//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM structural (i.e. non-navigation) property.
    /// </summary>
    public class EdmStructuralProperty : EdmProperty, IEdmStructuralProperty
    {
        private readonly string defaultValueString;
        private readonly EdmConcurrencyMode concurrencyMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuralProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">The type of the property.</param>
        public EdmStructuralProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
            : this(declaringType, name, type, null,  EdmConcurrencyMode.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuralProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="defaultValueString">The default value of this property.</param>
        /// <param name="concurrencyMode">The concurrency mode of this property.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "defaultValue might be confused for an IEdmValue.")]
        public EdmStructuralProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type, string defaultValueString, EdmConcurrencyMode concurrencyMode)
            : base(declaringType, name, type)
        {
            this.defaultValueString = defaultValueString;
            this.concurrencyMode = concurrencyMode;
        }

        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "defaultValue might be confused for an IEdmValue.")]
        public string DefaultValueString
        {
            get { return this.defaultValueString; }
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
