//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Expressions;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;
    using Microsoft.Data.Edm.Library.Expressions;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmFunctionImport"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmFunctionImport : EdmElement, IEdmFunctionImport
    {
        /// <summary>Default value for the IsBindable property.</summary>
        private const bool DefaultIsBindable = false;

        /// <summary>Default value of the IsComposable property.</summary>
        private const bool DefaultIsComposable = false;

        /// <summary>Default value for the IsSideEffecting property.</summary>
        private const bool DefaultIsSideEffecting = true;

        /// <summary>The model this instance belongs to.</summary>
        private readonly MetadataProviderEdmModel model;

        /// <summary>The container this instance belongs to.</summary>
        private readonly MetadataProviderEdmEntityContainer container;

        /// <summary>
        /// The resource operation underlying this function import.
        /// </summary>
        private readonly OperationWrapper operation;

        /// <summary>
        /// The parameters of the service operation.
        /// </summary>
        private readonly ReadOnlyCollection<IEdmFunctionParameter> parameters;

        /// <summary>
        /// Value indicating whether this function import has side-effects;
        /// cannot be true if <see cref="isComposable"/> is true.
        /// </summary>
        private readonly bool isSideEffecting;

        /// <summary>
        /// Value indicating whether this functon import can be composed inside expressions.
        /// <see cref="isComposable"/> cannot be true if <see cref="isSideEffecting"/> is true.
        /// </summary>
        private readonly bool isComposable;

        /// <summary>
        /// Value indicating whether this function import can be used as 
        /// an extension method for the type of the first parameter of this function import.
        /// </summary>
        private readonly bool isBindable;

        /// <summary>
        /// Gets the entity set path of the function import.
        /// </summary>
        private readonly string entitySetPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="container">The container this instance belongs to.</param>
        /// <param name="operation">The resource operation underlying this function import.</param>
        /// <remarks>This constructor assumes that the entity set for this service operation has already be created.</remarks>
        internal MetadataProviderEdmFunctionImport(MetadataProviderEdmModel model, MetadataProviderEdmEntityContainer container, OperationWrapper operation)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(container != null, "container != null");
            Debug.Assert(operation != null, "operation != null");

            this.container = container;
            this.model = model;
            this.operation = operation;

            if (operation.Kind == OperationKind.Action)
            {
                this.isSideEffecting = true;
                this.isComposable = DefaultIsComposable;
                this.isBindable = this.operation.BindingParameter != null;
            }
            else
            {
                Debug.Assert(operation.Kind == OperationKind.ServiceOperation, "serviceOperation.Kind == OperationKind.ServiceOperation");
                Debug.Assert(operation.OperationParameterBindingKind == OperationParameterBindingKind.Never, "operation.OperationParameterBindingKind == OperationParameterBindingKind.Never");
                this.isComposable = DefaultIsComposable;
                this.isSideEffecting = DefaultIsSideEffecting;
                this.isBindable = DefaultIsBindable;
            }

            // EntitySetPath=<path string>
            ResourceSetPathExpression resultSetPathExpression = operation.ResultSetPathExpression;
            this.entitySetPath = resultSetPathExpression == null ? null : resultSetPathExpression.PathExpression;

#if DEBUG
            ResourceType returnType = operation.ReturnType;
            ResourceSetWrapper resultSet = operation.ResourceSet;

            Debug.Assert(
                returnType == null || returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection || returnType.ResourceTypeKind == ResourceTypeKind.EntityType || (resultSet == null && resultSetPathExpression == null),
                "resultSet and resultSetPathExpression must be both null when the return type is not an entity type or an entity collection type.");
            Debug.Assert(
                (returnType == null || returnType.ResourceTypeKind != ResourceTypeKind.EntityCollection && returnType.ResourceTypeKind != ResourceTypeKind.EntityType) || (resultSet != null || resultSetPathExpression != null),
                "One of resultSet or resultSetPathExpression must be set when the return type is either an entity type or an entity collection type.");
            Debug.Assert(resultSet == null || resultSetPathExpression == null, "resultSet and resultSetPathExpression cannot be both set.");
#endif

            if (operation.Kind == OperationKind.ServiceOperation)
            {
                // m:HttpMethod="GET"|"POST"
                model.SetHttpMethod(this, operation.Method);
            }

            string mimeType = operation.MimeType;
            if (!string.IsNullOrEmpty(mimeType))
            {
                model.SetMimeType(this, mimeType);
            }

            switch (operation.OperationParameterBindingKind)
            {
                case OperationParameterBindingKind.Always:
                    model.SetIsAlwaysBindable(this, true);
                    break;

                default:
                    Debug.Assert(
                        operation.OperationParameterBindingKind == OperationParameterBindingKind.Sometimes || 
                        operation.OperationParameterBindingKind == OperationParameterBindingKind.Never,
                        "operation.OperationParameterBindingKind == OperationParameterBindingKind.Sometimes || operation.OperationParameterBindingKind == OperationParameterBindingKind.Never");
                    break;
            }

            ReadOnlyCollection<OperationParameter> operationParameters = operation.Parameters;
            if (operationParameters != null && operationParameters.Count > 0)
            {
                List<IEdmFunctionParameter> list = new List<IEdmFunctionParameter>(operationParameters.Count);
                foreach (OperationParameter parameter in operationParameters)
                {
                    IEdmTypeReference parameterType = this.model.EnsureTypeReference(parameter.ParameterType, /*annotations*/ null);

                    if (!parameterType.IsNullable && this.model.GetEdmVersion() < DataServiceProtocolVersion.V3.ToVersion())
                    {
                        parameterType = parameterType.Clone(/*nullable*/ true);
                    }

                    EdmFunctionParameter edmParameter = new EdmFunctionParameter(this, parameter.Name, parameterType, EdmFunctionParameterMode.In);
                    list.Add(edmParameter);
                }

                this.parameters = new ReadOnlyCollection<IEdmFunctionParameter>(list);
            }

            this.ReturnType = this.CreateReturnTypeReference();
        }

        /// <summary>
        /// Gets a value indicating whether this function import has side-effects.
        /// <see cref="IsSideEffecting"/> cannot be set to true if <see cref="IsComposable"/> is set to true.
        /// </summary>
        public bool IsSideEffecting
        {
            get { return this.isSideEffecting; }
        }

        /// <summary>
        /// Gets a value indicating whether this functon import can be composed inside expressions.
        /// <see cref="IsComposable"/> cannot be set to true if <see cref="IsSideEffecting"/> is set to true.
        /// </summary>
        public bool IsComposable
        {
            get { return this.isComposable; }
        }

        /// <summary>
        /// Gets whether value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        public bool IsBindable
        {
            get { return this.isBindable; }
        }

        /// <summary>
        /// The entity set underlying the result of the function import or null
        /// if no such entity set exists.
        /// </summary>
        /// <remarks>The property assumes that the entity set has already been created and cached by the model so we can look it up here.</remarks>
        public IEdmExpression EntitySet
        {
            get
            {
                ResourceSetWrapper resourceSet = this.operation.ResourceSet;
                if (resourceSet != null)
                {
                    return new EdmEntitySetReferenceExpression(this.model.EnsureEntitySet(resourceSet));
                }
               
                if (this.entitySetPath != null)
                {
                    // Construct the entity set path expression holding this.entitySetPath.
                    // Note that no name resolution is happening at this point. The constructed expression will provide the path value for serialization
                    // and it should also work with IEdmFunctionImport.TryGetRelativeEntitySetPath extension method.
                    // However, because the entitySetPathExpression.Referenced holds a stub named element some client code might get confused as it can try
                    // casting it to IEdmNavigationProperty.
                    string[] path = this.entitySetPath.Split(ResourceSetPathExpression.PathSeparator);
                    return new EdmPathExpression(path);
                }
                
                return null;
            }
        }

        /// <summary>
        /// An enumerable of all parameters of the function import.
        /// </summary>
        public IEnumerable<IEdmFunctionParameter> Parameters
        {
            get { return this.parameters ?? Enumerable.Empty<IEdmFunctionParameter>(); }
        }

        /// <summary>
        /// The <see cref="IEdmTypeReference"/> of the return type of the service
        /// operation or null if no return type exists.
        /// </summary>
        public IEdmTypeReference ReturnType
        {
            get; private set;
        }

        /// <summary>
        /// The name of the function import.
        /// </summary>
        public string Name
        {
            get { return this.operation.Name; }
        }

        /// <summary>
        /// The container element kind; EdmContainerElementKind.FunctionImport for function imports.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }

        /// <summary>
        /// Gets the container of this function import.
        /// </summary>
        public IEdmEntityContainer Container
        {
            // All function imports are in the default container.
            get { return this.container; }
        }

        /// <summary>
        /// The resource service operation underlying this function import.
        /// </summary>
        internal OperationWrapper ServiceOperation
        {
            get { return this.operation; }
        }

        /// <summary>
        /// Method to find a parameter of the function import by name.
        /// </summary>
        /// <param name="name">The name of the parameter to find.</param>
        /// <returns>An <see cref="IEdmFunctionParameter"/> with the given name or null if no such parameter is found.</returns>
        public IEdmFunctionParameter FindParameter(string name)
        {
            if (this.parameters == null)
            {
                return null;
            }

            return this.parameters.FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Creates an edm type reference for the return type of this operation.
        /// </summary>
        /// <returns>The return type reference.</returns>
        private IEdmTypeReference CreateReturnTypeReference()
        {
            ResourceType returnType = this.operation.ReturnType;
            if (returnType == null)
            {
                return null;
            }

            if (returnType.ResourceTypeKind == ResourceTypeKind.Collection || returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection)
            {
                ResourceType itemResourceType;
                if (returnType.ResourceTypeKind == ResourceTypeKind.Collection)
                {
                    itemResourceType = ((CollectionResourceType)returnType).ItemType;
                }
                else
                {
                    itemResourceType = ((EntityCollectionResourceType)returnType).ItemType;
                }

                return this.model.EnsureEntityPrimitiveOrComplexCollectionTypeReference(itemResourceType, returnType, /*annotations*/ null);
            }

            return this.model.EnsureTypeReference(returnType, /*annotations*/ null);
        }
    }
}
