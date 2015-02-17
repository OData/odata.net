//---------------------------------------------------------------------
// <copyright file="ODataUriSegmentPathCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a collection of segment paths as seen in the '$select' and '$expand' query options
    /// </summary>
    public class ODataUriSegmentPathCollection : Collection<ICollection<ODataUriSegment>>
    {
        /// <summary>
        /// Initializes a new instance of the ODataUriSegmentPathCollection class
        /// </summary>
        internal ODataUriSegmentPathCollection()
        {
        }
    }
}
