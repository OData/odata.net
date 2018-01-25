//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal class EdmModelCsdlSchemaWriter
    {
        protected XmlWriter xmlWriter;
        protected Version version;
        private readonly string edmxNamespace;
        private readonly VersioningDictionary<string, string> namespaceAliasMappings;
        private readonly IEdmModel model;

        internal EdmModelCsdlSchemaWriter(IEdmModel model, VersioningDictionary<string, string> namespaceAliasMappings, XmlWriter xmlWriter, Version edmVersion)
        {
            this.xmlWriter = xmlWriter;
            this.version = edmVersion;
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmVersion];
            this.model = model;
            this.namespaceAliasMappings = namespaceAliasMappings;
        }

        internal static string PathAsXml(IEnumerable<string> path)
        {
            return EdmUtil.JoinInternal("/", path);
        }

        internal void WriteReferenceElementHeader(IEdmReference reference)
        {
            // e.g. <edmx:Reference Uri="http://host/schema/VipCustomer.xml">
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Reference, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Uri, reference.Uri, EdmValueWriter.UriAsXml);
        }

        internal void WriteIncludeElement(IEdmInclude include)
        {
            // e.g. <edmx:Include Namespace="NS.Ref1" Alias="VPCT" />
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Include, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Namespace, include.Namespace, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Alias, include.Alias, EdmValueWriter.StringAsXml);
            this.xmlWriter.WriteEndElement();
        }

        internal void WriteIncludeAnnotationsElement(IEdmIncludeAnnotations includeAnnotations)
        {
            // e.g. <edmx:IncludeAnnotations ... />
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_IncludeAnnotations, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_TermNamespace, includeAnnotations.TermNamespace, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, includeAnnotations.Qualifier, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_TargetNamespace, includeAnnotations.TargetNamespace, EdmValueWriter.StringAsXml);
            this.xmlWriter.WriteEndElement();
        }

        internal void WriteTermElementHeader(IEdmTerm term, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Term);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, term.Name, EdmValueWriter.StringAsXml);
            if (inlineType && term.Type != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, term.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, term.DefaultValue, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_AppliesTo, term.AppliesTo, EdmValueWriter.StringAsXml);
        }

        internal void WriteComplexTypeElementHeader(IEdmComplexType complexType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ComplexType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, complexType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, complexType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteEnumTypeElementHeader(IEdmEnumType enumType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, enumType.Name, EdmValueWriter.StringAsXml);
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, enumType.UnderlyingType, this.TypeDefinitionAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsFlags, enumType.IsFlags, CsdlConstants.Default_IsFlags, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteEntityContainerElementHeader(IEdmEntityContainer container)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityContainer);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, container.Name, EdmValueWriter.StringAsXml);
            CsdlSemanticsEntityContainer tmp = container as CsdlSemanticsEntityContainer;
            CsdlEntityContainer csdlContainer = null;
            if (tmp != null && (csdlContainer = tmp.Element as CsdlEntityContainer) != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Extends, csdlContainer.Extends, EdmValueWriter.StringAsXml);
            }
        }

        internal void WriteEntitySetElementHeader(IEdmEntitySet entitySet)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntitySet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntityType, entitySet.EntityType().FullName(), EdmValueWriter.StringAsXml);
        }

        internal void WriteSingletonElementHeader(IEdmSingleton singleton)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Singleton);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, singleton.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, singleton.EntityType().FullName(), EdmValueWriter.StringAsXml);
        }

        internal void WriteNavigationPropertyBinding(IEdmNavigationSource navigationSource, IEdmNavigationPropertyBinding binding)
        {
            this.WriteNavigationPropertyBinding(binding);
        }

        internal void WriteEntityTypeElementHeader(IEdmEntityType entityType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entityType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, entityType.BaseEntityType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, entityType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, entityType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);

            // HasStream value should be inherited.  Only have it on base type is sufficient.
            bool writeHasStream = entityType.HasStream && (entityType.BaseEntityType() == null || (entityType.BaseEntityType() != null && !entityType.BaseEntityType().HasStream));
            this.WriteOptionalAttribute(CsdlConstants.Attribute_HasStream, writeHasStream, CsdlConstants.Default_HasStream, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteDelaredKeyPropertiesElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Key);
        }

        internal void WritePropertyRefElement(IEdmStructuralProperty property)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyRef);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteNavigationPropertyElementHeader(IEdmNavigationProperty member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationProperty);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml);

            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, member.Type, this.TypeReferenceAsXml);
            if (!member.Type.IsCollection() && member.Type.IsNullable != CsdlConstants.Default_Nullable)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Nullable, member.Type.IsNullable, EdmValueWriter.BooleanAsXml);
            }

            if (member.Partner != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Partner, member.GetPartnerPath().Path, EdmValueWriter.StringAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_ContainsTarget, member.ContainsTarget, CsdlConstants.Default_ContainsTarget, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteOperationActionElement(string elementName, EdmOnDeleteAction operationAction)
        {
            this.xmlWriter.WriteStartElement(elementName);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Action, operationAction.ToString(), EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteSchemaElementHeader(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string xmlNamespace = GetCsdlNamespace(this.version);
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Schema, xmlNamespace);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Namespace, schema.Namespace, string.Empty, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Alias, alias, EdmValueWriter.StringAsXml);
            if (mappings != null)
            {
                foreach (KeyValuePair<string, string> mapping in mappings)
                {
                    this.xmlWriter.WriteAttributeString(EdmConstants.XmlNamespacePrefix, mapping.Key, null, mapping.Value);
                }
            }
        }

        internal void WriteAnnotationsElementHeader(string annotationsTarget)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotations);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, annotationsTarget, EdmValueWriter.StringAsXml);
        }

        internal void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Property);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, property.DefaultValueString, EdmValueWriter.StringAsXml);
        }

        internal void WriteEnumMemberElementHeader(IEdmEnumMember member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Member);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml);
            bool? isExplicit = member.IsValueExplicit(this.model);
            if (!isExplicit.HasValue || isExplicit.Value)
            {
                this.xmlWriter.WriteAttributeString(CsdlConstants.Attribute_Value, EdmValueWriter.LongAsXml(member.Value.Value));
            }
        }

        internal void WriteNullableAttribute(IEdmTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteTypeDefinitionAttributes(IEdmTypeDefinitionReference reference)
        {
            IEdmTypeReference actualTypeReference = reference.AsActualTypeReference();

            if (actualTypeReference.IsBinary())
            {
                this.WriteBinaryTypeAttributes(actualTypeReference.AsBinary());
            }
            else if (actualTypeReference.IsString())
            {
                this.WriteStringTypeAttributes(actualTypeReference.AsString());
            }
            else if (actualTypeReference.IsTemporal())
            {
                this.WriteTemporalTypeAttributes(actualTypeReference.AsTemporal());
            }
            else if (actualTypeReference.IsDecimal())
            {
                this.WriteDecimalTypeAttributes(actualTypeReference.AsDecimal());
            }
            else if (actualTypeReference.IsSpatial())
            {
                this.WriteSpatialTypeAttributes(actualTypeReference.AsSpatial());
            }
        }

        internal void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }
            else
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
            }
        }

        internal void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Scale, reference.Scale, CsdlConstants.Default_Scale, ScaleAsXml);
        }

        internal void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference)
        {
            if (reference.IsGeography())
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeographySrid, SridAsXml);
            }
            else if (reference.IsGeometry())
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeometrySrid, SridAsXml);
            }
        }

        internal void WriteStringTypeAttributes(IEdmStringTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }
            else
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
            }

            if (reference.IsUnicode != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Unicode, reference.IsUnicode, CsdlConstants.Default_IsUnicode, EdmValueWriter.BooleanAsXml);
            }
        }

        internal void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision, EdmValueWriter.IntAsXml);
            }
        }

        internal void WriteReferentialConstraintElementHeader(IEdmNavigationProperty constraint)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReferentialConstraint);
        }

        internal void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReferentialConstraint);

            // <EntityType Name="Product">
            //   ...
            //   <Property Name="CategoryID" Type="Edm.String" Nullable="false"/>
            //  <NavigationProperty Name="Category" Type="Self.Category" Nullable="false">
            //     <ReferentialConstraint Property="CategoryID" ReferencedProperty="ID" />
            //   </NavigationProperty>
            // </EntityType>
            // the above CategoryID is DependentProperty, ID is PrincipalProperty.
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, pair.DependentProperty.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_ReferencedProperty, pair.PrincipalProperty.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteAttributeString(annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(edmValue));
            }
        }

        internal void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteRaw(((IEdmStringValue)edmValue).Value);
            }
        }

        internal void WriteActionElementHeader(IEdmAction action)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Action);
            this.WriteOperationElementAttributes(action);
        }

        internal void WriteFunctionElementHeader(IEdmFunction function)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Function);
            this.WriteOperationElementAttributes(function);

            if (function.IsComposable)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_IsComposable, function.IsComposable, EdmValueWriter.BooleanAsXml);
            }
        }

        internal void WriteReturnTypeElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReturnType);
        }

        internal void WriteTypeAttribute(IEdmTypeReference typeReference)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, typeReference, this.TypeReferenceAsXml);
        }

        internal void WriteActionImportElementHeader(IEdmActionImport actionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ActionImport);
            this.WriteOperationImportAttributes(actionImport, CsdlConstants.Attribute_Action);
        }

        internal void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_FunctionImport);
            this.WriteOperationImportAttributes(functionImport, CsdlConstants.Attribute_Function);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_IncludeInServiceDocument, functionImport.IncludeInServiceDocument, CsdlConstants.Default_IncludeInServiceDocument, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteOperationParameterElementHeader(IEdmOperationParameter parameter, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Parameter);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, parameter.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, parameter.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteOperationParameterEndElement(IEdmOperationParameter parameter)
        {
            IEdmOptionalParameter optionalParameter = parameter as IEdmOptionalParameter;
            if (optionalParameter != null && !(optionalParameter.VocabularyAnnotations(this.model).Any(a => a.Term == CoreVocabularyModel.OptionalParameterTerm)))
            {
                string defaultValue = optionalParameter.DefaultValueString;
                EdmRecordExpression optionalValue = new EdmRecordExpression();

                this.WriteVocabularyAnnotationElementHeader(new EdmVocabularyAnnotation(parameter, CoreVocabularyModel.OptionalParameterTerm, optionalValue), false);
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    EdmPropertyConstructor property = new EdmPropertyConstructor(CsdlConstants.Attribute_DefaultValue, new EdmStringConstant(defaultValue));
                    this.WriteRecordExpressionElementHeader(optionalValue);
                    this.WritePropertyValueElementHeader(property, true);
                    this.WriteEndElement();
                    this.WriteEndElement();
                }

                this.WriteEndElement();
            }

            this.WriteEndElement();
        }

        internal void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_CollectionType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_ElementType, collectionType.ElementType, this.TypeReferenceAsXml);
            }
        }

        internal void WriteInlineExpression(IEdmExpression expression)
        {
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.BinaryConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Binary, ((IEdmBinaryConstantExpression)expression).Value, EdmValueWriter.BinaryAsXml);
                    break;
                case EdmExpressionKind.BooleanConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Bool, ((IEdmBooleanConstantExpression)expression).Value, EdmValueWriter.BooleanAsXml);
                    break;
                case EdmExpressionKind.DateTimeOffsetConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_DateTimeOffset, ((IEdmDateTimeOffsetConstantExpression)expression).Value, EdmValueWriter.DateTimeOffsetAsXml);
                    break;
                case EdmExpressionKind.DecimalConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Decimal, ((IEdmDecimalConstantExpression)expression).Value, EdmValueWriter.DecimalAsXml);
                    break;
                case EdmExpressionKind.FloatingConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Float, ((IEdmFloatingConstantExpression)expression).Value, EdmValueWriter.FloatAsXml);
                    break;
                case EdmExpressionKind.GuidConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Guid, ((IEdmGuidConstantExpression)expression).Value, EdmValueWriter.GuidAsXml);
                    break;
                case EdmExpressionKind.IntegerConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Int, ((IEdmIntegerConstantExpression)expression).Value, EdmValueWriter.LongAsXml);
                    break;
                case EdmExpressionKind.Path:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Path, ((IEdmPathExpression)expression).PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.PropertyPath:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_PropertyPath, ((IEdmPathExpression)expression).PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.NavigationPropertyPath:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_NavigationPropertyPath, ((IEdmPathExpression)expression).PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.StringConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_String, ((IEdmStringConstantExpression)expression).Value, EdmValueWriter.StringAsXml);
                    break;
                case EdmExpressionKind.DurationConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Duration, ((IEdmDurationConstantExpression)expression).Value, EdmValueWriter.DurationAsXml);
                    break;
                case EdmExpressionKind.DateConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Date, ((IEdmDateConstantExpression)expression).Value, EdmValueWriter.DateAsXml);
                    break;
                case EdmExpressionKind.TimeOfDayConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_TimeOfDay, ((IEdmTimeOfDayConstantExpression)expression).Value, EdmValueWriter.TimeOfDayAsXml);
                    break;
                default:
                    Debug.Assert(false, "Attempted to inline an expression that was not one of the expected inlineable types.");
                    break;
            }
        }

        internal void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotation);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(annotation.Value);
            }
        }

        internal void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyValue);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, value.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(value.Value);
            }
        }

        internal void WriteRecordExpressionElementHeader(IEdmRecordExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Record);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Type, expression.DeclaredType, this.TypeReferenceAsXml);
        }

        internal void WritePropertyConstructorElementHeader(IEdmPropertyConstructor constructor, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyValue);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, constructor.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(constructor.Value);
            }
        }

        internal void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_String);

            this.xmlWriter.WriteString(EdmValueWriter.StringAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Binary);
            this.xmlWriter.WriteString(EdmValueWriter.BinaryAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Bool);
            this.xmlWriter.WriteString(EdmValueWriter.BooleanAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteNullConstantExpressionElement(IEdmNullExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Null);
            this.WriteEndElement();
        }

        internal void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Date);
            this.xmlWriter.WriteString(EdmValueWriter.DateAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_DateTimeOffset);
            this.xmlWriter.WriteString(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Duration);
            this.xmlWriter.WriteString(EdmValueWriter.DurationAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Decimal);
            this.xmlWriter.WriteString(EdmValueWriter.DecimalAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Float);
            this.xmlWriter.WriteString(EdmValueWriter.FloatAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Apply);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Function, expression.AppliedFunction, this.FunctionAsXml);
        }

        internal void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Guid);
            this.xmlWriter.WriteString(EdmValueWriter.GuidAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Int);
            this.xmlWriter.WriteString(EdmValueWriter.LongAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WritePathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Path);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal void WritePropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationPropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal void WriteIfExpressionElementHeader(IEdmIfExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_If);
        }

        internal void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Collection);
        }

        internal void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_LabeledElement);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, labeledElement.Name, EdmValueWriter.StringAsXml);
        }

        internal void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TimeOfDay);
            this.xmlWriter.WriteString(EdmValueWriter.TimeOfDayAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_IsType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Cast);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumMember);
            this.xmlWriter.WriteString(EnumMemberAsXml(expression.EnumMembers));
            this.WriteEndElement();
        }

        internal void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TypeDefinition);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, typeDefinition.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, typeDefinition.UnderlyingType, this.TypeDefinitionAsXml);
        }

        internal void WriteEndElement()
        {
            this.xmlWriter.WriteEndElement();
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, T defaultValue, Func<T, string> toXml)
        {
            if (!value.Equals(defaultValue))
            {
                this.xmlWriter.WriteAttributeString(attribute, toXml(value));
            }
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, Func<T, string> toXml)
        {
            if (value != null)
            {
                this.xmlWriter.WriteAttributeString(attribute, toXml(value));
            }
        }

        internal void WriteRequiredAttribute<T>(string attribute, T value, Func<T, string> toXml)
        {
            this.xmlWriter.WriteAttributeString(attribute, toXml(value));
        }

        private void WriteOperationElementAttributes(IEdmOperation operation)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, operation.Name, EdmValueWriter.StringAsXml);

            if (operation.IsBound)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_IsBound, operation.IsBound, EdmValueWriter.BooleanAsXml);
            }

            if (operation.EntitySetPath != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySetPath, operation.EntitySetPath.PathSegments, PathAsXml);
            }
        }

        private void WriteNavigationPropertyBinding(IEdmNavigationPropertyBinding binding)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationPropertyBinding);

            this.WriteRequiredAttribute(CsdlConstants.Attribute_Path, binding.Path.Path, EdmValueWriter.StringAsXml);

            // TODO: handle container names, etc.
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, binding.Target.Name, EdmValueWriter.StringAsXml);

            this.xmlWriter.WriteEndElement();
        }

        private static string EnumMemberAsXml(IEnumerable<IEdmEnumMember> members)
        {
            string enumTypeName = members.First().DeclaringType.FullName();
            List<string> memberList = new List<string>();
            foreach (var member in members)
            {
                memberList.Add(enumTypeName + "/" + member.Name);
            }

            return string.Join(" ", memberList.ToArray());
        }

        private static string SridAsXml(int? i)
        {
            return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_SridVariable;
        }

        private static string ScaleAsXml(int? i)
        {
            return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_ScaleVariable;
        }

        private static string GetCsdlNamespace(Version edmVersion)
        {
            string[] @namespaces;
            if (CsdlConstants.SupportedVersions.TryGetValue(edmVersion, out @namespaces))
            {
                return @namespaces[0];
            }

            throw new InvalidOperationException(Strings.Serializer_UnknownEdmVersion);
        }

        private void WriteOperationImportAttributes(IEdmOperationImport operationImport, string operationAttributeName)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, operationImport.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(operationAttributeName, operationImport.Operation.FullName(), EdmValueWriter.StringAsXml);

            if (operationImport.EntitySet != null)
            {
                var pathExpression = operationImport.EntitySet as IEdmPathExpression;
                if (pathExpression != null)
                {
                    this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySet, pathExpression.PathSegments, PathAsXml);
                }
                else
                {
                    throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(operationImport.Name));
                }
            }
        }

        private string SerializationName(IEdmSchemaElement element)
        {
            if (this.namespaceAliasMappings != null)
            {
                string alias;
                if (this.namespaceAliasMappings.TryGetValue(element.Namespace, out alias))
                {
                    return alias + "." + element.Name;
                }
            }

            return element.FullName();
        }

        private string TypeReferenceAsXml(IEdmTypeReference type)
        {
            if (type.IsCollection())
            {
                IEdmCollectionTypeReference collectionReference = type.AsCollection();
                Debug.Assert(collectionReference.ElementType().Definition is IEdmSchemaElement, "Cannot inline parameter type if not a named element or collection of named elements");
                return CsdlConstants.Value_Collection + "(" + this.SerializationName((IEdmSchemaElement)collectionReference.ElementType().Definition) + ")";
            }
            else if (type.IsEntityReference())
            {
                return CsdlConstants.Value_Ref + "(" + this.SerializationName(type.AsEntityReference().EntityReferenceDefinition().EntityType) + ")";
            }

            Debug.Assert(type.Definition is IEdmSchemaElement, "Cannot inline parameter type if not a named element or collection of named elements");
            return this.SerializationName((IEdmSchemaElement)type.Definition);
        }

        private string TypeDefinitionAsXml(IEdmSchemaType type)
        {
            return this.SerializationName(type);
        }

        private string FunctionAsXml(IEdmOperation operation)
        {
            return this.SerializationName(operation);
        }

        private string TermAsXml(IEdmTerm term)
        {
            if (term == null)
            {
                return string.Empty;
            }

            return this.SerializationName(term);
        }
    }
}
