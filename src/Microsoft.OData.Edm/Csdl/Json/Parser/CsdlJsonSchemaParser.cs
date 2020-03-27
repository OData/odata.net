//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Json.Parser
{
    /// <summary>
    /// Provides functionalities for parsing Schema JSON to Csdl elements.
    /// </summary>
    internal class CsdlJsonSchemaParser
    {
        private CsdlSerializerOptions _options;
        private Version _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsdlJsonSchemaParser"/> class.
        /// </summary>
        /// <param name="version">The Edm version of this parser working.</param>
        /// <param name="options">The parsing options.</param>
        public CsdlJsonSchemaParser(Version version, CsdlSerializerOptions options)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            _version = version;
            _options = options;
        }

        /// <summary>
        /// Parse the input <see cref="IJsonValue"/> to the <see cref="CsdlSchema"/>.
        /// </summary>
        /// <param name="name">The namespace of the schema.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The input json value path.</param>
        /// <returns>The parsed <see cref="CsdlSchema"/>.</returns>
        public CsdlSchema ParseCsdlSchema(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            // Its value is an object.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue schemaObject = (JsonObjectValue)jsonValue;

            IList<CsdlAnnotations> outOfLineAnnotations = new List<CsdlAnnotations>();
            List<CsdlElement> csdlElements = new List<CsdlElement>();
            List<CsdlOperation> csdlOpertions = new List<CsdlOperation>();
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
                        outOfLineAnnotations = ParseOutOfLineAnnotations(propertyValue, jsonPath);
                        break;

                    default:
                        // The schema object MAY contain members representing entity types,
                        // complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
                        CsdlElement element = TryParseSchemaElement(propertyName, propertyValue, jsonPath);
                        if (element != null)
                        {
                            csdlElements.Add(element);
                        }
                        else
                        {
                            IList<CsdlOperation> operations = TryParseCsdlOperationOverload(propertyName, propertyValue, jsonPath);
                            if (operations != null)
                            {
                                csdlOpertions.AddRange(operations);
                            }
                            else
                            {
                                // The schema object MAY also contain annotations that apply to the schema itself.
                                // ODL doesn't support the Schema itself annotations. So also reporting them as unexpected member.
                                ReportUnexpectedMember(propertyValue, jsonPath);
                            }
                        }

                        break;
                }
            });

            CsdlSchema schema = new CsdlSchema(name, alias, _version,
                csdlElements.OfType<CsdlStructuredType>(),
                csdlElements.OfType<CsdlEnumType>(),
                csdlOpertions,
                csdlElements.OfType<CsdlTerm>(),
                csdlElements.OfType<CsdlEntityContainer>(),
                outOfLineAnnotations,
                csdlElements.OfType<CsdlTypeDefinition>(),
                new CsdlLocation(jsonPath.Path));

            return schema;
        }

        #region OutOfLine Annotion
        // out of line annotations
        /*
         * "org.example": {
  "$Alias": "self",
  "$Annotations": {
    "self.Person": {
      "@Core.Description#Tablet": "Dummy",
      …
    }
  }
}
         * */
        public IList<CsdlAnnotations> ParseOutOfLineAnnotations(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            JsonObjectValue annotationsObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            IList<CsdlAnnotations> annotationsCollection = new List<CsdlAnnotations>();
            annotationsObj.ProcessProperty(jsonPath, (n, v) =>
            {
                // The member name is a path identifying the annotation target
                string target = n;

                string qualifier = null; // It's form the "target" name, or it's not set again in JSON?

                // the member value is an object containing annotations for that target.
                if (v.ValueKind == JsonValueKind.JObject)
                {
                    IList<CsdlAnnotation> subAnnotations = new List<CsdlAnnotation>();
                    JsonObjectValue subOject = (JsonObjectValue)v;
                    subOject.ProcessProperty(jsonPath, (subName, subValue) =>
                    {
                        CsdlAnnotation subAnnotation = AnnotationJsonParser.ParseCsdlAnnotation(subName, subValue, jsonPath);
                        subAnnotations.Add(subAnnotation);
                    });

                    CsdlAnnotations annotations = new CsdlAnnotations(subAnnotations, target, qualifier);
                    annotationsCollection.Add(annotations);
                }
                else
                {
                    ReportUnexpectedMember(v, jsonPath);
                }
            });

            return annotationsCollection;
        }
        #endregion

        #region EntityContainer
        /// <summary>
        /// Parse the input <see cref="IJsonValue"/> to the <see cref="CsdlEntityContainer"/>.
        /// </summary>
        /// <param name="name">The entity container unqualified name.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The input json value path.</param>
        /// <returns>null or the parsed <see cref="CsdlEntityContainer"/>.</returns>
        public CsdlEntityContainer ParseCsdlEntityContainer(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);

            // An entity container is represented as a member of the schema object:
            // whose name is the unqualified name of the entity container
            // whose value is an object.
            JsonObjectValue entityContainerObj = (JsonObjectValue)jsonValue;

            // The entity container object MUST contain the member $Kind with a string value of EntityContainer.
            ValidateKind(entityContainerObj, jsonPath, "EntityContainer", required: true);

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
                        CsdlElement element = ParseEntityContainerMember(propertyName, propertyValue, jsonPath);
                        if (element != null)
                        {
                            entityContainerMembers.Add(element);
                        }
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });

            CsdlEntityContainer entityContainer = new CsdlEntityContainer(name, extends,
                entityContainerMembers.OfType<CsdlEntitySet>(),
                entityContainerMembers.OfType<CsdlSingleton>(),
                entityContainerMembers.OfType<CsdlOperationImport>(),
                new CsdlLocation(jsonPath.Path));

            entityContainerMembers.OfType<CsdlAnnotation>().ForEach(a => entityContainer.AddAnnotation(a));
            return entityContainer;
        }

        private static CsdlElement ParseEntityContainerMember(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            Debug.Assert(name != null);
            Debug.Assert(jsonValue != null);
            Debug.Assert(jsonPath != null);

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
        #endregion

        #region Structured type
        /// <summary>
        /// Parse the input <see cref="IJsonValue"/> to the <see cref="CsdlEntityType"/>.
        /// </summary>
        /// <param name="name">the unqualified name of the entity type.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The input json value path.</param>
        /// <returns>null or the parsed <see cref="CsdlEntityType"/>.</returns>
        public CsdlEntityType ParseCsdlEntityType(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue entityObject = (JsonObjectValue)jsonValue;

            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            ValidateKind(entityObject, jsonPath, "EntityType", required: true);

            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            string baseType = null;
            bool? isOpen = null;
            bool? isAbstract = null;
            bool? hasStream = null;
            CsdlKey csdlKey = null;
            IList<CsdlElement> members = new List<CsdlElement>();
            entityObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // skip this, because it's verified at upper layer
                        break;

                    case "$BaseType":
                        // The value of $BaseType is the qualified (namespace or alias) name of the base type.
                        baseType = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Abstract":
                        // The value of $Abstract is one of the Boolean literals true or false. Absence of the member means false.
                        isAbstract = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$OpenType":
                        // The value of $OpenType is one of the Boolean literals true or false. Absence of the member means false.
                        isOpen = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$HasStream":
                        // The value of $HasStream is one of the Boolean literals true or false. Absence of the member means false.
                        hasStream = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$Key":
                        // The value of $Key is an array with one item per key property.
                        csdlKey = ParseCsdlKey(propertyName, propertyValue, jsonPath);
                        break;

                    default:
                        CsdlElement element = TryParseStructuredTypeElement(propertyName, propertyValue, jsonPath);
                        if (element != null)
                        {
                            members.Add(element);
                        }
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });


            CsdlEntityType entityType = new CsdlEntityType(name,
                baseType,
                isAbstract == null ? false : isAbstract.Value, // default is false
                isOpen == null ? false : isOpen.Value, // default is false
                hasStream == null ? false : hasStream.Value, // default is false
                csdlKey,
                members.OfType<CsdlProperty>(),
                members.OfType<CsdlNavigationProperty>(),
                new CsdlLocation(jsonPath.Path));

            foreach (var annotation in members.OfType<CsdlAnnotation>())
            {
                entityType.AddAnnotation(annotation);
            }

            return entityType;
        }

        private CsdlElement TryParseStructuredTypeElement(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            Debug.Assert(!string.IsNullOrEmpty(name), "json reader should verify that the property name string is not null or empty.");
            Debug.Assert(jsonValue != null);
            Debug.Assert(jsonPath != null);

            CsdlAnnotation annotation = AnnotationJsonParser.ParseCsdlAnnotation(name, jsonValue, jsonPath);
            if (annotation != null)
            {
                return annotation;
            }

            // The remaining is property or navigation property, both them are required an object
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            // so it's complex property
            // Property is an object
            JsonObjectValue objValue = (JsonObjectValue)jsonValue;
            string kind = GetKind(objValue, jsonPath);
            if (kind == "NavigationProperty")
            {
                return ParseCsdlNavigationProperty(name, jsonValue, jsonPath);
            }

            // The property object MAY contain the member $Kind with a string value of Property.
            // This member SHOULD be omitted to reduce document size.
            return ParseCsdlProperty(name, jsonValue, jsonPath);
        }

        /// <summary>
        /// Parse the input <see cref="IJsonValue"/> to the <see cref="CsdlComplexType"/>.
        /// </summary>
        /// <param name="name">the unqualified name of the complex type.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The input json value path.</param>
        /// <returns>null or the parsed <see cref="CsdlComplexType"/>.</returns>
        public CsdlComplexType BuildCsdlComplexType(string name, JsonObjectValue jsonValue, IJsonPath jsonPath)
        {
            // A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue complexObject = (JsonObjectValue)jsonValue;

            // The complex type object MUST contain the member $Kind with a string value of ComplexType.
            ValidateKind(complexObject, jsonPath, "ComplexType", required: true);

            // It MAY contain the members $BaseType, $Abstract, and $OpenType.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            string baseType = null;
            bool? isOpen = null;
            bool? isAbstract = null;
            IList<CsdlElement> members = new List<CsdlElement>();

            complexObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // skip this verification, because it's verified.
                        break;

                    case "$BaseType":
                        // The value of $BaseType is the qualified name of the base type.
                        baseType = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Abstract":
                        // The value of $Abstract is one of the Boolean literals true or false. Absence of the member means false.
                        isAbstract = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    case "$OpenType":
                        // The value of $OpenType is one of the Boolean literals true or false. Absence of the member means false.
                        isOpen = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        CsdlElement element = TryParseStructuredTypeElement(propertyName, propertyValue, jsonPath);
                        if(element != null)
                        {
                            members.Add(element);
                        }
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });

            CsdlComplexType csdlComplexType = new CsdlComplexType(name,
                baseType,
                isAbstract == null ? false : isAbstract.Value, // default is false
                isOpen == null ? false : isOpen.Value, // default is false
                members.OfType<CsdlProperty>(),
                members.OfType<CsdlNavigationProperty>(),
                new CsdlLocation(jsonPath.Path));

            members.OfType<CsdlAnnotation>().ForEach(a => csdlComplexType.AddAnnotation(a));
            return csdlComplexType;
        }

        /// <summary>
        /// Parse the <see cref="IJsonValue"/> to <see cref="CsdlKey"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The json path of this value.</param>
        /// <returns>the parsed <see cref="CsdlKey"/>.</returns>
        public CsdlKey ParseCsdlKey(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // The value of $Key is an array with one item per key property.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JArray);
            Debug.Assert(name == "$Key", "The name should be $Key.");

            JsonArrayValue keyArray = (JsonArrayValue)jsonValue;

            IList<CsdlPropertyReference> properties = new List<CsdlPropertyReference>();
            keyArray.ProcessItem(jsonPath, v =>
            {
                if (v.ValueKind != JsonValueKind.JPrimitive)
                {
                    // TODO: ODL doesn't support the key referencing a property of a complex type as below.
                    // "$Key": [
                    //    {
                    //      "EntityInfoID": "Info/ID"
                    //    }
                    //  ]
                    ReportUnexpectedMember(v, jsonPath);
                }

                string propertyName = v.ParseAsString(jsonPath);
                properties.Add(new CsdlPropertyReference(propertyName, new CsdlLocation(jsonPath.Path)));
            });

            return new CsdlKey(properties, new CsdlLocation(jsonPath.Path));
        }

        /// <summary>
        /// Parse the <see cref="IJsonValue"/> to <see cref="CsdlProperty"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The json path of this value.</param>
        /// <returns>the parsed <see cref="CsdlProperty"/>.</returns>
        public CsdlProperty ParseCsdlProperty(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // Structural properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue propertyObject = (JsonObjectValue)jsonValue;

            // The property object MAY contain the member $Kind with a string value of Property.
            // This member SHOULD be omitted to reduce document size.
            ValidateKind(propertyObject, jsonPath, "Property", required: false);

            // It MAY contain the member $Type, $Collection, $Nullable, $MaxLength, $Unicode, $Precision, $Scale, $SRID, and $DefaultValue.
            CsdlTypeReference typeReference = ParseCsdlTypeReference(propertyObject, jsonPath);

            string defaultValue = null;
            IList<CsdlAnnotation> propertyAnnotations = new List<CsdlAnnotation>();
            propertyObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                    case "$Type":
                    case "$Collection":
                    case "$Nullable":
                    case "$MaxLength":
                    case "$Precision":
                    case "$Scale":
                    case "$Unicode":
                    case "$SRID":
                        // skip them because they are processed
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        // So far, ODL only suppports the string default value.
                        defaultValue = propertyValue.ReadAsJsonString();
                        break;

                    default:
                        // it MAY contain annotations.
                        CsdlAnnotation annotation = AnnotationJsonParser.ParseCsdlAnnotation(propertyName, propertyValue, jsonPath);
                        if (annotation != null)
                        {
                            propertyAnnotations.Add(annotation);
                        }
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });

            CsdlProperty csdlProperty = new CsdlProperty(name, typeReference, defaultValue, new CsdlLocation(jsonPath.Path));
            propertyAnnotations.ForEach(a => csdlProperty.AddAnnotation(a));
            return csdlProperty;
        }

        /// <summary>
        /// Parse the <see cref="IJsonValue"/> to <see cref="CsdlNavigationProperty"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The json path of this value.</param>
        /// <returns>the parsed <see cref="CsdlNavigationProperty"/>.</returns>
        public CsdlNavigationProperty ParseCsdlNavigationProperty(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // Navigation properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue navigationObject = (JsonObjectValue)jsonValue;

            // The navigation property object MUST contain the member $Kind with a string value of NavigationProperty.
            ValidateKind(navigationObject, jsonPath, "NavigationProperty", required: true);

            // It MUST contain the member $Type, and it MAY contain the members $Collection, $Nullable, $Partner, $ContainsTarget, $ReferentialConstraint, and $OnDelete.
            string typeName = null;
            bool? collection = null;
            bool? nullable = null;
            string parter = null;
            bool? containsTarget = null; // Absence of the member means false.

            IList<CsdlAnnotation> onDeleteAnnotations = new List<CsdlAnnotation>();
            CsdlOnDelete csdlOnDelete = null;

            IList<CsdlAnnotation> navPropertyAnnotations = new List<CsdlAnnotation>();
            IList<CsdlReferentialConstraint> referentialConstraints = null;

            navigationObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        collection = propertyValue.ParseAsBoolean(jsonPath);
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
                        csdlOnDelete = ParseCsdlOnDelete(propertyValue, jsonPath);
                        break;

                    case "$ReferentialConstraint":
                        referentialConstraints = ParseCsdlReferentialConstraint(propertyValue, jsonPath);
                        break;

                    default:
                        // Navigation property may contain annotations
                        // Annotations for $OnDelete are prefixed with $OnDelete.
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null)
                        {
                            // termname with "@" and optional "#"
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
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });

            // If it's collection navigation property, let's construct the type name as Collection(entityType).
            if (collection != null && collection.Value)
            {
                typeName = "Collection(" + typeName + ")";
            }

            // Annotations for $OnDelete are prefixed with $OnDelete. However, It's not supported in ODL yet.
            if (csdlOnDelete != null && onDeleteAnnotations.Count > 0)
            {
                onDeleteAnnotations.ForEach(a => csdlOnDelete.AddAnnotation(a));
            }

            CsdlNavigationProperty csdlNavProperty = new CsdlNavigationProperty(name, typeName, nullable, parter,
                containsTarget != null ? containsTarget.Value : false, // Absence of the member means false.
                csdlOnDelete,
                referentialConstraints != null ? referentialConstraints : Enumerable.Empty<CsdlReferentialConstraint>(),
                new CsdlLocation(jsonPath.Path));

            navPropertyAnnotations.ForEach(a => csdlNavProperty.AddAnnotation(a));
            return csdlNavProperty;
        }

        public static CsdlOnDelete ParseCsdlOnDelete(IJsonValue value, IJsonPath jsonPath)
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

        private static IList<CsdlReferentialConstraint> ParseCsdlReferentialConstraint(IJsonValue value, IJsonPath jsonPath)
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
                    string valueString = propertyValue.ParseAsString(jsonPath);

                    // PropertyName is the dependent property
                    // The member name is the path to the dependent property, this path is relative to the structured type declaring the navigation property. 

                    // PropertyValue is the principal property.
                    // The member value is a string containing the path to the principal property, this path is relative to the entity type that is the target of the navigation property.
                    referentialConstraints.Add(new CsdlReferentialConstraint(propertyName, valueString, location));
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

        #endregion

        #region Enumeration Type

        /// <summary>
        /// Parse the <see cref="IJsonValue"/> to <see cref="CsdlEnumType"/>.
        /// The enumeration type object MUST contain the member $Kind with a string value of EnumType.
        /// It MAY contain the members $UnderlyingType and $IsFlags.
        /// The enumeration type object MUST contain members representing the enumeration type members.
        /// The enumeration type object MAY contain annotations.
        /// </summary>
        /// <param name="name">The nnum type name.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The enum type json path.</param>
        /// <returns>the parsed csdl enum type.</returns>
        public static CsdlEnumType ParseCsdlEnumType(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type and whose value is an object.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue enumObject = (JsonObjectValue)jsonValue;

            // The enumeration type object MUST contain the member $Kind with a string value of EnumType.
            ValidateKind(enumObject, jsonPath, "EnumType", required: true);

            CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlEnumMember> members = new List<CsdlEnumMember>();
            IList<CsdlAnnotation> enumAnnotations = new List<CsdlAnnotation>();
            IDictionary<string, IList<CsdlAnnotation>> memberAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();

            bool? isFlags = null;
            string underlyingTypeName = null;
            enumObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    case "$UnderlyingType":
                        // The value of $UnderlyingType is the qualified name of the underlying type.
                        underlyingTypeName = propertyValue.ParseAsString(jsonPath);
                        break;

                    case "$IsFlags":
                        // The value of $IsFlags is one of the Boolean literals true or false.Absence of the member means false.
                        isFlags = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName == null)
                        {
                            // Without '@' in the name, so it's normal enum member name
                            CsdlEnumMember enumMember = ParseCsdlEnumMember(propertyName, propertyValue, jsonPath);
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
            });

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

            CsdlEnumType enumType = new CsdlEnumType(name, underlyingTypeName, isFlags == null ? false : isFlags.Value, members, location);
            foreach (var annotation in enumAnnotations)
            {
                enumType.AddAnnotation(annotation);
            }

            return enumType;
        }

        public static CsdlEnumMember ParseCsdlEnumMember(string name, IJsonValue enumMemberObject, IJsonPath jsonPath)
        {
            // Enumeration Member Object
            // Enumeration type members are represented as JSON object members, where the object member name is the enumeration member name and the object member value is the enumeration member value.
            // For members of flags enumeration types a combined enumeration member value is equivalent to the bitwise OR of the discrete values.
            // Annotations for enumeration members are prefixed with the enumeration member name.

            long? value = enumMemberObject.ParseAsInteger(jsonPath);
            return new CsdlEnumMember(name, value, new CsdlLocation(jsonPath.Path));
        }

        #endregion

        #region Term

        /// <summary>
        /// Parse the <see cref="IJsonValue"/> to <see cref="CsdlTerm"/>.
        /// </summary>
        /// <param name="name">The type definition name.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The enum type json path.</param>
        /// <returns>the parsed csdl term.</returns>
        public CsdlTerm ParseCsdlTermType(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // Term Object
            // A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // The term object MUST contain the member $Kind with a string value of Term.
            // It MAY contain the members $Type, $Collection, $AppliesTo, $Nullable, $MaxLength, $Precision, $Scale, $SRID, and $DefaultValue, as well as $Unicode for 4.01 and greater payloads.
            // It MAY contain annotations.

            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue termObject = (JsonObjectValue)jsonValue;

            // The term object MUST contain the member $Kind with a string value of Term.
            ValidateKind(termObject, jsonPath, "Term", required: true);

            // Parse the term type.
            CsdlTypeReference typeReference = ParseCsdlTypeReference(termObject, jsonPath);

            IList<string> appliesTo = null;
            string defaultValue = null;
            IList<CsdlAnnotation> termAnnotations = new List<CsdlAnnotation>();
            termObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                    case "$Type":
                    case "$Collection":
                    case "$Nullable":
                    case "$MaxLength":
                    case "$Precision":
                    case "$Scale":
                    case "$Unicode":
                    case "$SRID":
                        // skip them because they are processed
                        break;

                    case "$BaseTerm":
                        // The value of $BaseTerm is the qualified name of the base term.
                        //Skip it because it's not supported
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        defaultValue = propertyValue.ReadAsJsonString();
                        break;

                    case "$AppliesTo":
                        // The value of $AppliesTo is an array whose items are strings containing symbolic values from the table above that identify model elements the term is intended to be applied to.
                        appliesTo = propertyValue.ParseArray<string>(jsonPath, (v, p) => v.ParseAsString(p));
                        break;

                    default:
                        // it MAY contain annotations.
                        CsdlAnnotation annotation = AnnotationJsonParser.ParseCsdlAnnotation(propertyName, propertyValue, jsonPath);
                        if (annotation != null)
                        {
                            termAnnotations.Add(annotation);
                        }
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });

            string appliesToStr = appliesTo != null ? string.Join(" ", appliesTo) : null;
            CsdlTerm termType = new CsdlTerm(name, typeReference, appliesToStr, defaultValue, new CsdlLocation(jsonPath.Path));
            termAnnotations.ForEach(a => termType.AddAnnotation(a));

            return termType;
        }
        #endregion

        #region TypeDefinition

        /// <summary>
        /// Parse the <see cref="IJsonValue"/> to <see cref="CsdlTypeDefinition"/>.
        /// </summary>
        /// <param name="name">The type definition name.</param>
        /// <param name="jsonValue">The json value to parse from.</param>
        /// <param name="jsonPath">The enum type json path.</param>
        /// <returns>the parsed csdl type definition.</returns>
        public CsdlTypeDefinition ParseCsdlTyeDefinition(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition and whose value is an object.
            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition and the member $UnderlyingType.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue typeDefinitionObject = (JsonObjectValue)jsonValue;

            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition
            ValidateKind(typeDefinitionObject, jsonPath, "TypeDefinition", required: true);

            // The type definition object MUST contain the member $UnderlyingType
            // It MAY contain the members $MaxLength, $Unicode, $Precision, $Scale, and $SRID, and it MAY contain annotations.
            string underlygingType = null;
            IList<CsdlAnnotation> typeDefinitionAnnotations = new List<CsdlAnnotation>();

            typeDefinitionObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                    case "$MaxLength":
                    case "$Unicode":
                    case "$Precision":
                    case "$Scale":
                    case "$SRID":
                        // skip them because verified or not supported.
                        break;

                    case "$UnderlyingType":
                        // The value of $UnderlyingType is the qualified name of the underlying type.
                        underlygingType = propertyValue.ParseAsString(jsonPath);
                        break;

                    default:
                        // it MAY contain annotations.
                        CsdlAnnotation annotation = AnnotationJsonParser.ParseCsdlAnnotation(propertyName, propertyValue, jsonPath);
                        if (annotation != null)
                        {
                            typeDefinitionAnnotations.Add(annotation);
                        }
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });

            CsdlTypeDefinition typeDefinition = new CsdlTypeDefinition(name, underlygingType, new CsdlLocation(jsonPath.Path));
            foreach (var item in typeDefinitionAnnotations)
            {
                typeDefinition.AddAnnotation(item);
            }

            return typeDefinition;
        }
        #endregion

        #region Operations (Action/Function)

        /// <summary>
        /// Parse the <see cref="IJsonValue"/> to a list of <see cref="CsdlOperation"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public CsdlOperation ParseCsdlOperation(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // Each action/function overload is an object
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue operationObject = (JsonObjectValue)jsonValue;

            // The action/funtion overload object MUST contain the member $Kind with a string value of Action/Function.
            string kind = GetKind(operationObject, jsonPath);
            bool isAction;
            if (kind == "Action")
            {
                isAction = true;
            }
            else if (kind == "Function")
            {
                isAction = false;
            }
            else
            {
                return null;
            }

            bool? isBound = null; // absence of the member means false
            bool? isComposable = null; // Absence of the member means false.
            string entitySetPath = null;
            IList<CsdlAnnotation> operationAnnotations = new List<CsdlAnnotation>();
            IList<CsdlOperationParameter> parameters = null;
            CsdlOperationReturn operationReturn = null;
            operationObject.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // Processed, skip it
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
                        parameters = ParseCsdlOperationParameters(propertyName, propertyValue, jsonPath);
                        break;

                    case "$ReturnType":
                        // The value of $ReturnType is an object.
                        operationReturn = ParseCsdlOperationReturn(propertyName, propertyValue, jsonPath);
                        break;

                    case "$IsComposable":
                        // The value of $IsComposable is one of the Boolean literals true or false. Absence of the member means false.
                        isComposable = propertyValue.ParseAsBoolean(jsonPath);
                        break;

                    default:
                        // it MAY contain annotations.
                        CsdlAnnotation annotation = AnnotationJsonParser.ParseCsdlAnnotation(propertyName, propertyValue, jsonPath);
                        if (annotation != null)
                        {
                            operationAnnotations.Add(annotation);
                        }
                        else
                        {
                            ReportUnexpectedMember(propertyValue, jsonPath);
                        }

                        break;
                }
            });

            CsdlLocation location = new CsdlLocation(jsonPath.Path);
            CsdlOperation csdlOperation;
            if (isAction)
            {
                // It's action
                csdlOperation = new CsdlAction(name, parameters, operationReturn, isBound == null ? false : isBound.Value, entitySetPath, location);
            }
            else
            {
                // It's function
                csdlOperation = new CsdlFunction(name, parameters, operationReturn, isBound == null ? false : isBound.Value, entitySetPath,
                    isComposable == null ? false : isComposable.Value, location);
            }

            operationAnnotations.ForEach(v => csdlOperation.AddAnnotation(v));
            return csdlOperation;
        }

        public IList<CsdlOperationParameter> ParseCsdlOperationParameters(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // The value of $Parameter is an array. The array contains one object per parameter.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JArray);
            Debug.Assert(name == "$Parameter", "The name should be $Parameter.");

            JsonArrayValue parameters = (JsonArrayValue)jsonValue;

            return parameters.ParseArray(jsonPath, BuildCsdlParameter);
        }

        private CsdlOperationParameter BuildCsdlParameter(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // A Parameter is Object
            CheckArgumentsAndValidateValueKind(" ", jsonValue, jsonPath, JsonValueKind.JObject);
            JsonObjectValue parameterObj = (JsonObjectValue)jsonValue;

            CsdlTypeReference parameterType = ParseCsdlTypeReference(parameterObj, jsonPath);

            IList<CsdlAnnotation> prameterAnnotations = new List<CsdlAnnotation>();
            string name = null;
            parameterObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Type":
                    case "$Collection":
                    case "$Nullable":
                    case "$MaxLength":
                    case "$Unicode":
                    case "$Precision":
                    case "$Scale":
                    case "$SRID":
                        // skip them because they are processed
                        break;

                    case "$Name":
                        // The value of $Name is a string containing the parameter name.
                        name = propertyValue.ParseAsString(jsonPath);
                        break;

                    default:
                        ParseCsdlAnnotation(propertyName, propertyValue, jsonPath, prameterAnnotations);
                        break;
                }

            });

            CsdlOperationParameter csdlParameter = new CsdlOperationParameter(name, parameterType, new CsdlLocation(jsonPath.Path));
            prameterAnnotations.ForEach(v => csdlParameter.AddAnnotation(v));

            return csdlParameter;
        }

        /// <summary>
        /// The value of $ReturnType is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public CsdlOperationReturn ParseCsdlOperationReturn(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // The value of $ReturnType is an object.
            CheckArgumentsAndValidateValueKind(name, jsonValue, jsonPath, JsonValueKind.JObject);
            Debug.Assert(name == "$ReturnType", "The name should be $ReturnType.");

            JsonObjectValue returnObj = (JsonObjectValue)jsonValue;

            // It MAY contain the members $Type, $Collection, $Nullable, $MaxLength, $Unicode, $Precision, $Scale, and $SRID.
            CsdlTypeReference typeReference = ParseCsdlTypeReference(returnObj, jsonPath);

            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            returnObj.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Type":
                    case "$Collection":
                    case "$Nullable":
                    case "$MaxLength":
                    case "$Unicode":
                    case "$Precision":
                    case "$Scale":
                    case "$SRID":
                        // skip them because verified or not supported.
                        break;

                    default:
                        // it MAY contain annotations.
                        ParseCsdlAnnotation(propertyName, propertyValue, jsonPath, annotations);
                        break;
                }
            });

            CsdlOperationReturn returnType = new CsdlOperationReturn(typeReference, new CsdlLocation(jsonPath.Path));
            annotations.ForEach(a => returnType.AddAnnotation(a));
            return returnType;
        }

        #endregion

        /// <summary>
        /// The schema object MAY contain members representing:
        /// entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
        /// This method trys to build the JSON value as one of the above member.
        /// </summary>
        /// <param name="name">The name of the schema member.</param>
        /// <param name="jsonValue">The schema member json value.</param>
        /// <param name="jsonPath">The JSON path.</param>
        /// <returns>Null or the built element.</returns>
        private CsdlElement TryParseSchemaElement(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            Debug.Assert(jsonValue != null);

            // The schema object member is representing entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
            // The JSON value of each member is a JSON object, if it's not an object, it's not a schema element.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(jsonPath != null);

            JsonObjectValue schemaElementObject = (JsonObjectValue)jsonValue;

            // Each schema member oject should include "$Kind" member, whose value is a string.
            string kind = GetKind(schemaElementObject, jsonPath);
            switch (kind)
            {
                case "EntityContainer":
                    return ParseCsdlEntityContainer(name, schemaElementObject, jsonPath);

                case "EntityType":
                    return ParseCsdlEntityType(name, schemaElementObject, jsonPath);

                case "ComplexType":
                    return BuildCsdlComplexType(name, schemaElementObject, jsonPath);

                case "EnumType":
                    return ParseCsdlEnumType(name, schemaElementObject, jsonPath);

                case "TypeDefinition":
                    return ParseCsdlTyeDefinition(name, schemaElementObject, jsonPath);

                case "Term":
                    return ParseCsdlTermType(name, schemaElementObject, jsonPath);

                default:
                    return null;
            }
        }

        private IList<CsdlOperation> TryParseCsdlOperationOverload(string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            Debug.Assert(jsonValue != null);

            // An action/function is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array.
            // The array contains one object per action/function overload.
            if (jsonValue.ValueKind != JsonValueKind.JArray)
            {
                return null;
            }

            JsonArrayValue operationArray = (JsonArrayValue)jsonValue;
            return operationArray.ParseArray(jsonPath, (v, p) => ParseCsdlOperation(name, v, p));
        }

        private void ParseCsdlAnnotation(string name, IJsonValue jsonValue, IJsonPath jsonPath, IList<CsdlAnnotation> annotationContainer)
        {
            Debug.Assert(name != null);
            Debug.Assert(jsonValue != null);
            Debug.Assert(jsonPath != null);
            Debug.Assert(annotationContainer != null);

            CsdlAnnotation annotation = AnnotationJsonParser.ParseCsdlAnnotation(name, jsonValue, jsonPath);
            if (annotation != null)
            {
                annotationContainer.Add(annotation);
            }
            else
            {
                ReportUnexpectedMember(jsonValue, jsonPath);
            }
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
                termName = name.Substring(index); // with @
                return name.Substring(0, index);
            }
        }

        private static CsdlTypeReference ParseCsdlTypeReference(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            Debug.Assert(objectValue != null && jsonPath != null);

            // $Type
            // For single-valued terms the value of $Type is the qualified name of the property/term’s type.
            // For collection-valued terms the value of $Type is the qualified name of the term’s item type, and the member $Collection MUST be present with the literal value true.
            // Absence of the $Type member means the type is Edm.String.
            string typeName = objectValue.ParseOptionalProperty("$Type", jsonPath, (v, p) => v.ParseAsString(p));
            typeName = typeName == null ? "Edm.String" : typeName;

            // $Collection
            // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
            // and the member $Collection MUST be present with the literal value true.
            bool? collection = objectValue.ParseOptionalProperty("$Collection", jsonPath, (v, p) => v.ParseAsBoolean(p));

            // $Nullable,
            // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
            bool? isNullable = objectValue.ParseOptionalProperty("$Nullable", jsonPath, (v, p) => v.ParseAsBoolean(p));
            bool nullable = isNullable == null ? false : isNullable.Value;

            // $MaxLength,
            // The value of $MaxLength is a positive integer.
            // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
            int? maxLength = objectValue.ParseOptionalProperty("$MaxLength", jsonPath, (v, p) => v.ParseAsInteger(p));

            // $Unicode,
            // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
            bool? unicode = objectValue.ParseOptionalProperty("$Unicode", jsonPath, (v, p) => v.ParseAsBoolean(p));

            // $Precision,
            // The value of $Precision is a number.
            int? precision = objectValue.ParseOptionalProperty("$Precision", jsonPath, (v, p) => v.ParseAsInteger(p));

            // $Scale,
            int? scale = objectValue.ParseOptionalProperty("$Scale", jsonPath, (v, p) => v.ParseAsInteger(p));

            // $SRID,
            // The value of $SRID is a string containing a number or the symbolic value variable.
            // So far, ODL doesn't support string of SRID.
            int? srid = objectValue.ParseOptionalProperty("$SRID", jsonPath, (v, p) => v.ParseAsInteger(p));

            CsdlLocation location = new CsdlLocation(jsonPath.Path);
            CsdlTypeReference csdlType = ParseNamedTypeReference(typeName, nullable, false, maxLength, unicode, precision, scale, srid, location);
            if (collection != null && collection.Value)
            {
                csdlType = new CsdlExpressionTypeReference(new CsdlCollectionType(csdlType, location), nullable, location);
            }

            return csdlType;
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

        /// <summary>
        /// Reporting the unexpected member.
        /// </summary>
        /// <param name="propertyValue">The currect json node value.</param>
        /// <param name="jsonPath">The current json node. which include the member name.</param>
        private void ReportUnexpectedMember(IJsonValue propertyValue, IJsonPath jsonPath)
        {
            Debug.Assert(propertyValue != null);
            Debug.Assert(jsonPath != null);

            if (_options.IgnoreUnexpectedMembers)
            {
                return;
            }

            throw new CsdlParseException(Strings.CsdlJsonParser_UnexpectedJsonMember(jsonPath, propertyValue.ValueKind));
        }

        internal static void CheckArgumentsAndValidateValueKind(string name, IJsonValue jsonValue, IJsonPath jsonPath, JsonValueKind expectedValueKind)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            // Make sure the input json value
            if (jsonValue.ValueKind != expectedValueKind)
            {
                throw new CsdlParseException(Strings.CsdlJsonParser_UnexpectedJsonValueKind(jsonValue.ValueKind, jsonPath.Path, expectedValueKind));
            }
        }

        internal static void ValidateKind(JsonObjectValue objectValue, IJsonPath jsonPath, string expectedKind, bool required)
        {
            Debug.Assert(objectValue != null);
            Debug.Assert(jsonPath != null);

            string kind = GetKind(objectValue, jsonPath);

            if (kind != null && kind == expectedKind)
            {
                // If existed, must equal to the expected kind.
                return;
            }
            else if (kind == null && !required)
            {
                // if non-existed and optional, it's ok.
                return;
            }

            throw new CsdlParseException(Strings.CsdlJsonParser_MissingKindMember(jsonPath.Path, expectedKind));
        }

        private static string GetKind(JsonObjectValue objValue, IJsonPath jsonPath)
        {
            Debug.Assert(objValue != null);
            Debug.Assert(jsonPath != null);

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
