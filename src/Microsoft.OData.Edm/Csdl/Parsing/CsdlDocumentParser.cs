//---------------------------------------------------------------------
// <copyright file="CsdlDocumentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Parsing.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// CSDL document parser.
    /// </summary>
    internal class CsdlDocumentParser : EdmXmlDocumentParser<CsdlSchema>
    {
        private Version artifactVersion;
        private int entityContainerCount;

        internal CsdlDocumentParser(string documentPath, XmlReader reader)
            : base(documentPath, reader)
        {
            entityContainerCount = 0;
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
                annotatedItem.AddAnnotation(new CsdlDirectValueAnnotation(xmlAnnotation.NamespaceName, xmlAnnotation.Name, xmlAnnotation.Value, xmlAnnotation.IsAttribute, xmlAnnotation.Location));
            }

            foreach (var annotation in childValues.ValuesOfType<CsdlAnnotation>())
            {
                annotatedItem.AddAnnotation(annotation);
            }
        }

        private XmlElementParser<CsdlSchema> CreateRootElementParser()
        {
            var documentationParser =
                //// <Documentation>
                CsdlElement<CsdlDocumentation>(CsdlConstants.Element_Documentation, this.OnDocumentationElement,
                   //// <Summary/>
                   Element(CsdlConstants.Element_Summary, (element, children) => children.FirstText.Value),
                   //// <LongDescription/>
                   Element(CsdlConstants.Element_LongDescription, (element, children) => children.FirstText.TextValue));
            //// </Documentation>

            // There is recursion in the grammar between CollectionType, ReturnType, and Property within RowType.
            // This requires breaking up the parser construction into pieces and then weaving them together with AddChildParser.
            var referenceTypeParser =
                //// <ReferenceType/>
                CsdlElement<CsdlTypeReference>(CsdlConstants.Element_ReferenceType, this.OnEntityReferenceTypeElement, documentationParser);

            var collectionTypeParser =
                //// <CollectionType>
                CsdlElement<CsdlTypeReference>(CsdlConstants.Element_CollectionType, this.OnCollectionTypeElement, documentationParser,
                    //// <TypeRef/>
                    CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser),
                    //// <ReferenceType/>
                    referenceTypeParser);
            //// </CollectionType>

            var nominalTypePropertyElementParser =
                //// <Property/>
                CsdlElement<CsdlProperty>(CsdlConstants.Element_Property, this.OnPropertyElement, documentationParser);

            var stringConstantExpressionParser =
                //// <String/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_String, OnStringConstantExpression);

            var binaryConstantExpressionParser =
                //// <Binary/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Binary, OnBinaryConstantExpression);

            var intConstantExpressionParser =
                //// <Int/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Int, OnIntConstantExpression);

            var floatConstantExpressionParser =
                //// <Float/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Float, OnFloatConstantExpression);

            var guidConstantExpressionParser =
                //// <Guid/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Guid, OnGuidConstantExpression);

            var decimalConstantExpressionParser =
                //// <Decimal/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Decimal, OnDecimalConstantExpression);

            var boolConstantExpressionParser =
                //// <Bool/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Bool, OnBoolConstantExpression);

            var durationConstantExpressionParser =
                //// <Duration/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Duration, OnDurationConstantExpression);

            var dateConstantExpressionParser =
                //// <Date/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Date, OnDateConstantExpression);

            var timeOfDayConstantExpressionParser =
                //// <TimeOfDay/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_TimeOfDay, OnTimeOfDayConstantExpression);

            var dateTimeOffsetConstantExpressionParser =
                //// <DateTimeOffset/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_DateTimeOffset, OnDateTimeOffsetConstantExpression);

            var nullConstantExpressionParser =
               //// <Null/>
               CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Null, OnNullConstantExpression);

            var pathExpressionParser =
                //// <Path/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Path, OnPathExpression);

            var propertyPathExpressionParser =
                //// <PropertyPath/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_PropertyPath, OnPropertyPathExpression);

            var navigationPropertyPathExpressionParser =
                //// <NavigationPropertyPath/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_NavigationPropertyPath, OnNavigationPropertyPathExpression);

            var enumMemberExpressionParser =
                //// <EnumMember/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_EnumMember, this.OnEnumMemberExpression);

            var ifExpressionParser =
                //// <If>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_If, this.OnIfExpression);

            var castExpressionParser =
                //// <Cast>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Cast, this.OnCastExpression);

            var isTypeExpressionParser =
                //// <IsType>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_IsType, this.OnIsTypeExpression);

            var propertyValueParser =
                //// <PropertyValue>
                CsdlElement<CsdlPropertyValue>(CsdlConstants.Element_PropertyValue, this.OnPropertyValueElement);

            var recordExpressionParser =
                //// <Record>
                CsdlElement<CsdlRecordExpression>(CsdlConstants.Element_Record, this.OnRecordElement,
                    //// <PropertyValue />
                    propertyValueParser);
            //// </Record>

            var labeledElementParser =
                //// <LabeledElement>
                CsdlElement<CsdlLabeledExpression>(CsdlConstants.Element_LabeledElement, this.OnLabeledElement);

            var collectionExpressionParser =
                //// <Collection>
                CsdlElement<CsdlCollectionExpression>(CsdlConstants.Element_Collection, this.OnCollectionElement);

            var applyExpressionParser =
                //// <Apply>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Apply, this.OnApplyElement);

            var labeledElementReferenceExpressionParser =
                //// <LabeledElementReference/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_LabeledElementReference, this.OnLabeledElementReferenceExpression);

            XmlElementParser[] expressionParsers =
            {
                //// <String/>
                stringConstantExpressionParser,
                //// <Binary/>
                binaryConstantExpressionParser,
                //// <Int/>
                intConstantExpressionParser,
                //// <Float/>
                floatConstantExpressionParser,
                //// <Guid/>
                guidConstantExpressionParser,
                //// <Decimal/>
                decimalConstantExpressionParser,
                //// <Bool/>
                boolConstantExpressionParser,
                //// <Date/>
                dateConstantExpressionParser,
                //// <DateTimeOffset/>
                dateTimeOffsetConstantExpressionParser,
                //// <Duration/>
                durationConstantExpressionParser,
                //// <TimeOfDay/>
                timeOfDayConstantExpressionParser,
                //// <Null/>
                nullConstantExpressionParser,
                //// <Path/>
                pathExpressionParser,
                //// <PropertyPath/>
                propertyPathExpressionParser,
                //// <NavigationPropertyPath/>
                navigationPropertyPathExpressionParser,
                //// <If/>
                ifExpressionParser,
                //// <IsType/>
                isTypeExpressionParser,
                //// <Cast>
                castExpressionParser,
                //// <Record/>
                recordExpressionParser,
                //// <Collection/>
                collectionExpressionParser,
                //// <LabeledElementReference/>
                labeledElementReferenceExpressionParser,
                //// <PropertyValue/>
                propertyValueParser,
                //// <LabeledElement/>
                labeledElementParser,
                //// <EnumConstant/>
                enumMemberExpressionParser,
                //// </Apply>
                applyExpressionParser
            };

            AddChildParsers(ifExpressionParser, expressionParsers);
            AddChildParsers(castExpressionParser, expressionParsers);
            AddChildParsers(isTypeExpressionParser, expressionParsers);
            AddChildParsers(propertyValueParser, expressionParsers);
            AddChildParsers(collectionExpressionParser, expressionParsers);
            AddChildParsers(labeledElementParser, expressionParsers);
            AddChildParsers(applyExpressionParser, expressionParsers);

            var annotationParser =
                //// <Annotation>
                CsdlElement<CsdlAnnotation>(CsdlConstants.Element_Annotation, this.OnAnnotationElement);

            AddChildParsers(annotationParser, expressionParsers);

            nominalTypePropertyElementParser.AddChildParser(annotationParser);

            collectionTypeParser.AddChildParser(collectionTypeParser);

            var rootElementParser =
            //// <Schema>
            CsdlElement<CsdlSchema>(CsdlConstants.Element_Schema, this.OnSchemaElement,
                documentationParser,
                //// <ComplexType>
                CsdlElement<CsdlComplexType>(CsdlConstants.Element_ComplexType, this.OnComplexTypeElement,
                    documentationParser,
                    //// <Property />
                    nominalTypePropertyElementParser,
                    //// <NavigationProperty>
                    CsdlElement<CsdlNamedElement>(CsdlConstants.Element_NavigationProperty, this.OnNavigationPropertyElement, documentationParser,
                        //// <ReferentialConstraint/>
                        CsdlElement<CsdlReferentialConstraint>(CsdlConstants.Element_ReferentialConstraint, this.OnReferentialConstraintElement, documentationParser),
                        //// <OnDelete/>
                        CsdlElement<CsdlOnDelete>(CsdlConstants.Element_OnDelete, this.OnDeleteActionElement, documentationParser),
                        //// <Annotation/>
                        annotationParser),
                    //// </NavigationProperty>
                    
                    //// <Annotation/>
                    annotationParser),
                //// </ComplexType>

                //// <EntityType>
                CsdlElement<CsdlEntityType>(CsdlConstants.Element_EntityType, this.OnEntityTypeElement,
                    documentationParser,
                    //// <Key>
                    CsdlElement<CsdlKey>(CsdlConstants.Element_Key, OnEntityKeyElement,
                        //// <PropertyRef/>
                        CsdlElement<CsdlPropertyReference>(CsdlConstants.Element_PropertyRef, this.OnPropertyRefElement)),
                    //// </Key>

                    //// <Property />
                    nominalTypePropertyElementParser,

                    //// <NavigationProperty>
                    CsdlElement<CsdlNamedElement>(CsdlConstants.Element_NavigationProperty, this.OnNavigationPropertyElement, documentationParser,
                        //// <ReferentialConstraint/>
                        CsdlElement<CsdlReferentialConstraint>(CsdlConstants.Element_ReferentialConstraint, this.OnReferentialConstraintElement, documentationParser),
                        //// <OnDelete/>
                        CsdlElement<CsdlOnDelete>(CsdlConstants.Element_OnDelete, this.OnDeleteActionElement, documentationParser),
                        //// <Annotation/>
                        annotationParser),
                    //// </NavigationProperty>

                    //// <Annotation/>
                    annotationParser),
                //// </EntityType>

                //// <EnumType>
                CsdlElement<CsdlEnumType>(CsdlConstants.Element_EnumType, this.OnEnumTypeElement,
                    documentationParser,
                    //// <Member>
                    CsdlElement<CsdlEnumMember>(CsdlConstants.Element_Member, this.OnEnumMemberElement, documentationParser, annotationParser),
                    //// <Annotation/>
                    annotationParser),
                //// </EnumType>

                //// <TypeDefinition>
                CsdlElement<CsdlTypeDefinition>(CsdlConstants.Element_TypeDefinition, this.OnTypeDefinitionElement,
                    //// <Annotation/>
                    annotationParser),
                //// </TypeDefinition>

                //// <Action>
                CsdlElement<CsdlAction>(CsdlConstants.Element_Action, this.OnActionElement,
                    documentationParser,
                    //// <Parameter>
                    CsdlElement<CsdlOperationParameter>(CsdlConstants.Element_Parameter, this.OnParameterElement,
                        documentationParser,
                        //// <TypeRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser),
                        //// <CollectionType/>
                        collectionTypeParser,
                        //// <ReferenceType/>
                        referenceTypeParser,
                        //// <Annotation/>
                        annotationParser),
                        //// </Parameter)

                        //// <ReturnType>
                        CsdlElement<CsdlOperationReturnType>(CsdlConstants.Element_ReturnType, this.OnReturnTypeElement,
                            documentationParser,
                            //// <TypeRef/>
                            CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser),
                            //// <CollectionType/>
                            collectionTypeParser,
                            //// <ReferenceType/>
                            referenceTypeParser,
                            //// <Annotation/>
                            annotationParser),
                        //// </ReturnType>

                        //// <Annotation/>
                        annotationParser),
                //// <Action>

                CsdlElement<CsdlOperation>(CsdlConstants.Element_Function, this.OnFunctionElement,
                    documentationParser,
                    //// <Parameter>
                    CsdlElement<CsdlOperationParameter>(CsdlConstants.Element_Parameter, this.OnParameterElement,
                        documentationParser,
                        //// <TypeRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser),
                        //// <CollectionType/>
                        collectionTypeParser,
                        //// <ReferenceType/>
                        referenceTypeParser,
                        //// <Annotation/>
                        annotationParser),
                    //// </Parameter

                    //// <ReturnType>
                    CsdlElement<CsdlOperationReturnType>(CsdlConstants.Element_ReturnType, this.OnReturnTypeElement,
                        documentationParser,
                        //// <TypeRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser),
                        //// <CollectionType/>
                        collectionTypeParser,
                        //// <ReferenceType/>
                        referenceTypeParser,
                        //// <Annotation/>
                        annotationParser),
                    //// </ReturnType>

                    //// <Annotation/>
                    annotationParser),
                //// </Function>

                //// <Term>
                CsdlElement<CsdlTerm>(CsdlConstants.Element_Term, this.OnTermElement,
                    //// <TypeRef/>
                    CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement, documentationParser),
                    //// <CollectionType/>
                    collectionTypeParser,
                    //// <ReferenceType/>
                    referenceTypeParser,
                    //// <Annotation/>
                    annotationParser),
                //// </Term>

                //// <Annotations>
                CsdlElement<CsdlAnnotations>(CsdlConstants.Element_Annotations, this.OnAnnotationsElement,
                    //// <Annotation/>
                    annotationParser),
                //// </Annotations>

                //// <EntityContainer>
                CsdlElement<CsdlEntityContainer>(CsdlConstants.Element_EntityContainer, this.OnEntityContainerElement,
                    documentationParser,
                    //// <EntitySet>
                    CsdlElement<CsdlEntitySet>(CsdlConstants.Element_EntitySet, this.OnEntitySetElement, documentationParser,
                        //// <NavigationPropertyBinding/>
                        CsdlElement<CsdlNavigationPropertyBinding>(CsdlConstants.Element_NavigationPropertyBinding, this.OnNavigationPropertyBindingElement),
                        //// <Annotation/>
                        annotationParser),
                    //// </EntitySet>

                    //// <Singleton>
                    CsdlElement<CsdlSingleton>(CsdlConstants.Element_Singleton, this.OnSingletonElement, documentationParser,
                        //// <NavigationPropertyBinding/>
                        CsdlElement<CsdlNavigationPropertyBinding>(CsdlConstants.Element_NavigationPropertyBinding, this.OnNavigationPropertyBindingElement),
                        //// <Annotation/>
                        annotationParser),
                    //// </Singleton>

                    //// <Action Import
                    CsdlElement<CsdlActionImport>(CsdlConstants.Element_ActionImport, this.OnActionImportElement,
                        documentationParser,
                        //// <Annotation/>
                        annotationParser),
                    ////</Actionmport

                    //// <Function Import
                    CsdlElement<CsdlOperationImport>(CsdlConstants.Element_FunctionImport, this.OnFunctionImportElement,
                        documentationParser,

                        //// <Parameter />
                        CsdlElement<CsdlOperationParameter>(CsdlConstants.Element_Parameter, this.OnFunctionImportParameterElement, documentationParser,
                            //// <Annotation/>
                            annotationParser),
                        ////</Parameter>

                        //// <Annotation/>
                        annotationParser),
                    ////</FunctionImport

                    //// <Annotation/>
                    annotationParser));
            ////</EntityContainer>
            //// </Schema>

            return rootElementParser;
        }

        private static CsdlDocumentation Documentation(XmlElementValueCollection childValues)
        {
            return childValues.ValuesOfType<CsdlDocumentation>().FirstOrDefault();
        }

        private CsdlSchema OnSchemaElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string namespaceName = Optional(CsdlConstants.Attribute_Namespace) ?? string.Empty;
            string alias = OptionalAlias(CsdlConstants.Attribute_Alias);

            CsdlSchema result =
                new CsdlSchema(
                    namespaceName,
                    alias,
                    this.artifactVersion,
                    childValues.ValuesOfType<CsdlStructuredType>(),
                    childValues.ValuesOfType<CsdlEnumType>(),
                    childValues.ValuesOfType<CsdlOperation>(),
                    childValues.ValuesOfType<CsdlTerm>(),
                    childValues.ValuesOfType<CsdlEntityContainer>(),
                    childValues.ValuesOfType<CsdlAnnotations>(),
                    childValues.ValuesOfType<CsdlTypeDefinition>(),
                    Documentation(childValues),
                    element.Location);

            return result;
        }

        private CsdlDocumentation OnDocumentationElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return new CsdlDocumentation(childValues[CsdlConstants.Element_Summary].TextValue, childValues[CsdlConstants.Element_LongDescription].TextValue, element.Location);
        }

        private CsdlComplexType OnComplexTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string baseType = OptionalQualifiedName(CsdlConstants.Attribute_BaseType);
            bool isOpen = OptionalBoolean(CsdlConstants.Attribute_OpenType) ?? CsdlConstants.Default_OpenType;
            bool isAbstract = OptionalBoolean(CsdlConstants.Attribute_Abstract) ?? CsdlConstants.Default_Abstract;

            return new CsdlComplexType(name, baseType, isAbstract, isOpen, childValues.ValuesOfType<CsdlProperty>(), childValues.ValuesOfType<CsdlNavigationProperty>(), Documentation(childValues), element.Location);
        }

        private CsdlEntityType OnEntityTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string baseType = OptionalQualifiedName(CsdlConstants.Attribute_BaseType);
            bool isOpen = OptionalBoolean(CsdlConstants.Attribute_OpenType) ?? CsdlConstants.Default_OpenType;
            bool isAbstract = OptionalBoolean(CsdlConstants.Attribute_Abstract) ?? CsdlConstants.Default_Abstract;
            bool hasStream = OptionalBoolean(CsdlConstants.Attribute_HasStream) ?? CsdlConstants.Default_HasStream;

            CsdlKey key = childValues.ValuesOfType<CsdlKey>().FirstOrDefault();

            return new CsdlEntityType(name, baseType, isAbstract, isOpen, hasStream, key, childValues.ValuesOfType<CsdlProperty>(), childValues.ValuesOfType<CsdlNavigationProperty>(), Documentation(childValues), element.Location);
        }

        private CsdlProperty OnPropertyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);
            string name = Required(CsdlConstants.Attribute_Name);
            string defaultValue = Optional(CsdlConstants.Attribute_DefaultValue);

            return new CsdlProperty(name, type, defaultValue, Documentation(childValues), element.Location);
        }

        private CsdlTerm OnTermElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);
            string name = Required(CsdlConstants.Attribute_Name);
            string appliesTo = Optional(CsdlConstants.Attribute_AppliesTo);
            string defaultValue = Optional(CsdlConstants.Attribute_DefaultValue);

            return new CsdlTerm(name, type, appliesTo, defaultValue, Documentation(childValues), element.Location);
        }

        private CsdlAnnotations OnAnnotationsElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string target = Required(CsdlConstants.Attribute_Target);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            IEnumerable<CsdlAnnotation> annotations = childValues.ValuesOfType<CsdlAnnotation>();

            return new CsdlAnnotations(annotations, target, qualifier);
        }

        private CsdlAnnotation OnAnnotationElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string term = RequiredQualifiedName(CsdlConstants.Attribute_Term);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            CsdlExpressionBase expression = this.ParseAnnotationExpression(element, childValues);

            return new CsdlAnnotation(term, qualifier, expression, element.Location);
        }

        private CsdlPropertyValue OnPropertyValueElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string property = Required(CsdlConstants.Attribute_Property);
            CsdlExpressionBase expression = this.ParseAnnotationExpression(element, childValues);

            return new CsdlPropertyValue(property, expression, element.Location);
        }

        private CsdlRecordExpression OnRecordElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string type = OptionalQualifiedName(CsdlConstants.Attribute_Type);
            IEnumerable<CsdlPropertyValue> propertyValues = childValues.ValuesOfType<CsdlPropertyValue>();

            return new CsdlRecordExpression(type != null ? new CsdlNamedTypeReference(type, false, element.Location) : null, propertyValues, element.Location);
        }

        private CsdlCollectionExpression OnCollectionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Optional);
            IEnumerable<CsdlExpressionBase> elementValues = childValues.ValuesOfType<CsdlExpressionBase>();

            return new CsdlCollectionExpression(type, elementValues, element.Location);
        }

        private CsdlLabeledExpression OnLabeledElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 1)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidLabeledElementExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidLabeledElementExpressionIncorrectNumberOfOperands);
            }

            return new CsdlLabeledExpression(
                name,
                expressions.ElementAtOrDefault(0),
                element.Location);
        }

        private CsdlApplyExpression OnApplyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string function = Optional(CsdlConstants.Attribute_Function);
            IEnumerable<CsdlExpressionBase> arguments = childValues.ValuesOfType<CsdlExpressionBase>();

            return new CsdlApplyExpression(function, arguments, element.Location);
        }

        private static void AddChildParsers(XmlElementParser parent, IEnumerable<XmlElementParser> children)
        {
            foreach (XmlElementParser child in children)
            {
                parent.AddChildParser(child);
            }
        }

        private static CsdlConstantExpression ConstantExpression(EdmValueKind kind, XmlElementValueCollection childValues, CsdlLocation location)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlConstantExpression(kind, text != null ? text.TextValue : string.Empty, location);
        }

        private static CsdlConstantExpression OnIntConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Integer, childValues, element.Location);
        }

        private static CsdlConstantExpression OnStringConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.String, childValues, element.Location);
        }

        private static CsdlConstantExpression OnBinaryConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Binary, childValues, element.Location);
        }

        private static CsdlConstantExpression OnFloatConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Floating, childValues, element.Location);
        }

        private static CsdlConstantExpression OnGuidConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Guid, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDecimalConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Decimal, childValues, element.Location);
        }

        private static CsdlConstantExpression OnBoolConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Boolean, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDurationConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Duration, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDateConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Date, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDateTimeOffsetConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.DateTimeOffset, childValues, element.Location);
        }

        private static CsdlConstantExpression OnTimeOfDayConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.TimeOfDay, childValues, element.Location);
        }

        private static CsdlConstantExpression OnNullConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Null, childValues, element.Location);
        }

        private static CsdlPathExpression OnPathExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlPathExpression(text != null ? text.TextValue : string.Empty, element.Location);
        }

        private static CsdlPropertyPathExpression OnPropertyPathExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlPropertyPathExpression(text != null ? text.TextValue : string.Empty, element.Location);
        }

        private static CsdlNavigationPropertyPathExpression OnNavigationPropertyPathExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlNavigationPropertyPathExpression(text != null ? text.TextValue : string.Empty, element.Location);
        }

        private CsdlLabeledExpressionReferenceExpression OnLabeledElementReferenceExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            return new CsdlLabeledExpressionReferenceExpression(name, element.Location);
        }

        private CsdlEnumMemberExpression OnEnumMemberExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string enumMemberPath = this.RequiredEnumMemberPath(childValues.FirstText);
            return new CsdlEnumMemberExpression(enumMemberPath, element.Location);
        }

        private CsdlExpressionBase OnIfExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 3)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidIfExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands);
            }

            return new CsdlIfExpression(
                expressions.ElementAtOrDefault(0),
                expressions.ElementAtOrDefault(1),
                expressions.ElementAtOrDefault(2),
                element.Location);
        }

        private CsdlExpressionBase OnCastExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);

            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 1)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidCastExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidCastExpressionIncorrectNumberOfOperands);
            }

            return new CsdlCastExpression(type, expressions.ElementAtOrDefault(0), element.Location);
        }

        private CsdlExpressionBase OnIsTypeExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);

            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 1)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidIsTypeExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidIsTypeExpressionIncorrectNumberOfOperands);
            }

            return new CsdlIsTypeExpression(type, expressions.ElementAtOrDefault(0), element.Location);
        }

        private CsdlTypeDefinition OnTypeDefinitionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string underlyingTypeName = RequiredType(CsdlConstants.Attribute_UnderlyingType);

            return new CsdlTypeDefinition(name, underlyingTypeName, element.Location);
        }

        private CsdlExpressionBase ParseAnnotationExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
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

            string propertyPathValue = Optional(CsdlConstants.Attribute_PropertyPath);
            if (propertyPathValue != null)
            {
                return new CsdlPropertyPathExpression(propertyPathValue, element.Location);
            }

            string navigationPropertyPathValue = Optional(CsdlConstants.Attribute_NavigationPropertyPath);
            if (navigationPropertyPathValue != null)
            {
                return new CsdlNavigationPropertyPathExpression(navigationPropertyPathValue, element.Location);
            }

            string enumMemberValue = Optional(CsdlConstants.Attribute_EnumMember);
            if (enumMemberValue != null)
            {
                return new CsdlEnumMemberExpression(this.ValidateEnumMembersPath(enumMemberValue), element.Location);
            }

            string annotationPath = Optional(CsdlConstants.Attribute_AnnotationPath);
            if (annotationPath != null)
            {
                return new CsdlAnnotationPathExpression(annotationPath, element.Location);
            }

            EdmValueKind kind;

            string value = Optional(CsdlConstants.Attribute_String);
            if (value != null)
            {
                kind = EdmValueKind.String;
            }
            else
            {
                value = Optional(CsdlConstants.Attribute_Bool);
                if (value != null)
                {
                    kind = EdmValueKind.Boolean;
                }
                else
                {
                    value = Optional(CsdlConstants.Attribute_Int);
                    if (value != null)
                    {
                        kind = EdmValueKind.Integer;
                    }
                    else
                    {
                        value = Optional(CsdlConstants.Attribute_Float);
                        if (value != null)
                        {
                            kind = EdmValueKind.Floating;
                        }
                        else
                        {
                            value = Optional(CsdlConstants.Attribute_DateTimeOffset);
                            if (value != null)
                            {
                                kind = EdmValueKind.DateTimeOffset;
                            }
                            else
                            {
                                value = Optional(CsdlConstants.Attribute_Duration);
                                if (value != null)
                                {
                                    kind = EdmValueKind.Duration;
                                }
                                else
                                {
                                    value = Optional(CsdlConstants.Attribute_Decimal);
                                    if (value != null)
                                    {
                                        kind = EdmValueKind.Decimal;
                                    }
                                    else
                                    {
                                        value = Optional(CsdlConstants.Attribute_Binary);
                                        if (value != null)
                                        {
                                            kind = EdmValueKind.Binary;
                                        }
                                        else
                                        {
                                            value = Optional(CsdlConstants.Attribute_Guid);
                                            if (value != null)
                                            {
                                                kind = EdmValueKind.Guid;
                                            }
                                            else
                                            {
                                                value = Optional(CsdlConstants.Attribute_Date);
                                                if (value != null)
                                                {
                                                    kind = EdmValueKind.Date;
                                                }
                                                else
                                                {
                                                    value = Optional(CsdlConstants.Attribute_TimeOfDay);
                                                    if (value != null)
                                                    {
                                                        kind = EdmValueKind.TimeOfDay;
                                                    }
                                                    else
                                                    {
                                                        //// Annotation expressions are always optional.
                                                        return null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new CsdlConstantExpression(kind, value, element.Location);
        }

        private CsdlNamedTypeReference ParseNamedTypeReference(string typeName, bool isNullable, CsdlLocation parentLocation)
        {
            bool isUnbounded;
            int? maxLength;
            bool? unicode;
            int? precision;
            int? scale;
            int? srid;

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
                    return new CsdlPrimitiveTypeReference(kind, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Binary:
                    this.ParseBinaryFacets(out isUnbounded, out maxLength);
                    return new CsdlBinaryTypeReference(isUnbounded, maxLength, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    this.ParseTemporalFacets(out precision);
                    return new CsdlTemporalTypeReference(kind, precision, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Decimal:
                    this.ParseDecimalFacets(out precision, out scale);
                    return new CsdlDecimalTypeReference(precision, scale, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.String:
                    this.ParseStringFacets(out isUnbounded, out maxLength, out unicode);
                    return new CsdlStringTypeReference(isUnbounded, maxLength, unicode, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    this.ParseSpatialFacets(out srid, CsdlConstants.Default_SpatialGeographySrid);
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    this.ParseSpatialFacets(out srid, CsdlConstants.Default_SpatialGeometrySrid);
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.None:
                    if (string.Equals(typeName, CsdlConstants.TypeName_Untyped, StringComparison.Ordinal))
                    {
                        return new CsdlUntypedTypeReference(typeName, parentLocation);
                    }

                    break;
            }

            this.ParseTypeDefinitionFacets(out isUnbounded, out maxLength, out unicode, out precision, out scale, out srid);
            return new CsdlNamedTypeReference(isUnbounded, maxLength, unicode, precision, scale, srid, typeName, isNullable, parentLocation);
        }

        private CsdlTypeReference ParseTypeReference(string typeString, XmlElementValueCollection childValues, CsdlLocation parentLocation, Optionality typeInfoOptionality)
        {
            bool isNullable = OptionalBoolean(CsdlConstants.Attribute_Nullable) ?? CsdlConstants.Default_Nullable;

            CsdlTypeReference elementType = null;
            if (typeString != null)
            {
                string[] typeInformation = typeString.Split(new char[] { '(', ')' });
                string typeName = typeInformation[0];
                switch (typeName)
                {
                    case CsdlConstants.Value_Collection:
                        {
                            string elementTypeName = typeInformation.Count() > 1 ? typeInformation[1] : typeString;
                            elementType = new CsdlExpressionTypeReference(
                                          new CsdlCollectionType(
                                          this.ParseNamedTypeReference(elementTypeName, isNullable, parentLocation), parentLocation), isNullable, parentLocation);
                        }

                        break;
                    case CsdlConstants.Value_Ref:
                        {
                            string elementTypeName = typeInformation.Count() > 1 ? typeInformation[1] : typeString;
                            elementType = new CsdlExpressionTypeReference(
                                          new CsdlEntityReferenceType(
                                          this.ParseNamedTypeReference(elementTypeName, isNullable, parentLocation), parentLocation), CsdlConstants.Default_Nullable, parentLocation);
                        }

                        break;
                    default:
                        elementType = this.ParseNamedTypeReference(typeName, isNullable, parentLocation);
                        break;
                }
            }
            else if (childValues != null)
            {
                elementType = childValues.ValuesOfType<CsdlTypeReference>().FirstOrDefault();
            }

            if (elementType == null && typeInfoOptionality == Optionality.Required)
            {
                if (childValues != null)
                {
                    // If childValues is null, then it is the case when a required type attribute was expected.
                    // In this case, we do not report the error as it should already be reported by EdmXmlDocumentParser.RequiredType method.
                    this.ReportError(parentLocation, EdmErrorCode.MissingType, Edm.Strings.CsdlParser_MissingTypeAttributeOrElement);
                }
                else
                {
                    Debug.Assert(this.Errors.Count() > 0, "There should be an error reported for the missing required type attribute.");
                }
                elementType = new CsdlNamedTypeReference(String.Empty, isNullable, parentLocation);
            }

            return elementType;
        }

        private CsdlNamedElement OnNavigationPropertyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);

            string typeName = RequiredType(CsdlConstants.Attribute_Type);
            bool? isNullable = OptionalBoolean(CsdlConstants.Attribute_Nullable);
            string partner = Optional(CsdlConstants.Attribute_Partner);

            bool? containsTarget = OptionalBoolean(CsdlConstants.Attribute_ContainsTarget);
            CsdlOnDelete onDelete = childValues.ValuesOfType<CsdlOnDelete>().FirstOrDefault();
            IEnumerable<CsdlReferentialConstraint> referentialConstraints = childValues.ValuesOfType<CsdlReferentialConstraint>().ToList();

            return new CsdlNavigationProperty(name, typeName, isNullable, partner, containsTarget ?? false, onDelete, referentialConstraints, Documentation(childValues), element.Location);
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
            string underlyingType = OptionalType(CsdlConstants.Attribute_UnderlyingType);
            bool? isFlags = OptionalBoolean(CsdlConstants.Attribute_IsFlags);

            return new CsdlEnumType(name, underlyingType, isFlags ?? false, childValues.ValuesOfType<CsdlEnumMember>(), Documentation(childValues), element.Location);
        }

        private CsdlEnumMember OnEnumMemberElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            long? value = OptionalLong(CsdlConstants.Attribute_Value);

            return new CsdlEnumMember(name, value, Documentation(childValues), element.Location);
        }

        private CsdlOnDelete OnDeleteActionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            EdmOnDeleteAction action = RequiredOnDeleteAction(CsdlConstants.Attribute_Action);

            return new CsdlOnDelete(action, Documentation(childValues), element.Location);
        }

        private CsdlReferentialConstraint OnReferentialConstraintElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string propertyName = this.Required(CsdlConstants.Attribute_Property);
            string referencedPropertyName = this.Required(CsdlConstants.Attribute_ReferencedProperty);

            return new CsdlReferentialConstraint(propertyName, referencedPropertyName, Documentation(childValues), element.Location);
        }

        internal CsdlAction OnActionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            bool isBound = OptionalBoolean(CsdlConstants.Attribute_IsBound) ?? CsdlConstants.Default_IsBound;
            string entitySetPath = Optional(CsdlConstants.Attribute_EntitySetPath);

            IEnumerable<CsdlOperationParameter> parameters = childValues.ValuesOfType<CsdlOperationParameter>();

            CsdlOperationReturnType returnTypeElement = childValues.ValuesOfType<CsdlOperationReturnType>().FirstOrDefault();
            CsdlTypeReference returnType = returnTypeElement == null ? null : returnTypeElement.ReturnType;

            this.ReportOperationReadErrorsIfExist(entitySetPath, isBound, name);

            return new CsdlAction(name, parameters, returnType, isBound, entitySetPath, Documentation(childValues), element.Location);
        }

        internal CsdlFunction OnFunctionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            bool isBound = OptionalBoolean(CsdlConstants.Attribute_IsBound) ?? CsdlConstants.Default_IsBound;
            string entitySetPath = Optional(CsdlConstants.Attribute_EntitySetPath);
            bool isComposable = OptionalBoolean(CsdlConstants.Attribute_IsComposable) ?? CsdlConstants.Default_IsComposable;

            IEnumerable<CsdlOperationParameter> parameters = childValues.ValuesOfType<CsdlOperationParameter>();

            CsdlOperationReturnType returnTypeElement = childValues.ValuesOfType<CsdlOperationReturnType>().FirstOrDefault();
            CsdlTypeReference returnType = returnTypeElement == null ? null : returnTypeElement.ReturnType;

            this.ReportOperationReadErrorsIfExist(entitySetPath, isBound, name);

            return new CsdlFunction(name, parameters, returnType, isBound, entitySetPath, isComposable, Documentation(childValues), element.Location);
        }

        private void ReportOperationReadErrorsIfExist(string entitySetPath, bool isBound, string name)
        {
            if (entitySetPath != null && !isBound)
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEntitySetPath, Edm.Strings.CsdlParser_InvalidEntitySetPathWithUnboundAction(CsdlConstants.Element_Action, name));
            }
        }

        private CsdlOperationParameter OnParameterElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            string defaultValue = null;
            bool isOptional = false;

            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);

            // TODO (Issue #855): handle out-of-line annotations
            XmlElementValue optionalAnnotationValue = childValues.Where(c =>
                c is XmlElementValue<CsdlAnnotation> &&
                    (c.ValueAs<CsdlAnnotation>().Term == CoreVocabularyModel.OptionalParameterTerm.ShortQualifiedName() ||
                     c.ValueAs<CsdlAnnotation>().Term == CoreVocabularyModel.OptionalParameterTerm.FullName())
            ).FirstOrDefault();

            if (optionalAnnotationValue != null)
            {
                isOptional = true;
                CsdlRecordExpression optionalValueExpression = optionalAnnotationValue.ValueAs<CsdlAnnotation>().Expression as CsdlRecordExpression;
                if (optionalValueExpression != null)
                {
                    foreach (CsdlPropertyValue property in optionalValueExpression.PropertyValues)
                    {
                        CsdlConstantExpression propertyValue = property.Expression as CsdlConstantExpression;
                        if (propertyValue != null)
                        {
                            if (property.Property == CsdlConstants.Attribute_DefaultValue)
                            {
                                defaultValue = propertyValue.Value;
                            }
                        }
                    }
                }

                childValues.Remove(optionalAnnotationValue);
            }

            return new CsdlOperationParameter(name, type, Documentation(childValues), element.Location, isOptional, defaultValue);
        }

        private CsdlActionImport OnActionImportElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string qualifiedActionName = RequiredQualifiedName(CsdlConstants.Attribute_Action);
            string entitySet = Optional(CsdlConstants.Attribute_EntitySet);

            return new CsdlActionImport(name, qualifiedActionName, entitySet, Documentation(childValues), element.Location);
        }

        private CsdlFunctionImport OnFunctionImportElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string qualifiedActionName = RequiredQualifiedName(CsdlConstants.Attribute_Function);
            string entitySet = Optional(CsdlConstants.Attribute_EntitySet);
            bool includeInServiceDocument = OptionalBoolean(CsdlConstants.Attribute_IncludeInServiceDocument) ?? CsdlConstants.Default_IncludeInServiceDocument;

            return new CsdlFunctionImport(name, qualifiedActionName, entitySet, includeInServiceDocument, Documentation(childValues), element.Location);
        }

        private CsdlOperationParameter OnFunctionImportParameterElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string typeName = RequiredType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, null, element.Location, Optionality.Required);
            return new CsdlOperationParameter(name, type, Documentation(childValues), element.Location);
        }

        private CsdlTypeReference OnEntityReferenceTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = RequiredType(CsdlConstants.Attribute_Type);
            return new CsdlExpressionTypeReference(new CsdlEntityReferenceType(this.ParseTypeReference(typeName, null, element.Location, Optionality.Required), element.Location), CsdlConstants.Default_Nullable, element.Location);
        }

        private CsdlTypeReference OnTypeRefElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = RequiredType(CsdlConstants.Attribute_Type);
            return this.ParseTypeReference(typeName, null, element.Location, Optionality.Required);
        }

        private CsdlTypeReference OnCollectionTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string elementTypeName = OptionalType(CsdlConstants.Attribute_ElementType);
            CsdlTypeReference elementType = this.ParseTypeReference(elementTypeName, childValues, element.Location, Optionality.Required);

            return new CsdlExpressionTypeReference(new CsdlCollectionType(elementType, element.Location), elementType.IsNullable, element.Location);
        }

        private CsdlOperationReturnType OnReturnTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = RequiredType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);
            return new CsdlOperationReturnType(type, element.Location);
        }

        private CsdlEntityContainer OnEntityContainerElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string extends = Optional(CsdlConstants.Attribute_Extends);

            if (entityContainerCount++ > 0)
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.MetadataDocumentCannotHaveMoreThanOneEntityContainer, Edm.Strings.CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer);
            }

            return new CsdlEntityContainer(
                name,
                extends,
                childValues.ValuesOfType<CsdlEntitySet>(),
                childValues.ValuesOfType<CsdlSingleton>(),
                childValues.ValuesOfType<CsdlOperationImport>(),
                Documentation(childValues),
                element.Location);
        }

        private CsdlEntitySet OnEntitySetElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string entityType = RequiredQualifiedName(CsdlConstants.Attribute_EntityType);
            bool? includeInServiceDocument = OptionalBoolean(CsdlConstants.Attribute_IncludeInServiceDocument);

            if (includeInServiceDocument == null)
            {
                return new CsdlEntitySet(name, entityType, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), Documentation(childValues), element.Location);
            }
            else
            {
                return new CsdlEntitySet(name, entityType, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), Documentation(childValues), element.Location, (bool)includeInServiceDocument);
            }
        }

        private CsdlSingleton OnSingletonElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string type = RequiredQualifiedName(CsdlConstants.Attribute_Type);

            return new CsdlSingleton(name, type, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), Documentation(childValues), element.Location);
        }

        private CsdlNavigationPropertyBinding OnNavigationPropertyBindingElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string path = Required(CsdlConstants.Attribute_Path);
            string entitySet = Required(CsdlConstants.Attribute_Target);

            return new CsdlNavigationPropertyBinding(path, entitySet, Documentation(childValues), element.Location);
        }

        private void ParseMaxLength(out bool Unbounded, out int? maxLength)
        {
            string max = Optional(CsdlConstants.Attribute_MaxLength);
            if (max == null)
            {
                Unbounded = false;
                maxLength = null;
            }
            else if (max.EqualsOrdinalIgnoreCase(CsdlConstants.Value_Max))
            {
                Unbounded = true;
                maxLength = null;
            }
            else
            {
                Unbounded = false;
                maxLength = OptionalMaxLength(CsdlConstants.Attribute_MaxLength);
            }
        }

        private void ParseBinaryFacets(out bool Unbounded, out int? maxLength)
        {
            this.ParseMaxLength(out Unbounded, out maxLength);
        }

        private void ParseDecimalFacets(out int? precision, out int? scale)
        {
            precision = OptionalInteger(CsdlConstants.Attribute_Precision);
            scale = OptionalScale(CsdlConstants.Attribute_Scale);
        }

        private void ParseStringFacets(out bool Unbounded, out int? maxLength, out bool? unicode)
        {
            this.ParseMaxLength(out Unbounded, out maxLength);
            unicode = OptionalBoolean(CsdlConstants.Attribute_Unicode) ?? CsdlConstants.Default_IsUnicode;
        }

        private void ParseTemporalFacets(out int? precision)
        {
            precision = OptionalInteger(CsdlConstants.Attribute_Precision) ?? CsdlConstants.Default_TemporalPrecision;
        }

        private void ParseSpatialFacets(out int? srid, int defaultSrid)
        {
            srid = OptionalSrid(CsdlConstants.Attribute_Srid, defaultSrid);
        }

        private void ParseTypeDefinitionFacets(
            out bool isUnbounded,
            out int? maxLength,
            out bool? unicode,
            out int? precision,
            out int? scale,
            out int? srid)
        {
            this.ParseMaxLength(out isUnbounded, out maxLength);
            unicode = OptionalBoolean(CsdlConstants.Attribute_Unicode);
            precision = OptionalInteger(CsdlConstants.Attribute_Precision);
            scale = OptionalScale(CsdlConstants.Attribute_Scale);
            srid = OptionalSrid(CsdlConstants.Attribute_Srid, CsdlConstants.Default_UnspecifiedSrid);
        }

        private enum Optionality
        {
            Optional,
            Required
        }
    }
}
