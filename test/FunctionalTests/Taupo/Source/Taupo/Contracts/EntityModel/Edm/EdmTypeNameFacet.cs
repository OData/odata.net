//---------------------------------------------------------------------
// <copyright file="EdmTypeNameFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// EDM Type Name.
    /// </summary>
    public class EdmTypeNameFacet : PrimitiveDataTypeFacet<string>
    {
        /// <summary>
        /// Initializes a new instance of the EdmTypeNameFacet class.
        /// </summary>
        /// <param name="edmTypeName">Name of the EDM type.</param>
        public EdmTypeNameFacet(string edmTypeName)
            : base(edmTypeName)
        {
        }
    }
}
