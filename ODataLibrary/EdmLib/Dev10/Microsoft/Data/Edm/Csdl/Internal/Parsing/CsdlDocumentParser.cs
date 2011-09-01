//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Common;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing
{
    /// <summary>
    /// CSDL document parser.
    /// </summary>
    internal class CsdlDocumentParser : EdmXmlDocumentParser<CsdlSchema>
    {
        private Version artifactVersion;
                
        internal CsdlDocumentParser(string documentPath, XmlReader reader)
            : base(documentPath, reader)
        {
        }

        internal override IEnumerable<KeyValuePair<Version, string>> SupportedVersions
        {
            get { return CsdlConstants.SupportedVersions.SelectMany(kvp => kvp.Value.Select(ns => new KeyValuePair<Version, string>(kvp.Key, ns))); }
        }

        protected override bool TryGetDocumentElementParser(Version csdlArtifactVersion, XmlElementInfo rootElement, out XmlElementParser<CsdlSchema> parser)
        {
            EdmUtil.CheckArgumentNull(rootElement, "rootElement");
            this.artifactVersion = csdlArtifactVersion;
            if (string.Equals(rootElement.Name, CsdlConstants.Element_Schema, StringComparison.Ordinal))
            {
                parser = this.CreateRootElementParser();
                return true;
            }

            parser = null;
            return false;
        }

        protected override void AnnotateItem(object result, XmlElementValueCollection childValues)
        {
            CsdlElement annotatedItem = result as CsdlElement;
            if (annotatedItem == null)
            {
                return;
            }

            foreach (var xmlAnnotation in this.currentElement.Annotations)
            {
                annotatedItem.AddAnnotation(new CsdlImmediateValueAnnotation(xmlAnnotation.NamespaceName, xmlAnnotation.Name, xmlAnnotation.Value));
            }

            foreach (var annotation in childValues.ValuesOfType<CsdlVocabularyAnnotationBase>())
            {
                annotatedItem.AddAnnotation(annotation);
            }
        }

        private XmlElementParser<CsdlSchema> CreateRootElementParser()
        {
            var documentationParser =
                // <Documentation>
                CsdlElement<CsdlDocumentation>(CsdlConstants.Element_Documentation, this.OnDocumentationElement,
                   // <Summary/>
                   Element(CsdlConstants.Element_Summary, (element, children) => children.FirstText.Value),
                   // <LongDescription/>
                   Element(CsdlConstants.Element_LongDescription, (element, children) => children.FirstText.TextValue)
                // </Documentation>
                );

            // There is recursion in the grammar between RowType, CollectionType, ReturnType, and Property within RowType.
            // This requires breaking up the parser construction into pieces and then weaving them together with AddChildParser.
            var referenceTypeParser =
                // <ReferenceType/>
                CsdlElement<CsdlTypeReference>(CsdlConstants.Element_ReferenceType, this.OnEntityReferenceTypeElement, documentationParser);

            var rowTypeParser =
                // <RowType/>
                CsdlElement<CsdlTypeReference>(CsdlConstants.Element_RowType, this.OnRowTypeElement);

            var collectionTypeParser =
                // <CollectionType>
                CsdlElement<CsdlTypeReference>(CsdlConstants.Element_CollectionType, this.OnCollectionTypeElement, documentationParser,
                    // <TypeRef/>
                    CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser),
                    // <RowType/>
                    rowTypeParser,
                    // <ReferenceType/>
                    referenceTypeParser
                // </CollectionType>
                );

            var rowTypePropertyElementParser =
                // <Property>
                CsdlElement<CsdlProperty>(CsdlConstants.Element_Property, this.OnRowTypePropertyElement, documentationParser,
                    // <RowType/>
                    rowTypeParser,
                    // <CollectionType/>
                    collectionTypeParser,
                    // <ReferenceType/>
                    referenceTypeParser
                    // </Property>
                );

            var stringConstantExpressionParser =
                // <String/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_String, this.OnStringConstantExpression);

            var intConstantExpressionParser =
                // <Int/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Int, this.OnIntConstantExpression);

            var floatConstantExpressionParser =
                // <Float/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Float, this.OnFloatConstantExpression);

            var decimalConstantExpressionParser =
                // <Decimal/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Decimal, this.OnDecimalConstantExpression);

            var boolConstantExpressionParser =
                // <Bool/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Bool, this.OnBoolConstantExpression);

            var dateTimeConstantExpressionParser =
                // <DateTime/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_DateTime, this.OnDateTimeConstantExpression);

            var pathExpressionParser =
                // <Path/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Path, this.OnPathExpression);

            var propertyValueParser =
                // <PropertyValue>
                CsdlElement<CsdlPropertyValue>(CsdlConstants.Element_PropertyValue, this.OnPropertyValueElement,

                    // <String/>
                    stringConstantExpressionParser,

                    // <Int/>
                    intConstantExpressionParser,

                    // <Float/>
                    floatConstantExpressionParser,

                    // <Decimal/>
                    decimalConstantExpressionParser,

                    // <Bool/>
                    boolConstantExpressionParser,

                    // <DateTime/>
                    dateTimeConstantExpressionParser,

                    // <Path/>
                    pathExpressionParser

                // </PropertyValue>
                );

            var recordExpressionParser =
                // <Record>
                CsdlElement<CsdlRecordExpression>(CsdlConstants.Element_Record, this.OnRecordElement,

                    // <PropertyValue />
                    propertyValueParser

                // </Record>
                );

            propertyValueParser.AddChildParser(recordExpressionParser);

            var valueAnnotationParser =
                // <ValueAnnotation>
                CsdlElement<CsdlValueAnnotation>(CsdlConstants.Element_ValueAnnotation, this.OnValueAnnotationElement,

                    // <String/>
                    stringConstantExpressionParser,

                    // <Int/>
                    intConstantExpressionParser,

                    // <Float/>
                    floatConstantExpressionParser,

                    // <Decimal/>
                    decimalConstantExpressionParser,

                    // <Bool/>
                    boolConstantExpressionParser,

                    // <DateTime/>
                    dateTimeConstantExpressionParser,

                    // <Path/>
                    pathExpressionParser,

                    // <Record />
                    recordExpressionParser

                // </ValueAnnotation>
                );

            var typeAnnotationParser =
                // <TypeAnnotation>
                CsdlElement<CsdlTypeAnnotation>(CsdlConstants.Element_TypeAnnotation, this.OnTypeAnnotationElement,

                    // <PropertyValue />
                    propertyValueParser
                    
                // </TypeAnnotation>
                );

            rowTypeParser.AddChildParser(rowTypePropertyElementParser);
            collectionTypeParser.AddChildParser(collectionTypeParser);

            var rootElementParser =

            // <Schema>
            CsdlElement<CsdlSchema>(CsdlConstants.Element_Schema, this.OnSchemaElement,
                documentationParser,

                // <Using/>
                CsdlElement<CsdlUsing>(CsdlConstants.Element_Using, this.OnUsingElement),

                // <ComplexType>
                CsdlElement<CsdlComplexType>(CsdlConstants.Element_ComplexType, this.OnComplexTypeElement,
                    documentationParser,
                    // <Property>
                    CsdlElement<CsdlProperty>(CsdlConstants.Element_Property, this.OnPropertyElement, 
                        documentationParser,
                        // <PropertyRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser)
                    // </Property>
                    )
                // </ComplexType>
                ),

                // <EntityType>
                CsdlElement<CsdlEntityType>(CsdlConstants.Element_EntityType, this.OnEntityTypeElement,
                    documentationParser,
                    // <Key>
                    CsdlElement<CsdlKey>(CsdlConstants.Element_Key, OnEntityKeyElement,
                        // <PropertyRef/>
                        CsdlElement<CsdlPropertyReference>(CsdlConstants.Element_PropertyRef, this.OnPropertyRefElement)
                    // </Key>
                    ),
                    // <Property>
                    CsdlElement<CsdlProperty>(CsdlConstants.Element_Property, this.OnPropertyElement, 
                        documentationParser,
                        // <PropertyRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser)
                    // </Property>
                    ),
                    // <NavigationProperty/>
                    CsdlElement<CsdlNavigationProperty>(CsdlConstants.Element_NavigationProperty, this.OnNavigationPropertyElement, documentationParser),
                    // <ValueAnnotation/>
                    valueAnnotationParser,
                    // <TypeAnnotation/>
                    typeAnnotationParser
                // </EntityType>
                ),

                // <Association>
                CsdlElement<CsdlAssociation>(CsdlConstants.Element_Association, this.OnAssociationElement,
                    documentationParser,
                    // <End>
                    CsdlElement<CsdlAssociationEnd>(CsdlConstants.Element_End, this.OnAssociationEndElement,
                        documentationParser,
                        // <OnDelete/>
                        CsdlElement<CsdlOnDelete>(CsdlConstants.Element_OnDelete, this.OnDeleteActionElement, documentationParser)
                    // </End>
                    ),
                    // <ReferentialConstraint>
                    CsdlElement<CsdlReferentialConstraint>(CsdlConstants.Element_ReferentialConstraint, this.OnReferentialConstraintElement,
                        documentationParser,

                        // <Principal>
                        CsdlElement<CsdlReferentialConstraintRole>(CsdlConstants.Element_Principal, this.OnReferentialConstraintRoleElement,
                            documentationParser,

                            // <PropertyRef/>
                            CsdlElement<CsdlPropertyReference>(CsdlConstants.Element_PropertyRef, this.OnPropertyRefElement)
                        // </Principal>
                        ),
                        // <Dependent>
                        CsdlElement<CsdlReferentialConstraintRole>(CsdlConstants.Element_Dependent, this.OnReferentialConstraintRoleElement,
                            documentationParser,

                            // <PropertyRef/>
                            CsdlElement<CsdlPropertyReference>(CsdlConstants.Element_PropertyRef, this.OnPropertyRefElement)
                        // </Dependent>
                        )
                    // </ReferentialConstraint>
                    )
                // </Association>
                ),

                // <EnumType>
                CsdlElement<CsdlEnumType>(CsdlConstants.Element_EnumType, this.OnEnumTypeElement,
                    documentationParser,
                // <Member>
                    CsdlElement<CsdlEnumMember>(CsdlConstants.Element_Member, this.OnEnumMemberElement, documentationParser)
                // </EnumType>
                ),

                // Function
                CsdlElement<CsdlFunction>(CsdlConstants.Element_Function, this.OnFunctionElement,
                    documentationParser,

                    // <Parameter>
                    CsdlElement<CsdlFunctionParameter>(CsdlConstants.Element_Parameter, this.OnParameterElement,
                        documentationParser,
                        // <ReferenceType/>
                        referenceTypeParser,
                        // <RowType/>
                        rowTypeParser,
                        // <CollectionType/>
                        collectionTypeParser
                     // </Parameter>
                    ),

                    // <DefiningExpression/>
                    Element(CsdlConstants.Element_DefiningExpression, (element, children) => children.FirstText.Value),

                    // <ReturnType>
                    CsdlElement<CsdlFunctionReturnType>(CsdlConstants.Element_ReturnType, this.OnReturnTypeElement,
                        documentationParser,
                        // <ReferenceType/>
                        referenceTypeParser,
                        // <RowType/>
                        rowTypeParser,
                        // <CollectionType/>
                        collectionTypeParser
                    // </ReturnType>
                    )
                // </Function>
                ),

                // <ValueTerm/>
                CsdlElement<CsdlValueTerm>(CsdlConstants.Element_ValueTerm, this.OnValueTermElement),

                // <Annotations>
                CsdlElement<CsdlAnnotations>(CsdlConstants.Element_Annotations, this.OnAnnotationsElement,
                
                    // <ValueAnnotation/>
                    valueAnnotationParser,

                    // <TypeAnnotation>
                    typeAnnotationParser
                // </Annotations>
                ),

                // <EntityContainer>
                CsdlElement<CsdlEntityContainer>(CsdlConstants.Element_EntityContainer, this.OnEntityContainerElement,
                    documentationParser,

                    // <EntitySet/>
                    CsdlElement<CsdlEntitySet>(CsdlConstants.Element_EntitySet, this.OnEntitySetElement, documentationParser),

                    // <AssociationSet>
                    CsdlElement<CsdlAssociationSet>(CsdlConstants.Element_AssociationSet, this.OnAssociationSetElement,
                        documentationParser,

                        // <End/>
                        CsdlElement<CsdlAssociationSetEnd>(CsdlConstants.Element_End, this.OnAssociationSetEndElement, documentationParser)

                    // </AssociationSet>
                    ),

                    // <Function Import
                    CsdlElement<CsdlFunctionImport>(CsdlConstants.Element_FunctionImport, this.OnFunctionImportElement,
                        documentationParser,

                        // <Parameter>
                        CsdlElement<CsdlFunctionParameter>(CsdlConstants.Element_Parameter, this.OnFunctionImportParameterElement,
                            documentationParser
                        // </Parameter>
                        )
                    //</FunctionImport
                    )

                //</EntityContainer>
                )

            // </Schema>
            );

            return rootElementParser;
        }

        private static CsdlDocumentation Documentation(XmlElementValueCollection childValues)
        {
            return childValues.ValuesOfType<CsdlDocumentation>().FirstOrDefault();
        }

        private CsdlSchema OnSchemaElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string namespaceName = Required(CsdlConstants.Attribute_Namespace);
            string namespaceUri = Optional(CsdlConstants.Attribute_NamespaceUri);
            string alias = Optional(CsdlConstants.Attribute_Alias);

            CsdlSchema result =
                new CsdlSchema(
                    namespaceName,
                    namespaceUri,
                    alias,
                    this.artifactVersion,
                    childValues.ValuesOfType<CsdlUsing>(),
                    childValues.ValuesOfType<CsdlAssociation>(),
                    childValues.ValuesOfType<CsdlStructuredType>(),
                    childValues.ValuesOfType<CsdlEnumType>(),
                    childValues.ValuesOfType<CsdlFunction>(),
                    childValues.ValuesOfType<CsdlValueTerm>(),
                    childValues.ValuesOfType<CsdlEntityContainer>(),
                    childValues.ValuesOfType<CsdlAnnotations>(),
                    Documentation(childValues), 
                    element.Location);

            return result;
        }

        private CsdlDocumentation OnDocumentationElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return new CsdlDocumentation(childValues[CsdlConstants.Element_Summary].TextValue, childValues[CsdlConstants.Element_LongDescription].TextValue);
        }

        private CsdlUsing OnUsingElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string namespaceName = Optional(CsdlConstants.Attribute_Namespace);
            string namespaceUri = null;
            if (namespaceName == null)
            {
                namespaceUri = Required(CsdlConstants.Attribute_NamespaceUri);
            }

            string alias = Optional(CsdlConstants.Attribute_Alias);

            return new CsdlUsing(namespaceName, namespaceUri, alias, Documentation(childValues), element.Location);
        }

        private CsdlComplexType OnComplexTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string baseType = Optional(CsdlConstants.Attribute_BaseType);
            bool isAbstract = OptionalBoolean(CsdlConstants.Attribute_Abstract) ?? CsdlConstants.Default_Abstract;

            return new CsdlComplexType(name, baseType, isAbstract, childValues.ValuesOfType<CsdlProperty>(), Documentation(childValues), element.Location);
        }

        private CsdlEntityType OnEntityTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string baseType = Optional(CsdlConstants.Attribute_BaseType);
            bool isOpen = OptionalBoolean(CsdlConstants.Attribute_OpenType) ?? CsdlConstants.Default_OpenType;
            bool isAbstract = OptionalBoolean(CsdlConstants.Attribute_Abstract) ?? CsdlConstants.Default_Abstract;

            // TODO: Give a syntax error if there is more than one key.
            CsdlKey key = childValues.ValuesOfType<CsdlKey>().FirstOrDefault();

            return new CsdlEntityType(name, baseType, isAbstract, isOpen, key, childValues.ValuesOfType<CsdlProperty>(), childValues.ValuesOfType<CsdlNavigationProperty>(), Documentation(childValues), element.Location);
        }
                
        private CsdlProperty OnPropertyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = Required(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location);
            string name = Required(CsdlConstants.Attribute_Name);
            string defaultValue = Optional(CsdlConstants.Attribute_DefaultValue);
            EdmConcurrencyMode? concurrencyMode = OptionalConcurrencyMode(CsdlConstants.Attribute_ConcurrencyMode);
            bool isFixedConcurrency = (concurrencyMode ?? EdmConcurrencyMode.None) == EdmConcurrencyMode.Fixed;

            return new CsdlProperty(name, type, isFixedConcurrency, defaultValue, Documentation(childValues), element.Location);

#if false // TODO: why is this not used? (CollectionKind should be removed with extreme prejudice. Default value should be reconstituted when we have proper Edm types. Concurrency mode should probably be reenabled.)
            SetName(property);
            property.PropertyType = ParsePropertyTypeReference(property);

            // Concurrency Mode
            Optional(CsdlConstants.Attribute_ConcurrencyMode, EnumProperty<EdmConcurrencyMode>(EdmUtil.TryGetConcurrencyModeFromString, propMode => property.ConcurrencyMode = propMode, 
                modeAttr => RaiseError(modeAttr.Location, XmlErrorCode.InvalidConcurrencyMode, Edm.Strings.CsdlParser_InvalidConcurrencyMode(modeAttr.Value))));
                                    
            // Default Value
            Optional(CsdlConstants.Attribute_DefaultValue, defaultValueAttr =>
                {
                    if (property.PropertyType.EdmType != null && property.PropertyType.EdmType.ItemKind == EdmItemKind.PrimitiveType)
                    {
                        object parsedDefault;
                        if (TryParseDefaultValue(property.PropertyType.PrimitiveType.PrimitiveTypeKind, defaultValueAttr.Value, defaultValueAttr.Location, out parsedDefault))
                        {
                            property.DefaultValue = parsedDefault;
                            AddProperty(EdmConstants.Property_DefaultValue, defaultValueAttr.Location);
                        }
                    }
                    else
                    {

                        // Default value is only allowed for primitive types
                        RaiseError(defaultValueAttr.Location, XmlErrorCode.DefaultNotAllowed, Edm.Strings.CsdlParser_DefaultNotAllowed);
                    }
                });

            // For 1.1, CollectionKind adjusts the property type and applies specific collection semantics.
            if (this.DocumentVersion == CsdlConstants.Version1_1)
            {
                Optional(CsdlConstants.Attribute_CollectionKind, EnumProperty<EdmCollectionKind>(TryGetCollectionKindFromCsdlCollectionKind,
                    kind => { if (kind != EdmCollectionKind.Default) { property.CollectionKind = kind; property.PropertyType.CollectionRank = 1; } },
                    invalidKindAttr => RaiseError(invalidKindAttr.Location, XmlErrorCode.InvalidCollectionKind, Edm.Strings.CsdlParser_InvalidCollectionKind(invalidKindAttr.Value))));
            }
#endif
        }

        private CsdlValueTerm OnValueTermElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = Required(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location);
            string name = Required(CsdlConstants.Attribute_Name);

            return new CsdlValueTerm(name, type, Documentation(childValues), element.Location);
        }

        private CsdlAnnotations OnAnnotationsElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string target = Required(CsdlConstants.Attribute_Target);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            IEnumerable<CsdlVocabularyAnnotationBase> annotations = childValues.ValuesOfType<CsdlVocabularyAnnotationBase>();

            return new CsdlAnnotations(annotations, target, qualifier);
        }

        private CsdlValueAnnotation OnValueAnnotationElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string term = Required(CsdlConstants.Attribute_Term);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            CsdlExpressionBase expression = this.ParseAnnotationExpression(element, childValues);

            return new CsdlValueAnnotation(term, qualifier, expression, element.Location);
        }

        private CsdlTypeAnnotation OnTypeAnnotationElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string term = Required(CsdlConstants.Attribute_Term);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            IEnumerable<CsdlPropertyValue> properties = childValues.ValuesOfType<CsdlPropertyValue>();

            return new CsdlTypeAnnotation(term, qualifier, properties, element.Location);
        }

        private CsdlPropertyValue OnPropertyValueElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string property = Required(CsdlConstants.Attribute_Property);
            CsdlExpressionBase expression = this.ParseAnnotationExpression(element, childValues);

            return new CsdlPropertyValue(property, expression, element.Location);
        }

        private CsdlRecordExpression OnRecordElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string type = Optional(CsdlConstants.Attribute_Type);
            IEnumerable<CsdlPropertyValue> propertyValues = childValues.ValuesOfType<CsdlPropertyValue>();

            return new CsdlRecordExpression(new CsdlNamedTypeReference(type, false, element.Location), propertyValues, element.Location);
        }

        private static CsdlConstantExpression ConstantExpression(CsdlConstantExpressionKind kind, XmlElementValueCollection childValues, CsdlLocation location)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlConstantExpression(kind, text != null ? text.TextValue : string.Empty, location);
        }

        private CsdlConstantExpression OnIntConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(CsdlConstantExpressionKind.Int, childValues, element.Location);
        }

        private CsdlConstantExpression OnStringConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(CsdlConstantExpressionKind.String, childValues, element.Location);
        }

        private CsdlConstantExpression OnFloatConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(CsdlConstantExpressionKind.Float, childValues, element.Location);
        }

        private CsdlConstantExpression OnDecimalConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(CsdlConstantExpressionKind.Decimal, childValues, element.Location);
        }

        private CsdlConstantExpression OnBoolConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(CsdlConstantExpressionKind.Bool, childValues, element.Location);
        }

        private CsdlConstantExpression OnDateTimeConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(CsdlConstantExpressionKind.DateTime, childValues, element.Location);
        }

        private CsdlPathExpression OnPathExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlPathExpression(text != null ? text.TextValue : string.Empty, element.Location);
        }

        private CsdlExpressionBase ParseAnnotationExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            // ToDo: Give a syntax error if there is more than one expression.
            CsdlExpressionBase expression = childValues.ValuesOfType<CsdlExpressionBase>().FirstOrDefault();
            if (expression != null)
            {
                return expression;
            }

            string pathValue = Optional(CsdlConstants.Attribute_Path);
            if (pathValue != null)
            {
                return new CsdlPathExpression(pathValue, element.Location);
            }

            CsdlConstantExpressionKind kind;

            string value = Optional(CsdlConstants.Attribute_String);
            if (value != null)
            {
                kind = CsdlConstantExpressionKind.String;
            }
            else
            {
                value = Optional(CsdlConstants.Attribute_Int);
                if (value != null)
                {
                    kind = CsdlConstantExpressionKind.Int;
                }
                else
                {
                    value = Optional(CsdlConstants.Attribute_Float);
                    if (value != null)
                    {
                        kind = CsdlConstantExpressionKind.Float;
                    }
                    else
                    {
                        value = Optional(CsdlConstants.Attribute_Decimal);
                        if (value != null)
                        {
                            kind = CsdlConstantExpressionKind.Decimal;
                        }
                        else
                        {
                            value = Optional(CsdlConstants.Attribute_Bool);
                            if (value != null)
                            {
                                kind = CsdlConstantExpressionKind.Bool;
                            }
                            else
                            {
                                value = Optional(CsdlConstants.Attribute_DateTime);
                                if (value != null)
                                {
                                    kind = CsdlConstantExpressionKind.DateTime;
                                }
                                else
                                {
                                    // Annotation expressions are always optional.
                                    return null;
                                }
                            }
                        }
                    }
                }
            }

            return new CsdlConstantExpression(kind, value, element.Location);
        }

        private CsdlNamedTypeReference ParseNamedTypeReference(string typeName, CsdlLocation parentLocation)
        {
            var edm = Microsoft.Data.Edm.Library.EdmCoreModel.Instance;
            bool isNullable = OptionalBoolean(CsdlConstants.Attribute_Nullable) ?? CsdlConstants.Default_Nullable;
            EdmPrimitiveTypeKind kind = edm.GetPrimitiveTypeKind(typeName);
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
                    return new CsdlPrimitiveTypeReference(kind, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Binary:
                    {
                        int? maxLength;
                        bool isMaxMaxLength;
                        bool? isFixedLength;
                        this.ParseBinaryFacets(out isMaxMaxLength, out maxLength, out isFixedLength);

                        return new CsdlBinaryTypeReference(isFixedLength, isMaxMaxLength, maxLength, typeName, isNullable, parentLocation);
                    }

                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Time:
                    {
                        int? precision;
                        this.ParseTemporalFacets(out precision);

                        return new CsdlTemporalTypeReference(kind, precision, typeName, isNullable, parentLocation);
                    }

                case EdmPrimitiveTypeKind.Decimal:
                    {
                        int? precision;
                        int? scale;
                        this.ParseDecimalFacets(out precision, out scale);

                        return new CsdlDecimalTypeReference(precision, scale, typeName, isNullable, parentLocation);
                    }

                case EdmPrimitiveTypeKind.String:
                    {
                        int? maxLength;
                        bool isMaxMaxLength;
                        bool? isFixedLength;
                        bool? isUnicode;
                        string collation;
                        this.ParseStringFacets(out isMaxMaxLength, out maxLength, out isFixedLength, out isUnicode, out collation);

                        return new CsdlStringTypeReference(isFixedLength, isMaxMaxLength, maxLength, isUnicode, collation, typeName, isNullable, parentLocation);
                    }

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                    {
                        int? srid;
                        this.ParseSpatialFacets(out srid, CsdlConstants.Default_SpatialGeographySrid);
                        return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);
                    }

                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometricPoint:
                case EdmPrimitiveTypeKind.GeometricLineString:
                case EdmPrimitiveTypeKind.GeometricPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                case EdmPrimitiveTypeKind.GeometricMultiLineString:
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                    {
                        int? srid;
                        this.ParseSpatialFacets(out srid, CsdlConstants.Default_SpatialGeometrySrid);
                        return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);
                    }
            }

            return new CsdlNamedTypeReference(typeName, isNullable, parentLocation);
        }

        private CsdlTypeReference ParseTypeReference(string typeString, XmlElementValueCollection childValues, CsdlLocation parentLocation)
        {
            CsdlTypeReference elementType;
            if (typeString != null)
            {
                string[] typeInformation = typeString.Split(new char[] { '(', ')' });
                string typeName = typeInformation[0];
                switch (typeName)
                {
                    case CsdlConstants.Value_Collection:
                    case CsdlConstants.Value_MultiValue:
                        {
                            string elementTypeName = null;
                            if (typeInformation.Count() > 1)
                            {
                                elementTypeName = typeInformation[1];
                            }

                            elementType = new CsdlExpressionTypeReference(new CsdlCollectionType(this.ParseTypeReference(elementTypeName, childValues, parentLocation), typeName == CsdlConstants.Value_MultiValue, parentLocation), CsdlConstants.Default_Nullable, parentLocation);
                        }

                        break;
                    case CsdlConstants.Value_Ref:
                        {
                            string elementTypeName = typeInformation[1];
                            elementType = new CsdlExpressionTypeReference(new CsdlEntityReferenceType(new CsdlNamedTypeReference(elementTypeName, CsdlConstants.Default_Nullable, parentLocation), parentLocation), CsdlConstants.Default_Nullable, parentLocation);
                        }

                        break;
                    default:
                        elementType = this.ParseNamedTypeReference(typeName, parentLocation);
                        break;
                }
            }
            else
            {
                elementType = childValues.ValuesOfType<CsdlTypeReference>().FirstOrDefault();
            }

            return elementType;
        }

        private CsdlNavigationProperty OnNavigationPropertyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string relationship = Required(CsdlConstants.Attribute_Relationship);
            string toRole = Required(CsdlConstants.Attribute_ToRole);
            string fromRole = Required(CsdlConstants.Attribute_FromRole);

            return new CsdlNavigationProperty(name, relationship, toRole, fromRole, Documentation(childValues), element.Location);
        }

        private static CsdlKey OnEntityKeyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return new CsdlKey(new List<CsdlPropertyReference>(childValues.ValuesOfType<CsdlPropertyReference>()), element.Location);
        }

        private CsdlPropertyReference OnPropertyRefElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return new CsdlPropertyReference(Required(CsdlConstants.Attribute_Name), element.Location);
        }

        private CsdlEnumType OnEnumTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string underlyingType = Optional(CsdlConstants.Attribute_UnderlyingType);
            bool? isFlags = OptionalBoolean(CsdlConstants.Attribute_IsFlags);

            return new CsdlEnumType(name, underlyingType, isFlags ?? false, childValues.ValuesOfType<CsdlEnumMember>(), Documentation(childValues), element.Location);
        }

        private CsdlEnumMember OnEnumMemberElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            long? value = OptionalLong(CsdlConstants.Attribute_Value);

            return new CsdlEnumMember(name, value, Documentation(childValues), element.Location);
        }

        private CsdlAssociation OnAssociationElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);

            IEnumerable<CsdlAssociationEnd> ends = childValues.ValuesOfType<CsdlAssociationEnd>();
            if (ends.Count() != 2)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidAssociation, Edm.Strings.CsdlParser_InvalidAssociationIncorrectNumberOfEnds(name));
            }

            IEnumerable<CsdlReferentialConstraint> constraints = childValues.ValuesOfType<CsdlReferentialConstraint>();
            if (constraints.Count() > 1)
            {
                this.ReportError(childValues.OfResultType<CsdlReferentialConstraint>().ElementAt(1).Location, EdmErrorCode.InvalidAssociation, Edm.Strings.CsdlParser_AssociationHasAtMostOneConstraint);
            }

            return new CsdlAssociation(name, ends.ElementAtOrDefault(0), ends.ElementAtOrDefault(1), constraints.FirstOrDefault(), Documentation(childValues), element.Location);
        }

        private CsdlAssociationEnd OnAssociationEndElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string type = Required(CsdlConstants.Attribute_Type);
            string role = Optional(CsdlConstants.Attribute_Role);
            EdmAssociationMultiplicity multiplicity = RequiredMultiplicity(CsdlConstants.Attribute_Multiplicity);
            CsdlOnDelete onDelete = childValues.ValuesOfType<CsdlOnDelete>().FirstOrDefault();

            bool isNullable;
            switch (multiplicity)
            {
                case EdmAssociationMultiplicity.One:
                case EdmAssociationMultiplicity.Many:
                    isNullable = false;
                    break;
                default:
                    Debug.Assert(multiplicity == EdmAssociationMultiplicity.ZeroOrOne, "multiplicity == EdmAssociationMultiplicity.ZeroOrOne");
                    isNullable = true;
                    break;
            }

            CsdlNamedTypeReference endType = new CsdlNamedTypeReference(type, isNullable, element.Location);
            return new CsdlAssociationEnd(role, endType, multiplicity, onDelete, Documentation(childValues), element.Location);
        }

        private CsdlOnDelete OnDeleteActionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            EdmOnDeleteAction action = RequiredOnDeleteAction(CsdlConstants.Attribute_Action);

            return new CsdlOnDelete(action, Documentation(childValues), element.Location);
        }

        private CsdlReferentialConstraint OnReferentialConstraintElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            CsdlReferentialConstraintRole principal = this.GetConstraintRole(childValues, CsdlConstants.Element_Principal, () => Edm.Strings.CsdlParser_ReferentialConstraintRequiresOnePrincipal);
            CsdlReferentialConstraintRole dependent = this.GetConstraintRole(childValues, CsdlConstants.Element_Dependent, () => Edm.Strings.CsdlParser_ReferentialConstraintRequiresOneDependent);

            return new CsdlReferentialConstraint(principal, dependent, Documentation(childValues), element.Location);
        }

        private CsdlReferentialConstraintRole GetConstraintRole(XmlElementValueCollection childValues, string roleElementName, Func<string> excessErrorMessage)
        {
            CsdlReferentialConstraintRole result = null;

            childValues.FindByName<CsdlReferentialConstraintRole>(roleElementName)
                .Do(roleElement => result = roleElement.Value)
                .Then(excessRoleElement => ReportError(excessRoleElement.Location, EdmErrorCode.InvalidAssociation, excessErrorMessage()));

            return result;
        }

        private CsdlReferentialConstraintRole OnReferentialConstraintRoleElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string role = Required(CsdlConstants.Attribute_Role);
            IEnumerable<CsdlPropertyReference> properties = childValues.ValuesOfType<CsdlPropertyReference>();

            return new CsdlReferentialConstraintRole(role, properties, Documentation(childValues), element.Location);
        }

        private CsdlFunction OnFunctionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string returnTypeName = Optional(CsdlConstants.Attribute_ReturnType);
            IEnumerable<CsdlFunctionParameter> parameters = childValues.ValuesOfType<CsdlFunctionParameter>();
            var definingExpressionElement = childValues[CsdlConstants.Element_DefiningExpression];
            string definingExpression = null;
            if (!(definingExpressionElement is XmlElementValueCollection.MissingXmlElementValue))
            {
                // If the element exists we want to reflect that it had a defining expression, but that it was empty 
                // rather than never having one at all.
                definingExpression = definingExpressionElement.TextValue ?? string.Empty;
            }

            CsdlTypeReference returnType = null;
            if (returnTypeName == null)
            {
                // TODO: Use the CsdlFunctionReturnType annotations somewhere
                CsdlFunctionReturnType returnTypeElementValue = childValues.ValuesOfType<CsdlFunctionReturnType>().FirstOrDefault();
                if (returnTypeElementValue != null)
                {
                    returnType = returnTypeElementValue.ReturnType;
                }
            }
            else
            {
                // Still need to cope with Collection(type) shortcuts so ParseNamedTypeReference cannot be used.
                returnType = this.ParseTypeReference(returnTypeName, null, element.Location);
            }

            return new CsdlFunction(name, parameters, definingExpression, returnType, Documentation(childValues), element.Location);
        }

        private CsdlFunctionParameter OnParameterElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string typeName = Optional(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location);

            return new CsdlFunctionParameter(name, type, CsdlConstants.Default_FunctionParameterMode, Documentation(childValues), element.Location);
        }

        private CsdlFunctionImport OnFunctionImportElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string returnTypeName = Optional(CsdlConstants.Attribute_ReturnType);
            CsdlTypeReference returnType = this.ParseTypeReference(returnTypeName, childValues, element.Location);
            bool composable = OptionalBoolean(CsdlConstants.Attribute_IsComposable) ?? CsdlConstants.Default_IsComposable;
            bool sideEffecting = OptionalBoolean(CsdlConstants.Attribute_IsSideEffecting) ?? (composable ? false : CsdlConstants.Default_IsSideEffecting);
            bool bindable = OptionalBoolean(CsdlConstants.Attribute_IsBindable) ?? CsdlConstants.Default_IsBindable;
            string entitySetPath = Optional(CsdlConstants.Attribute_EntitySetPath);
            string entitySet = Optional(CsdlConstants.Attribute_EntitySet);
            IEnumerable<CsdlFunctionParameter> parameters = childValues.ValuesOfType<CsdlFunctionParameter>();

            return new CsdlFunctionImport(
                name,
                sideEffecting,
                composable,
                bindable,
                entitySetPath,
                entitySet,
                parameters,
                returnType,
                Documentation(childValues),
                element.Location);
        }

        private CsdlFunctionParameter OnFunctionImportParameterElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string typeName = Required(CsdlConstants.Attribute_Type);
            EdmFunctionParameterMode? mode = OptionalFunctionParameterMode(CsdlConstants.Attribute_Mode);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location);
            return new CsdlFunctionParameter(name, type, mode ?? CsdlConstants.Default_FunctionParameterMode, Documentation(childValues), element.Location);
        }

        private CsdlTypeReference OnEntityReferenceTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = Required(CsdlConstants.Attribute_Type);
            return new CsdlExpressionTypeReference(new CsdlEntityReferenceType(this.ParseTypeReference(typeName, null, element.Location), element.Location), CsdlConstants.Default_Nullable, element.Location);
        }

        private CsdlTypeReference OnTypeRefElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = Required(CsdlConstants.Attribute_Type);
            return this.ParseTypeReference(typeName, null, element.Location);
        }

        private CsdlTypeReference OnRowTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return new CsdlExpressionTypeReference(new CsdlRowType(childValues.ValuesOfType<CsdlProperty>(), element.Location), true, element.Location);
        }

        private CsdlTypeReference OnCollectionTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string elementTypeName = Optional(CsdlConstants.Attribute_ElementType);
            CsdlTypeReference elementType = this.ParseTypeReference(elementTypeName, childValues, element.Location);

            return new CsdlExpressionTypeReference(new CsdlCollectionType(elementType, CsdlConstants.Collection_IsAtomic, element.Location), false, element.Location);
        }

        private CsdlProperty OnRowTypePropertyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string typeName = Optional(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location);

            return new CsdlProperty(name, type, false, null, Documentation(childValues), element.Location);
        }

        private CsdlFunctionReturnType OnReturnTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = Optional(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location);
            return new CsdlFunctionReturnType(type, element.Location);
        }

        private CsdlEntityContainer OnEntityContainerElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string extends = Optional(CsdlConstants.Attribute_Extends);

            return new CsdlEntityContainer(
                name, 
                extends, 
                childValues.ValuesOfType<CsdlEntitySet>(), 
                childValues.ValuesOfType<CsdlAssociationSet>(),
                childValues.ValuesOfType<CsdlFunctionImport>(),
                Documentation(childValues), 
                element.Location);
        }

        private CsdlEntitySet OnEntitySetElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string entityType = Required(CsdlConstants.Attribute_EntityType);

            return new CsdlEntitySet(name, entityType, Documentation(childValues), element.Location);
        }

        private CsdlAssociationSet OnAssociationSetElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string association = Required(CsdlConstants.Attribute_Association);
            IEnumerable<CsdlAssociationSetEnd> ends = childValues.ValuesOfType<CsdlAssociationSetEnd>();
            if (ends.Count() != 2)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidAssociationSet, Edm.Strings.CsdlParser_InvalidAssociationSetIncorrectNumberOfEnds(name));
            }

            return new CsdlAssociationSet(name, association, ends.ElementAtOrDefault(0), ends.ElementAtOrDefault(1) , Documentation(childValues), element.Location);
        }

        private CsdlAssociationSetEnd OnAssociationSetEndElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string role = Required(CsdlConstants.Attribute_Role);
            string entitySet = Required(CsdlConstants.Attribute_EntitySet);

            return new CsdlAssociationSetEnd(role, entitySet, Documentation(childValues), element.Location);
        }

#if false
        private void InitializeConstraint(EdmAssociationType association, XmlElementValue<EdmAssociationConstraint> constraint)
        {
            if (constraint.Value != null)
            {
                association.Constraint = constraint.Value;
                ResolveReferentialConstraintAction resolveConstraintAction = null;// = this.constraintActions[constraint.Value];
                resolveConstraintAction.Association = association;
                ResolveConstraintRoleEnd(association, resolveConstraintAction.Principal);
                ResolveConstraintRoleEnd(association, resolveConstraintAction.Dependent);
                if (resolveConstraintAction.Dependent != null && resolveConstraintAction.Dependent.Role != null)
                {
                    association.Constraint.DependentEnd = resolveConstraintAction.Dependent.Role;
                }
                this.AddAction(resolveConstraintAction);
            }
        }

        private void ResolveConstraintRoleEnd(EdmAssociationType association, ConstraintRoleInfo roleInfo)
        {
            EdmAssociationEnd roleEnd;
            if (association.TryGetAssociationEnd(roleInfo.RoleName, out roleEnd))
            {
                roleInfo.Role = roleEnd;
            }
            else
            {
                this.RaiseError(roleInfo.RoleNameLocation, XmlErrorCode.InvalidRoleInRelationshipConstraint,
                    Edm.Strings.CsdlParser_InvalidEndRoleInRelationshipConstraint(roleInfo.RoleName, association.Name)
                );
            }
        }
                
       
        private SetEndInfo OnAssociationSetEndElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string roleName = null;
            LineLocation roleLocation = default(LineLocation);
            string entitySetName = null;
            LineLocation entitySetLocation = default(LineLocation);
                  
            Optional(element, CsdlConstants.Attribute_Role,      Name((attribute, value) => { roleName = value; roleLocation = attribute.Location; }));
            Optional(element, CsdlConstants.Attribute_EntitySet, Name((attribute, value) => { entitySetName = value; entitySetLocation = attribute.Location; }));
            
            SetEndInfo result = new SetEndInfo(element.Location, roleName, roleLocation, entitySetName, entitySetLocation);
            
            // Documentation is allowed by CSDL, so mark it as 'used' even though it cannot be added to the model.
            childValues.ValuesOfType<DataModelDocumentation>().Do(doc => { });

            return result;
        }

        private void AddAction(CsdlParserAction action)
        {
        }
        
        private static bool TryGetAssociationEndKindFromCsdlMultiplicity(string mult, out EdmAssociationEndKind kind)
        {
            switch (mult)
            {
                case CsdlConstants.Value_EndMany:
                    kind = EdmAssociationEndKind.Many;
                    return true;

                case CsdlConstants.Value_EndOptional:
                    kind = EdmAssociationEndKind.Optional;
                    return true;

                case CsdlConstants.Value_EndRequired:
                    kind = EdmAssociationEndKind.Required;
                    return true;

                default:
                    kind = EdmAssociationEndKind.None;
                    return false;
            }
        }

        private static bool TryGetCollectionKindFromCsdlCollectionKind(string csdlKind, out EdmCollectionKind kind)
        {
            switch (csdlKind)
            {
                case CsdlConstants.Value_Bag:
                    kind = EdmCollectionKind.Bag;
                    return true;

                case CsdlConstants.Value_List:
                    kind = EdmCollectionKind.List;
                    return true;

                case CsdlConstants.Value_None:
                    kind = EdmCollectionKind.Default;
                    return true;

                default:
                    kind = EdmCollectionKind.None;
                    return false;
            }
        }
#endif
        private void ParseMaxLength(out bool maxMaxLength, out int? maxLength)
        {
            string max = Optional(CsdlConstants.Attribute_MaxLength);
            if (max == null)
            {
                maxMaxLength = false;
                maxLength = null;
            }
            else if (max.EqualsOrdinalIgnoreCase(CsdlConstants.Value_Max))
            {
                maxMaxLength = true;
                maxLength = null;
            }
            else
            {
                maxMaxLength = false;
                maxLength = OptionalMaxLength(CsdlConstants.Attribute_MaxLength);
            }
        }

        private void ParseBinaryFacets(out bool maxMaxLength, out int? maxLength, out bool? fixedLength)
        {
            this.ParseMaxLength(out maxMaxLength, out maxLength);
            fixedLength = OptionalBoolean(CsdlConstants.Attribute_FixedLength);
        }

        private void ParseDecimalFacets(out int? precision, out int? scale)
        {
            precision = OptionalInteger(CsdlConstants.Attribute_Precision);
            scale = OptionalInteger(CsdlConstants.Attribute_Scale);
        }

        private void ParseStringFacets(out bool maxMaxLength, out int? maxLength, out bool? fixedLength, out bool? unicode, out string collation)
        {
            this.ParseMaxLength(out maxMaxLength, out maxLength);
            fixedLength = OptionalBoolean(CsdlConstants.Attribute_FixedLength);
            unicode = OptionalBoolean(CsdlConstants.Attribute_Unicode);
            collation = Optional(CsdlConstants.Attribute_Collation);
        }

        private void ParseTemporalFacets(out int? precision)
        {
            precision = OptionalInteger(CsdlConstants.Attribute_Precision);
        }

        private void ParseSpatialFacets(out int? srid, int defaultSrid)
        {
            srid = OptionalSrid(CsdlConstants.Attribute_Srid, defaultSrid);
        }

        /*
        private XmlElementParser<TItem> Element<TItem>(string elementName, Func<EdmModel, TItem> constructor, Action<TItem, XmlElementInfo, XmlElementValueCollection> initializer, params XmlElementParser[] childParsers)
            where TItem : EdmNamedMetadataItem
        {
            return Element<TItem>(elementName, (element, childValues) =>
                {
                    TItem result = constructor(this.model);
                    BeginNamedItem(result, element, childValues);
                    initializer(result, element, childValues);
                    EndItem(result);
                    return result;
                },
                childParsers);
        }*/

#if false
        private Action<XmlAttributeInfo> TypeReference(Action<XmlAttributeInfo, CsdlNamespaceSymbol, string> onValidReference)
        {
            return attribute =>
            {
                string namespaceOrAlias;
                string name;
                if (TryGetOptionallyQualifiedName(attribute, out namespaceOrAlias, out name))
                {
                    CsdlNamespaceSymbol ns = null;
                    if (namespaceOrAlias != null && !EdmConstants.EdmNamespace.EqualsOrdinal(namespaceOrAlias))
                    {
                        ns = GetNamespaceReference(namespaceOrAlias);
                    }
                    onValidReference(attribute, ns, name);
                }
            };
        }

        private CsdlNamespaceSymbol GetNamespaceReference(string namespaceNameOrAlias)
        {
            CsdlNamespaceSymbol result;
            if (!this.referencedNamespaces.TryGetValue(namespaceNameOrAlias, out result))
            {
                result = new CsdlNamespaceSymbol(namespaceNameOrAlias);
                this.referencedNamespaces[namespaceNameOrAlias] = result;
            }
            return result;
        }

        private Action<XmlAttributeInfo> QualifiedName(Action<XmlAttributeInfo, CsdlNamespaceSymbol, string> onValidQualifiedName)
        {
            return attribute =>
            {
                string namespaceOrAlias;
                string name;
                if (TryGetOptionallyQualifiedName(attribute, out namespaceOrAlias, out name))
                {
                    if (!string.IsNullOrEmpty(namespaceOrAlias))
                    {
                        CsdlNamespaceSymbol namespaceRef = GetNamespaceReference(namespaceOrAlias);
                        onValidQualifiedName(attribute, namespaceRef, name);
                    }
                    else
                    {
                        this.RaiseError(attribute.Location, XmlErrorCode.NotInNamespace, Edm.Strings.CsdlParser_NotNamespaceQualified(name));
                    }
                }
            };
        }

        private Action<XmlAttributeInfo> Resolve<TItem>(TItem target, Action<TItem, EdmAssociationType> onResolvedAssociation)
        {
            return QualifiedName((attribute, namespaceName, name) => AddAction(new AssociationTypeAction<TItem>(this.DocumentPath, this.onError, namespaceName, name, attribute.Location, target, onResolvedAssociation)));
        }

        private Action<XmlAttributeInfo> Resolve<TItem>(TItem target, Action<TItem, EdmComplexType> onResolvedComplexType)
        {
            return QualifiedName((attribute, namespaceName, name) => AddAction(new ComplexTypeAction<TItem>(this.DocumentPath, this.onError, namespaceName, name, attribute.Location, target, onResolvedComplexType)));
        }

        private Action<XmlAttributeInfo> Resolve<TItem>(TItem target, Action<TItem, EdmEntityType> onResolvedEntity)
        {
            return QualifiedName((attribute, namespaceName, name) => AddAction(new EntityTypeAction<TItem>(this.DocumentPath, this.onError, namespaceName, name, attribute.Location, target, onResolvedEntity)));
        }
#endif
    }
}
