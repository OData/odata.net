//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Csdl.Internal.Serialization;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL serialization services for EDM models.
    /// </summary>
    public static class CsdlWriter
    {
        /// <summary>
        /// Outputs a CSDL artifact to the provided writer.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated CSDL will be written to.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(this IEdmModel model, XmlWriter writer, out IEnumerable<EdmError> errors)
        {
            return TryWriteCsdl(model, x => writer, true, out errors);
        }

        /// <summary>
        /// Outputs a CSDL artifact to the provided writers.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writerProvider">A delegate that takes in a schema namespace name and returns an XmlWriter to write the schema to.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(this IEdmModel model, Func<string, XmlWriter> writerProvider, out IEnumerable<EdmError> errors)
        {
            return TryWriteCsdl(model, writerProvider, false, out errors);
        }

        internal static bool TryWriteCsdl(IEdmModel model, Func<string, XmlWriter> writerProvider, bool singleFileExpected, out IEnumerable<EdmError> errors)
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

            if (schemas.Count() == 0)
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.NoSchemasProduced, Edm.Strings.Serializer_NoSchemasProduced) };
                return false;
            }

            WriteSchemas(model, schemas, writerProvider);

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        internal static void WriteSchemas(IEdmModel model, IEnumerable<EdmSchema> schemas, Func<string, XmlWriter> writerProvider)
        {
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
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
    }
}
