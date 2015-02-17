//---------------------------------------------------------------------
// <copyright file="EdmDataTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Resolves data type specifications into EDM Data Types.
    /// </summary>
    [ImplementationName(typeof(IPrimitiveDataTypeResolver), "Default")]
    public class EdmDataTypeResolver : IPrimitiveDataTypeResolver
    {
        private static Dictionary<int, PrimitiveDataType> integerSizesToType = new Dictionary<int, PrimitiveDataType>()
        {
            { 8, EdmDataTypes.Byte },
            { 16, EdmDataTypes.Int16 },
            { 32, EdmDataTypes.Int32 },
            { 64, EdmDataTypes.Int64 },
        };

        private static Dictionary<int, PrimitiveDataType> floatingPointSizesToType = new Dictionary<int, PrimitiveDataType>()
        {
            { 32, EdmDataTypes.Single },
            { 64, EdmDataTypes.Double },
        };

        private static Dictionary<Type, PrimitiveDataType> clrTypeToEdmDataType = new Dictionary<Type, PrimitiveDataType>()
        {
            { typeof(byte), EdmDataTypes.Byte },
            { typeof(sbyte), EdmDataTypes.SByte },
            { typeof(short), EdmDataTypes.Int16 },
            { typeof(int), EdmDataTypes.Int32 },
            { typeof(long), EdmDataTypes.Int64 },
            { typeof(float), EdmDataTypes.Single },
            { typeof(double), EdmDataTypes.Double },
            { typeof(Guid), EdmDataTypes.Guid },
        };

        private static Dictionary<string, PrimitiveDataType> edmTypeNamesToType = new Dictionary<string, PrimitiveDataType>(StringComparer.OrdinalIgnoreCase)
        {
            { "Single", EdmDataTypes.Single },
            { "Double", EdmDataTypes.Double },
        };

        private EdmTypeResolvingVisitor visitor;

        /// <summary>
        /// Initializes a new instance of the EdmDataTypeResolver class.
        /// </summary>
        public EdmDataTypeResolver()
        {
            this.visitor = new EdmTypeResolvingVisitor(this);
        }

        /// <summary>
        /// Gets or sets the spatial data type definition resolver.
        /// </summary>
        [InjectDependency]
        public ISpatialDataTypeDefinitionResolver SpatialDefinitionResolver { get; set; }

        /// <summary>
        /// Resolves the primitive type specification into EDM type.
        /// </summary>
        /// <param name="dataTypeSpecification">The data type specification.</param>
        /// <returns>Fully resolved data type.</returns>
        public PrimitiveDataType ResolvePrimitiveType(PrimitiveDataType dataTypeSpecification)
        {
            var resolvedType = this.visitor.ResolvePrimitiveType(dataTypeSpecification);

            // apply original nullable flag from specification
            return resolvedType.Nullable(dataTypeSpecification.IsNullable);
        }

        /// <summary>
        /// Visitor which transforms data type specifications into fully resolved EDM types.
        /// </summary>
        private class EdmTypeResolvingVisitor : IPrimitiveDataTypeVisitor<PrimitiveDataType>
        {
            private readonly EdmDataTypeResolver parent;

            /// <summary>
            /// Initializes a new instance of the EdmTypeResolvingVisitor class.
            /// </summary>
            /// <param name="parent">the parent edm data type resolver.</param>
            public EdmTypeResolvingVisitor(EdmDataTypeResolver parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Resolves the primitive type into EDM primitive type.
            /// </summary>
            /// <param name="dataTypeSpecification">The data type specification.</param>
            /// <returns>Resolved EDM primitive type.</returns>
            public PrimitiveDataType ResolvePrimitiveType(PrimitiveDataType dataTypeSpecification)
            {
                PrimitiveDataType dataType;

                if (dataTypeSpecification is EnumDataType)
                {
                    dataType = dataTypeSpecification.Accept(this);
                }
                else
                {
                    Type clrType = dataTypeSpecification.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                    if (clrType == null || !clrTypeToEdmDataType.TryGetValue(clrType, out dataType))
                    {
                        string typeName = dataTypeSpecification.GetFacetValue<EdmTypeNameFacet, string>(null);
                        if (typeName == null || !edmTypeNamesToType.TryGetValue(typeName, out dataType))
                        {
                            dataType = dataTypeSpecification.Accept(this);
                        }
                    }

                    // Preserve facet for unqualified database type name
                    string unqualifiedDatabaseTypeName = dataTypeSpecification.GetFacetValue<UnqualifiedDatabaseTypeNameFacet, string>(null);
                    if (!string.IsNullOrEmpty(unqualifiedDatabaseTypeName))
                    {
                        dataType = dataType.WithFacet(new UnqualifiedDatabaseTypeNameFacet(unqualifiedDatabaseTypeName));
                    }
                }

                return dataType;
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(BinaryDataType dataType)
            {
                int? maxLength = null;
                MaxLengthFacet maxLengthFacet;

                if (dataType.TryGetFacet(out maxLengthFacet))
                {
                    maxLength = maxLengthFacet.Value;
                }

                return EdmDataTypes.Binary(maxLength);
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(BooleanDataType dataType)
            {
                return EdmDataTypes.Boolean;
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(DateTimeDataType dataType)
            {
                int? precision = null;

                TimePrecisionFacet timePrecisionFacet;

                if (dataType.TryGetFacet(out timePrecisionFacet))
                {
                    precision = timePrecisionFacet.Value;
                }

                string typeName = dataType.GetFacetValue<EdmTypeNameFacet, string>(null);
                bool hasTimeZoneOffsetFacet = dataType.GetFacetValue<HasTimeZoneOffsetFacet, bool>(false);
                Type clrType = dataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                bool hasDateTimeOffsetEdmTypeNameFacet = "DateTimeOffset".Equals(typeName, StringComparison.OrdinalIgnoreCase);

                if (hasTimeZoneOffsetFacet && typeName != null && !hasDateTimeOffsetEdmTypeNameFacet)
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Cannot resolve date time data type as it has 'true' value for the '{0}' which contradicts with the edm type name '{1}'", typeof(HasTimeZoneOffsetFacet).Name, typeName));
                }

                if (hasTimeZoneOffsetFacet || hasDateTimeOffsetEdmTypeNameFacet || clrType == typeof(DateTimeOffset))
                {
                    return EdmDataTypes.DateTimeOffset(precision);
                }

                if (typeName != null && !"DateTime".Equals(typeName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Edm type name '{0}' is invalid for date time data types.", typeName));
                }

                return EdmDataTypes.DateTime(precision);
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(EnumDataType dataType)
            {
                // Notes: 
                //      1. Do not add facets for Edm namespace/type name because:
                //         - enum types are defined by user and are not Edm built-in types
                //         - we should not have namespace/type name in multiple places (i.e. facets and in EnumDataType.Definition) as it can get out of sync
                //         Use EdmDataTypes.GetEdmFullName/GetEdmName/GetEdmNamespace to get full name for any data type including enums
                //      2. Do not add facet for Clr primitive type because it's not clear which type we should put: underlying type or actual Clr enum type:
                //         - actual Clr enum type makes sense only in the object layer, while in the conceptual layer underlying type is used
                //         - at this point we don't have actual Clr type
                //      3. Do we need to clear facets for primitive Clr type, Edm namespace and Emd type name?
                //         For now we leave whatever facets user specified explicitly
                return dataType;
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(FixedPointDataType dataType)
            {
                int? precision = null;
                int? scale = null;

                NumericPrecisionFacet precisionFacet;
                NumericScaleFacet scaleFacet;

                if (dataType.TryGetFacet(out precisionFacet))
                {
                    precision = precisionFacet.Value;
                }

                if (dataType.TryGetFacet(out scaleFacet))
                {
                    scale = scaleFacet.Value;
                }

                return EdmDataTypes.Decimal(precision, scale);
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(FloatingPointDataType dataType)
            {
                int typeSize = dataType.GetFacetValue<TypeSizeFacet, int>(64);
                PrimitiveDataType result;

                if (!floatingPointSizesToType.TryGetValue(typeSize, out result))
                {
                    throw new TaupoNotSupportedException("Floating point data types with " + typeSize + " bits are not supported by EDM.");
                }

                return result;
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(GuidDataType dataType)
            {
                return EdmDataTypes.Guid;
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(IntegerDataType dataType)
            {
                int typeSize = dataType.GetFacetValue<TypeSizeFacet, int>(32);
                PrimitiveDataType result;

                if (!integerSizesToType.TryGetValue(typeSize, out result))
                {
                    throw new TaupoNotSupportedException("Integers with " + typeSize + " bits are not supported by EDM.");
                }

                return result;
            }

            /// <summary>
            /// Resolves the specified data type specification into EDM data type.
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Implementation-specific value.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(SpatialDataType dataType)
            {
                ExceptionUtilities.CheckObjectNotNull(this.parent.SpatialDefinitionResolver, "Cannot resolve full type definition data type. {0} is not provided.", typeof(ISpatialDataTypeDefinitionResolver));
                return this.parent.SpatialDefinitionResolver.ResolveFullDefinition(dataType);
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(StreamDataType dataType)
            {
                return EdmDataTypes.Stream;
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(StringDataType dataType)
            {
                int? maxLength = null;
                bool? isUnicode = null;
                MaxLengthFacet maxLengthFacet;
                IsUnicodeFacet isUnicodeFacet;

                if (dataType.TryGetFacet(out maxLengthFacet))
                {
                    maxLength = maxLengthFacet.Value;
                }

                if (dataType.TryGetFacet(out isUnicodeFacet))
                {
                    isUnicode = isUnicodeFacet.Value;
                }

                return EdmDataTypes.String(maxLength, isUnicode);
            }

            /// <summary>
            /// Resolves the specified data type.
            /// </summary>
            /// <param name="dataType">Data type specification.</param>
            /// <returns>Resolved EDM Type.</returns>
            PrimitiveDataType IPrimitiveDataTypeVisitor<PrimitiveDataType>.Visit(TimeOfDayDataType dataType)
            {
                int? precision = null;
                TimePrecisionFacet timePrecisionFacet;

                if (dataType.TryGetFacet(out timePrecisionFacet))
                {
                    precision = timePrecisionFacet.Value;
                }

                return EdmDataTypes.Time(precision);
            }
        }
    }
}
