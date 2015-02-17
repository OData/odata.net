//---------------------------------------------------------------------
// <copyright file="EntitySetPathAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annotation that contains the value of FunctionImport EntitySetPath attribute
    /// </summary>
    public class EntitySetPathAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the EntitySetPathAnnotation class.
        /// </summary>
        /// <param name="entitySetPath">The EntitySetPath value.</param>
        public EntitySetPathAnnotation(string entitySetPath)
        {
            this.EntitySetPath = entitySetPath;
        }

        /// <summary>
        /// Gets the EntitySetPath value
        /// </summary>
        public string EntitySetPath { get; private set; }
    }
}
