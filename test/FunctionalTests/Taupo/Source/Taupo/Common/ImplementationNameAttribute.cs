//---------------------------------------------------------------------
// <copyright file="ImplementationNameAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;

    /// <summary>
    /// Assigns a short name to the implementation.
    /// </summary>
    /// <remarks>
    /// The short name is used to select implementation for a contract from command line.
    /// <example>
    /// Consider the following classes:
    /// [ImplementationSelector("MyArg", DefaultImplementation = "Impl1")]
    /// interface IContract { }
    /// [ImplementationName(typeof(IContract), "Impl1")]
    /// class Implementation1 : IContract { }
    /// [ImplementationName(typeof(IContract), "Impl2")]
    /// class Implementation2 : IContract { }
    /// [ImplementationName(typeof(IContract), "Impl3")]
    /// class Implementation3 : IContract { }
    /// With this in place it is possible to select default implementation of the contract
    /// by specifying 'MyArg=Impl1', 'MyArg=Impl2' or 'MyArg=Impl3' test parameters (for example in LTM alias).
    /// If you don't specify a parameter value, "Impl1" will be used because it has been declared as
    /// default implementation.
    /// </example>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ImplementationNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ImplementationNameAttribute class.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="implementationName">The name of the implementation.</param>
        public ImplementationNameAttribute(Type contractType, string implementationName)
        {
            this.ContractType = contractType;
            this.ImplementationName = implementationName;
            this.Tags = new string[] { };
        }

        /// <summary>
        /// Gets the contract type.
        /// </summary>
        /// <value>The type of the contract.</value>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Gets the name of the implementation.
        /// </summary>
        /// <value>The name of the implementation.</value>
        public string ImplementationName { get; private set; }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets Tags that are used to identify how various
        /// Implementation Names are related
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required as its an attribute")]
        public string[] Tags { get; set; }
    }
}