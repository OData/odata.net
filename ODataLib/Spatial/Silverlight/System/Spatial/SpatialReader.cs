//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Spatial
{
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif

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
