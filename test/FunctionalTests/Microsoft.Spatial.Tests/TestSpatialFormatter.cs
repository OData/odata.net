//---------------------------------------------------------------------
// <copyright file="TestSpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Data.Spatial;

namespace Microsoft.Spatial.Tests
{
    internal class TestSpatialFormatter<TParseParameter, TWriterTarget> : SpatialFormatter<TParseParameter, TWriterTarget>
    {
        public TestSpatialFormatter() : base(new DataServicesSpatialImplementation())
        {
        }

        public Func<TWriterTarget, SpatialPipeline> CreateWriterFunc { get; set; }

        public Action<TParseParameter, SpatialPipeline> ReadGeometryAction { get; set; }

        public Action<TParseParameter, SpatialPipeline> ReadGeographyAction { get; set; }

        protected override void ReadGeography(TParseParameter readerStream, SpatialPipeline pipeline)
        {
            if (this.ReadGeographyAction != null)
            {
                this.ReadGeographyAction(readerStream, pipeline);
                return;
            }

            throw new NotImplementedException();
        }

        protected override void ReadGeometry(TParseParameter readerStream, SpatialPipeline pipeline)
        {
            if (this.ReadGeometryAction != null)
            {
                this.ReadGeometryAction(readerStream, pipeline);
                return;
            }

            throw new NotImplementedException();
        }

        public override SpatialPipeline CreateWriter(TWriterTarget target)
        {
            if (this.CreateWriterFunc != null)
            {
                return this.CreateWriterFunc(target);
            }

            throw new NotImplementedException();
        }
    }
}
