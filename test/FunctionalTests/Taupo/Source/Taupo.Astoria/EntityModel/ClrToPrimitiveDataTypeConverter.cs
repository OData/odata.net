//---------------------------------------------------------------------
// <copyright file="ClrToPrimitiveDataTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Default implementation of the clr to primitive data type converter
    /// </summary>
    [ImplementationName(typeof(IClrToPrimitiveDataTypeConverter), "Default")]
    public class ClrToPrimitiveDataTypeConverter : IClrToPrimitiveDataTypeConverter
    {
        private readonly Dictionary<Type, PrimitiveDataType> clrToEdm = new Dictionary<Type, PrimitiveDataType>();
        private readonly Dictionary<string, Type> edmNameToClr = new Dictionary<string, Type>();
        private bool initialized = false;

        /// <summary>
        /// Gets or sets the spatial clr type resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISpatialClrTypeResolver SpatialResolver { get; set; }

        /// <summary>
        /// Converts the clr type to a primitive data type
        /// </summary>
        /// <param name="clrType">The clr type</param>
        /// <returns>A primitive data type that corresponds to the clr type</returns>
        public PrimitiveDataType ToDataType(Type clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");
            this.Initialize();

            PrimitiveDataType dataType;
            if (!this.clrToEdm.TryGetValue(clrType, out dataType))
            {
                dataType = null;

                Type currentMatch = typeof(object);
                foreach (var pair in this.clrToEdm)
                {
                    if (pair.Key.IsAssignableFrom(clrType) && currentMatch.IsAssignableFrom(pair.Key))
                    {
                        // move the current match down the hierarchy so that 
                        // we know we will get the most derived type
                        currentMatch = pair.Key;
                        dataType = pair.Value;
                    }
                }
            }

            return dataType;
        }

        /// <summary>
        /// Converts the primitive data type to a clr type
        /// </summary>
        /// <param name="dataType">The data type</param>
        /// <returns>The clr type that corresponds to the primitive data type</returns>
        public Type ToClrType(PrimitiveDataType dataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");
            return this.ToClrType(EdmDataTypes.GetEdmFullName(dataType));
        }

        /// <summary>
        /// Gets the clr type that corresponds to the edm primitive data type name
        /// </summary>
        /// <param name="edmTypeName">The edm name of the data type</param>
        /// <returns>The clr type</returns>
        public Type ToClrType(string edmTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(edmTypeName, "edmTypeName");
            this.Initialize();

            Type clrType;
            if (!this.edmNameToClr.TryGetValue(edmTypeName, out clrType))
            {
                return null;
            }

            return clrType;
        }

        /// <summary>
        /// Initializes the converter
        /// </summary>
        internal void Initialize()
        {
            if (!this.initialized)
            {
                ExceptionUtilities.CheckAllRequiredDependencies(this);
                this.initialized = true;

                var types = EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.Latest).ToList();

                foreach (PrimitiveDataType type in types)
                {
                    string edmTypeName = EdmDataTypes.GetEdmFullName(type);

                    Type clrType;
                    var spatialType = type as SpatialDataType;
                    if (spatialType != null)
                    {
                        clrType = this.SpatialResolver.GetClrType(spatialType);
                    }
                    else
                    {
                        clrType = type.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                    }

                    ExceptionUtilities.CheckObjectNotNull(clrType, "Could not determine clr type for edm data type: '{0}", edmTypeName);

                    this.clrToEdm[clrType] = type;
                    this.edmNameToClr[edmTypeName] = clrType;
                }
            }
        }
    }
}