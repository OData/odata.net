//---------------------------------------------------------------------
// <copyright file="SchemaJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides for the loading and conversion of one or more CSDL XML readers into Entity Data Model.
    /// </summary>
    internal static class SchemaJsonParser
    {
        /// <summary>
        /// Parse the input <see cref="JsonElement"/> to the <see cref="CsdlSchema"/>.
        /// </summary>
        /// <param name="schemaNamespace">The namespace of the schema.</param>
        /// <param name="version">The CSDL version.</param>
        /// <param name="element">The json element to parse from.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>The parsed <see cref="CsdlSchema"/>.</returns>
        internal static CsdlSchema ParseCsdlSchema(string schemaNamespace, Version version, JsonElement element, JsonParserContext context)
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            Debug.Assert(element.ValueKind == JsonValueKind.Object);

            IList<CsdlAnnotations> outOfLineAnnotations = new List<CsdlAnnotations>();
            List<CsdlElement> csdlElements = new List<CsdlElement>();
            List<CsdlOperation> csdlOpertions = new List<CsdlOperation>();
            string alias = null;
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the schema.
                        alias = propertyValue.ParseAsString(context);
                        break;

                    case "$Annotations":
                        // The value of $Annotations is an object with one member per annotation target.
                        outOfLineAnnotations = ParseCsdlOutOfLineAnnotations(propertyName, propertyValue, context);
                        break;

                    default:
                        // The schema object MAY contain members representing entity types,
                        // complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
                        CsdlElement csdlElement = TryParseSchemaElement(propertyName, propertyValue, context);
                        if (csdlElement != null)
                        {
                            csdlElements.Add(csdlElement);
                        }
                        else
                        {
                            IList<CsdlOperation> operations = TryParseCsdlOperationOverload(propertyName, propertyValue, context);
                            if (operations != null)
                            {
                                csdlOpertions.AddRange(operations);
                            }
                            else
                            {
                                // The schema object MAY also contain annotations that apply to the schema itself.
                                // ODL doesn't support the Schema itself annotations. So also reporting them as unexpected member.
                                ReportUnexpectedElement(propertyValue, context);
                            }
                        }

                        break;
                }
            });

            CsdlSchema schema = new CsdlSchema(schemaNamespace, alias, version,
                csdlElements.OfType<CsdlStructuredType>(),
                csdlElements.OfType<CsdlEnumType>(),
                csdlOpertions,
                csdlElements.OfType<CsdlTerm>(),
                csdlElements.OfType<CsdlEntityContainer>(),
                outOfLineAnnotations,
                csdlElements.OfType<CsdlTypeDefinition>(),
                context.Location());

            if (schema.EntityContainers.Count() > 1)
            {
                context.ReportError(EdmErrorCode.SchemaCannotHaveMoreThanOneEntityContainer,
                    Strings.CsdlJsonParser_SchemaCannotHaveMoreThanOneEntityContainer(context.Path));
            }

            return schema;
        }

        #region OutOfLine Annotion
        // out of line annotations

        /// <summary>
        ///         * "org.example": {
        ///  "$Alias": "self",
        ///  "$Annotations": {
        ///    "self.Person": {
        ///      "@Core.Description#Tablet": "Dummy",
        ///      …
        ///    }
        ///  }
        ///}
        /// </summary>
        /// <param name="name">The annotation name.</param>
        /// <param name="element">The json element to parse from.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>The collection of CSDL annotation.</returns>
        internal static IList<CsdlAnnotations> ParseCsdlOutOfLineAnnotations(string name, JsonElement element, JsonParserContext context)
        {
            Debug.Assert(name == "$Annotations");
            Debug.Assert(element.ValueKind == JsonValueKind.Object);

            IList<CsdlAnnotations> annotationsCollection = new List<CsdlAnnotations>();
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                // The member name is a path identifying the annotation target
                string target = propertyName;

                string qualifier = null; // the qualifier is set on the individual annotations, not on the Annotations annotation.

                // the member value is an object containing annotations for that target.
                if (propertyValue.ValueKind == JsonValueKind.Object)
                {
                    IList<CsdlAnnotation> subAnnotations = new List<CsdlAnnotation>();
                    propertyValue.ParseAsObject(context, (subName, subValue) =>
                    {
                        if (AnnotationJsonParser.TryParseCsdlAnnotation(subName, subValue, context, out CsdlAnnotation subAnnotation))
                        {
                            subAnnotations.Add(subAnnotation);
                        }
                    });

                    CsdlAnnotations annotations = new CsdlAnnotations(subAnnotations, target, qualifier);
                    annotationsCollection.Add(annotations);
                }
                else
                {
                    ReportUnexpectedElement(propertyValue, context);
                }
            });

            return annotationsCollection;
        }
        #endregion

        #region EntityContainer
        /// <summary>
        /// Parse the input <see cref="JsonElement"/> to the <see cref="CsdlEntityContainer"/>.
        /// </summary>
        /// <param name="name">The entity container unqualified name.</param>
        /// <param name="containerObject">The json value to parse from.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>null or the parsed <see cref="CsdlEntityContainer"/>.</returns>
        internal static CsdlEntityContainer ParseCsdlEntityContainer(string name, JsonElement containerObject, JsonParserContext context)
        {
            // An entity container is represented as a member of the schema object:
            // whose name is the unqualified name of the entity container
            // whose value is an object.
            Debug.Assert(containerObject.ValueKind == JsonValueKind.Object);

            // The entity container object MUST contain the member $Kind with a string value of EntityContainer.
            ValidateKind(containerObject, context, "EntityContainer", required: true);

            IList<CsdlElement> entityContainerMembers = new List<CsdlElement>();
            string extends = null;
            containerObject.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // Skip it, it's parsed above.
                        break;

                    case "$Extends":
                        // The entity container object MAY contain the member $Extends,
                        // The value of $Extends is the qualified name of the entity container to be extended.
                        extends = propertyValue.ParseAsString(context);
                        break;

                    default:
                        CsdlElement element = ParseEntityContainerMember(propertyName, propertyValue, context);
                        if (element != null)
                        {
                            entityContainerMembers.Add(element);
                        }
                        else
                        {
                            ReportUnexpectedElement(propertyValue, context);
                        }

                        break;
                }
            });

            CsdlEntityContainer entityContainer = new CsdlEntityContainer(name, extends,
                entityContainerMembers.OfType<CsdlEntitySet>(),
                entityContainerMembers.OfType<CsdlSingleton>(),
                entityContainerMembers.OfType<CsdlOperationImport>(),
                context.Location());

            entityContainerMembers.OfType<CsdlAnnotation>().ForEach(a => entityContainer.AddAnnotation(a));
            return entityContainer;
        }

        private static CsdlElement ParseEntityContainerMember(string name, JsonElement element, JsonParserContext context)
        {
            Debug.Assert(name != null);
            Debug.Assert(context != null);

            // It maybe entity container's annotation
            if (AnnotationJsonParser.TryParseCsdlAnnotation(name, element, context, out CsdlAnnotation annotation))
            {
                return annotation;
            }

            CsdlOperationImport operationImport;
            if (TryParseOperationImport(name, element, context, out operationImport))
            {
                return operationImport;
            }

            CsdlAbstractNavigationSource navigationSource;
            if (TryParseNavigationSource(name, element, context, out navigationSource))
            {
                return navigationSource;
            }

            return null;
        }

        internal static CsdlActionImport ParseCsdlActionImport(string name, JsonElement element, JsonParserContext context)
        {
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            string action = null;
            string entitySet = null;
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Action":
                        // The value of $Action is a string containing the qualified name of an unbound action.
                        action = propertyValue.ParseAsString(context);
                        break;

                    case "$EntitySet":
                        // The value of $EntitySet is a string containing either the unqualified name of an entity set in the same entity container
                        // or a path to an entity set in a different entity container.
                        entitySet = propertyValue.ParseAsString(context);
                        break;

                    default:
                        // It may contain annotation
                        ParseCsdlAnnotation(propertyName, propertyValue, context, annotations);
                        break;
                }
            });

            CsdlActionImport actionImport = new CsdlActionImport(name, action, entitySet, context.Location());
            annotations.ForEach(a => actionImport.AddAnnotation(a));
            return actionImport;
        }

        internal static CsdlFunctionImport ParseCsdlFunctionImport(string name, JsonElement element, JsonParserContext context)
        {
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            string function = null;
            string entitySet = null;
            bool? includeInServiceDocument = false; // Absence of the member means false.
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Function":
                        // The value of $Function is a string containing the qualified name of an unbound function.
                        function = propertyValue.ParseAsString(context);
                        break;

                    case "$EntitySet":
                        // The value of $EntitySet is a string containing either the unqualified name of an entity set in the same entity container
                        // or a path to an entity set in a different entity container.
                        entitySet = propertyValue.ParseAsString(context);
                        break;

                    case "$IncludeInServiceDocument":
                        // The value of $IncludeInServiceDocument is one of the Boolean literals true or false. Absence of the member means false.
                        includeInServiceDocument = propertyValue.ParseAsBoolean(context);
                        break;

                    default:
                        // It may contain annotation
                        ParseCsdlAnnotation(propertyName, propertyValue, context, annotations);
                        break;
                }
            });

            CsdlFunctionImport functionImport = new CsdlFunctionImport(name, function, entitySet, includeInServiceDocument.Value, context.Location());
            annotations.ForEach(a => functionImport.AddAnnotation(a));
            return functionImport;
        }

        internal static bool TryParseOperationImport(string name, JsonElement element, JsonParserContext context, out CsdlOperationImport operationImport)
        {
            operationImport = null;
            if (element.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            if (element.TryGetProperty("$Action", out _))
            {
                // ActionImport
                operationImport = ParseCsdlActionImport(name, element, context);
                return true;
            }

            if (element.TryGetProperty("$Function", out _))
            {
                // FunctionImport
                operationImport = ParseCsdlFunctionImport(name, element, context);
                return true;
            }

            return false;
        }

        public static bool TryParseNavigationSource(string name, JsonElement element, JsonParserContext context, out CsdlAbstractNavigationSource navigationSource)
        {
            navigationSource = null;
            if (element.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            IList<CsdlNavigationPropertyBinding> navigationPropertyBindings = new List<CsdlNavigationPropertyBinding>();
            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            string type = null;
            bool? isCollection = null;
            bool? includeInServiceDocument = null;
            bool? nullable = null;
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Type":
                        // The value of $Type is the qualified name of an entity type.
                        type = propertyValue.ParseAsString(context);
                        break;

                    case "$Collection":
                        // The value of $Collection is the Booelan value true.
                        isCollection = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$IncludeInServiceDocument":
                        // The value of $IncludeInServiceDocument is one of the Boolean literals true or false. Absence of the member means true.
                        includeInServiceDocument = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        // In OData 4.0 responses this member MUST NOT be specified.
                        nullable = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$NavigationPropertyBinding":
                        // The value of $NavigationPropertyBinding is an object.
                        navigationPropertyBindings = ParseCsdlNavigationPropertyBinding(propertyValue, context);
                        break;

                    default:
                        // It may contain annotation
                        ParseCsdlAnnotation(propertyName, propertyValue, context, annotations);
                        break;
                }
            });

            if (type == null)
            {
                return false;
            }

            CsdlLocation location = context.Location();

            if (isCollection != null)
            {
                // entitySet
                if (!isCollection.Value)
                {
                    // If presented, $IsCollection should be true
                    context.ReportError(EdmErrorCode.InvalidCollectionValue, "$IsCollection should be true");
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

        public static IList<CsdlNavigationPropertyBinding> ParseCsdlNavigationPropertyBinding(JsonElement element, JsonParserContext context)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            CsdlLocation location = context.Location();

            IList<CsdlNavigationPropertyBinding> bindings = new List<CsdlNavigationPropertyBinding>();
            foreach (JsonProperty property in element.EnumerateObject())
            {
                string bindingPath = property.Name;
                string target = property.Value.GetString();

                bindings.Add(new CsdlNavigationPropertyBinding(bindingPath, target, location));
            }

            return bindings;
        }
        #endregion

        #region Structured type
        /// <summary>
        /// Parse the input <see cref="JsonElement"/> to the <see cref="CsdlEntityType"/>.
        /// </summary>
        /// <param name="name">the unqualified name of the entity type.</param>
        /// <param name="entityObject">The json value to parse from.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>null or the parsed <see cref="CsdlEntityType"/>.</returns>
        internal static CsdlEntityType ParseCsdlEntityType(string name, JsonElement entityObject, JsonParserContext context)
        {
            // An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
            Debug.Assert(entityObject.ValueKind == JsonValueKind.Object);

            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            ValidateKind(entityObject, context, "EntityType", required: true);

            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            string baseType = null;
            bool? isOpen = null;
            bool? isAbstract = null;
            bool? hasStream = null;
            CsdlKey csdlKey = null;
            IList<CsdlElement> members = new List<CsdlElement>();
            entityObject.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // skip this, because it's verified at upper layer
                        break;

                    case "$BaseType":
                        // The value of $BaseType is the qualified (namespace or alias) name of the base type.
                        baseType = propertyValue.ParseAsString(context);
                        break;

                    case "$Abstract":
                        // The value of $Abstract is one of the Boolean literals true or false. Absence of the member means false.
                        isAbstract = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$OpenType":
                        // The value of $OpenType is one of the Boolean literals true or false. Absence of the member means false.
                        isOpen = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$HasStream":
                        // The value of $HasStream is one of the Boolean literals true or false. Absence of the member means false.
                        hasStream = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$Key":
                        // The value of $Key is an array with one item per key property.
                        csdlKey = ParseCsdlKey(propertyName, propertyValue, context);
                        break;

                    default:
                        CsdlElement element = TryParseStructuredTypeElement(propertyName, propertyValue, context);
                        if (element != null)
                        {
                            members.Add(element);
                        }
                        else
                        {
                            ReportUnexpectedElement(propertyValue, context);
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
                context.Location());

            foreach (var annotation in members.OfType<CsdlAnnotation>())
            {
                entityType.AddAnnotation(annotation);
            }

            return entityType;
        }

        internal static CsdlElement TryParseStructuredTypeElement(string name, JsonElement element, JsonParserContext context)
        {
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            Debug.Assert(!string.IsNullOrEmpty(name), "json reader should verify that the property name string is not null or empty.");
            Debug.Assert(context != null);

            if (AnnotationJsonParser.TryParseCsdlAnnotation(name, element, context, out CsdlAnnotation annotation))
            {
                return annotation;
            }

            // The remaining is property or navigation property, both them are required an object
            if (element.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            // so it's structured property, Property is an object
            string kind = GetKind(element, context);
            if (kind == "NavigationProperty")
            {
                return ParseCsdlNavigationProperty(name, element, context);
            }

            // The property object MAY contain the member $Kind with a string value of Property.
            // This member SHOULD be omitted to reduce document size.
            return ParseCsdlProperty(name, element, context);
        }

        /// <summary>
        /// Parse the input <see cref="JsonElement"/> to the <see cref="CsdlComplexType"/>.
        /// </summary>
        /// <param name="name">the unqualified name of the complex type.</param>
        /// <param name="complexObject">The json value to parse from.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>null or the parsed <see cref="CsdlComplexType"/>.</returns>
        internal static CsdlComplexType ParseCsdlComplexType(string name, JsonElement complexObject, JsonParserContext context)
        {
            // A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
            Debug.Assert(complexObject.ValueKind == JsonValueKind.Object);

            // The complex type object MUST contain the member $Kind with a string value of ComplexType.
            ValidateKind(complexObject, context, "ComplexType", required: true);

            // It MAY contain the members $BaseType, $Abstract, and $OpenType.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            string baseType = null;
            bool? isOpen = null;
            bool? isAbstract = null;
            IList<CsdlElement> members = new List<CsdlElement>();

            complexObject.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // skip this verification, because it's verified.
                        break;

                    case "$BaseType":
                        // The value of $BaseType is the qualified name of the base type.
                        baseType = propertyValue.ParseAsString(context);
                        break;

                    case "$Abstract":
                        // The value of $Abstract is one of the Boolean literals true or false. Absence of the member means false.
                        isAbstract = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$OpenType":
                        // The value of $OpenType is one of the Boolean literals true or false. Absence of the member means false.
                        isOpen = propertyValue.ParseAsBoolean(context);
                        break;

                    default:
                        CsdlElement element = TryParseStructuredTypeElement(propertyName, propertyValue, context);
                        if (element != null)
                        {
                            members.Add(element);
                        }
                        else
                        {
                            ReportUnexpectedElement(propertyValue, context);
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
                context.Location());

            members.OfType<CsdlAnnotation>().ForEach(a => csdlComplexType.AddAnnotation(a));
            return csdlComplexType;
        }

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to <see cref="CsdlKey"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="keyArray">The json value to parse from.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>the parsed <see cref="CsdlKey"/>.</returns>
        internal static CsdlKey ParseCsdlKey(string name, JsonElement keyArray, JsonParserContext context)
        {
            // The value of $Key is an array with one item per key property.
            Debug.Assert(keyArray.ValueKind == JsonValueKind.Array);
            Debug.Assert(name == "$Key", "The name should be $Key.");

            IList<CsdlPropertyReference> properties = keyArray.ParseAsArray(context, (v, p) =>
            {
                if (v.ValueKind == JsonValueKind.Object)
                {
                    // TODO: ODL doesn't support the key referencing a property of a complex type as below.
                    // "$Key": [
                    //    {
                    //      "EntityInfoID": "Info/ID"
                    //    }
                    //  ]
                    p.ReportError(EdmErrorCode.InvalidKeyValue, "It doesn't support the key object.");
                }

                string propertyName = v.ParseAsString(context);
                return new CsdlPropertyReference(propertyName, context.Location());
            });

            return new CsdlKey(properties, context.Location());
        }

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to <see cref="CsdlProperty"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="propertyObject">The json value to parse from.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>the parsed <see cref="CsdlProperty"/>.</returns>
        internal static CsdlProperty ParseCsdlProperty(string name, JsonElement propertyObject, JsonParserContext context)
        {
            // Structural properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            Debug.Assert(propertyObject.ValueKind == JsonValueKind.Object);

            // The property object MAY contain the member $Kind with a string value of Property.
            // This member SHOULD be omitted to reduce document size.
            ValidateKind(propertyObject, context, "Property", required: false);

            // It MAY contain the member $Type, $Collection, $Nullable, $MaxLength, $Unicode, $Precision, $Scale, $SRID, and $DefaultValue.
            CsdlTypeReference typeReference = CsdlJsonParseHelper.ParseCsdlTypeReference(propertyObject, context);

            string defaultValue = null;
            IList<CsdlAnnotation> propertyAnnotations = new List<CsdlAnnotation>();
            propertyObject.ParseAsObject(context, (propertyName, propertyValue) =>
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
                        defaultValue = propertyValue.ParseAsJson();
                        break;

                    default:
                        // it MAY contain annotations.
                        ParseCsdlAnnotation(propertyName, propertyValue, context, propertyAnnotations);
                        break;
                }
            });

            CsdlProperty csdlProperty = new CsdlProperty(name, typeReference, defaultValue, context.Location());
            propertyAnnotations.ForEach(a => csdlProperty.AddAnnotation(a));
            return csdlProperty;
        }

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to <see cref="CsdlNavigationProperty"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="navObject">The json value to parse from.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>the parsed <see cref="CsdlNavigationProperty"/>.</returns>
        internal static CsdlNavigationProperty ParseCsdlNavigationProperty(string name, JsonElement navObject, JsonParserContext context)
        {
            Debug.Assert(navObject.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            // Navigation properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            // The navigation property object MUST contain the member $Kind with a string value of NavigationProperty.
            ValidateKind(navObject, context, "NavigationProperty", required: true);

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

            navObject.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        break;

                    case "$Type":
                        // For the type name of the navigation property.
                        typeName = propertyValue.ParseAsString(context);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        collection = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$Partner":
                        // The value of $Partner is a string containing the path to the partner navigation property.
                        parter = propertyValue.ParseAsString(context);
                        break;

                    case "$ContainsTarget":
                        // The value of $ContainsTarget is one of the Boolean literals true or false. Absence of the member means false.
                        containsTarget = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$OnDelete":
                        // The value of $OnDelete is a string with one of the values Cascade, None, SetNull, or SetDefault.
                        csdlOnDelete = ParseCsdlOnDelete(propertyValue, context);
                        break;

                    case "$ReferentialConstraint":
                        referentialConstraints = ParseCsdlReferentialConstraint(propertyValue, context);
                        break;

                    default:
                        // Navigation property may contain annotations
                        // Annotations for $OnDelete are prefixed with $OnDelete.
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null)
                        {
                            // termname with "@" and optional "#"
                            if (AnnotationJsonParser.TryParseCsdlAnnotation(termName, propertyValue, context, out CsdlAnnotation annotation))
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
                            ReportUnexpectedElement(propertyValue, context);
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
                context.Location());

            navPropertyAnnotations.ForEach(a => csdlNavProperty.AddAnnotation(a));
            return csdlNavProperty;
        }

        public static CsdlOnDelete ParseCsdlOnDelete(JsonElement element, JsonParserContext context)
        {
            string onDelete = element.ParseAsString(context);

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

            return new CsdlOnDelete(edmOnDelete, context.Location());
        }

        private static IList<CsdlReferentialConstraint> ParseCsdlReferentialConstraint(JsonElement element, JsonParserContext context)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            // The value of $ReferentialConstraint is an object with one member per referential constraint.
            // The member name is the path to the dependent property, this path is relative to the structured type declaring the navigation property.
            // The member value is a string containing the path to the principal property, this path is relative to the entity type that is the target of the navigation property.
            // It also MAY contain annotations.These are prefixed with the path of the dependent property of the annotated referential constraint.
            IList<CsdlReferentialConstraint> referentialConstraints = new List<CsdlReferentialConstraint>();
            IDictionary<string, IList<CsdlAnnotation>> itemsAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();
            CsdlLocation location = new CsdlLocation(-1, -1);
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                string termName;
                propertyName = SeperateAnnotationName(propertyName, out termName);
                if (termName != null)
                {
                    // "CategoryKind@Core.Description": "Referential Constraint to non-key property"
                    if (AnnotationJsonParser.TryParseCsdlAnnotation(termName, propertyValue, context, out CsdlAnnotation annotation))
                    {
                        IList<CsdlAnnotation> annotations;
                        if (!itemsAnnotations.TryGetValue(propertyName, out annotations))
                        {
                            annotations = new List<CsdlAnnotation>();
                            itemsAnnotations[propertyName] = annotations;
                        }

                        annotations.Add(annotation);
                    }
                }
                else
                {
                    // "CategoryKind": "Kind"
                    string valueString = propertyValue.ParseAsString(context);

                    // PropertyName is the dependent property
                    // The member name is the path to the dependent property, this path is relative to the structured type declaring the navigation property. 

                    // PropertyValue is the principal property.
                    // The member value is a string containing the path to the principal property, this path is relative to the entity type that is the target of the navigation property.
                    referentialConstraints.Add(new CsdlReferentialConstraint(propertyName, valueString, location));
                }
            });

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
        /// Parse the <see cref="JsonElement"/> to <see cref="CsdlEnumType"/>.
        /// The enumeration type object MUST contain the member $Kind with a string value of EnumType.
        /// It MAY contain the members $UnderlyingType and $IsFlags.
        /// The enumeration type object MUST contain members representing the enumeration type members.
        /// The enumeration type object MAY contain annotations.
        /// </summary>
        /// <param name="name">The nnum type name.</param>
        /// <param name="enumObject">The json value to parse from.</param>
        /// <param name="context">The enum type json path.</param>
        /// <returns>the parsed csdl enum type.</returns>
        internal static CsdlEnumType ParseCsdlEnumType(string name, JsonElement enumObject, JsonParserContext context)
        {
            Debug.Assert(enumObject.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            // An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type and whose value is an object.
            // The enumeration type object MUST contain the member $Kind with a string value of EnumType.
            ValidateKind(enumObject, context, "EnumType", required: true);

            CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlEnumMember> members = new List<CsdlEnumMember>();
            IList<CsdlAnnotation> enumAnnotations = new List<CsdlAnnotation>();
            IDictionary<string, IList<CsdlAnnotation>> memberAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();

            bool? isFlags = null;
            string underlyingTypeName = null;
            enumObject.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    case "$UnderlyingType":
                        // The value of $UnderlyingType is the qualified name of the underlying type.
                        underlyingTypeName = propertyValue.ParseAsString(context);
                        break;

                    case "$IsFlags":
                        // The value of $IsFlags is one of the Boolean literals true or false.Absence of the member means false.
                        isFlags = propertyValue.ParseAsBoolean(context);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName == null)
                        {
                            // Without '@' in the name, so it's normal enum member name
                            CsdlEnumMember enumMember = ParseCsdlEnumMember(propertyName, propertyValue, context);
                            members.Add(enumMember);
                            break;
                        }
                        else
                        {
                            if (AnnotationJsonParser.TryParseCsdlAnnotation(termName, propertyValue, context, out CsdlAnnotation annotation))
                            {
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

                                break;
                            }
                        }

                        ReportUnexpectedElement(propertyValue, context);
                        break;
                }
            });

            foreach (var item in memberAnnotations)
            {
                // TODO: maybe refactor later for performance
                CsdlEnumMember member = members.FirstOrDefault(c => c.Name == item.Key);
                if (member == null)
                {
                    context.ReportError(EdmErrorCode.UnexpectedElement, "Cannot find enum member that an annotation applys for.");
                    continue;
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

        internal static CsdlEnumMember ParseCsdlEnumMember(string name, JsonElement enumMemberObject, JsonParserContext context)
        {
            // Enumeration Member Object
            // Enumeration type members are represented as JSON object members, where the object member name is the enumeration member name and the object member value is the enumeration member value.
            // For members of flags enumeration types a combined enumeration member value is equivalent to the bitwise OR of the discrete values.
            // Annotations for enumeration members are prefixed with the enumeration member name.

            long? value = enumMemberObject.ParseAsInteger(context);
            return new CsdlEnumMember(name, value, context.Location());
        }

        #endregion

        #region Term

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to <see cref="CsdlTerm"/>.
        /// </summary>
        /// <param name="name">The type definition name.</param>
        /// <param name="element">The json value to parse from.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>the parsed csdl term.</returns>
        internal static CsdlTerm ParseCsdlTermType(string name, JsonElement element, JsonParserContext context)
        {
            // Term Object
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            // A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // The term object MUST contain the member $Kind with a string value of Term.
            ValidateKind(element, context, "Term", required: true);

            // It MAY contain the members $Type, $Collection, $AppliesTo, $Nullable, $MaxLength, $Precision, $Scale, $SRID, and $DefaultValue, as well as $Unicode for 4.01 and greater payloads.
            // It MAY contain annotations.
            // Parse the term type.
            CsdlTypeReference typeReference = CsdlJsonParseHelper.ParseCsdlTypeReference(element, context);

            IList<string> appliesTo = null;
            string defaultValue = null;
            IList<CsdlAnnotation> termAnnotations = new List<CsdlAnnotation>();
            element.ParseAsObject(context, (propertyName, propertyValue) =>
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
                        // Skip it because it's not supported
                        context.ReportError(EdmErrorCode.UnexpectedElement, Strings.CsdlJsonParser_UnexpectedJsonMember(context.Path, element.ValueKind));
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        defaultValue = propertyValue.ParseAsJson();
                        break;

                    case "$AppliesTo":
                        // The value of $AppliesTo is an array whose items are strings containing symbolic values from the table above that identify model elements the term is intended to be applied to.
                        appliesTo = propertyValue.ParseAsArray<string>(context, (v, p) => v.ParseAsString(p));
                        break;

                    default:
                        // it MAY contain annotations.
                        ParseCsdlAnnotation(propertyName, propertyValue, context, termAnnotations);
                        break;
                }
            });

            string appliesToStr = appliesTo != null ? string.Join(" ", appliesTo) : null;
            CsdlTerm termType = new CsdlTerm(name, typeReference, appliesToStr, defaultValue, context.Location());
            termAnnotations.ForEach(a => termType.AddAnnotation(a));

            return termType;
        }
        #endregion

        #region TypeDefinition

        /// <summary>
        /// Parse the <see cref="JsonElement"/> to <see cref="CsdlTypeDefinition"/>.
        /// </summary>
        /// <param name="name">The type definition name.</param>
        /// <param name="element">The json value to parse from.</param>
        /// <param name="context">The enum type json path.</param>
        /// <returns>the parsed csdl type definition.</returns>
        internal static CsdlTypeDefinition ParseCsdlTypeDefinition(string name, JsonElement element, JsonParserContext context)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            // A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition and whose value is an object.
            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition and the member $UnderlyingType.

            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition
            ValidateKind(element, context, "TypeDefinition", required: true);

            // The type definition object MUST contain the member $UnderlyingType
            // It MAY contain the members $MaxLength, $Unicode, $Precision, $Scale, and $SRID, and it MAY contain annotations.
            string underlygingType = null;
            IList<CsdlAnnotation> typeDefinitionAnnotations = new List<CsdlAnnotation>();

            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                    case "$MaxLength":
                    case "$Unicode":
                    case "$Precision":
                    case "$Scale":
                    case "$SRID":
                        // skip them because verified ($Kind) or not supported (others).
                        break;

                    case "$UnderlyingType":
                        // The value of $UnderlyingType is the qualified name of the underlying type.
                        underlygingType = propertyValue.ParseAsString(context);
                        break;

                    default:
                        // it MAY contain annotations.
                        ParseCsdlAnnotation(propertyName, propertyValue, context, typeDefinitionAnnotations);
                        break;
                }
            });

            CsdlTypeDefinition typeDefinition = new CsdlTypeDefinition(name, underlygingType, context.Location());
            foreach (var item in typeDefinitionAnnotations)
            {
                typeDefinition.AddAnnotation(item);
            }

            return typeDefinition;
        }
        #endregion

        #region Operation
        /// <summary>
        /// Parse the <see cref="JsonElement"/> to a list of <see cref="CsdlOperation"/>.
        /// </summary>
        /// <param name="name">The operation name.</param>
        /// <param name="element">The json value to parse from.</param>
        /// <param name="context">The parse context.</param>
        /// <returns></returns>
        internal static CsdlOperation ParseCsdlOperation(string name, JsonElement element, JsonParserContext context)
        {
            // Each action/function overload is an object
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            // The action/funtion overload object MUST contain the member $Kind with a string value of Action/Function.
            string kind = GetKind(element, context);
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
            IList<CsdlOperationParameter> parameters = new List<CsdlOperationParameter>();
            CsdlOperationReturn operationReturn = null;
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                switch (propertyName)
                {
                    case "$Kind":
                        // Processed, skip it
                        break;

                    case "$IsBound":
                        // The value of $IsBound is one of the Boolean literals true or false. Absence of the member means false.
                        isBound = propertyValue.ParseAsBoolean(context);
                        break;

                    case "$EntitySetPath":
                        // The value of $EntitySetPath is a string containing the entity set path.
                        entitySetPath = propertyValue.ParseAsString(context);
                        break;

                    case "$Parameter":
                        // The value of $Parameter is an array. The array contains one object per parameter.
                        if (propertyValue.ValueKind != JsonValueKind.Array)
                        {
                            context.ReportError(EdmErrorCode.UnexpectedValueKind, Strings.CsdlJsonParser_UnexpectedJsonValueKind(element.ValueKind, context.Path, "Array"));
                        }
                        else
                        {
                            parameters = propertyValue.ParseAsArray(context, ParseCsdlParameter);
                        }
                        break;

                    case "$ReturnType":
                        // The value of $ReturnType is an object.
                        operationReturn = ParseCsdlOperationReturn(propertyName, propertyValue, context);
                        break;

                    case "$IsComposable":
                        // The value of $IsComposable is one of the Boolean literals true or false. Absence of the member means false.
                        isComposable = propertyValue.ParseAsBoolean(context);
                        break;

                    default:
                        // it MAY contain annotations.
                        ParseCsdlAnnotation(propertyName, propertyValue, context, operationAnnotations);
                        break;
                }
            });

            CsdlLocation location = context.Location();
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

        internal static CsdlOperationParameter ParseCsdlParameter(JsonElement element, JsonParserContext context)
        {
            // A Parameter is Object
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            CsdlTypeReference parameterType = CsdlJsonParseHelper.ParseCsdlTypeReference(element, context);

            IList<CsdlAnnotation> prameterAnnotations = new List<CsdlAnnotation>();
            string name = null;
            element.ParseAsObject(context, (propertyName, propertyValue) =>
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
                        name = propertyValue.ParseAsString(context);
                        break;

                    default:
                        ParseCsdlAnnotation(propertyName, propertyValue, context, prameterAnnotations);
                        break;
                }

            });

            // Workaround to process the optional parameter
            bool isOptional = false;
            string defaultValue = null;
            // todo: Consider the namespace alias?
            CsdlAnnotation optionalAnnotationValue = prameterAnnotations.Where(a =>
                a.Term == CoreVocabularyModel.OptionalParameterTerm.ShortQualifiedName() ||
                a.Term == CoreVocabularyModel.OptionalParameterTerm.FullName()).FirstOrDefault();
            if (optionalAnnotationValue != null)
            {
                isOptional = true;
                CsdlRecordExpression optionalValueExpression = optionalAnnotationValue.Expression as CsdlRecordExpression;
                if (optionalValueExpression != null)
                {
                    foreach (CsdlPropertyValue property in optionalValueExpression.PropertyValues)
                    {
                        CsdlConstantExpression propertyValue = property.Expression as CsdlConstantExpression;
                        if (propertyValue != null)
                        {
                            if (property.Property == "DefaultValue")
                            {
                                defaultValue = propertyValue.Value;
                            }
                        }
                    }
                }

                prameterAnnotations.Remove(optionalAnnotationValue);
            }

            CsdlOperationParameter csdlParameter = new CsdlOperationParameter(name, parameterType, context.Location(), isOptional, defaultValue);
            prameterAnnotations.ForEach(v => csdlParameter.AddAnnotation(v));

            return csdlParameter;
        }

        /// <summary>
        /// The value of $ReturnType is an object.
        /// </summary>
        /// <param name="name">The operation return name.</param>
        /// <param name="element">The json value to parse from</param>
        /// <param name="context">The parse context.</param>
        /// <returns>The built CSDL operation return.</returns>
        internal static CsdlOperationReturn ParseCsdlOperationReturn(string name, JsonElement element, JsonParserContext context)
        {
            // The value of $ReturnType is an object.
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);
            Debug.Assert(name == "$ReturnType", "The name should be $ReturnType.");

            // It MAY contain the members $Type, $Collection, $Nullable, $MaxLength, $Unicode, $Precision, $Scale, and $SRID.
            CsdlTypeReference typeReference = CsdlJsonParseHelper.ParseCsdlTypeReference(element, context);

            IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            element.ParseAsObject(context, (propertyName, propertyValue) =>
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
                        ParseCsdlAnnotation(propertyName, propertyValue, context, annotations);
                        break;
                }
            });

            CsdlOperationReturn returnType = new CsdlOperationReturn(typeReference, context.Location());
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
        /// <param name="element">The schema member json value.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>Null or the built element.</returns>
        private static CsdlElement TryParseSchemaElement(string name, JsonElement element, JsonParserContext context)
        {
            Debug.Assert(context != null);

            // The schema object member is representing entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
            // The JSON value of each member is a JSON object, if it's not an object, it's not a schema element.
            if (element.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            Debug.Assert(!string.IsNullOrEmpty(name));

            // Each schema member oject should include "$Kind" member, whose value is a string.
            string kind = GetKind(element, context);
            switch (kind)
            {
                case "EntityContainer":
                    return ParseCsdlEntityContainer(name, element, context);

                case "EntityType":
                    return ParseCsdlEntityType(name, element, context);

                case "ComplexType":
                    return ParseCsdlComplexType(name, element, context);

                case "EnumType":
                    return ParseCsdlEnumType(name, element, context);

                case "TypeDefinition":
                    return ParseCsdlTypeDefinition(name, element, context);

                case "Term":
                    return ParseCsdlTermType(name, element, context);

                default:
                    return null;
            }
        }

        internal static IList<CsdlOperation> TryParseCsdlOperationOverload(string name, JsonElement operationArray, JsonParserContext context)
        {
            Debug.Assert(context != null);

            // An action/function is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array.
            // The array contains one object per action/function overload.
            if (operationArray.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            return operationArray.ParseAsArray(context, (v, p) => ParseCsdlOperation(name, v, p));
        }

        internal static void ParseCsdlAnnotation(string name, JsonElement element, JsonParserContext context, IList<CsdlAnnotation> annotationContainer)
        {
            Debug.Assert(name != null);
            Debug.Assert(context != null);
            Debug.Assert(annotationContainer != null);

            if (AnnotationJsonParser.TryParseCsdlAnnotation(name, element, context, out CsdlAnnotation annotation))
            {
                annotationContainer.Add(annotation);
            }
            else
            {
                context.ReportError(EdmErrorCode.UnexpectedElement, Strings.CsdlJsonParser_UnexpectedJsonMember(context.Path, element.ValueKind));
            }
        }

        private static void ReportUnexpectedElement(JsonElement element, JsonParserContext context)
        {
            context.ReportError(EdmErrorCode.UnexpectedElement, Strings.CsdlJsonParser_UnexpectedJsonMember(context.Path, element.ValueKind));
        }

        private static void ValidateKind(JsonElement element, JsonParserContext context, string expectedKind, bool required)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            string kind = GetKind(element, context);

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

            context.ReportError(EdmErrorCode.MissingRequiredProperty, Strings.CsdlJsonParser_MissingKindMember(context.Path, expectedKind));
        }

        /// <summary>
        /// Get Kind property value if existing.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>Null or the kind string.</returns>
        private static string GetKind(JsonElement element, JsonParserContext context)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            string kind = null;
            JsonElement kindValue;
            if (element.TryGetProperty("$Kind", out kindValue))
            {
                context.EnterScope("$Kind");
                kind = kindValue.ParseAsString(context);
                context.LeaveScope();
            }

            return kind;
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
                termName = name;
                return null;
            }
            else
            {
                termName = name.Substring(index); // with @
                return name.Substring(0, index);
            }
        }
    }
}
#endif
