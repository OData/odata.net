//---------------------------------------------------------------------
// <copyright file="EdmFunctionImportFacade.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Client.Metadata
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Wraps a function import from the server-side model.
    /// </summary>
    internal class EdmFunctionImportFacade : IEdmFunctionImport
    {
        /// <summary>
        /// The Edm Container facade this function import belongs to.
        /// </summary>
        private readonly EdmEntityContainerFacade containerFacade;

        /// <summary>
        /// Stores the function import from the server-side model which we are wrapping.
        /// </summary>
        private readonly IEdmFunctionImport serverFunctionImport;

        /// <summary>
        /// The return type of this function.
        /// </summary>
        private readonly IEdmTypeReference returnType;

        /// <summary>
        /// The collection of parameters for this function.
        /// </summary>
        private readonly IEdmFunctionParameter[] parameters;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionImportFacade"/> class.
        /// </summary>
        /// <param name="serverFunctionImport">The function import from the server-side model which we are wrapping.</param>
        /// <param name="containerFacade">The edm container facade this function import belongs to.</param>
        /// <param name="modelFacade">The edm model facade this function import belongs to.</param>
        public EdmFunctionImportFacade(IEdmFunctionImport serverFunctionImport, EdmEntityContainerFacade containerFacade, EdmModelFacade modelFacade)
        {
            Debug.Assert(serverFunctionImport != null, "serverFunctionImport != null");
            Debug.Assert(containerFacade != null, "containerFacade != null");
            Debug.Assert(modelFacade != null, "modelFacade != null");
            this.serverFunctionImport = serverFunctionImport;
            this.containerFacade = containerFacade;

            IEdmTypeReference serverReturnTypeReference = serverFunctionImport.ReturnType;
            if (serverReturnTypeReference == null)
            {
                this.returnType = null;
            }
            else
            {
                IEdmType serverReturnType = modelFacade.GetOrCreateEntityTypeFacadeOrReturnNonEntityServerType(serverReturnTypeReference.Definition);
                this.returnType = serverReturnType.ToEdmTypeReference(serverReturnTypeReference.IsNullable);
            }

            this.parameters = serverFunctionImport.Parameters.Select(p => new EdmFunctionParameterFacade(p, this, modelFacade)).ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether this function import has side-effects.
        /// <see cref="IsSideEffecting"/> cannot be set to true if <see cref="IsComposable"/> is set to true.
        /// </summary>
        public bool IsSideEffecting
        {
            get { return this.serverFunctionImport.IsSideEffecting; }
        }

        /// <summary>
        /// Gets a value indicating whether this functon import can be composed inside expressions.
        /// <see cref="IsComposable"/> cannot be set to true if <see cref="IsSideEffecting"/> is set to true.
        /// </summary>
        public bool IsComposable
        {
            get { return this.serverFunctionImport.IsComposable; }
        }

        /// <summary>
        /// Gets a value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        public bool IsBindable
        {
            get { return this.serverFunctionImport.IsBindable; }
        }

        /// <summary>
        /// Gets the entity set containing entities returned by this function import.
        /// </summary>
        public IEdmExpression EntitySet
        {
            get { throw CreateExceptionForUnsupportedPublicMethod("get_EntitySet"); }
        }

        /// <summary>
        /// Gets the kind of this function, which is always FunctionImport.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }

        /// <summary>
        /// Gets the container of this function.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.containerFacade; }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.serverFunctionImport.Name; }
        }

        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        public IEdmTypeReference ReturnType
        {
            get { return this.returnType; }
        }

        /// <summary>
        /// Gets the collection of parameters for this function.
        /// </summary>
        public IEnumerable<IEdmFunctionParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Searches for a parameter with the given name, and returns null if no such parameter exists.
        /// </summary>
        /// <param name="name">The name of the parameter being found.</param>
        /// <returns>The requested parameter or null if no such parameter exists.</returns>
        public IEdmFunctionParameter FindParameter(string name)
        {
            return this.parameters.SingleOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Retrieves an annotation value for function import from the server model. Returns null if no annotation with the given name exists.
        /// </summary>
        /// <param name="serverManager">The annotation manager from the server model.</param>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Local name of the annotation.</param>
        /// <returns>Returns the annotation value that corresponds to the provided name. Returns null if no annotation with the given name exists. </returns>
        internal object GetAnnotationValue(IEdmDirectValueAnnotationsManager serverManager, string namespaceName, string localName)
        {
            Debug.Assert(serverManager != null, "serverManager != null");
            return serverManager.GetAnnotationValue(this.serverFunctionImport, namespaceName, localName);
        }

        /// <summary>
        /// Creates an exception for a intentionally-unsupported part of the API.
        /// This is used to prevent new code from calling previously-unused API, which could be a breaking change
        /// for user implementations of the interface.
        /// </summary>
        /// <param name="methodName">Name of the unsupported method.</param>
        /// <returns>The exception</returns>
        private static Exception CreateExceptionForUnsupportedPublicMethod(string methodName)
        {
            return new NotSupportedException(Microsoft.OData.Service.Client.Strings.EdmModelFacade_UnsupportedMethod("IEdmFunctionImport", methodName));
        }
    }
}
