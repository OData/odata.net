//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Internal;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Csdl.Internal.Serialization
{
    internal class EdmModelCsdlSchemaWriter
    {
        protected XmlWriter xmlWriter;
        protected Version version;
        private readonly VersioningDictionary<string, string> namespaceAliasMappings;
        private readonly IEdmModel model;

        internal EdmModelCsdlSchemaWriter(IEdmModel model, VersioningDictionary<string, string> namespaceAliasMappings, XmlWriter xmlWriter, Version edmVersion)
        {
            this.xmlWriter = xmlWriter;
            this.version = edmVersion;
            this.model = model;
            this.namespaceAliasMappings = namespaceAliasMappings;
        }

        internal void WriteValueTermElementHeader(IEdmValueTerm term, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ValueTerm);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, term.Name, EdmValueWriter.StringAsXml);
            if (inlineType && term.Type != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, term.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteAssociationElementHeader(IEdmNavigationProperty navigationProperty)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Association);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, this.model.GetAssociationName(navigationProperty), EdmValueWriter.StringAsXml);
        }

        internal void WriteAssociationSetElementHeader(IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_AssociationSet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, this.model.GetAssociationSetName(entitySet, navigationProperty), EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Association, this.model.GetAssociationFullName(navigationProperty), EdmValueWriter.StringAsXml);
        }

        internal void WriteComplexTypeElementHeader(IEdmComplexType complexType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ComplexType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, complexType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
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

        internal void WriteDocumentationElement(IEdmDocumentation documentation)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Documentation);
            if (documentation.Summary != null)
            {
                this.xmlWriter.WriteStartElement(CsdlConstants.Element_Summary);
                this.xmlWriter.WriteString(documentation.Summary);
                this.WriteEndElement();
            }

            if (documentation.Description != null)
            {
                this.xmlWriter.WriteStartElement(CsdlConstants.Element_LongDescription);
                this.xmlWriter.WriteString(documentation.Description);
                this.WriteEndElement();
            }

            this.WriteEndElement();
        }

        internal void WriteAssociationSetEndElementHeader(IEdmEntitySet entitySet, IEdmNavigationProperty property)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_End);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, this.model.GetAssociationEndName(property), EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntitySet, entitySet.Name, EdmValueWriter.StringAsXml);
        }

        internal void WriteAssociationEndElementHeader(IEdmNavigationProperty associationEnd)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_End);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, ((IEdmEntityType)associationEnd.DeclaringType).FullName(), EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, this.model.GetAssociationEndName(associationEnd), EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Multiplicity, associationEnd.Multiplicity(), MultiplicityAsXml);
        }

        internal void WriteEntityContainerElementHeader(IEdmEntityContainer container)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityContainer);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, container.Name, EdmValueWriter.StringAsXml);
        }

        internal void WriteEntitySetElementHeader(IEdmEntitySet entitySet)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntitySet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntityType, entitySet.ElementType.FullName(), EdmValueWriter.StringAsXml);
        }

        internal void WriteEntityTypeElementHeader(IEdmEntityType entityType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entityType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, entityType.BaseEntityType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, entityType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, entityType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);
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
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Relationship, this.model.GetAssociationFullName(member), EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_ToRole, this.model.GetAssociationEndName(member.Partner), EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_FromRole, this.model.GetAssociationEndName(member), EdmValueWriter.StringAsXml);
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

            this.WriteOptionalAttribute(CsdlConstants.Attribute_ConcurrencyMode, property.ConcurrencyMode, CsdlConstants.Default_ConcurrencyMode, ConcurrencyModeAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, property.DefaultValueString, EdmValueWriter.StringAsXml);
        }

        internal void WriteEnumMemberElementHeader(IEdmEnumMember member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Member);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml);
            bool? isExplicit = member.IsValueExplicit(this.model);
            if (!isExplicit.HasValue || isExplicit.Value)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Value, member.Value, EdmValueWriter.PrimitiveValueAsXml);
            }
        }

        internal void WriteNullableAttribute(IEdmTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
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

            this.WriteOptionalAttribute(CsdlConstants.Attribute_FixedLength, reference.IsFixedLength, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Scale, reference.Scale, EdmValueWriter.IntAsXml);
        }

        internal void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, SridAsXml);
        }

        internal void WriteStringTypeAttributes(IEdmStringTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Collation, reference.Collation, EdmValueWriter.StringAsXml);
            if (reference.IsUnbounded)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }
            else
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_FixedLength, reference.IsFixedLength, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Unicode, reference.IsUnicode, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
        {
            WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml);
        }

        internal void WriteReferentialConstraintElementHeader(IEdmNavigationProperty constraint)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReferentialConstraint);
        }

        internal void WriteReferentialConstraintPrincipalEndElementHeader(IEdmNavigationProperty end)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Principal);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, this.model.GetAssociationEndName(end), EdmValueWriter.StringAsXml);
        }

        internal void WriteReferentialConstraintDependentEndElementHeader(IEdmNavigationProperty end)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Dependent);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, this.model.GetAssociationEndName(end), EdmValueWriter.StringAsXml);
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

        internal void WriteOperationElementHeader(IEdmOperation operation, bool inlineReturnType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Function);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, operation.Name, EdmValueWriter.StringAsXml);
            if (inlineReturnType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_ReturnType, operation.ReturnType, this.TypeReferenceAsXml);
            }
        }

        internal void WriteReturnTypeElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReturnType);
        }

        internal void WriteOperationImportElementHeader(IEdmOperationImport operationImport)
        {
            // operationImport can't be Composable and sideEffecting at the same time.
            if (operationImport.IsComposable && operationImport.IsSideEffecting)
            {
                throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_ComposableFunctionImportCannotBeSideEffecting(operationImport.Name));
            }

            this.xmlWriter.WriteStartElement(CsdlConstants.Element_FunctionImport);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, operationImport.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_ReturnType, operationImport.ReturnType, this.TypeReferenceAsXml);

            // IsSideEffecting is optional, however its default applies to non-composable operation imports only.
            // Composable operation imports can't be side-effecting, so we don't emit false.
            if (!operationImport.IsComposable && operationImport.IsSideEffecting != CsdlConstants.Default_IsSideEffecting)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_IsSideEffecting, operationImport.IsSideEffecting, EdmValueWriter.BooleanAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsComposable, operationImport.IsComposable, CsdlConstants.Default_IsComposable, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsBindable, operationImport.IsBindable, CsdlConstants.Default_IsBindable, EdmValueWriter.BooleanAsXml);

            if (operationImport.EntitySet != null)
            {
                var entitySetReference = operationImport.EntitySet as IEdmEntitySetReferenceExpression;
                if (entitySetReference != null)
                {
                    this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySet, entitySetReference.ReferencedEntitySet.Name, EdmValueWriter.StringAsXml);
                }
                else
                {
                    var pathExpression = operationImport.EntitySet as IEdmPathExpression;
                    if (pathExpression != null)
                    {
                        this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySetPath, pathExpression.Path, PathAsXml);
                    }
                    else
                    {
                        throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_FunctionImportEntitySetExpressionIsInvalid(operationImport.Name));
                    }
                }
            }
        }

        internal void WriteFunctionParameterElementHeader(IEdmOperationParameter parameter, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Parameter);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, parameter.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, parameter.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_CollectionType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_ElementType, collectionType.ElementType, this.TypeReferenceAsXml);
            }
        }

        internal void WriteRowTypeElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_RowType);
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
                case EdmExpressionKind.DateTimeConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_DateTime, ((IEdmDateTimeConstantExpression)expression).Value, EdmValueWriter.DateTimeAsXml);
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
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Path, ((IEdmPathExpression)expression).Path, PathAsXml);
                    break;
                case EdmExpressionKind.StringConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_String, ((IEdmStringConstantExpression)expression).Value, EdmValueWriter.StringAsXml);
                    break;
                case EdmExpressionKind.DurationConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Duration, ((IEdmDurationConstantExpression)expression).Value, EdmValueWriter.DurationAsXml);
                    break;
                default:
                    Debug.Assert(false, "Attempted to inline an expression that was not one of the expected inlineable types.");
                    break;
            }
        }

        internal void WriteValueAnnotationElementHeader(IEdmValueAnnotation annotation, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ValueAnnotation);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(annotation.Value);
            }
        }

        internal void WriteTypeAnnotationElementHeader(IEdmTypeAnnotation annotation)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TypeAnnotation);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml);
        }

        internal void WritePropertyValueElementHeader(IEdmPropertyValueBinding value, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyValue);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, value.BoundProperty.Name, EdmValueWriter.StringAsXml);
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
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_String);
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

        internal void WriteDateTimeConstantExpressionElement(IEdmDateTimeConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_DateTime);
            this.xmlWriter.WriteString(EdmValueWriter.DateTimeAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_DateTimeOffset);
            this.xmlWriter.WriteString(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));
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

        internal void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression, bool isFunction)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Apply);
            if (isFunction)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Function, ((IEdmOperationReferenceExpression)expression.AppliedOperation).ReferencedOperation, this.FunctionAsXml);
            }
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
            this.xmlWriter.WriteString(PathAsXml(expression.Path));
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

        internal void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_IsType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteAssertTypeExpressionElementHeader(IEdmAssertTypeExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_AssertType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteEntitySetReferenceExpressionElement(IEdmEntitySetReferenceExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntitySetReference);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, expression.ReferencedEntitySet, EntitySetAsXml);
            this.WriteEndElement();
        }

        internal void WriteParameterReferenceExpressionElement(IEdmParameterReferenceExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ParameterReference);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, expression.ReferencedParameter, ParameterAsXml);
            this.WriteEndElement();
        }

        internal void WriteOperationReferenceExpressionElement(IEdmOperationReferenceExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_FunctionReference);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, expression.ReferencedOperation, this.FunctionAsXml);
            this.WriteEndElement();
        }

        internal void WriteEnumMemberReferenceExpressionElement(IEdmEnumMemberReferenceExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumMemberReference);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, expression.ReferencedEnumMember, EnumMemberAsXml);
            this.WriteEndElement();
        }

        internal void WritePropertyReferenceExpressionElementHeader(IEdmPropertyReferenceExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyReference);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, expression.ReferencedProperty, PropertyAsXml);
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

        private static string MultiplicityAsXml(EdmMultiplicity endKind)
        {
            switch (endKind)
            {
                case EdmMultiplicity.Many:
                    return CsdlConstants.Value_EndMany;
                case EdmMultiplicity.One:
                    return CsdlConstants.Value_EndRequired;
                case EdmMultiplicity.ZeroOrOne:
                    return CsdlConstants.Value_EndOptional;
                default:
                    throw new InvalidOperationException(Strings.UnknownEnumVal_Multiplicity(endKind.ToString()));
            }
        }

        private static string ConcurrencyModeAsXml(EdmConcurrencyMode mode)
        {
            switch (mode)
            {
                case EdmConcurrencyMode.Fixed:
                    return CsdlConstants.Value_Fixed;
                case EdmConcurrencyMode.None:
                    return CsdlConstants.Value_None;
                default:
                    throw new InvalidOperationException(Strings.UnknownEnumVal_ConcurrencyMode(mode.ToString()));
            }
        }

        private static string PathAsXml(IEnumerable<string> path)
        {
            return EdmUtil.JoinInternal("/", path);
        }

        private static string ParameterAsXml(IEdmOperationParameter parameter)
        {
            return parameter.Name;
        }

        private static string PropertyAsXml(IEdmProperty property)
        {
            return property.Name;
        }

        private static string EnumMemberAsXml(IEdmEnumMember member)
        {
            return member.DeclaringType.FullName() + "/" + member.Name;
        }

        private static string EntitySetAsXml(IEdmEntitySet set)
        {
            return set.Container.FullName() + "/" + set.Name;
        }

        private static string SridAsXml(int? i)
        {
            return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_SridVariable;
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
