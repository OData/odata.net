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
using System.Threading.Tasks;
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
        private readonly CsdlXmlWriterSettings writerSettings;

        internal EdmModelCsdlSchemaXmlWriter(IEdmModel model, XmlWriter xmlWriter, Version edmVersion, CsdlXmlWriterSettings csdlXmlWriterSettings)
            : base(model, edmVersion)
        {
            this.xmlWriter = xmlWriter;
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmVersion];

            Debug.Assert(csdlXmlWriterSettings != null, "Writer settings must be initialized.");

            this.writerSettings = csdlXmlWriterSettings;
        }

        internal override void WriteReferenceElementHeader(IEdmReference reference)
        {
            // e.g. <edmx:Reference Uri="http://host/schema/VipCustomer.xml">
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Reference, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Uri, reference.Uri, EdmValueWriter.UriAsXml);
        }

        /// <summary>
        /// Asynchronously writes Reference element header.
        /// </summary>
        /// <param name="reference">edmx:reference element</param>
        /// <returns></returns>
        internal override async Task WriteReferenceElementHeaderAsync(IEdmReference reference)
        {
            // e.g. <edmx:Reference Uri="http://host/schema/VipCustomer.xml">
            await this.xmlWriter.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Reference, this.edmxNamespace);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Uri, reference.Uri, EdmValueWriter.UriAsXml);
        }

        internal override void WriteReferenceElementEnd(IEdmReference reference)
        {
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the reference element end.
        /// </summary>
        /// <param name="reference">edmx:reference element</param>
        /// <returns></returns>
        internal override async Task WriteReferenceElementEndAsync(IEdmReference reference)
        {
            await this.xmlWriter.WriteEndElementAsync();
        }

        internal override void WritIncludeElementHeader(IEdmInclude include)
        {
            // e.g. <edmx:Include Namespace="NS.Ref1" Alias="VPCT" />
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Include, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Namespace, include.Namespace, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Alias, include.Alias, EdmValueWriter.StringAsXml);
        }

        /// <summary>
        /// Asynchronously writes the Include element header.
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        internal override async Task WritIncludeElementHeaderAsync(IEdmInclude include)
        {
            // e.g. <edmx:Include Namespace="NS.Ref1" Alias="VPCT" />
            await this.xmlWriter.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Include, this.edmxNamespace);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Namespace, include.Namespace, EdmValueWriter.StringAsXml);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Alias, include.Alias, EdmValueWriter.StringAsXml);
        }

        internal override void WriteIncludeElementEnd(IEdmInclude include)
        {
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Async writes the IncludeAnnotations end.
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        internal override async Task WriteIncludeElementEndAsync(IEdmInclude include)
        {
            await this.xmlWriter.WriteEndElementAsync();
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

        /// <summary>
        /// Async writes the IncludeAnnotations element.
        /// </summary>
        /// <param name="includeAnnotations"></param>
        /// <returns></returns>
        internal async Task WriteIncludeAnnotationsElementAsync(IEdmIncludeAnnotations includeAnnotations)
        {
            // e.g. <edmx:IncludeAnnotations ... />
            await this.xmlWriter.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_IncludeAnnotations, this.edmxNamespace);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_TermNamespace, includeAnnotations.TermNamespace, EdmValueWriter.StringAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Qualifier, includeAnnotations.Qualifier, EdmValueWriter.StringAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_TargetNamespace, includeAnnotations.TargetNamespace, EdmValueWriter.StringAsXml);
            await this.xmlWriter.WriteEndElementAsync();
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

        /// <summary>
        /// Asynchronously writes Term element header.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        internal override async Task WriteTermElementHeaderAsync(IEdmTerm term, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Term, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, term.Name, EdmValueWriter.StringAsXml);
            if (inlineType && term.Type is not null)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, term.Type, this.TypeReferenceAsXml);
            }

            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_DefaultValue, term.DefaultValue, EdmValueWriter.StringAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_AppliesTo, term.AppliesTo, EdmValueWriter.StringAsXml);
        }

        internal override void WriteComplexTypeElementHeader(IEdmComplexType complexType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ComplexType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, complexType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, complexType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);
        }

        /// <summary>
        /// Asynchronously writes the ComplexType element header.
        /// </summary>
        /// <param name="complexType"></param>
        /// <returns></returns>
        internal override async Task WriteComplexTypeElementHeaderAsync(IEdmComplexType complexType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ComplexType, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, complexType.Name, EdmValueWriter.StringAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_OpenType, complexType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);
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

        /// <summary>
        /// Asynchronously writes the EnumType element header.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        internal override async Task WriteEnumTypeElementHeaderAsync(IEdmEnumType enumType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EnumType, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, enumType.Name, EdmValueWriter.StringAsXml);
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_UnderlyingType, enumType.UnderlyingType, this.TypeDefinitionAsXml);
            }

            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IsFlags, enumType.IsFlags, CsdlConstants.Default_IsFlags, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteEnumTypeElementEnd(IEdmEnumType enumType)
        {
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the EnumContainer element end.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        internal override async Task WriteEnumTypeElementEndAsync(IEdmEnumType enumType)
        {
            await this.WriteEndElementAsync();
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

        /// <summary>
        /// Asynchronously writes the EntityContainer element header.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        internal override async Task WriteEntityContainerElementHeaderAsync(IEdmEntityContainer container)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EntityContainer, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, container.Name, EdmValueWriter.StringAsXml);
            if (container is CsdlSemanticsEntityContainer tmp && tmp.Element is CsdlEntityContainer csdlContainer)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Extends, csdlContainer.Extends, EdmValueWriter.StringAsXml);
            }
        }

        internal override void WriteEntitySetElementHeader(IEdmEntitySet entitySet)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntitySet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntityType, entitySet.EntityType.FullName(), EdmValueWriter.StringAsXml);
        }

        /// <summary>
        /// Asynchronously writes the EntitySet element header.
        /// </summary>
        /// <param name="entitySet"></param>
        /// <returns></returns>
        internal override async Task WriteEntitySetElementHeaderAsync(IEdmEntitySet entitySet)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EntitySet, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_EntityType, entitySet.EntityType.FullName(), EdmValueWriter.StringAsXml);
        }

        internal override void WriteSingletonElementHeader(IEdmSingleton singleton)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Singleton);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, singleton.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, singleton.EntityType.FullName(), EdmValueWriter.StringAsXml);
        }

        /// <summary>
        /// Asynchronously writes the Singleton element header.
        /// </summary>
        /// <param name="singleton"></param>
        /// <returns></returns>
        internal override async Task WriteSingletonElementHeaderAsync(IEdmSingleton singleton)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Singleton, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, singleton.Name, EdmValueWriter.StringAsXml);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, singleton.EntityType.FullName(), EdmValueWriter.StringAsXml);
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

        /// <summary>
        /// Asynchronously writes the EntityType element header.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        internal override async Task WriteEntityTypeElementHeaderAsync(IEdmEntityType entityType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EntityType, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, entityType.Name, EdmValueWriter.StringAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_BaseType, entityType.BaseEntityType(), this.TypeDefinitionAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Abstract, entityType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_OpenType, entityType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);

            // HasStream value should be inherited.  Only have it on base type is sufficient.
            bool writeHasStream = entityType.HasStream && (entityType.BaseEntityType() == null || (entityType.BaseEntityType() != null && !entityType.BaseEntityType().HasStream));
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_HasStream, writeHasStream, CsdlConstants.Default_HasStream, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteDeclaredKeyPropertiesElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Key);
        }

        /// <summary>
        /// Asynchronously writes the key properties element header.
        /// </summary>
        /// <returns></returns>
        internal override async Task WriteDeclaredKeyPropertiesElementHeaderAsync()
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Key, null);
        }

        internal override void WritePropertyRefElement(IEdmStructuralProperty property)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyRef);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        /// <summary>
        /// Async writes the PropertyRef element.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal override async Task WritePropertyRefElementAsync(IEdmStructuralProperty property)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyRef, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            await this.WriteEndElementAsync();
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

        /// <summary>
        /// Asynchronously writes the NavigationProperty element header.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal override async Task WriteNavigationPropertyElementHeaderAsync(IEdmNavigationProperty property)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_NavigationProperty, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);

            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml);
            if (!property.Type.IsCollection() && property.Type.IsNullable != CsdlConstants.Default_Nullable)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Nullable, property.Type.IsNullable, EdmValueWriter.BooleanAsXml);
            }

            if (property.Partner != null)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Partner, property.GetPartnerPath()?.Path, EdmValueWriter.StringAsXml);
            }

            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_ContainsTarget, property.ContainsTarget, CsdlConstants.Default_ContainsTarget, EdmValueWriter.BooleanAsXml);
        }

        internal override void WriteNavigationOnDeleteActionElement(EdmOnDeleteAction operationAction)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_OnDelete);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Action, operationAction.ToString(), EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the NavigationOnDeleteAction element.
        /// </summary>
        /// <param name="operationAction"></param>
        /// <returns></returns>
        internal override async Task WriteNavigationOnDeleteActionElementAsync(EdmOnDeleteAction operationAction)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_OnDelete, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Action, operationAction.ToString(), EdmValueWriter.StringAsXml);
            await this.WriteEndElementAsync();
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

        /// <summary>
        /// Asynchronously writes the Schema element header.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="alias"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        internal override async Task WriteSchemaElementHeaderAsync(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string xmlNamespace = GetCsdlNamespace(EdmVersion);
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Schema, xmlNamespace);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Namespace, schema.Namespace, string.Empty, EdmValueWriter.StringAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Alias, alias, EdmValueWriter.StringAsXml);
            if (mappings != null)
            {
                foreach (KeyValuePair<string, string> mapping in mappings)
                {
                    await this.xmlWriter.WriteAttributeStringAsync(EdmConstants.XmlNamespacePrefix, mapping.Key, null, mapping.Value);
                }
            }
        }

        internal override void WriteAnnotationsElementHeader(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotations);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, annotationsForTarget.Key, EdmValueWriter.StringAsXml);
        }


        /// <summary>
        /// Asynchronously writes the Annotations element header.
        /// </summary>
        /// <param name="annotationsForTarget"></param>
        /// <returns></returns>
        internal override async Task WriteAnnotationsElementHeaderAsync(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Annotations, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Target, annotationsForTarget.Key, EdmValueWriter.StringAsXml);
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

        /// <summary>
        /// Asynchronously writes the StructuralProperty element header.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        internal override async Task WriteStructuralPropertyElementHeaderAsync(IEdmStructuralProperty property, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Property, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml);
            }

            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_DefaultValue, property.DefaultValueString, EdmValueWriter.StringAsXml);
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

        /// <summary>
        /// Asynchronously writes Enum Member element header.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        internal override async Task WriteEnumMemberElementHeaderAsync(IEdmEnumMember member)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Member, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml);
            bool? isExplicit = member.IsValueExplicit(this.Model);
            if (!isExplicit.HasValue || isExplicit.Value)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, CsdlConstants.Attribute_Value, null, EdmValueWriter.LongAsXml(member.Value.Value));
            }
        }

        internal override void WriteEnumMemberElementEnd(IEdmEnumMember member)
        {
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes EnumMember element end.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        internal override async Task WriteEnumMemberElementEndAsync(IEdmEnumMember member)
        {
            await this.xmlWriter.WriteEndElementAsync();
        }

        internal override void WriteNullableAttribute(IEdmTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
        }

        /// <summary>
        /// Asynchronously writes Nullable attribute.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal override async Task WriteNullableAttributeAsync(IEdmTypeReference reference)
        {
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
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

        /// <summary>
        /// Asynchronously writes TypeDefinition attributes.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal override async Task WriteTypeDefinitionAttributesAsync(IEdmTypeDefinitionReference reference)
        {
            IEdmTypeReference actualTypeReference = reference.AsActualTypeReference();

            if (actualTypeReference.IsBinary())
            {
                await this.WriteBinaryTypeAttributesAsync(actualTypeReference.AsBinary());
            }
            else if (actualTypeReference.IsString())
            {
                await this.WriteStringTypeAttributesAsync(actualTypeReference.AsString());
            }
            else if (actualTypeReference.IsTemporal())
            {
                await this.WriteTemporalTypeAttributesAsync(actualTypeReference.AsTemporal());
            }
            else if (actualTypeReference.IsDecimal())
            {
                await this.WriteDecimalTypeAttributesAsync(actualTypeReference.AsDecimal());
            }
            else if (actualTypeReference.IsSpatial())
            {
                await this.WriteSpatialTypeAttributesAsync(actualTypeReference.AsSpatial());
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

        /// <summary>
        /// Asynchronously writes BinaryType attributes.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal override async Task WriteBinaryTypeAttributesAsync(IEdmBinaryTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }
            else
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
            }
        }

        internal override void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml);

            // Starting with versions > 7.10.0 we always write the scale even if it's the default scale.
            // This is because we changed default scale from 0 to null/variable and wanted
            // to preserve backwards compatibility with code which expected null scale to be written out to the CSDL by default
            // see PR: https://github.com/OData/odata.net/pull/2346
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Scale, reference.Scale, ScaleAsXml);
        }

        /// <summary>
        /// Asynchronously writes DecimalType attributes.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal override async Task WriteDecimalTypeAttributesAsync(IEdmDecimalTypeReference reference)
        {
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Scale, reference.Scale, ScaleAsXml);
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

        /// <summary>
        /// Asynchronously writes SpatialType attributes.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal override async Task WriteSpatialTypeAttributesAsync(IEdmSpatialTypeReference reference)
        {
            if (reference.IsGeography())
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeographySrid, SridAsXml);
            }
            else if (reference.IsGeometry())
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeometrySrid, SridAsXml);
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

        /// <summary>
        /// Asynchronously writes StringType attributes.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal override async Task WriteStringTypeAttributesAsync(IEdmStringTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }
            else
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
            }

            if (reference.IsUnicode != null)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Unicode, reference.IsUnicode, CsdlConstants.Default_IsUnicode, EdmValueWriter.BooleanAsXml);
            }
        }

        internal override void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision, EdmValueWriter.IntAsXml);
            }
        }

        /// <summary>
        /// Asynchronously writes TemporalType attributes.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal override async Task WriteTemporalTypeAttributesAsync(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision is not null)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision, EdmValueWriter.IntAsXml);
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

        /// <summary>
        /// Asynchronously writes ReferentialConstraint pair.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        internal override async Task WriteReferentialConstraintPairAsync(EdmReferentialConstraintPropertyPair pair)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ReferentialConstraint, null);

            // <EntityType Name="Product">
            //   ...
            //   <Property Name="CategoryID" Type="Edm.String" Nullable="false"/>
            //  <NavigationProperty Name="Category" Type="Self.Category" Nullable="false">
            //     <ReferentialConstraint Property="CategoryID" ReferencedProperty="ID" />
            //   </NavigationProperty>
            // </EntityType>
            // the above CategoryID is DependentProperty, ID is PrincipalProperty.
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Property, pair.DependentProperty.Name, EdmValueWriter.StringAsXml);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_ReferencedProperty, pair.PrincipalProperty.Name, EdmValueWriter.StringAsXml);
            await this.WriteEndElementAsync();
        }

        internal override void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteAttributeString(annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(edmValue));
            }
        }

        /// <summary>
        /// Asynchronously writes Annotation string attribute.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        internal override async Task WriteAnnotationStringAttributeAsync(IEdmDirectValueAnnotation annotation)
        {
            if (annotation.Value is IEdmPrimitiveValue edmValue)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(edmValue));
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

        /// <summary>
        /// Asynchronously writes Annotation string element.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        internal override async Task WriteAnnotationStringElementAsync(IEdmDirectValueAnnotation annotation)
        {
            if (annotation.Value is IEdmPrimitiveValue edmValue)
            {
                await this.xmlWriter.WriteRawAsync(((IEdmStringValue)edmValue).Value);
            }
        }

        internal override void WriteActionElementHeader(IEdmAction action)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Action);
            this.WriteOperationElementAttributes(action);
        }

        /// <summary>
        /// Asynchronously writes Action element header.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        internal override async Task WriteActionElementHeaderAsync(IEdmAction action)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Action, null);
            await this.WriteOperationElementAttributesAsync(action);
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

        /// <summary>
        /// Asynchronously writes Function element header.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        internal override async Task WriteFunctionElementHeaderAsync(IEdmFunction function)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Function, null);
            await this.WriteOperationElementAttributesAsync(function);

            if (function.IsComposable)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IsComposable, function.IsComposable, EdmValueWriter.BooleanAsXml);
            }
        }

        internal override void WriteReturnTypeElementHeader(IEdmOperationReturn operationReturn)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReturnType);
        }

        /// <summary>
        /// Asynchronously writes ReturnType element header.
        /// </summary>
        /// <param name="operationReturn"></param>
        /// <returns></returns>
        internal override async Task WriteReturnTypeElementHeaderAsync(IEdmOperationReturn operationReturn)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ReturnType, null);
        }

        internal override void WriteTypeAttribute(IEdmTypeReference typeReference)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, typeReference, this.TypeReferenceAsXml);
        }

        /// <summary>
        /// Asynchronously writes Type attribute.
        /// </summary>
        /// <param name="typeReference"></param>
        /// <returns></returns>
        internal override async Task WriteTypeAttributeAsync(IEdmTypeReference typeReference)
        {
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, typeReference, this.TypeReferenceAsXml);
        }

        internal override void WriteActionImportElementHeader(IEdmActionImport actionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ActionImport);
            this.WriteOperationImportAttributes(actionImport, CsdlConstants.Attribute_Action);
        }

        /// <summary>
        /// Asynchronously writes ActionImport element header.
        /// </summary>
        /// <param name="actionImport"></param>
        /// <returns></returns>
        internal override async Task WriteActionImportElementHeaderAsync(IEdmActionImport actionImport)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_ActionImport, null);
            await this.WriteOperationImportAttributesAsync(actionImport, CsdlConstants.Attribute_Action);
        }

        internal override void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_FunctionImport);
            this.WriteOperationImportAttributes(functionImport, CsdlConstants.Attribute_Function);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_IncludeInServiceDocument, functionImport.IncludeInServiceDocument, CsdlConstants.Default_IncludeInServiceDocument, EdmValueWriter.BooleanAsXml);
        }

        /// <summary>
        /// Asynchronously writes FunctionImport element header.
        /// </summary>
        /// <param name="functionImport"></param>
        /// <returns></returns>
        internal override async Task WriteFunctionImportElementHeaderAsync(IEdmFunctionImport functionImport)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_FunctionImport, null);
            await this.WriteOperationImportAttributesAsync(functionImport, CsdlConstants.Attribute_Function);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IncludeInServiceDocument, functionImport.IncludeInServiceDocument, CsdlConstants.Default_IncludeInServiceDocument, EdmValueWriter.BooleanAsXml);
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

        /// <summary>
        /// Asynchronously writes OperationParameter element header.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        internal override async Task WriteOperationParameterElementHeaderAsync(IEdmOperationParameter parameter, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Parameter, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, parameter.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, parameter.Type, this.TypeReferenceAsXml);
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

        /// <summary>
        /// Asynchronously writes OperationParameter end element.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal override async Task WriteOperationParameterEndElementAsync(IEdmOperationParameter parameter)
        {
            if (parameter is IEdmOptionalParameter optionalParameter && 
                !(optionalParameter.VocabularyAnnotations(this.Model).Any(a => a.Term == CoreVocabularyModel.OptionalParameterTerm)))
            {
                var optionalValue = new EdmRecordExpression();

                var vocabularyAnnotation = new EdmVocabularyAnnotation(parameter, CoreVocabularyModel.OptionalParameterTerm, optionalValue);
                await this.WriteVocabularyAnnotationElementHeaderAsync(vocabularyAnnotation, false);

                string defaultValue = optionalParameter.DefaultValueString;
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    var property = new EdmPropertyConstructor(CsdlConstants.Attribute_DefaultValue, new EdmStringConstant(defaultValue));
                    await this.WriteRecordExpressionElementHeaderAsync(optionalValue);
                    await this.WritePropertyValueElementHeaderAsync(property, true);
                    await this.WriteEndElementAsync();
                    await this.WriteEndElementAsync();
                }

                await this.WriteEndElementAsync();
            }

            await this.WriteEndElementAsync();
        }

        internal override void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_CollectionType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_ElementType, collectionType.ElementType, this.TypeReferenceAsXml);
            }
        }

        /// <summary>
        /// Asynchronously writes CollectionType element header.
        /// </summary>
        /// <param name="collectionType"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        internal override async Task WriteCollectionTypeElementHeaderAsync(IEdmCollectionType collectionType, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_CollectionType, null);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_ElementType, collectionType.ElementType, this.TypeReferenceAsXml);
            }
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

        /// <summary>
        /// Asynchronously writes Inline expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteInlineExpressionAsync(IEdmExpression expression)
        {
            IEdmPathExpression pathExpression = expression as IEdmPathExpression;
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.BinaryConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Binary, ((IEdmBinaryConstantExpression)expression).Value, EdmValueWriter.BinaryAsXml);
                    break;
                case EdmExpressionKind.BooleanConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Bool, ((IEdmBooleanConstantExpression)expression).Value, EdmValueWriter.BooleanAsXml);
                    break;
                case EdmExpressionKind.DateTimeOffsetConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_DateTimeOffset, ((IEdmDateTimeOffsetConstantExpression)expression).Value, EdmValueWriter.DateTimeOffsetAsXml);
                    break;
                case EdmExpressionKind.DecimalConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Decimal, ((IEdmDecimalConstantExpression)expression).Value, EdmValueWriter.DecimalAsXml);
                    break;
                case EdmExpressionKind.FloatingConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Float, ((IEdmFloatingConstantExpression)expression).Value, EdmValueWriter.FloatAsXml);
                    break;
                case EdmExpressionKind.GuidConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Guid, ((IEdmGuidConstantExpression)expression).Value, EdmValueWriter.GuidAsXml);
                    break;
                case EdmExpressionKind.IntegerConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Int, ((IEdmIntegerConstantExpression)expression).Value, EdmValueWriter.LongAsXml);
                    break;
                case EdmExpressionKind.Path:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Path, pathExpression.PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.PropertyPath:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_PropertyPath, pathExpression.PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.NavigationPropertyPath:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_NavigationPropertyPath, pathExpression.PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.AnnotationPath:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_AnnotationPath, pathExpression.PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.StringConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_String, ((IEdmStringConstantExpression)expression).Value, EdmValueWriter.StringAsXml);
                    break;
                case EdmExpressionKind.DurationConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Duration, ((IEdmDurationConstantExpression)expression).Value, EdmValueWriter.DurationAsXml);
                    break;
                case EdmExpressionKind.DateConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Date, ((IEdmDateConstantExpression)expression).Value, EdmValueWriter.DateAsXml);
                    break;
                case EdmExpressionKind.TimeOfDayConstant:
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_TimeOfDay, ((IEdmTimeOfDayConstantExpression)expression).Value, EdmValueWriter.TimeOfDayAsXml);
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

        /// <summary>
        /// Asynchronously writes PropertyConstructor element end.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        internal override async Task WritePropertyConstructorElementEndAsync(IEdmPropertyConstructor constructor)
        {
            await this.WriteEndElementAsync();
        }

        internal override void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotation);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml);

            if (isInline && !annotation.UsesDefault)
            {
                // in xml format, we can (should) skip writing the expression value if it matches the term default value.
                this.WriteInlineExpression(annotation.Value);
            }
        }

        /// <summary>
        /// Asynchronously writes VocabularyAnnotation element header.
        /// </summary>
        /// <param name="annotation"></param>
        /// <param name="isInline"></param>
        /// <returns></returns>
        internal override async Task WriteVocabularyAnnotationElementHeaderAsync(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Annotation, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml);

            if (isInline && !annotation.UsesDefault)
            {
                // in xml format, we can (should) skip writing the expression value if it matches the term default value.
                await this.WriteInlineExpressionAsync(annotation.Value);
            }
        }

        internal override void WriteVocabularyAnnotationElementEnd(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes VocabularyAnnotation element end.
        /// </summary>
        /// <param name="annotation"></param>
        /// <param name="isInline"></param>
        /// <returns></returns>
        internal override async Task WriteVocabularyAnnotationElementEndAsync(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            await WriteEndElementAsync();
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

        /// <summary>
        /// Asynchronously writes PropertyValue element header.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isInline"></param>
        /// <returns></returns>
        internal override async Task WritePropertyValueElementHeaderAsync(IEdmPropertyConstructor value, bool isInline)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyValue, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Property, value.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                await this.WriteInlineExpressionAsync(value.Value);
            }
        }

        internal override void WriteRecordExpressionElementHeader(IEdmRecordExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Record);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Type, expression.DeclaredType, this.TypeReferenceAsXml);
        }

        /// <summary>
        /// Asynchronously writes RecordExpression element header.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteRecordExpressionElementHeaderAsync(IEdmRecordExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Record, null);
            await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_Type, expression.DeclaredType, this.TypeReferenceAsXml);
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

        /// <summary>
        /// Asynchronously writes PropertyConstructor element header.
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="isInline"></param>
        /// <returns></returns>
        internal override async Task WritePropertyConstructorElementHeaderAsync(IEdmPropertyConstructor constructor, bool isInline)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyValue, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Property, constructor.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                await this.WriteInlineExpressionAsync(constructor.Value);
            }
        }

        internal override void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_String);

            this.xmlWriter.WriteString(EdmValueWriter.StringAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes StringConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteStringConstantExpressionElementAsync(IEdmStringConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_String, null);

            await this.xmlWriter.WriteStringAsync(EdmValueWriter.StringAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Binary);
            this.xmlWriter.WriteString(EdmValueWriter.BinaryAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes BinaryConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteBinaryConstantExpressionElementAsync(IEdmBinaryConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Binary, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.BinaryAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Bool);
            this.xmlWriter.WriteString(EdmValueWriter.BooleanAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes BooleanConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteBooleanConstantExpressionElementAsync(IEdmBooleanConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Bool, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.BooleanAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteNullConstantExpressionElement(IEdmNullExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Null);
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes NullConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteNullConstantExpressionElementAsync(IEdmNullExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Null, null);
            await this.WriteEndElementAsync();
        }

        internal override void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Date);
            this.xmlWriter.WriteString(EdmValueWriter.DateAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes DateConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteDateConstantExpressionElementAsync(IEdmDateConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Date, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DateAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_DateTimeOffset);
            this.xmlWriter.WriteString(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes DateTimeOffsetConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteDateTimeOffsetConstantExpressionElementAsync(IEdmDateTimeOffsetConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_DateTimeOffset, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Duration);
            this.xmlWriter.WriteString(EdmValueWriter.DurationAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes DurationConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteDurationConstantExpressionElementAsync(IEdmDurationConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Duration, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DurationAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Decimal);
            this.xmlWriter.WriteString(EdmValueWriter.DecimalAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes DecimalConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteDecimalConstantExpressionElementAsync(IEdmDecimalConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Decimal, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.DecimalAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Float);
            this.xmlWriter.WriteString(EdmValueWriter.FloatAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes FloatingConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteFloatingConstantExpressionElementAsync(IEdmFloatingConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Float, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.FloatAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Apply);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Function, expression.AppliedFunction, this.FunctionAsXml);
        }

        /// <summary>
        /// Asynchronously writes FunctionApplication element header.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteFunctionApplicationElementHeaderAsync(IEdmApplyExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Apply, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Function, expression.AppliedFunction, this.FunctionAsXml);
        }

        internal override void WriteFunctionApplicationElementEnd(IEdmApplyExpression expression)
        {
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes FunctionApplication element end.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteFunctionApplicationElementEndAsync(IEdmApplyExpression expression)
        {
            await this.WriteEndElementAsync();
        }

        internal override void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Guid);
            this.xmlWriter.WriteString(EdmValueWriter.GuidAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes GuidConstantExpression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteGuidConstantExpressionElementAsync(IEdmGuidConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Guid, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.GuidAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Int);
            this.xmlWriter.WriteString(EdmValueWriter.LongAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes IntegerConstant Expression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteIntegerConstantExpressionElementAsync(IEdmIntegerConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Int, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.LongAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WritePathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Path);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes Path Expression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WritePathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Path, null);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments));
            await this.WriteEndElementAsync();
        }

        internal override void WritePropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }


        /// <summary>
        /// Asynchronously writes PropertyPath Expression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WritePropertyPathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_PropertyPath, null);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments));
            await this.WriteEndElementAsync();
        }

        internal override void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationPropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes NavigationPropertyPath Expression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteNavigationPropertyPathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_NavigationPropertyPath, null);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments));
            await this.WriteEndElementAsync();
        }

        internal override void WriteAnnotationPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_AnnotationPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes AnnotationPath Expression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteAnnotationPathExpressionElementAsync(IEdmPathExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_AnnotationPath, null);
            await this.xmlWriter.WriteStringAsync(PathAsXml(expression.PathSegments));
            await this.WriteEndElementAsync();
        }

        internal override void WriteIfExpressionElementHeader(IEdmIfExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_If);
        }

        /// <summary>
        /// Asynchronously writes If Expression element header.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteIfExpressionElementHeaderAsync(IEdmIfExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_If, null);
        }

        internal override void WriteIfExpressionElementEnd(IEdmIfExpression expression)
        {
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes If Expression element end.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteIfExpressionElementEndAsync(IEdmIfExpression expression)
        {
            await this.WriteEndElementAsync();
        }

        internal override void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Collection);
        }

        /// <summary>
        /// Asynchronously writes Collection Expression element header.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteCollectionExpressionElementHeaderAsync(IEdmCollectionExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Collection, null);
        }

        internal override void WriteCollectionExpressionElementEnd(IEdmCollectionExpression expression)
        {
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes Collection Expression element end.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteCollectionExpressionElementEndAsync(IEdmCollectionExpression expression)
        {
            await this.WriteEndElementAsync();
        }

        internal override void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_LabeledElement);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, labeledElement.Name, EdmValueWriter.StringAsXml);
        }

        /// <summary>
        /// Asynchronously writes LabeledElement header.
        /// </summary>
        /// <param name="labeledElement"></param>
        /// <returns></returns>
        internal override async Task WriteLabeledElementHeaderAsync(IEdmLabeledExpression labeledElement)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_LabeledElement, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, labeledElement.Name, EdmValueWriter.StringAsXml);
        }

        internal override void WriteLabeledExpressionReferenceExpression(IEdmLabeledExpressionReferenceExpression labeledExpressionReference)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_LabeledElementReference);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, labeledExpressionReference.ReferencedLabeledExpression.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes LabeledExpressionReference Expression.
        /// </summary>
        /// <param name="labeledExpressionReference"></param>
        /// <returns></returns>
        internal override async Task WriteLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression labeledExpressionReference)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_LabeledElementReference, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, labeledExpressionReference.ReferencedLabeledExpression.Name, EdmValueWriter.StringAsXml);
            await this.WriteEndElementAsync();
        }

        internal override void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TimeOfDay);
            this.xmlWriter.WriteString(EdmValueWriter.TimeOfDayAsXml(expression.Value));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes TimeOfDay Constant Expression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteTimeOfDayConstantExpressionElementAsync(IEdmTimeOfDayConstantExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_TimeOfDay, null);
            await this.xmlWriter.WriteStringAsync(EdmValueWriter.TimeOfDayAsXml(expression.Value));
            await this.WriteEndElementAsync();
        }

        internal override void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_IsType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }


        /// <summary>
        /// Asynchronously writes IsType Expression header.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        internal override async Task WriteIsTypeExpressionElementHeaderAsync(IEdmIsTypeExpression expression, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_IsType, null);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
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

        /// <summary>
        /// Asynchronously writes Cast Expression header.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        internal override async Task WriteCastExpressionElementHeaderAsync(IEdmCastExpression expression, bool inlineType)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_Cast, null);
            if (inlineType)
            {
                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal override void WriteCastExpressionElementEnd(IEdmCastExpression expression, bool inlineType)
        {
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes Cast Expression end.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        internal override async Task WriteCastExpressionElementEndAsync(IEdmCastExpression expression, bool inlineType)
        {
            await this.WriteEndElementAsync();
        }

        internal override void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumMember);
            this.xmlWriter.WriteString(EnumMemberAsXmlOrJson(expression.EnumMembers));
            this.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes EnumMember Expression element.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal override async Task WriteEnumMemberExpressionElementAsync(IEdmEnumMemberExpression expression)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_EnumMember, null);
            await this.xmlWriter.WriteStringAsync(EnumMemberAsXmlOrJson(expression.EnumMembers));
            await this.WriteEndElementAsync();
        }

        internal override void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TypeDefinition);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, typeDefinition.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, typeDefinition.UnderlyingType, this.TypeDefinitionAsXml);
        }

        /// <summary>
        /// Asynchronously writes TypeDefinition element header.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <returns></returns>
        internal override async Task WriteTypeDefinitionElementHeaderAsync(IEdmTypeDefinition typeDefinition)
        {
            await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_TypeDefinition, null);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, typeDefinition.Name, EdmValueWriter.StringAsXml);
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_UnderlyingType, typeDefinition.UnderlyingType, this.TypeDefinitionAsXml);
        }

        internal override void WriteEndElement()
        {
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes End Element.
        /// </summary>
        /// <returns></returns>
        internal override async Task WriteEndElementAsync()
        {
            await this.xmlWriter.WriteEndElementAsync();
        }

        internal override void WriteArrayEndElement()
        {
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes Array End Element.
        /// </summary>
        /// <returns></returns>
        internal override async Task WriteArrayEndElementAsync()
        {
            await this.xmlWriter.WriteEndElementAsync();
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, T defaultValue, Func<T, string> getStringFunc)
        {
            if (!value.Equals(defaultValue))
            {
                this.xmlWriter.WriteAttributeString(attribute, getStringFunc(value));
            }
        }

        /// <summary>
        /// Asynchronously writes Optional attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="getStringFunc"></param>
        /// <returns></returns>
        internal async Task WriteOptionalAttributeAsync<T>(string attribute, T value, T defaultValue, Func<T, string> getStringFunc)
        {
            if (!value.Equals(defaultValue))
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, getStringFunc(value));
            }
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, Func<T, string> getStringFunc)
        {
            if (value != null)
            {
                this.xmlWriter.WriteAttributeString(attribute, getStringFunc(value));
            }
        }

        /// <summary>
        /// Asynchronously writes Optional attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="getStringFunc"></param>
        /// <returns></returns>
        internal async Task WriteOptionalAttributeAsync<T>(string attribute, T value, Func<T, string> getStringFunc)
        {
            if (value != null)
            {
                await this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, getStringFunc(value));
            }
        }

        internal void WriteRequiredAttribute<T>(string attribute, T value, Func<T, string> toXml)
        {
            this.xmlWriter.WriteAttributeString(attribute, toXml(value));
        }

        /// <summary>
        /// Asynchronously writes Required attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="toXml"></param>
        /// <returns></returns>
        internal async Task WriteRequiredAttributeAsync<T>(string attribute, T value, Func<T, string> toXml)
        {
            await this.xmlWriter.WriteAttributeStringAsync(null, attribute, null, toXml(value));
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

        /// <summary>
        /// Asynchronously writes Operation element attributes.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        internal override async Task WriteOperationElementAttributesAsync(IEdmOperation operation)
        {
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, operation.Name, EdmValueWriter.StringAsXml);

            if (operation.IsBound)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_IsBound, operation.IsBound, EdmValueWriter.BooleanAsXml);
            }

            if (operation.EntitySetPath != null)
            {
                await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_EntitySetPath, operation.EntitySetPath.PathSegments, PathAsXml);
            }
        }

        internal override void WriteNavigationPropertyBinding(IEdmNavigationPropertyBinding binding)
        {
            // For backwards compatability, only write annotations that vary by type cast in versions > 4.0
            if (this.Model.GetEdmVersion() > EdmConstants.EdmVersion4 || binding.Path.PathSegments.Last().IndexOf('.', StringComparison.Ordinal) < 0)
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
        }

        /// <summary>
        /// Asynchronously writes NavigationPropertyBinding.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        internal override async Task WriteNavigationPropertyBindingAsync(IEdmNavigationPropertyBinding binding)
        {
            // For backwards compatability, only write annotations that vary by type cast in versions > 4.0
            if (this.Model.GetEdmVersion() > EdmConstants.EdmVersion4 || binding.Path.PathSegments.Last().IndexOf('.', StringComparison.Ordinal) < 0)
            {
                await this.xmlWriter.WriteStartElementAsync(null, CsdlConstants.Element_NavigationPropertyBinding, null);

                await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Path, binding.Path.Path, EdmValueWriter.StringAsXml);

                // TODO: handle container names, etc.
                if (binding.Target is IEdmContainedEntitySet containedEntitySet)
                {
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Target, containedEntitySet.Path.Path, EdmValueWriter.StringAsXml);
                }
                else
                {
                    await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Target, binding.Target.Name, EdmValueWriter.StringAsXml);
                }

                await this.xmlWriter.WriteEndElementAsync();
            }
        }

        private string SridAsXml(int? i)
        {
            if (this.writerSettings.LibraryCompatibility.HasFlag(EdmLibraryCompatibility.UseLegacyVariableCasing))
            {
                return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_SridVariable_Legacy;
            }
            else
            {
                return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_SridVariable;
            }
        }

        private string ScaleAsXml(int? i)
        {
            if (this.writerSettings.LibraryCompatibility.HasFlag(EdmLibraryCompatibility.UseLegacyVariableCasing))
            {
                return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_ScaleVariable_Legacy;
            }
            else
            {
                return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_ScaleVariable;
            }
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

        /// <summary>
        /// Asynchronously writes OperationImport attributes.
        /// </summary>
        /// <param name="operationImport"></param>
        /// <param name="operationAttributeName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal override async Task WriteOperationImportAttributesAsync(IEdmOperationImport operationImport, string operationAttributeName)
        {
            await this.WriteRequiredAttributeAsync(CsdlConstants.Attribute_Name, operationImport.Name, EdmValueWriter.StringAsXml);
            await this.WriteRequiredAttributeAsync(operationAttributeName, operationImport.Operation.FullName(), EdmValueWriter.StringAsXml);

            if (operationImport.EntitySet != null)
            {
                if (operationImport.EntitySet is IEdmPathExpression pathExpression)
                {
                    await this.WriteOptionalAttributeAsync(CsdlConstants.Attribute_EntitySet, pathExpression.PathSegments, PathAsXml);
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
