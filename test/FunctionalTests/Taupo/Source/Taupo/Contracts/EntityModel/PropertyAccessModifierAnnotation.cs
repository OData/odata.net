//---------------------------------------------------------------------
// <copyright file="PropertyAccessModifierAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annotation for specifying visibility on property getter to be used for codegen
    /// </summary>
    public class PropertyAccessModifierAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessModifierAnnotation"/> class.
        /// </summary>
        /// <param name="setterAccessModifier">The setter visibility.</param>
        /// <param name="getterAccessModifier">The getter visibility.</param>
        public PropertyAccessModifierAnnotation(AccessModifier setterAccessModifier, AccessModifier getterAccessModifier)
        {
            this.GetterAccessModifier = getterAccessModifier;
            this.SetterAccessModifier = setterAccessModifier;
        }

        /// <summary>
        /// Gets the visibility on property setter
        /// </summary>
        public AccessModifier SetterAccessModifier { get; private set; }

        /// <summary>
        /// Gets the visibility on property getter
        /// </summary>
        public AccessModifier GetterAccessModifier { get; private set; }
    }
}
