//---------------------------------------------------------------------
// <copyright file="CsdlJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Reader
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// </summary>
    internal class CsdlJsonReader : CsdlReader
    {
  
      //  private readonly List<EdmError> errors;
      //  private string entityContainer;


        ///// <summary>
        ///// Indicates where the document comes from.
        ///// </summary>
        //private string source;

        private IJsonReader _jsonReader;
        private CsdlSerializerOptions _options;

        private readonly IEdmModel _edmModel;

        public CsdlJsonReader(TextReader reader)
            : this (reader, CsdlSerializerOptions.DefaultOptions)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The XmlReader for current CSDL doc</param>
        /// <param name="referencedModelFunc">The function to load referenced model xml. If null, will stop loading the referenced model.</param>
        public CsdlJsonReader(TextReader reader, CsdlSerializerOptions options)
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
            _edmModel = null;
        }

        public IEdmModel BuildEdmModel()
        {
            // If parsed, just return the model.
            if (_edmModel != null)
            {
                return _edmModel;
            }

            // Read the whole CSDL into "JsonObjectValue".
            // stream ==> IJsonValue, following up the JSON rules
            JsonObjectValue csdlObject = _jsonReader.ReadAsObject();
            Debug.Assert(csdlObject != null);

            // CSDL Syntax parser, parse JSON object into CSDL model
            CsdlModel csdlModel = ParseCsdlModel(csdlObject);

            List<CsdlModel> referencedCsdlModels = this.BuildReferencedModel(csdlModel.CurrentModelReferences);

            if (referencedCsdlModels == null)
            {

            }
            // CSDL Semantic binder, bind CSDL model to IEdmModel.

            //ParseCsdlDocument();

            return _edmModel;
        }

        /// <summary>
        /// Load and parse the referenced model but ignored any further referenced model.
        /// </summary>
        /// <param name="mainCsdlVersion">The main CSDL version.</param>
        /// <returns>A list of CsdlModel (no semantics) of the referenced models.</returns>
        public List<CsdlModel> BuildReferencedModel(IEnumerable<IEdmReference> references)
        {
            if (_options.ReferencedModelJsonFactory == null)
            {
                return null;
            }

            List<CsdlModel> referencedAstModels = new List<CsdlModel>();
            foreach (var edmReference in references)
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

                // recursively use CsdlReader to parse sub edm:
                IJsonReader jsonReader = new JsonReader(referencedJsonReader, _options.GetJsonReaderOptions());

                JsonObjectValue objValue = jsonReader.ReadAsObject();

                CsdlModel referencedAstModel = ParseCsdlModel(objValue);

                //if (!mainCsdlVersion.Equals(referencedEdmxVersion))
                //{
                //    // TODO: REF add exception message
                //    this.errors.Add(null);
                //}

                referencedAstModel.AddParentModelReferences(edmReference);
                referencedAstModels.Add(referencedAstModel);

                //this.errors.AddRange(referencedEdmxReader.errors);
            }

            return referencedAstModels;
        }

        private static Version GetCsdlVersion(JsonObjectValue csdlObject)
        {
            Debug.Assert(csdlObject != null);

            IJsonValue versionValue;
            if (csdlObject.TryGetValue("$Version", out versionValue))
            {
                string strVersion = versionValue.ParseAsStringPrimitive();
                if (strVersion == "4.0")
                {
                    return EdmConstants.EdmVersion4;
                }
                else if (strVersion == "4.01")
                {
                    return EdmConstants.EdmVersion401;
                }
            }

            // The value of $Version is a string containing either 4.0 or 4.01.
            // So far, it only supports 4.0 and 4.01, so for other, throw
            throw new Exception();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="csdlObject"></param>
        /// <returns></returns>
        internal CsdlModel ParseCsdlModel(JsonObjectValue csdlObject)
        {
            // A CSDL JSON document consists of a single JSON object.
            // This document object MUST contain the member $Version.
            Version version = GetCsdlVersion(csdlObject);

            CsdlModel csdlModel = new CsdlModel
            {
                Version = version
            };

            JsonPath jsonPath = new JsonPath(_options.GetJsonPathOptions());
            IList<IEdmReference> references = null;
            foreach (var property in csdlObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                jsonPath.Push(propertyName);
                switch (propertyName)
                {
                    case "$Version":
                        // Processed above, so skip it.
                        break;

                    case "$EntityContainer":
                        // The value of $EntityContainer is value is the namespace-qualified name of the entity container of that service.
                        // So far, i don't know how to use it. So skip it.
                        // string entityContainer = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$Reference":
                        // The document object MAY contain the member $Reference to reference other CSDL documents.
                        
                        references = ParseCsdlReferences(propertyValue, jsonPath);
                        
                        break;

                    default:
                        if (propertyValue.ValueKind == JsonValueKind.JObject)
                        {
                            // for all others, it maybe the CsdlSchema
                            CsdlSchema schema = SchemaJsonReader.BuildCsdlSchema(propertyName, version, propertyValue, jsonPath);
                            if (schema != null)
                            {
                                csdlModel.AddSchema(schema);
                                break;
                            }
                        }

                        if (_options.IgnoreUnexpectedAttributesAndElements)
                        {
                            break;
                        }

                        break;
                }

                jsonPath.Pop();
            }

            //CsdlLocation location = new CsdlLocation(-1, -1);

            if (references != null)
            {
                csdlModel.AddCurrentModelReferences(references);
            }

            return csdlModel;
        }

        public static IList<IEdmReference> ParseCsdlReferences(IJsonValue jsonValue, JsonPath jsonPath)
        {
            // The value of $Reference is an object that contains one member per referenced CSDL document.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            IList<IEdmReference> references = new List<IEdmReference>(0);
            JsonObjectValue referenceObj = (JsonObjectValue)jsonValue;
            foreach (var property in referenceObj)
            {
                // The name of the pair is a URI for the referenced document.
                // The URI MAY be relative to the document containing the $Reference. 
                string url = property.Key;

                // The value of each member is a reference object.
                IJsonValue propertyValue = property.Value;

                jsonPath.Push(url);

                IEdmReference result = ParseReferenceObject(url, propertyValue, jsonPath);
                references.Add(result);

                jsonPath.Pop();
            }

            return references;
        }

        public static IEdmReference ParseReferenceObject(string url, IJsonValue jsonValue, JsonPath jsonPath)
        {
            // The reference object MAY contain the members $Include and $IncludeAnnotations as well as annotations.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            EdmReference edmReference = new EdmReference(new Uri(url, UriKind.RelativeOrAbsolute));
            JsonObjectValue referenceObj = (JsonObjectValue)jsonValue;
            foreach (var property in referenceObj)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;
                JsonArrayValue arrayValue = propertyValue as JsonArrayValue;

                jsonPath.Push(propertyName);

                // The reference object MAY contain the members $Include and $IncludeAnnotations as well as annotations.
                switch (propertyName)
                {
                    case "$Include":
                        // The value of $Include is an array. Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                        if (arrayValue == null)
                        {
                            throw new Exception();
                        }

                        foreach (IJsonValue item in arrayValue)
                        {
                            IEdmInclude include = BuildInclude(item);
                            edmReference.AddInclude(include);
                        }
                        break;

                    case "$IncludeAnnotations":
                        // The value of $IncludeAnnotations is an array.
                        // Array items are objects that MUST contain the member $TermNamespace and MAY contain the members $Qualifier and $TargetNamespace.
                        if (arrayValue == null)
                        {
                            throw new Exception();
                        }

                        foreach (IJsonValue item in arrayValue)
                        {
                            IEdmIncludeAnnotations includeAnnotations = BuildIncludeAnnotations(item);
                            edmReference.AddIncludeAnnotations(includeAnnotations);
                        }
                        break;

                    default:
                        // The item objects MAY contain annotations.
                        // However, it's not supported yet. so skip it.
                        break;

                }

                jsonPath.Pop();
            }

            return edmReference;
        }

        public static IEdmInclude BuildInclude(IJsonValue jsonValue)
        {
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                throw new Exception();
            }

            JsonObjectValue includeObj = (JsonObjectValue)jsonValue;
            string includeNamespace = null;
            string includeAlias = null;
            foreach (var property in includeObj)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the included schema.
                        includeAlias = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$Namespace":
                        // The value of $Namespace is a string containing the namespace of the included schema
                        includeNamespace = propertyValue.ParseAsStringPrimitive();
                        break;

                    default:
                        // The item objects MAY contain annotations.
                        // However, it's not supported yet. so skip it.
                        break;
                }
            }

            return new EdmInclude(includeAlias, includeNamespace);
        }

        public static IEdmIncludeAnnotations BuildIncludeAnnotations(IJsonValue jsonValue)
        {
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                throw new Exception();
            }

            JsonObjectValue includeObj = (JsonObjectValue)jsonValue;
            string termNamespace = null;
            string qualifier = null;
            string targetNamespace = null;
            foreach (var property in includeObj)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                // Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                switch (propertyName)
                {
                    case "$TermNamespace":
                        // The value of $TermNamespace is a namespace.
                        termNamespace = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$Qualifier":
                        // The value of $Qualifier is a simple identifier.
                        qualifier = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$TargetNamespace":
                        // The value of $TargetNamespace is a namespace.
                        targetNamespace = propertyValue.ParseAsStringPrimitive();
                        break;

                    default:
                        // The item objects MAY contain annotations.
                        // However, it's not supported yet. so skip it.
                        break;
                }
            }

            return new EdmIncludeAnnotations(termNamespace, qualifier, targetNamespace);
        }

#if false
        internal static IEdmModel BuildEdmModel(JsonObjectValue csdlOject)
        {
            EdmModel model = new EdmModel();

            SetEdmVersion(model, csdlOject);

            SetEntityContainer(model, csdlOject);

            return model;
        }

        private static void SetEdmVersion(IEdmModel model, JsonObjectValue csdlObject)
        {
            // $version
            string propertyName = CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Version;

            IJsonValue value;
            if (!csdlObject.TryGetValue(propertyName, out value))
            {
                // Throw exception, or add error into log
                throw new Exception();
            }

            if (value.ValueType != JsonValueType.JPrimitive)
            {
                throw new Exception();
            }

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)value;
            string strVersion = primitiveValue.Value as string; 

            if (strVersion == "4.0")
            {
                model.SetEdmVersion(EdmConstants.EdmVersion4);
            }
            else if (strVersion == "4.01")
            {
                model.SetEdmVersion(EdmConstants.EdmVersion401);
            }
            else
            {
                // The value of $Version is a string containing either 4.0 or 4.01.
                // So far, it only supports 4.0 and 4.01, so for other, throw
                throw new Exception();
            }
        }

        // If the CSDL JSON document is the metadata document of an OData service,
        // the document object MUST contain the member $EntityContainer.
        // The value of $EntityContainer is value is the namespace-qualified name of the entity container of that service.
        // This is the only place where a model element MUST be referenced with its namespace-qualified name and use of the alias-qualified name is not allowed
        private static void SetEntityContainer(IEdmModel model, JsonObjectValue csdlObject)
        {
            // $EntityContainer
            string propertyName = CsdlConstants.Prefix_Dollar + CsdlConstants.Element_EntityContainer;

            IJsonValue value;
            if (!csdlObject.TryGetValue(propertyName, out value))
            {
                return;
            }
        }


        private void SetReference(IEdmModel model, JsonObjectValue csdlObject)
        {
            // $Reference
            string propertyName = CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Reference;

            IJsonValue value;
            if (!csdlObject.TryGetValue(propertyName, out value))
            {
                return;
            }

          //  model.AddReferencedModel
        }

#endif

    }
}
