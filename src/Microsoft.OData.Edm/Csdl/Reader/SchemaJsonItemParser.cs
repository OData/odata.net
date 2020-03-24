//---------------------------------------------------------------------
// <copyright file="SchemaJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Csdl.Json.Value;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides functionalities for parsing Schema JSON for Csdl elements.
    /// </summary>
    internal class SchemaJsonItemParser
    {
        private Version _version;
        private string _schemaNamespace;
        private CsdlSerializerOptions _options;

        public SchemaJsonItemParser(Version version, string schemaNamespace, CsdlSerializerOptions options)
        {
            _version = version;
            _schemaNamespace = schemaNamespace;
            _options = options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaNamespace"></param>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <param name="version"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public CsdlJsonSchema TryParseCsdlJsonSchema(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            // Its value is an object.
            JsonObjectValue schemaObject = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            CsdlJsonSchema schema = new CsdlJsonSchema(_schemaNamespace);
            schemaObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the schema.
                        // If a schema specifies an alias, the alias MUST be used instead of the namespace within qualified names
                        // throughout the document to identify model elements of that schema.
                        // A mixed use of namespace-qualified names and alias-qualified names is not allowed.
                        schema.Alias = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$Annotations":
                        // The value of $Annotations is an object with one member per annotation target.
                        schema.OutOfLineAnnotations = propertyValue;
                        break;

                    default:
                        CsdlJsonSchemaItem item = ParseSchemaElement(propertyName, propertyValue, jsonPath);
                        if (item != null)
                        {
                            schema.Add(item);
                        }
                        break;
                }
            });

            return schema;
        }

        /// <summary>
        /// The schema object MAY contain members representing:
        /// entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
        /// This method trys to build the JSON value as one of the above member.
        /// </summary>
        /// <param name="name">The name of the schema member.</param>
        /// <param name="jsonValue">The schema member json value.</param>
        /// <param name="jsonPath">The JSON path.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>Null or built element.</returns>
        public CsdlJsonSchemaItem ParseSchemaElement(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (_version != EdmConstants.EdmVersion4 || _schemaNamespace == null)
            {
                throw new Exception(name);
            }

            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            // The schema object member is representing entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
            // The JSON value of each member is a JSON object, if it's not an object, it's not a schema element.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                jsonValue.ReportUnknownMember(jsonPath, _options);
                return null;
            }

            JsonObjectValue schemaElementObject = (JsonObjectValue)jsonValue;

            CsdlJsonSchemaItem jsonItem;
            // Each schema member oject should include "$Kind" member, whose value is a string
            string kind = GetKind(schemaElementObject, jsonPath);
            switch (kind)
            {
                case "EntityContainer":
                    jsonItem = new CsdlJsonSchemaEntityContainerItem
                    {
                        JsonPath = jsonPath,
                     //   JsonValue = jsonValue,
                    };
                    break;

                case "EntityType":
                    jsonItem = ParseCsdlEntityType(name, schemaElementObject, jsonPath);
                    break;

                case "ComplexType":
                    jsonItem = new CsdlJsonSchemaComplexItem();
                    break;

                case "EnumType":
                    jsonItem = new CsdlJsonSchemaEnumItem();
                    break;

                case "TypeDefinition":
                    jsonItem = new CsdlJsonSchemaTypeDefinitionItem();
                    break;

                case "Term":
                    jsonItem = new CsdlJsonSchemaTermItem();
                    break;

                default:
                    // If there's no "$Kind" or unknow kind, it's not a schema element
                    jsonValue.ReportUnknownMember(jsonPath, _options);
                    return null;
            }

            jsonItem.Namespace = _schemaNamespace;
            jsonItem.Name = name;
            jsonItem.JsonPath = jsonPath;
            return jsonItem;
        }

        /// <summary>
        /// An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public static CsdlJsonSchemaEntityItem ParseCsdlEntityType(string name, JsonObjectValue entityObject, IJsonPath jsonPath)
        {
            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            CsdlJsonSchemaEntityItem entityTypeItem = new CsdlJsonSchemaEntityItem();
            entityTypeItem.Name = name;
            foreach (var property in entityObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    // $Kind
                    case "$Kind":
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    // $BaseType
                    case "$BaseType":
                        string baseType = propertyValue.ParseAsStringPrimitive(jsonPath);
                        entityTypeItem.BaseType = baseType;
                        break;

                    // $Abstract
                    case "$Abstract":
                        entityTypeItem.IsAbstract = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        break;

                    // $OpenType
                    case "$OpenType":
                        entityTypeItem.IsOpen = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        break;

                    // $HasStream
                    case "$HasStream":
                        entityTypeItem.HasStream = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        break;

                    default:
                        entityTypeItem.AddMember(propertyName, propertyValue);
                        break;
                }
            }

            return entityTypeItem;
        }

        private static string GetKind(JsonObjectValue objValue, IJsonPath jsonPath)
        {
            string kind = null;
            IJsonValue kindValue;
            if (objValue.TryGetValue("$Kind", out kindValue))
            {
                jsonPath.Push("$Kind");
                kind = kindValue.ParseAsStringPrimitive(jsonPath);
                jsonPath.Pop();
            }

            return kind;
        }
    }
}
