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
    using System.Data.OData;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Use this type to represent a parameter on a service operation.
    /// </summary>
#if !SILVERLIGHT && !WINDOWS_PHONE
    [DebuggerVisualizer("ServiceOperationParameter={Name}")]
#endif
#if INTERNAL_DROP
    internal class ServiceOperationParameter : ODataAnnotatable
#else
    public class ServiceOperationParameter : ODataAnnotatable
#endif
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Parameter type.
        /// </summary>
        private readonly ResourceType type;

        /// <summary>
        /// Is true, if the service operation parameter is set to readonly i.e. fully initialized and validated.
        /// No more changes can be made, after this is set to readonly.
        /// </summary>
        private bool isReadOnly;

        /// <summary>
        /// Initializes a new <see cref="ServiceOperationParameter"/>.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="parameterType">resource type of parameter value.</param>
        public ServiceOperationParameter(string name, ResourceType parameterType)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            ExceptionUtils.CheckArgumentNotNull(parameterType, "parameterType");

            if (parameterType.ResourceTypeKind != ResourceTypeKind.Primitive)
            {
                throw new ArgumentException(Strings.ServiceOperationParameter_TypeNotSupported(name, parameterType.FullName));
            }

            this.name = name;
            this.type = parameterType;
        }

        /// <summary>
        /// Name of parameter.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Type of parameter values.
        /// </summary>
        public ResourceType ParameterType
        {
            get { return this.type; }
        }

        /// <summary>
        /// PlaceHolder to hold custom state information about service operation parameter.
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
        /// Returns true, if this parameter has been set to read only. Otherwise returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary>
        /// Sets this service operation parameter to readonly.
        /// </summary>
        public void SetReadOnly()
        {
            if (this.isReadOnly)
            {
                return;
            }

            this.isReadOnly = true;
        }
    }
}
