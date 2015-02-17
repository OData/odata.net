//---------------------------------------------------------------------
// <copyright file="IShapeProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Provides access to the constructed geography or geometry.</summary>
    public interface IShapeProvider : IGeographyProvider, IGeometryProvider
    {
    }
}
