//---------------------------------------------------------------------
// <copyright file="SchemaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides Schema serialization services for EDM models.
    /// </summary>
    public static class SchemaWriter
    {
        /// <summary>
        /// Outputs a Schema artifact to the provided writer.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated Schema will be written to.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteSchema(this IEdmModel model, XmlWriter writer, out IEnumerable<EdmError> errors)
        {
            return TryWriteSchema(model, x => writer, true, out errors);
        }

        /// <summary>
        /// Asynchronously Outputs a Schema artifact to the provided writer.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated Schema will be written to.</param>
        /// <returns>A task represents a Tuple with value indicating whether serialization was successful and Errors that prevented successful serialization, or no errors if serialization was successful.</returns>
        public static Task<(bool, IEnumerable<EdmError>)> TryWriteSchemaAsync(this IEdmModel model, XmlWriter writer)
        {
            return TryWriteSchemaAsync(model, x => writer, true);
        }

        /// <summary>
        /// Outputs Schema artifacts to the provided writers.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writerProvider">A delegate that takes in a Schema namespace name and returns an XmlWriter to write the Schema to.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteSchema(this IEdmModel model, Func<string, XmlWriter> writerProvider, out IEnumerable<EdmError> errors)
        {
            return TryWriteSchema(model, writerProvider, false, out errors);
        }

        /// <summary>
        /// Asynchronously Outputs Schema artifacts to the provided writers.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writerProvider">A delegate that takes in a Schema namespace name and returns an XmlWriter to write the Schema to.</param>
        /// <returns>A task represents a Tuple with value indicating whether serialization was successful and Errors that prevented successful serialization, or no errors if serialization was successful.</returns>
        public static Task<(bool, IEnumerable<EdmError>)> TryWriteSchemaAsync(this IEdmModel model, Func<string, XmlWriter> writerProvider)
        {
            return TryWriteSchemaAsync(model, writerProvider, false);
        }

        internal static bool TryWriteSchema(IEdmModel model, Func<string, XmlWriter> writerProvider, bool singleFileExpected, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writerProvider, "writerProvider");

            errors = model.GetSerializationErrors();
            if (errors.FirstOrDefault() != null)
            {
                return false;
            }

            IEnumerable<EdmSchema> schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();
            if (schemas.Count() > 1 && singleFileExpected)
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.SingleFileExpected, Edm.Strings.Serializer_SingleFileExpected) };
                return false;
            }

            if (!schemas.Any())
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.NoSchemasProduced, Edm.Strings.Serializer_NoSchemasProduced) };
                return false;
            }

            WriteSchemas(model, schemas, writerProvider);

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        internal static async Task<(bool, IEnumerable<EdmError>)> TryWriteSchemaAsync(IEdmModel model, Func<string, XmlWriter> writerProvider, bool singleFileExpected)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writerProvider, "writerProvider");

            IEnumerable<EdmError> errors = model.GetSerializationErrors();
            if (errors.FirstOrDefault() != null)
            {
                return (false, errors);
            }

            IEnumerable<EdmSchema> schemas = await (new EdmModelSchemaSeparationSerializationVisitor(model)).GetSchemasAsync().ConfigureAwait(false);
            if (schemas.Count() > 1 && singleFileExpected)
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.SingleFileExpected, Edm.Strings.Serializer_SingleFileExpected) };
                return (false, errors);
            }

            if (!schemas.Any())
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.NoSchemasProduced, Edm.Strings.Serializer_NoSchemasProduced) };
                return (false, errors);
            }

            await WriteSchemasAsync(model, schemas, writerProvider).ConfigureAwait(false);

            errors = Enumerable.Empty<EdmError>();
            return (true, errors);
        }

        internal static void WriteSchemas(IEdmModel model, IEnumerable<EdmSchema> schemas, Func<string, XmlWriter> writerProvider)
        {
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = model.GetEdmVersion() ?? EdmConstants.EdmVersionDefault;
            foreach (EdmSchema schema in schemas)
            {
                XmlWriter writer = writerProvider(schema.Namespace);
                if (writer != null)
                {
                    var schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, writer, edmVersion, new CsdlXmlWriterSettings());
                    visitor = new EdmModelCsdlSerializationVisitor(model, schemaWriter);
                    visitor.VisitEdmSchema(schema, model.GetNamespacePrefixMappings());
                }
            }
        }

        internal static async Task WriteSchemasAsync(IEdmModel model, IEnumerable<EdmSchema> schemas, Func<string, XmlWriter> writerProvider)
        {
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = model.GetEdmVersion() ?? EdmConstants.EdmVersionDefault;
            foreach (EdmSchema schema in schemas)
            {
                XmlWriter writer = writerProvider(schema.Namespace);
                if (writer != null)
                {
                    var schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, writer, edmVersion, new CsdlXmlWriterSettings());
                    visitor = new EdmModelCsdlSerializationVisitor(model, schemaWriter);
                    await visitor.VisitEdmSchemaAsync(schema, model.GetNamespacePrefixMappings()).ConfigureAwait(false);
                }
            }
        }
    }
}
