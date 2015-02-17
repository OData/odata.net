//---------------------------------------------------------------------
// <copyright file="InjectTestParameterAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;

    /// <summary>
    /// Marks the specified property for external test property (e.g. LTM arguments) injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class InjectTestParameterAttribute : Attribute
    {
        private readonly string parameterName;

        /// <summary>
        /// Initializes a new instance of the InjectTestParameterAttribute class with given parameter name.
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        public InjectTestParameterAttribute(string parameterName)
        {
            this.parameterName = parameterName;
            this.IsObsolete = false;
        }

        /// <summary>
        /// Gets parameter name to inject.
        /// </summary>
        /// <remarks>
        /// This parameter may come from LTM Alias init string. For example 'Server/Version'
        /// </remarks>
        public string ParameterName
        {
            get { return this.parameterName; }
        }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets a description of the default value (to be displayed in help text).
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValueDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is obsolete and will soon be removed or renamed. Only one non-obsolete attribute is allowed on a given property.
        /// </summary>
        public bool IsObsolete { get; set; }
    }
}