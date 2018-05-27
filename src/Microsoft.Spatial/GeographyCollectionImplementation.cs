//---------------------------------------------------------------------
// <copyright file="GeographyCollectionImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Geography Collection
    /// </summary>
    internal class GeographyCollectionImplementation : GeographyCollection
    {
        /// <summary>
        /// Collection of geography instances
        /// </summary>
        private Geography[] geographyArray;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="geography">Collection of geography instances</param>
        internal GeographyCollectionImplementation(CoordinateSystem coordinateSystem, SpatialImplementation creator, params Geography[] geography)
            : base(coordinateSystem, creator)
        {
            this.geographyArray = geography ?? new Geography[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        /// <param name="geography">Collection of geography instances</param>
        internal GeographyCollectionImplementation(SpatialImplementation creator, params Geography[] geography)
            : this(CoordinateSystem.DefaultGeography, creator, geography)
        {
        }

        /// <summary>
        /// Is Geography Collection Empty
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return this.geographyArray.Length == 0;
            }
        }

        /// <summary>
        /// Geographies
        /// </summary>
        public override ReadOnlyCollection<Geography> Geographies
        {
            get { return new ReadOnlyCollection<Geography>(this.geographyArray); }
        }

        /// <summary>
        /// Sends the current spatial object to the given pipeline
        /// </summary>
        /// <param name="pipeline">The spatial pipeline</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
        public override void SendTo(GeographyPipeline pipeline)
        {
            base.SendTo(pipeline);
            pipeline.BeginGeography(SpatialType.Collection);
            for (int i = 0; i < this.geographyArray.Length; ++i)
            {
                this.geographyArray[i].SendTo(pipeline);
            }

            pipeline.EndGeography();
        }
    }
}
