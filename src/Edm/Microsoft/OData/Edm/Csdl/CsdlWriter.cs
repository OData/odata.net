//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl
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
