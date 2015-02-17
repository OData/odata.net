//---------------------------------------------------------------------
// <copyright file="DataTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Provides entry point for data type construction.
    /// </summary>
    public static class DataTypes
    {
        /// <summary>
        /// Initializes static members of the DataTypes class.
        /// </summary>
        static DataTypes()
        {
            Integer = new IntegerDataType();
            Stream = new StreamDataType();
            String = new StringDataType();
            Boolean = new BooleanDataType();
            FixedPoint = new FixedPointDataType();
            FloatingPoint = new FloatingPointDataType();
            DateTime = new DateTimeDataType();
            Binary = new BinaryDataType();
            Guid = new GuidDataType();
            TimeOfDay = new TimeOfDayDataType();
            ComplexType = new ComplexDataType();
            EntityType = new EntityDataType();
            CollectionType = new CollectionDataType();
            ReferenceType = new ReferenceDataType();
            RowType = new RowDataType();
            EnumType = new EnumDataType();
            Spatial = new SpatialDataType();
        }

        /// <summary>
        /// Gets the <see cref="IntegerDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static IntegerDataType Integer { get; private set; }

        /// <summary>
        /// Gets the <see cref="StreamDataType"/> without a definition
        /// </summary>
        public static StreamDataType Stream { get; private set; }

        /// <summary>
        /// Gets the <see cref="StringDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static StringDataType String { get; private set; }

        /// <summary>
        /// Gets the <see cref="BooleanDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static BooleanDataType Boolean { get; private set; }

        /// <summary>
        /// Gets the <see cref="FixedPointDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static FixedPointDataType FixedPoint { get; private set; }

        /// <summary>
        /// Gets the <see cref="FloatingPointDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static FloatingPointDataType FloatingPoint { get; private set; }

        /// <summary>
        /// Gets the <see cref="DateTimeDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static DateTimeDataType DateTime { get; private set; }

        /// <summary>
        /// Gets the <see cref="BinaryDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static BinaryDataType Binary { get; private set; }

        /// <summary>
        /// Gets the <see cref="GuidDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static GuidDataType Guid { get; private set; }

        /// <summary>
        /// Gets the <see cref="TimeOfDayDataType"/> without facets.
        /// </summary>
        /// <value>The data type.</value>
        public static TimeOfDayDataType TimeOfDay { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComplexDataType"/> without a definition.
        /// </summary>
        /// <value>The data type.</value>
        public static ComplexDataType ComplexType { get; private set; }

        /// <summary>
        /// Gets the <see cref="EntityDataType"/> without a definition.
        /// </summary>
        /// <value>The data type.</value>
        public static EntityDataType EntityType { get; private set; }

        /// <summary>
        /// Gets the <see cref="CollectionDataType"/> without an Element data type.
        /// </summary>
        public static CollectionDataType CollectionType { get; private set; }

        /// <summary>
        /// Gets the <see cref="ReferenceDataType"/> without referenced entity type 
        /// </summary>
        public static ReferenceDataType ReferenceType { get; private set; }

        /// <summary>
        /// Gets the <see cref="RowDataType"/> without a definition
        /// </summary>
        public static RowDataType RowType { get; private set; }
        
        /// <summary>
        /// Gets the <see cref="EnumDataType"/> without a definition
        /// </summary>
        public static EnumDataType EnumType { get; private set; }

        /// <summary>
        /// Gets the <see cref="SpatialDataType"/> without a definition
        /// </summary>
        public static SpatialDataType Spatial { get; private set; }

        /// <summary>
        /// Gets the <see cref="CollectionDataType"/> with entiy data type as element type.
        /// </summary>
        /// <param name="entityType">Entity data type definition.</param>
        /// <returns>The data type.</returns>
        public static CollectionDataType CollectionOfEntities(EntityType entityType)
        {
            return CollectionType.WithElementDataType(EntityType.WithDefinition(entityType));
        }

        /// <summary>
        /// Gets the <see cref="CollectionDataType"/> with complex data type as element type.
        /// </summary>
        /// <param name="complexType">Complex data type definition.</param>
        /// <returns>The data type.</returns>
        public static CollectionDataType CollectionOfComplex(ComplexType complexType)
        {
            return CollectionType.WithElementDataType(ComplexType.WithDefinition(complexType));
        }

        /// <summary>
        /// Gets the <see cref="CollectionDataType"/> with row data type as element type.
        /// </summary>
        /// <param name="rowType">Row data type definition.</param>
        /// <returns>The data type.</returns>
        public static CollectionDataType CollectionOfRows(RowType rowType)
        {
            return CollectionType.WithElementDataType(RowType.WithDefinition(rowType));
        }
    }
}
