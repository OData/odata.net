//---------------------------------------------------------------------
// <copyright file="ISpatialPrimitiveTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Threading.Tasks;
    using Microsoft.OData.Json;
    #endregion

    /// <summary>
    /// Marker interface to enable targeted registration, resolution, or swapping of spatial converters.
    /// </summary>
    public interface ISpatialPrimitiveTypeConverter : IPrimitiveTypeConverter
    {
    }
}
