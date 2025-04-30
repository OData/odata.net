//---------------------------------------------------------------------
// <copyright file="StubEdmType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.StubEdm;

/// <summary>
/// Stub implementation of IEdmType
/// </summary>
public class StubEdmType : StubEdmElement, IEdmType
{
    /// <summary>
    /// Initializes a new instance of the StubEdmType class.
    /// </summary>
    public StubEdmType()
    {
    }

    /// <summary>
    /// Gets the type kind
    /// </summary>
    public EdmTypeKind TypeKind
    {
        get { return EdmTypeKind.None; }
    }
}
