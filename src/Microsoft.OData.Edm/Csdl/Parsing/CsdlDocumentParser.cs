//---------------------------------------------------------------------
// <copyright file="CsdlDocumentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Parsing.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// CSDL document (Schema) parser.
    /// </summary>
    internal class CsdlDocumentParser : CsdlDocumentParserBase<CsdlSchema>
    {
        private Version artifactVersion;
        private int entityContainerCount;

        internal CsdlDocumentParser(string documentPath, XmlReader reader)
            : base(documentPath, reader)
        {
            entityContainerCount = 0;
        }

        protected override bool CheckAnnotationNamespace => true;

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

        private XmlElementParser<CsdlSchema> CreateRootElementParser()
        {
            // There is recursion in the grammar between CollectionType, ReturnType, and Property within RowType.
            // This requires breaking up the parser construction into pieces and then weaving them together with AddChildParser.
            var referenceTypeParser =
                //// <ReferenceType/>
                CsdlElement<CsdlTypeReference>(CsdlConstants.Element_ReferenceType, this.OnEntityReferenceTypeElement);

            var collectionTypeParser =
                //// <CollectionType>
                CsdlElement<CsdlTypeReference>(CsdlConstants.Element_CollectionType, this.OnCollectionTypeElement,
                    //// <TypeRef/>
                    CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement),
                    //// <ReferenceType/>
                    referenceTypeParser);
            //// </CollectionType>

            var nominalTypePropertyElementParser =
                //// <Property/>
                CsdlElement<CsdlProperty>(CsdlConstants.Element_Property, this.OnPropertyElement);

            var annotationParser = CreateAnnotationParser();

            nominalTypePropertyElementParser.AddChildParser(annotationParser);

            collectionTypeParser.AddChildParser(collectionTypeParser);

            var rootElementParser =
            //// <Schema>
            CsdlElement<CsdlSchema>(CsdlConstants.Element_Schema, this.OnSchemaElement,
                //// <ComplexType>
                CsdlElement<CsdlComplexType>(CsdlConstants.Element_ComplexType, this.OnComplexTypeElement,
                    //// <Property />
                    nominalTypePropertyElementParser,
                    //// <NavigationProperty>
                    CsdlElement<CsdlNamedElement>(CsdlConstants.Element_NavigationProperty, this.OnNavigationPropertyElement,
                        //// <ReferentialConstraint/>
                        CsdlElement<CsdlReferentialConstraint>(CsdlConstants.Element_ReferentialConstraint, this.OnReferentialConstraintElement),
                        //// <OnDelete/>
                        CsdlElement<CsdlOnDelete>(CsdlConstants.Element_OnDelete, this.OnDeleteActionElement),
                        //// <Annotation/>
                        annotationParser),
                    //// </NavigationProperty>
                    
                    //// <Annotation/>
                    annotationParser),
                //// </ComplexType>

                //// <EntityType>
                CsdlElement<CsdlEntityType>(CsdlConstants.Element_EntityType, this.OnEntityTypeElement,
                    //// <Key>
                    CsdlElement<CsdlKey>(CsdlConstants.Element_Key, OnEntityKeyElement,
                        //// <PropertyRef/>
                        CsdlElement<CsdlPropertyReference>(CsdlConstants.Element_PropertyRef, this.OnPropertyRefElement)),
                    //// </Key>

                    //// <Property />
                    nominalTypePropertyElementParser,

                    //// <NavigationProperty>
                    CsdlElement<CsdlNamedElement>(CsdlConstants.Element_NavigationProperty, this.OnNavigationPropertyElement,
                        //// <ReferentialConstraint/>
                        CsdlElement<CsdlReferentialConstraint>(CsdlConstants.Element_ReferentialConstraint, this.OnReferentialConstraintElement),
                        //// <OnDelete/>
                        CsdlElement<CsdlOnDelete>(CsdlConstants.Element_OnDelete, this.OnDeleteActionElement),
                        //// <Annotation/>
                        annotationParser),
                    //// </NavigationProperty>

                    //// <Annotation/>
                    annotationParser),
                //// </EntityType>

                //// <EnumType>
                CsdlElement<CsdlEnumType>(CsdlConstants.Element_EnumType, this.OnEnumTypeElement,
                    //// <Member>
                    CsdlElement<CsdlEnumMember>(CsdlConstants.Element_Member, this.OnEnumMemberElement, annotationParser),
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
                    //// <Parameter>
                    CsdlElement<CsdlOperationParameter>(CsdlConstants.Element_Parameter, this.OnParameterElement,
                        //// <TypeRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement),
                        //// <CollectionType/>
                        collectionTypeParser,
                        //// <ReferenceType/>
                        referenceTypeParser,
                        //// <Annotation/>
                        annotationParser),
                        //// </Parameter)

                        //// <ReturnType>
                        CsdlElement<CsdlOperationReturn>(CsdlConstants.Element_ReturnType, this.OnReturnTypeElement,
                            //// <TypeRef/>
                            CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement),
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
                    //// <Parameter>
                    CsdlElement<CsdlOperationParameter>(CsdlConstants.Element_Parameter, this.OnParameterElement,
                        //// <TypeRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement),
                        //// <CollectionType/>
                        collectionTypeParser,
                        //// <ReferenceType/>
                        referenceTypeParser,
                        //// <Annotation/>
                        annotationParser),
                    //// </Parameter

                    //// <ReturnType>
                    CsdlElement<CsdlOperationReturn>(CsdlConstants.Element_ReturnType, this.OnReturnTypeElement,
                        //// <TypeRef/>
                        CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement),
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
                    CsdlElement<CsdlTypeReference>(CsdlConstants.Element_TypeRef, this.OnTypeRefElement),
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
                    //// <EntitySet>
                    CsdlElement<CsdlEntitySet>(CsdlConstants.Element_EntitySet, this.OnEntitySetElement,
                        //// <NavigationPropertyBinding/>
                        CsdlElement<CsdlNavigationPropertyBinding>(CsdlConstants.Element_NavigationPropertyBinding, this.OnNavigationPropertyBindingElement),
                        //// <Annotation/>
                        annotationParser),
                    //// </EntitySet>

                    //// <Singleton>
                    CsdlElement<CsdlSingleton>(CsdlConstants.Element_Singleton, this.OnSingletonElement,
                        //// <NavigationPropertyBinding/>
                        CsdlElement<CsdlNavigationPropertyBinding>(CsdlConstants.Element_NavigationPropertyBinding, this.OnNavigationPropertyBindingElement),
                        //// <Annotation/>
                        annotationParser),
                    //// </Singleton>

                    //// <Action Import
                    CsdlElement<CsdlActionImport>(CsdlConstants.Element_ActionImport, this.OnActionImportElement,
                        //// <Annotation/>
                        annotationParser),
                    ////</Actionmport

                    //// <Function Import
                    CsdlElement<CsdlOperationImport>(CsdlConstants.Element_FunctionImport, this.OnFunctionImportElement,

                        //// <Parameter />
                        CsdlElement<CsdlOperationParameter>(CsdlConstants.Element_Parameter, this.OnFunctionImportParameterElement,
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
                    element.Location);

            return result;
        }

        private CsdlComplexType OnComplexTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string baseType = OptionalQualifiedName(CsdlConstants.Attribute_BaseType);
            bool isOpen = OptionalBoolean(CsdlConstants.Attribute_OpenType) ?? CsdlConstants.Default_OpenType;
            bool isAbstract = OptionalBoolean(CsdlConstants.Attribute_Abstract) ?? CsdlConstants.Default_Abstract;

            return new CsdlComplexType(name, baseType, isAbstract, isOpen, childValues.ValuesOfType<CsdlProperty>(), childValues.ValuesOfType<CsdlNavigationProperty>(), element.Location);
        }

        private CsdlEntityType OnEntityTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string baseType = OptionalQualifiedName(CsdlConstants.Attribute_BaseType);
            bool isOpen = OptionalBoolean(CsdlConstants.Attribute_OpenType) ?? CsdlConstants.Default_OpenType;
            bool isAbstract = OptionalBoolean(CsdlConstants.Attribute_Abstract) ?? CsdlConstants.Default_Abstract;
            bool hasStream = OptionalBoolean(CsdlConstants.Attribute_HasStream) ?? CsdlConstants.Default_HasStream;

            CsdlKey key = childValues.ValuesOfType<CsdlKey>().FirstOrDefault();

            return new CsdlEntityType(name, baseType, isAbstract, isOpen, hasStream, key, childValues.ValuesOfType<CsdlProperty>(), childValues.ValuesOfType<CsdlNavigationProperty>(), element.Location);
        }

        private CsdlProperty OnPropertyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);
            string name = Required(CsdlConstants.Attribute_Name);
            string defaultValue = Optional(CsdlConstants.Attribute_DefaultValue);

            return new CsdlProperty(name, type, defaultValue, element.Location);
        }

        private CsdlTerm OnTermElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);
            string name = Required(CsdlConstants.Attribute_Name);
            string appliesTo = Optional(CsdlConstants.Attribute_AppliesTo);
            string defaultValue = Optional(CsdlConstants.Attribute_DefaultValue);

            return new CsdlTerm(name, type, appliesTo, defaultValue, element.Location);
        }

        private CsdlAnnotations OnAnnotationsElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string target = Required(CsdlConstants.Attribute_Target);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            IEnumerable<CsdlAnnotation> annotations = childValues.ValuesOfType<CsdlAnnotation>();

            return new CsdlAnnotations(annotations, target, qualifier);
        }

        private CsdlTypeDefinition OnTypeDefinitionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string underlyingTypeName = RequiredType(CsdlConstants.Attribute_UnderlyingType);

            return new CsdlTypeDefinition(name, underlyingTypeName, element.Location);
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

            return new CsdlNavigationProperty(name, typeName, isNullable, partner, containsTarget ?? false, onDelete, referentialConstraints, element.Location);
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

            return new CsdlEnumType(name, underlyingType, isFlags ?? false, childValues.ValuesOfType<CsdlEnumMember>(), element.Location);
        }

        private CsdlEnumMember OnEnumMemberElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            long? value = OptionalLong(CsdlConstants.Attribute_Value);

            return new CsdlEnumMember(name, value, element.Location);
        }

        private CsdlOnDelete OnDeleteActionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            EdmOnDeleteAction action = RequiredOnDeleteAction(CsdlConstants.Attribute_Action);

            return new CsdlOnDelete(action, element.Location);
        }

        private CsdlReferentialConstraint OnReferentialConstraintElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string propertyName = this.Required(CsdlConstants.Attribute_Property);
            string referencedPropertyName = this.Required(CsdlConstants.Attribute_ReferencedProperty);

            return new CsdlReferentialConstraint(propertyName, referencedPropertyName, element.Location);
        }

        internal CsdlAction OnActionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            bool isBound = OptionalBoolean(CsdlConstants.Attribute_IsBound) ?? CsdlConstants.Default_IsBound;
            string entitySetPath = Optional(CsdlConstants.Attribute_EntitySetPath);

            IEnumerable<CsdlOperationParameter> parameters = childValues.ValuesOfType<CsdlOperationParameter>();

            CsdlOperationReturn returnElement = childValues.ValuesOfType<CsdlOperationReturn>().FirstOrDefault();

            this.ReportOperationReadErrorsIfExist(entitySetPath, isBound, name);

            return new CsdlAction(name, parameters, returnElement, isBound, entitySetPath, element.Location);
        }

        internal CsdlFunction OnFunctionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            bool isBound = OptionalBoolean(CsdlConstants.Attribute_IsBound) ?? CsdlConstants.Default_IsBound;
            string entitySetPath = Optional(CsdlConstants.Attribute_EntitySetPath);
            bool isComposable = OptionalBoolean(CsdlConstants.Attribute_IsComposable) ?? CsdlConstants.Default_IsComposable;

            IEnumerable<CsdlOperationParameter> parameters = childValues.ValuesOfType<CsdlOperationParameter>();

            CsdlOperationReturn returnElement = childValues.ValuesOfType<CsdlOperationReturn>().FirstOrDefault();

            this.ReportOperationReadErrorsIfExist(entitySetPath, isBound, name);

            return new CsdlFunction(name, parameters, returnElement, isBound, entitySetPath, isComposable, element.Location);
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
                     c.ValueAs<CsdlAnnotation>().Term == CoreVocabularyModel.OptionalParameterTerm.FullName()))
                .FirstOrDefault();

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

            return new CsdlOperationParameter(name, type, element.Location, isOptional, defaultValue);
        }

        private CsdlActionImport OnActionImportElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string qualifiedActionName = RequiredQualifiedName(CsdlConstants.Attribute_Action);
            string entitySet = Optional(CsdlConstants.Attribute_EntitySet);

            return new CsdlActionImport(name, qualifiedActionName, entitySet, element.Location);
        }

        private CsdlFunctionImport OnFunctionImportElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string qualifiedActionName = RequiredQualifiedName(CsdlConstants.Attribute_Function);
            string entitySet = Optional(CsdlConstants.Attribute_EntitySet);
            bool includeInServiceDocument = OptionalBoolean(CsdlConstants.Attribute_IncludeInServiceDocument) ?? CsdlConstants.Default_IncludeInServiceDocument;

            return new CsdlFunctionImport(name, qualifiedActionName, entitySet, includeInServiceDocument, element.Location);
        }

        private CsdlOperationParameter OnFunctionImportParameterElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string typeName = RequiredType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, null, element.Location, Optionality.Required);
            return new CsdlOperationParameter(name, type, element.Location);
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

        private CsdlOperationReturn OnReturnTypeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = RequiredType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);
            return new CsdlOperationReturn(type, element.Location);
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
                element.Location);
        }

        private CsdlEntitySet OnEntitySetElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string entityType = RequiredQualifiedName(CsdlConstants.Attribute_EntityType);
            bool? includeInServiceDocument = OptionalBoolean(CsdlConstants.Attribute_IncludeInServiceDocument);

            if (includeInServiceDocument == null)
            {
                return new CsdlEntitySet(name, entityType, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), element.Location);
            }
            else
            {
                return new CsdlEntitySet(name, entityType, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), element.Location, (bool)includeInServiceDocument);
            }
        }

        private CsdlSingleton OnSingletonElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            string type = RequiredQualifiedName(CsdlConstants.Attribute_Type);

            return new CsdlSingleton(name, type, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), element.Location);
        }

        private CsdlNavigationPropertyBinding OnNavigationPropertyBindingElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string path = Required(CsdlConstants.Attribute_Path);
            string entitySet = Required(CsdlConstants.Attribute_Target);

            return new CsdlNavigationPropertyBinding(path, entitySet, element.Location);
        }
    }
}
