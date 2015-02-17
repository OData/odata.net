//---------------------------------------------------------------------
// <copyright file="WellKnownTextSpatialEqualityComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial
{
    using System.Collections.Generic;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Default implementation of the spatial equality comparer contract which uses well-known-text string comparison
    /// </summary>
    [ImplementationName(typeof(ISpatialEqualityComparer), "WellKnownText")]
    public class WellKnownTextSpatialEqualityComparer : EqualityComparer<object>, ISpatialEqualityComparer
    {
        /// <summary>
        /// Gets or sets the well-known-text formatter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IWellKnownTextSpatialFormatter WellKnownTextFormatter { get; set; }

        /// <summary>
        /// Performs an equality comparison of two spatial objects based on well-known-text representation
        /// </summary>
        /// <param name="x">The first spatial object</param>
        /// <param name="y">The second spatial object</param>
        /// <returns>A value indicating whether the given spatial objects are equivalent</returns>
        public override bool Equals(object x, object y)
        {
            if (x == null)
            {
                return y == null;
            }
            else if (y == null)
            {
                return false;
            }

            var coordinateSystem1 = x as CoordinateSystem;
            if (coordinateSystem1 != null)
            {
                return coordinateSystem1.Equals(y);
            }

            return this.Convert(x) == this.Convert(y);
        }

        /// <summary>
        /// Gets the hash code of the given spatial object after formatting as well-known-text
        /// </summary>
        /// <param name="obj">The spatial object</param>
        /// <returns>The hashcode of the well-known-text representation</returns>
        public override int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (obj is CoordinateSystem)
            {
                return obj.GetHashCode();
            }

            var converted = this.Convert(obj);
            return converted.GetHashCode();
        }

        private string Convert(object spatialObject)
        {
            ExceptionUtilities.CheckObjectNotNull(this.WellKnownTextFormatter, "Cannot convert spatial object without a formatter dependency being set");
            return this.WellKnownTextFormatter.Convert(spatialObject);
        }
    }
}
