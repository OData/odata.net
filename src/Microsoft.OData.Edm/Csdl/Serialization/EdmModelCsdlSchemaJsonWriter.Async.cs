﻿//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaJsonWriter.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// OData Common Schema Definition Language (CSDL) JSON writer
    /// </summary>
    internal partial class EdmModelCsdlSchemaJsonWriter : EdmModelCsdlSchemaWriter
    {

        /// <summary>
        /// Asynchronously writes reference element header.
        /// </summary>
        /// <param name="reference">The Edm reference.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteReferenceElementHeaderAsync(IEdmReference reference)
        {
            // The name of the pair is a URI for the referenced document.
            this.jsonWriter.WritePropertyName(reference.Uri.OriginalString);

            // The value of each member is a reference object.
            this.jsonWriter.WriteStartObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes reference element end.
        /// </summary>
        /// <param name="reference">The Edm reference element.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteReferenceElementEndAsync(IEdmReference reference)
        {
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes include element header.
        /// </summary>
        /// <param name="include">The Edm include information.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WritIncludeElementHeaderAsync(IEdmInclude include)
        {
            // Array items are objects.
            this.jsonWriter.WriteStartObject();

            // MUST contain the member $Namespace
            this.jsonWriter.WriteRequiredProperty("$Namespace", include.Namespace);

            // MAY contain the member $Alias
            this.jsonWriter.WriteOptionalProperty("$Alias", include.Alias);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes include element end.
        /// </summary>
        /// <param name="include">The Edm include information.</param>
        /// <returns>Task representing asynchronous</returns>
        internal override Task WriteIncludeElementEndAsync(IEdmInclude include)
        {
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes term object header.
        /// </summary>
        /// <param name="term">The Edm term</param>
        /// <param name="inlineType">Is inline type or not.</param>
        internal override async Task WriteTermElementHeaderAsync(IEdmTerm term, bool inlineType)
        {
            // A term is represented as a member of the schema object whose name is the unqualified name of the term.
            this.jsonWriter.WritePropertyName(term.Name);

            // whose value is an object.
            this.jsonWriter.WriteStartObject();

            // The term object MUST contain the member $Kind with a string value of Term.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_Term);

            // It MAY contain the members $Type, $Collection.
            if (inlineType && term.Type != null)
            {
                await WriteTypeReferenceAsync(term.Type).ConfigureAwait(false);
            }

            // A term MAY specialize another term in scope by specifying it as its base term.
            // The value of $BaseTerm is the qualified name of the base term. So far, it's not supported.

            // It MAY contain the members $AppliesTo.
            // The value of $AppliesTo is an array whose items are strings containing symbolic values from a table
            // that identify model elements the term is intended to be applied to.
            if (term.AppliesTo != null)
            {
                string[] appliesTo = term.AppliesTo.Split(',');
                this.jsonWriter.WritePropertyName("$AppliesTo");
                this.jsonWriter.WriteStartArray();
                foreach (string applyTo in appliesTo)
                {
                    this.jsonWriter.WriteStringValue(applyTo);
                }
                this.jsonWriter.WriteEndArray();
            }

            // It MAY contain the members $DefaultValue.
            // The value of $DefaultValue is the type-specific JSON representation of the default value of the term
            this.jsonWriter.WriteOptionalProperty("$DefaultValue", term.DefaultValue);
        }

        /// <summary>
        /// Asynchronously writes entity type object.
        /// </summary>
        /// <param name="entityType">The Edm entity type.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteEntityTypeElementHeaderAsync(IEdmEntityType entityType)
        {
            // An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type
            this.jsonWriter.WritePropertyName(entityType.Name);

            // whose value is an object
            this.jsonWriter.WriteStartObject();

            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_EntityType);

            // It MAY contain the members $BaseType
            this.jsonWriter.WriteOptionalProperty("$BaseType", entityType.BaseEntityType(), this.TypeDefinitionAsJson);

            // It MAY contain the members $Abstract, The value of $Abstract is one of the Boolean literals true or false. Absence of the member means false.
            this.jsonWriter.WriteOptionalProperty("$Abstract", entityType.IsAbstract, CsdlConstants.Default_Abstract);

            // It MAY contain the members $OpenType, The value of $OpenType is one of the Boolean literals true or false. Absence of the member means false.
            this.jsonWriter.WriteOptionalProperty("$OpenType", entityType.IsOpen, CsdlConstants.Default_OpenType);

            // It MAY contain the members $HasStream, The value of $HasStream is one of the Boolean literals true or false. Absence of the member means false
            this.jsonWriter.WriteOptionalProperty("$HasStream", entityType.HasStream, CsdlConstants.Default_HasStream);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes complex type object.
        /// </summary>
        /// <param name="complexType">The Edm complex type.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteComplexTypeElementHeaderAsync(IEdmComplexType complexType)
        {
            // A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type
            this.jsonWriter.WritePropertyName(complexType.Name);

            // whose value is an object
            this.jsonWriter.WriteStartObject();

            // The complex type object MUST contain the member $Kind with a string value of ComplexType.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_ComplexType);

            // It MAY contain the members $BaseType
            this.jsonWriter.WriteOptionalProperty("$BaseType", complexType.BaseComplexType(), this.TypeDefinitionAsJson);

            // It MAY contain the members $Abstract, The value of $Abstract is one of the Boolean literals true or false. Absence of the member means false.
            this.jsonWriter.WriteOptionalProperty("$Abstract", complexType.IsAbstract, CsdlConstants.Default_Abstract);

            // It MAY contain the members $OpenType, The value of $OpenType is one of the Boolean literals true or false. Absence of the member means false.
            this.jsonWriter.WriteOptionalProperty("$OpenType", complexType.IsOpen, CsdlConstants.Default_OpenType);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes key.
        /// </summary>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteDeclaredKeyPropertiesElementHeaderAsync()
        {
            // The value of $Key is an array with one item per key property.
            this.jsonWriter.WritePropertyName("$Key");

            // Its value is an array.
            this.jsonWriter.WriteStartArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes the key property.
        /// </summary>
        /// <param name="property">The Edm structural property.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WritePropertyRefElementAsync(IEdmStructuralProperty property)
        {
            // Key properties without a key alias are represented as strings containing the property name.
            // Key properties with a key alias are represented as objects with one member whose name is the key alias and whose value is a string containing the path to the property.
            this.jsonWriter.WriteStringValue(property.Name);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes navigation property object.
        /// </summary>
        /// <param name="property">The Edm navigation property.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteNavigationPropertyElementHeaderAsync(IEdmNavigationProperty property)
        {
            // Navigation properties are represented as members of the object representing a structured type. The member name is the property name.
            this.jsonWriter.WritePropertyName(property.Name);

            // the member value is an object
            this.jsonWriter.WriteStartObject();

            // The navigation property object MUST contain the member $Kind with a string value of NavigationProperty.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_NavigationProperty);

            // It MUST contain the member $Type (because the navigation property type never be Edm.String)
            // It MAY contain the members $Collection.
            await WriteTypeReferenceAsync(property.Type).ConfigureAwait(false);

            // It MAY contain the members $Partner.
            // A navigation property of an entity type MAY specify a partner navigation property. Navigation properties of complex types MUST NOT specify a partner.
            // So far, it doesn't support to set the path.
            if (property.Partner != null)
            {
                IEdmPathExpression pathExpression = property.GetPartnerPath();
                if (pathExpression != null)
                {
                    this.jsonWriter.WriteRequiredProperty("$Partner", property.GetPartnerPath().Path);
                }
            }

            // It MAY contain the members $Collection, $Nullable, $Partner, $ContainsTarget
            // The value of $ContainsTarget is one of the Boolean literals true or false. Absence of the member means false.
            this.jsonWriter.WriteOptionalProperty("$ContainsTarget", property.ContainsTarget, CsdlConstants.Default_ContainsTarget);
        }

        /// <summary>
        /// Asynchronously writes referential constraint begin.
        /// </summary>
        /// <param name="referentialConstraint">The Edm referential constraint.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteReferentialConstraintBeginAsync(IEdmReferentialConstraint referentialConstraint)
        {
            // The value of $ReferentialConstraint is an object with one member per referential constraint.
            this.jsonWriter.WritePropertyName("$ReferentialConstraint");

            this.jsonWriter.WriteStartObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes referential constraint end.
        /// </summary>
        /// <param name="referentialConstraint">The Edm referential constraint.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteReferentialConstraintEndAsync(IEdmReferentialConstraint referentialConstraint)
        {
            // It also MAY contain annotations. These are prefixed with the path of the dependent property of the annotated referential constraint.
            // So far, it's not supported.

            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes referential constraint pair.
        /// </summary>
        /// <param name="pair">The Edm referential constraint property pair.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteReferentialConstraintPairAsync(EdmReferentialConstraintPropertyPair pair)
        {
            // One member per referential constraint
            // The member name is the path to the dependent property, this path is relative to the structured type declaring the navigation property.
            this.jsonWriter.WritePropertyName(pair.DependentProperty.Name); // It should be the path, so far, it's not supported.

            // The member value is a string containing the path to the principal property,
            this.jsonWriter.WriteStringValue(pair.PrincipalProperty.Name); // It should be the path, so far it's not supported.

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes navigation OnDelete action element.
        /// </summary>
        /// <param name="operationAction">The Edm OnDelete action.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteNavigationOnDeleteActionElementAsync(EdmOnDeleteAction operationAction)
        {
            // $OnDelete
            this.jsonWriter.WritePropertyName("$OnDelete");

            // The value of $OnDelete is a string with one of the values Cascade, None, SetNull, or SetDefault.
            this.jsonWriter.WriteStringValue(operationAction.ToString());

            // Annotations for $OnDelete are prefixed with $OnDelete. So far, it's not supported now.

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes schema object.
        /// </summary>
        /// <param name="schema">The schema</param>
        /// <param name="alias">The alias</param>
        /// <param name="mappings">The namespace prefix mapping. It's used in XML, Not apply for JSON.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteSchemaElementHeaderAsync(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            this.jsonWriter.WritePropertyName(schema.Namespace);

            // Its value is an object
            this.jsonWriter.WriteStartObject();

            // It MAY contain the members $Alias
            this.jsonWriter.WriteOptionalProperty("$Alias", alias);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes annotations begin : {
        /// </summary>
        /// <param name="outOfLineAnnotations">The total out of line annotations.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteOutOfLineAnnotationsBeginAsync(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            // The value of $Annotations is an object with one member per annotation target.
            this.jsonWriter.WritePropertyName("$Annotations");
            this.jsonWriter.WriteStartObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes annotations with external targeting.
        /// </summary>
        /// <param name="annotationsForTarget">The annotation.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteAnnotationsElementHeaderAsync(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget)
        {
            // The member name is a path identifying the annotation target, the member value is an object containing annotations for that target.
            this.jsonWriter.WritePropertyName(annotationsForTarget.Key);
            this.jsonWriter.WriteStartObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes annotations end: }
        /// </summary>
        /// <param name="outOfLineAnnotations">The total out of line annotations.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteOutOfLineAnnotationsEndAsync(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes structural property object.
        /// </summary>
        /// <param name="property">The Edm structural property.</param>
        /// <param name="inlineType">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteStructuralPropertyElementHeaderAsync(IEdmStructuralProperty property, bool inlineType)
        {
            // Structural properties are represented as members of the object representing a structured type. The member name is the property name.
            this.jsonWriter.WritePropertyName(property.Name);

            // The member value is an object.
            this.jsonWriter.WriteStartObject();

            // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
            // So, we omit the $Kind for property.

            // It MAY contain the member $Type & $Collection
            if (inlineType)
            {
                await WriteTypeReferenceAsync(property.Type).ConfigureAwait(false);
            }

            // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
            // So far, it only includes the string format.
            this.jsonWriter.WriteOptionalProperty("$DefaultValue", property.DefaultValueString);
        }

        /// <summary>
        /// Asynchronously writes enumeration type object header
        /// </summary>
        /// <param name="enumType">The given enumeration type.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteEnumTypeElementHeaderAsync(IEdmEnumType enumType)
        {
            // An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type
            this.jsonWriter.WritePropertyName(enumType.Name);

            // whose value is an object
            this.jsonWriter.WriteStartObject();

            // The enumeration type object MUST contain the member $Kind with a string value of EnumType.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_EnumType);

            // An enumeration type MAY specify one of Edm.Byte, Edm.SByte, Edm.Int16, Edm.Int32, or Edm.Int64 as its underlying type.
            // If not explicitly specified, Edm.Int32 is used as the underlying type.
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                this.jsonWriter.WriteRequiredProperty("$UnderlyingType", enumType.UnderlyingType, this.TypeDefinitionAsJson);
            }

            // The value of $IsFlags is one of the Boolean literals true or false. Absence of the member means false.
            this.jsonWriter.WriteOptionalProperty("$IsFlags", enumType.IsFlags, CsdlConstants.Default_IsFlags);

            this.isInEnumTypeWriting = true;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes enumeration type object end
        /// </summary>
        /// <param name="enumType">The given enumeration type.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteEnumTypeElementEndAsync(IEdmEnumType enumType)
        {
            await WriteEndElementAsync().ConfigureAwait(false);
            this.isInEnumTypeWriting = false;
        }

        /// <summary>
        /// Asynchronously writes enumeration member object start
        /// </summary>
        /// <param name="member">The Edm Enum member.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteEnumMemberElementHeaderAsync(IEdmEnumMember member)
        {
            // Enumeration type members are represented as JSON object members.
            // Member name is the enumeration member name.
            // member value is the enumeration member value
            this.jsonWriter.WriteRequiredProperty(member.Name, member.Value.Value);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes nullable, A boolean value specifying whether a value is required for the property.
        /// </summary>
        /// <param name="reference">The Edm type reference.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteNullableAttributeAsync(IEdmTypeReference reference)
        {
            // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
            this.jsonWriter.WriteOptionalProperty("$Nullable", reference.IsNullable, defaultValue: false);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes type definition attributes.
        /// </summary>
        /// <param name="reference">The Edm type definition reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteTypeDefinitionAttributesAsync(IEdmTypeDefinitionReference reference)
        {
            IEdmTypeReference actualTypeReference = reference.AsActualTypeReference();

            if (actualTypeReference.IsBinary())
            {
                await this.WriteBinaryTypeAttributesAsync(actualTypeReference.AsBinary()).ConfigureAwait(false);
            }
            else if (actualTypeReference.IsString())
            {
                await this.WriteStringTypeAttributesAsync(actualTypeReference.AsString()).ConfigureAwait(false);
            }
            else if (actualTypeReference.IsTemporal())
            {
                await this.WriteTemporalTypeAttributesAsync(actualTypeReference.AsTemporal()).ConfigureAwait(false);
            }
            else if (actualTypeReference.IsDecimal())
            {
                await this.WriteDecimalTypeAttributesAsync(actualTypeReference.AsDecimal()).ConfigureAwait(false);
            }
            else if (actualTypeReference.IsSpatial())
            {
                await this.WriteSpatialTypeAttributesAsync(actualTypeReference.AsSpatial()).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes binary type attributes.
        /// </summary>
        /// <param name="reference">The Edm binary type reference</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteBinaryTypeAttributesAsync(IEdmBinaryTypeReference reference)
        {
            // CSDL XML defines a symbolic value max that is only allowed in OData 4.0 responses.
            // This symbolic value is not allowed in CDSL JSON documents at all.
            // So, 'IsUnbounded' is skipped in CSDL JSON.
            this.jsonWriter.WriteOptionalProperty("$MaxLength", reference.MaxLength);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes decimal type attributes.
        /// </summary>
        /// <param name="reference">The Edm decimal type reference.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteDecimalTypeAttributesAsync(IEdmDecimalTypeReference reference)
        {
            // The value of $Precision is a number. Absence of $Precision means arbitrary precision.
            this.jsonWriter.WriteOptionalProperty("$Precision", reference.Precision);

            // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
            // TODO: the symbolic values floating or variable is not supported now.
            // Absence of $Scale means variable.
            this.jsonWriter.WriteOptionalProperty("$Scale", reference.Scale);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes spatial type attributes. $SRID
        /// </summary>
        /// <param name="reference">The Edm spatial type reference.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteSpatialTypeAttributesAsync(IEdmSpatialTypeReference reference)
        {
            // The value of $SRID is a string containing a number or the symbolic value variable
            // The value of the SRID facet MUST be a non-negative integer or the special value variable.
            // TODO: the special value variable is not supported now.
            // If no value is specified, the facet defaults to 0 for Geometry types or 4326 for Geography types.
            if (reference.IsGeography())
            {
                this.jsonWriter.WriteOptionalProperty("$SRID", reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeographySrid);
            }
            else if (reference.IsGeometry())
            {
                this.jsonWriter.WriteOptionalProperty("$SRID", reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeometrySrid);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes string type attributes. $MaxLength, $Unicode
        /// </summary>
        /// <param name="reference">The Edm string type reference.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteStringTypeAttributesAsync(IEdmStringTypeReference reference)
        {
            // CSDL XML defines a symbolic value max that is only allowed in OData 4.0 responses.
            // This symbolic value is not allowed in CDSL JSON documents at all.
            this.jsonWriter.WriteOptionalProperty("$MaxLength", reference.MaxLength);

            // The value of $Unicode is one of the Boolean literals true or false.Absence of the member means true.
            this.jsonWriter.WriteOptionalProperty("$Unicode", reference.IsUnicode, CsdlConstants.Default_IsUnicode);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes temporal type attributes. $Precision
        /// </summary>
        /// <param name="reference">The Edm temporal type reference.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteTemporalTypeAttributesAsync(IEdmTemporalTypeReference reference)
        {
            // The value of $Precision is a number. Absence of $Precision means arbitrary precision.
            this.jsonWriter.WriteOptionalProperty("$Precision", reference.Precision);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes annotation string attribute.
        /// </summary>
        /// <param name="annotation">The Edm direct value annotation.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteAnnotationStringAttributeAsync(IEdmDirectValueAnnotation annotation)
        {
            if ((IEdmPrimitiveValue)annotation.Value != null)
            {
                this.jsonWriter.WriteRequiredProperty(annotation.Name, EdmValueWriter.PrimitiveValueAsXml((IEdmPrimitiveValue)annotation.Value));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes annotation string element.
        /// </summary>
        /// <param name="annotation">The Edm direct value annotation.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteAnnotationStringElementAsync(IEdmDirectValueAnnotation annotation)
        {
            IEdmPrimitiveValue edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.jsonWriter.WriteStringValue(((IEdmStringValue)edmValue).Value);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes schema operations header.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation">The Key/Value pair operation. An operation is represented as a member of the schema object whose name is the unqualified name of the operation.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteSchemaOperationsHeaderAsync<T>(KeyValuePair<string, IList<T>> operation)
        {
            // An operation is represented as a member of the schema object whose name is the unqualified name of the operation.
            this.jsonWriter.WritePropertyName(operation.Key);

            // Whose value is an array
            this.jsonWriter.WriteStartArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes schema operations end.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation">The Key/Value pair operation. An operation is represented as a member of the schema object whose name is the unqualified name of the operation.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteSchemaOperationsEndAsync<T>(KeyValuePair<string, IList<T>> operation)
        {
            this.jsonWriter.WriteEndArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes action element header. $Kind
        /// </summary>
        /// <param name="action">The Edm action.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteActionElementHeaderAsync(IEdmAction action)
        {
            this.jsonWriter.WriteStartObject();

            // The action overload object MUST contain the member $Kind with a string value of Action.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_Action);

            await this.WriteOperationElementAttributesAsync(action).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes function element header. $Kind, $IsComposable
        /// </summary>
        /// <param name="function">The Edm function.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFunctionElementHeaderAsync(IEdmFunction function)
        {
            this.jsonWriter.WriteStartObject();

            // The action overload object MUST contain the member $Kind with a string value of Action.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_Function);

            await this.WriteOperationElementAttributesAsync(function).ConfigureAwait(false);

            if (function.IsComposable)
            {
                this.jsonWriter.WriteRequiredProperty("$IsComposable", true);
            }
        }

        /// <summary>
        /// Asynchronously writes operation element attributes. $IsBound, $EntitySetPath
        /// </summary>
        /// <param name="operation">The Edm operation.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteOperationElementAttributesAsync(IEdmOperation operation)
        {
            if (operation.IsBound)
            {
                this.jsonWriter.WriteRequiredProperty("$IsBound", true);
            }

            if (operation.EntitySetPath != null)
            {
                this.jsonWriter.WriteRequiredProperty("$EntitySetPath", operation.EntitySetPath.Path);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes operation parameters begin. $Parameter
        /// </summary>
        /// <param name="parameters">The Edm operation parameters.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteOperationParametersBeginAsync(IEnumerable<IEdmOperationParameter> parameters)
        {
            if (parameters != null && parameters.Any())
            {
                this.jsonWriter.WritePropertyName("$Parameter");
                this.jsonWriter.WriteStartArray();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes operation parameters end.
        /// </summary>
        /// <param name="parameters">The Edm operation parameters.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteOperationParametersEndAsync(IEnumerable<IEdmOperationParameter> parameters)
        {
            if (parameters != null && parameters.Any())
            {
                this.jsonWriter.WriteEndArray();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes return type element header. $ReturnType
        /// </summary>
        /// <param name="operationReturn">The Edm operation return.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteReturnTypeElementHeaderAsync(IEdmOperationReturn operationReturn)
        {
            // $ReturnType
            this.jsonWriter.WritePropertyName("$ReturnType");

            // The value of $ReturnType is an object.
            this.jsonWriter.WriteStartObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes type attribute
        /// </summary>
        /// <param name="typeReference">The Edm type reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override Task WriteTypeAttributeAsync(IEdmTypeReference typeReference)
        {
            return WriteTypeReferenceAsync(typeReference);
        }

        /// <summary>
        /// Asynchronously writes entity container element header.
        /// </summary>
        /// <param name="container">The Edm entity container.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteEntityContainerElementHeaderAsync(IEdmEntityContainer container)
        {
            // An entity container is represented as a member of the schema object whose name is the unqualified name of the entity container
            this.jsonWriter.WritePropertyName(container.Name);

            // whose value is an object
            this.jsonWriter.WriteStartObject();

            // The entity container object MUST contain the member $Kind with a string value of EntityContainer.
            // Be caution: Example 33 has the $Kind, but no other words.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_EntityContainer);

            // The entity container object MAY contain the member $Extends, so far it only supports in Csdl Semantics.
            CsdlSemanticsEntityContainer tmp = container as CsdlSemanticsEntityContainer;
            CsdlEntityContainer csdlContainer;
            if (tmp != null && (csdlContainer = tmp.Element as CsdlEntityContainer) != null)
            {
                this.jsonWriter.WriteOptionalProperty("$Extends", csdlContainer.Extends);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes entity set object.
        /// </summary>
        /// <param name="entitySet">The Edm entity set.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteEntitySetElementHeaderAsync(IEdmEntitySet entitySet)
        {
            // An entity set is represented as a member of the entity container object whose name is the name of the entity set
            this.jsonWriter.WritePropertyName(entitySet.Name);

            // whose value is an object
            this.jsonWriter.WriteStartObject();

            // The entity set object MUST contain the members $Collection, it's value as true
            this.jsonWriter.WriteRequiredProperty("$Collection", true);

            // The entity set object MUST contain the member $Type whose string value is the qualified name of an entity type.
            this.jsonWriter.WriteRequiredProperty("$Type", entitySet.EntityType().FullName());

            // It MAY contain the members $IncludeInServiceDocument. Absence of the member means true.
            this.jsonWriter.WriteOptionalProperty("$IncludeInServiceDocument", entitySet.IncludeInServiceDocument, true);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes singleton object.
        /// </summary>
        /// <param name="singleton">The Edm singleton.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteSingletonElementHeaderAsync(IEdmSingleton singleton)
        {
            // A singleton is represented as a member of the entity container object whose name is the name of the singleton
            this.jsonWriter.WritePropertyName(singleton.Name);

            // whose value is an object
            this.jsonWriter.WriteStartObject();

            // The singleton object MUST contain the member $Type whose string value is the qualified name of an entity type.
            this.jsonWriter.WriteRequiredProperty("$Type", singleton.EntityType().FullName());

            // The singleton object MAY contain the member $Nullable. In OData 4.0 responses this member MUST NOT be specified.
            // So far, IEdmSingleton doesn't have the property defined, so skip it now.

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes action import object.
        /// </summary>
        /// <param name="actionImport">The Edm action import.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteActionImportElementHeaderAsync(IEdmActionImport actionImport)
        {
            // An action import is represented as a member of the entity container object whose name is the name of the action import.
            this.jsonWriter.WritePropertyName(actionImport.Name);

            // whose value is an object
            this.jsonWriter.WriteStartObject();

            // The action import object MUST contain the member $Kind with a string value of ActionImport, and the member $Action.
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_ActionImport);

            // The action import object MUST contain the member $Action. The value of $Action is a string containing the qualified name of an unbound action.
            this.jsonWriter.WriteRequiredProperty("$Action", actionImport.Operation.FullName());

            // The action import object MAY contain the member $EntitySet.
            await this.WriteOperationImportAttributesAsync(actionImport, CsdlConstants.Attribute_Action).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes function import object.
        /// </summary>
        /// <param name="functionImport">The Edm function import.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteFunctionImportElementHeaderAsync(IEdmFunctionImport functionImport)
        {
            // A function import is represented as a member of the entity container object whose name is the name of the function import
            this.jsonWriter.WritePropertyName(functionImport.Name);

            // whose value is an object.
            this.jsonWriter.WriteStartObject();

            // The function import object MUST contain the member $Kind with a string value of FunctionImport
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_FunctionImport);

            // The function import object MUST contain the member $Function.
            this.jsonWriter.WriteRequiredProperty("$Function", functionImport.Operation.FullName());

            // The function import object MAY contain the member $EntitySet.
            await this.WriteOperationImportAttributesAsync(functionImport, CsdlConstants.Attribute_Function).ConfigureAwait(false);

            // The value of $IncludeInServiceDocument is one of the Boolean literals true or false. Absence of the member means false.
            if (functionImport.IncludeInServiceDocument)
            {
                this.jsonWriter.WriteRequiredProperty("$IncludeInServiceDocument", functionImport.IncludeInServiceDocument);
            }
        }

        /// <summary>
        /// Asynchronously writes operation parameter object.
        /// </summary>
        /// <param name="parameter">The Edm operation parameter.</param>
        /// <param name="inlineType">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteOperationParameterElementHeaderAsync(IEdmOperationParameter parameter, bool inlineType)
        {
            this.jsonWriter.WriteStartObject();

            // A parameter object MUST contain the member $Name
            this.jsonWriter.WriteRequiredProperty("$Name", parameter.Name);

            if (inlineType)
            {
                await WriteTypeReferenceAsync(parameter.Type).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes type reference.
        /// </summary>
        /// <param name="type">The Edm type reference.</param>
        /// <param name="defaultTypeName">The Default type name. Set to "Edm.String" by default.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal Task WriteTypeReferenceAsync(IEdmTypeReference type, string defaultTypeName = "Edm.String")
        {
            IEdmTypeReference elementType = type;
            if (type.IsCollection())
            {
                this.jsonWriter.WriteRequiredProperty("$Collection", true);

                IEdmCollectionTypeReference collectionReference = type.AsCollection();
                elementType = collectionReference.ElementType();
            }

            // Absence of the $Type member means the type is Edm.String.
            // Does it mean to omit the type for Collection(Edm.String)? No, $Collection is used to identify whether it's collection of not.
            if (elementType.FullName() != defaultTypeName)
            {
                string typeName = this.SerializationName((IEdmSchemaElement)elementType.Definition);
                this.jsonWriter.WriteRequiredProperty("$Type", typeName);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes operation parameter end element.
        /// </summary>
        /// <param name="parameter">The Edm operation parameter.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteOperationParameterEndElementAsync(IEdmOperationParameter parameter)
        {
            IEdmOptionalParameter optionalParameter = parameter as IEdmOptionalParameter;
            if (optionalParameter != null && !(optionalParameter.VocabularyAnnotations(this.Model).Any(a => a.Term == CoreVocabularyModel.OptionalParameterTerm)))
            {
                string defaultValue = optionalParameter.DefaultValueString;
                EdmRecordExpression optionalValue = new EdmRecordExpression();

                await this.WriteVocabularyAnnotationElementHeaderAsync(new EdmVocabularyAnnotation(parameter, CoreVocabularyModel.OptionalParameterTerm, optionalValue), false).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    EdmPropertyConstructor property = new EdmPropertyConstructor(CsdlConstants.Attribute_DefaultValue, new EdmStringConstant(defaultValue));
                    await this.WriteRecordExpressionElementHeaderAsync(optionalValue).ConfigureAwait(false);
                    await this.WritePropertyValueElementHeaderAsync(property, true).ConfigureAwait(false);
                    await this.WriteEndElementAsync().ConfigureAwait(false);
                }
                else
                {
                    this.jsonWriter.WriteStartObject();
                    this.jsonWriter.WriteEndObject();
                }
            }

            await this.WriteEndElementAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes collection type element header.
        /// </summary>
        /// <param name="collectionType">The Edm collection type.</param>
        /// <param name="inlineType">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteCollectionTypeElementHeaderAsync(IEdmCollectionType collectionType, bool inlineType)
        {
            return Task.CompletedTask;
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
                    return WriteBinaryConstantExpressionElementAsync((IEdmBinaryConstantExpression)expression);
                case EdmExpressionKind.BooleanConstant:
                    return WriteBooleanConstantExpressionElementAsync((IEdmBooleanConstantExpression)expression);
                case EdmExpressionKind.DateTimeOffsetConstant:
                    return WriteDateTimeOffsetConstantExpressionElementAsync((IEdmDateTimeOffsetConstantExpression)expression);
                case EdmExpressionKind.DecimalConstant:
                    return WriteDecimalConstantExpressionElementAsync((IEdmDecimalConstantExpression)expression);
                case EdmExpressionKind.FloatingConstant:
                    return WriteFloatingConstantExpressionElementAsync((IEdmFloatingConstantExpression)expression);
                case EdmExpressionKind.GuidConstant:
                    return WriteGuidConstantExpressionElementAsync((IEdmGuidConstantExpression)expression);
                case EdmExpressionKind.IntegerConstant:
                    return WriteIntegerConstantExpressionElementAsync((IEdmIntegerConstantExpression)expression);
                case EdmExpressionKind.Path:
                    return WritePathExpressionElementAsync(pathExpression);
                case EdmExpressionKind.PropertyPath:
                    return WritePropertyPathExpressionElementAsync(pathExpression);
                case EdmExpressionKind.NavigationPropertyPath:
                    return WriteNavigationPropertyPathExpressionElementAsync(pathExpression);
                case EdmExpressionKind.AnnotationPath:
                    return WriteAnnotationPathExpressionElementAsync(pathExpression);
                case EdmExpressionKind.StringConstant:
                    return WriteStringConstantExpressionElementAsync((IEdmStringConstantExpression)expression);
                case EdmExpressionKind.DurationConstant:
                    return WriteDurationConstantExpressionElementAsync((IEdmDurationConstantExpression)expression);
                case EdmExpressionKind.DateConstant:
                    return WriteDateConstantExpressionElementAsync((IEdmDateConstantExpression)expression);
                case EdmExpressionKind.TimeOfDayConstant:
                    return WriteTimeOfDayConstantExpressionElementAsync((IEdmTimeOfDayConstantExpression)expression);
                default:
                    Debug.Assert(false, "Attempted to inline an expression that was not one of the expected inlineable types.");
                    return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Asynchronously writes vocabulary annotation header.
        /// </summary>
        /// <param name="annotation">The Edm vocabulary annotation.</param>
        /// <param name="isInline">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WriteVocabularyAnnotationElementHeaderAsync(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            this.jsonWriter.WritePropertyName(AnnotationToString(annotation));

            if (isInline)
            {
                // In JSON, we always write the annotation value.
                await this.WriteInlineExpressionAsync(annotation.Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes vocabulary annotation end.
        /// </summary>
        /// <param name="annotation">The Edm vocabulary annotation.</param>
        /// <param name="isInline">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteVocabularyAnnotationElementEndAsync(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes property value header.
        /// </summary>
        /// <param name="value">The Edm property constructor.</param>
        /// <param name="isInline">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePropertyValueElementHeaderAsync(IEdmPropertyConstructor value, bool isInline)
        {
            this.jsonWriter.WritePropertyName(value.Name);

            if (isInline)
            {
                await this.WriteInlineExpressionAsync(value.Value).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes record expression header.
        /// </summary>
        /// <param name="expression">The Edm record expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteRecordExpressionElementHeaderAsync(IEdmRecordExpression expression)
        {
            // Record expressions are represented as objects with one member per property value expression.
            this.jsonWriter.WriteStartObject();

            if (expression.DeclaredType != null)
            {
                // The type of a record expression is represented as the @type control information.
                this.jsonWriter.WriteRequiredProperty("@type", expression.DeclaredType.FullName());
            }
            // It MAY contain annotations for itself. It's not supported now.

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes property constructor header.
        /// </summary>
        /// <param name="constructor">The Edm property constructor.</param>
        /// <param name="isInline">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal override async Task WritePropertyConstructorElementHeaderAsync(IEdmPropertyConstructor constructor, bool isInline)
        {
            // The member name is the property name, and the member value is the property value expression.
            this.jsonWriter.WritePropertyName(constructor.Name);

            if (isInline)
            {
                await this.WriteInlineExpressionAsync(constructor.Value).ConfigureAwait(false);
            }

            // Annotations for record members are prefixed with the member name. It's not supported now.
        }

        /// <summary>
        /// Asynchronously writes property constructor end.
        /// </summary>
        /// <param name="constructor">The Edm property constructor.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WritePropertyConstructorElementEndAsync(IEdmPropertyConstructor constructor)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes string constant expression.
        /// </summary>
        /// <param name="expression">The Edm string constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteStringConstantExpressionElementAsync(IEdmStringConstantExpression expression)
        {
            // String expressions are represented as a JSON string.
            this.jsonWriter.WriteStringValue(expression.Value);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes binary constant expression.
        /// </summary>
        /// <param name="expression">The Edm string constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteBinaryConstantExpressionElementAsync(IEdmBinaryConstantExpression expression)
        {
            // Binary expressions are represented as a string containing the base64url-encoded binary value.
            this.jsonWriter.WriteStringValue(BinaryToString(expression));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes boolean constant expression.
        /// </summary>
        /// <param name="expression">The Edm boolean constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteBooleanConstantExpressionElementAsync(IEdmBooleanConstantExpression expression)
        {
            // Boolean expressions are represented as the literals true or false.
            this.jsonWriter.WriteBooleanValue(expression.Value);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes null constant expression.
        /// </summary>
        /// <param name="expression">The Edm null constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteNullConstantExpressionElementAsync(IEdmNullExpression expression)
        {
            // Null expressions that do not contain annotations are represented as the literal null.
            this.jsonWriter.WriteNullValue();

            // Null expression containing annotations are represented as an object with a member $Null whose value is the literal null.
            // So far, it's not supported.

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes Date constant expression.
        /// </summary>
        /// <param name="expression">The Edm Date constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteDateConstantExpressionElementAsync(IEdmDateConstantExpression expression)
        {
            // Date expressions are represented as a string containing the date value.
            this.jsonWriter.WriteStringValue(expression.Value.ToString());

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes DateTimeOffset constant expression.
        /// </summary>
        /// <param name="expression">The Edm datetimeoffset constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteDateTimeOffsetConstantExpressionElementAsync(IEdmDateTimeOffsetConstantExpression expression)
        {
            // Datetimestamp expressions are represented as a string containing the timestamp value.
            this.jsonWriter.WriteStringValue(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes duration constant expression.
        /// </summary>
        /// <param name="expression">The Edm duration constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteDurationConstantExpressionElementAsync(IEdmDurationConstantExpression expression)
        {
            // Duration expressions are represented as a string containing the duration value.
            this.jsonWriter.WriteStringValue(EdmValueWriter.DurationAsXml(expression.Value));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes decimal constant expression.
        /// </summary>
        /// <param name="expression">The Edm decimal constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteDecimalConstantExpressionElementAsync(IEdmDecimalConstantExpression expression)
        {
            // Decimal expressions are represented as either a number or a string.
            // The special values INF, -INF, or NaN are represented as strings. so far, that's not supported.
            if (this.settings.IsIeee754Compatible)
            {
                this.jsonWriter.WriteStringValue(expression.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.jsonWriter.WriteNumberValue(expression.Value);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes floating constant expression.
        /// </summary>
        /// <param name="expression">The Edm floating constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteFloatingConstantExpressionElementAsync(IEdmFloatingConstantExpression expression)
        {
            // Ut8JsonWriter can't write the Infinity double, 
            // it throws ".NET number values such as positive and negative infinity cannot be written as valid JSON."
            if (double.IsNegativeInfinity(expression.Value))
            {
                this.jsonWriter.WriteStringValue("-INF");
            }
            else if (double.IsPositiveInfinity(expression.Value))
            {
                this.jsonWriter.WriteStringValue("INF");
            }
            else if (double.IsNaN(expression.Value))
            {
                this.jsonWriter.WriteStringValue("NaN");
            }
            else
            {
                this.jsonWriter.WriteNumberValue(expression.Value);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes function application header.
        /// </summary>
        /// <param name="expression">The Edm Apply expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteFunctionApplicationElementHeaderAsync(IEdmApplyExpression expression)
        {
            // Apply expressions are represented as an object.
            this.jsonWriter.WriteStartObject();

            // a member $Apply
            this.jsonWriter.WritePropertyName("$Apply");

            // whose value is an array of annotation expressions.
            this.jsonWriter.WriteStartArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes function application end.
        /// </summary>
        /// <param name="expression">The Edm Apply expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteFunctionApplicationElementEndAsync(IEdmApplyExpression expression)
        {
            // End of $Apply
            this.jsonWriter.WriteEndArray();

            // a member $Function whose value is a string containing the qualified name of the client-side function to be applied
            this.jsonWriter.WriteRequiredProperty("$Function", expression.AppliedFunction.FullName());

            // End of Annotation Value.
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes Guid constant expression.
        /// </summary>
        /// <param name="expression">The Edm Guid constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteGuidConstantExpressionElementAsync(IEdmGuidConstantExpression expression)
        {
            // Guid expressions are represented as a string containing the guid value.
            this.jsonWriter.WriteStringValue(EdmValueWriter.GuidAsXml(expression.Value));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes Integer constant expression.
        /// </summary>
        /// <param name="expression">The Edm Integer constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteIntegerConstantExpressionElementAsync(IEdmIntegerConstantExpression expression)
        {
            // Integer expressions are represented as a numbers or strings depending on the media type parameter IEEE754Compatible.
            if (this.settings.IsIeee754Compatible)
            {
                this.jsonWriter.WriteStringValue(expression.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.jsonWriter.WriteNumberValue(expression.Value);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes path expression.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WritePathExpressionElementAsync(IEdmPathExpression expression)
        {
            // We consider base Path expression is a value path, not same as NavigationPropertyPath, we don't have a value path.
            // Path expressions are represented as an object with a single member $Path whose value is a string containing a path.
            this.jsonWriter.WriteStartObject();
            this.jsonWriter.WriteRequiredProperty("$Path", PathAsXml(expression.PathSegments));
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes property path expression.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WritePropertyPathExpressionElementAsync(IEdmPathExpression expression)
        {
            // Navigation property path expressions are represented as a string containing a path.
            this.jsonWriter.WriteStringValue(PathAsXml(expression.PathSegments));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes navigation property path expression.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteNavigationPropertyPathExpressionElementAsync(IEdmPathExpression expression)
        {
            // Property path expressions are represented as a string containing a path.
            this.jsonWriter.WriteStringValue(PathAsXml(expression.PathSegments));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes annotation path expression.
        /// </summary>
        /// <param name="expression">The Edm path expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteAnnotationPathExpressionElementAsync(IEdmPathExpression expression)
        {
            // Annotation path expressions are represented as a string containing a path.
            this.jsonWriter.WriteStringValue(PathAsXml(expression.PathSegments));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes 'if' expression header.
        /// </summary>
        /// <param name="expression">The if expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteIfExpressionElementHeaderAsync(IEdmIfExpression expression)
        {
            // Is-of expressions are represented as an object
            this.jsonWriter.WriteStartObject();

            // Conditional expressions are represented as an object with a member $If
            this.jsonWriter.WritePropertyName("$If");

            // whose value is an array of two or three annotation expressions
            this.jsonWriter.WriteStartArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes 'if' expression end.
        /// </summary>
        /// <param name="expression">The Edm if expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteIfExpressionElementEndAsync(IEdmIfExpression expression)
        {
            this.jsonWriter.WriteEndArray();

            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes collection expression header.
        /// </summary>
        /// <param name="expression">The Edm collection expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteCollectionExpressionElementHeaderAsync(IEdmCollectionExpression expression)
        {
            this.jsonWriter.WriteStartArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes collection expression end.
        /// </summary>
        /// <param name="expression">The Edm collection expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteCollectionExpressionElementEndAsync(IEdmCollectionExpression expression)
        {
            this.jsonWriter.WriteEndArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes labeled header.
        /// </summary>
        /// <param name="labeledElement">The Edm labeled expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteLabeledElementHeaderAsync(IEdmLabeledExpression labeledElement)
        {
            // Labeled element expressions are represented as an object with a member $LabeledElement whose value is an annotation expression.
            this.jsonWriter.WriteStartObject();

            // a member $Name whose value is a string containing the labeled element’s name
            this.jsonWriter.WriteRequiredProperty("$Name", labeledElement.Name);

            // an object with a member $LabeledElement
            this.jsonWriter.WritePropertyName("$LabeledElement");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes labeled expression reference.
        /// </summary>
        /// <param name="labeledExpressionReference">The Edm labeled expression reference.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression labeledExpressionReference)
        {
            // Labeled element reference expressions are represented as an object
            this.jsonWriter.WriteStartObject();

            // with a member $LabeledElementReference
            this.jsonWriter.WritePropertyName("$LabeledElementReference");

            // Whose value is a string containing an qualified name.
            // Here's the problem that we don't have the namespace for the labeled expression,
            // Even though we can get the namespace from the up-level schema, we can't query it here.
            // So, leave it using the name.
            this.jsonWriter.WriteStringValue(labeledExpressionReference.ReferencedLabeledExpression.Name);

            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes time of day constant expression.
        /// </summary>
        /// <param name="expression">The Edm TimeOfDay constant expression.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteTimeOfDayConstantExpressionElementAsync(IEdmTimeOfDayConstantExpression expression)
        {
            // Time-of-day expressions are represented as a string containing the time-of-day value.
            this.jsonWriter.WriteStringValue(EdmValueWriter.TimeOfDayAsXml(expression.Value));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes IsOf type expression header.
        /// </summary>
        /// <param name="expression">The Edm IsOf type expression.</param>
        /// <param name="inlineType">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteIsTypeExpressionElementHeaderAsync(IEdmIsTypeExpression expression, bool inlineType)
        {
            // Is-of expressions are represented as an object
            this.jsonWriter.WriteStartObject();

            // a member $IsOf whose value is an annotation expression
            // fix it using $IsOf, not using $IsType
            this.jsonWriter.WritePropertyName("$IsOf");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes 'IsOf' expression type.
        /// </summary>
        /// <param name="expression">The Edm IsOf type expression.</param>
        /// <param name="inlineType">Is line type or not.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteIsOfExpressionTypeAsync(IEdmIsTypeExpression expression, bool inlineType)
        {
            if (inlineType)
            {
                return WriteTypeReferenceAsync(expression.Type, "None");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes cast expression header.
        /// </summary>
        /// <param name="expression">The cast expression</param>
        /// <param name="inlineType">Is inline type.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteCastExpressionElementHeaderAsync(IEdmCastExpression expression, bool inlineType)
        {
            // Cast expressions are represented as an object with a member $Cast whose value is an annotation expression, 
            this.jsonWriter.WriteStartObject();

            this.jsonWriter.WritePropertyName("$Cast");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes cast expression end.
        /// </summary>
        /// <param name="expression">The Edm cast expression</param>
        /// <param name="inlineType">Is inline type.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteCastExpressionElementEndAsync(IEdmCastExpression expression, bool inlineType)
        {
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes cast expression type.
        /// </summary>
        /// <param name="expression">The Edm cast expression</param>
        /// <param name="inlineType">Is inline type.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteCastExpressionTypeAsync(IEdmCastExpression expression, bool inlineType)
        {
            if (inlineType)
            {
                return WriteTypeReferenceAsync(expression.Type, "None");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously enumeration member expression
        /// </summary>
        /// <param name="expression">The Edm enumeration member expression.</param>
        internal override Task WriteEnumMemberExpressionElementAsync(IEdmEnumMemberExpression expression)
        {
            // Enumeration member expressions are represented as a string containing the numeric or symbolic enumeration value.
            this.jsonWriter.WriteStringValue(EnumMemberExpressionAsJson(expression.EnumMembers));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes type definition object
        /// </summary>
        /// <param name="typeDefinition">The Edm type definition.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteTypeDefinitionElementHeaderAsync(IEdmTypeDefinition typeDefinition)
        {
            // A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition
            this.jsonWriter.WritePropertyName(typeDefinition.Name);

            // whose value is an object.
            this.jsonWriter.WriteStartObject();

            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition
            this.jsonWriter.WriteRequiredProperty("$Kind", CsdlConstants.Element_TypeDefinition);

            // The type definition object MUST contain he member $UnderlyingType.
            this.jsonWriter.WriteRequiredProperty("$UnderlyingType", typeDefinition.UnderlyingType, TypeDefinitionAsJson);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes navigation property bindings begin
        /// </summary>
        /// <param name="bindings">The collection the bindings.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteNavigationPropertyBindingsBeginAsync(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            // It MAY contain the member $NavigationPropertyBinding
            this.jsonWriter.WritePropertyName("$NavigationPropertyBinding");

            // whose value is an object.
            this.jsonWriter.WriteStartObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes navigation property bindings end
        /// </summary>
        /// <param name="bindings">The collection the bindings.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteNavigationPropertyBindingsEndAsync(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes the navigation property binding member in navigation property binding object.
        /// </summary>
        /// <param name="binding">The Edm navigation property binding.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteNavigationPropertyBindingAsync(IEdmNavigationPropertyBinding binding)
        {
            // whose name is the navigation property binding path.
            this.jsonWriter.WritePropertyName(binding.Path.Path);

            // whose value is a string containing the navigation property binding target.
            if (binding.Target is IEdmContainedEntitySet containedEntitySet)
            {
                this.jsonWriter.WriteStringValue(containedEntitySet.Path.Path);
            }
            else
            {
                this.jsonWriter.WriteStringValue(binding.Target.Name);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes end element.
        /// </summary>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteEndElementAsync()
        {
            this.jsonWriter.WriteEndObject();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes array end element.
        /// </summary>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteArrayEndElementAsync()
        {
            this.jsonWriter.WriteEndArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously writes operation import attributes.
        /// </summary>
        /// <param name="operationImport">The Edm operation import.</param>
        /// <param name="operationAttributeName">Operation attribute name.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        internal override Task WriteOperationImportAttributesAsync(IEdmOperationImport operationImport, string operationAttributeName)
        {
            if (operationImport.EntitySet != null)
            {
                if (operationImport.EntitySet is IEdmPathExpression pathExpression)
                {
                    this.jsonWriter.WriteRequiredProperty("$EntitySet", pathExpression.PathSegments, PathAsXml);
                }
                else
                {
                    throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(operationImport.Name));
                }
            }

            return Task.CompletedTask;
        }
    }
}
#endif