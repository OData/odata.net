//---------------------------------------------------------------------
// <copyright file="CsdlJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Json.Parser;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// </summary>
    internal static class CsdlJsonParser
    {
        /// <summary>
        /// Parse the JSON value <see cref="IJsonValue"/> to <see cref="CsdlModel"/>.
        /// </summary>
        /// <param name="jsonValue">The JSON value to parse.</param>
        /// <param name="jsonPath">The JSON path to track on the path of the processed JSON value.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns>Null or the generated <see cref="CsdlModel"/>.</returns>
        internal static CsdlModel ParseCsdlModel(IJsonValue jsonValue, JsonPath jsonPath, CsdlSerializerOptions options)
        {
            // A CSDL JSON document consists of a single JSON object.
            JsonObjectValue csdlObject = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            // This document object MUST contain the member $Version.
            Version version = GetCsdlVersion(csdlObject, jsonPath);
            Debug.Assert(version != null);

            CsdlModel csdlModel = new CsdlModel
            {
                Version = version
            };

            IList<IEdmReference> references = null;
            csdlObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Version":
                        // Processed above, so skip it.
                        break;

                    case "$EntityContainer":
                        // The value of $EntityContainer is value is the namespace-qualified name of the entity container of that service.
                        // So far, i don't know how to use it. So skip it.
                        // string entityContainer = propertyValue.ParseAsString();
                        break;

                    case "$Reference":
                        // The document object MAY contain the member $Reference to reference other CSDL documents.
                        references = ParseReferences(propertyValue, jsonPath, options);
                        break;

                    default:
                        // CSDL document also MAY contain members for schemas.
                        // Each schema's value is an object.
                        //CsdlSchema csdlSchema = CsdlJsonSchemaParser.ParseCsdlSchema(propertyName, propertyValue, jsonPath, version, options);
                        //if (csdlSchema != null)
                        //{
                        //    csdlModel.AddSchema(csdlSchema);
                        //}
                        //else
                        //{
                        //    propertyValue.ReportUnknownMember(jsonPath, options);
                        //}
                        break;
                }
            });

            if (references != null && references.Count > 0)
            {
                csdlModel.AddCurrentModelReferences(references);
            }

            return csdlModel;
        }

        /// <summary>
        /// Build the JSON value <see cref="IJsonValue"/> to collection of <see cref="IEdmReference"/>.
        /// It's weird that we parse it into IEdmReference, not CsdlReference.
        /// That's because CsdlModel's definition need "IEdmReference".
        /// TODO: maybe we should consider to change the definition in CsdlModel to accept CsdlReference.
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public static IList<IEdmReference> ParseReferences(IJsonValue jsonValue, JsonPath jsonPath, CsdlSerializerOptions options)
        {
            // The value of $Reference is an object that contains one member per referenced CSDL document.
            JsonObjectValue referenceObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            IList<IEdmReference> references = new List<IEdmReference>(0);

            referenceObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                // The name of the pair is a URI for the referenced document.
                // The URI MAY be relative to the document containing the $Reference.
                IEdmReference result = ParseReferenceObject(propertyName, propertyValue, jsonPath, options);
                references.Add(result);
            });

            return references;
        }

        public static IEdmReference ParseReferenceObject(string url, IJsonValue jsonValue, JsonPath jsonPath, CsdlSerializerOptions options)
        {
            // The value of each reference object is an object.
            JsonObjectValue referenceObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            IList<IEdmInclude> includes = null;
            IList<IEdmIncludeAnnotations> includeAnnotations = null;

            referenceObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                // The reference object MAY contain the members $Include and $IncludeAnnotations as well as annotations.
                switch (propertyName)
                {
                    case "$Include":
                        // The value of $Include is an array.
                        // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                        includes = propertyValue.ParseArray(jsonPath, options, ParseInclude);
                        break;

                    case "$IncludeAnnotations":
                        // The value of $IncludeAnnotations is an array.
                        // Array items are objects that MUST contain the member $TermNamespace and MAY contain the members $Qualifier and $TargetNamespace.
                        includeAnnotations = propertyValue.ParseArray(jsonPath, options, ParseIncludeAnnotations);
                        break;

                    default:
                        // The reference objects MAY contain annotations.However, EdmReference doesn't support annotation.
                        // So, skip the annotation.

                        propertyValue.ReportUnknownMember(jsonPath, options);
                        break;
                }
            });

            EdmReference edmReference = new EdmReference(new Uri(url, UriKind.RelativeOrAbsolute));
            includes.ForEach(i => edmReference.AddInclude(i));
            includeAnnotations.ForEach(i => edmReference.AddIncludeAnnotations(i));
            return edmReference;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEdmInclude ParseInclude(IJsonValue jsonValue, JsonPath jsonPath, CsdlSerializerOptions options)
        {
            JsonObjectValue includeObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            string includeNamespace = null;
            string includeAlias = null;
            includeObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the included schema.
                        includeAlias = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Namespace":
                        // The value of $Namespace is a string containing the namespace of the included schema
                        includeNamespace = propertyValue.ParseAsString(jsonPath);
                        break;

                    default:
                        // The item objects MAY contain annotations. However, EdmInclude does not supported yet. So skip
                        propertyValue.ReportUnknownMember(jsonPath, options);
                        break;
                }
            });

            return new EdmInclude(includeAlias, includeNamespace);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEdmIncludeAnnotations ParseIncludeAnnotations(IJsonValue jsonValue, JsonPath jsonPath, CsdlSerializerOptions options)
        {
            JsonObjectValue includeObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);
            string termNamespace = null;
            string qualifier = null;
            string targetNamespace = null;
            includeObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                switch (propertyName)
                {
                    case "$TermNamespace":
                        // The value of $TermNamespace is a namespace.
                        termNamespace = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Qualifier":
                        // The value of $Qualifier is a simple identifier.
                        qualifier = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$TargetNamespace":
                        // The value of $TargetNamespace is a namespace.
                        targetNamespace = propertyValue.ParseAsString(jsonPath);
                        break;

                    default:
                        // The item objects MAY contain annotations. However, IEdmIncludeAnnotations doesn't support to have annotations.
                        propertyValue.ReportUnknownMember(jsonPath, options);
                        break;
                }
            });

            return new EdmIncludeAnnotations(termNamespace, qualifier, targetNamespace);
        }

        private static Version GetCsdlVersion(JsonObjectValue csdlObject, JsonPath parentPath)
        {
            Debug.Assert(csdlObject != null);

            Version version = null;
            IJsonValue versionValue;
            if (csdlObject.TryGetValue("$Version", out versionValue))
            {
                parentPath.Push("$Version");

                string strVersion = versionValue.ParseAsString(parentPath);
                if (strVersion == "4.0")
                {
                    version = EdmConstants.EdmVersion4;
                }
                else if (strVersion == "4.01")
                {
                    version = EdmConstants.EdmVersion401;
                }

                parentPath.Pop();
            }

            if (version != null)
            {
                return version;
            }

            // This document object MUST contain the member $Version.
            throw new CsdlParseException(Strings.CsdlJsonParser_MissingMemberInObject("$Version", parentPath));
        }
    }
}
