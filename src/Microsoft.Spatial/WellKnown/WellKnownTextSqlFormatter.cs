//---------------------------------------------------------------------
// <copyright file="WellKnownTextSqlFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.IO;

    /// <summary>
    /// The object to move spatial types to and from the WellKnownTextSql format
    /// </summary>
    public abstract class WellKnownTextSqlFormatter : SpatialFormatter<TextReader, TextWriter>
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.WellKnownTextSqlFormatter" /> class.</summary>
        /// <param name="creator">The implementation that created this instance.</param>
        protected WellKnownTextSqlFormatter(SpatialImplementation creator) : base(creator)
        {
        }

        /// <summary>Creates the implementation of the formatter.</summary>
        /// <returns>Returns the created WellKnownTextSqlFormatter implementation.</returns>
        public static WellKnownTextSqlFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter();
        }

        /// <summary>Creates the implementation of the formatter and checks whether the specified formatter has Z.</summary>
        /// <returns>The created WellKnownTextSqlFormatter.</returns>
        /// <param name="allowOnlyTwoDimensions">Restricts the formatter to allow only two dimensions.</param>
        public static WellKnownTextSqlFormatter Create(bool allowOnlyTwoDimensions)
        {
            return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(allowOnlyTwoDimensions);
        }
    }
}
