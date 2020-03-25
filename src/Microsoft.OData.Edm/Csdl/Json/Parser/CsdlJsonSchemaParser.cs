//---------------------------------------------------------------------
// <copyright file="SchemaJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Json.Parser
{
    /// <summary>
    /// Provides functionalities for parsing Schema JSON for Csdl elements.
    /// </summary>
    internal class CsdlJsonSchemaParser
    {
        public CsdlJsonSchemaParser(CsdlSerializerOptions options)
        {

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
        public static CsdlSchema ParseCsdlSchema(string schemaNamespace, IJsonValue jsonValue, IJsonPath jsonPath, Version version, CsdlSerializerOptions options)
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            // Its value is an object.
            JsonObjectValue schemaObject = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            IList<CsdlAnnotations> outOfLineAnnotations = new List<CsdlAnnotations>();
            IList<CsdlElement> csdlElements = new List<CsdlElement>();
            string alias = null;
            schemaObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the schema.
                        alias = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Annotations":
                        // The value of $Annotations is an object with one member per annotation target.
                        outOfLineAnnotations = AnnotationJsonParser.ParseOutOfLineAnnotations(propertyValue, jsonPath);
                        break;

                    default:
                        CsdlElement element = ParseSchemaElement(propertyName, propertyValue, jsonPath, options);
                        if (element != null)
                        {
                            csdlElements.Add(element);
                        }

                        break;
                }
            });

            CsdlLocation location = new CsdlLocation(jsonPath.ToString());
            CsdlSchema schema = new CsdlSchema(schemaNamespace, alias, version,
                csdlElements.OfType<CsdlStructuredType>(),
                csdlElements.OfType<CsdlEnumType>(),
                csdlElements.OfType<CsdlOperation>(),
                csdlElements.OfType<CsdlTerm>(),
                csdlElements.OfType<CsdlEntityContainer>(),
                outOfLineAnnotations,
                csdlElements.OfType<CsdlTypeDefinition>(),
                location);

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
        public static CsdlElement ParseSchemaElement(string name, IJsonValue jsonValue, IJsonPath jsonPath, CsdlSerializerOptions options)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            // The schema object member is representing entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
            // The JSON value of each member is a JSON object, if it's not an object, it's not a schema element.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                jsonValue.ReportUnknownMember(jsonPath, options);
                return null;
            }

            JsonObjectValue schemaElementObject = (JsonObjectValue)jsonValue;

            // Each schema member oject should include "$Kind" member, whose value is a string
            string kind = GetKind(schemaElementObject, jsonPath);
            switch (kind)
            {
                case "EntityContainer":
                    return ParseCsdlEntityContainer(name, schemaElementObject, jsonPath, options);

                case "EntityType":
                    return ParseCsdlEntityType(name, schemaElementObject, jsonPath);

                case "ComplexType":
                    return BuildCsdlComplexType(name, schemaElementObject, jsonPath);

                case "EnumType":
                    return BuildCsdlEnumType(name, schemaElementObject, jsonPath);

                case "TypeDefinition":
                    return BuildCsdlTyeDefinition(name, schemaElementObject, jsonPath);

                case "Term":
                    return BuildCsdlTermType(name, schemaElementObject, jsonPath);

                default:
                    // If there's no "$Kind" or unknow kind, it's not a schema element
                    jsonValue.ReportUnknownMember(jsonPath, options);
                    return null;
            }
        }

        /// <summary>
        /// Build the schema member : EntityContainer
        /// An entity container is represented as a member of the schema object:
        /// whose name is the unqualified name of the entity container
        /// whose value is an object.
        /// </summary>
        /// <param name="name">The entity container unqualified name.</param>
        /// <param name="jsonValue">The entity contain value is an object.</param>
        /// <returns></returns>
        public static CsdlEntityContainer ParseCsdlEntityContainer(string name, IJsonValue jsonValue, IJsonPath jsonPath, CsdlSerializerOptions options)
        {
            // An entity container is represented as a member of the schema object whose name is the unqualified name of the entity container and whose value is an object.
            JsonObjectValue entityContainerObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            // The entity container object MUST contain the member $Kind with a string value of EntityContainer.
            string kind = GetKind(entityContainerObj, jsonPath);
            if (kind != "EntityContainer")
            {
                throw new CsdlParseException(Strings.SchemaJsonParser_MissingKindMember("entity container", "EntityContainer"));
            }

            IList<CsdlElement> entityContainerMembers = new List<CsdlElement>();
            string extends = null;
            entityContainerObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // Skip it, it's parsed above.
                        break;

                    case "$Extends":
                        // The entity container object MAY contain the member $Extends,
                        // The value of $Extends is the qualified name of the entity container to be extended.
                        extends = propertyValue.ParseAsString(jsonPath);
                        break;

                    default:
                        CsdlElement element = ParseEntityContainerMember(propertyName, propertyValue, jsonPath, options);
                        if (element != null)
                        {
                            entityContainerMembers.Add(element);
                        }
                        else
                        {
                            propertyValue.ReportUnknownMember(jsonPath, options);
                        }

                        break;
                }
            });

            CsdlLocation location = new CsdlLocation(jsonPath.ToString());
            CsdlEntityContainer entityContainer = new CsdlEntityContainer(name, extends,
                entityContainerMembers.OfType<CsdlEntitySet>(),
                entityContainerMembers.OfType<CsdlSingleton>(),
                entityContainerMembers.OfType<CsdlOperationImport>(),
                location);

            foreach (var annotation in entityContainerMembers.OfType<CsdlAnnotation>())
            {
                entityContainer.AddAnnotation(annotation);
            }

            return entityContainer;
        }

        public static CsdlElement ParseEntityContainerMember(string name, IJsonValue jsonValue, IJsonPath jsonPath, CsdlSerializerOptions options)
        {
            // It maybe entity container's annotation
            if (name[0] == '@')
            {
                string termName = name.Substring(1);
                return BuildCsdlAnnotation(termName, jsonValue, jsonPath);
            }

            CsdlOperationImport operationImport;
            if (TryParseOperationImport(name, jsonValue, jsonPath, out operationImport))
            {
                return operationImport;
            }

            CsdlAbstractNavigationSource navigationSource;
            if (TryParseNavigationSource(name, jsonValue, jsonPath, out navigationSource))
            {
                return navigationSource;
            }

            return null;
        }

        public static bool TryParseNavigationSource(string name, IJsonValue jsonValue, IJsonPath jsonPath, out CsdlAbstractNavigationSource navigationSource)
        {
            navigationSource = null;
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return false;
            }

            JsonObjectValue objValue = (JsonObjectValue)jsonValue;

            IList<CsdlNavigationPropertyBinding> navigationPropertyBindings = new List<CsdlNavigationPropertyBinding>();
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            string type = null;
            bool? isCollection = null;
            bool? includeInServiceDocument = null;
            bool? nullable = null;
            foreach (var property in objValue)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Type":
                        // The value of $Type is the qualified name of an entity type.
                        type = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Collection":
                        // The value of $Collection is the Booelan value true.
                        isCollection = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$IncludeInServiceDocument":
                        // The value of $IncludeInServiceDocument is one of the Boolean literals true or false. Absence of the member means true.
                        includeInServiceDocument = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        // In OData 4.0 responses this member MUST NOT be specified.
                        nullable = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$NavigationPropertyBinding":
                        // The value of $NavigationPropertyBinding is an object.
                        navigationPropertyBindings = BuildCsdlNavigationPropertyBinding(propertyValue, jsonPath);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            string termName = propertyName.Substring(1);
                            annotations.Add(BuildCsdlAnnotation(termName, propertyValue, jsonPath));
                        }
                        break;
                }
            }

            if (type == null)
            {
                return false;
            }

            CsdlLocation location = new CsdlLocation(jsonPath.ToString());

            if (isCollection != null)
            {
                // entitySet
                if (!isCollection.Value)
                {
                    // If presented, $IsCollection should be true
                    throw new Exception();
                }

                if (includeInServiceDocument == null || includeInServiceDocument.Value)
                {
                    navigationSource = new CsdlEntitySet(name, type, navigationPropertyBindings, location);
                }
                else
                {
                    navigationSource = new CsdlEntitySet(name, type, navigationPropertyBindings, location, false);
                }
            }
            else
            {
                // singleton
                navigationSource = new CsdlSingleton(name, type, navigationPropertyBindings, location);
            }

            foreach (var annotation in annotations)
            {
                navigationSource.AddAnnotation(annotation);
            }

            return navigationSource != null;
        }

        public static IList<CsdlNavigationPropertyBinding> BuildCsdlNavigationPropertyBinding(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            CsdlLocation location = new CsdlLocation(-1, -1);
            JsonObjectValue objValue = (JsonObjectValue)jsonValue;

            IList<CsdlNavigationPropertyBinding> bindings = new List<CsdlNavigationPropertyBinding>();
            foreach (var property in objValue)
            {
                string bindingPath = property.Key;
                string target = property.Value.ParseAsString(jsonPath);

                bindings.Add(new CsdlNavigationPropertyBinding(bindingPath, target, location));
            }

            return bindings;
        }

        public static bool TryParseOperationImport(string name, IJsonValue jsonValue, IJsonPath jsonPath, out CsdlOperationImport operationImport)
        {
            operationImport = null;
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return false;
            }

            JsonObjectValue objValue = (JsonObjectValue)jsonValue;

            IJsonValue importValue;
            if (objValue.TryGetValue("$Action", out importValue))
            {
                // ActionImport
                operationImport = BuildCsdlActionImport(name, objValue, jsonPath);
                return true;
            }

            if (objValue.TryGetValue("$Function", out importValue))
            {
                // FunctionImport
                operationImport = BuildCsdlFunctionImport(name, objValue, jsonPath);
                return true;
            }

            return false;
        }

        public static CsdlActionImport BuildCsdlActionImport(string name, JsonObjectValue importObject, IJsonPath jsonPath)
        {
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            string action = null;
            string entitySet = null;
            foreach (var property in importObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Action":
                        // The value of $Action is a string containing the qualified name of an unbound action.
                        action = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$EntitySet":
                        // The value of $EntitySet is a string containing either the unqualified name of an entity set in the same entity container
                        // or a path to an entity set in a different entity container.
                        entitySet = propertyValue.ParseAsString(jsonPath);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            string termName = propertyName.Substring(1);
                            annotations.Add(BuildCsdlAnnotation(termName, propertyValue, jsonPath));
                        }

                        break;
                }
            }

            CsdlLocation location = new CsdlLocation(-1, -1);
            CsdlActionImport actionImport = new CsdlActionImport(name, action, entitySet, location);
            foreach (var annotation in annotations)
            {
                actionImport.AddAnnotation(annotation);
            }

            return actionImport;
        }

        public static CsdlFunctionImport BuildCsdlFunctionImport(string name, JsonObjectValue importObject, IJsonPath jsonPath)
        {
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            string function = null;
            string entitySet = null;
            bool? includeInServiceDocument = false; // Absence of the member means false.
            foreach (var property in importObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Function":
                        // The value of $Function is a string containing the qualified name of an unbound function.
                        function = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$EntitySet":
                        // The value of $EntitySet is a string containing either the unqualified name of an entity set in the same entity container
                        // or a path to an entity set in a different entity container.
                        entitySet = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$IncludeInServiceDocument":
                        // The value of $IncludeInServiceDocument is one of the Boolean literals true or false. Absence of the member means false.
                        includeInServiceDocument = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            string termName = propertyName.Substring(1);
                            annotations.Add(BuildCsdlAnnotation(termName, propertyValue, jsonPath));
                        }

                        break;
                }
            }

            CsdlLocation location = new CsdlLocation(-1, -1);
            CsdlFunctionImport functionImport = new CsdlFunctionImport(name, function, entitySet, includeInServiceDocument.Value, location);
            foreach (var annotation in annotations)
            {
                functionImport.AddAnnotation(annotation);
            }

            return functionImport;
        }

        /// <summary>
        /// An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public static CsdlEntityType ParseCsdlEntityType(string name, JsonObjectValue entityObject, IJsonPath jsonPath)
        {
            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            bool? isOpen = null;
            bool? isAbstract = null;
            string baseType = null;
            IList<CsdlProperty> properties = new List<CsdlProperty>();
            IList<CsdlNavigationProperty> navProperties = new List<CsdlNavigationProperty>();
            IList<CsdlAnnotation> complexAnnotations = new List<CsdlAnnotation>();
            IDictionary<string, IList<CsdlAnnotation>> propertyAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();
            string kind = null;
            bool? hasStream = null;
            CsdlKey csdlKey = null;
            foreach (var property in entityObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    // $Kind
                    case "$Kind":
                        kind = propertyValue.ParseAsString(jsonPath);
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    // $BaseType
                    case "$BaseType":
                        baseType = propertyValue.ParseAsString(jsonPath);
                        break;

                    // $Abstract
                    case "$Abstract":
                        isAbstract = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    // $OpenType
                    case "$OpenType":
                        isOpen = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    // $HasStream
                    case "$HasStream":
                        hasStream = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$Key":
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName == null)
                        {
                            // so it's complex property
                            // Property is an object
                            JsonObjectValue objValue = (JsonObjectValue)propertyValue;
                            IJsonValue kindValue;
                            if (objValue.TryGetValue(CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Kind, out kindValue))
                            {
                                string kindString = kindValue.ParseAsString(jsonPath);
                                if (kindString == CsdlConstants.Element_NavigationProperty)
                                {
                                    CsdlNavigationProperty navProperty = BuildCsdlNavigationProperty(propertyName, objValue, jsonPath);
                                    navProperties.Add(navProperty);
                                    break;
                                }
                            }

                            // The property object MAY contain the member $Kind with a string value of Property.
                            // This member SHOULD be omitted to reduce document size.
                            CsdlProperty structuralProperty = BuildCsdlProperty(propertyName, objValue, jsonPath);
                            properties.Add(structuralProperty);
                        }
                        else
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);

                            if (propertyName == null)
                            {
                                // annotation for the complex type
                                complexAnnotations.Add(annotation);
                            }
                            else
                            {
                                // annotation for complex property
                                IList<CsdlAnnotation> values;
                                if (!propertyAnnotations.TryGetValue(propertyName, out values))
                                {
                                    values = new List<CsdlAnnotation>();
                                    propertyAnnotations[propertyName] = values;
                                }
                                values.Add(annotation);
                            }
                        }

                        break;
                }
            }

            if (kind != CsdlConstants.Element_EntityType)
            {
                throw new Exception();
            }

            foreach (var item in propertyAnnotations)
            {
                // TODO: maybe refactor later for performance
                CsdlProperty structuralProperty = properties.FirstOrDefault(c => c.Name == item.Key);
                if (structuralProperty != null)
                {
                    foreach (var annotation in item.Value)
                    {
                        structuralProperty.AddAnnotation(annotation);
                    }

                    continue;
                }

                CsdlNavigationProperty navProperty = navProperties.FirstOrDefault(c => c.Name == item.Key);
                if (navProperty != null)
                {
                    foreach (var annotation in item.Value)
                    {
                        navProperty.AddAnnotation(annotation);
                    }

                    continue;
                }

                throw new Exception();
            }

            CsdlEntityType entityType = new CsdlEntityType(name,
                baseType,
                isAbstract == null ? false : isAbstract.Value,
                isOpen == null ? false : isOpen.Value,
                hasStream == null ? false : hasStream.Value,
                csdlKey,
                properties,
                navProperties,
                new CsdlLocation(jsonPath.Path));

            foreach (var annotation in complexAnnotations)
            {
                entityType.AddAnnotation(annotation);
            }

            return entityType;
        }

        /// <summary>
        /// A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="complexObject"></param>
        /// <returns></returns>
        public static CsdlComplexType BuildCsdlComplexType(string name, JsonObjectValue complexObject, IJsonPath jsonPath)
        {
            // The complex type object MUST contain the member $Kind with a string value of ComplexType.
            // It MAY contain the members $BaseType, $Abstract, and $OpenType.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            bool? isOpen = null;
            bool? isAbstract = null;
            string baseType = null;
            IList<CsdlProperty> properties = new List<CsdlProperty>();
            IList<CsdlNavigationProperty> navProperties = new List<CsdlNavigationProperty>();
            IList<CsdlAnnotation> complexAnnotations = new List<CsdlAnnotation>();
            IDictionary<string, IList<CsdlAnnotation>> propertyAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();
            foreach (var property in complexObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        string kind = propertyValue.ParseAsString(jsonPath);
                        if (kind != CsdlConstants.Element_ComplexType)
                        {
                            throw new Exception();
                        }
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    case "$BaseType":
                        baseType = propertyValue.ParseAsString(jsonPath);
                        break;

                    // $Abstract
                    case "$Abstract":
                        isAbstract = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    // $OpenType
                    case "$OpenType":
                        isOpen = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName == null)
                        {
                            // so it's complex property
                            // Property is an object
                            JsonObjectValue objValue = (JsonObjectValue)propertyValue;
                            IJsonValue kindValue;
                            if (objValue.TryGetValue(CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Kind, out kindValue))
                            {
                                string kindString = kindValue.ParseAsString(jsonPath);
                                if (kindString == CsdlConstants.Element_NavigationProperty)
                                {
                                    CsdlNavigationProperty navProperty = BuildCsdlNavigationProperty(propertyName, objValue, jsonPath);
                                    navProperties.Add(navProperty);
                                    break;
                                }
                            }

                            // The property object MAY contain the member $Kind with a string value of Property.
                            // This member SHOULD be omitted to reduce document size.
                            CsdlProperty structuralProperty = BuildCsdlProperty(propertyName, objValue, jsonPath);
                            properties.Add(structuralProperty);
                        }
                        else
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);

                            if (propertyName == null)
                            {
                                // annotation for the complex type
                                complexAnnotations.Add(annotation);
                            }
                            else
                            {
                                // annotation for complex property
                                IList<CsdlAnnotation> values;
                                if (!propertyAnnotations.TryGetValue(propertyName, out values))
                                {
                                    values = new List<CsdlAnnotation>();
                                    propertyAnnotations[propertyName] = values;
                                }
                                values.Add(annotation);
                            }
                        }

                        break;
                }
            }

            foreach (var item in propertyAnnotations)
            {
                // TODO: maybe refactor later for performance
                CsdlProperty structuralProperty = properties.FirstOrDefault(c => c.Name == item.Key);
                if (structuralProperty != null)
                {
                    foreach (var annotation in item.Value)
                    {
                        structuralProperty.AddAnnotation(annotation);
                    }

                    continue;
                }

                CsdlNavigationProperty navProperty = navProperties.FirstOrDefault(c => c.Name == item.Key);
                if (navProperty != null)
                {
                    foreach (var annotation in item.Value)
                    {
                        navProperty.AddAnnotation(annotation);
                    }

                    continue;
                }

                throw new Exception();
            }

            CsdlLocation location = new CsdlLocation(-1, -1);

            // CsdlComplexType(string name, string baseTypeName, bool isAbstract, bool isOpen, IEnumerable<CsdlProperty> structuralProperties, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlLocation location)
            CsdlComplexType complexType = new CsdlComplexType(name, baseType,
                isAbstract == null ? false : isAbstract.Value,
                isOpen == null ? false : isOpen.Value,
                properties, navProperties, location);
            foreach (var annotation in complexAnnotations)
            {
                complexType.AddAnnotation(annotation);
            }

            return complexType;
        }

        public static CsdlProperty BuildCsdlProperty(string name, JsonObjectValue propertyObject, IJsonPath jsonPath)
        {
            // Structural properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
            // It MAY contain the member $Type, $Collection, $Nullable, $MaxLength, $Unicode, $Precision, $Scale, $SRID, and $DefaultValue.
            string typeName = "Edm.String"; // Absence of the $Type member means the type is Edm.String.
            bool? isCollection = null;
            bool? nullable = null; //  Absence of the member means false.
            int? maxLength = null;
            int? precision = null;
            int? scale = null;
            bool? unicode = null; // Absence of the member means true.
            int? srid = null;
            string defaultValue = null;
            string kind;
            IList<CsdlAnnotation> propertyAnnotations = new List<CsdlAnnotation>();
            foreach (var property in propertyObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
                        kind = propertyValue.ParseAsString(jsonPath);
                        if (kind != "Property")
                        {
                            throw new Exception();
                        }
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        isCollection = propertyValue.ParseAsBoolean(jsonPath);
                        // TODO: should verify this value must be true?
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        maxLength = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        precision = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        scale = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        unicode = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$SRID":
                        // The value of $SRID is a string containing a number or the symbolic value variable.
                        // So far, ODL doesn't support string of SRID.
                        srid = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        // So far, ODL only suppports the string default value.
                        defaultValue = propertyValue.ReadAsJsonString();
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null && propertyName == null)
                        {
                            // annotation for the property
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);
                            propertyAnnotations.Add(annotation);
                            break;
                        }

                        // Without '@' in the name, it's not allowed in Navigation Property object
                        throw new Exception();
                }
            }

            CsdlLocation location = new CsdlLocation(-1, -1);
            CsdlTypeReference typeReference = ParseTypeReference(typeName,
                isCollection == null ? false : isCollection.Value,
                false,
                nullable == null ? false : nullable.Value,
                maxLength, unicode, precision, scale, srid, location);

            CsdlProperty csdlProperty = new CsdlProperty(name, typeReference, defaultValue, location);
            foreach (var item in propertyAnnotations)
            {
                csdlProperty.AddAnnotation(item);
            }
            return csdlProperty;
        }

        public static CsdlNavigationProperty BuildCsdlNavigationProperty(string name, JsonObjectValue navigtionObject, IJsonPath jsonPath)
        {
            // Navigation properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            // The navigation property object MUST contain the member $Kind with a string value of NavigationProperty.
            // It MUST contain the member $Type, and it MAY contain the members $Collection, $Nullable, $Partner, $ContainsTarget, $ReferentialConstraint, and $OnDelete.
            string kind = null;
            string typeName = null;
            bool? isCollection = null;
            bool? nullable = null;
            string parter = null;
            bool? containsTarget = null; // Absence of the member means false.
            IList<CsdlAnnotation> onDeleteAnnotations = new List<CsdlAnnotation>();
            CsdlOnDelete csdlOnDelete = null;

            CsdlLocation location = new CsdlLocation(-1, -1);

            IList<CsdlAnnotation> navPropertyAnnotations = new List<CsdlAnnotation>();
            IList<CsdlReferentialConstraint> referentialConstraints = null;
            foreach (var property in navigtionObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of NavigationProperty
                        kind = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        isCollection = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$Partner":
                        // The value of $Partner is a string containing the path to the partner navigation property.
                        parter = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$ContainsTarget":
                        // The value of $ContainsTarget is one of the Boolean literals true or false. Absence of the member means false.
                        containsTarget = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$OnDelete":
                        // The value of $OnDelete is a string with one of the values Cascade, None, SetNull, or SetDefault.
                        csdlOnDelete = BuildCsdlOnDelete(propertyValue, jsonPath);
                        break;

                    case "$ReferentialConstraint":
                        referentialConstraints = BuildCsdlReferentialConstraint(propertyValue, jsonPath);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null)
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);
                            if (propertyName == null)
                            {
                                // annotation for the navigation property
                                navPropertyAnnotations.Add(annotation);
                                break;
                            }
                            else if (propertyName == "$OnDelete")
                            {
                                // annotation for OnDelte
                                onDeleteAnnotations.Add(annotation);
                                break;
                            }
                        }

                        // Without '@' in the name, it's not allowed in Navigation Property object
                        throw new Exception();
                }
            }

            if (kind == null || kind != "NavigationProperty")
            {
                throw new Exception();
            }

            if (isCollection != null && isCollection.Value)
            {
                typeName = "Collection(" + typeName + ")";
            }


            if (csdlOnDelete != null && onDeleteAnnotations.Count > 0)
            {
                foreach (var item in onDeleteAnnotations)
                {
                    csdlOnDelete.AddAnnotation(item);
                }
            }

            CsdlNavigationProperty csdlNavProperty = new CsdlNavigationProperty(name, typeName, nullable, parter,
                containsTarget == null ? false : containsTarget.Value,
                csdlOnDelete, referentialConstraints, location);
            foreach (var item in navPropertyAnnotations)
            {
                csdlNavProperty.AddAnnotation(item);
            }
            return csdlNavProperty;
        }

        public static IList<CsdlReferentialConstraint> BuildCsdlReferentialConstraint(IJsonValue value, IJsonPath jsonPath)
        {
            if (value.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            JsonObjectValue referentialConstraintObject = (JsonObjectValue)value;
            // The value of $ReferentialConstraint is an object with one member per referential constraint.
            // The member name is the path to the dependent property, this path is relative to the structured type declaring the navigation property.
            // The member value is a string containing the path to the principal property, this path is relative to the entity type that is the target of the navigation property.
            // It also MAY contain annotations.These are prefixed with the path of the dependent property of the annotated referential constraint.
            IList<CsdlReferentialConstraint> referentialConstraints = new List<CsdlReferentialConstraint>();
            IDictionary<string, IList<CsdlAnnotation>> itemsAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();
            CsdlLocation location = new CsdlLocation(-1, -1);
            foreach (var property in referentialConstraintObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                string termName;
                propertyName = SeperateAnnotationName(propertyName, out termName);
                if (termName != null)
                {
                    // "CategoryKind@Core.Description": "Referential Constraint to non-key property"
                    CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);

                    IList<CsdlAnnotation> annotations;
                    if (!itemsAnnotations.TryGetValue(propertyName, out annotations))
                    {
                        annotations = new List<CsdlAnnotation>();
                        itemsAnnotations[propertyName] = annotations;
                    }
                    annotations.Add(annotation);
                }
                else
                {
                    // "CategoryKind": "Kind"
                    string referedProperty = propertyValue.ParseAsString(jsonPath);

                    // PropertyName is the dependent property
                    // PropertyValue is the principal property.
                    referentialConstraints.Add(new CsdlReferentialConstraint(referedProperty, propertyName, location));
                }
            }

            foreach (var item in referentialConstraints)
            {
                IList<CsdlAnnotation> annotations;
                if (itemsAnnotations.TryGetValue(item.PropertyName, out annotations))
                {
                    foreach (var ann in annotations)
                    {
                        item.AddAnnotation(ann);
                    }
                }
            }

            return referentialConstraints;
        }

        public static CsdlOnDelete BuildCsdlOnDelete(IJsonValue value, IJsonPath jsonPath)
        {
            string onDelete = value.ParseAsString(jsonPath);

            EdmOnDeleteAction edmOnDelete = EdmOnDeleteAction.None;
            switch (onDelete)
            {
                case "Cascade":
                    edmOnDelete = EdmOnDeleteAction.Cascade;
                    break;
                default:
                    // So far, ODL only supports "Cascade", for others "SetNull, or SetDefault.", we use "None" for them
                    break;
            }

            return new CsdlOnDelete(edmOnDelete, new CsdlLocation(jsonPath.Path));
        }

        public static CsdlTypeDefinition BuildCsdlTyeDefinition(string name, JsonObjectValue typeDefinitionObject, IJsonPath jsonPath)
        {
            // A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition and whose value is an object.
            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition and the member $UnderlyingType.
            // It MAY contain the members $MaxLength, $Unicode, $Precision, $Scale, and $SRID, and it MAY contain annotations.
            string kind = null;
            int? maxLength = null;
            int? precision = null;
            int? scale = null;
            bool? unicode = null; // Absence of the member means true.
            string underlygingType = null;
            IList<CsdlAnnotation> typeDefinitionAnnotations = new List<CsdlAnnotation>();
            CsdlLocation location = new CsdlLocation(-1, -1);

            foreach (var property in typeDefinitionObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of NavigationProperty
                        kind = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$UnderlyingType":
                        // The value of $UnderlyingType is the qualified name of the underlying type.
                        underlygingType = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        maxLength = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        precision = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        scale = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        unicode = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null && propertyName == null)
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);
                            // annotation for the navigation property
                            typeDefinitionAnnotations.Add(annotation);
                            break;
                        }

                        // Without '@' in the name, it's not allowed in Navigation Property object
                        throw new Exception();
                }
            }

            if (kind == null || kind != "TypeDefinition")
            {
                throw new Exception();
            }

            CsdlTypeDefinition typeDefinition = new CsdlTypeDefinition(name, underlygingType, location);
            foreach (var item in typeDefinitionAnnotations)
            {
                typeDefinition.AddAnnotation(item);
            }

            return typeDefinition;
        }

        public static CsdlOperation BuildCsdlOperation(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // The action/funtion overload object MUST contain the member $Kind with a string value of Action/Function.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            JsonObjectValue operationObject = (JsonObjectValue)jsonValue;

            bool? isBound = null; // absence of the member means false
            bool? isComposable = null; // Absence of the member means false.
            string kind = null;
            string entitySetPath = null;
            IList<CsdlAnnotation> operationAnnotations = new List<CsdlAnnotation>();
            IList<CsdlOperationParameter> parameters = null;
            CsdlOperationReturn operationReturn = null;
            foreach (var property in operationObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of Action/Function
                        kind = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$IsBound":
                        // The value of $IsBound is one of the Boolean literals true or false. Absence of the member means false.
                        isBound = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$EntitySetPath":
                        // The value of $EntitySetPath is a string containing the entity set path.
                        entitySetPath = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Parameter":
                        // The value of $Parameter is an array. The array contains one object per parameter.
                        parameters = BuildCsdlOperationParameters(propertyValue);
                        break;

                    case "$ReturnType":
                        // The value of $ReturnType is an object.
                        operationReturn = BuildCsdlOperationReturn(propertyValue);
                        break;

                    case "$IsComposable":
                        // The value of $IsComposable is one of the Boolean literals true or false. Absence of the member means false.
                        isComposable = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null && propertyName == null)
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);
                            // annotation for the operation
                            operationAnnotations.Add(annotation);
                            break;
                        }

                        throw new Exception();
                }
            }

            if (kind == null)
            {
                throw new Exception();
            }

            CsdlLocation location = new CsdlLocation(jsonPath.Path);
            CsdlOperation operation;
            if (kind == "Action")
            {
                // It's action
                operation = new CsdlAction(name, parameters, operationReturn, isBound == null ? false : isBound.Value, entitySetPath, location);
            }
            else if (kind == "Function")
            {
                // It's function
                operation = new CsdlFunction(name, parameters, operationReturn, isBound == null ? false : isBound.Value, entitySetPath,
                    isComposable == null ? false : isComposable.Value, location);
            }
            else
            {
                throw new Exception();
            }

            foreach (var annotation in operationAnnotations)
            {
                operation.AddAnnotation(annotation);
            }

            return operation;
        }

        public static IList<CsdlOperationParameter> BuildCsdlOperationParameters(IJsonValue jsonValue)
        {
            return null;
        }

        public static CsdlOperationReturn BuildCsdlOperationReturn(IJsonValue jsonValue)
        {
            return null;
        }

        public static IList<CsdlOperation> BuildCsdlOperationOverload(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // An action/function is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array.
            // The array contains one object per action/function overload.
            if (jsonValue.ValueKind != JsonValueKind.JArray)
            {
                return null;
            }

            JsonArrayValue operationArray = (JsonArrayValue)jsonValue;
            //   CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlOperation> operations = new List<CsdlOperation>();
            foreach (var item in operationArray)
            {
                CsdlOperation operation = BuildCsdlOperation(name, item, jsonPath);
                operations.Add(operation);
            }

            return operations;
        }

        public static CsdlTerm BuildCsdlTermType(string name, JsonObjectValue termObject, IJsonPath jsonPath)
        {
            // Term Object
            // A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // The term object MUST contain the member $Kind with a string value of Term.
            // It MAY contain the members $Type, $Collection, $AppliesTo, $Nullable, $MaxLength, $Precision, $Scale, $SRID, and $DefaultValue, as well as $Unicode for 4.01 and greater payloads.
            // It MAY contain annotations.
            CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlAnnotation> termAnnotations = new List<CsdlAnnotation>();

            string typeName = "Edm.String"; // Absence of the $Type member means the type is Edm.String.
            bool? isCollection = null;
            bool? nullable = null; //  Absence of the member means false.
            int? maxLength = null;
            int? precision = null;
            int? scale = null;
            bool? unicode = null; // Absence of the member means true.
            int? srid = null;
            string defaultValue = null;
            string kind;
            IList<string> appliesTo = null;
            termObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
                        kind = propertyValue.ParseAsString(jsonPath);
                        if (kind != "Term")
                        {
                            throw new Exception();
                        }
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        isCollection = propertyValue.ParseAsBoolean(jsonPath);
                        // TODO: should verify this value must be true?
                        break;

                    case "$AppliesTo":
                        // The value of $AppliesTo is an array whose items are strings containing symbolic values from the table above that identify model elements the term is intended to be applied to.
                        JsonArrayValue appliesToArray = propertyValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);
                        appliesTo = appliesToArray.ParseArray<string>(jsonPath, (v, p) => v.ParseAsString(p));
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        maxLength = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        precision = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        scale = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        unicode = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$SRID":
                        // The value of $SRID is a string containing a number or the symbolic value variable.
                        // So far, ODL doesn't support string of SRID.
                        srid = propertyValue.ParseAsInteger(jsonPath);
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        // So far, ODL only suppports the string default value.
                        defaultValue = propertyValue.ReadAsJsonString();
                        break;

                    case "$BaseTerm":
                        // The value of $BaseTerm is the qualified name of the base term.
                        // So far, it's not supported, so skip it.
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null && propertyName == null)
                        {
                            // annotation for the property
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);
                            termAnnotations.Add(annotation);
                            break;
                        }

                        // Without '@' in the name, it's not allowed in Navigation Property object
                        throw new Exception();
                }
            });

            CsdlTypeReference typeReference = ParseTypeReference(typeName,
                isCollection == null ? false : isCollection.Value, false,
                nullable == null ? false : nullable.Value, maxLength, unicode, precision, scale, srid, location);

            string appliesToStr = appliesTo != null ? string.Join(" ", appliesTo) : null;
            CsdlTerm termType = new CsdlTerm(name, typeReference, appliesToStr, defaultValue, location);
            foreach (var annotation in termAnnotations)
            {
                termType.AddAnnotation(annotation);
            }

            return termType;
        }

        public static CsdlEnumType BuildCsdlEnumType(string name, JsonObjectValue enumObject, IJsonPath jsonPath)
        {
            // Enumeration Type Object
            // An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type and whose value is an object.
            // The enumeration type object MUST contain the member $Kind with a string value of EnumType.
            // It MAY contain the members $UnderlyingType and $IsFlags.
            // The enumeration type object MUST contain members representing the enumeration type members.
            // The enumeration type object MAY contain annotations.
            CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlEnumMember> members = new List<CsdlEnumMember>();
            IList<CsdlAnnotation> enumAnnotations = new List<CsdlAnnotation>();
            IDictionary<string, IList<CsdlAnnotation>> memberAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();
            bool? isFlags = null;
            string underlyingTypeName = null;

            foreach (var property in enumObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        string kind = propertyValue.ParseAsString(jsonPath);
                        if (kind != CsdlConstants.Element_EnumType)
                        {
                            throw new Exception();
                        }
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    case "$UnderlyingType":
                        underlyingTypeName = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$IsFlags":
                        isFlags = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName == null)
                        {
                            // Without '@' in the name, so it's normal enum member name
                            CsdlEnumMember enumMember = BuildCsdlEnumMember(propertyName, propertyValue, jsonPath);
                            members.Add(enumMember);
                        }
                        else
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue, jsonPath);

                            if (propertyName == null)
                            {
                                // annotation for the enum type
                                enumAnnotations.Add(annotation);
                            }
                            else
                            {
                                // annotation for enum member
                                IList<CsdlAnnotation> values;
                                if (!memberAnnotations.TryGetValue(propertyName, out values))
                                {
                                    values = new List<CsdlAnnotation>();
                                    memberAnnotations[propertyName] = values;
                                }
                                values.Add(annotation);
                            }
                        }

                        break;
                }
            }

            foreach (var item in memberAnnotations)
            {
                // TODO: maybe refactor later for performance
                CsdlEnumMember member = members.FirstOrDefault(c => c.Name == item.Key);
                if (member == null)
                {
                    throw new Exception();
                }

                foreach (var annotation in item.Value)
                {
                    member.AddAnnotation(annotation);
                }
            }

            CsdlEnumType enumType = new CsdlEnumType(name, underlyingTypeName, isFlags == null ? false: isFlags.Value, members, location);
            foreach (var annotation in enumAnnotations)
            {
                enumType.AddAnnotation(annotation);
            }

            return enumType;
        }

        public static CsdlAnnotation BuildCsdlAnnotation(string termName, IJsonValue annotationValue, IJsonPath jsonPath)
        {
            // Enumeration Member Object
            // Enumeration type members are represented as JSON object members, where the object member name is the enumeration member name and the object member value is the enumeration member value.
            // For members of flags enumeration types a combined enumeration member value is equivalent to the bitwise OR of the discrete values.
            // Annotations for enumeration members are prefixed with the enumeration member name.
            string qualifier = null;
            int index = termName.IndexOf('#');
            if (index != -1)
            {
                qualifier = termName.Substring(index + 1);
                termName = termName.Substring(0, index);
            }
            CsdlLocation location = new CsdlLocation(-1, -1);
            CsdlExpressionBase expression = AnnotationJsonParser.BuildExpression(annotationValue, jsonPath);

            return new CsdlAnnotation(termName, qualifier, expression, location);
        }

        public static CsdlEnumMember BuildCsdlEnumMember(string name, IJsonValue enumMemberObject, IJsonPath jsonPath)
        {
            // Enumeration Member Object
            // Enumeration type members are represented as JSON object members, where the object member name is the enumeration member name and the object member value is the enumeration member value.
            // For members of flags enumeration types a combined enumeration member value is equivalent to the bitwise OR of the discrete values.
            // Annotations for enumeration members are prefixed with the enumeration member name.

            long? value = enumMemberObject.ParseAsInteger(jsonPath);
            return new CsdlEnumMember(name, value, new CsdlLocation(jsonPath.Path));
        }

        private static string SeperateAnnotationName(string name, out string termName)
        {
            termName = null;
            int index = name.IndexOf('@');

            if (index == -1)
            {
                return name;
            }
            else if (index == 0)
            {
                termName = name.Substring(1);
                return null;
            }
            else
            {
                termName = name.Substring(index + 1);
                return name.Substring(0, index);
            }
        }

        public static CsdlTypeReference ParseTypeReference(string typeString, // if it's collection, it's element type string
            bool isCollection,
             bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid,
            CsdlLocation parentLocation)
        {
            CsdlTypeReference elementType = ParseNamedTypeReference(typeString, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid, parentLocation);
            if (isCollection)
            {
                elementType = new CsdlExpressionTypeReference(new CsdlCollectionType(elementType, parentLocation), isNullable, parentLocation);
            }

            return elementType;
        }

        private static CsdlNamedTypeReference ParseNamedTypeReference(string typeName, bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid,
             CsdlLocation parentLocation)
        {
            EdmPrimitiveTypeKind kind = EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName);
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                    return new CsdlPrimitiveTypeReference(kind, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Binary:
                    return new CsdlBinaryTypeReference(isUnbounded, maxLength, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new CsdlTemporalTypeReference(kind, precision, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Decimal:
                    return new CsdlDecimalTypeReference(precision, scale, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.String:
                    return new CsdlStringTypeReference(isUnbounded, maxLength, unicode, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.None:
                    if (string.Equals(typeName, CsdlConstants.TypeName_Untyped, StringComparison.Ordinal))
                    {
                        return new CsdlUntypedTypeReference(typeName, parentLocation);
                    }

                    break;
            }

            return new CsdlNamedTypeReference(isUnbounded, maxLength, unicode, precision, scale, srid, typeName, isNullable, parentLocation);
        }

        private static string GetKind(JsonObjectValue objValue, IJsonPath jsonPath)
        {
            string kind = null;
            IJsonValue kindValue;
            if (objValue.TryGetValue("$Kind", out kindValue))
            {
                jsonPath.Push("$Kind");
                kind = kindValue.ParseAsString(jsonPath);
                jsonPath.Pop();
            }

            return kind;
        }
    }
}
