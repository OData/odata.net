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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.Serialization
{
    internal class EdmModelCsdlSchemaWriter
    {
        protected XmlWriter xmlWriter;
        protected Version version;

        internal EdmModelCsdlSchemaWriter(XmlWriter xmlWriter, Version edmVersion)
        {
            this.xmlWriter = xmlWriter;
            this.version = edmVersion;
        }

        internal void WriteAssociationElementHeader(IEdmAssociation associationType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Association);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, associationType.Name, StringAsXml);
        }

        internal void WriteAssociationSetElementHeader(IEdmAssociationSet associationSet)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_AssociationSet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, associationSet.Name, StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Association, associationSet.Association.FullName(), StringAsXml);
        }

        internal void WriteComplexTypeElementHeader(IEdmComplexType complexType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ComplexType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, complexType.Name, StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, BooleanAsXml);
        }

        internal void WriteEnumTypeElementHeader(IEdmEnumType enumType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, enumType.Name, StringAsXml);
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, enumType.UnderlyingType, this.TypeDefinitionAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsFlags, enumType.TreatAsBits, CsdlConstants.Default_IsFlags, BooleanAsXml);
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

            if (documentation.LongDescription != null)
            {
                this.xmlWriter.WriteStartElement(CsdlConstants.Element_LongDescription);
                this.xmlWriter.WriteString(documentation.LongDescription);
                this.WriteEndElement();
            }

            this.WriteEndElement();
        }

        internal void WriteAssociationSetEndElementHeader(IEdmAssociationSetEnd end)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_End);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, end.Role.Name, StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntitySet, end.EntitySet.Name, StringAsXml);
        }

        internal void WriteAssociationEndElementHeader(IEdmAssociationEnd associationEnd)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_End);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, associationEnd.EntityType.FullName(), StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, associationEnd.Name, StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Multiplicity, associationEnd.Multiplicity, MultiplicityAsXml);
        }

        internal void WriteEntityContainerElementHeader(IEdmEntityContainer container)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityContainer);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, container.Name, StringAsXml);
        }

        internal void WriteEntitySetElementHeader(IEdmEntitySet entitySet)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntitySet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entitySet.Name, StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntityType, entitySet.ElementType.FullName(), StringAsXml);
        }

        internal void WriteEntityTypeElementHeader(IEdmEntityType entityType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entityType.Name, StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, entityType.BaseEntityType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, entityType.IsAbstract, CsdlConstants.Default_Abstract, BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, entityType.IsOpen, CsdlConstants.Default_OpenType, BooleanAsXml);
        }

        internal void WriteDelaredKeyPropertiesElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Key);
        }

        internal void WritePropertyRefElement(IEdmStructuralProperty property)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyRef);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteNavigationPropertyElementHeader(IEdmNavigationProperty member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationProperty);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Relationship, member.Association().FullName(), StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_ToRole, member.To.Name, StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_FromRole, member.From().Name, StringAsXml);
        }

        internal void WriteOperationActionElement(string elementName, EdmOnDeleteAction operationAction)
        {
            this.xmlWriter.WriteStartElement(elementName);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Action, operationAction.ToString(), StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteSchemaElementHeader(string schemaNamespace, XmlNamespaceManager manager)
        {
            string xmlNamespace = GetCsdlNamespace(this.version);
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Schema, xmlNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Namespace, schemaNamespace, StringAsXml);
            if (manager != null)
            {
                //ToDo: find a more flexible way to exclude autogenerated namespaces from XmlNamespaceManager
                string[] autoGeneratedNamespaces = { string.Empty, EdmConstants.XmlPrefix, EdmConstants.XmlNamespacePrefix };
                foreach (string prefix in manager)
                {
                    if (!autoGeneratedNamespaces.Contains(prefix))
                    {
                        this.xmlWriter.WriteAttributeString(EdmConstants.XmlNamespacePrefix, prefix, null, manager.LookupNamespace(prefix));
                    }
                }
            }
        }

        internal void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Property);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_ConcurrencyMode, property.ConcurrencyMode, CsdlConstants.Default_ConcurrencyMode, ConcurrencyModeAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, property.DefaultValue, StringAsXml);
        }

        internal void WriteEnumMemberElementHeader(IEdmEnumMember member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Member);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, StringAsXml);
            // TODO: write value when IEdmIntegerValue comes in.
            //WriteRequiredAttribute(CsdlConstants.Attribute_Value, StringAsXml);
        }

        internal void WriteNullableAttribute(IEdmTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, BooleanAsXml);
        }

        internal void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference)
        {
            if (reference.IsMaxMaxLength)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, StringAsXml);
            }
            else 
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_MaxLength, reference.MaxLength, IntAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_FixedLength, reference.IsFixedLength, BooleanAsXml);
        }

        internal void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, IntAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Scale, reference.Scale, IntAsXml);
        }

        internal void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, SridAsXml);
        }

        internal void WriteStringTypeAttributes(IEdmStringTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Collation, reference.Collation, StringAsXml);
            if (reference.IsMaxMaxLength)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, StringAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_MaxLength, reference.MaxLength, IntAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_FixedLength, reference.IsFixedLength, BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Unicode, reference.IsUnicode, BooleanAsXml);
        }

        internal void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
        {
            WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, IntAsXml);
        }

        internal void WriteReferentialConstraintElementHeader(IEdmReferentialConstraint constraint)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReferentialConstraint);
        }

        internal void WriteReferentialConstraintPrincipleEndElementHeader(IEdmAssociationEnd end)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Principal);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, end.Name, StringAsXml);
        }

        internal void WriteReferentialConstraintDependentEndElementHeader(IEdmAssociationEnd end)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Dependent);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Role, end.Name, StringAsXml);
        }

        internal void WriteUsingElement(string usingNamespace)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Using);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Namespace, usingNamespace, StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Alias, usingNamespace, CreateUsingAlias);
            this.WriteEndElement();
        }

        internal void WriteAnnotationStringAttribute(IEdmImmediateValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteAttributeString(annotation.LocalName(), annotation.Namespace(), PrimitiveValueAsXml(edmValue));
            }
        }

        internal void WriteFunctionElementHeader(IEdmFunction function, bool inlineReturnType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Function);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, function.Name, StringAsXml);
            if (inlineReturnType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_ReturnType, function.ReturnType, this.TypeReferenceAsXml);
            }
        }

        internal void WriteDefiningExpressionElement(string expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_DefiningExpression);
            this.xmlWriter.WriteString(expression);
            this.xmlWriter.WriteEndElement();
        }

        internal void WriteReturnTypeElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReturnType);
        }

        internal void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_FunctionImport);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, functionImport.Name, StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_ReturnType, functionImport.ReturnType, this.TypeReferenceAsXml);

            // SideEffecting is optional, however its default applies to non-composable function imports only.
            // Composable function imports can't be side-effecting, so we don't emit false. However we emit true, 
            // to preserve the invalid model roundtripability.
            if (functionImport.Composable && functionImport.SideEffecting ||
                !functionImport.Composable && functionImport.SideEffecting != CsdlConstants.Default_IsSideEffecting)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_IsSideEffecting, functionImport.SideEffecting, BooleanAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsComposable, functionImport.Composable, CsdlConstants.Default_IsComposable, BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsBindable, functionImport.Bindable, CsdlConstants.Default_IsBindable, BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySetPath, functionImport.EntitySetPath, StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySet, functionImport.EntitySet, this.EntitySetAsXml);
        }

        internal void WriteFunctionParameterElementHeader(IEdmFunctionParameter parameter, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Parameter);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, parameter.Name, StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, parameter.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_Mode, parameter.Mode, CsdlConstants.Default_FunctionParameterMode, FunctionParameterModeAsXml);
        }

        internal void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType)
        {
            Debug.Assert(!collectionType.IsAtomic, "Atomic collections must be serialized as multivalue");
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

        internal void WriteEntityReferenceTypeElementHeader(IEdmEntityReferenceType entityReferenceType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReferenceType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, entityReferenceType.EntityType, this.TypeDefinitionAsXml);
        }

        internal void WriteEndElement()
        {
            this.xmlWriter.WriteEndElement();
        }

        internal string TypeReferenceAsXml(IEdmTypeReference type)
        {
            if (type.TypeKind() == EdmTypeKind.Collection)
            {
                IEdmCollectionTypeReference collectionReference = type.AsCollection();
                Debug.Assert(collectionReference.ElementType().Definition is IEdmSchemaElement, "Cannot inline parameter type if not a named element or collection of named elements");
                if (collectionReference.IsAtomic())
                {
                    return CsdlConstants.Value_MultiValue + "(" + collectionReference.ElementType().FullName() + ")";
                }

                return CsdlConstants.Value_Collection + "(" + collectionReference.ElementType().FullName() + ")";
            }

            Debug.Assert(type.Definition is IEdmSchemaElement, "Cannot inline parameter type if not a named element or collection of named elements");
            return type.FullName();
        }

        internal string TypeDefinitionAsXml(IEdmSchemaType type)
        {
            return type.FullName();
        }

        internal string EntitySetAsXml(IEdmEntitySet set)
        {
            return set.Name;
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, T defaultValue, Func<T, string> ToXml)
        {
           if (!value.Equals(defaultValue))
           {
               this.xmlWriter.WriteAttributeString(attribute, ToXml(value));
           }
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, Func<T, string> ToXml)
        {
            if (value != null)
            {
                this.xmlWriter.WriteAttributeString(attribute, ToXml(value));
            }
        }

        internal void WriteRequiredAttribute<T>(string attribute, T value, Func<T, string> ToXml)
        {
            this.xmlWriter.WriteAttributeString(attribute, ToXml(value));
        }

        private static string MultiplicityAsXml(EdmAssociationMultiplicity endKind)
        {
            switch (endKind)
            {
                case EdmAssociationMultiplicity.Many:
                    return CsdlConstants.Value_EndMany;
                case EdmAssociationMultiplicity.One:
                    return CsdlConstants.Value_EndRequired;
                case EdmAssociationMultiplicity.ZeroOrOne:
                    return CsdlConstants.Value_EndOptional;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_Multiplicity(endKind.ToString()));
            }
        }

        private static string FunctionParameterModeAsXml(EdmFunctionParameterMode mode)
        {
            switch(mode)
            {
                case EdmFunctionParameterMode.In:
                    return CsdlConstants.Value_ModeIn;
                case EdmFunctionParameterMode.InOut:
                    return CsdlConstants.Value_ModeInOut;
                case EdmFunctionParameterMode.Out:
                    return CsdlConstants.Value_ModeOut;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_FunctionParameterMode(mode.ToString()));
            }
        }

        private static string ConcurrencyModeAsXml(EdmConcurrencyMode mode)
        {
            switch(mode)
            {
                case EdmConcurrencyMode.Fixed:
                    return CsdlConstants.Value_Fixed;
                case EdmConcurrencyMode.None:
                    return CsdlConstants.Value_None;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ConcurrencyMode(mode.ToString()));
            }
        }

        private static string BooleanAsXml(bool b)
        {
            return (b) ? CsdlConstants.Value_True : CsdlConstants.Value_False;
        }

        private static string BooleanAsXml(bool? b)
        {
            Debug.Assert(b.HasValue, "Serialized nullable boolean must have value.");
            return BooleanAsXml(b.Value);
        }

        private static string IntAsXml(int? i)
        {
            Debug.Assert(i.HasValue, "Serialized nullable integer must have value.");
            return Convert.ToString(i.Value, CultureInfo.InvariantCulture);
        }

        private static string SridAsXml(int? i)
        {
            return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_SridVariable;
        }

        private static string StringAsXml(string s)
        {
            return s;
        }

        private static string PrimitiveValueAsXml(IEdmPrimitiveValue v)
        {
            // TODO: Handle other values
            return ((IEdmStringValue)v).Value;
        }

        private static string GetCsdlNamespace(Version edmVersion)
        {
            string[] @namespaces;
            if (CsdlConstants.SupportedVersions.TryGetValue(edmVersion, out @namespaces))
            {
                return @namespaces[0];
            }

            throw new InvalidOperationException(Edm.Strings.Serializer_UnknownEdmVersion);
        }

        private static string CreateUsingAlias(string usingNamespace)
        {
            // Dots are invalid in namespace aliases.
            return usingNamespace.Replace(".", "_") + "_Alias";
        }
    }
}
