//---------------------------------------------------------------------
// <copyright file="ODataParameters.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Test OM representing a parameters payload.
    /// </summary>
    /// <remarks>
    /// There is no OData OM which represents a parameters payload since parameters are just name value pairs.
    /// We will use this type in places where we would read the payload into the product OM or to write the
    /// product OM to a payload.
    /// </remarks>
    public class ODataParameters : List<KeyValuePair<string, object>>
    {
    }
}
