//---------------------------------------------------------------------
// <copyright file="SpatialPrimitiveFormatterBase`3.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Common
{
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Base implementation of the spatial-primitive-formatter contract which performs some common logic by wrapping
    /// a particular Microsoft.Spatial.SpatialFormat implementation
    /// </summary>
    /// <typeparam name="TFormatted">The type for the formatted representation</typeparam>
    /// <typeparam name="TReadStream">The type expected by the wrapped format when parsing</typeparam>
    /// <typeparam name="TWriteStream">The writer target type of the wrapped format</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Last three are needed for the product")]
    public abstract class SpatialPrimitiveFormatterBase<TFormatted, TReadStream, TWriteStream> : ISpatialPrimitiveFormatter<TFormatted>
    {
        /// <summary>
        /// Gets the spatial format to use
        /// </summary>
        internal SpatialFormatter<TReadStream, TWriteStream> Formatter 
        {
            get
            {
                return this.CreateFormatter();
            }
        }

        /// <summary>
        /// Tries to convert the given spatial instance to the formatted representation
        /// </summary>
        /// <param name="spatialInstance">The spatial instance to format</param>
        /// <param name="formatted">The formatted spatial value</param>
        /// <returns>True if the value was spatial and could be formatted, otherwise false</returns>
        public bool TryConvert(object spatialInstance, out TFormatted formatted)
        {
            var spatial = spatialInstance as ISpatial;
            if (spatial != null)
            {
                formatted = this.WriteFormattedValue(spatial);
                return true;
            }

            formatted = default(TFormatted);
            return false;
        }

        /// <summary>
        /// Tries to parse the given formatted spatial representation.
        /// </summary>
        /// <param name="formatted">The formatted representation</param>
        /// <param name="expectedSpatialType">The expected spatial type.</param>
        /// <param name="spatialInstance">The parsed spatial instance</param>
        /// <returns>True if the value could be parsed, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exceptions, as the value may not actually be spatial")]
        public bool TryParse(TFormatted formatted, SpatialTypeKind? expectedSpatialType, out object spatialInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(formatted, "formatted");

            if (this.IsDefinitelyNotSpatial(formatted))
            {
                spatialInstance = null;
                return false;
            }

            var prepared = this.PrepareFormattedValueForParsing(formatted);
            if (expectedSpatialType == SpatialTypeKind.Geography)
            {
                return TryParse<Geography>(prepared, out spatialInstance);
            }
            else if (expectedSpatialType == SpatialTypeKind.Geometry)
            {
                return TryParse<Geometry>(prepared, out spatialInstance);
            }
            else
            {
                // try geometry first because it does not have value restrictions like geography does
                return TryParse<Geometry>(prepared, out spatialInstance) || TryParse<Geography>(this.PrepareFormattedValueForParsing(formatted), out spatialInstance);
            }
        }
        
        /// <summary>
        /// Prepared a formatted value to be parsed by the wrapped format
        /// Typically used to convert from string to TextReader or other similar transformations.
        /// </summary>
        /// <param name="formatted">The formatted value</param>
        /// <returns>The prepared value to pass into the wrapped format</returns>
        protected internal abstract TReadStream PrepareFormattedValueForParsing(TFormatted formatted);

        /// <summary>
        /// Calls Write on the Formatter, and converts the write stream to the desired type
        /// Typically used to convert from XmlWriter to XElement or other similar transformations.
        /// </summary>
        /// <param name="spatialInstance">The spatial instance to write.</param>
        /// <returns>The format of the correct data type.</returns>
        protected internal abstract TFormatted WriteFormattedValue(ISpatial spatialInstance);

        /// <summary>
        /// Create an instance of the spatial formatter to use
        /// </summary>
        /// <returns>The spatial format to use</returns>
        protected abstract SpatialFormatter<TReadStream, TWriteStream> CreateFormatter();

        /// <summary>
        /// Determines whether the formatted value cannot possibly be spatial.
        /// </summary>
        /// <param name="formatted">The formatted value.</param>
        /// <returns>true if it is definitely not spatial</returns>
        protected virtual bool IsDefinitelyNotSpatial(TFormatted formatted)
        {
            return false;
        }

        private bool TryParse<TSpatial>(TReadStream prepared, out object spatialInstance) where TSpatial : class, ISpatial
        {
            try
            {
                spatialInstance = this.Formatter.Read<TSpatial>(prepared);
                return true;
            }
            catch (ParseErrorException)
            {
                spatialInstance = null;
                return false;
            }
        }
    }
}
