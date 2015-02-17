//---------------------------------------------------------------------
// <copyright file="StubEdmTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Stub implementation of EdmComplexType
    /// </summary>
    public class StubEdmTypeReference : StubEdmElement, IEdmTypeReference
    {
        /// <summary>
        /// Gets or sets a value indicating whether the type is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the definition of this type refererence.
        /// </summary>
        public IEdmType Definition { get; set; }
    }
}
