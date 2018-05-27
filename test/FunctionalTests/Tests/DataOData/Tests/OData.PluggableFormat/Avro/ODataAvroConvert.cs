//---------------------------------------------------------------------
// <copyright file="ODataAvroConvert.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.OData;

    internal static class ODataAvroConvert
    {
        public static object FromODataObject(object value, Schema schema)
        {
            var entry = value as ODataResource;
            if (entry != null)
            {
                RecordSchema recordSchema = schema as RecordSchema;

                if (recordSchema == null)
                {
                    var unionSchema = schema as UnionSchema;
                    if (unionSchema != null)
                    {
                        recordSchema = unionSchema.Schemas.OfType<RecordSchema>().Single();
                    }
                    else
                    {
                        throw new ApplicationException("not supported schema found.");
                    }
                }

                var record = new AvroRecord(recordSchema);
                foreach (var property in entry.Properties)
                {
                    RecordField field;
                    recordSchema.TryGetField(property.Name, out field);
                    record[property.Name] = FromODataObject(property.Value, field != null ? field.TypeSchema : null);
                }

                return record;
            }

            var error = value as ODataError;
            if (error != null)
            {
                var record = new AvroRecord(schema);
                record["ErrorCode"] = error.ErrorCode;
                record["Message"] = error.Message;
                return record;
            }

            var collection = value as ODataCollectionValue;
            if (collection != null)
            {
                if (collection.Items is IEnumerable<object>)
                {
                    return ((IEnumerable<object>)collection.Items).ToArray();
                }

                return collection.Items;
            }

            return value;
        }

        public static object UpdateNestedInfoFromODataObject(object currentObj, ODataResource entry, ODataNestedResourceInfo property, Schema schema)
        {
            var record = currentObj as AvroRecord;
            if (record != null)
            {
                var recordSchema = (RecordSchema)schema;
                RecordField field;
                recordSchema.TryGetField(property.Name, out field);

                record[property.Name] = FromODataObject(entry, field != null ? field.TypeSchema : null);
            }

            return record;
        }

        public static ODataResource ToODataEntry(AvroRecord record)
        {
            return new ODataResource
            {
                TypeName = record.Schema.FullName,
                Properties = GetProperties(record)
            };
        }

        public static ODataResource ToODataEntry(AvroRecord parentRecord, string fieldName)
        {
            var subRecord = parentRecord[fieldName] as AvroRecord;
            return ToODataEntry(subRecord);
        }

        public static ODataValue ToODataValue(object obj)
        {
            var array = obj as Array;
            if (array != null && !(array is byte[]))
            {
                return new ODataCollectionValue { Items = array.Cast<object>() };
            }

            return new ODataPrimitiveValue(obj);
        }

        private static IEnumerable<ODataProperty> GetProperties(AvroRecord record)
        {
            return record.Schema.Fields
                .Where(field => !(record[field.Name] is AvroRecord))
                .Select(field => new ODataProperty
                {
                    Name = field.Name,
                    Value = ToODataValue(record[field.Name]),
                });
        }

        public static ODataNestedResourceInfo GetNestedResourceInfo(AvroRecord record)
        {
            return record.Schema.Fields
                .Where(field => (record[field.Name] is AvroRecord))
                .Select(field => new ODataNestedResourceInfo
                {
                    Name = field.Name,
                    IsCollection = false
                })
                .SingleOrDefault();
        }
    }
}
#endif