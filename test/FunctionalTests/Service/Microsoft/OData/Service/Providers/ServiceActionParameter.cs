//---------------------------------------------------------------------
// <copyright file="ServiceActionParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
