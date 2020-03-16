//---------------------------------------------------------------------
// <copyright file="CsdlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Reader
{
    /// <summary>
    /// Provides CSDL parsing services for EDM models.
    /// </summary>
    internal class CsdlJsonReader : CsdlReader
    {
        //private static readonly Dictionary<string, Action> EmptyParserLookup = new Dictionary<string, Action>();
        private readonly IDictionary<string, Action> csdlPropertyParserLookup;

      //  private readonly List<EdmError> errors;
        private readonly List<IEdmReference> edmReferences;
        private string entityContainer;

        private Stack<string> paths;

        ///// <summary>
        ///// Indicates where the document comes from.
        ///// </summary>
        //private string source;

        private IJsonReader _jsonReader;
      //  private JsonReaderOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The XmlReader for current CSDL doc</param>
        /// <param name="referencedModelFunc">The function to load referenced model xml. If null, will stop loading the referenced model.</param>
        public CsdlJsonReader(TextReader reader, JsonReaderOptions options)
        {
            _jsonReader = new JsonReader(reader, options);
     //       this.errors = new List<EdmError>();
            this.edmReferences = new List<IEdmReference>();

            // Setup the edmx parser.
            this.csdlPropertyParserLookup = new Dictionary<string, Action>
            {
              //  { CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Version, this.ParseVersion },
                { CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Reference, this.ParseReference },
                { CsdlConstants.Prefix_Dollar + CsdlConstants.Element_EntityContainer, this.ParseEntityContainer}
            };

            // for the iteration path
            paths = new Stack<string>();
            paths.Push(".");
        }

        public bool TryParse(out IEdmModel model)
        {
            model = null;

            //Version csdlVersion;
            //CsdlModel csdlModel;
            //TryParseCsdlFileToCsdlModel(out csdlVersion, out csdlModel);

            Debug.Assert(this._jsonReader.NodeType == JsonNodeType.None);
            this._jsonReader.Read();

            ParseCsdlDocument();

            return true;
        }

        /// <summary>
        /// Load and parse the referenced model but ignored any further referenced model.
        /// </summary>
        /// <param name="mainCsdlVersion">The main CSDL version.</param>
        /// <returns>A list of CsdlModel (no semantics) of the referenced models.</returns>
        public static List<CsdlModel> LoadAndParseReferencedModel(Version mainCsdlVersion)
        {
            List<CsdlModel> referencedAstModels = new List<CsdlModel>();
            if (this.getReferencedModelReaderFunc == null)
            {
                // don't try to load Edm xml doc, but this.edmReferences's namespace-alias need to be used later.
                return referencedAstModels;
            }

            foreach (var edmReference in this.edmReferences)
            {
                if (!edmReference.Includes.Any() && !edmReference.IncludeAnnotations.Any())
                {
                    this.RaiseError(EdmErrorCode.ReferenceElementMustContainAtLeastOneIncludeOrIncludeAnnotationsElement, Strings.EdmxParser_InvalidReferenceIncorrectNumberOfIncludes);
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

                XmlReader referencedXmlReader = this.getReferencedModelReaderFunc(edmReference.Uri);
                if (referencedXmlReader == null)
                {
                    this.RaiseError(EdmErrorCode.UnresolvedReferenceUriInEdmxReference, Strings.EdmxParser_UnresolvedReferenceUriInEdmxReference);
                    continue;
                }

                // recursively use CsdlReader to parse sub edm:
                CsdlReader referencedEdmxReader = new CsdlReader(referencedXmlReader, /*getReferencedModelReaderFunc*/ null);
                referencedEdmxReader.source = edmReference.Uri != null ? edmReference.Uri.OriginalString : null;
                referencedEdmxReader.ignoreUnexpectedAttributesAndElements = this.ignoreUnexpectedAttributesAndElements;
                Version referencedEdmxVersion;
                CsdlModel referencedAstModel;
                if (referencedEdmxReader.TryParseCsdlFileToCsdlModel(out referencedEdmxVersion, out referencedAstModel))
                {
                    if (!mainCsdlVersion.Equals(referencedEdmxVersion))
                    {
                        // TODO: REF add exception message
                        this.errors.Add(null);
                    }

                    referencedAstModel.AddParentModelReferences(edmReference);
                    referencedAstModels.Add(referencedAstModel);
                }

                this.errors.AddRange(referencedEdmxReader.errors);
            }

            return referencedAstModels;
        }

        public static CsdlModel BuildCsdlModel(IJsonValue jsonValue)
        {
            // A CSDL JSON document consists of a single JSON object.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            JsonObjectValue csdlObject = (JsonObjectValue)jsonValue;

            // $Version
            string strVersion = null;
            Version version;
            csdlObject.ProcessProperty("$Version", v => strVersion = v.ParseAsStringPrimitive());
            if (strVersion == "4.0")
            {
                version = EdmConstants.EdmVersion4;
            }
            else if (strVersion == "4.01")
            {
                version = EdmConstants.EdmVersion401;
            }
            else
            {
                // The value of $Version is a string containing either 4.0 or 4.01.
                // So far, it only supports 4.0 and 4.01, so for other, throw
                throw new Exception();
            }

            CsdlModel csdlModel = new CsdlModel();

            IList<IEdmReference> references = null;
            string entityContainer = null;
            foreach (var property in csdlObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Version":
                        // Processed, skip it
                        break;

                    case "$EntityContainer":
                        // The value of $EntityContainer is value is the namespace-qualified name of the entity container of that service.
                        // So far, i don't know how to use it.
                        entityContainer = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$Reference":
                        // The document object MAY contain the member $Reference to reference other CSDL documents.
                        references = BuildCsdlReferences(propertyValue);
                        break;

                    default:
                        CsdlSchema schema = SchemaJsonReader.BuildCsdlSchema(propertyName, version, propertyValue);
                        if (schema != null)
                        {
                            csdlModel.AddSchema(schema);
                            break;
                        }

                        break;
                }
            }

            CsdlLocation location = new CsdlLocation(-1, -1);

            if (references != null)
            {
                csdlModel.AddCurrentModelReferences(references);
            }

            return csdlModel;
        }

        public static IList<IEdmReference> BuildCsdlReferences(IJsonValue jsonValue)
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

                IEdmReference result = BuildReferenceObject(url, propertyValue);
                references.Add(result);
            }

            return references;
        }

        public static IEdmReference BuildReferenceObject(string url, IJsonValue jsonValue)
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

                // The reference object MAY contain the members $Include and $IncludeAnnotations as well as annotations.
                switch (propertyName)
                {
                    case "$Include":
                        // The value of $Include is an array. Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                        if (propertyValue.ValueKind != JsonValueKind.JArray)
                        {
                            throw new Exception();
                        }

                        JsonArrayValue arrayValue = (JsonArrayValue)propertyValue;
                        foreach (IJsonValue item in arrayValue)
                        {
                            IEdmInclude include = BuildInclude(item);
                            edmReference.AddInclude(include);
                        }
                        break;

                    case "$IncludeAnnotations":
                        // The value of $IncludeAnnotations is an array.
                        // Array items are objects that MUST contain the member $TermNamespace and MAY contain the members $Qualifier and $TargetNamespace.
                        if (propertyValue.ValueKind != JsonValueKind.JArray)
                        {
                            throw new Exception();
                        }

                        JsonArrayValue includeAnnotationsArray = (JsonArrayValue)propertyValue;
                        foreach (IJsonValue item in includeAnnotationsArray)
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

        private void ParseEntityContainer()
        {
            if (this._jsonReader.NodeType != JsonNodeType.PrimitiveValue)
            {
                throw new Exception();
            }

            this.entityContainer = this._jsonReader.ReadStringValue();
            Debug.Assert(this.entityContainer != null);
        }

        private Version ParseCsdlDocument()
        {
            Debug.Assert(this._jsonReader.NodeType == JsonNodeType.StartObject);

            // Pass the "{" tag.
            this._jsonReader.Read();

            while (this._jsonReader.NodeType != JsonNodeType.EndOfInput)
            {
                if (this._jsonReader.NodeType == JsonNodeType.Property)
                {
                    // Get the property name and move json reader to next token
                    string propertyName = this._jsonReader.ReadPropertyName();
                    if (this.csdlPropertyParserLookup.ContainsKey(propertyName))
                    {
                        this.csdlPropertyParserLookup[propertyName]();
                    }
                    else
                    {
                        this.ParseCsdlSchema(propertyName);
                    }
                }
                else
                {
                    if (!this._jsonReader.Read())
                    {
                        break;
                    }
                }
            }

            // Consume the "}" tag.
            this._jsonReader.Read();

            return null;
        }

        private void ParseReference()
        {
            if (this._jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new Exception();
            }
        }

        private void ParseCsdlSchema(string namesp)
        {
            Debug.Assert(this._jsonReader.NodeType == JsonNodeType.StartObject);

            this._jsonReader.ReadStartObject();
        }
    }
}
