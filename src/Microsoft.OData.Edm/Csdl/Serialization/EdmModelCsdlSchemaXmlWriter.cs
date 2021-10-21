//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaXmlWriter.cs" company="Microsoft">
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
    /// <summary>
    /// OData Common Schema Definition Language (CSDL) XML writer
    /// </summary>
    internal class EdmModelCsdlSchemaXmlWriter : EdmModelCsdlSchemaWriter
    {
        protected XmlWriter xmlWriter;
        protected readonly string edmxNamespace;

        internal EdmModelCsdlSchemaXmlWriter(IEdmModel model, XmlWriter xmlWriter, Version edmVersion)
            : base(model, edmVersion)
        {
            this.xmlWriter = xmlWriter;
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmVersion];
        }

        internal override void WriteReferenceElementHeader(IEdmReference reference)
        {
            // e.g. <edmx:Reference Uri="http://host/schema/VipCustomer.xml">
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Reference, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Uri, reference.Uri, EdmValueWriter.UriAsXml);
        }

        internal override void WriteReferenceElementEnd(IEdmReference reference)
        {
            this.xmlWriter.WriteEndElement();
        }

        internal override void WritIncludeElementHeader(IEdmInclude include)
        {
            // e.g. <edmx:Include Namespace="NS.Ref1" Alias="VPCT" />
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Include, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Namespace, include.Namespace, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Alias, include.Alias, EdmValueWriter.StringAsXml);
        }

        internal override void WriteIncludeElementEnd(IEdmInclude include)
        {
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

        internal override void WriteTermElementHeader(IEdmTerm term, bool inlineType)
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

        internal override void WriteComplexTypeElementHeader(IEdmComplexType complexType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ComplexType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, complexType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, complexType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteEnumTypeElementHeader(IEdmEnumType enumType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, enumType.Name, EdmValueWriter.StringAsXml);
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, enumType.UnderlyingType, this.TypeDefinitionAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsFlags, enumType.IsFlags, CsdlConstants.Default_IsFlags, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteEnumTypeElementEnd(IEdmEnumType enumType)
        {
            this.WriteEndElement();
        }

        internal override void WriteEntityContainerElementHeader(IEdmEntityContainer container)
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

        internal override void WriteEntitySetElementHeader(IEdmEntitySet entitySet)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntitySet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntityType, entitySet.EntityType().FullName(), EdmValueWriter.StringAsXml);
        }

        internal override void WriteSingletonElementHeader(IEdmSingleton singleton)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Singleton);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, singleton.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, singleton.EntityType().FullName(), EdmValueWriter.StringAsXml);
        }

        internal override void WriteEntityTypeElementHeader(IEdmEntityType entityType)
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

        internal override void WriteDeclaredKeyPropertiesElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Key);
        }

        internal override void WritePropertyRefElement(IEdmStructuralProperty property)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyRef);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal override void WriteNavigationPropertyElementHeader(IEdmNavigationProperty property)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationProperty);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);

            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml);
            if (!property.Type.IsCollection() && property.Type.IsNullable != CsdlConstants.Default_Nullable)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Nullable, property.Type.IsNullable, EdmValueWriter.BooleanAsXml);
            }

            if (property.Partner != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Partner, property.GetPartnerPath()?.Path, EdmValueWriter.StringAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_ContainsTarget, property.ContainsTarget, CsdlConstants.Default_ContainsTarget, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteNavigationOnDeleteActionElement(EdmOnDeleteAction operationAction)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_OnDelete);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Action, operationAction.ToString(), EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal override void WriteSchemaElementHeader(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string xmlNamespace = GetCsdlNamespace(EdmVersion);
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

        internal override void WriteAnnotationsElementHeader(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotations);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, annotationsForTarget.Key, EdmValueWriter.StringAsXml);
        }

        internal override void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Property);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, property.DefaultValueString, EdmValueWriter.StringAsXml);
        }

        internal override void WriteEnumMemberElementHeader(IEdmEnumMember member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Member);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml);
            bool? isExplicit = member.IsValueExplicit(this.Model);
            if (!isExplicit.HasValue || isExplicit.Value)
            {
                this.xmlWriter.WriteAttributeString(CsdlConstants.Attribute_Value, EdmValueWriter.LongAsXml(member.Value.Value));
            }
        }

        internal override void WriteEnumMemberElementEnd(IEdmEnumMember member)
        {
            this.xmlWriter.WriteEndElement();
        }

        internal override void WriteNullableAttribute(IEdmTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteTypeDefinitionAttributes(IEdmTypeDefinitionReference reference)
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

        internal override void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference)
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

        internal override void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Scale, reference.Scale, CsdlConstants.Default_Scale, ScaleAsXml);
        }

        internal override void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference)
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

        internal override void WriteStringTypeAttributes(IEdmStringTypeReference reference)
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

        internal override void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision, EdmValueWriter.IntAsXml);
            }
        }

        internal override void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair)
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

        internal override void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteAttributeString(annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(edmValue));
            }
        }

        internal override void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteRaw(((IEdmStringValue)edmValue).Value);
            }
        }

        internal override void WriteActionElementHeader(IEdmAction action)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Action);
            this.WriteOperationElementAttributes(action);
        }

        internal override void WriteFunctionElementHeader(IEdmFunction function)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Function);
            this.WriteOperationElementAttributes(function);

            if (function.IsComposable)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_IsComposable, function.IsComposable, EdmValueWriter.BooleanAsXml);
            }
        }

        internal override void WriteReturnTypeElementHeader(IEdmOperationReturn operationReturn)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReturnType);
        }

        internal override void WriteTypeAttribute(IEdmTypeReference typeReference)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, typeReference, this.TypeReferenceAsXml);
        }

        internal override void WriteActionImportElementHeader(IEdmActionImport actionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ActionImport);
            this.WriteOperationImportAttributes(actionImport, CsdlConstants.Attribute_Action);
        }

        internal override void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_FunctionImport);
            this.WriteOperationImportAttributes(functionImport, CsdlConstants.Attribute_Function);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_IncludeInServiceDocument, functionImport.IncludeInServiceDocument, CsdlConstants.Default_IncludeInServiceDocument, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteOperationParameterElementHeader(IEdmOperationParameter parameter, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Parameter);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, parameter.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, parameter.Type, this.TypeReferenceAsXml);
            }
        }

        internal override void WriteOperationParameterEndElement(IEdmOperationParameter parameter)
        {
            IEdmOptionalParameter optionalParameter = parameter as IEdmOptionalParameter;
            if (optionalParameter != null && !(optionalParameter.VocabularyAnnotations(this.Model).Any(a => a.Term == CoreVocabularyModel.OptionalParameterTerm)))
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

        internal override void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_CollectionType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_ElementType, collectionType.ElementType, this.TypeReferenceAsXml);
            }
        }

        internal static bool IsUsingDefaultValue(IEdmVocabularyAnnotation annotation)
        {
            EdmVocabularyAnnotation edmAnnotation = annotation as EdmVocabularyAnnotation;
            if (edmAnnotation != null)
            {
                return edmAnnotation.UseDefault;
            }

            CsdlSemanticsVocabularyAnnotation csdlAnnotation = annotation as CsdlSemanticsVocabularyAnnotation;
            if (csdlAnnotation != null)
            {
                return csdlAnnotation.UseDefault;
            }

            return false;
        }

        internal override void WriteInlineExpression(IEdmExpression expression)
        {
            IEdmPathExpression pathExpression = expression as IEdmPathExpression;
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
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Path, pathExpression.PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.PropertyPath:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_PropertyPath, pathExpression.PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.NavigationPropertyPath:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_NavigationPropertyPath, pathExpression.PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.AnnotationPath:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_AnnotationPath, pathExpression.PathSegments, PathAsXml);
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

        internal override void WritePropertyConstructorElementEnd(IEdmPropertyConstructor constructor)
        {
            this.WriteEndElement();
        }

        internal override void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotation);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml);

            if (isInline && !IsUsingDefaultValue(annotation))
            {
                // in xml format, we can (should) skip writing the expression value if it matches the term default value.
                this.WriteInlineExpression(annotation.Value);
            }
        }

        internal override void WriteVocabularyAnnotationElementEnd(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            WriteEndElement();
        }

        internal override void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyValue);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, value.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(value.Value);
            }
        }

        internal override void WriteRecordExpressionElementHeader(IEdmRecordExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Record);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Type, expression.DeclaredType, this.TypeReferenceAsXml);
        }

        internal override void WritePropertyConstructorElementHeader(IEdmPropertyConstructor constructor, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyValue);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, constructor.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(constructor.Value);
            }
        }

        internal override void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_String);

            this.xmlWriter.WriteString(EdmValueWriter.StringAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Binary);
            this.xmlWriter.WriteString(EdmValueWriter.BinaryAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Bool);
            this.xmlWriter.WriteString(EdmValueWriter.BooleanAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteNullConstantExpressionElement(IEdmNullExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Null);
            this.WriteEndElement();
        }

        internal override void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Date);
            this.xmlWriter.WriteString(EdmValueWriter.DateAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_DateTimeOffset);
            this.xmlWriter.WriteString(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Duration);
            this.xmlWriter.WriteString(EdmValueWriter.DurationAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Decimal);
            this.xmlWriter.WriteString(EdmValueWriter.DecimalAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Float);
            this.xmlWriter.WriteString(EdmValueWriter.FloatAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Apply);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Function, expression.AppliedFunction, this.FunctionAsXml);
        }

        internal override void WriteFunctionApplicationElementEnd(IEdmApplyExpression expression)
        {
            this.WriteEndElement();
        }

        internal override void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Guid);
            this.xmlWriter.WriteString(EdmValueWriter.GuidAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Int);
            this.xmlWriter.WriteString(EdmValueWriter.LongAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WritePathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Path);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal override void WritePropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal override void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationPropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal override void WriteAnnotationPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_AnnotationPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal override void WriteIfExpressionElementHeader(IEdmIfExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_If);
        }

        internal override void WriteIfExpressionElementEnd(IEdmIfExpression expression)
        {
            this.WriteEndElement();
        }

        internal override void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Collection);
        }

        internal override void WriteCollectionExpressionElementEnd(IEdmCollectionExpression expression)
        {
            this.WriteEndElement();
        }

        internal override void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_LabeledElement);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, labeledElement.Name, EdmValueWriter.StringAsXml);
        }

        internal override void WriteLabeledExpressionReferenceExpression(IEdmLabeledExpressionReferenceExpression labeledExpressionReference)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_LabeledElementReference);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, labeledExpressionReference.ReferencedLabeledExpression.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal override void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TimeOfDay);
            this.xmlWriter.WriteString(EdmValueWriter.TimeOfDayAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal override void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_IsType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal override void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Cast);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal override void WriteCastExpressionElementEnd(IEdmCastExpression expression, bool inlineType)
        {
            this.WriteEndElement();
        }

        internal override void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumMember);
            this.xmlWriter.WriteString(EnumMemberAsXmlOrJson(expression.EnumMembers));
            this.WriteEndElement();
        }

        internal override void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TypeDefinition);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, typeDefinition.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, typeDefinition.UnderlyingType, this.TypeDefinitionAsXml);
        }

        internal override void WriteEndElement()
        {
            this.xmlWriter.WriteEndElement();
        }

        internal override void WriteArrayEndElement()
        {
            this.xmlWriter.WriteEndElement();
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, T defaultValue, Func<T, string> getStringFunc)
        {
            if (!value.Equals(defaultValue))
            {
                this.xmlWriter.WriteAttributeString(attribute, getStringFunc(value));
            }
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, Func<T, string> getStringFunc)
        {
            if (value != null)
            {
                this.xmlWriter.WriteAttributeString(attribute, getStringFunc(value));
            }
        }

        internal void WriteRequiredAttribute<T>(string attribute, T value, Func<T, string> toXml)
        {
            this.xmlWriter.WriteAttributeString(attribute, toXml(value));
        }

        internal override void WriteOperationElementAttributes(IEdmOperation operation)
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

        internal override void WriteNavigationPropertyBinding(IEdmNavigationPropertyBinding binding)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationPropertyBinding);

            this.WriteRequiredAttribute(CsdlConstants.Attribute_Path, binding.Path.Path, EdmValueWriter.StringAsXml);

            // TODO: handle container names, etc.
            IEdmContainedEntitySet containedEntitySet = binding.Target as IEdmContainedEntitySet;
            if (containedEntitySet != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, containedEntitySet.Path.Path, EdmValueWriter.StringAsXml);
            }
            else
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, binding.Target.Name, EdmValueWriter.StringAsXml);
            }

            this.xmlWriter.WriteEndElement();
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

            throw new InvalidOperationException(Strings.Serializer_UnknownEdmVersion(edmVersion.ToString()));
        }

        internal override void WriteOperationImportAttributes(IEdmOperationImport operationImport, string operationAttributeName)
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
            if (this.NamespaceAliasMappings != null)
            {
                string alias;
                if (this.NamespaceAliasMappings.TryGetValue(element.Namespace, out alias))
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
