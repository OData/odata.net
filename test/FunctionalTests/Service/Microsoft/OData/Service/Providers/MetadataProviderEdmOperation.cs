//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmOperation"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal abstract class MetadataProviderEdmOperation : EdmElement, IEdmOperation
    {
        /// <summary>Default value for the IsBindable property.</summary>
        private const bool DefaultIsBindable = false;

        /// <summary>The model this instance belongs to.</summary>
        private readonly MetadataProviderEdmModel model;

        /// <summary>
        /// The parameters of the service operation.
        /// </summary>
        private readonly ReadOnlyCollection<IEdmOperationParameter> parameters;

        /// <summary>
        /// Value indicating whether this function import can be used as 
        /// an extension method for the type of the first parameter of this function import.
        /// </summary>
        private readonly bool isBound;

        /// <summary>
        /// Gets the entity set path of the function import.
        /// </summary>
        private readonly string entitySetPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="operation">The resource operation underlying this function import.</param>
        /// <param name="namespaceName">The namespace of the operation.</param>
        /// <remarks>This constructor assumes that the entity set for this service operation has already be created.</remarks>
        protected internal MetadataProviderEdmOperation(MetadataProviderEdmModel model, OperationWrapper operation, string namespaceName)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(operation != null, "operation != null");

            this.model = model;
            this.ServiceOperation = operation;
            this.Namespace = namespaceName;

            if (operation.Kind == OperationKind.Action)
            {
                this.isBound = this.ServiceOperation.BindingParameter != null;
            }
            else
            {
                Debug.Assert(operation.Kind == OperationKind.ServiceOperation, "serviceOperation.Kind == OperationKind.ServiceOperation");
                Debug.Assert(operation.OperationParameterBindingKind == OperationParameterBindingKind.Never, "operation.OperationParameterBindingKind == OperationParameterBindingKind.Never");
                this.isBound = DefaultIsBindable;
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

            string mimeType = operation.MimeType;
            if (!string.IsNullOrEmpty(mimeType))
            {
                model.SetMimeType(this, mimeType);
            }

            switch (this.ServiceOperation.OperationParameterBindingKind)
            {
                case OperationParameterBindingKind.Always:
                    break;

                default:
                    Debug.Assert(
                        this.ServiceOperation.OperationParameterBindingKind == OperationParameterBindingKind.Sometimes ||
                        this.ServiceOperation.OperationParameterBindingKind == OperationParameterBindingKind.Never,
                        "this.ServiceOperation.OperationParameterBindingKind == OperationParameterBindingKind.Sometimes || this.ServiceOperation.OperationParameterBindingKind == OperationParameterBindingKind.Never");
                    break;
            }

            ReadOnlyCollection<OperationParameter> operationParameters = operation.Parameters;
            if (operationParameters != null && operationParameters.Count > 0)
            {
                List<IEdmOperationParameter> list = new List<IEdmOperationParameter>(operationParameters.Count);
                foreach (OperationParameter parameter in operationParameters)
                {
                    IEdmTypeReference parameterType = this.model.EnsureTypeReference(parameter.ParameterType, /*annotations*/ null);
                    EdmOperationParameter edmParameter = new EdmOperationParameter(this, parameter.Name, parameterType);
                    list.Add(edmParameter);
                }

                this.parameters = new ReadOnlyCollection<IEdmOperationParameter>(list);
            }

            this.ReturnType = this.CreateReturnTypeReference();
        }

        /// <summary>
        /// Gets whether value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        public bool IsBound
        {
            get { return this.isBound; }
        }

        /// <summary>
        /// Gets the entity set path expression.
        /// </summary>
        /// <value>
        /// The entity set path expression.
        /// </value>
        public IEdmPathExpression EntitySetPath
        {
            get
            {             
                if (this.entitySetPath != null)
                {
                    // Construct the entity set path expression holding this.entitySetPath.
                    // Note that no name resolution is happening at this point. The constructed expression will provide the path value for serialization
                    // and it should also work with IEdmOperationImport.TryGetRelativeEntitySetPath extension method.
                    // However, because the entitySetPathExpression.Referenced holds a stub named element some client code might get confused as it can try
                    // casting it to IEdmNavigationProperty.
                    string[] path = this.entitySetPath.Split(ResourceSetPathExpression.PathSeparator);
                    return new EdmPathExpression(path);
                }
                
                return null;
            }
        }

        /// <summary>
        /// An enumerable of all parameters of the operation import.
        /// </summary>
        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.parameters ?? Enumerable.Empty<IEdmOperationParameter>(); }
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
            get { return this.ServiceOperation.Name; }
        }

        /// <summary>
        /// Gets the namespace this schema element belongs to.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public abstract EdmSchemaElementKind SchemaElementKind { get; }

        /// <summary>
        /// The resource service operation underlying this function import.
        /// </summary>
        internal OperationWrapper ServiceOperation { get; private set; }

        /// <summary>
        /// Method to find a parameter of the function import by name.
        /// </summary>
        /// <param name="name">The name of the parameter to find.</param>
        /// <returns>An <see cref="IEdmOperationParameter"/> with the given name or null if no such parameter is found.</returns>
        public IEdmOperationParameter FindParameter(string name)
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
            ResourceType returnType = this.ServiceOperation.ReturnType;
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
