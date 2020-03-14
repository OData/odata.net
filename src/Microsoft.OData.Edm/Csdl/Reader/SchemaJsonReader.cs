//---------------------------------------------------------------------
// <copyright file="CsdlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Reader
{
    internal enum Optionality
    {
        Optional,
        Required
    }

    /// <summary>
    /// Provides CSDL parsing services for EDM models.
    /// </summary>
    internal class SchemaJsonReader
    {

        ///// <summary>
        ///// Indicates where the document comes from.
        ///// </summary>
        //private string source;

        //   private IJsonReader jsonReader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The XmlReader for current CSDL doc</param>
        /// <param name="referencedModelFunc">The function to load referenced model xml. If null, will stop loading the referenced model.</param>
        public SchemaJsonReader(IJsonReader reader, JsonReaderOptions options)
        {
            //      this.jsonReader = reader;
            //       this.errors = new List<EdmError>();
            //     this.edmReferences = new List<IEdmReference>();

            //// Setup the edmx parser.
            //this.schemaPropertyParserLookup = new Dictionary<string, Action>
            //{
            //    // $Alias
            //    { CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Alias, this.ParseAlias },

            //    // $Annotations
            //    { CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Annotations, this.ParseAnnotations }
            //};
        }

        public static CsdlSchema BuildCsdlSchema(string schemaNamespace, Version version, IJsonValue jsonValue)
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            // Its value is an object.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }
            JsonObjectValue schemaObject = (JsonObjectValue)jsonValue;

            IList<CsdlAnnotations> outOfLineAnnotations = new List<CsdlAnnotations>();
            IList<CsdlElement> csdlElements = new List<CsdlElement>();
            string alias = null;
            foreach (var property in schemaObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Alias":
                        // The value of $Alias is a string containing the alias for the schema.
                        alias = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$Annotations":
                        // The value of $Annotations is an object with one member per annotation target. 
                        outOfLineAnnotations = BuildOutOfLineAnnotations(propertyValue);
                        break;

                    default:
                        CsdlElement element = BuildSchemaElement(propertyValue);
                        if (element != null)
                        {
                            csdlElements.Add(element);
                        }

                        break;
                }
            }

            CsdlLocation location = new CsdlLocation(-1, -1);
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

        public static IList<CsdlAnnotations> BuildOutOfLineAnnotations(IJsonValue jsonValue)
        {
            return null;
        }

        public static CsdlElement BuildSchemaElement(IJsonValue jsonValue)
        {
            return null;
        }

        /// <summary>
        /// An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public static CsdlEntityType BuildCsdlEntityType(string name, JsonObjectValue entityObject)
        {
            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.
            bool isOpen = false;
            bool isAbstract = false;
            string baseType = null;
            IList<CsdlProperty> properties = new List<CsdlProperty>();
            IList<CsdlNavigationProperty> navProperties = new List<CsdlNavigationProperty>();
            IList<CsdlAnnotation> complexAnnotations = new List<CsdlAnnotation>();
            IDictionary<string, IList<CsdlAnnotation>> propertyAnnotations = new Dictionary<string, IList<CsdlAnnotation>>();
            string kind = null;
            bool hasStream = false;
            CsdlKey csdlKey = null;
            foreach (var property in entityObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    // $Kind
                    case "$Kind":
                        kind = ParseAsStringPrimitive(propertyValue);
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    // $BaseType
                    case "$BaseType":
                        baseType = ParseAsStringPrimitive(propertyValue);
                        break;

                    // $Abstract
                    case "$Abstract":
                        isAbstract = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    // $OpenType
                    case "$OpenType":
                        isOpen = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    // $HasStream
                    case "$HasStream":
                        isOpen = ParseAsBooleanPrimitive(propertyValue);
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
                                string kindString = ParseAsStringPrimitive(kindValue);
                                if (kindString == CsdlConstants.Element_NavigationProperty)
                                {
                                    CsdlNavigationProperty navProperty = BuildCsdlNavigationProperty(propertyName, objValue);
                                    navProperties.Add(navProperty);
                                    break;
                                }
                            }

                            // The property object MAY contain the member $Kind with a string value of Property.
                            // This member SHOULD be omitted to reduce document size.
                            CsdlProperty structuralProperty = BuildCsdlProperty(propertyName, objValue);
                            properties.Add(structuralProperty);
                        }
                        else
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);

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

            CsdlLocation location = new CsdlLocation(-1, -1);

            // CsdlComplexType(string name, string baseTypeName, bool isAbstract, bool isOpen, IEnumerable<CsdlProperty> structuralProperties, IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlLocation location)
            CsdlEntityType entityType = new CsdlEntityType(name, baseType, isAbstract, isOpen, hasStream, csdlKey,
                properties, navProperties, location);
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
        public static CsdlComplexType BuildCsdlComplexType(string name, JsonObjectValue complexObject)
        {
            // The complex type object MUST contain the member $Kind with a string value of ComplexType.
            // It MAY contain the members $BaseType, $Abstract, and $OpenType.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.

            bool isOpen = false;
            bool isAbstract = false;
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
                    // $Kind
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Kind:
                        string kind = ParseAsStringPrimitive(propertyValue);
                        if (kind != CsdlConstants.Element_ComplexType)
                        {
                            throw new Exception();
                        }
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    // $BaseType
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_BaseType:
                        baseType = ParseAsStringPrimitive(propertyValue);
                        break;

                    // $Abstract
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Abstract:
                        isAbstract = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    // $OpenType
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_OpenType:
                        isOpen = ParseAsBooleanPrimitive(propertyValue);
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
                                string kindString = ParseAsStringPrimitive(kindValue);
                                if (kindString == CsdlConstants.Element_NavigationProperty)
                                {
                                    CsdlNavigationProperty navProperty = BuildCsdlNavigationProperty(propertyName, objValue);
                                    navProperties.Add(navProperty);
                                    break;
                                }
                            }

                            // The property object MAY contain the member $Kind with a string value of Property.
                            // This member SHOULD be omitted to reduce document size.
                            CsdlProperty structuralProperty = BuildCsdlProperty(propertyName, objValue);
                            properties.Add(structuralProperty);
                        }
                        else
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);

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
            CsdlComplexType complexType = new CsdlComplexType(name, baseType, isAbstract, isOpen,
                properties, navProperties, location);
            foreach (var annotation in complexAnnotations)
            {
                complexType.AddAnnotation(annotation);
            }

            return complexType;
        }

        public static CsdlProperty BuildCsdlProperty(string name, JsonObjectValue propertyObject)
        {
            // Structural properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
            // It MAY contain the member $Type, $Collection, $Nullable, $MaxLength, $Unicode, $Precision, $Scale, $SRID, and $DefaultValue.
            string typeName = "Edm.String"; // Absence of the $Type member means the type is Edm.String.
            bool isCollection = false;
            bool nullable = false; //  Absence of the member means false.
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
                        kind = ParseAsStringPrimitive(propertyValue);
                        if (kind != "Property")
                        {
                            throw new Exception();
                        }
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        isCollection = ParseAsBooleanPrimitive(propertyValue);
                        // TODO: should verify this value must be true?
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        maxLength = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        precision = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        scale = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        unicode = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$SRID":
                        // The value of $SRID is a string containing a number or the symbolic value variable.
                        // So far, ODL doesn't support string of SRID.
                        srid = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        // So far, ODL only suppports the string default value.
                        defaultValue = ParseAsStringPrimitive(propertyValue);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null && propertyName == null)
                        {
                            // annotation for the property
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);
                            propertyAnnotations.Add(annotation);
                            break;
                        }

                        // Without '@' in the name, it's not allowed in Navigation Property object
                        throw new Exception();
                }
            }

            CsdlLocation location = new CsdlLocation(-1, -1);
            CsdlTypeReference typeReference = ParseTypeReference(typeName, isCollection, false, nullable, maxLength, unicode, precision, scale, srid, location);

            CsdlProperty csdlProperty = new CsdlProperty(name, typeReference, defaultValue, location);
            foreach (var item in propertyAnnotations)
            {
                csdlProperty.AddAnnotation(item);
            }
            return csdlProperty;
        }

        public static CsdlNavigationProperty BuildCsdlNavigationProperty(string name, JsonObjectValue navigtionObject)
        {
            // Navigation properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            // The navigation property object MUST contain the member $Kind with a string value of NavigationProperty.
            // It MUST contain the member $Type, and it MAY contain the members $Collection, $Nullable, $Partner, $ContainsTarget, $ReferentialConstraint, and $OnDelete.
            string kind = null;
            string typeName = null;
            bool isCollection = false;
            bool nullable = false;
            string parter = null;
            bool containsTarget = false; // Absence of the member means false.
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
                        kind = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        isCollection = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$Partner":
                        // The value of $Partner is a string containing the path to the partner navigation property.
                        parter = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$ContainsTarget":
                        // The value of $ContainsTarget is one of the Boolean literals true or false. Absence of the member means false.
                        containsTarget = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$OnDelete":
                        // The value of $OnDelete is a string with one of the values Cascade, None, SetNull, or SetDefault.
                        csdlOnDelete = BuildCsdlOnDelete(propertyValue, location);
                        break;

                    case "$ReferentialConstraint":
                        referentialConstraints = BuildCsdlReferentialConstraint(propertyValue);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null)
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);
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

            if (isCollection)
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

            CsdlNavigationProperty csdlNavProperty = new CsdlNavigationProperty(name, typeName, nullable, parter, containsTarget, csdlOnDelete, referentialConstraints, location);
            foreach (var item in navPropertyAnnotations)
            {
                csdlNavProperty.AddAnnotation(item);
            }
            return csdlNavProperty;
        }

        public static IList<CsdlReferentialConstraint> BuildCsdlReferentialConstraint(IJsonValue value)
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
                    CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);

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
                    string referedProperty = ParseAsStringPrimitive(propertyValue);

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

        public static CsdlOnDelete BuildCsdlOnDelete(IJsonValue value, CsdlLocation parentLocation)
        {
            string onDelete = ParseAsStringPrimitive(value);

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

            return new CsdlOnDelete(edmOnDelete, parentLocation);
        }

        public static CsdlTypeDefinition BuildCsdlTyeDefinition(string name, JsonObjectValue typeDefinitionObject)
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
                        kind = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$UnderlyingType":
                        // The value of $UnderlyingType is the qualified name of the underlying type.
                        underlygingType = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        maxLength = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        precision = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        scale = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        unicode = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null && propertyName == null)
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);
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

        public static CsdlOperation BuildCsdlOperation(string name, IJsonValue jsonValue)
        {
            // The action/funtion overload object MUST contain the member $Kind with a string value of Action/Function.
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return null;
            }

            JsonObjectValue operationObject = (JsonObjectValue)jsonValue;

            bool isBound = false; // absence of the member means false
            bool isComposable = false; // Absence of the member means false.
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
                        kind = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$IsBound":
                        // The value of $IsBound is one of the Boolean literals true or false. Absence of the member means false.
                        isBound = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$EntitySetPath":
                        // The value of $EntitySetPath is a string containing the entity set path.
                        entitySetPath = ParseAsStringPrimitive(propertyValue);
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
                        entitySetPath = ParseAsStringPrimitive(propertyValue);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName != null && propertyName == null)
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);
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

            bool isAction = kind == "Action";

            CsdlLocation location = new CsdlLocation(-1, -1);
            CsdlOperation operation;
            if (kind == "Action")
            {
                // It's action
                operation = new CsdlAction(name, parameters, operationReturn, isBound, entitySetPath, location);
            }
            else if (kind == "Function")
            {
                // It's function
                operation = new CsdlFunction(name, parameters, operationReturn, isBound, entitySetPath, isComposable, location);
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

        public static IList<CsdlOperation> BuildCsdlOperationOverload(string name, IJsonValue jsonValue)
        {
            // An action/function is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array.
            // The array contains one object per action/function overload.
            if (jsonValue.ValueKind != JsonValueKind.JArray)
            {
                return null;
            }

            JsonArrayValue operationArray = (JsonArrayValue)jsonValue;
            CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlOperation> operations = new List<CsdlOperation>();
            foreach (var item in operationArray)
            {
                CsdlOperation operation = BuildCsdlOperation(name, item);
                operations.Add(operation);
            }

            return operations;
        }

        public static CsdlTerm BuildCsdlTermType(string name, JsonObjectValue termObject)
        {
            // Term Object
            // A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // The term object MUST contain the member $Kind with a string value of Term.
            // It MAY contain the members $Type, $Collection, $AppliesTo, $Nullable, $MaxLength, $Precision, $Scale, $SRID, and $DefaultValue, as well as $Unicode for 4.01 and greater payloads.
            // It MAY contain annotations.
            CsdlLocation location = new CsdlLocation(-1, -1);
            IList<CsdlAnnotation> termAnnotations = new List<CsdlAnnotation>();

            string typeName = "Edm.String"; // Absence of the $Type member means the type is Edm.String.
            bool isCollection = false;
            bool nullable = false; //  Absence of the member means false.
            int? maxLength = null;
            int? precision = null;
            int? scale = null;
            bool? unicode = null; // Absence of the member means true.
            int? srid = null;
            string defaultValue = null;
            string kind;
            string appliesTo = null;
            foreach (var property in termObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
                        kind = ParseAsStringPrimitive(propertyValue);
                        if (kind != "Property")
                        {
                            throw new Exception();
                        }
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = ParseAsStringPrimitive(propertyValue);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        isCollection = ParseAsBooleanPrimitive(propertyValue);
                        // TODO: should verify this value must be true?
                        break;

                    case "$AppliesTo":
                        // The value of $AppliesTo is an array whose items are strings containing symbolic values from the table above that identify model elements the term is intended to be applied to.
                        isCollection = ParseAsBooleanPrimitive(propertyValue);
                        // TODO: should verify this value must be true?
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        maxLength = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        precision = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        scale = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        unicode = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    case "$SRID":
                        // The value of $SRID is a string containing a number or the symbolic value variable.
                        // So far, ODL doesn't support string of SRID.
                        srid = ParseAsIntegerPrimitive(propertyValue);
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        // So far, ODL only suppports the string default value.
                        defaultValue = ParseAsStringPrimitive(propertyValue);
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
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);
                            termAnnotations.Add(annotation);
                            break;
                        }

                        // Without '@' in the name, it's not allowed in Navigation Property object
                        throw new Exception();
                }
            }

            CsdlTypeReference typeReference = ParseTypeReference(typeName, isCollection, false, nullable, maxLength, unicode, precision, scale, srid, location);
            // CsdlTerm(string name, CsdlTypeReference type, string appliesTo, string defaultValue, CsdlLocation location)
            CsdlTerm termType = new CsdlTerm(name, typeReference, appliesTo, defaultValue, location);
            foreach (var annotation in termAnnotations)
            {
                termType.AddAnnotation(annotation);
            }

            return termType;
        }

        public static CsdlEnumType BuildCsdlEnumType(string name, JsonObjectValue enumObject)
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
            bool isFlags = false;
            string underlyingTypeName = null;

            foreach (var property in enumObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    // $Kind
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Kind:
                        string kind = ParseAsStringPrimitive(propertyValue);
                        if (kind != CsdlConstants.Element_EnumType)
                        {
                            throw new Exception();
                        }
                        // we can skip this verification, because it's verified at upper layer
                        break;

                    // $UnderlyingType
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_UnderlyingType:
                        underlyingTypeName = ParseAsStringPrimitive(propertyValue);
                        break;

                   // $IsFlags
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_IsFlags:
                        isFlags = ParseAsBooleanPrimitive(propertyValue);
                        break;

                    default:
                        string termName;
                        propertyName = SeperateAnnotationName(propertyName, out termName);
                        if (termName == null)
                        {
                            // Without '@' in the name, so it's normal enum member name
                            CsdlEnumMember enumMember = BuildCsdlEnumMember(propertyName, propertyValue);
                            members.Add(enumMember);
                        }
                        else
                        {
                            CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);

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

            CsdlEnumType enumType = new CsdlEnumType(name, underlyingTypeName, isFlags, members, location);
            foreach (var annotation in enumAnnotations)
            {
                enumType.AddAnnotation(annotation);
            }

            return enumType;
        }

        public static CsdlAnnotation BuildCsdlAnnotation(string termName, IJsonValue annotationValue)
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
            CsdlExpressionBase expression = BuildExpression(annotationValue);

            return new CsdlAnnotation(termName, qualifier, expression, location);
        }

        public static CsdlExpressionBase BuildExpression(IJsonValue annotationValue)
        {
            return null;
        }

        public static CsdlEnumMember BuildCsdlEnumMember(string name, IJsonValue enumMemberObject)
        {
            // Enumeration Member Object
            // Enumeration type members are represented as JSON object members, where the object member name is the enumeration member name and the object member value is the enumeration member value.
            // For members of flags enumeration types a combined enumeration member value is equivalent to the bitwise OR of the discrete values.
            // Annotations for enumeration members are prefixed with the enumeration member name.

            CsdlLocation location = new CsdlLocation(-1, -1);
            long? value = ParseAsLongPrimitive(enumMemberObject);


            return new CsdlEnumMember(name, value, location);
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

        private static long? ParseAsLongPrimitive(IJsonValue jsonValue)
        {
            if (jsonValue.ValueKind != JsonValueKind.JPrimitive)
            {
                throw new Exception();
            }

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return null;
            }

            if (primitiveValue.Value.GetType() == typeof(int))
            {
                return (int)primitiveValue.Value;
            }

            return (long)primitiveValue.Value;
        }

        private static int? ParseAsIntegerPrimitive(IJsonValue jsonValue)
        {
            if (jsonValue.ValueKind != JsonValueKind.JPrimitive)
            {
                throw new Exception();
            }

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return null;
            }

            if (primitiveValue.Value.GetType() == typeof(int))
            {
                return (int)primitiveValue.Value;
            }

            throw new Exception();
        }

        private static string ParseAsStringPrimitive(IJsonValue jsonValue)
        { 
            if (jsonValue.ValueKind != JsonValueKind.JPrimitive)
            {
                throw new Exception();
            }

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            return primitiveValue.Value as string;
        }

        private static bool ParseAsBooleanPrimitive(IJsonValue jsonValue)
        {
            if (jsonValue.ValueKind != JsonValueKind.JPrimitive)
            {
                throw new Exception();
            }

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            return (bool)primitiveValue.Value;
        }


        public static CsdlSchema ParseSchemObject(IJsonReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw new ArgumentNullException("jsonReader");
            }

            // Supports to read from Begin
            if (jsonReader.NodeType == JsonNodeType.None)
            {
                jsonReader.Read();
            }

            // Make sure the input is an object
            if (jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new Exception("");
            }

            // Pass the "{" tag.
            jsonReader.Read();

            while (jsonReader.NodeType != JsonNodeType.EndObject)
            {
                // Get the property name and move json reader to next token
                string propertyName = jsonReader.ReadPropertyName();

                // Now the Json reader point to the value.
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new Exception();
                }

                switch (propertyName)
                {
                    // "$Alias"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Alias:
                        ParseAlias(jsonReader);
                        break;

                    // "$Annotations"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Annotations:
                        ParseAnnotations(jsonReader);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            // Annotation for the schema
                            ParseSchemaAnnotations(jsonReader);
                        }
                        else
                        {
                            // Schema members (entity type, complex type...)
                            ParseSchemaElements(propertyName, jsonReader);
                        }
                        break;
                }
            }

            // Consume the "}" tag.
            jsonReader.Read();

            return null;
        }

        private static void ParseAlias(IJsonReader jsonReader)
        {
            if (jsonReader.NodeType != JsonNodeType.PrimitiveValue)
            {
                throw new Exception();
            }

            string version = jsonReader.ReadStringValue();
            if (version != "4.0" && version != "4.01")
            {
                throw new Exception();
            }
        }

        private static void ParseAnnotations(IJsonReader jsonReader)
        {
            if (jsonReader.NodeType != JsonNodeType.PrimitiveValue)
            {
                throw new Exception();
            }
        }

        // The schema object MAY also contain annotations that apply to the schema itself.
        private static void ParseSchemaAnnotations(IJsonReader jsonReader)
        {
            // parse the "@Measures.ISOCurrency": {
            //            "$Path": "Currency"

        }

        private static void ParseSchemaElements(string name, IJsonReader jsonReader)
        {
            // The schema object MAY contain members representing entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
            // 1) An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
            // 2) A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
            // 3) An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type and whose value is an object.
            // 4) A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition and whose value is an object.
            // 5) An action is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array. The array contains one object per action overload.
            // 6) A function is represented as a member of the schema object whose name is the unqualified name of the function and whose value is an array.
            // 7) A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // 8) An entity container is represented as a member of the schema object whose name is the unqualified name of the entity container and whose value is an object.

            // Make sure the input is an object
            if (jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new Exception("");
            }

            // Because we don't know the order or each property presented in the JSON schema object
            // For example $Kind maybe the last property in the element.


            // return null;
        }

#if false
        private static void ParseSchemaElements(string name, IJsonReader jsonReader)
        {
            // The schema object MAY contain members representing entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
            // 1) An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
            // 2) A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
            // 3) An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type and whose value is an object.
            // 4) A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition and whose value is an object.
            // 5) An action is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array. The array contains one object per action overload.
            // 6) A function is represented as a member of the schema object whose name is the unqualified name of the function and whose value is an array.
            // 7) A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // 8) An entity container is represented as a member of the schema object whose name is the unqualified name of the entity container and whose value is an object.

            // Make sure the input is an object
            if (jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new Exception("");
            }

            // Pass the "{" tag.
            jsonReader.Read();

            while (jsonReader.NodeType != JsonNodeType.EndObject)
            {
                // Get the property name and move json reader to next token
                string propertyName = jsonReader.ReadPropertyName();

                // Now the Json reader point to the value.
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new Exception();
                }

                switch (propertyName)
                {
                    // "$Alias"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Alias:
                        ParseAlias(jsonReader);
                        break;

                    // "$Annotations"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Annotations:
                        ParseAnnotations(jsonReader);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            // Annotation for the schema
                            ParseSchemaAnnotations(jsonReader);
                        }
                        else
                        {
                            // Schema members (entity type, complex type...)
                            ParseSchemaElements(propertyName, jsonReader);
                        }
                        break;
                }
            }
            // Consume the "}" tag.
            jsonReader.Read();

           // return null;
        }

#endif

        private static CsdlTypeReference ParseTypeReference(string typeString, // if it's collection, it's element type string
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
    }
}
