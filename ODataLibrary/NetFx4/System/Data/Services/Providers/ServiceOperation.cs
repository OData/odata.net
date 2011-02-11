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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.OData;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Use this class to represent a custom service operation.
    /// </summary>
#if !SILVERLIGHT && !WINDOWS_PHONE
    [DebuggerVisualizer("ServiceOperation={Name}")]
#endif
#if INTERNAL_DROP
    internal class ServiceOperation : ODataAnnotatable
#else
    public class ServiceOperation : ODataAnnotatable
#endif
    {
        /// <summary>
        /// HTTP method the service operation responds to.
        /// </summary>
        private readonly string method;

        /// <summary>
        /// In-order parameters for this operation.
        /// </summary>
        private readonly ReadOnlyCollection<ServiceOperationParameter> parameters;

        /// <summary>
        /// Kind of result expected from this operation.
        /// </summary>
        private readonly ServiceOperationResultKind resultKind;

        /// <summary>
        /// Type of element of the method result.
        /// </summary>
        private readonly ResourceType resultType;

        /// <summary>
        /// Empty parameter collection.
        /// </summary>
        private static ReadOnlyCollection<ServiceOperationParameter> emptyParameterCollection = new ReadOnlyCollection<ServiceOperationParameter>(new ServiceOperationParameter[0]);

        /// <summary>
        /// MIME type specified on primitive results, possibly null.
        /// </summary>
        private string mimeType;

        /// <summary>
        /// Entity set from which entities are read, if applicable.
        /// </summary>
        private ResourceSet resourceSet;

        /// <summary>
        /// Name of the service operation.
        /// </summary>
        private string name;

        /// <summary>
        /// Is true, if the service operation is set to readonly i.e. fully initialized and validated. No more changes can be made,
        /// after the service operation is set to readonly.
        /// </summary>
        private bool isReadOnly;

        /// <summary>
        /// Initializes a new <see cref="ServiceOperation"/> instance.
        /// </summary>
        /// <param name="name">name of the service operation.</param>
        /// <param name="resultKind">Kind of result expected from this operation.</param>
        /// <param name="resultType">Type of element of the method result.</param>
        /// <param name="resultSet">EntitySet of the result expected from this operation.</param>
        /// <param name="method">Protocol (for example HTTP) method the service operation responds to.</param>
        /// <param name="parameters">In-order parameters for this operation.</param>
        public ServiceOperation(
            string name,
            ServiceOperationResultKind resultKind,
            ResourceType resultType,
            ResourceSet resultSet,
            string method,
            IEnumerable<ServiceOperationParameter> parameters)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            CheckServiceOperationResultKind(resultKind, "resultKind");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(method, "method");

            if ((resultKind == ServiceOperationResultKind.Void && resultType != null) ||
                (resultKind != ServiceOperationResultKind.Void && resultType == null))
            {
                throw new ArgumentException(Strings.ServiceOperation_ResultTypeAndKindMustMatch("resultKind", "resultType", ServiceOperationResultKind.Void));
            }

            if ((resultType == null || resultType.ResourceTypeKind != ResourceTypeKind.EntityType) && resultSet != null)
            {
                throw new ArgumentException(Strings.ServiceOperation_ResultSetMustBeNull("resultSet", "resultType"));
            }

            if (resultType != null && resultType.ResourceTypeKind == ResourceTypeKind.EntityType && (resultSet == null || !resultSet.ResourceType.IsAssignableFrom(resultType)))
            {
                throw new ArgumentException(Strings.ServiceOperation_ResultTypeAndResultSetMustMatch("resultType", "resultSet"));
            }

            if (resultType != null && resultType.ResourceTypeKind == ResourceTypeKind.MultiValue)
            {
                throw new ArgumentException(Strings.ServiceOperation_InvalidResultType(resultType.FullName));
            }

            if (method != HttpConstants.HttpMethodGet && method != HttpConstants.HttpMethodPost)
            {
                throw new ArgumentException(Strings.ServiceOperation_NotSupportedProtocolMethod(method, name));
            }

            this.name = name;
            this.resultKind = resultKind;
            this.resultType = resultType;
            this.resourceSet = resultSet;
            this.method = method;
            if (parameters == null)
            {
                this.parameters = ServiceOperation.emptyParameterCollection;
            }
            else
            {
                this.parameters = new ReadOnlyCollection<ServiceOperationParameter>(new List<ServiceOperationParameter>(parameters));
                HashSet<string> paramNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (ServiceOperationParameter p in this.parameters)
                {
                    if (!paramNames.Add(p.Name))
                    {
                        throw new ArgumentException(Strings.ServiceOperation_DuplicateParameterName(p.Name), "parameters");
                    }
                }
            }
        }

        /// <summary>
        /// Protocol (for example HTTP) method the service operation responds to.
        /// </summary>
        public string Method
        {
            get { return this.method; }
        }

        /// <summary>
        /// MIME type specified on primitive results, possibly null.
        /// </summary>
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

                if (!HttpUtils.IsValidMediaTypeName(value))
                {
                    throw new InvalidOperationException(Strings.ServiceOperation_MimeTypeNotValid(value, this.Name));
                }

                this.mimeType = value;
            }
        }

        /// <summary>
        /// Name of the service operation.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Returns all the parameters for the given service operations.
        /// </summary>
        public ReadOnlyCollection<ServiceOperationParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Kind of result expected from this operation.
        /// </summary>
        public ServiceOperationResultKind ResultKind
        {
            get { return this.resultKind; }
        }

        /// <summary>
        /// Element of result type.
        /// </summary>
        /// <remarks>
        /// Note that if the method returns an IEnumerable&lt;string&gt;, 
        /// this property will be typeof(string).
        /// </remarks>
        public ResourceType ResultType
        {
            get { return this.resultType; }
        }

        /// <summary>
        /// PlaceHolder to hold custom state information about service operation.
        /// </summary>
        public object CustomState
        {
            get
            {
                return this.GetCustomState();
            }

            set
            {
                this.SetCustomState(value);
            }
        }

        /// <summary>
        /// Returns true, if this service operation has been set to read only. Otherwise returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary>
        /// Entity set from which entities are read (possibly null).
        /// </summary>
        public ResourceSet ResourceSet
        {
            get { return this.resourceSet; }
        }

        /// <summary>
        /// Set this service operation to readonly.
        /// </summary>
        public void SetReadOnly()
        {
            if (this.isReadOnly)
            {
                return;
            }

            foreach (ServiceOperationParameter parameter in this.Parameters)
            {
                parameter.SetReadOnly();
            }

            this.isReadOnly = true;
        }

        /// <summary>
        /// Check whether the given value for ServiceOperationResultKind is valid. If not, throw argument exception.
        /// </summary>
        /// <param name="kind">value for ServiceOperationResultKind</param>
        /// <param name="parameterName">name of the parameter</param>
        /// <exception cref="ArgumentException">if the value is not valid.</exception>
        private static void CheckServiceOperationResultKind(ServiceOperationResultKind kind, string parameterName)
        {
            if (kind < ServiceOperationResultKind.DirectValue ||
                kind > ServiceOperationResultKind.Void)
            {
                throw new ArgumentException(Strings.General_InvalidEnumValue(kind.GetType().Name), parameterName);
            }
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
