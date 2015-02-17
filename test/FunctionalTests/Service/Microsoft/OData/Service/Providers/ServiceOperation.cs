//---------------------------------------------------------------------
// <copyright file="ServiceOperation.cs" company="Microsoft">
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
    [DebuggerVisualizer("ServiceOperation={Name}")]
    public class ServiceOperation : Operation
    {
        /// <summary>In-order parameters for this operation.</summary>
        private readonly ReadOnlyCollection<ServiceOperationParameter> parameters;

        /// <summary>Creates a new instance of the service operation.</summary>
        /// <param name="name">Name of the service operation.</param>
        /// <param name="resultKind">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ServiceOperationResultKind" /> that is the kind of result expected from this operation.</param>
        /// <param name="resultType">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> that is the result of the operation.</param>
        /// <param name="resultSet">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ResourceSet" /> that is the result of the operation.</param>
        /// <param name="method">Protocol method to which the service operation responds.</param>
        /// <param name="parameters">Ordered collection of <see cref="T:Microsoft.OData.Service.Providers.ServiceOperationParameter" /> objects that are parameters for the operation.</param>
        public ServiceOperation(string name, ServiceOperationResultKind resultKind, ResourceType resultType, ResourceSet resultSet, string method, IEnumerable<ServiceOperationParameter> parameters)
            : base(
            name,
            resultKind,
            ServiceOperation.GetReturnTypeFromResultType(resultType, resultKind),
            resultSet,
            null /*resultSetExpression*/,
            method,
            parameters,
            OperationParameterBindingKind.Never,
            OperationKind.ServiceOperation)
        {
            Debug.Assert(this.OperationParameters != null, "this.OperationParameters != null");
            if (this.OperationParameters == OperationParameter.EmptyOperationParameterCollection)
            {
                this.parameters = ServiceOperationParameter.EmptyServiceOperationParameterCollection;
            }
            else
            {
                this.parameters = new ReadOnlyCollection<ServiceOperationParameter>(this.OperationParameters.Cast<ServiceOperationParameter>().ToList());
            }
        }

        /// <summary>Collection of in-order parameters for the service operation.</summary>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:Microsoft.OData.Service.Providers.ServiceOperationParameter" /> objects.</returns>
        public ReadOnlyCollection<ServiceOperationParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>The kind of result that is expected by this service operation.</summary>
        /// <returns><see cref="T:Microsoft.OData.Service.Providers.ServiceOperationResultKind" /> that is the kind of result expected from this operation.</returns>
        public ServiceOperationResultKind ResultKind
        {
            get { return this.OperationResultKind; }
        }

        /// <summary>Type of results returned by this service operation.</summary>
        /// <returns>Type of the results as a <see cref="T:Microsoft.OData.Service.Providers.ResourceType" />.</returns>
        /// <remarks>If the return type is a collection type, this is the item type of the return type; otherwise this is the same as return type.</remarks>
        public ResourceType ResultType
        {
            get { return this.OperationResultType; }
        }

        /// <summary>
        /// Gets the return type of the operation.
        /// </summary>
        /// <param name="resultType">Type of element of the method result. This is the item type of the return type if the return type is a collection type.</param>
        /// <param name="resultKind">Kind of result expected from this operation.</param>
        /// <returns>Returns the return type of the operation.</returns>
        private static ResourceType GetReturnTypeFromResultType(ResourceType resultType, ServiceOperationResultKind resultKind)
        {
            if ((resultKind == ServiceOperationResultKind.Void && resultType != null) ||
                (resultKind != ServiceOperationResultKind.Void && resultType == null))
            {
                throw new ArgumentException(Strings.ServiceOperation_ResultTypeAndKindMustMatch("resultKind", "resultType", ServiceOperationResultKind.Void));
            }

            if (resultType != null && resultType.ResourceTypeKind == ResourceTypeKind.Collection)
            {
                throw new ArgumentException(Strings.ServiceOperation_InvalidResultType(resultType.FullName));
            }

            if (resultType != null && resultType.ResourceTypeKind == ResourceTypeKind.EntityCollection)
            {
                throw new ArgumentException(Strings.ServiceOperation_InvalidResultType(resultType.FullName));
            }

            ResourceType returnType;
            if (resultType == null)
            {
                Debug.Assert(resultKind == ServiceOperationResultKind.Void, "resultKind == ServiceOperationResultKind.Void");
                returnType = null;
            }
            else if ((resultType.ResourceTypeKind == ResourceTypeKind.Primitive || resultType.ResourceTypeKind == ResourceTypeKind.ComplexType) && (resultKind == ServiceOperationResultKind.Enumeration || resultKind == ServiceOperationResultKind.QueryWithMultipleResults))
            {
                returnType = ResourceType.GetCollectionResourceType(resultType);
            }
            else if (resultType.ResourceTypeKind == ResourceTypeKind.EntityType && (resultKind == ServiceOperationResultKind.Enumeration || resultKind == ServiceOperationResultKind.QueryWithMultipleResults))
            {
                returnType = ResourceType.GetEntityCollectionResourceType(resultType);
            }
            else
            {
                Debug.Assert(
                    resultKind == ServiceOperationResultKind.DirectValue || resultKind == ServiceOperationResultKind.QueryWithSingleResult,
                    "resultKind == ServiceOperationResultKind.DirectValue || resultKind == ServiceOperationResultKind.QueryWithSingleResult");
                returnType = resultType;
            }

            return returnType;
        }
    }
}