//---------------------------------------------------------------------
// <copyright file="DataServiceProtocolVersion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Services.Common
{
    /// <summary>Represents the versions of the Open Data Protocol (OData) that the data service may support. </summary>
    public enum DataServiceProtocolVersion
    {
        /// <summary>Version 1 </summary>
        V1, 

        /// <summary>Version 2</summary>
        V2,

        /// <summary>Version 3</summary>
        V3
    }
}
