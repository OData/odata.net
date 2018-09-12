//---------------------------------------------------------------------
// <copyright file="EdmTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM term.
    /// </summary>
    public class EdmTerm : EdmNamedElement, IEdmTerm
    {
        private readonly string namespaceName;
        private readonly IEdmTypeReference type;
        private readonly string appliesTo;
        private readonly string defaultValue;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmTerm"/> class.
        /// The new term will be of the nullable primitive <paramref name="type"/>.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        public EdmTerm(string namespaceName, string name, EdmPrimitiveTypeKind type)
            : this(namespaceName, name, type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmTerm"/> class.
        /// The new term will be of the nullable primitive <paramref name="type"/>.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        /// <param name="appliesTo">AppliesTo of the term.</param>
        public EdmTerm(string namespaceName, string name, EdmPrimitiveTypeKind type, string appliesTo)
            : this(namespaceName, name, EdmCoreModel.Instance.GetPrimitive(type, true), appliesTo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTerm"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        public EdmTerm(string namespaceName, string name, IEdmTypeReference type)
            : this(namespaceName, name, type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTerm"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        /// <param name="appliesTo">AppliesTo of the term.</param>
        public EdmTerm(string namespaceName, string name, IEdmTypeReference type, string appliesTo)
            : this(namespaceName, name, type, appliesTo, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTerm"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        /// <param name="appliesTo">AppliesTo of the term.</param>
        /// <param name="defaultValue">DefaultValue of the term.</param>
        public EdmTerm(string namespaceName, string name, IEdmTypeReference type, string appliesTo, string defaultValue)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(type, "type");

            this.namespaceName = namespaceName;
            this.type = type;
            this.appliesTo = appliesTo;
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the namespace of this term.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the type of this term.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the AppliesTo of this term.
        /// </summary>
        public string AppliesTo
        {
            get { return this.appliesTo; }
        }

        /// <summary>
        /// Gets the DefaultValue of this term.
        /// </summary>
        public string DefaultValue
        {
            get { return this.defaultValue; }
        }

        /// <summary>
        /// Gets the schema element kind of this term.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Term; }
        }
    }
}
