//---------------------------------------------------------------------
// <copyright file="GenericPropertyTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// An annotation applied to properties that allows them to be typed as
    /// a generic type parameter instead of as an actual data type.
    /// </summary>
    public class GenericPropertyTypeAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericPropertyTypeAnnotation"/> class.
        /// </summary>
        /// <param name="genericTypeParameterName">The name of the generic type parameter used as the type of the property.</param>
        public GenericPropertyTypeAnnotation(string genericTypeParameterName)
        {
            this.GenericTypeParameterName = genericTypeParameterName;
        }

        /// <summary>
        /// Gets the name of the generic type parameter used as
        /// the type for the property.
        /// </summary>
        public string GenericTypeParameterName { get; private set; }
    }
}
