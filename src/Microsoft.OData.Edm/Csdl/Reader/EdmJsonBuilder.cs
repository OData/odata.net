//---------------------------------------------------------------------
// <copyright file="CsdlJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    internal class EdmModelBuilder
    {
        private CsdlSerializerOptions _options;
        public EdmModelBuilder(CsdlSerializerOptions options)
        {
            _options = options;
        }

        public IEdmModel TryBuildEdmModel(CsdlJsonModel csdlJsonModel)
        {
            //List<SchemaJsonItem> allSchemaJsonItems = new List<SchemaJsonItem>();
            IDictionary<CsdlJsonSchemaItem, CsdlJsonModel> allSchemaJsonItems = new Dictionary<CsdlJsonSchemaItem, CsdlJsonModel>();

            foreach ( var schema in csdlJsonModel.Schemas)
            {
                foreach (CsdlJsonSchemaItem item in schema.StructuredTypes)
                {
                    allSchemaJsonItems.Add(item, csdlJsonModel);
                }

                foreach (CsdlJsonSchemaItem item in schema.EnumTypes)
                {
                    allSchemaJsonItems.Add(item, csdlJsonModel);
                }
            }

            if (csdlJsonModel.ReferencedModels != null)
            {
                foreach (var referenced in csdlJsonModel.ReferencedModels)
                {
                    foreach (var schema in referenced.Schemas)
                    {
                        foreach (CsdlJsonSchemaItem item in schema.StructuredTypes)
                        {
                            allSchemaJsonItems.Add(item, referenced);
                        }

                        foreach (CsdlJsonSchemaItem item in schema.EnumTypes)
                        {
                            allSchemaJsonItems.Add(item, referenced);
                        }
                    }
                }
            }

            // Build All Structural types
         //   IEnumerable<StructuredTypeJsonItem> structuralTypeJsonItems = allSchemaJsonItems.OfType<StructuredTypeJsonItem>();

            EdmTypeJsonBuilder typeBuilder = new EdmTypeJsonBuilder(allSchemaJsonItems, _options);

            EdmModel mainModel = new EdmModel(false);

            typeBuilder.BuildSchemaItems();

            var allTypes = typeBuilder.BuiltTypes;

            if (csdlJsonModel.ReferencedModels != null)
            {
                foreach (var referenced in csdlJsonModel.ReferencedModels)
                {
                    EdmModel subModel = new EdmModel(false);
                    foreach (var schema in referenced.Schemas)
                    {
                        foreach (CsdlJsonSchemaItem item in schema.StructuredTypes)
                        {
                            IEdmSchemaElement schemaElement = allTypes[item.FullName];
                            subModel.AddElement(schemaElement);
                        }
                    }

                    subModel.SetEdmVersion(referenced.Version);

                    subModel.SetEdmReferences(referenced.CurrentModelReferences);

                    if (referenced.NamespaceAlias != null)
                    {
                        foreach (var item in referenced.NamespaceAlias)
                        {
                            subModel.SetNamespaceAlias(item.Value, item.Key);
                        }
                    }

                    mainModel.AddReferencedModel(subModel);
                }
            }

            foreach (var schema in csdlJsonModel.Schemas)
            {
                foreach (CsdlJsonSchemaItem item in schema.StructuredTypes)
                {
                    IEdmSchemaElement schemaElement = allTypes[item.FullName];
                    mainModel.AddElement(schemaElement);
                }
            }

            mainModel.SetEdmVersion(csdlJsonModel.Version);
            mainModel.SetEdmReferences(csdlJsonModel.CurrentModelReferences);

            if (csdlJsonModel.NamespaceAlias != null)
            {
                foreach (var item in csdlJsonModel.NamespaceAlias)
                {
                    mainModel.SetNamespaceAlias(item.Value, item.Key);
                }
            }

            // Build All Enum Types, TypeDefintions

            // Build All Terms, Actions, Function

            // Build EntityContainer

            // Now, build all bodies

            return mainModel;
        }
    }

#if false
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// </summary>
    internal class EdmModelBuilder2
    {
        private IJsonReader _jsonReader;
        private CsdlSerializerOptions _options;
  //      private EdmModel _mainMain;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The XmlReader for current CSDL doc</param>
        /// <param name="referencedModelFunc">The function to load referenced model xml. If null, will stop loading the referenced model.</param>
        public EdmModelBuilder2(TextReader reader, CsdlSerializerOptions options)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            _jsonReader = new JsonReader(reader, options.GetJsonReaderOptions());
            _options = options;
        //    _mainMain = null;
        }

        //private static void AppendReferenceModel(EdmModel mainModel, IEdmModel referencedModel)
        //{
        //    if (mainModel.ReferencedModels.Any(r => object.ReferenceEquals(r, referencedModel)))
        //    {
        //        return;
        //    }

        //    mainModel.AddReferencedModel(referencedModel);
        //}

        internal IList<IEdmModel> BuildReferencedModels(EdmModel model, IList<IEdmReference> edmReferences)
        {
            if (edmReferences == null || _options.ReferencedModelJsonFactory == null)
            {
                return null;
            }

            List<IEdmModel> referencedModels = new List<IEdmModel>();
            foreach (var edmReference in edmReferences)
            {
                if (!edmReference.Includes.Any() && !edmReference.IncludeAnnotations.Any())
                {
                    //  this.RaiseError(EdmErrorCode.ReferenceElementMustContainAtLeastOneIncludeOrIncludeAnnotationsElement, Strings.EdmxParser_InvalidReferenceIncorrectNumberOfIncludes);
                    continue;
                }

                if (edmReference.Uri != null && (edmReference.Uri.OriginalString.EndsWith(CoreVocabularyConstants.VocabularyUrlSuffix, StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith(CapabilitiesVocabularyConstants.VocabularyUrlSuffix, StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith("/Org.OData.Authorization.V1.xml", StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith("/Org.OData.Validation.V1.xml", StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith("/Org.OData.Community.V1.xml", StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith(AlternateKeysVocabularyConstants.VocabularyUrlSuffix, StringComparison.Ordinal)))
                {
                    continue;
                }

                TextReader referencedJsonReader = _options.ReferencedModelJsonFactory(edmReference.Uri);
                if (referencedJsonReader == null)
                {
                    //  this.RaiseError(EdmErrorCode.UnresolvedReferenceUriInEdmxReference, Strings.EdmxParser_UnresolvedReferenceUriInEdmxReference);
                    continue;
                }

                CsdlSerializerOptions options = _options.Clone();

                options.ReferencedModelJsonFactory = null; // don't parser the sub reference.

                // recursively use CsdlReader to parse sub edm:
                EdmModelBuilder2 builder = new EdmModelBuilder2(referencedJsonReader, options);
          //      builder._mainMain = model;

                string source = edmReference.Uri != null ? edmReference.Uri.OriginalString : null;

                JsonPath jsonPath = new JsonPath(source);
                IEdmModel referencedAstModel = builder.BuildEdmModel(jsonPath);

                //if (!mainCsdlVersion.Equals(referencedEdmxVersion))
                //{
                //    // TODO: REF add exception message
                //    this.errors.Add(null);
                //}

                // referencedAstModel.AddParentModelReferences(edmReference);
                referencedModels.Add(referencedAstModel);
            }

            return referencedModels;
        }

        /// <summary>
        /// Parse the JSON value <see cref="IJsonValue"/> to <see cref="CsdlModel"/>.
        /// </summary>
        /// <param name="jsonValue">The JSON value to parse.</param>
        /// <param name="jsonPath">The JSON path to track on the path of the processed JSON value.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns>Null or the generated <see cref="CsdlModel"/>.</returns>
        internal IEdmModel BuildEdmModel(JsonPath jsonPath)
        {
            // A CSDL JSON document consists of a single JSON object.
            // JsonObjectValue csdlObject = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);
            JsonObjectValue csdlObject = _jsonReader.ReadAsObject();

            IList<IEdmReference> references = null;
            Version version = null;
            IDictionary<string, IJsonValue> members = new Dictionary<string, IJsonValue>();
            csdlObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Version":
                        // This document object MUST contain the member $Version.
                        version = ParseVersion(propertyValue, jsonPath, _options);
                        break;

                    case "$EntityContainer":
                        // The value of $EntityContainer is value is the namespace-qualified name of the entity container of that service.
                        // So far, i don't know how to use it. So skip it.
                        // string entityContainer = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$Reference":
                        // The document object MAY contain the member $Reference to reference other CSDL documents.
                        references = ParseReferences(propertyValue, jsonPath, _options);
                        break;

                    default:
                        // CSDL document also MAY contain members for schemas.
                        // Each schema's value is an object.
                        members[propertyName] = propertyValue;
                        break;
                }
            });

            Debug.Assert(version != null);

            EdmModel model = new EdmModel(includeDefaultVocabularies: false);
            model.SetEdmVersion(version);

            if (references != null)
            {
                model.SetEdmReferences(references);

                foreach (var tmp in references.SelectMany(s => s.Includes))
                {
                    // in any referenced model, alias may point to a further referenced model, now make alias available:
                    model.SetNamespaceAlias(tmp.Namespace, tmp.Alias);
                }
            }

            IList<IEdmModel> referencedModels = BuildReferencedModels(model, references);
            referencedModels.ForEach(m => model.AddReferencedModel(m));

            // TODO: add the built-in vocabulary models, so we can query the types later

            List<SchemaJsonItem> schemaJsonItems = new List<SchemaJsonItem>();
            foreach (var member in members)
            {
                SchemaJsonItemParser schemaJsonParser = new SchemaJsonItemParser(/*model,*/ version, member.Key, _options);
                schemaJsonParser.TryParseCsdlSchema(member.Value, jsonPath);

                schemaJsonItems.AddRange(schemaJsonParser.SchemaItems);
            }

       //     EdmTypeJsonBuilder builder = new EdmTypeJsonBuilder(/*model, _mainMain,*/ schemaJsonItems, _options);
         //   builder.BuildSchemaItems();

            return model;
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
                        includeAlias = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$Namespace":
                        // The value of $Namespace is a string containing the namespace of the included schema
                        includeNamespace = propertyValue.ParseAsStringPrimitive(jsonPath);
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
                        propertyValue.ReportUnknownMember(jsonPath, options);
                        break;
                }
            });

            return new EdmIncludeAnnotations(termNamespace, qualifier, targetNamespace);
        }


        private static Version ParseVersion(IJsonValue jsonValue, JsonPath parentPath, CsdlSerializerOptions options)
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
#endif
}
