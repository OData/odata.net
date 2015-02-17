//---------------------------------------------------------------------
// <copyright file="ImplementationSelectorAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;

    /// <summary>
    /// Associates contract with test argument used to select concrete implementation at runtime.
    /// </summary>
    /// <remarks>
    /// This attribute must be placed on a contract type (interface or abstract class) and 
    /// <see cref="ImplementationNameAttribute"/> must be placed on all implementations.
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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class ImplementationSelectorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ImplementationSelectorAttribute class.
        /// </summary>
        /// <param name="testArgumentName">Name of the test argument used to select implementation.</param>
        public ImplementationSelectorAttribute(string testArgumentName)
        {
            this.TestArgumentName = testArgumentName;
        }

        /// <summary>
        /// Gets the name of the test argument used to select implementation.
        /// </summary>
        /// <value>The name of the argument.</value>
        public string TestArgumentName { get; private set; }

        /// <summary>
        /// Gets or sets the name of the default implementation for this contract.
        /// </summary>
        /// <value>The default name of the implementation.</value>
        public string DefaultImplementation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether implementation of this contract must be defined.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether implementations of the interface are transient.
        /// </summary>
        /// <value>
        /// A value <c>true</c> if this instance is transient (should not be cached by dependency injection container);
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsTransient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether objects implementing the interface are global.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is global; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Global objects are pre-initialized before any other objects (typically 
        /// during test module initialization)
        /// </remarks>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText { get; set; }
    }
}