//---------------------------------------------------------------------
// <copyright file="Operation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>Use this class to represent a custom service operation.</summary>
    [DebuggerVisualizer("Operation={Name}")]
    public abstract class Operation
    {
        /// <summary>The binding parameter to this function/action; null if this function/action is top-level or this operation is not a function or action.</summary>
        private readonly OperationParameter bindingParameter;

        /// <summary>The kind of the operation parameter binding - (Never, Sometimes, Always)</summary>
        private readonly OperationParameterBindingKind operationParameterBindingKind;

        /// <summary>Return type of the operation.</summary>
        private readonly ResourceType returnType;

        /// <summary>Protocol (for example HTTP) method the service operation responds to.</summary>
        private readonly string method;

        /// <summary>In-order parameters for this operation.</summary>
        private readonly ReadOnlyCollection<OperationParameter> operationParameters;

        /// <summary>Kind of result expected from this operation.</summary>
        private readonly ServiceOperationResultKind resultKind;

        /// <summary>name of the service operation.</summary>
        private readonly string name;

        /// <summary>Entity set from which entities are read, if applicable.</summary>
        private readonly ResourceSet resourceSet;

        /// <summary>Path expression to calculate the result resource set of the function if the function returns an entity or a collection of entity.</summary>
        private readonly ResourceSetPathExpression resultSetPathExpression;

        /// <summary>The kind of the current service operation.</summary>
        private readonly OperationKind kind;

        /// <summary>MIME type specified on primitive results, possibly null.</summary>
        private string mimeType;

        /// <summary>Is true, if the service operation is set to readonly i.e. fully initialized and validated. No more changes can be made,
        /// after the service operation is set to readonly.</summary>
        private bool isReadOnly;

        /// <summary>
        /// Initializes a new <see cref="Operation"/> instance.
        /// </summary>
        /// <param name="name">name of the operation.</param>
        /// <param name="resultKind">Kind of result expected from this operation.</param>
        /// <param name="returnType">Return type of the operation.</param>
        /// <param name="resultSet">EntitySet of the result expected from this operation, must be null if <paramref name="resultSetPathExpression"/> is not null.</param>
        /// <param name="resultSetPathExpression">Path expression to calculate the result set of the operation, must be null if <paramref name="resultSet"/> is not null.</param>
        /// <param name="method">Protocol (for example HTTP) method the service operation responds to.</param>
        /// <param name="parameters">In-order parameters for this operation.</param>
        /// <param name="operationParameterBindingKind">the kind of the operation parameter binding (Never, Sometimes, Always).</param>
        /// <param name="kind">The kind of the current service operation.</param>
        internal Operation(
            string name,
            ServiceOperationResultKind resultKind,
            ResourceType returnType,
            ResourceSet resultSet,
            ResourceSetPathExpression resultSetPathExpression,
            string method,
            IEnumerable<OperationParameter> parameters,
            OperationParameterBindingKind operationParameterBindingKind,
            OperationKind kind)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            WebUtil.CheckServiceOperationResultKind(resultKind, "resultKind");
            WebUtil.CheckStringArgumentNullOrEmpty(method, "method");

            Debug.Assert(
                this.GetType() == typeof(ServiceOperation) && kind == OperationKind.ServiceOperation ||
                this.GetType() == typeof(ServiceAction) && kind == OperationKind.Action,
                "OperationKind and the current type doesn't match.");

            ValidateConstructorArguments(name, returnType, resultSet, resultSetPathExpression, method, operationParameterBindingKind, kind);
            this.name = name;
            this.resultKind = resultKind;
            this.returnType = returnType;
            this.resourceSet = resultSet;
            this.resultSetPathExpression = resultSetPathExpression;
            this.method = method;
            this.kind = kind;
            this.operationParameterBindingKind = operationParameterBindingKind;
            this.operationParameters = Operation.ValidateParameters(this.operationParameterBindingKind, parameters);

            if (this.operationParameterBindingKind != OperationParameterBindingKind.Never)
            {
                Debug.Assert(
                    this.operationParameterBindingKind == OperationParameterBindingKind.Always || this.operationParameterBindingKind == OperationParameterBindingKind.Sometimes,
                    "Value of operationParameterBindingKind was expected to be 'always' or 'sometimes'.");

                Debug.Assert(this.kind != OperationKind.ServiceOperation, "ServiceOperations should never be bindable.");
                this.bindingParameter = this.operationParameters.FirstOrDefault();
                if (this.bindingParameter == null)
                {
                    throw new ArgumentException(Strings.ServiceOperation_BindableOperationMustHaveAtLeastOneParameter, "operationParameterBindingKind");
                }

                if (resourceSet != null)
                {
                    throw new ArgumentException(Strings.Opereration_BoundOperationsMustNotSpecifyEntitySetOnlyEntitySetPath(this.name), "resourceSet");
                }

                if (resultSetPathExpression != null &&
                    this.bindingParameter.ParameterType.ResourceTypeKind != ResourceTypeKind.EntityType &&
                    this.bindingParameter.ParameterType.ResourceTypeKind != ResourceTypeKind.EntityCollection)
                {
                    throw new ArgumentException(Strings.ServiceOperation_BindingParameterMustBeEntityToUsePathExpression("resultSetPathExpression"));
                }

                if (this.kind == OperationKind.Action &&
                    !(this.bindingParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType ||
                    this.bindingParameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection))
                {
                    throw new ArgumentException(Strings.ServiceOperation_ActionBindingMustBeEntityOrEntityCollection, "parameters");
                }

                if (this.resultSetPathExpression != null)
                {
                    this.resultSetPathExpression.SetBindingParameter(this.bindingParameter);
                }
            }

            Debug.Assert(this.kind != OperationKind.Action || string.CompareOrdinal(XmlConstants.HttpMethodPost, this.method) == 0, "HttpMethod must be POST for Actions.");
            Debug.Assert(this.resourceSet == null || this.resultSetPathExpression == null, "'resultSet' and 'resultSetPathExpression' cannot be both set by the constructor.");
        }

        /// <summary>Protocol (for example HTTP) method the service operation responds to.</summary>
        public string Method
        {
            get { return this.method; }
        }

        /// <summary>MIME type specified on primitive results, possibly null.</summary>
        public string MimeType
        {
            get
            {
                return this.mimeType;
            }

            set
            {
                this.ThrowIfSealed();
                if (String.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException(Strings.ServiceOperation_MimeTypeCannotBeEmpty(this.Name));
                }

                if (!WebUtil.IsValidMimeType(value))
                {
                    throw new InvalidOperationException(Strings.ServiceOperation_MimeTypeNotValid(value, this.Name));
                }

                this.mimeType = value;
            }
        }

        /// <summary>Name of the service operation.</summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary> PlaceHolder to hold custom state information about service operation. </summary>
        public object CustomState
        {
            get;
            set;
        }

        /// <summary> Returns true, if this service operation has been set to read only. Otherwise returns false. </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary>Entity set from which entities are read (possibly null).</summary>
        public ResourceSet ResourceSet
        {
            get { return this.resourceSet; }
        }

        /// <summary>Returns all the parameters for the given service operations.</summary>
        internal ReadOnlyCollection<OperationParameter> OperationParameters
        {
            get { return this.operationParameters; }
        }

        /// <summary>Kind of result expected from this operation.</summary>
        internal ServiceOperationResultKind OperationResultKind
        {
            get { return this.resultKind; }
        }

        /// <summary>Element type of the operation result. If the return type is a collection type,
        /// this is the item type of the return type; otherwise this is the same as return type.</summary>
        /// <example>
        /// For example if the operation returns an IEnumerable&lt;string&gt;, the return type would be of type
        /// Collection(Edm.String) and this property would be of type Edm.String.
        /// </example>
        internal ResourceType OperationResultType
        {
            get
            {
                EntityCollectionResourceType ct = this.returnType as EntityCollectionResourceType;
                if (ct != null)
                {
                    return ct.ItemType;
                }

                CollectionResourceType mvt = this.returnType as CollectionResourceType;
                if (mvt != null)
                {
                    return mvt.ItemType;
                }

                Debug.Assert(
                    this.returnType == null ||
                    this.returnType.ResourceTypeKind == ResourceTypeKind.EntityType ||
                    this.returnType.ResourceTypeKind == ResourceTypeKind.ComplexType ||
                    this.returnType.ResourceTypeKind == ResourceTypeKind.Primitive,
                    "Return type should be entity type, complex type or primitive type.");

                return this.returnType;
            }
        }

        /// <summary>Return type of the operation. Note that if this property is a collection type, the ResultType
        /// property is the item type of this property; otherwise the ResultType property is the same as this property.</summary>
        /// <example>
        /// For example if the operation returns an IQueryable&lt;Customer&gt;, this property would be of type Collection(Customer)
        /// and the ResultType property would be of type Customer.
        /// </example>
        internal ResourceType OperationReturnType
        {
            get { return this.returnType; }
        }

        /// <summary>Path expression to calculate the result resource set of the function if the function returns an entity or a collection of entity.</summary>
        internal ResourceSetPathExpression OperationResultSetPathExpression
        {
            get { return this.resultSetPathExpression; }
        }

        /// <summary>
        /// The binding parameter to this function/action; null if this function/action is top-level or this operation is not a function or action.
        /// </summary>
        internal OperationParameter OperationBindingParameter
        {
            get { return this.bindingParameter; }
        }

        /// <summary>
        /// The kind of the operation parameter binding - (Never, Sometimes, Always).
        /// </summary>
        /// <remarks>If the first parameter of the operation is a binding parameter then the value must be set to OperationParameterBindingKind.Sometimes or 
        /// OperationParameterBindingKind.Always. If the first parameter is not a binding parameter then the value must be set to OperationParameterBindingKind.Never. 
        /// If the value is set to OperationParameterBindingKind.Always then the IDataServiceActionProvider.AdvertiseServiceAction method will not be called 
        /// for the action and the action will be always advertised by the default convention.</remarks>
        internal OperationParameterBindingKind OperationParameterBindingKind
        {
            get { return this.operationParameterBindingKind; }
        }

        /// <summary>
        /// The kind of the current service operation
        /// </summary>
        internal OperationKind Kind
        {
            get { return this.kind; }
        }

        /// <summary> Set this service operation to readonly. </summary>
        public void SetReadOnly()
        {
            if (this.isReadOnly)
            {
                return;
            }

            foreach (OperationParameter parameter in this.OperationParameters)
            {
                parameter.SetReadOnly();
            }

            this.isReadOnly = true;
        }

        /// <summary>
        /// Returns the <see cref="ServiceOperationResultKind"/> based on the <paramref name="returnType"/> of the operation.
        /// </summary>
        /// <param name="returnType">The return type of the operation.</param>
        /// <param name="isComposable">true if further composition is allowed after calling this operation; false otherwise.</param>
        /// <returns>Returns the <see cref="ServiceOperationResultKind"/> based on the <paramref name="returnType"/> of the operation.</returns>
        internal static ServiceOperationResultKind GetResultKindFromReturnType(ResourceType returnType, bool isComposable)
        {
            ServiceOperationResultKind resultKind;
            if (returnType == null)
            {
                resultKind = ServiceOperationResultKind.Void;
            }
            else if (returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection && isComposable)
            {
                resultKind = ServiceOperationResultKind.QueryWithMultipleResults;
            }
            else if ((returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection && !isComposable) || returnType.ResourceTypeKind == ResourceTypeKind.Collection)
            {
                resultKind = ServiceOperationResultKind.Enumeration;
            }
            else
            {
                resultKind = ServiceOperationResultKind.DirectValue;
            }

            return resultKind;
        }

        /// <summary>
        /// Ensures the operation has been marked read-only.
        /// </summary>
        internal void EnsureReadOnly()
        {
            if (!this.isReadOnly)
            {
                throw new DataServiceException(500, Strings.DataServiceProviderWrapper_ServiceOperationNotReadonly(this.Name));
            }
        }

        /// <summary>
        /// Validates arguments to the constructor.
        /// </summary>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="returnType">Return type of the operation.</param>
        /// <param name="resultSet">EntitySet of the result expected from this operation, must be null if <paramref name="resultSetPathExpression"/> is not null.</param>
        /// <param name="resultSetPathExpression">Path expression to calculate the result set of the operation, must be null if <paramref name="resultSet"/> is not null.</param>
        /// <param name="method">Protocol (for example HTTP) method the service operation responds to.</param>
        /// <param name="operationParameterBindingKind">the kind of the operation parameter binding (Never, Sometimes, Always).</param>
        /// <param name="kind">The kind of the current service operation.</param>
        private static void ValidateConstructorArguments(
            string operationName, 
            ResourceType returnType, 
            ResourceSet resultSet, 
            ResourceSetPathExpression resultSetPathExpression, 
            string method, 
            OperationParameterBindingKind operationParameterBindingKind, 
            OperationKind kind)
        {
            if (returnType != null && (returnType.ResourceTypeKind == ResourceTypeKind.EntityType || returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection))
            {
                ResourceType resultType = returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection ? ((EntityCollectionResourceType)returnType).ItemType : returnType;
                if (resultSet == null && resultSetPathExpression == null || resultSet != null && !resultSet.ResourceType.IsAssignableFrom(resultType))
                {
                    if (kind == OperationKind.ServiceOperation)
                    {
                        throw new ArgumentException(Strings.ServiceOperation_ResultTypeAndResultSetMustMatch("resultType", "resultSet"));
                    }

                    throw new ArgumentException(Strings.ServiceOperation_ReturnTypeAndResultSetMustMatch("returnType", "resultSetPathExpression", "resultSet"));
                }
            }
            else
            {
                if (resultSet != null || resultSetPathExpression != null)
                {
                    string setParameterName = resultSet != null ? "resultSet" : "resultSetPathExpression";
                    if (kind == OperationKind.ServiceOperation)
                    {
                        throw new ArgumentException(Strings.ServiceOperation_ResultSetMustBeNullForGivenResultType(setParameterName, "resultType"));
                    }

                    throw new ArgumentException(Strings.ServiceOperation_ResultSetMustBeNullForGivenReturnType(setParameterName, "returnType"));
                }
            }

            if (returnType != null && returnType == ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))
            {
                string parameterName;
                string errorMessage;
                if (kind == OperationKind.ServiceOperation)
                {
                    parameterName = "resultType";
                    errorMessage = Strings.ServiceOperation_InvalidResultType(returnType.FullName);
                }
                else
                {
                    parameterName = "returnType";
                    errorMessage = Strings.ServiceOperation_InvalidReturnType(returnType.FullName);
                }

                throw new ArgumentException(errorMessage, parameterName);
            }

            if (string.CompareOrdinal(XmlConstants.HttpMethodGet, method) != 0 && 
                string.CompareOrdinal(XmlConstants.HttpMethodPost, method) != 0)
            {
                throw new ArgumentException(Strings.ServiceOperation_NotSupportedProtocolMethod(method, operationName), "method");
            }

            if (resultSetPathExpression != null && operationParameterBindingKind == OperationParameterBindingKind.Never)
            {
                // If the operation is not bindable, we want users to call the constructor that uses resultSet.
                throw new ArgumentException(Strings.ServiceOperation_MustBeBindableToUsePathExpression("resultSetPathExpression"), "resultSetPathExpression");
            }
        }

        /// <summary>
        /// Validates the input parameters and convert it to a read only collection of parameters.
        /// </summary>
        /// <param name="operationParameterBindingKind">the kind of the operation parameter binding (Never, Sometimes, Always).</param>
        /// <param name="parameters">In-order parameters for this operation.</param>
        /// <returns>A read only collection of parameters.</returns>
        private static ReadOnlyCollection<OperationParameter> ValidateParameters(OperationParameterBindingKind operationParameterBindingKind, IEnumerable<OperationParameter> parameters)
        {
            Debug.Assert(
                operationParameterBindingKind == OperationParameterBindingKind.Never || 
                operationParameterBindingKind == OperationParameterBindingKind.Always || 
                operationParameterBindingKind == OperationParameterBindingKind.Sometimes, 
                "Unexpected value of OperationParameterBindingKind.");

            ReadOnlyCollection<OperationParameter> resultParameters;
            if (parameters == null)
            {
                resultParameters = OperationParameter.EmptyOperationParameterCollection;
            }
            else
            {
                resultParameters = new ReadOnlyCollection<OperationParameter>(new List<OperationParameter>(parameters));
                HashSet<string> paramNames = new HashSet<string>(StringComparer.Ordinal);

                int bindingParameterIndex = operationParameterBindingKind != OperationParameterBindingKind.Never ? 0 : -1;
                for (int idx = 0; idx < resultParameters.Count; idx++)
                {
                    OperationParameter parameter = resultParameters[idx];
                    if (!paramNames.Add(parameter.Name))
                    {
                        throw new ArgumentException(Strings.ServiceOperation_DuplicateParameterName(parameter.Name), "parameters");
                    }

                    if (idx > bindingParameterIndex)
                    {
                        ResourceTypeKind parameterTypeKind = parameter.ParameterType.ResourceTypeKind;
                        if (parameterTypeKind == ResourceTypeKind.EntityType || parameterTypeKind == ResourceTypeKind.EntityCollection)
                        {
                            throw new ArgumentException(Strings.ServiceOperation_NonBindingParametersCannotBeEntityorEntityCollection(parameter.Name, parameterTypeKind));
                        }
                    }
                }
            }

            return resultParameters;
        }

        /// <summary>
        /// Throws an InvalidOperationException if this service operation is already set to readonly.
        /// </summary>
        private void ThrowIfSealed()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException(Strings.ServiceOperation_Sealed(this.Name));
            }
        }
    }
}
