//---------------------------------------------------------------------
// <copyright file="CsdlJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Parse one CSDL-JSON into CSDL model.
    /// </summary>
    internal class CsdlJsonParser
    {
        /// <summary>
        /// Parse CSDL-JSON doc into CsdlModel, error messages are stored in <see cref="JsonParserContext"/>
        /// </summary>
        /// <param name="jsonReader">The JSON reader.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>Null or parsed <see cref="CsdlModel"/>.</returns>
        internal static CsdlModel ParseCsdlDocument(ref Utf8JsonReader jsonReader, JsonParserContext context)
        {
            Debug.Assert(context != null);

            JsonDocument jsonDocument = GetJsonDocument(ref jsonReader, context);
            if (jsonDocument == null)
            {
                return null;
            }

            // make sure to dispose the JsonDocument.
            using (jsonDocument)
            {
                JsonElement rootElement = jsonDocument.RootElement;

                // A CSDL JSON document consists of a single JSON object.
                if (!rootElement.ValidateValueKind(JsonValueKind.Object, context))
                {
                    return null;
                }

                // This document object MUST contain the member $Version.
                Version version = rootElement.ProcessRequiredProperty("$Version", context, ParseVersion);
                if (version == null)
                {
                    return null;
                }

                CsdlModel csdlModel = new CsdlModel
                {
                    CsdlVersion = version
                };

                IList<CsdlReference> references = null;
                rootElement.ParseAsObject(context, (propertyName, propertyValue) =>
                {
                    switch (propertyName)
                    {
                        case "$Version":
                            // skip, because processed
                            break;

                        case "$EntityContainer":
                            // The value of $EntityContainer is the namespace-qualified name of the entity container of that service.
                            // So far, i don't know how to use it. So skip it.
                            break;

                        case "$Reference":
                            // The document object MAY contain the member $Reference to reference other CSDL documents.
                            references = ParseReferences(propertyValue, context);
                            break;

                        default:
                            // CSDL document also MAY contain members for schemas.
                            // Each schema's value is an object.
                            if (propertyValue.ValueKind == JsonValueKind.Object)
                            {
                                CsdlSchema schema = SchemaJsonParser.ParseCsdlSchema(propertyName, csdlModel.CsdlVersion, propertyValue, context);
                                if (schema != null)
                                {
                                    csdlModel.AddSchema(schema);
                                    break;
                                }
                            }

                            context.ReportError(EdmErrorCode.UnexpectedElement, Strings.CsdlJsonParser_UnexpectedJsonMember(context.Path, propertyValue.ValueKind));
                            break;
                    }
                });


                if (references != null)
                {
                    csdlModel.AddCurrentModelReferences(references);
                }

                return csdlModel;
            }
        }

        /// <summary>
        /// Build the JSON value <see cref="JsonElement"/> to collection of <see cref="IEdmReference"/>.
        /// It's weird that we parse it into IEdmReference, not CsdlReference.
        /// It is because CsdlModel needs "IEdmReference". :-(
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="jsonPath">The parser context.</param>
        /// <returns>null or parsed collection of <see cref="IEdmReference"/>.</returns>
        internal static IList<CsdlReference> ParseReferences(JsonElement element, JsonParserContext context)
        {
            // The value of $Reference is an object that contains one member per referenced CSDL document.
            if (!element.ValidateValueKind(JsonValueKind.Object, context))
            {
                return null;
            }

            IList<CsdlReference> references = new List<CsdlReference>();
            element.ParseAsObject(context, (url, propertyValue) =>
            {
                CsdlReference reference = ParseReference(url, propertyValue, context);
                if (reference != null)
                {
                    references.Add(reference);
                }
            });

            return references;
        }

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to a <see cref="IEdmReference"/>.
        /// </summary>
        /// <param name="url">The reference Url string.</param>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>null or parsed <see cref="IEdmReference"/>.</returns>
        internal static CsdlReference ParseReference(string url, JsonElement element, JsonParserContext context)
        {
            // The value of each reference object is an object.
            if (!element.ValidateValueKind(JsonValueKind.Object, context))
            {
                return null;
            }

            IList<CsdlInclude> includes = null;
            IList<CsdlIncludeAnnotations> includeAnnotations = null;
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                // The reference object MAY contain the members $Include and $IncludeAnnotations as well as annotations.
                switch (propertyName)
                {
                    case "$Include":
                        // The value of $Include is an array.
                        // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                        includes = propertyValue.ParseAsArray(context, ParseInclude);
                        break;

                    case "$IncludeAnnotations":
                        // The value of $IncludeAnnotations is an array.
                        // Array items are objects that MUST contain the member $TermNamespace and MAY contain the members $Qualifier and $TargetNamespace.
                        includeAnnotations = propertyValue.ParseAsArray(context, ParseIncludeAnnotations);
                        break;

                    default:
                        // The reference objects MAY contain annotations.
                        SchemaJsonParser.ParseCsdlAnnotation(propertyName, propertyValue, context, annotations);
                        break;
                }
            });

            CsdlReference reference = new CsdlReference(url,
                includes ?? Enumerable.Empty<CsdlInclude>(),
                includeAnnotations ?? Enumerable.Empty<CsdlIncludeAnnotations>(),
                context.Location());
            annotations.ForEach(a => reference.AddAnnotation(a));
            return reference;
        }

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to a <see cref="IEdmInclude"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>null or parsed <see cref="IEdmInclude"/>.</returns>
        internal static CsdlInclude ParseInclude(JsonElement element, JsonParserContext context)
        {
            // Each item in $Include is an object.
            if (!element.ValidateValueKind(JsonValueKind.Object, context))
            {
                return null;
            }

            string includeNamespace = null;
            string includeAlias = null;
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the included schema.
                        includeAlias = propertyValue.ParseAsString(context);
                        break;

                    case "$Namespace":
                        // The value of $Namespace is a string containing the namespace of the included schema
                        includeNamespace = propertyValue.ParseAsString(context);
                        break;

                    default:
                        // The item objects MAY contain annotations.
                        SchemaJsonParser.ParseCsdlAnnotation(propertyName, propertyValue, context, annotations);
                        break;
                }
            });

            CsdlInclude include = new CsdlInclude(includeAlias, includeNamespace, context.Location());
            annotations.ForEach(a => include.AddAnnotation(a));
            return include;
        }

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to a <see cref="IEdmIncludeAnnotations"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>null or parsed <see cref="IEdmIncludeAnnotations"/>.</returns>
        internal static CsdlIncludeAnnotations ParseIncludeAnnotations(JsonElement element, JsonParserContext context)
        {
            // Each item in $IncludeAnnotations is an object.
            if (!element.ValidateValueKind(JsonValueKind.Object, context))
            {
                return null;
            }

            string termNamespace = null;
            string qualifier = null;
            string targetNamespace = null;
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                switch (propertyName)
                {
                    case "$TermNamespace":
                        // The value of $TermNamespace is a namespace.
                        termNamespace = propertyValue.ParseAsString(context);
                        break;

                    case "$Qualifier":
                        // The value of $Qualifier is a simple identifier.
                        qualifier = propertyValue.ParseAsString(context);
                        break;

                    case "$TargetNamespace":
                        // The value of $TargetNamespace is a namespace.
                        targetNamespace = propertyValue.ParseAsString(context);
                        break;

                    default:
                        // The item objects doesn't contain vocabulary annotation.
                        context.ReportError(EdmErrorCode.UnexpectedElement,
                            Strings.CsdlJsonParser_UnexpectedJsonMember(context.Path, propertyValue.ValueKind));
                        break;
                }
            });

            return new CsdlIncludeAnnotations(termNamespace, qualifier, targetNamespace, context.Location());
        }

        private static JsonDocument GetJsonDocument(ref Utf8JsonReader jsonReader, JsonParserContext context)
        {
            Debug.Assert(context != null);

            try
            {
                JsonDocument.TryParseValue(ref jsonReader, out JsonDocument jsonDocument);
                return jsonDocument;
            }
            catch (JsonException jsonEx)
            {
                StringBuilder sb = new StringBuilder(context.Source != null ? context.Source : "$");
                sb.Append(" LineNumber:");
                sb.Append(jsonEx.LineNumber != null ? jsonEx.LineNumber.Value.ToString(CultureInfo.InvariantCulture) : "N/A");

                sb.Append(" BytePositionInLine:");
                sb.Append(jsonEx.BytePositionInLine != null ? jsonEx.BytePositionInLine.Value.ToString(CultureInfo.InvariantCulture) : "N/A");

                sb.Append(" Path:");
                sb.Append(jsonEx.Path != null ? jsonEx.Path : "N/A");

                sb.Append(" ActualMessage:");
                sb.Append(jsonEx.Message != null ? jsonEx.Message : "N/A");

                context.ReportError(EdmErrorCode.InvalidJson, sb.ToString());
                return null;
            }
        }

        private static Version ParseVersion(JsonElement element, JsonParserContext context)
        {
            Debug.Assert(context != null);

            Version version = null;
            string strVersion = element.ParseAsString(context);
            if (context.IsSucceeded())
            {
                if (strVersion == "4.0")
                {
                    version = EdmConstants.EdmVersion4;
                }
                else if (strVersion == "4.01")
                {
                    version = EdmConstants.EdmVersion401;
                }

                // This document object MUST contain the member $Version.
                if (version == null)
                {
                    context.ReportError(EdmErrorCode.InvalidVersionNumber, Strings.CsdlJsonParser_InvalidCsdlVersion(context.Path));
                }
            }

            return version;
        }
    }
}
#endif