//---------------------------------------------------------------------
// <copyright file="TaupoToEdmPrimitiveDataTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib
{
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Converts primitive data type from Taupo term into Edm term
    /// </summary>
    public sealed class TaupoToEdmPrimitiveDataTypeConverter : IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>
    {
        /// <summary>
        /// Converts primitive data type from Taupo term into Edm term
        /// </summary>
        /// <param name="dataType">The primitive DataType (Taupo term)</param>
        /// <returns>The primitive TypeReference (Edm term)</returns>
        public IEdmPrimitiveTypeReference ConvertToEdmTypeReference(PrimitiveDataType dataType)
        {
            return dataType.Accept(this);
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(BinaryDataType dataType)
        {
            IEdmPrimitiveType typeDefinition = this.GetEdmTypeDefinition(dataType);

            int? maxLength = null;
            if (dataType.HasFacet<MaxLengthFacet>())
            {
                maxLength = dataType.GetFacet<MaxLengthFacet>().Value;
            }

            var typeReference = new EdmBinaryTypeReference(
                typeDefinition,
                dataType.IsNullable,
                false,
                maxLength);

            return typeReference;
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(BooleanDataType dataType)
        {
            return this.GetFacetlessEdmTypeReference(dataType);
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(DateTimeDataType dataType)
        {
            return this.GetTemporalTypeReference(dataType);
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(FixedPointDataType dataType)
        {
            IEdmPrimitiveType typeDefinition = this.GetEdmTypeDefinition(dataType);

            int? precision = null, scale = null;
            if (dataType.HasFacet<NumericPrecisionFacet>())
            {
                precision = dataType.GetFacet<NumericPrecisionFacet>().Value;
            }

            if (dataType.HasFacet<NumericScaleFacet>())
            {
                scale = dataType.GetFacet<NumericScaleFacet>().Value;
            }

            var typeReference = new EdmDecimalTypeReference(
                typeDefinition,
                dataType.IsNullable,
                precision,
                scale);

            return typeReference;
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(FloatingPointDataType dataType)
        {
            return this.GetFacetlessEdmTypeReference(dataType);
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(GuidDataType dataType)
        {
            return this.GetFacetlessEdmTypeReference(dataType);
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(IntegerDataType dataType)
        {
            return this.GetFacetlessEdmTypeReference(dataType);
        }
        
        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(SpatialDataType dataType)
        {
            IEdmPrimitiveType typeDefinition = this.GetEdmTypeDefinition(dataType);
            return new EdmSpatialTypeReference(typeDefinition, dataType.IsNullable, null);
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(StreamDataType dataType)
        {
            IEdmPrimitiveType typeDefinition = this.GetEdmTypeDefinition(dataType);
            
            var typeReference = new EdmPrimitiveTypeReference(
                typeDefinition, 
                dataType.IsNullable);

            return typeReference;
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(EnumDataType dataType)
        {
            // TODO: enum
            throw new System.NotImplementedException();
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(StringDataType dataType)
        {
            IEdmPrimitiveType typeDefinition = this.GetEdmTypeDefinition(dataType);

            bool isUnicode = dataType.GetFacetValue<IsUnicodeFacet, bool>(true);

            int? maxLength = null;
            if (dataType.HasFacet<MaxLengthFacet>())
            {
                maxLength = dataType.GetFacet<MaxLengthFacet>().Value;
            }

            var typeReference = new EdmStringTypeReference(
                typeDefinition,
                dataType.IsNullable,
                false,
                maxLength,
                isUnicode);

            return typeReference;
        }

        IEdmPrimitiveTypeReference IPrimitiveDataTypeVisitor<IEdmPrimitiveTypeReference>.Visit(TimeOfDayDataType dataType)
        {
            return this.GetTemporalTypeReference(dataType);
        }

        private IEdmPrimitiveTypeReference GetTemporalTypeReference(PrimitiveDataType dataType)
        {
            IEdmPrimitiveType typeDefinition = this.GetEdmTypeDefinition(dataType);

            int? precision = null;
            if (dataType.HasFacet<TimePrecisionFacet>())
            {
                precision = dataType.GetFacet<TimePrecisionFacet>().Value;
            }

            var typeReference = new EdmTemporalTypeReference(
                typeDefinition,
                dataType.IsNullable,
                precision);

            return typeReference;
        }

        private IEdmPrimitiveTypeReference GetFacetlessEdmTypeReference(PrimitiveDataType dataType)
        {
            IEdmPrimitiveType typeDefinition = this.GetEdmTypeDefinition(dataType);
            var typeReference = new EdmPrimitiveTypeReference(typeDefinition, dataType.IsNullable);
            return typeReference;
        }

        private IEdmPrimitiveType GetEdmTypeDefinition(PrimitiveDataType dataType)
        {
            string edmTypeName = dataType.GetFacet<EdmTypeNameFacet>().Value;
            EdmPrimitiveTypeKind primitiveTypeKind = (EdmPrimitiveTypeKind)Enum.Parse(typeof(EdmPrimitiveTypeKind), edmTypeName, false);
            IEdmPrimitiveType edmIntTypeDefinition = EdmCoreModel.Instance.GetPrimitiveType(primitiveTypeKind);
            return edmIntTypeDefinition;
        }
    }
}
