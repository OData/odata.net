//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Use this type to represent a parameter on a service operation.</summary>
    [DebuggerVisualizer("ServiceOperationParameter={Name}")]
    public class ServiceOperationParameter : OperationParameter
    {
        /// <summary>Empty parameter collection.</summary>
        internal static readonly ReadOnlyCollection<ServiceOperationParameter> EmptyServiceOperationParameterCollection = new ReadOnlyCollection<ServiceOperationParameter>(new ServiceOperationParameter[0]);

        /// <summary>Creates a new instance of <see cref="T:Microsoft.OData.Service.Providers.ServiceOperationParameter" />.</summary>
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
