//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>Use this class to represent a action.</summary>
    [DebuggerVisualizer("ServiceAction={Name}")]
    public class ServiceAction : Operation
    {
        /// <summary>In-order parameters for this action.</summary>
        private readonly ReadOnlyCollection<ServiceActionParameter> parameters;

        /// <summary>Initializes a new <see cref="T:System.Data.Services.Providers.ServiceAction" /> instance.</summary>
        /// <param name="name">Name of the action.</param>
        /// <param name="returnType">Return type of the action.</param>
        /// <param name="resultSet">Result resource set of the action if the action returns an entity or a collection of entity; null otherwise.</param>
        /// <param name="operationParameterBindingKind">the kind of the operation parameter binding (Never, Sometimes, Always).</param>
        /// <param name="parameters">In-order parameters for this action.</param>
        /// <remarks>The value of <paramref name="operationParameterBindingKind"/> must be set to <see cref="OperationParameterBindingKind.Sometimes"/> or
        /// <see cref="OperationParameterBindingKind.Always"/> if the first parameter in <paramref name="parameters"/> is the binding parameter
        /// or <see cref="OperationParameterBindingKind.Never"/> if the first parameter is not a binding parameter. If the value of <paramref name="operationParameterBindingKind"/>
        /// is set to <see cref="OperationParameterBindingKind.Always"/> then the IDataServiceActionProvider.AdvertiseServiceAction method will not be called for the action
        /// and the action will be always advertised by the default convention.</remarks>
        public ServiceAction(string name, ResourceType returnType, ResourceSet resultSet, OperationParameterBindingKind operationParameterBindingKind, IEnumerable<ServiceActionParameter> parameters)
            : this(name, returnType, resultSet, null /*resultSetPathExpression*/, parameters, operationParameterBindingKind)
        {
        }

        /// <summary>Initializes a new <see cref="T:System.Data.Services.Providers.ServiceAction" /> instance.</summary>
        /// <param name="name">Name of the action.</param>
        /// <param name="returnType">Return type of the action.</param>
        /// <param name="operationParameterBindingKind">the kind of the operation parameter binding (Never, Sometimes, Always).</param>
        /// <param name="parameters">In-order parameters for this action; the first parameter is the binding parameter.</param>
        /// <param name="resultSetPathExpression">Path expression to calculate the result resource set of the function if the action returns an entity or a collection of entity; null otherwise.</param>
        /// <remarks>The value of <paramref name="operationParameterBindingKind"/> must be set to <see cref="OperationParameterBindingKind.Sometimes"/> or
        /// <see cref="OperationParameterBindingKind.Always"/> if the first parameter in <paramref name="parameters"/> is the binding parameter
        /// or <see cref="OperationParameterBindingKind.Never"/> if the first parameter is not a binding parameter. If the value of <paramref name="operationParameterBindingKind"/>
        /// is set to <see cref="OperationParameterBindingKind.Always"/> then the IDataServiceActionProvider.AdvertiseServiceAction method will not be called for the action
        /// and the action will be always advertised by the default convention.</remarks>
        public ServiceAction(string name, ResourceType returnType, OperationParameterBindingKind operationParameterBindingKind, IEnumerable<ServiceActionParameter> parameters, ResourceSetPathExpression resultSetPathExpression)
            : this(name, returnType, null /*resultSet*/, resultSetPathExpression, parameters, operationParameterBindingKind)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ServiceAction"/> instance.
        /// </summary>
        /// <param name="name">Name of the action.</param>
        /// <param name="returnType">Return type of the action.</param>
        /// <param name="resultSet">Result resource set of the action if the action returns an entity or a collection of entity; null otherwise.</param>
        /// <param name="resultSetPathExpression">Path expression to calculate the result resource set of the function if the action returns an entity or a collection of entity; null otherwise.</param>
        /// <param name="parameters">In-order parameters for this action; the first parameter is the binding parameter.</param>
        /// <param name="operationParameterBindingKind">the kind of the operation parameter binding (Never, Sometimes, Always).</param>
        /// <remarks>The value of <paramref name="operationParameterBindingKind"/> must be set to <see cref="OperationParameterBindingKind.Sometimes"/> or 
        /// <see cref="OperationParameterBindingKind.Always"/> if the first parameter in <paramref name="parameters"/> is the binding parameter 
        /// or <see cref="OperationParameterBindingKind.Never"/> if the first parameter is not a binding parameter. If the value of <paramref name="operationParameterBindingKind"/> 
        /// is set to <see cref="OperationParameterBindingKind.Always"/> then the IDataServiceActionProvider.AdvertiseServiceAction method will not be called for the action
        /// and the action will be always advertised by the default convention.</remarks>
        private ServiceAction(string name, ResourceType returnType, ResourceSet resultSet, ResourceSetPathExpression resultSetPathExpression, IEnumerable<ServiceActionParameter> parameters, OperationParameterBindingKind operationParameterBindingKind)
            : base(
            name,
            Operation.GetResultKindFromReturnType(returnType, isComposable: false),
            returnType,
            resultSet,
            resultSetPathExpression,
            XmlConstants.HttpMethodPost,
            parameters,
            operationParameterBindingKind,
            OperationKind.Action)
        {
            Debug.Assert(this.OperationParameters != null, "this.OperationParameters != null");
            if (this.OperationParameters == OperationParameter.EmptyOperationParameterCollection)
            {
                this.parameters = ServiceActionParameter.EmptyServiceActionParameterCollection;
            }
            else
            {
                this.parameters = new ReadOnlyCollection<ServiceActionParameter>(this.OperationParameters.Cast<ServiceActionParameter>().ToList());
            }
        }
        
        /// <summary>Gets all the parameters for the given service action.</summary>
        /// <returns>The parameters for the given service action.</returns>
        public ReadOnlyCollection<ServiceActionParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>Gets the return type of the action.</summary>
        /// <returns>The return type of the action.</returns>
        public ResourceType ReturnType
        {
            get { return this.OperationReturnType; }
        }

        /// <summary>Gets the path expression to calculate the result resource set of the function if the function returns an entity or a collection of entity.</summary>
        /// <returns>The Path expression to calculate.</returns>
        public ResourceSetPathExpression ResultSetPathExpression
        {
            get { return this.OperationResultSetPathExpression; }
        }

        /// <summary>Gets the binding parameter to this action; null if this action is top-level.</summary>
        /// <returns>The binding parameter to this action; null if this action is top-level.</returns>
        public ServiceActionParameter BindingParameter
        {
            get { return (ServiceActionParameter)this.OperationBindingParameter; }
        }
    }
}
