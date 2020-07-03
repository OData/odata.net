//---------------------------------------------------------------------
// <copyright file="CsdlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if NETSTANDARD2_0
using System.Text.Json;
#endif
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL serialization services (XML & JSON) for EDM models.
    /// </summary>
    public class CsdlWriter
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

#if NETSTANDARD2_0
        /// <summary>
        /// Outputs a CSDL JSON artifact to the provided <see cref="Utf8JsonWriter"/>.
        /// </summary>
        /// <param name="model">The Edm model to be written.</param>
        /// <param name="writer">JSON writer the generated CSDL will be written to.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(IEdmModel model, Utf8JsonWriter writer, out IEnumerable<EdmError> errors)
        {
            return TryWriteCsdl(model, writer, CsdlJsonWriterSettings.Default, out errors);
        }

        /// <summary>
        /// Outputs a CSDL JSON artifact to the provided <see cref="Utf8JsonWriter"/> using the settings.
        /// </summary>
        /// <param name="model">The Edm model to be written.</param>
        /// <param name="writer">JSON writer the generated CSDL will be written to.</param>
        /// <param name="settings">The CSDL writer settings.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(IEdmModel model, Utf8JsonWriter writer, CsdlJsonWriterSettings settings, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, nameof(model));
            EdmUtil.CheckArgumentNull(writer, nameof(writer));
            EdmUtil.CheckArgumentNull(settings, nameof(settings));

            Version edmxVersion;
            if (!VerifyAndGetVersion(model, out edmxVersion, out errors))
            {
                return false;
            }

            CsdlWriter csdlWriter = new CsdlJsonWriter(model, writer, settings, edmxVersion);
            csdlWriter.WriteCsdl();

            errors = Enumerable.Empty<EdmError>();
            return true;
        }
#endif

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
            if (!VerifyAndGetVersion(model, out edmxVersion, out errors))
            {
                return false;
            }

            CsdlWriter csdlWriter = new CsdlXmlWriter(model, writer, edmxVersion, target);
            csdlWriter.WriteCsdl();

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        /// <summary>
        /// Write CSDL output.
        /// </summary>
        protected virtual void WriteCsdl()
        {
            // nothing here
        }

        /// <summary>
        /// Gets the string form of the EdmVersion.
        /// Note that Version 4.01 needs two digits of minor version precision.
        /// </summary>
        protected static string GetVersionString(Version version)
        {
            Debug.Assert(version != null);

            if (version == EdmConstants.EdmVersion401)
            {
                return EdmConstants.EdmVersion401String;
            }

            return version.ToString();
        }

        private static bool VerifyAndGetVersion(IEdmModel model, out Version edmxVersion, out IEnumerable<EdmError> errors)
        {
            Debug.Assert(model != null);

            edmxVersion = model.GetEdmxVersion();

            errors = model.GetSerializationErrors();
            if (errors.Any())
            {
                return false;
            }

            if (edmxVersion != null)
            {
                if (!CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion))
                {
                    errors = new EdmError[]
                    {
                        new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmxVersion, Strings.Serializer_UnknownEdmxVersion(edmxVersion.ToString()))
                    };

                    return false;
                }
            }
            else
            {
                Version edmVersion = model.GetEdmVersion() ?? EdmConstants.EdmVersionDefault;
                if (!CsdlConstants.EdmToEdmxVersions.TryGetValue(edmVersion, out edmxVersion))
                {
                    errors = new EdmError[]
                    {
                        new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmVersion, Strings.Serializer_UnknownEdmVersion(edmVersion.ToString()))
                    };

                    return false;
                }
            }

            errors = Enumerable.Empty<EdmError>();
            return true;
        }
    }
}
