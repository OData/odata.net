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

    /// <summary>Use this type to represent a parameter on a service action.</summary>
    [DebuggerVisualizer("ServiceActionParameter={Name}")]
    public class ServiceActionParameter : OperationParameter
    {
        /// <summary>Empty parameter collection.</summary>
        internal static readonly ReadOnlyCollection<ServiceActionParameter> EmptyServiceActionParameterCollection = new ReadOnlyCollection<ServiceActionParameter>(new ServiceActionParameter[0]);

        /// <summary> Initializes a new <see cref="T:System.Data.Services.Providers.ServiceActionParameter" />. </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="parameterType">resource type of parameter value.</param>
        public ServiceActionParameter(string name, ResourceType parameterType)
            : base(name, parameterType)
        {
        }
    }
}
