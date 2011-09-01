//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Csdl.Internal.Serialization;
using Microsoft.Data.Edm.Library;

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
        public static void WriteCsdl(this IEdmModel model, XmlWriter writer)
        {
            WriteCsdl(model, x => writer, true);
        }

        /// <summary>
        /// Outputs a CSDL artifact to the provided writers.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writerProvider">A delegate that takes in a schema namespace name and returns an XmlWriter to write the schema to.</param>
        public static void WriteCsdl(this IEdmModel model, Func<string, XmlWriter> writerProvider)
        {
            WriteCsdl(model, writerProvider, false);
        }

        private static void WriteCsdl(IEdmModel model, Func<string, XmlWriter> writerProvider, bool singleFileExpected)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writerProvider, "writerProvider");

            IEnumerable<EdmSchema> schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();
            EdmModelCsdlSerializationVisitor visitor;
            if (schemas.Count() > 1 && singleFileExpected)
            {
                throw new InvalidOperationException(Edm.Strings.Serializer_SingleFileExpected);
            }

            Version edmVersion = model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;

            foreach (EdmSchema schema in schemas)
            {
                XmlWriter writer = writerProvider(schema.Namespace);
                if (writer != null)
                {
                    visitor = new EdmModelCsdlSerializationVisitor(writer, edmVersion);
                    visitor.VisitEdmSchema(schema, model.GetNamespacePrefixMappings());
                }
            }
        }
    }
}
