//---------------------------------------------------------------------
// <copyright file="EdmOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an EDM operation.
    /// </summary>
    public abstract class EdmOperation : EdmNamedElement, IEdmOperation, IEdmFullNamedElement
    {
        private readonly string fullName;
        private readonly List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperation"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPathExpression">The entity set path expression.</param>
        protected EdmOperation(string namespaceName, string name, IEdmTypeReference returnType, bool isBound, IEdmPathExpression entitySetPathExpression)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");

            this.Return = returnType == null ? null : new EdmOperationReturn(this, returnType);
            this.Namespace = namespaceName;
            this.IsBound = isBound;
            this.EntitySetPath = entitySetPathExpression;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(namespaceName, this.Name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperation"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        protected EdmOperation(string namespaceName, string name, IEdmTypeReference returnType)
            : this(namespaceName, name, returnType, false, null)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsBound { get; private set; }

        /// <summary>
        /// Gets the entity set path expression.
        /// </summary>
        /// <value>
        /// The entity set path expression.
        /// </value>
        public IEdmPathExpression EntitySetPath { get; private set; }

        /// <summary>
        /// Gets the element kind of this operation, which is always Operation.
        /// virtual will be removed in the near future, stop gap to enable testing for now.
        /// </summary>
        public abstract EdmSchemaElementKind SchemaElementKind { get; }

        /// <summary>
        /// Gets the namespace of this operation.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Gets the return type of this operation.
        /// </summary>
        public IEdmTypeReference ReturnType
        {
            get { return Return == null ? null : Return.Type; }
        }

        /// <summary>
        /// Gets the parameters of this operation.
        /// </summary>
        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Gets the return of this operation.
        /// </summary>
        internal IEdmOperationReturn Return { get; private set; }

        /// <summary>
        /// Searches for a parameter with the given name in this operation and returns null if no such parameter exists.
        /// </summary>
        /// <param name="name">The name of the parameter to be found.</param>
        /// <returns>The requested parameter, or null if no such parameter exists.</returns>
        public IEdmOperationParameter FindParameter(string name)
        {
            foreach (IEdmOperationParameter parameter in this.Parameters)
            {
                if (parameter.Name == name)
                {
                    return parameter;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates and adds a parameter to this operation (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <returns>Created parameter.</returns>
        public EdmOperationParameter AddParameter(string name, IEdmTypeReference type)
        {
            EdmOperationParameter parameter = new EdmOperationParameter(this, name, type);
            this.parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Creates and adds an optional parameter to this operation (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <returns>Created parameter.</returns>
        public EdmOptionalParameter AddOptionalParameter(string name, IEdmTypeReference type)
        {
            return AddOptionalParameter(name, type, null);
        }

        /// <summary>
        /// Creates and adds an optional parameter to this operation (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <param name="defaultValue">The default value for the parameter being added.</param>
        /// <returns>Created parameter.</returns>
        public EdmOptionalParameter AddOptionalParameter(string name, IEdmTypeReference type, string defaultValue)
        {
            EdmOptionalParameter parameter = new EdmOptionalParameter(this, name, type, defaultValue);
            this.parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds a parameter to this operation (as the last parameter).
        /// </summary>
        /// <param name="parameter">The parameter being added.</param>
        public void AddParameter(IEdmOperationParameter parameter)
        {
            EdmUtil.CheckArgumentNull(parameter, "parameter");

            this.parameters.Add(parameter);
        }
    }
}
