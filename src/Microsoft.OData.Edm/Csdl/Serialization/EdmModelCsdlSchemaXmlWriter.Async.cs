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
        /// Asynchronously writes reference element header.
        /// </summary>
        /// <param name="reference">edmx:Reference element</param>
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
        /// <param name="reference">edmx:Reference element</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteReferenceElementEndAsync(IEdmReference reference)
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes the include element header.
        /// </summary>
        /// <param name="include">The Edm include information.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritIncludeElementHeaderAsync(IEdmInclude include)
        {
            // e.g. <edmx:Include Namespace="NS.Ref1" Alias="VPCT" />
            await this.xmlWriter.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Include, this.edmxNamespace).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Namespace, include.Namespace, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Alias, include.Alias, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the include annotations end.
        /// </summary>
        /// <param name="include">The Edm include information.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteIncludeElementEndAsync(IEdmInclude include)
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes the include annotations element.
        /// </summary>
        /// <param name="includeAnnotations">The Edm include annotations.</param>
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
        /// Asynchronously writes term element header.
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
        /// Asynchronously writes the complex type element header.
        /// </summary>
        /// <param name="complexType">The Edm complex type.</param>
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
        /// Asynchronously writes the enum type element header.
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
        /// Asynchronously writes the enum container element end.
        /// </summary>
        /// <param name="enumType">The Edm enumaration type.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteEnumTypeElementEndAsync(IEdmEnumType enumType)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes the entity container element header.
        /// </summary>
        /// <param name="container">The Edm entity container.</param>
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
        /// Asynchronously writes the entity set element header.
        /// </summary>
        /// <param name="entitySet">The Edm entity set.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEntitySetElementHeaderAsync(IEdmEntitySet entitySet)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EntitySet, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_EntityType, entitySet.EntityType().FullName(), EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the singleton element header.
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
        /// Asynchronously writes the entity type element header.
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
        /// Asynchronously writes the property ref element.
        /// </summary>
        /// <param name="property">The Edm structural property.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePropertyRefElementAsync(IEdmStructuralProperty property)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyRef, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the navigation property element header.
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
        /// Asynchronously writes the navigation OnDelete action element.
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
        /// Asynchronously writes the schema element header.
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
        /// Asynchronously writes the annotations element header.
        /// </summary>
        /// <param name="annotationsForTarget">The Key/Value of Edm vocabulary annotation.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteAnnotationsElementHeaderAsync(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Annotations, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Target, annotationsForTarget.Key, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the structural property element header.
        /// </summary>
        /// <param name="property">The Edm structural property.</param>
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
        /// Asynchronously writes enum member element end.
        /// </summary>
        /// <param name="member">The Edm enumeration member.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteEnumMemberElementEndAsync(IEdmEnumMember member)
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes nullable attribute.
        /// </summary>
        /// <param name="reference">The Edm type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteNullableAttributeAsync(IEdmTypeReference reference)
        {
            return this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
        }

        /// <summary>
        /// Asynchronously writes type definition attributes.
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
        /// Asynchronously writes binary type attributes.
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
        /// Asynchronously writes decimal type attributes.
        /// </summary>
        /// <param name="reference">The Edm Decimal type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteDecimalTypeAttributesAsync(IEdmDecimalTypeReference reference)
        {
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Scale, reference.Scale, ScaleAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes spatial type attributes.
        /// </summary>
        /// <param name="reference">The Edm spatial type reference.</param>
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
        /// Asynchronously writes string type attributes.
        /// </summary>
        /// <param name="reference">The Edm string type reference.</param>
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
        /// Asynchronously writes temporal type attributes.
        /// </summary>
        /// <param name="reference">The Edm temporal type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteTemporalTypeAttributesAsync(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision != null)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision, EdmValueWriter.IntAsXml).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes referential constraint pair.
        /// </summary>
        /// <param name="pair">The Edm referential constraint property pair.</param>
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
        /// Asynchronously writes annotation string attribute.
        /// </summary>
        /// <param name="annotation">The Edm direct value annotation.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteAnnotationStringAttributeAsync(IEdmDirectValueAnnotation annotation)
        {
            if (annotation.Value is IEdmPrimitiveValue edmValue)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(edmValue)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes annotation string element.
        /// </summary>
        /// <param name="annotation">The Edm direct value annotation.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteAnnotationStringElementAsync(IEdmDirectValueAnnotation annotation)
        {
            if (annotation.Value is IEdmPrimitiveValue edmValue)
            {
                await this.xmlWriter.WriteRawAsync(((IEdmStringValue)edmValue).Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes action element header.
        /// </summary>
        /// <param name="action">The Edm action.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteActionElementHeaderAsync(IEdmAction action)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Action, null).ConfigureAwait(false);
            await this.WriteOperationElementAttributesAsync(action).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes function element header.
        /// </summary>
        /// <param name="function">The Edm function.</param>
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
        /// Asynchronously writes return type element header.
        /// </summary>
        /// <param name="operationReturn">The Edm operation return.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteReturnTypeElementHeaderAsync(IEdmOperationReturn operationReturn)
        {
            return this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ReturnType, null);
        }

        /// <summary>
        /// Asynchronously writes type attribute.
        /// </summary>
        /// <param name="typeReference">The Edm type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteTypeAttributeAsync(IEdmTypeReference typeReference)
        {
            return this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, typeReference, this.TypeReferenceAsXml);
        }

        /// <summary>
        /// Asynchronously writes action import element header.
        /// </summary>
        /// <param name="actionImport">The Edm action import.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteActionImportElementHeaderAsync(IEdmActionImport actionImport)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ActionImport, null).ConfigureAwait(false);
            await this.WriteOperationImportAttributesAsync(actionImport, CsdlConstants.Attribute_Action).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes function import element header.
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
        /// Asynchronously writes operation parameter element header.
        /// </summary>
        /// <param name="parameter">The Edm operation parameter</param>
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
        /// Asynchronously writes operation parameter end element.
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
        /// Asynchronously writes collection type element header.
        /// </summary>
        /// <param name="collectionType">The Edm collection type.</param>
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
        /// Asynchronously writes inline expression.
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
        /// Asynchronously writes property constructor element end.
        /// </summary>
        /// <param name="constructor">The Edm property constructor.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WritePropertyConstructorElementEndAsync(IEdmPropertyConstructor constructor)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes vocabulary annotation element header.
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
        /// Asynchronously writes vocabulary annotation element end.
        /// </summary>
        /// <param name="annotation">The Edm vocabulary annotation.</param>
        /// <param name="isInline">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteVocabularyAnnotationElementEndAsync(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            return WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes property value element header.
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
        /// Asynchronously writes record expression element header.
        /// </summary>
        /// <param name="expression">The Edm Record expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteRecordExpressionElementHeaderAsync(IEdmRecordExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Record, null).ConfigureAwait(false);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Type, expression.DeclaredType, this.TypeReferenceAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes property constructor element header.
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
        /// Asynchronously writes string constant expression element.
        /// </summary>
        /// <param name="expression">The Edm string constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteStringConstantExpressionElementAsync(IEdmStringConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_String, null).ConfigureAwait(false);

            await this.xmlWriter.WriteStringAsync(EdmValueWriter.StringAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes binary constant expression element.
        /// </summary>
        /// <param name="expression">The Edm binary constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteBinaryConstantExpressionElementAsync(IEdmBinaryConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Binary, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.BinaryAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes boolean constant expression element.
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
        /// Asynchronously writes null constant expression element.
        /// </summary>
        /// <param name="expression">The Edm null expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteNullConstantExpressionElementAsync(IEdmNullExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Null, null).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes Date constant expression element.
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
        /// Asynchronously writes DateTimeOffset constant expression element.
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
        /// Asynchronously writes durarion constant expression element.
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
        /// Asynchronously writes decimal constant expression element.
        /// </summary>
        /// <param name="expression">The Edm decimal constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteDecimalConstantExpressionElementAsync(IEdmDecimalConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Decimal, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DecimalAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes floating constant expression element.
        /// </summary>
        /// <param name="expression">The Edm floating constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFloatingConstantExpressionElementAsync(IEdmFloatingConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Float, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.FloatAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes function apply element header.
        /// </summary>
        /// <param name="expression">The Edm apply expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFunctionApplicationElementHeaderAsync(IEdmApplyExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Apply, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Function, expression.AppliedFunction, this.FunctionAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes function apply element end.
        /// </summary>
        /// <param name="expression">The Edm apply expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteFunctionApplicationElementEndAsync(IEdmApplyExpression expression)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes Guid constant expression element.
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
        /// Asynchronously writes integer constant expression element.
        /// </summary>
        /// <param name="expression">The Edm integer constant expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteIntegerConstantExpressionElementAsync(IEdmIntegerConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Int, null).ConfigureAwait(false);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.LongAsXml(expression.Value)).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes path expression element.
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
        /// Asynchronously writes property path expression element.
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
        /// Asynchronously writes navigation property path expression element.
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
        /// Asynchronously writes annotation path expression element.
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
        /// Asynchronously writes 'if' expression element header.
        /// </summary>
        /// <param name="expression">Edm if Expression</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteIfExpressionElementHeaderAsync(IEdmIfExpression expression)
        {
            return this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_If, null);
        }

        /// <summary>
        /// Asynchronously writes If expression element end.
        /// </summary>
        /// <param name="expression">Edm if Expression</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteIfExpressionElementEndAsync(IEdmIfExpression expression)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes collection expression element header.
        /// </summary>
        /// <param name="expression">The Edm collection expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteCollectionExpressionElementHeaderAsync(IEdmCollectionExpression expression)
        {
            return this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Collection, null);
        }

        /// <summary>
        /// Asynchronously writes collection expression element end.
        /// </summary>
        /// <param name="expression">The Edm collection expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteCollectionExpressionElementEndAsync(IEdmCollectionExpression expression)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes labeled element header.
        /// </summary>
        /// <param name="labeledElement">The Edm labeled expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteLabeledElementHeaderAsync(IEdmLabeledExpression labeledElement)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_LabeledElement, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, labeledElement.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes labeled expression reference.
        /// </summary>
        /// <param name="labeledExpressionReference">The Edm labeled expression expression.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression labeledExpressionReference)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_LabeledElementReference, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, labeledExpressionReference.ReferencedLabeledExpression.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes TimeOfDay constant expression element.
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
        /// Asynchronously writes IsType expression header.
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
        /// Asynchronously writes cast expression header.
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
        /// Asynchronously writes cast expression end.
        /// </summary>
        /// <param name="expression">The Edm Cast expression.</param>
        /// <param name="inlineType">Is inline type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteCastExpressionElementEndAsync(IEdmCastExpression expression, bool inlineType)
        {
            return this.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes enum member expression element.
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
        /// Asynchronously writes type definition element header.
        /// </summary>
        /// <param name="typeDefinition">The Edm type definition.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteTypeDefinitionElementHeaderAsync(IEdmTypeDefinition typeDefinition)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_TypeDefinition, null).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, typeDefinition.Name, EdmValueWriter.StringAsXml).ConfigureAwait(false);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_UnderlyingType, typeDefinition.UnderlyingType, this.TypeDefinitionAsXml).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes end element.
        /// </summary>
        internal override Task WriteEndElementAsync()
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes array end element.
        /// </summary>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteArrayEndElementAsync()
        {
            return this.xmlWriter.WriteEndElementAsync();
        }

        /// <summary>
        /// Asynchronously writes optional attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="defaultValue">The attribute default value.</param>
        /// <param name="getStringFunc">Get string function.</param>
        internal async Task WriteOptionalAttributeAsync<T>(string attribute, T value, T defaultValue, Func<T, string> getStringFunc)
        {
            if (!value.Equals(defaultValue))
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, getStringFunc(value)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes optional attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="getStringFunc">Get string function.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal async Task WriteOptionalAttributeAsync<T>(string attribute, T value, Func<T, string> getStringFunc)
        {
            if (value != null)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, getStringFunc(value)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes required attribute.
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
        /// Asynchronously writes operation element attributes.
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
        /// Asynchronously writes navigation property binding.
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
        /// Asynchronously writes operation import attributes.
        /// </summary>
        /// <param name="operationImport">The Edm operation import.</param>
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
