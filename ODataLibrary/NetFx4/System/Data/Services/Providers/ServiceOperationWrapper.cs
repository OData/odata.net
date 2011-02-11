//   Copyright 2011 Microsoft Corporation
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
    #region Namespaces.
    using System;
    using System.Collections.ObjectModel;
    using System.Data.OData;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Use this class to represent a custom service operation.
    /// </summary>
    /// <remarks>Needed for example to atomize the resource types and resource sets.</remarks>
#if !SILVERLIGHT && !WINDOWS_PHONE
    [DebuggerVisualizer("ServiceOperation={Name}")]
#endif
    internal sealed class ServiceOperationWrapper
    {
        #region Private Fields
        /// <summary>
        /// Wrapped instance of the service operation.
        /// </summary>
        private readonly ServiceOperation serviceOperation;

        /// <summary>
        /// Entity set from which entities are read, if applicable.
        /// </summary>
        private ResourceSetWrapper resourceSet;

        /// <summary>
        /// The resource type of the result of the service operation.
        /// </summary>
        private ResourceType resultType;

#if DEBUG
        /// <summary>Is true, if the service operation is fully initialized and validated. No more changes can be made once its set to readonly.</summary>
        private bool isReadOnly;
#endif
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ServiceOperationWrapper"/> instance.
        /// </summary>
        /// <param name="serviceOperation">ServiceOperation instance to be wrapped.</param>
        private ServiceOperationWrapper(ServiceOperation serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");

            this.serviceOperation = serviceOperation;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Name of the service operation.
        /// </summary>
        internal string Name
        {
            [DebuggerStepThrough]
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serviceOperation.Name; 
            }
        }

        /// <summary>
        /// Returns all the parameters for the given service operations.
        /// </summary>
        internal ReadOnlyCollection<ServiceOperationParameter> Parameters
        {
            [DebuggerStepThrough]
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serviceOperation.Parameters; 
            }
        }

        /// <summary>
        /// Kind of result expected from this operation.
        /// </summary>
        internal ServiceOperationResultKind ResultKind
        {
            [DebuggerStepThrough]
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serviceOperation.ResultKind; 
            }
        }

        /// <summary>Element of result type.</summary>
        /// <remarks>
        /// Note that if the method returns an IEnumerable&lt;string&gt;, 
        /// this property will be typeof(string).
        /// </remarks>
        internal ResourceType ResultType
        {
            [DebuggerStepThrough]
            get 
            {
                DebugUtils.CheckNoExternalCallers();
#if DEBUG
                Debug.Assert(this.isReadOnly, "Wrapper class has not been initialized yet.");
#endif
                return this.resultType; 
            }
        }

        /// <summary>
        /// Gets the wrapped service operation
        /// </summary>
        internal ServiceOperation ServiceOperation
        {
            [DebuggerStepThrough]
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serviceOperation; 
            }
        }

        /// <summary>
        /// Resource set from which entities are read (possibly null).
        /// </summary>
        internal ResourceSetWrapper ResourceSet
        {
            [DebuggerStepThrough]
            get
            {
                DebugUtils.CheckNoExternalCallers();
#if DEBUG
                Debug.Assert(this.isReadOnly, "Wrapper class has not been initialized yet.");
#endif
                return this.resourceSet;
            }
        }
        #endregion Properties

        /// <summary>
        /// Creates the wrapper from the given service operation.
        /// </summary>
        /// <param name="serviceOperation">Service operation instance whose wrapper needs to get created.</param>
        /// <param name="resourceSetValidator">Resource set validator.</param>
        /// <param name="resourceTypeValidator">Resource type validator.</param>
        /// <returns>Wrapper for the given service operation.</returns>
        internal static ServiceOperationWrapper CreateServiceOperationWrapper(
            ServiceOperation serviceOperation,
            Func<ResourceSet, ResourceSetWrapper> resourceSetValidator,
            Func<ResourceType, ResourceType> resourceTypeValidator)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            Debug.Assert(serviceOperation.IsReadOnly, "The serviceOperation must be read-only by now.");
            Debug.Assert(resourceSetValidator != null, "resourceSetValidator != null");
            Debug.Assert(resourceTypeValidator != null, "resourceTypeValidator != null");

            ServiceOperationWrapper serviceOperationWrapper = new ServiceOperationWrapper(serviceOperation);
#if DEBUG
            serviceOperationWrapper.isReadOnly = true;
#endif

            serviceOperationWrapper.resourceSet = resourceSetValidator(serviceOperation.ResourceSet);
            ResourceType resultType = serviceOperation.ResultType;
            if (resultType == null || resultType.ResourceTypeKind == ResourceTypeKind.Primitive)
            {
                // No need to validate primitive resource types
                serviceOperationWrapper.resultType = resultType;
            }
            else
            {
                // Validate the resource type of the result
                serviceOperationWrapper.resultType = resourceTypeValidator(resultType);
            }

            return serviceOperationWrapper;
        }
    }
}
