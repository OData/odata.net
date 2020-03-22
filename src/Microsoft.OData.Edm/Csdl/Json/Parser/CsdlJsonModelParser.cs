//---------------------------------------------------------------------
// <copyright file="CsdlJsonModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Csdl.Json.Reader;
using Microsoft.OData.Edm.Csdl.Json.Value;

namespace Microsoft.OData.Edm.Csdl.Json.Parser
{
    /// <summary>
    /// For example we have A, B, C, D models
    /// A -> B
    ///      B -> C
    ///          C -> D
    ///               D -> B
    ///      B -> D
    /// So, If we load start from A, A is the main model, B, C, D are the refereneced models of A
    ///     If we load start from C, C is the mainn model, D, B are the referenced models of C
    /// </summary>
    internal class CsdlJsonModelBuilder
    {
        private CsdlSerializerOptions _options;
        private CsdlJsonModel _mainModel;
        private TextReader _textReader;
        private string _source;
        public CsdlJsonModelBuilder(TextReader textReader, CsdlSerializerOptions options)
            : this(textReader, options, null, null)
        {
        }

        public CsdlJsonModelBuilder(TextReader textReader, CsdlSerializerOptions options, string source, CsdlJsonModel mainModel)
        {
            _textReader = textReader;
            _options = options;
            _source = source;
            _mainModel = mainModel;
        }

        /// <summary>
        /// Parse the JSON value <see cref="IJsonValue"/> to <see cref="CsdlJsonModel"/>.
        /// </summary>
        /// <param name="jsonValue">The JSON value to parse.</param>
        /// <param name="jsonPath">The JSON path to track on the path of the processed JSON value.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns>Null or the generated <see cref="CsdlJsonModel"/>.</returns>
        internal CsdlJsonModel TryParseCsdlJsonModel()
        {
            JsonReader jsonReader = new JsonReader(_textReader, _options.GetJsonReaderOptions());
            JsonObjectValue csdlObject = jsonReader.ReadAsObject();

            // A CSDL JSON document consists of a single JSON object.
            // JsonObjectValue csdlObject = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            IJsonPath jsonPath = new JsonPath(_source);
            IList<IEdmReference> references = null;
            Version version = null;
            IDictionary<string, IJsonValue> members = new Dictionary<string, IJsonValue>();
            csdlObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Version":
                        // This document object MUST contain the member $Version.
                        version = ParseVersion(propertyValue, jsonPath);
                        break;

                    case "$EntityContainer":
                        // The value of $EntityContainer is value is the namespace-qualified name of the entity container of that service.
                        // So far, i don't know how to use it. So skip it.
                        // string entityContainer = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$Reference":
                        // The document object MAY contain the member $Reference to reference other CSDL documents.
                        references = ParseReferences(propertyValue, jsonPath);
                        break;

                    default:
                        // CSDL document also MAY contain members for schemas.
                        // Each schema's value is an object.
                        members[propertyName] = propertyValue;
                        break;
                }
            });

            Debug.Assert(version != null);

            CsdlJsonModel model = new CsdlJsonModel(_source, version);
            model.AddCurrentModelReferences(references);

            foreach (var member in members)
            {
                CsdlJsonSchemaParser schemaJsonParser = new CsdlJsonSchemaParser(version, member.Key, _options);
                //schemaJsonParser.TryParseCsdlSchema(member.Value, jsonPath);

                //model.AddSchemaJsonItems(schemaJsonParser.SchemaItems);
                CsdlJsonSchema schema = schemaJsonParser.TryParseCsdlJsonSchema(member.Value, jsonPath);

                model.AddSchema(schema);
            }

            model.BuildNamespaceAlias();

            // We are building the reference models, add into the main model.
            if (_mainModel != null)
            {
                _mainModel.AddReferencedModel(model);
                BuildReferencedModels(_mainModel, references);
            }
            else
            {
                BuildReferencedModels(model, references);
            }

            return model;
        }

        internal void BuildReferencedModels(CsdlJsonModel mainModel, IList<IEdmReference> edmReferences)
        {
            if (edmReferences == null)
            {
                return;
            }

            foreach (var edmReference in edmReferences)
            {
                // If added, skip to parse it.
                string sourceUri = edmReference.Uri.OriginalString;
                if (mainModel.IsReferencedModelAdded(sourceUri))
                {
                    continue;
                }

                // If nothing included, why does it exist?
                if (!edmReference.Includes.Any() && !edmReference.IncludeAnnotations.Any())
                {
                    continue;
                }

                // Customer can provide their own built-in vocabulary model
                TextReader referencedJsonReader = _options.ReferencedModelJsonFactory(edmReference.Uri);
                if (referencedJsonReader == null)
                {
                    // If provided, we doesn't need to load it again.
                    referencedJsonReader = LoadBuiltInVocabulary(edmReference.Uri);
                }

                if (referencedJsonReader == null)
                {
                    throw new Exception();
                }

                CsdlJsonModelBuilder builder = new CsdlJsonModelBuilder(referencedJsonReader, _options, sourceUri, mainModel);
                builder.TryParseCsdlJsonModel();
            }
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
        public IList<IEdmReference> ParseReferences(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // The value of $Reference is an object that contains one member per referenced CSDL document.
            JsonObjectValue referenceObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            IList<IEdmReference> references = new List<IEdmReference>(0);

            referenceObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                // The name of the pair is a URI for the referenced document.
                // The URI MAY be relative to the document containing the $Reference.
                IEdmReference result = ParseReferenceObject(propertyName, propertyValue, jsonPath);
                references.Add(result);
            });

            return references;
        }

        public IEdmReference ParseReferenceObject(string url, IJsonValue jsonValue, IJsonPath jsonPath)
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
                        includes = propertyValue.ParseArray(jsonPath, ParseInclude);
                        break;

                    case "$IncludeAnnotations":
                        // The value of $IncludeAnnotations is an array.
                        // Array items are objects that MUST contain the member $TermNamespace and MAY contain the members $Qualifier and $TargetNamespace.
                        includeAnnotations = propertyValue.ParseArray(jsonPath, ParseIncludeAnnotations);
                        break;

                    default:
                        // The reference objects MAY contain annotations.However, EdmReference doesn't support annotation.
                        // So, skip the annotation.

                        propertyValue.ReportUnknownMember(jsonPath, _options);
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
        public IEdmInclude ParseInclude(IJsonValue jsonValue, IJsonPath jsonPath)
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
                        includeAlias = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$Namespace":
                        // The value of $Namespace is a string containing the namespace of the included schema
                        includeNamespace = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    default:
                        // The item objects MAY contain annotations. However, EdmInclude does not supported yet. So skip
                        propertyValue.ReportUnknownMember(jsonPath, _options);
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
        public IEdmIncludeAnnotations ParseIncludeAnnotations(IJsonValue jsonValue, IJsonPath jsonPath)
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
                        termNamespace = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$Qualifier":
                        // The value of $Qualifier is a simple identifier.
                        qualifier = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$TargetNamespace":
                        // The value of $TargetNamespace is a namespace.
                        targetNamespace = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    default:
                        // The item objects MAY contain annotations. However, IEdmIncludeAnnotations doesn't support to have annotations.
                        propertyValue.ReportUnknownMember(jsonPath, _options);
                        break;
                }
            });

            return new EdmIncludeAnnotations(termNamespace, qualifier, targetNamespace);
        }

        private static TextReader LoadBuiltInVocabulary(Uri uri)
        {
            string sourceUri = uri.OriginalString;
            if (sourceUri.Contains("Org.OData.Core.V1"))
            {
                return null;
            }

            return null;
        }

        private static Version ParseVersion(IJsonValue jsonValue, IJsonPath parentPath)
        {
            Debug.Assert(jsonValue != null);

            Version version = null;

            string strVersion = jsonValue.ParseAsStringPrimitive(parentPath);
            if (strVersion == "4.0")
            {
                version = EdmConstants.EdmVersion4;
            }
            else if (strVersion == "4.01")
            {
                version = EdmConstants.EdmVersion401;
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
