//---------------------------------------------------------------------
// <copyright file="OperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Use this type to represent a parameter on an operation.</summary>
    [DebuggerVisualizer("OperationParameterBase={Name}")]
    public abstract class OperationParameter
    {
        /// <summary>Empty parameter collection.</summary>
        internal static readonly ReadOnlyCollection<OperationParameter> EmptyOperationParameterCollection = new ReadOnlyCollection<OperationParameter>(new OperationParameter[0]);

        /// <summary>Parameter name.</summary>
        private readonly string name;

        /// <summary>Parameter type.</summary>
        private readonly ResourceType type;

        /// <summary>true if the operation parameter is set to readonly i.e. fully initialized and validated.
        /// No more changes can be made, after this is set to readonly.</summary>
        private bool isReadOnly;

        /// <summary> Initializes a new <see cref="T:Microsoft.OData.Service.Providers.OperationParameter" />. </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="parameterType">resource type of parameter value.</param>
        protected internal OperationParameter(string name, ResourceType parameterType)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            WebUtil.CheckArgumentNull(parameterType, "parameterType");

            if (parameterType.ResourceTypeKind == ResourceTypeKind.Primitive && parameterType == ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))
            {
                throw new ArgumentException(Strings.ServiceOperationParameter_TypeNotSupported(name, parameterType.FullName), "parameterType");
            }

            this.name = name;
            this.type = parameterType;
        }

        /// <summary>Name of parameter.</summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>Type of parameter values.</summary>
        public ResourceType ParameterType
        {
            get { return this.type; }
        }

        /// <summary> PlaceHolder to hold custom state information about service operation parameter. </summary>
        public object CustomState
        {
            get;
            set;
        }

        /// <summary> Returns true, if this parameter has been set to read only. Otherwise returns false. </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary> Sets this service operation parameter to readonly. </summary>
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
