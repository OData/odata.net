//---------------------------------------------------------------------
// <copyright file="StubEdmEnumType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Stub implementation of the EdmEnumType
    /// </summary>
    public class StubEdmEnumType : StubEdmElement, IEdmEnumType
    {
        private List<IEdmEnumMember> members = new List<IEdmEnumMember>();

        /// <summary>
        /// Initializes a new instance of the StubEdmEnumType class.
        /// </summary>
        /// <param name="namespaceName">the namepace name</param>
        /// <param name="name">the name of the enum type</param>
        public StubEdmEnumType(string namespaceName, string name)
        {
            this.Namespace = namespaceName;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it should be treated as bits
        /// </summary>
        public bool IsFlags { get; set; }

        /// <summary>
        /// Gets or sets the underlying type
        /// </summary>
        public IEdmPrimitiveType UnderlyingType { get; set; }

        /// <summary>
        /// Gets the members
        /// </summary>
        public IEnumerable<IEdmEnumMember> Members
        {
            get { return this.members; }
        }

        /// <summary>
        /// Gets the schema element kind
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the type kind
        /// </summary>
        public EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Enum; }
        }

        /// <summary>
        /// Adds a enum member
        /// </summary>
        /// <param name="member">the specified enum member</param>
        public void Add(IEdmEnumMember member)
        {
            this.members.Add(member);
            ((StubEdmEnumMember)member).DeclaringType = this;
        }
    }
}
