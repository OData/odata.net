//---------------------------------------------------------------------
// <copyright file="SchemaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides Schema serialization services for EDM models (XML & JSON).
    /// </summary>
    public static class SchemaWriter
    {
        public static bool TryWriteJson(this IEdmModel model, TextWriter writer, out IEnumerable<EdmError> errors)
        {
            return model.TryWriteJson(x => writer, CsdlWriterSettings.Default, out errors);
        }

        public static bool TryWriteJson(this IEdmModel model, TextWriter writer, CsdlWriterSettings settings, out IEnumerable<EdmError> errors)
        {
            return model.TryWriteJson(x => writer, settings, out errors);
        }

        public static bool TryWriteJson(this IEdmModel model, Func<string, TextWriter> writerProvider, CsdlWriterSettings settings, out IEnumerable<EdmError> errors)
        {
            errors = Enumerable.Empty<EdmError>();
            return true;
            //return TryWriteSchema(model, writerProvider,  out errors);
        }

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

        internal static bool TryWriteSchema(IEdmModel model, Func<string, XmlWriter> writerProvider, bool singleFileExpected, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writerProvider, "writerProvider");

            IEnumerable<EdmSchema> schemas;
            if (!Verify(model, singleFileExpected, out schemas, out errors))
            {
                return false;
            }

            WriteSchemas(model, schemas, writerProvider);

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        internal static bool TryWriteSchema(IEdmModel model, Func<string, TextWriter> writerProvider, CsdlWriterSettings settings, bool singleFileExpected, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writerProvider, "writerProvider");

            IEnumerable<EdmSchema> schemas;
            if (!Verify(model, singleFileExpected, out schemas, out errors))
            {
                return false;
            }

            WriteSchemas(model, schemas, writerProvider, settings);

            errors = Enumerable.Empty<EdmError>();
            return true;
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
                    visitor = new EdmModelCsdlSerializationVisitor(model, writer, edmVersion);
                    visitor.VisitEdmSchema(schema, model.GetNamespacePrefixMappings());
                }
            }
        }

        internal static void WriteSchemas(IEdmModel model, IEnumerable<EdmSchema> schemas, Func<string, TextWriter> writerProvider, CsdlWriterSettings settings)
        {
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
            foreach (EdmSchema schema in schemas)
            {
                TextWriter writer = writerProvider(schema.Namespace);
                if (writer != null)
                {
                    IEdmJsonWriter jsonWriter = new EdmJsonWriter(writer, settings);
                    visitor = new EdmModelCsdlSerializationVisitor(model, jsonWriter, edmVersion);
                    visitor.VisitEdmSchema(schema, model.GetNamespacePrefixMappings());
                }
            }
        }

        private static bool Verify(IEdmModel model, bool singleFileExpected, out IEnumerable<EdmSchema> schemas, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");

            schemas = null;
            errors = model.GetSerializationErrors();
            if (errors.FirstOrDefault() != null)
            {
                return false;
            }

            schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();
            if (schemas.Count() > 1 && singleFileExpected)
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.SingleFileExpected, Edm.Strings.Serializer_SingleFileExpected) };
                return false;
            }

            if (schemas.Count() == 0)
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.NoSchemasProduced, Edm.Strings.Serializer_NoSchemasProduced) };
                return false;
            }

            errors = Enumerable.Empty<EdmError>();
            return true;
        }
    }
}
