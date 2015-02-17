//---------------------------------------------------------------------
// <copyright file="IGeometryToGeographyConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Interface for converting an instance of a geometry type into a geography type
    /// </summary>
    [ImplementationSelector("GeometryToGeographyConverter", DefaultImplementation = "Default")]
    public interface IGeometryToGeographyConverter
    {
        /// <summary>
        /// Converts the given geometry into a geography.
        /// </summary>
        /// <param name="geometry">The geometry to convert.</param>
        /// <param name="treatXCoordinateAsLatitude">if set to <c>true</c> the x coordinate should be converted to latitude. Otherwise, it should be longitude.</param>
        /// <returns>The converted geography</returns>
        object Convert(object geometry, bool treatXCoordinateAsLatitude);
    }
}
