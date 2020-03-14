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
        //private readonly Dictionary<string, Action> runtimeParserLookup;
        //private readonly Dictionary<string, Action> conceptualModelsParserLookup;
        //private readonly Dictionary<string, Action> dataServicesParserLookup;
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

        //public IEdmModel Parse()
        //{
        //    JsonObjectValue csdlOject = _jsonReader.ReadAsObject();

//    if (_options.ReaderAsInmutableModel)
//    {
//        BuildEdmModel(csdlOject);
//    }
//    else
//    {

//    }
//}
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

        /// <summary>
        /// Parse CSDL xml doc into CsdlModel, error messages are stored in this.errors.
        /// </summary>
        /// <param name="csdlVersion">The csdlVersion out.</param>
        /// <param name="csdlModel">The CsdlModel out.</param>
        /// <returns>True if succeeded.</returns>
        internal bool TryParseCsdlFileToCsdlModel(out Version csdlVersion, out CsdlModel csdlModel)
        {
            csdlVersion = null;
            csdlModel = null;
            try
            {
                // CSDL JSON document consists of a single JSON object.
                if (this._jsonReader.NodeType != JsonNodeType.StartObject)
                {
                    while (this._jsonReader.Read() && this._jsonReader.NodeType != JsonNodeType.StartObject)
                    {
                    }
                }

                // There must be a root element for all current artifacts
                if (this._jsonReader.NodeType == JsonNodeType.EndOfInput)
                {
                   // this.RaiseEmptyFile();
                    return false;
                }

                //if (this.jsonReader.LocalName != CsdlConstants.Element_Edmx ||
                //    !CsdlConstants.SupportedEdmxNamespaces.TryGetValue(this.reader.NamespaceURI, out csdlVersion))
                //{
                //    this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Edm.Strings.XmlParser_UnexpectedRootElement(this.reader.Name, CsdlConstants.Element_Edmx));
                //    return false;
                //}

                //csdlVersion = this.ParseEdmxElement(csdlVersion);
                //IEnumerable<EdmError> err;
                //if (!this.csdlParser.GetResult(out csdlModel, out err))
                //{
                //    this.errors.AddRange(err);
                //    if (this.HasIntolerableError())
                //    {
                //        return false;
                //    }
                //}
            }
            catch (XmlException e)
            {
                string a = e.Message;
                //  this.errors.Add(new EdmError(new CsdlLocation(this.source, e.LineNumber, e.LinePosition), EdmErrorCode.XmlError, e.Message));
                return a != null;
            }

            csdlModel.AddCurrentModelReferences(this.edmReferences);
            return true;
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
