//---------------------------------------------------------------------
// <copyright file="DerivedPhoto.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{

    /// <summary>
    /// Type that derives from the Photo entity type and should also be treated as though it has a [HasStreamAttribute] on it. 
    /// </summary>
    public class DerivedFromPhoto : Photo
    {
        public string PropertyOnDerivedType { get; set; }
    }
}
