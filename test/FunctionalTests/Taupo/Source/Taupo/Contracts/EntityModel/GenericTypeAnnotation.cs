//---------------------------------------------------------------------
// <copyright file="GenericTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections.ObjectModel;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents that code generation should create a generic class.
    /// </summary>
    public class GenericTypeAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTypeAnnotation"/> class.
        /// </summary>
        /// <param name="genericTypeParameterNames">The names of the generic type parameters.</param>
        public GenericTypeAnnotation(params string[] genericTypeParameterNames)
        {
            ExceptionUtilities.CheckArgumentNotNull(genericTypeParameterNames, "genericTypeParameterNames");
            ExceptionUtilities.CheckCollectionNotEmpty(genericTypeParameterNames, "genericTypeParameterNames");

            this.TypeParameters = new ReadOnlyCollection<string>(genericTypeParameterNames);
        }

        /// <summary>
        /// Gets the names of the generic type parameters.
        /// </summary>
        public ReadOnlyCollection<string> TypeParameters { get; private set; }
    }
}
