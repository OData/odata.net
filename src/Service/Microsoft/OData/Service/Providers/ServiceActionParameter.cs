//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    /// <summary>Use this type to represent a parameter on a service action.</summary>
    [DebuggerVisualizer("ServiceActionParameter={Name}")]
    public class ServiceActionParameter : OperationParameter
    {
        /// <summary>Empty parameter collection.</summary>
        internal static readonly ReadOnlyCollection<ServiceActionParameter> EmptyServiceActionParameterCollection = new ReadOnlyCollection<ServiceActionParameter>(new ServiceActionParameter[0]);

        /// <summary> Initializes a new <see cref="T:Microsoft.OData.Service.Providers.ServiceActionParameter" />. </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="parameterType">resource type of parameter value.</param>
        public ServiceActionParameter(string name, ResourceType parameterType)
            : base(name, parameterType)
        {
        }
    }
}
