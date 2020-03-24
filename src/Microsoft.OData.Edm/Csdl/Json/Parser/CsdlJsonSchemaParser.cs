//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Csdl.Json.Value;

namespace Microsoft.OData.Edm.Csdl.Json.Parser
{
    /// <summary>
    /// Provides functionalities for parsing Schema JSON for Csdl elements.
    /// </summary>
    internal class CsdlJsonSchemaParser
    {
        private Version _version;
        private string _schemaNamespace;
        private CsdlSerializerOptions _options;

        public CsdlJsonSchemaParser(Version version, string schemaNamespace, CsdlSerializerOptions options)
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
                        if (propertyValue.ValueKind == JsonValueKind.JArray)
                        {
                            var operations = BuildCsdlOperationOverload(propertyName, propertyValue, jsonPath);
                            operations.ForEach(v => { v.Namespace = _schemaNamespace; schema.Add(v); });
                        }
                        else
                        {
                            CsdlJsonSchemaItem item = ParseSchemaElement(propertyName, propertyValue, jsonPath);
                            if (item != null)
                            {
                                schema.Add(item);
                            }
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

            CsdlJsonSchemaItem schemaItem;
            // Each schema member oject should include "$Kind" member, whose value is a string
            string kind = GetKind(schemaElementObject, jsonPath);
            switch (kind)
            {
                case "EntityContainer":
                    schemaItem = ParseCsdlEntityContainer(name, schemaElementObject, jsonPath);
                    break;

                case "EntityType":
                    schemaItem = ParseCsdlEntityType(name, schemaElementObject, jsonPath);
                    break;

                case "ComplexType":
                    schemaItem = BuildCsdlComplexType(name, schemaElementObject, jsonPath);
                    break;

                case "EnumType":
                    schemaItem = BuildCsdlEnumType(name, schemaElementObject, jsonPath);
                    break;

                case "TypeDefinition":
                    schemaItem = BuildCsdlTyeDefinition(name, schemaElementObject, jsonPath);
                    break;

                case "Term":
                    schemaItem = BuildCsdlTermType(name, schemaElementObject, jsonPath);
                    break;

                //case "Action":
                //    schemaItem = new CsdlJsonSchemaActionItem();
                //    break;

                //case "Function":
                //    schemaItem = new CsdlJsonSchemaFunctionItem();
                //    break;

                default:
                    // If there's no "$Kind" or unknow kind, it's not a schema element
                    jsonValue.ReportUnknownMember(jsonPath, _options);
                    return null;
            }

            schemaItem.Namespace = _schemaNamespace;
            schemaItem.Name = name;
            schemaItem.JsonPath = jsonPath;
            return schemaItem;
        }

        /// <summary>
        /// An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public static CsdlJsonSchemaEntityContainerItem ParseCsdlEntityContainer(string name, JsonObjectValue entityObject, IJsonPath jsonPath)
        {
            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            CsdlJsonSchemaEntityContainerItem containerItem = new CsdlJsonSchemaEntityContainerItem();
            containerItem.Name = name;

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

                    // $Extends
                    case "$Extends":
                        // The value of $Extends is the qualified name of the entity container to be extended.
                        containerItem.Extends = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    default:
                        containerItem.AddMember(propertyName, propertyValue);
                        break;
                }
            }

            return containerItem;
        }

        public static CsdlJsonSchemaEnumItem BuildCsdlEnumType(string name, JsonObjectValue enumObject, IJsonPath jsonPath)
        {
            // Enumeration Type Object
            // An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type and whose value is an object.
            // The enumeration type object MUST contain the member $Kind with a string value of EnumType.
            // It MAY contain the members $UnderlyingType and $IsFlags.
            // The enumeration type object MUST contain members representing the enumeration type members.
            // The enumeration type object MAY contain annotations.
            CsdlJsonSchemaEnumItem enumItem = new CsdlJsonSchemaEnumItem();
            enumItem.Name = name;
            foreach (var property in enumObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    // $UnderlyingType
                    case "$UnderlyingType":
                        enumItem.UnderlyingTypeName = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    // $IsFlags
                    case "$IsFlags":
                        enumItem.IsFlags = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        break;

                    default:
                        enumItem.AddMember(propertyName, propertyValue);
                        break;
                }
            }

            return enumItem;
        }


        public static CsdlJsonSchemaTypeDefinitionItem BuildCsdlTyeDefinition(string name, JsonObjectValue typeDefinitionObject, IJsonPath jsonPath)
        {
            // A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition and whose value is an object.
            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition and the member $UnderlyingType.
            // It MAY contain the members $MaxLength, $Unicode, $Precision, $Scale, and $SRID, and it MAY contain annotations.

            CsdlJsonSchemaTypeDefinitionItem typeDefinitionItem = new CsdlJsonSchemaTypeDefinitionItem();
            typeDefinitionItem.Name = name;

            foreach (var property in typeDefinitionObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of NavigationProperty
                        break;

                    case "$UnderlyingType":
                        // Why does it can't be omit for "Edm.String"?
                        // The value of $UnderlyingType is the qualified name of the underlying type.
                        typeDefinitionItem.UnderlyingTypeName = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        typeDefinitionItem.MaxLength = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        typeDefinitionItem.Precision = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        typeDefinitionItem.Scale = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$SRID":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        typeDefinitionItem.Srid = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        typeDefinitionItem.Unicode = propertyValue.ParseAsBooleanPrimitive(jsonPath);
                        break;

                    default:
                        typeDefinitionItem.AddMember(propertyName, propertyValue);
                        break;
                }
            }

            return typeDefinitionItem;
        }

        public static CsdlJsonSchemaTermItem BuildCsdlTermType(string name, JsonObjectValue termObject, IJsonPath jsonPath)
        {
            // Term Object
            // A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // The term object MUST contain the member $Kind with a string value of Term.
            // It MAY contain the members $Type, $Collection, $AppliesTo, $Nullable, $MaxLength, $Precision, $Scale, $SRID, and $DefaultValue, as well as $Unicode for 4.01 and greater payloads.
            // It MAY contain annotations.
            CsdlJsonSchemaTermItem termItem = new CsdlJsonSchemaTermItem();
            termItem.Name = name;
            termItem.QualifiedTypeName = "Edm.String"; // Absence of the $Type member means the type is Edm.String.

            foreach (var property in termObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        termItem.QualifiedTypeName = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        termItem.IsCollection = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        // TODO: should verify this value must be true?
                        break;

                    case "$AppliesTo":
                        // The value of $AppliesTo is an array whose items are strings containing symbolic values from the table above that identify model elements the term is intended to be applied to.
                        JsonArrayValue appliesToArray = propertyValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);
                        termItem.AppliesTo = appliesToArray.ParseArray<string>(jsonPath, (v, p) => v.ParseAsStringPrimitive(p));
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        termItem.Nulable = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        termItem.MaxLength = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        termItem.Precision = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        termItem.Scale = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        termItem.Unicode = propertyValue.ParseAsBooleanPrimitive(jsonPath);
                        break;

                    case "$SRID":
                        // The value of $SRID is a string containing a number or the symbolic value variable.
                        // So far, ODL doesn't support string of SRID.
                        termItem.Srid = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        // So far, ODL only suppports the string default value.
                        termItem.DefaultValue = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$BaseTerm":
                        // The value of $BaseTerm is the qualified name of the base term.
                        // So far, it's not supported, so skip it.
                        break;

                    default:
                        termItem.AddMember(propertyName, propertyValue);
                        break;
                }
            }

            return termItem;
        }

        /// <summary>
        /// A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="complexObject"></param>
        /// <returns></returns>
        public static CsdlJsonSchemaComplexItem BuildCsdlComplexType(string name, JsonObjectValue complexObject, IJsonPath jsonPath)
        {
            // The complex type object MUST contain the member $Kind with a string value of ComplexType.
            // It MAY contain the members $BaseType, $Abstract, and $OpenType.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.

            CsdlJsonSchemaComplexItem complexItem = new CsdlJsonSchemaComplexItem();
            complexItem.Name = name;

            foreach (var property in complexObject)
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
                        complexItem.BaseType = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    // $Abstract
                    case "$Abstract":
                        complexItem.IsAbstract = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        break;

                    // $OpenType
                    case "$OpenType":
                        complexItem.IsOpen = propertyValue.ParseAsBooleanPrimitive(jsonPath).Value;
                        break;

                    default:
                        complexItem.AddMember(propertyName, propertyValue);
                        break;
                }
            }

            return complexItem;
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
                        entityTypeItem.BaseType = propertyValue.ParseAsStringPrimitive(jsonPath);
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


        public static IList<CsdlJsonSchemaOperationItem> BuildCsdlOperationOverload(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // An action/function is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array.
            // The array contains one object per action/function overload.
            if (jsonValue.ValueKind != JsonValueKind.JArray)
            {
                return null;
            }

            JsonArrayValue operationArray = (JsonArrayValue)jsonValue;
            //   CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlJsonSchemaOperationItem> operations = new List<CsdlJsonSchemaOperationItem>();
            operationArray.ProcessItem(jsonPath, v =>
            {
                CsdlJsonSchemaOperationItem operation = BuildCsdlOperation(name, v, jsonPath);
                operations.Add(operation);
            });

            return operations;
        }

        public static CsdlJsonSchemaOperationItem BuildCsdlOperation(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // The action/funtion overload object MUST contain the member $Kind with a string value of Action/Function.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            JsonObjectValue operationObject = (JsonObjectValue)jsonValue;

            CsdlJsonSchemaOperationItem opertationItem = null;
            string kind = GetKind(operationObject, jsonPath);
            if (kind == "Action")
            {
                // It's action
                opertationItem = new CsdlJsonSchemaActionItem();
            }
            else
            {
                // It's function
                opertationItem = new CsdlJsonSchemaFunctionItem();
            }
            opertationItem.Name = name;

            foreach (var property in operationObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of Action/Function
                        break;

                    default:
                        opertationItem.AddMember(propertyName, propertyValue);
                        break;
                }
            }

            return opertationItem;
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
