//---------------------------------------------------------------------
// <copyright file="CsdlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL serialization services (XML & JSON) for EDM models.
    /// </summary>
    public abstract class CsdlWriter
    {
        internal readonly IEdmModel model;
        internal readonly IEnumerable<EdmSchema> schemas;
        internal readonly Version edmxVersion;

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="edmxVersion">The Edmx version.</param>
        protected CsdlWriter(IEdmModel model, Version edmxVersion)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(edmxVersion, "edmxVersion");

            this.model = model;
            this.schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();
            this.edmxVersion = edmxVersion;
        }

        /// <summary>
        /// Outputs a CSDL JSON artifact to the provided <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="model">The Edm model to be written.</param>
        /// <param name="writer">XmlWriter the generated CSDL will be written to.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteJson(IEdmModel model, TextWriter writer, out IEnumerable<EdmError> errors)
        {
            return TryWriteJson(model, writer, CsdlJsonWriterSettings.Default, out errors);
        }

        /// <summary>
        /// Outputs a CSDL JSON artifact to the provided <see cref="TextWriter"/> using the settings.
        /// </summary>
        /// <param name="model">The Edm model to be written.</param>
        /// <param name="writer">XmlWriter the generated CSDL will be written to.</param>
        /// <param name="settings">The CSDL writer settings.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteJson(IEdmModel model, TextWriter writer, CsdlJsonWriterSettings settings, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writer, "writer");

            Version edmxVersion;
            if (!Verify(model, out edmxVersion, out errors))
            {
                return false;
            }

            CsdlWriter csdlWriter = new CsdlJsonWriter(model, writer, settings, edmxVersion);
            csdlWriter.WriteCsdl();

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        /// <summary>
        /// Outputs a CSDL XML artifact to the provided <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated CSDL will be written to.</param>
        /// <param name="target">Target implementation of the CSDL being generated.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(IEdmModel model, XmlWriter writer, CsdlTarget target, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writer, "writer");

            Version edmxVersion;
            if (!Verify(model, out edmxVersion, out errors))
            {
                return false;
            }

            CsdlWriter csdlWriter = new CsdlXmlWriter(model, writer, edmxVersion, target);
            csdlWriter.WriteCsdl();

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        private void WriteEdmxElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Edmx, this.edmxNamespace);
            this.writer.WriteAttributeString(CsdlConstants.Attribute_Version, GetVersionString(this.edmxVersion));
        }

        /// <summary>
        /// Write CSDL output.
        /// </summary>
        protected abstract void WriteCsdl();

        private static bool Verify(IEdmModel model, out Version edmxVersion, out IEnumerable<EdmError> errors)
        {
            Debug.Assert(model != null);

            edmxVersion = model.GetEdmxVersion();

            errors = model.GetSerializationErrors();
            if (errors.FirstOrDefault() != null)
            {
                return false;
            }

            if (edmxVersion != null)
            {
                if (!CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion))
                {
                    errors = new EdmError[]
                    {
                        new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmxVersion, Strings.Serializer_UnknownEdmxVersion)
                    };


        private void WriteSchemas()
        {
            // TODO: for referenced model - write alias as is, instead of writing its namespace.
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = this.model.GetEdmVersion() ?? EdmConstants.EdmVersionDefault;
            foreach (EdmSchema schema in this.schemas)

                    return false;
                }
            }
            else if (!CsdlConstants.EdmToEdmxVersions.TryGetValue(model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest, out edmxVersion))

            {
                errors = new EdmError[]
                {
                    new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmVersion, Strings.Serializer_UnknownEdmVersion)
                };

                return false;
            }

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        // Gets the string form of the EdmVersion.
        // Note that Version 4.01 needs two digits of minor version precision.
        private static string GetVersionString(Version version)
        {
            if (version == EdmConstants.EdmVersion401)
            {
                return EdmConstants.EdmVersion401String;
            }

            return version.ToString();
        }
    }
}
