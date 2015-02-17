//---------------------------------------------------------------------
// <copyright file="MethodAccessModifierAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annotation for specifying visibility on function
    /// </summary>
    public class MethodAccessModifierAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodAccessModifierAnnotation"/> class.
        /// </summary>
        /// <param name="methodAccessModifier">The Method visibility.</param>
        public MethodAccessModifierAnnotation(AccessModifier methodAccessModifier)
        {
            this.MethodAccessModifier = methodAccessModifier;
        }

        /// <summary>
        /// Gets the visibility on Method
        /// </summary>
        public AccessModifier MethodAccessModifier { get; private set; }
    }
}
