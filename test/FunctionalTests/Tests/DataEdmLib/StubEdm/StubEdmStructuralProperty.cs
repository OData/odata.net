//---------------------------------------------------------------------
// <copyright file="StubEdmStructuralProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Stub implementation of EdmStructuralProperty
    /// </summary>
    public class StubEdmStructuralProperty : StubEdmElement, IEdmStructuralProperty
    {
        /// <summary>
        /// Initializes a new instance of the StubEdmStructuralProperty class.
        /// </summary>
        /// <param name="name">the name of the property</param>
        public StubEdmStructuralProperty(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public string DefaultValueString { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public IEdmTypeReference Type { get; set; }

        /// <summary>
        /// Gets or sets the declaring type
        /// </summary>
        public IEdmStructuredType DeclaringType { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the property kind
        /// </summary>
        public EdmPropertyKind PropertyKind 
        { 
            get { return EdmPropertyKind.Structural; } 
        }
    }
}
