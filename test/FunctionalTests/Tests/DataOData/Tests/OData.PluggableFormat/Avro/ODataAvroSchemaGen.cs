//---------------------------------------------------------------------
// <copyright file="ODataAvroSchemaGen.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.Linq;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    internal static class ODataAvroSchemaGen
    {
        private static readonly RecordSchema ErrorSchema;

        static ODataAvroSchemaGen()
        {
            ErrorSchema = Schema.CreateRecord(AvroConstants.ODataErrorType, null);
            var fields = new[]
                {
                    Schema.CreateField(AvroConstants.ODataErrorFieldErrorCode, Schema.CreateString()),
                    Schema.CreateField(AvroConstants.ODataErrorFieldMessage, Schema.CreateString()),
                };

            Schema.SetFields(ErrorSchema, fields);
        }

        public static RecordSchema GetSchema(ODataResource entry)
        {
            RecordSchema rs = Schema.CreateRecord(entry.TypeName, null);
            Schema.SetFields(rs, entry.Properties.Select(property => Schema.CreateField(property.Name, GetSchema(property.Value))));
            return rs;
        }

        public static ArraySchema GetSchema(ODataCollectionValue colValue)
        {
            var enumerator = colValue.Items.GetEnumerator();
            enumerator.MoveNext();
            var item = enumerator.Current;
            return Schema.CreateArray(GetSchema(item));
        }

        public static TypeSchema GetSchema(object value)
        {
            if (value == null)
            {
                return Schema.CreateNull();
            }

            ODataResource entry = value as ODataResource;
            if (entry != null)
            {
                return GetSchema(entry);
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                return GetSchema(collectionValue);
            }

            ODataError error = value as ODataError;
            if (error != null)
            {
                return ErrorSchema;
            }

            byte[] bytes = value as byte[];
            if (bytes != null)
            {
                return Schema.CreateBytes();
            }

            Type type = value.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return Schema.CreateBoolean();
                case TypeCode.Int32:
                    return Schema.CreateInt();
                case TypeCode.Int64:
                    return Schema.CreateLong();
                case TypeCode.Single:
                    return Schema.CreateFloat();
                case TypeCode.Double:
                    return Schema.CreateDouble();
                case TypeCode.String:
                    return Schema.CreateString();
                default:
                    throw new Exception(string.Format("unsupported type: {0}", type.FullName));
            }
        }

        public static ArraySchema GetArraySchema(TypeSchema schema)
        {
            return Schema.CreateArray(schema);
        }

        public static TypeSchema GetSchemaFromModel(IEdmTypeReference typeReference)
        {
            var def = GetSchemaFromModel(typeReference.Definition);

            return typeReference.IsNullable
                ? Schema.CreateUnion(new[] { Schema.CreateNull(), def })
                : def;
        }

        private static RecordSchema GetSchemaFromModel(IEdmStructuredType type)
        {
            RecordSchema rs = Schema.CreateRecord(type.FullTypeName(), null);
            Schema.SetFields(rs, type.Properties().Select(property => Schema.CreateField(property.Name, GetSchemaFromModel(property.Type))));
            return rs;
        }

        public static TypeSchema GetSchemaFromModel(IEdmType type)
        {
            var structure = type as IEdmStructuredType;
            if (structure != null)
            {
                return GetSchemaFromModel(structure);
            }

            var collection = type as IEdmCollectionType;
            if (collection != null)
            {
                return Schema.CreateArray(GetSchemaFromModel(collection.ElementType));
            }

            var primitive = type as IEdmPrimitiveType;
            if (primitive != null)
            {
                switch (primitive.PrimitiveKind)
                {

                    case EdmPrimitiveTypeKind.Binary:
                        return Schema.CreateBytes();
                    case EdmPrimitiveTypeKind.Boolean:
                        return Schema.CreateBoolean();
                    case EdmPrimitiveTypeKind.Int32:
                        return Schema.CreateInt();
                    case EdmPrimitiveTypeKind.Int64:
                        return Schema.CreateLong();
                    case EdmPrimitiveTypeKind.Single:
                        return Schema.CreateFloat();
                    case EdmPrimitiveTypeKind.Double:
                        return Schema.CreateDouble();
                    case EdmPrimitiveTypeKind.String:
                        return Schema.CreateString();
                }
            }

            throw new Exception(string.Format("unsupported type: {0}", type.FullTypeName()));
        }

        public static TypeSchema GetItemSchema(TypeSchema schema)
        {
            var unionSchema = schema as UnionSchema;
            if (unionSchema != null)
            {
                schema = unionSchema.Schemas.OfType<ArraySchema>().SingleOrDefault();
            }

            var arraySchema = schema as ArraySchema;
            if (arraySchema != null)
            {
                return ((ArraySchema)AvroSerializer.CreateGeneric(arraySchema.ToString()).WriterSchema).ItemSchema;
            }

            throw new ApplicationException("Schema must be array schema");
        }
    }
}
#endif