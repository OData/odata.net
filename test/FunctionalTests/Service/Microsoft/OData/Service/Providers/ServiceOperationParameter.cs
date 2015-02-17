//---------------------------------------------------------------------
// <copyright file="ServiceOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
