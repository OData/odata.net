//---------------------------------------------------------------------
// <copyright file="TypeAccessModifierAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotation for specifying visibility on EntityType, ComplexType, EntityContainer
    /// </summary>
    public class TypeAccessModifierAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAccessModifierAnnotation"/> class.
        /// </summary>
        /// <param name="typeAccessModifier">The type visibility.</param>
        public TypeAccessModifierAnnotation(AccessModifier typeAccessModifier)
        {
            ExceptionUtilities.Assert(typeAccessModifier == AccessModifier.Public || typeAccessModifier == AccessModifier.Internal || typeAccessModifier == AccessModifier.Unspecified, "Cannot have AccessModiier " + typeAccessModifier.ToString() + " on typeAccess");
            this.TypeAccessModifier = typeAccessModifier;
        }

        /// <summary>
        /// Gets the visibility on type
        /// </summary>
        public AccessModifier TypeAccessModifier { get; private set; }
    }
}
