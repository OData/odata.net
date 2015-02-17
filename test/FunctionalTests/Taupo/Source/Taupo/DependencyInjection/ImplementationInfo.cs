//---------------------------------------------------------------------
// <copyright file="ImplementationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;

    /// <summary>
    /// Information about default implementation for a contract.
    /// </summary>
    public class ImplementationInfo
    {
        /// <summary>
        /// Initializes a new instance of the ImplementationInfo class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="isTransient">If set to <c>true</c> the implementation should not be cached by the container.</param>
        /// <param name="isGlobal">If set to <c>true</c> the object is global (should be pre-resolved at the TestModule level).</param>
        /// <param name="isTestParameterSpecified">If set to <c>true</c> the object is specified by a test parameter.</param>
        /// <param name="isDefault">If set to <c>true</c> the object is default</param>
        public ImplementationInfo(Type contractType, Type implementationType, bool isTransient, bool isGlobal, bool isTestParameterSpecified, bool isDefault)
        {
            this.ContractType = contractType;
            this.ImplementationType = implementationType;
            this.IsTransient = isTransient;
            this.IsGlobal = isGlobal;
            this.IsTestParameterSpecified = isTestParameterSpecified;
            this.IsDefault = isDefault;
        }

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>The type of the contract.</value>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Gets the type of the implementation.
        /// </summary>
        /// <value>The type of the implementation.</value>
        public Type ImplementationType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this dependency is transient.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is transient; otherwise, <c>false</c>.
        /// </value>
        public bool IsTransient { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this dependency is global.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is global; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Global dependencies are pre-resolved and initalized at the TestModule level even if nobody 
        /// requests them.</remarks>
        public bool IsGlobal { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this dependency is specified as a test parameter.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is specified as a test parameter otherwise, <c>false</c>.
        /// </value>
        public bool IsTestParameterSpecified { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this dependency is default.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; private set; }
    }
}
