//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Reader to be used by spatial formats
    /// </summary>
    /// <typeparam name="TSource">The type of source that the reader operates on.</typeparam>
    internal abstract class SpatialReader<TSource>
    {
        /// <summary>
        /// Creates a reader
        /// </summary>
        /// <param name="destination">the instance of the pipeline that the reader will message while it is reading.</param>
        protected SpatialReader(SpatialPipeline destination)
        {
            Util.CheckArgumentNull(destination, "destination");

            this.Destination = destination;
        }

        /// <summary>
        /// The pipeline that is messaged while the reader is reading.
        /// </summary>
        protected SpatialPipeline Destination { get; set; }

        /// <summary>
        ///   Parses some serialized format that represents one or more Geography spatial values, passing the first one down the pipeline.
        /// </summary>
        /// <exception cref = "ParseErrorException">Throws if the input is not valid. In that case, guarantees that it will not pass anything down the pipeline, or will clear the pipeline by passing down a Reset.</exception>
        /// <param name = "input">The input string</param>
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
        public void ReadGeography(TSource input)
        {
            Util.CheckArgumentNull(input, "input");
            
            try
            {
                this.ReadGeographyImplementation(input);
            }
            catch (Exception ex)
            {
                if (Util.IsCatchableExceptionType(ex))
                {
                    // transform it to a parse exception
                    throw new ParseErrorException(ex.Message, ex);
                }

                throw;
            }
        }

        /// <summary>
        ///   Parses some serialized format that represents one or more Geometry spatial values, passing the first one down the pipeline.
        /// </summary>
        /// <exception cref = "ParseErrorException">Throws if the input is not valid. In that case, guarantees that it will not pass anything down the pipeline, or will clear the pipeline by passing down a Reset.</exception>
        /// <param name = "input">The input string</param>
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
        public void ReadGeometry(TSource input)
        {
            Util.CheckArgumentNull(input, "input");

            try
            {
                this.ReadGeometryImplementation(input);
            }
            catch (Exception ex)
            {
                if (Util.IsCatchableExceptionType(ex))
                {
                    // transform it to a parse exception
                    throw new ParseErrorException(ex.Message, ex);
                }

                throw;
            }
        }   

        /// <summary>
        /// Sets the reader and underlying Destination back to a clean
        /// starting state after an exception
        /// </summary>
        public virtual void Reset()
        {
            ((GeographyPipeline)Destination).Reset();
            ((GeometryPipeline)Destination).Reset();
        }

        /// <summary>
        ///   Parses some serialized format that represents one or more Geometry spatial values, passing the first one down the pipeline.
        /// </summary>
        /// <exception cref = "ParseErrorException">Throws if the input is not valid. In that case, guarantees that it will not pass anything down the pipeline, or will clear the pipeline by passing down a Reset.</exception>
        /// <param name = "input">The input string</param>
        protected abstract void ReadGeometryImplementation(TSource input);

        /// <summary>
        ///   Parses some serialized format that represents one or more Geography spatial values, passing the first one down the pipeline.
        /// </summary>
        /// <exception cref = "ParseErrorException">Throws if the input is not valid. In that case, guarantees that it will not pass anything down the pipeline, or will clear the pipeline by passing down a Reset.</exception>
        /// <param name = "input">The input string</param>
        protected abstract void ReadGeographyImplementation(TSource input);
    }
}
