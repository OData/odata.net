//---------------------------------------------------------------------
// <copyright file="EdmStructuralPropertyAlias.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM structural (i.e. non-navigation) property with an alias.
    /// </summary>
    public class EdmStructuralPropertyAlias : EdmProperty, IEdmStructuralPropertyAlias
    {
        private string propertyAlias;
        private IEnumerable<string> path;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuralPropertyAlias"/> class.
        /// </summary>
        /// <param name="declaringType">The type that owns this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="alias">The property alias.</param>
        /// <param name="path">The path to the referenced property.</param>
        public EdmStructuralPropertyAlias(IEdmStructuredType declaringType, string name, IEdmTypeReference type, string propertyAlias, IEnumerable<string> path)
            : base(declaringType, name, type)
        {
            if (propertyAlias == null)
            {
                throw new ArgumentNullException(nameof(propertyAlias));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.propertyAlias = propertyAlias;
            this.path = path;
        }

        /// <inheritdoc/>
        public string PropertyAlias
        {
            get { return this.propertyAlias; }

        }

        /// <inheritdoc/>
        public IEnumerable<string> Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "defaultValue might be confused for an IEdmValue.")]
        public string DefaultValueString
        {
            get;
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
