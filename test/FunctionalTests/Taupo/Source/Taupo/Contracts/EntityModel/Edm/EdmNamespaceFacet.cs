//---------------------------------------------------------------------
// <copyright file="EdmNamespaceFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Namespace of EDM Type.
    /// </summary>
    public class EdmNamespaceFacet : PrimitiveDataTypeFacet<string>
    {
        /// <summary>
        /// Initializes a new instance of the EdmNamespaceFacet class.
        /// </summary>
        /// <param name="edmNamespace">The edm namespace.</param>
        public EdmNamespaceFacet(string edmNamespace)
            : base(edmNamespace)
        {
        }
    }
}
