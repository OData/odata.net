//---------------------------------------------------------------------
// <copyright file="SpatialDataGeneratorResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;    
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.DataGeneration;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Default implementation of the ISpatialDataGeneratorResolver contract
    /// </summary>
    [ImplementationName(typeof(ISpatialDataGeneratorResolver), "Default")]
    public class SpatialDataGeneratorResolver : ISpatialDataGeneratorResolver
    {
        /// <summary>
        /// Gets or sets the well-known-text formatter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IWellKnownTextSpatialFormatter WellKnownTextFormatter { get; set; }

        /// <summary>
        /// Finds and returns the data generator for given Spatial Data type
        /// </summary>
        /// <param name="spatialType">the spatial data type to get data generator for</param>
        /// <param name="isUnique">whether the generator should produce unique data.</param>
        /// <param name="random">The random number generator.</param>
        /// <param name="hints">The data generation hints.</param>
        /// <returns>the data generator for given spatial type</returns>
        public IDataGenerator GetDataGenerator(SpatialDataType spatialType, bool isUnique, IRandomNumberGenerator random, params DataGenerationHint[] hints)
        {
            ExceptionUtilities.CheckArgumentNotNull(spatialType, "spatialType");

            string dataTypeName = spatialType.GetFacetValue<EdmTypeNameFacet, string>(string.Empty);
            var shapeKind = GetHintValue<SpatialShapeKindHint, SpatialShapeKind>(SpatialShapeKind.Unspecified, hints);
            if (shapeKind != SpatialShapeKind.Unspecified)
            {
                dataTypeName = shapeKind.ToString();
            }

            ExceptionUtilities.Assert(
                !string.IsNullOrEmpty(dataTypeName),
                "the data type is not supported as spatial data type.");

            string defaultSrid;
            SpatialTypeKind kind;
            if (dataTypeName.StartsWith("Geom", StringComparison.OrdinalIgnoreCase))
            {
                defaultSrid = SpatialConstants.DefaultSridForGeometry;
                kind = SpatialTypeKind.Geometry;
            }
            else
            {
                defaultSrid = SpatialConstants.DefaultSridForGeography;
                kind = SpatialTypeKind.Geography;
            }

            bool isNullable = spatialType.IsNullable && !hints.OfType<NoNullsHint>().Any();

            return this.BuildGenerator(defaultSrid, dataTypeName, kind, isUnique, random, isNullable);
        }

        /// <summary>
        /// Returns a set of available data for the targeted spatial type, for example, LINESTRING as geometry in SRID 0.
        /// </summary>
        /// <param name="fileName">the local file name containing the available data in embedded resource</param>
        /// <returns>the list of strings as available data.</returns>
        private static IEnumerable<string> ReadTargetedDataSet(string fileName)
        {
            string commentMarker = "#";
            IList<string> dataStrings = new List<string>();
            Assembly currentAssembly = typeof(SpatialDataGeneratorResolver).GetAssembly();
            string resourceName = currentAssembly.GetManifestResourceNames().Where(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            ExceptionUtilities.CheckObjectNotNull(resourceName, "Cannot find " + fileName + " in embeded resources.");
            using (StreamReader dataReader = new StreamReader(currentAssembly.GetManifestResourceStream(resourceName)))
            {
                string dataLine;
                while ((dataLine = dataReader.ReadLine()) != null)
                {
                    if (!dataLine.StartsWith(commentMarker, StringComparison.OrdinalIgnoreCase))
                    {
                        dataStrings.Add(dataLine);
                    }
                }
            }

            return dataStrings.Where(s => !string.IsNullOrEmpty(s.Trim()));
        }

        /// <summary>
        /// Gets the data generation hint value or returns a default value if the specified hint was not found.
        /// </summary>
        /// <typeparam name="THint">The type of the hint.</typeparam>
        /// <typeparam name="TValue">The type of the hint value.</typeparam>
        /// <param name="defaultValue">The default value to return if the hint is not found.</param>
        /// <param name="hints">available hint types to get from.</param>
        /// <returns>Hint value or default.</returns>
        private static TValue GetHintValue<THint, TValue>(TValue defaultValue, params DataGenerationHint[] hints)
            where THint : DataGenerationHint<TValue>
        {
            THint hint = hints.OfType<THint>().SingleOrDefault();

            if (hint != null)
            {
                return hint.Value;
            }

            return defaultValue;
        }

        private IEnumerable<ISpatial> BuildTargetedDataSet(string srid, string commonTypeName, SpatialTypeKind kind)
        {
            string fileName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.txt", commonTypeName.ToUpperInvariant(), srid);
            var data = ReadTargetedDataSet(fileName);

            return data.Select(s => this.WellKnownTextFormatter.Parse(kind, s)).Cast<ISpatial>();
        }

        private IDataGenerator<ISpatial> BuildGenerator(string srid, string commonTypeName, SpatialTypeKind kind, bool isUnique, IRandomNumberGenerator random, bool isNullable)
        {
            var availableData = this.BuildTargetedDataSet(srid, commonTypeName, kind).ToList();
            if (isNullable)
            {
                availableData.Add(null);
            }

            return new FixedSetDataGenerator<ISpatial>(random, !isUnique, availableData);
        }
    }
}