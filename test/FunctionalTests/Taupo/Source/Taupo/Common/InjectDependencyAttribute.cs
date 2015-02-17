//---------------------------------------------------------------------
// <copyright file="InjectDependencyAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;

    /// <summary>
    /// Marks the specified property for external dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InjectDependencyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether the dependency is required.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired { get; set; }
    }
}