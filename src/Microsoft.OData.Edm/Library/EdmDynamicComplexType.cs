//---------------------------------------------------------------------
// <copyright file="DynamicEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a definition of an dynamically generated complex type.
    /// </summary>
    public sealed class EdmDynamicComplexType : EdmStructuredType, IEdmComplexType, IEdmDynamicType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDynamicComplexType"/> class.
        /// </summary>
        /// <param name="name">Name of the entity.</param>
        public EdmDynamicComplexType(string name)
            : base(isAbstract: false, isOpen: true, baseStructuredType: null)
        {
            this.Name = name;
        }

        public string Name
        {
            get; private set;
        }

        public string Namespace
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the schema element kind of this element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }

        /// <summary>
        /// Gets the kind of this term.
        /// </summary>
        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }
    }
}
