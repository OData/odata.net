//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaXmlWriter.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Helpers;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// OData Common Schema Definition Language (CSDL) XML writer
    /// </summary>
    internal partial class EdmModelCsdlSchemaXmlWriter : EdmModelCsdlSchemaWriter
    {
        /// <summary>
        /// Asynchronously writes Reference element header.
        /// </summary>
        /// <param name="reference">edmx:reference element</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteReferenceElementHeaderAsync(IEdmReference reference)
        {
            // e.g. <edmx:Reference Uri="http://host/schema/VipCustomer.xml">
            await this.xmlWriter.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Reference, this.edmxNamespace).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Uri, reference.Uri, EdmValueWriter.UriAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the reference element end.
        /// </summary>
        /// <param name="reference">edmx:reference element</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteReferenceElementEndAsync(IEdmReference reference)
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes the Include element header.
        /// </summary>
        /// <param name="include">The Edm Include information.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritIncludeElementHeaderAsync(IEdmInclude include)
        {
            // e.g. <edmx:Include Namespace="NS.Ref1" Alias="VPCT" />
            await this.xmlWriter.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Include, this.edmxNamespace).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Namespace, include.Namespace, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Alias, include.Alias, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the IncludeAnnotations end.
        /// </summary>
        /// <param name="include">The Edm Include information.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteIncludeElementEndAsync(IEdmInclude include)
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes the IncludeAnnotations element.
        /// </summary>
        /// <param name="includeAnnotations">The Edm Include annotations.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal async Task WriteIncludeAnnotationsElementAsync(IEdmIncludeAnnotations includeAnnotations)
        {
            // e.g. <edmx:IncludeAnnotations ... />
            await this.xmlWriter.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_IncludeAnnotations, this.edmxNamespace).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_TermNamespace, includeAnnotations.TermNamespace, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Qualifier, includeAnnotations.Qualifier, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_TargetNamespace, includeAnnotations.TargetNamespace, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes Term element header.
        /// </summary>
        /// <param name="term">The Edm term.</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteTermElementHeaderAsync(IEdmTerm term, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Term, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, term.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (inlineType && term.Type != null)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, term.Type, this.TypeReferenceAsXml).ConfigureAwait(false);
            }
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_DefaultValue, term.DefaultValue, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_AppliesTo, term.AppliesTo, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the ComplexType element header.
        /// </summary>
        /// <param name="complexType">The Edm Complex type.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteComplexTypeElementHeaderAsync(IEdmComplexType complexType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ComplexType, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, complexType.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_OpenType, complexType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the EnumType element header.
        /// </summary>
        /// <param name="enumType">The Edm enumaration type.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEnumTypeElementHeaderAsync(IEdmEnumType enumType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EnumType, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, enumType.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_UnderlyingType, enumType.UnderlyingType, this.TypeDefinitionAsXml).ConfigureAwait(false);
            }

            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IsFlags, enumType.IsFlags, CsdlConstants.Default_IsFlags, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the EnumContainer element end.
        /// </summary>
        /// <param name="enumType">The Edm enumaration type.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteEnumTypeElementEndAsync(IEdmEnumType enumType)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes the EntityContainer element header.
        /// </summary>
        /// <param name="container">The Edm Entity container.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEntityContainerElementHeaderAsync(IEdmEntityContainer container)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EntityContainer, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, container.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (container is CsdlSemanticsEntityContainer tmp && tmp.Element is CsdlEntityContainer csdlContainer)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Extends, csdlContainer.Extends, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes the EntitySet element header.
        /// </summary>
        /// <param name="entitySet">The Edm Entity set.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEntitySetElementHeaderAsync(IEdmEntitySet entitySet)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EntitySet, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_EntityType, entitySet.EntityType().FullName(), EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the Singleton element header.
        /// </summary>
        /// <param name="singleton">The Edm singleton.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteSingletonElementHeaderAsync(IEdmSingleton singleton)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Singleton, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, singleton.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, singleton.EntityType().FullName(), EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the EntityType element header.
        /// </summary>
        /// <param name="entityType">The Edm entity type.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEntityTypeElementHeaderAsync(IEdmEntityType entityType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EntityType, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, entityType.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_BaseType, entityType.BaseEntityType(), this.TypeDefinitionAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Abstract, entityType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_OpenType, entityType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);

            // HasStream value should be inherited.  Only have it on base type is sufficient.
            bool writeHasStream = entityType.HasStream && (entityType.BaseEntityType() == null || (entityType.BaseEntityType() != null && !entityType.BaseEntityType().HasStream));
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_HasStream, writeHasStream, CsdlConstants.Default_HasStream, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the key properties element header.
        /// </summary>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteDeclaredKeyPropertiesElementHeaderAsync()
        {
            return this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Key, null);
        }

        /// <summary>
        /// Asynchronously writes the PropertyRef element.
        /// </summary>
        /// <param name="property">The Edm Structural Property.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePropertyRefElementAsync(IEdmStructuralProperty property)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyRef, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the NavigationProperty element header.
        /// </summary>
        /// <param name="property">The Edm navigation property.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteNavigationPropertyElementHeaderAsync(IEdmNavigationProperty property)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_NavigationProperty, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);

            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml).ConfigureAwait(false);
            if (!property.Type.IsCollection() && property.Type.IsNullable != CsdlConstants.Default_Nullable)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Nullable, property.Type.IsNullable, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
            }

            if (property.Partner != null)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Partner, property.GetPartnerPath()?.Path, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            }

            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_ContainsTarget, property.ContainsTarget, CsdlConstants.Default_ContainsTarget, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the NavigationOnDeleteAction element.
        /// </summary>
        /// <param name="operationAction">The Edm OnDelete action.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteNavigationOnDeleteActionElementAsync(EdmOnDeleteAction operationAction)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_OnDelete, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Action, operationAction.ToString(), EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the Schema element header.
        /// </summary>
        /// <param name="schema">The Edm schema.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="mappings">Collection of mappings.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteSchemaElementHeaderAsync(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string xmlNamespace = GetCsdlNamespace(EdmVersion);
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Schema, xmlNamespace).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Namespace, schema.Namespace, string.Empty, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Alias, alias, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (mappings != null)
            {
                foreach (KeyValuePair<string, string> mapping in mappings)
                {
                    await this.xmlWriter.WriteAttributeStringAsync(EdmConstants.XmlNamespacePrefix, mapping.Key, null, mapping.Value).ConfigureAwait(false);
                }
            }
        }


        /// <summary>
        /// Asynchronously writes the Annotations element header.
        /// </summary>
        /// <param name="annotationsForTarget">The Key/Value of Edm Vocabulary annotation.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteAnnotationsElementHeaderAsync(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Annotations, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Target, annotationsForTarget.Key, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the StructuralProperty element header.
        /// </summary>
        /// <param name="property">The Edm Structural Property.</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteStructuralPropertyElementHeaderAsync(IEdmStructuralProperty property, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Property, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml).ConfigureAwait(false);
            }

            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_DefaultValue, property.DefaultValueString, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes enumeration member element header.
        /// </summary>
        /// <param name="member">The Edm enumeration member.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEnumMemberElementHeaderAsync(IEdmEnumMember member)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Member, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            bool? isExplicit = member.IsValueExplicit(this.Model);
            if (!isExplicit.HasValue || isExplicit.Value)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, CsdlConstants.Attribute_Value, null, EdmValueWriter.LongAsXml(member.Value.Value)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes EnumMember element end.
        /// </summary>
        /// <param name="member">The Edm enumeration member.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteEnumMemberElementEndAsync(IEdmEnumMember member)
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes Nullable attribute.
        /// </summary>
        /// <param name="reference">The Edm type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteNullableAttributeAsync(IEdmTypeReference reference)
        {
            return this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
        }

        /// <summary>
        /// Asynchronously writes TypeDefinition attributes.
        /// </summary>
        /// <param name="reference">The Edm type defination reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteTypeDefinitionAttributesAsync(IEdmTypeDefinitionReference reference)
        {
            IEdmTypeReference actualTypeReference = reference.AsActualTypeReference();

            if (actualTypeReference.IsBinary())
            {
                return this.WriteBinaryTypeAttributesAsync(actualTypeReference.AsBinary());
            }

            if (actualTypeReference.IsString())
            {
                return this.WriteStringTypeAttributesAsync(actualTypeReference.AsString());
            }

            if (actualTypeReference.IsTemporal())
            {
                return this.WriteTemporalTypeAttributesAsync(actualTypeReference.AsTemporal());
            }

            if (actualTypeReference.IsDecimal())
            {
                return this.WriteDecimalTypeAttributesAsync(actualTypeReference.AsDecimal());
            }

            if (actualTypeReference.IsSpatial())
            {
                return this.WriteSpatialTypeAttributesAsync(actualTypeReference.AsSpatial());
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes BinaryType attributes.
        /// </summary>
        /// <param name="reference">The Edm binary type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteBinaryTypeAttributesAsync(IEdmBinaryTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }

            return this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
        }

        /// <summary>
        /// Asynchronously writes DecimalType attributes.
        /// </summary>
        /// <param name="reference">The Edm Decimal type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteDecimalTypeAttributesAsync(IEdmDecimalTypeReference reference)
        {
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Scale, reference.Scale, ScaleAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes SpatialType attributes.
        /// </summary>
        /// <param name="reference">The Edm Spatial type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteSpatialTypeAttributesAsync(IEdmSpatialTypeReference reference)
        {
            if (reference.IsGeography())
            {
                return this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeographySrid, SridAsXml);
            }

            if (reference.IsGeometry())
            {
                return this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeometrySrid, SridAsXml);
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes StringType attributes.
        /// </summary>
        /// <param name="reference">The Edm String type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteStringTypeAttributesAsync(IEdmStringTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            }
            else
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml).ConfigureAwait(false);
            }

            if (reference.IsUnicode != null)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Unicode, reference.IsUnicode, CsdlConstants.Default_IsUnicode, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes TemporalType attributes.
        /// </summary>
        /// <param name="reference">The Edm Temporal type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteTemporalTypeAttributesAsync(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision != null)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision, EdmValueWriter.IntAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes ReferentialConstraint pair.
        /// </summary>
        /// <param name="pair">The Edm Referential Constraint property pair.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteReferentialConstraintPairAsync(EdmReferentialConstraintPropertyPair pair)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ReferentialConstraint, null).ConfigureAwait(false);

            // <EntityType Name="Product">
            //   ...
            //   <Property Name="CategoryID" Type="Edm.String" Nullable="false"/>
            //  <NavigationProperty Name="Category" Type="Self.Category" Nullable="false">
            //     <ReferentialConstraint Property="CategoryID" ReferencedProperty="ID" />
            //   </NavigationProperty>
            // </EntityType>
            // the above CategoryID is DependentProperty, ID is PrincipalProperty.
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Property, pair.DependentProperty.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_ReferencedProperty, pair.PrincipalProperty.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes Annotation string attribute.
        /// </summary>
        /// <param name="annotation">The Edm Direct value annotation.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteAnnotationStringAttributeAsync(IEdmDirectValueAnnotation annotation)
        {
            if (annotation.Value is IEdmPrimitiveValue edmValue)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(edmValue)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes Annotation string element.
        /// </summary>
        /// <param name="annotation">The Edm Direct value annotation.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteAnnotationStringElementAsync(IEdmDirectValueAnnotation annotation)
        {
            if (annotation.Value is IEdmPrimitiveValue edmValue)
            {
                await this.xmlWriter.WriteRawAsync(((IEdmStringValue)edmValue).Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes Action element header.
        /// </summary>
        /// <param name="action">The Edm Action.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteActionElementHeaderAsync(IEdmAction action)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Action, null).ConfigureAwait(false);
            await this.WriteOperationElementAttributesAsync(action).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes Function element header.
        /// </summary>
        /// <param name="function">The Edm Function.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFunctionElementHeaderAsync(IEdmFunction function)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Function, null).ConfigureAwait(false);
            await this.WriteOperationElementAttributesAsync(function).ConfigureAwait(false);

            if (function.IsComposable)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IsComposable, function.IsComposable, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes ReturnType element header.
        /// </summary>
        /// <param name="operationReturn">The Edm Operation return.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteReturnTypeElementHeaderAsync(IEdmOperationReturn operationReturn)
        {
            return this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ReturnType, null);
        }

        /// <summary>
        /// Asynchronously writes Type attribute.
        /// </summary>
        /// <param name="typeReference">The Edm type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteTypeAttributeAsync(IEdmTypeReference typeReference)
        {
            return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, typeReference, this.TypeReferenceAsXml);
        }

        /// <summary>
        /// Asynchronously writes ActionImport element header.
        /// </summary>
        /// <param name="actionImport">The Edm action import.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteActionImportElementHeaderAsync(IEdmActionImport actionImport)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ActionImport, null).ConfigureAwait(false);
            await this.WriteOperationImportAttributesAsync(actionImport, CsdlConstants.Attribute_Action).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes FunctionImport element header.
        /// </summary>
        /// <param name="functionImport">The Edm function import.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFunctionImportElementHeaderAsync(IEdmFunctionImport functionImport)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_FunctionImport, null).ConfigureAwait(false);
            await this.WriteOperationImportAttributesAsync(functionImport, CsdlConstants.Attribute_Function).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IncludeInServiceDocument, functionImport.IncludeInServiceDocument, CsdlConstants.Default_IncludeInServiceDocument, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes OperationParameter element header.
        /// </summary>
        /// <param name="parameter">The Edm Operation parameter</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteOperationParameterElementHeaderAsync(IEdmOperationParameter parameter, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Parameter, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, parameter.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, parameter.Type, this.TypeReferenceAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes OperationParameter end element.
        /// </summary>
        /// <param name="parameter">The Edm operation paramater.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteOperationParameterEndElementAsync(IEdmOperationParameter parameter)
        {
            if (parameter is IEdmOptionalParameter optionalParameter &&
                !(optionalParameter.VocabularyAnnotations(this.Model).Any(a => a.Term == CoreVocabularyModel.OptionalParameterTerm)))
            {
                EdmRecordExpression optionalValue = new EdmRecordExpression();

                EdmVocabularyAnnotation vocabularyAnnotation = new EdmVocabularyAnnotation(parameter, CoreVocabularyModel.OptionalParameterTerm, optionalValue);
                await this.WriteVocabularyAnnotationElementHeaderAsync(vocabularyAnnotation, false).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(optionalParameter.DefaultValueString))
                {
                    EdmPropertyConstructor property = new EdmPropertyConstructor(CsdlConstants.Attribute_DefaultValue, new EdmStringConstant(optionalParameter.DefaultValueString));
                    await this.WriteRecordExpressionElementHeaderAsync(optionalValue).ConfigureAwait(false);
                    await this.WritePropertyValueElementHeaderAsync(property, true).ConfigureAwait(false);
                    await this.WriteEndElementAsync().ConfigureAwait(false);
                    await this.WriteEndElementAsync().ConfigureAwait(false);
                }

                await this.WriteEndElementAsync().ConfigureAwait(false);
            }

            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes CollectionType element header.
        /// </summary>
        /// <param name="collectionType">The Edm Collection type.</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteCollectionTypeElementHeaderAsync(IEdmCollectionType collectionType, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_CollectionType, null).ConfigureAwait(false);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_ElementType, collectionType.ElementType, this.TypeReferenceAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes Inline expression.
        /// </summary>
        /// <param name="expression">The Edm expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteInlineExpressionAsync(IEdmExpression expression)
        {
            IEdmPathExpression pathExpression = expression as IEdmPathExpression;
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.BinaryConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Binary, ((IEdmBinaryConstantExpression)expression).Value, EdmValueWriter.BinaryAsXml);
                case EdmExpressionKind.BooleanConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Bool, ((IEdmBooleanConstantExpression)expression).Value, EdmValueWriter.BooleanAsXml);
                case EdmExpressionKind.DateTimeOffsetConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_DateTimeOffset, ((IEdmDateTimeOffsetConstantExpression)expression).Value, EdmValueWriter.DateTimeOffsetAsXml);
                case EdmExpressionKind.DecimalConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Decimal, ((IEdmDecimalConstantExpression)expression).Value, EdmValueWriter.DecimalAsXml);
                case EdmExpressionKind.FloatingConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Float, ((IEdmFloatingConstantExpression)expression).Value, EdmValueWriter.FloatAsXml);
                case EdmExpressionKind.GuidConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Guid, ((IEdmGuidConstantExpression)expression).Value, EdmValueWriter.GuidAsXml);
                case EdmExpressionKind.IntegerConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Int, ((IEdmIntegerConstantExpression)expression).Value, EdmValueWriter.LongAsXml);
                case EdmExpressionKind.Path:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Path, pathExpression.PathSegments, PathAsXml);
                case EdmExpressionKind.PropertyPath:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_PropertyPath, pathExpression.PathSegments, PathAsXml);
                case EdmExpressionKind.NavigationPropertyPath:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_NavigationPropertyPath, pathExpression.PathSegments, PathAsXml);
                case EdmExpressionKind.AnnotationPath:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_AnnotationPath, pathExpression.PathSegments, PathAsXml);
                case EdmExpressionKind.StringConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_String, ((IEdmStringConstantExpression)expression).Value, EdmValueWriter.StringAsXml);
                case EdmExpressionKind.DurationConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Duration, ((IEdmDurationConstantExpression)expression).Value, EdmValueWriter.DurationAsXml);
                case EdmExpressionKind.DateConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Date, ((IEdmDateConstantExpression)expression).Value, EdmValueWriter.DateAsXml);
                case EdmExpressionKind.TimeOfDayConstant:
                    return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_TimeOfDay, ((IEdmTimeOfDayConstantExpression)expression).Value, EdmValueWriter.TimeOfDayAsXml);
                default:
                    Debug.Assert(false, "Attempted to inline an expression that was not one of the expected inlineable types.");
                    return TaskUtils.CompletedTask;
            }
        }

        /// <summary>
        /// Asynchronously writes PropertyConstructor element end.
        /// </summary>
        /// <param name="constructor">The Edm property constructor.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WritePropertyConstructorElementEndAsync(IEdmPropertyConstructor constructor)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes VocabularyAnnotation element header.
        /// </summary>
        /// <param name="annotation">The Edm vocabulary annotation.</param>
        /// <param name="isInline">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteVocabularyAnnotationElementHeaderAsync(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Annotation, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml).ConfigureAwait(false);

            if (isInline && !IsUsingDefaultValue(annotation))
            {
                // in xml format, we can (should) skip writing the expression value if it matches the term default value.
                await this.WriteInlineExpressionAsync(annotation.Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes VocabularyAnnotation element end.
        /// </summary>
        /// <param name="annotation">The Edm vocabulary annotation.</param>
        /// <param name="isInline">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteVocabularyAnnotationElementEndAsync(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            return WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes PropertyValue element header.
        /// </summary>
        /// <param name="value">The Edm property constructor.</param>
        /// <param name="isInline">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePropertyValueElementHeaderAsync(IEdmPropertyConstructor value, bool isInline)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyValue, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Property, value.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (isInline)
            {
                await this.WriteInlineExpressionAsync(value.Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes RecordExpression element header.
        /// </summary>
        /// <param name="expression">The Edm Record expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteRecordExpressionElementHeaderAsync(IEdmRecordExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Record, null).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Type, expression.DeclaredType, this.TypeReferenceAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes PropertyConstructor element header.
        /// </summary>
        /// <param name="constructor">The Edm property constructor.</param>
        /// <param name="isInline">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePropertyConstructorElementHeaderAsync(IEdmPropertyConstructor constructor, bool isInline)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyValue, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Property, constructor.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            if (isInline)
            {
                await this.WriteInlineExpressionAsync(constructor.Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes StringConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm String constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteStringConstantExpressionElementAsync(IEdmStringConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_String, null).ConfigureAwait(false);

            await this.xmlWriter.WriteStringAsync(EdmValueWriter.StringAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes BinaryConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Binary constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteBinaryConstantExpressionElementAsync(IEdmBinaryConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Binary, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.BinaryAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes BooleanConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Boolean constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteBooleanConstantExpressionElementAsync(IEdmBooleanConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Bool, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.BooleanAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes NullConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Null expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteNullConstantExpressionElementAsync(IEdmNullExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Null, null).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes DateConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Date constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteDateConstantExpressionElementAsync(IEdmDateConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Date, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DateAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes DateTimeOffsetConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm DateTimeOffset constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteDateTimeOffsetConstantExpressionElementAsync(IEdmDateTimeOffsetConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_DateTimeOffset, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DateTimeOffsetAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes DurationConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Duration constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteDurationConstantExpressionElementAsync(IEdmDurationConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Duration, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DurationAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes DecimalConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Decimal constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteDecimalConstantExpressionElementAsync(IEdmDecimalConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Decimal, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DecimalAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes FloatingConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Floating constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFloatingConstantExpressionElementAsync(IEdmFloatingConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Float, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.FloatAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes FunctionApplication element header.
        /// </summary>
        /// <param name="expression">The Edm apply expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFunctionApplicationElementHeaderAsync(IEdmApplyExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Apply, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Function, expression.AppliedFunction, this.FunctionAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes FunctionApplication element end.
        /// </summary>
        /// <param name="expression">The Edm Apply expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteFunctionApplicationElementEndAsync(IEdmApplyExpression expression)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes GuidConstantExpression element.
        /// </summary>
        /// <param name="expression">The Edm Guid constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteGuidConstantExpressionElementAsync(IEdmGuidConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Guid, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.GuidAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes IntegerConstant Expression element.
        /// </summary>
        /// <param name="expression">The Edm Integer constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteIntegerConstantExpressionElementAsync(IEdmIntegerConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Int, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.LongAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes Path Expression element.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Path, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes PropertyPath Expression element.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePropertyPathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyPath, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes NavigationPropertyPath Expression element.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteNavigationPropertyPathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_NavigationPropertyPath, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes AnnotationPath Expression element.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteAnnotationPathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_AnnotationPath, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes If Expression element header.
        /// </summary>
        /// <param name="expression">EDM if Expression</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteIfExpressionElementHeaderAsync(IEdmIfExpression expression)
        {
            return this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_If, null);
        }

        /// <summary>
        /// Asynchronously writes If Expression element end.
        /// </summary>
        /// <param name="expression">EDM if Expression</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteIfExpressionElementEndAsync(IEdmIfExpression expression)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes Collection Expression element header.
        /// </summary>
        /// <param name="expression">The Edm collection expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteCollectionExpressionElementHeaderAsync(IEdmCollectionExpression expression)
        {
            return this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Collection, null);
        }

        /// <summary>
        /// Asynchronously writes Collection Expression element end.
        /// </summary>
        /// <param name="expression">The Edm collection expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteCollectionExpressionElementEndAsync(IEdmCollectionExpression expression)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes LabeledElement header.
        /// </summary>
        /// <param name="labeledElement">The Edm Labeled expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteLabeledElementHeaderAsync(IEdmLabeledExpression labeledElement)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_LabeledElement, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, labeledElement.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes LabeledExpressionReference Expression.
        /// </summary>
        /// <param name="labeledExpressionReference">The Edm Labeled Expression expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression labeledExpressionReference)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_LabeledElementReference, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, labeledExpressionReference.ReferencedLabeledExpression.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes TimeOfDay Constant Expression element.
        /// </summary>
        /// <param name="expression">The Edm TimeOfDay constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteTimeOfDayConstantExpressionElementAsync(IEdmTimeOfDayConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_TimeOfDay, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.TimeOfDayAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes IsType Expression header.
        /// </summary>
        /// <param name="expression">The Edm IsType expression.</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteIsTypeExpressionElementHeaderAsync(IEdmIsTypeExpression expression, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_IsType, null).ConfigureAwait(false);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes Cast Expression header.
        /// </summary>
        /// <param name="expression">The Edm Cast expression.</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteCastExpressionElementHeaderAsync(IEdmCastExpression expression, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Cast, null).ConfigureAwait(false);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes Cast Expression end.
        /// </summary>
        /// <param name="expression">The Edm Cast expression.</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteCastExpressionElementEndAsync(IEdmCastExpression expression, bool inlineType)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes EnumMember Expression element.
        /// </summary>
        /// <param name="expression">The Edm enumaration member expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEnumMemberExpressionElementAsync(IEdmEnumMemberExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EnumMember, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EnumMemberAsXmlOrJson(expression.EnumMembers)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes TypeDefinition element header.
        /// </summary>
        /// <param name="typeDefinition">The Edm Type definition.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteTypeDefinitionElementHeaderAsync(IEdmTypeDefinition typeDefinition)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_TypeDefinition, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, typeDefinition.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_UnderlyingType, typeDefinition.UnderlyingType, this.TypeDefinitionAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes End Element.
        /// </summary>
        internal override Task WriteEndElementAsync()
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes Array End Element.
        /// </summary>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteArrayEndElementAsync()
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes Optional attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="defaultValue">The attribute default value.</param>
        /// <param name="getStringFunc">Get string Function.</param>
        internal async Task WriteOptionalAttributeAsync<T>(string attribute, T value, T defaultValue, Func<T, string> getStringFunc)
        {
            if (!value.Equals(defaultValue))
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, getStringFunc(value)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes Optional attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="getStringFunc">Get string Function.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal async Task WriteOptionalAttributeAsync<T>(string attribute, T value, Func<T, string> getStringFunc)
        {
            if (value != null)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, getStringFunc(value)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes Required attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="toXml">Value to xml function.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal Task WriteRequiredAttributeAsync<T>(string attribute, T value, Func<T, string> toXml)
        {
            return this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, toXml(value));
        }

        /// <summary>
        /// Asynchronously writes Operation element attributes.
        /// </summary>
        /// <param name="operation">The Edm operation.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteOperationElementAttributesAsync(IEdmOperation operation)
        {
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, operation.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);

            if (operation.IsBound)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IsBound, operation.IsBound, EdmValueWriter.BooleanAsXml).ConfigureAwait(false);
            }

            if (operation.EntitySetPath != null)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_EntitySetPath, operation.EntitySetPath.PathSegments, PathAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes NavigationPropertyBinding.
        /// </summary>
        /// <param name="binding">The Edm navigation property binding.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteNavigationPropertyBindingAsync(IEdmNavigationPropertyBinding binding)
        {
            // For backwards compatability, only write annotations that vary by type cast in versions > 4.0
            if (this.Model.GetEdmVersion() > EdmConstants.EdmVersion4 || binding.Path.PathSegments.Last().IndexOf('.') < 0)
            {
                await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_NavigationPropertyBinding, null).ConfigureAwait(false);

                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Path, binding.Path.Path, EdmValueWriter.StringAsXml).ConfigureAwait(false);

                // TODO: handle container names, etc.
                if (binding.Target is IEdmContainedEntitySet containedEntitySet)
                {
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Target, containedEntitySet.Path.Path, EdmValueWriter.StringAsXml).ConfigureAwait(false);
                }
                else
                {
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Target, binding.Target.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
                }

                await this.xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes OperationImport attributes.
        /// </summary>
        /// <param name="operationImport">The Edm Operation import.</param>
        /// <param name="operationAttributeName">Operation attribute name.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteOperationImportAttributesAsync(IEdmOperationImport operationImport, string operationAttributeName)
        {
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, operationImport.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(operationAttributeName, operationImport.Operation.FullName(), EdmValueWriter.StringAsXml).ConfigureAwait(false);

            if (operationImport.EntitySet != null)
            {
                if (operationImport.EntitySet is IEdmPathExpression pathExpression)
                {
                    await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_EntitySet, pathExpression.PathSegments, PathAsXml).ConfigureAwait(false);
                }
                else
                {
                    throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(operationImport.Name));
                }
            }
        }
    }
}
