//---------------------------------------------------------------------
// <copyright file="StubEdmOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib.StubEdm
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Stub implementation of EdmOperation
    /// </summary>
    public class StubEdmOperation : StubEdmElement, IEdmOperation
    {
        private List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>();

        /// <summary>
        /// Initializes a new instance of the StubEdmOperation class.
        /// </summary>
        /// <param name="namespaceName">The namespace name</param>
        /// <param name="name">the name of the operation</param>
        public StubEdmOperation(string namespaceName, string name)
        {
            this.Namespace = namespaceName;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the return type
        /// </summary>
        public IEdmTypeReference ReturnType { get; set; }

        /// <summary>
        /// Gets the parameters
        /// </summary>
        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.parameters.AsEnumerable(); }
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is bound.
        /// </summary>
        public bool IsBound { get; set; }

        /// <summary>
        /// Gets or sets the entity set path expression.
        /// </summary>
        public IEdmPathExpression EntitySetPath { get; set; }

        /// <summary>
        /// Gets or sets the schema element kind
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind { get; set; }

        /// <summary>
        /// Finds a parameter by name
        /// </summary>
        /// <param name="name">the name of the parameter</param>
        /// <returns>the parameter</returns>
        public IEdmOperationParameter FindParameter(string name)
        {
            return this.Parameters.FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Adds a parameter
        /// </summary>
        /// <param name="parameter">The parameter to add</param>
        public void Add(IEdmOperationParameter parameter)
        {
            if (parameter.DeclaringOperation == null)
            {
                StubEdmOperationParameter stubEdmOperationParameter = parameter as StubEdmOperationParameter;
                if (stubEdmOperationParameter != null)
                {
                    stubEdmOperationParameter.DeclaringOperation = this;
                }
            }
            
            this.parameters.Add(parameter);
        }
    }
}
