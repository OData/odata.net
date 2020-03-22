//---------------------------------------------------------------------
// <copyright file="SchemaJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    internal enum SchemaMemberKind
    {
        Entity,
        Complex,
        Enum,
        TypeDefinition,
        Action,
        Function,
        Term,
        EntityContainer,
        OutOfLineAnnotations
    }
    internal abstract class SchemaJsonItem
    {
        public SchemaJsonItem()
        {
            Members = new Dictionary<string, IJsonValue>();
        }

        private string _fullName;
        public string FullName
        {
            get
            {
                if (_fullName == null)
                {
                    _fullName = Namespace + "." + Name;
                }
                return _fullName;
            }
        }
        public string Namespace { get; set; }

        public string Name { get; set; }

        public abstract SchemaMemberKind Kind { get; }

        public IJsonValue JsonValue { get; set; }

        public IDictionary<string, IJsonValue> Members { get; }

        public IJsonPath JsonPath { get; set; }

        public void AddMember(string name, IJsonValue value)
        {
            Members[name] = value;
        }
    }

    internal class EntityContainerJsonItem : SchemaJsonItem
    {
        public string Extends { get; set; }
        public override SchemaMemberKind Kind => SchemaMemberKind.EntityContainer;
    }

    internal class OutofLineAnnotationsJsonItem : SchemaJsonItem
    {

        public override SchemaMemberKind Kind => SchemaMemberKind.Term;
    }

    internal class TermJsonItem : SchemaJsonItem
    {
        public string QualifiedTypeName { get; set; }

        public bool IsCollection { get; set; }

        public string DefaultValue { get; set; }

        public string AppliesTo { get; set; }

        public bool Nulable { get; set; }

        public int? MaxLength { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public int? Srid { get; set; }

        // 4.01 and greater payloads
        public bool? Unicode { get; set; }


        public override SchemaMemberKind Kind => SchemaMemberKind.OutOfLineAnnotations;
    }


    internal abstract class OperationJsonItem : SchemaJsonItem
    {

    }

    internal class ActionJsonItem : OperationJsonItem
    {

        public override SchemaMemberKind Kind => SchemaMemberKind.Action;
    }

    internal class FunctionJsonItem : OperationJsonItem
    {

        public override SchemaMemberKind Kind => SchemaMemberKind.Function;
    }

    internal class TypeDefinitionJsonItem : SchemaJsonItem
    {
        public string UnderlyingTypeName { get; set; }

        public override SchemaMemberKind Kind => SchemaMemberKind.TypeDefinition;
    }

    internal class EnumTypeJsonItem : SchemaJsonItem
    {
        public string UnderlyingTypeName { get; set; }

        public override SchemaMemberKind Kind => SchemaMemberKind.Enum;

        public bool IsFlags { get; set; }
    }

    internal abstract class StructuredTypeJsonItem : SchemaJsonItem
    {
        public string BaseType { get; set; }

        public bool IsAbstract { get; set; }
        public bool IsOpen { get; set; }
    }

    internal class ComplexTypeJsonItem : StructuredTypeJsonItem
    {
        public override SchemaMemberKind Kind => SchemaMemberKind.Complex;
    }

    internal class EntityTypeJsonItem : StructuredTypeJsonItem
    {
        public override SchemaMemberKind Kind => SchemaMemberKind.Entity;

        public bool HasStream { get; set; }
    }

    /// <summary>
    /// Provides functionalities for parsing Schema JSON for Csdl elements.
    /// </summary>
    internal class SchemaJsonItemParser
    {
        private IList<SchemaJsonItem> _schemaJsonItems;

      //  private IEdmModel _model;
        private Version _version;
        private string _schemaNamespace;
        private CsdlSerializerOptions _options;

        public SchemaJsonItemParser(/*IEdmModel model, */Version version, string schemaNamespace, CsdlSerializerOptions options)
        {
            _schemaJsonItems = new List<SchemaJsonItem>();
          //  _model = model;
            _version = version;
            _schemaNamespace = schemaNamespace;
            _options = options;
        }

        public IList<SchemaJsonItem> SchemaItems { get { return _schemaJsonItems; } }

        private string _alias;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaNamespace"></param>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <param name="version"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public void TryParseCsdlSchema(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            // Its value is an object.
            JsonObjectValue schemaObject = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

         //   IList<CsdlAnnotations> outOfLineAnnotations = new List<CsdlAnnotations>();
         //   IList<CsdlElement> csdlElements = new List<CsdlElement>();
            string alias = null;
            schemaObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the schema.
                        alias = propertyValue.ParseAsStringPrimitive(jsonPath);
                        _alias = alias;
                        break;

                    case "$Annotations":
                        // The value of $Annotations is an object with one member per annotation target.
                        _schemaJsonItems.Add(new OutofLineAnnotationsJsonItem { });
                        break;

                    default:
                        ParseSchemaElement(propertyName, propertyValue, jsonPath);
                        break;
                }
            });

            return;
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
        public void ParseSchemaElement(string name, IJsonValue jsonValue, IJsonPath jsonPath)
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
                return;
            }

            JsonObjectValue schemaElementObject = (JsonObjectValue)jsonValue;

            SchemaJsonItem jsonItem;
            // Each schema member oject should include "$Kind" member, whose value is a string
            string kind = GetKind(schemaElementObject, jsonPath);
            switch (kind)
            {
                case "EntityContainer":
                    jsonItem = new EntityContainerJsonItem
                    {
                        JsonPath = jsonPath,
                        JsonValue = jsonValue,
                    };
                    break;

                case "EntityType":
                    jsonItem = ParseCsdlEntityType(name, schemaElementObject, jsonPath);
                    break;

                case "ComplexType":
                    jsonItem = new ComplexTypeJsonItem();
                    break;

                case "EnumType":
                    jsonItem = new EnumTypeJsonItem();
                    break;

                case "TypeDefinition":
                    jsonItem = new TypeDefinitionJsonItem();
                    break;

                case "Term":
                    jsonItem = new TermJsonItem();
                    break;

                default:
                    // If there's no "$Kind" or unknow kind, it's not a schema element
                    jsonValue.ReportUnknownMember(jsonPath, _options);
                    return;
            }

            jsonItem.Namespace = _schemaNamespace;
            jsonItem.Name = name;
            _schemaJsonItems.Add(jsonItem);
        }

        /// <summary>
        /// An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public EntityTypeJsonItem ParseCsdlEntityType(string name, JsonObjectValue entityObject, IJsonPath jsonPath)
        {
            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            EntityTypeJsonItem entityTypeItem = new EntityTypeJsonItem();
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
                        baseType.Replace(_alias, _schemaNamespace);
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
