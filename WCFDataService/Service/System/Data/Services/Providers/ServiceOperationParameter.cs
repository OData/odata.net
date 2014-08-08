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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Use this type to represent a parameter on a service operation.</summary>
    [DebuggerVisualizer("ServiceOperationParameter={Name}")]
    public class ServiceOperationParameter : OperationParameter
    {
        /// <summary>Empty parameter collection.</summary>
        internal static readonly ReadOnlyCollection<ServiceOperationParameter> EmptyServiceOperationParameterCollection = new ReadOnlyCollection<ServiceOperationParameter>(new ServiceOperationParameter[0]);

        /// <summary>Creates a new instance of <see cref="T:System.Data.Services.Providers.ServiceOperationParameter" />.</summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="parameterType">Data type of parameter.</param>
        public ServiceOperationParameter(string name, ResourceType parameterType)
            : base(name, parameterType)
        {
            WebUtil.CheckArgumentNull(parameterType, "parameterType");
            if (parameterType.ResourceTypeKind != ResourceTypeKind.Primitive)
            {
                throw new ArgumentException(Strings.ServiceOperationParameter_TypeNotSupported(name, parameterType.FullName), "parameterType");
            }
        }
    }
}
