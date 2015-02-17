//---------------------------------------------------------------------
// <copyright file="ClrMethodAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Reflection;

    /// <summary>
    /// Annotation that contains information about clr method
    /// </summary>
    public class ClrMethodAnnotation : Annotation
    {
        /// <summary>
        /// Gets or sets full type name.
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// Gets or sets method name.
        /// </summary>
        public string MethodName { get; set; }
    }
}