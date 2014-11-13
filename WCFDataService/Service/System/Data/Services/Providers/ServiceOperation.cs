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
        ///   <see cref="T:System.Data.Services.Providers.ServiceOperationResultKind" /> that is the kind of result expected from this operation.</param>
        /// <param name="resultType">
        ///   <see cref="T:System.Data.Services.Providers.ResourceType" /> that is the result of the operation.</param>
        /// <param name="resultSet">
        ///   <see cref="T:System.Data.Services.Providers.ResourceSet" /> that is the result of the operation.</param>
        /// <param name="method">Protocol method to which the service operation responds.</param>
        /// <param name="parameters">Ordered collection of <see cref="T:System.Data.Services.Providers.ServiceOperationParameter" /> objects that are parameters for the operation.</param>
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
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Data.Services.Providers.ServiceOperationParameter" /> objects.</returns>
        public ReadOnlyCollection<ServiceOperationParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>The kind of result that is expected by this service operation.</summary>
        /// <returns><see cref="T:System.Data.Services.Providers.ServiceOperationResultKind" /> that is the kind of result expected from this operation.</returns>
        public ServiceOperationResultKind ResultKind
        {
            get { return this.OperationResultKind; }
        }

        /// <summary>Type of results returned by this service operation.</summary>
        /// <returns>Type of the results as a <see cref="T:System.Data.Services.Providers.ResourceType" />.</returns>
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
