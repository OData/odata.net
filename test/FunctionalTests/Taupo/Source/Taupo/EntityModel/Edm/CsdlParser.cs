//---------------------------------------------------------------------
// <copyright file="CsdlParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Parses CSDL files.
    /// </summary>
    [ImplementationName(typeof(ICsdlParser), "Default")]
    public sealed class CsdlParser : XsdlParserBase, ICsdlParser
    {
        private static readonly Dictionary<string, PrimitiveDataType> name2EdmDataType = 
            EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.Latest).ToDictionary(t => EdmDataTypes.GetEdmName(t), t => t);

        private static readonly string[] validCsdlNamespaceNames = 
            new[]
            {
                EdmConstants.CsdlOasisNamespace.NamespaceName,
            };

        /// <summary>
        /// Gets or sets a value indicating whether to consider SRID while parsing Spatial data types
        /// </summary>
        public bool ConsiderSrid { get; set; }

        /// <summary>
        /// Returns a value indicating whether the element belongs to a namespace.
        /// </summary>
        /// <param name="el">the element</param>
        /// <param name="localName">the local name of the element</param>
        /// <returns>true if the element belongs to the namespace, false otherwise.</returns>
        protected override bool IsXsdlElement(XElement el, string localName)
        {
            if (!this.IsXsdlNamespace(el.Name.NamespaceName))
            {
                return false;
            }

            return el.Name.LocalName == localName;
        }

        /// <summary>
        /// Returns a value indicating whether the namespacename is the expected namespace.
        /// </summary>
        /// <param name="namespaceName">the namespace name</param>
        /// <returns>true if the namespacename is one of csdl namespaces, false otherwise.</returns>
        protected override bool IsXsdlNamespace(string namespaceName)
        {
            return validCsdlNamespaceNames.Contains(namespaceName);
        }

        /// <summary>
        /// Returns the default namespace used by csdl
        /// </summary>
        /// <returns>The default namespace used by csdl</returns>
        protected override string GetDefaultEdmTypeNamespace()
        {
            return "Edm";
        }

        /// <summary>
        /// Parses a single csdl file.
        /// </summary>
        /// <param name="model">the entity model schema which the csdl file parses to</param>
        /// <param name="schemaElement">the top level schema element in the csdl file</param>
        protected override void ParseSingleXsdl(EntityModelSchema model, XElement schemaElement)
        {
            base.ParseSingleXsdl(model, schemaElement);

            foreach (var complexTypeElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "ComplexType")))
            {
                model.Add(this.ParseComplexType(complexTypeElement));
            }

            foreach (var enumTypeElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "EnumType")))
            {
                model.Add(this.ParseEnumType(enumTypeElement));
            }

            this.ProcessAllContainersWithExtends(
                model,
                schemaElement.Elements().Where(el => this.IsXsdlElement(el, "EntityContainer") && el.Attributes().Any(a => a.Name == "Extends")));
        }

        /// <summary>
        /// Parses an entity container element in the csdl file.
        /// </summary>
        /// <param name="entityContainerElement">the entity container element to parse</param>
        /// <returns>the parsed entity container object in the entity model schema</returns>
        protected override EntityContainer ParseEntityContainer(XElement entityContainerElement)
        {
            var entityContainer = base.ParseEntityContainer(entityContainerElement);
            foreach (var functionImportElement in entityContainerElement.Elements().Where(el => this.IsXsdlElement(el, "FunctionImport")))
            {
                entityContainer.Add(this.ParseFunctionImport(functionImportElement));
            }

            string typeaccess = entityContainerElement.GetOptionalAttributeValue(EdmConstants.CodegenNamespace + "TypeAccess", null);
            if (typeaccess != null)
            {
                entityContainer.Annotations.Add(new TypeAccessModifierAnnotation(this.GetAccessModifier(typeaccess)));
            }

            return entityContainer;
        }

        /// <summary>
        /// Parses an entity type element in the csdl file.
        /// </summary>
        /// <param name="entityTypeElement">the entity type element to parse</param>
        /// <returns>the parsed entity type object in the entity model schema</returns>
        protected override EntityType ParseEntityType(XElement entityTypeElement)
        {
            var entityType = base.ParseEntityType(entityTypeElement);

            this.ParseNamedStructuralTypeAttributes(entityTypeElement, entityType);

            foreach (var navigationPropertyElement in entityTypeElement.Elements().Where(el => this.IsXsdlElement(el, "NavigationProperty")))
            {
                entityType.NavigationProperties.Add(this.ParseNavigationProperty(navigationPropertyElement));
            }

            return entityType;
        }

        /// <summary>
        /// Parses the property Element to determine the DataType of the Property
        /// </summary>
        /// <param name="propertyElement">Property Element used as input to find the DataType</param>
        /// <returns>DataType that is determined from the property element</returns>
        protected override DataType ParsePropertyDataType(XElement propertyElement)
        {
            bool isNullable = XmlConvert.ToBoolean(propertyElement.GetOptionalAttributeValue("Nullable", "true"));
            string parameterTypeName = propertyElement.GetOptionalAttributeValue("Type", null);
            if (parameterTypeName != null)
            {
                return this.ParseType(parameterTypeName, isNullable, propertyElement.Attributes());
            }
            else
            {
                var parameterTypeElement = propertyElement.Elements().Single(e => this.IsXsdlNamespace(e.Name.NamespaceName));
                return this.ParseType(parameterTypeElement);
            }
        }

        /// <summary>
        /// Parses the primitive type 
        /// </summary>
        /// <param name="typeName">the name of the primitive type</param>
        /// <param name="facets">the facets attributes associated with the type</param>
        /// <returns>the primitive data type representation in the entity model schema</returns>
        protected override PrimitiveDataType ParsePrimitiveType(string typeName, IEnumerable<XAttribute> facets)
        {
            IEnumerable<string> spatialTypes =
                EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.V40)
                            .OfType<SpatialDataType>()
                            .Select(t => EdmDataTypes.GetEdmName(t));

            if (typeName == "Binary")
            {
                return EdmDataTypes.Binary(
                    this.GetIntFacetValue(facets, "MaxLength"));
            }
            else if (typeName == "DateTime")
            {
                return EdmDataTypes.DateTime(this.GetIntFacetValue(facets, "Precision"));
            }
            else if (typeName == "DateTimeOffset")
            {
                return EdmDataTypes.DateTimeOffset(this.GetIntFacetValue(facets, "Precision"));
            }
            else if (typeName == "Decimal")
            {
                return EdmDataTypes.Decimal(
                    this.GetIntFacetValue(facets, "Precision"),
                    this.GetIntFacetValue(facets, "Scale"));
            }
            else if (typeName == "String")
            {
                return EdmDataTypes.String(
                    this.GetIntFacetValue(facets, "MaxLength"),
                    this.GetBoolFacetValue(facets, "Unicode"));
            }
            else if (typeName == "Duration")
            {
                return EdmDataTypes.Time(this.GetIntFacetValue(facets, "Precision"));
            }
            else if (this.ConsiderSrid)
            {
                if (spatialTypes.Contains(typeName))
                {
                    SpatialDataType spatialDataType = name2EdmDataType[typeName] as SpatialDataType;
                    return this.GetSpatialDataTypeWithSrid(spatialDataType, facets);
                }
                else
                {
                    ExceptionUtilities.Assert(name2EdmDataType.ContainsKey(typeName), "EDM type '" + typeName + "' not supported.");
                    return name2EdmDataType[typeName];
                }
            }
            else
            {
                ExceptionUtilities.Assert(name2EdmDataType.ContainsKey(typeName), "EDM type '" + typeName + "' not supported.");
                return name2EdmDataType[typeName];
            }
        }

        /// <summary>
        /// Parse a Property from its XElement
        /// </summary>
        /// <param name="propertyElement">XElement to parse Property from</param>
        /// <returns>A memberProperty</returns>
        protected override MemberProperty ParseProperty(XElement propertyElement)
        {
            MemberProperty memberProperty = base.ParseProperty(propertyElement);

            string defaultValueString = propertyElement.GetOptionalAttributeValue("DefaultValue", null);
            if (defaultValueString != null)
            {
                memberProperty.DefaultValue = this.ParseDefaultValueString(defaultValueString, memberProperty.PropertyType);
            }

            string getteraccess = propertyElement.GetOptionalAttributeValue(EdmConstants.CodegenNamespace + "GetterAccess", null);
            string setteraccess = propertyElement.GetOptionalAttributeValue(EdmConstants.CodegenNamespace + "SetterAccess", null);

            if (getteraccess != null || setteraccess != null)
            {
                AccessModifier setter;
                AccessModifier getter;
                this.GetGetterAndSetterModifiers(getteraccess, setteraccess, out setter, out getter);
                memberProperty.Annotations.Add(new PropertyAccessModifierAnnotation(setter, getter));
            }

            string collectionKindString = propertyElement.GetOptionalAttributeValue("CollectionKind", null);
            if (collectionKindString != null)
            {
                CollectionKind kind = (CollectionKind)Enum.Parse(typeof(CollectionKind), collectionKindString, false);
                memberProperty.Annotations.Add(new CollectionKindAnnotation(kind));
            }

            return memberProperty;
        }

        /// <summary>
        /// Parses a function element
        /// </summary>
        /// <param name="functionElement">the XElement to represent a Function</param>
        /// <returns>the Function representation in conceptual entity data model</returns>
        protected override Function ParseFunction(XElement functionElement)
        {
            Function function = base.ParseFunction(functionElement);

            var definingExpressionElement = functionElement.Elements().Where(el => this.IsXsdlElement(el, "DefiningExpression")).SingleOrDefault();
            if (definingExpressionElement != null)
            {
                function.Annotations.Add(new FunctionInModelAnnotation() { DefiningExpression = definingExpressionElement.Value });
            }

            return function;
        }

        private void ParseNamedStructuralTypeAttributes(XElement typeElement, NamedStructuralType type)
        {
            type.IsAbstract = XmlConvert.ToBoolean(typeElement.GetOptionalAttributeValue("Abstract", "false"));
            type.IsOpen = XmlConvert.ToBoolean(typeElement.GetOptionalAttributeValue("OpenType", "false"));

            string baseTypeFullName = typeElement.GetOptionalAttributeValue("BaseType", null);
            if (baseTypeFullName != null)
            {
                string[] typeNameInfo = this.ParseEdmTypeName(baseTypeFullName);
                string baseTypeNamespace = typeNameInfo[0];
                string baseTypeName = typeNameInfo[1];

                ComplexType complexType = type as ComplexType;
                EntityType entityType = type as EntityType;
                if (complexType != null)
                {
                    complexType.BaseType = new ComplexTypeReference(baseTypeNamespace, baseTypeName);
                }
                else
                {
                    ExceptionUtilities.Assert(entityType != null, "{0} is neither Entity nor Complex, but {1}!", type.FullName, type.GetType());
                    entityType.BaseType = new EntityTypeReference(baseTypeNamespace, baseTypeName);
                }
            }

            string typeaccess = typeElement.GetOptionalAttributeValue(EdmConstants.CodegenNamespace + "TypeAccess", null);
            if (typeaccess != null)
            {
                type.Annotations.Add(new TypeAccessModifierAnnotation(this.GetAccessModifier(typeaccess)));
            }
        }

        private ComplexType ParseComplexType(XElement complexTypeElement)
        {
            string name = complexTypeElement.GetRequiredAttributeValue("Name");
            var complexType = new ComplexType(this.CurrentNamespace, name);

            this.ParseNamedStructuralTypeAttributes(complexTypeElement, complexType);

            foreach (var propertyElement in complexTypeElement.Elements().Where(el => this.IsXsdlElement(el, "Property")))
            {
                complexType.Properties.Add(this.ParseProperty(propertyElement));
            }

            this.ParseAnnotations(complexType, complexTypeElement);
            return complexType;
        }

        private EnumType ParseEnumType(XElement enumTypeElement)
        {
            string name = enumTypeElement.GetRequiredAttributeValue("Name");
            string underlyingTypeString = enumTypeElement.GetOptionalAttributeValue("UnderlyingType", null);

            Type underlyingType = null;
            if (underlyingTypeString != null)
            {
                string underlyingTypeName = this.ParseEdmTypeName(underlyingTypeString)[1];
                underlyingType = Type.GetType("System." + underlyingTypeName);
            }

            var isFlagsString = enumTypeElement.Attribute("IsFlags");
            var enumType = new EnumType(this.CurrentNamespace, name) { IsFlags = isFlagsString == null ? (bool?)null : bool.Parse(isFlagsString.Value), UnderlyingType = underlyingType };

            foreach (var memberElement in enumTypeElement.Elements().Where(el => this.IsXsdlElement(el, "Member")))
            {
                enumType.Members.Add(this.ParseEnumMember(memberElement));
            }

            this.ParseAnnotations(enumType, enumTypeElement);
            return enumType;
        }

        private EnumMember ParseEnumMember(XElement memberElement)
        {
            string name = memberElement.GetRequiredAttributeValue("Name");
            string valueString = memberElement.GetOptionalAttributeValue("Value", null);

            var member = new EnumMember(name, valueString);

            this.ParseAnnotations(member, memberElement);
            return member;
        }

        private NavigationProperty ParseNavigationProperty(XElement navigationPropertyElement)
        {
            var name = navigationPropertyElement.GetRequiredAttributeValue("Name");
            string relationshipNamespace = this.ParseEdmTypeName(navigationPropertyElement.GetRequiredAttributeValue("Relationship"))[0];
            string relationshipName = this.ParseEdmTypeName(navigationPropertyElement.GetRequiredAttributeValue("Relationship"))[1];

            string fromRole = navigationPropertyElement.GetRequiredAttributeValue("FromRole");
            string toRole = navigationPropertyElement.GetRequiredAttributeValue("ToRole");

            var navProp = new NavigationProperty(name, new AssociationTypeReference(relationshipNamespace, relationshipName), fromRole, toRole);
            this.ParseAnnotations(navProp, navigationPropertyElement);

            string getteraccess = navigationPropertyElement.GetOptionalAttributeValue(EdmConstants.CodegenNamespace + "GetterAccess", null);
            string setteraccess = navigationPropertyElement.GetOptionalAttributeValue(EdmConstants.CodegenNamespace + "SetterAccess", null);
            if (getteraccess != null || setteraccess != null)
            {
                AccessModifier setter;
                AccessModifier getter;
                this.GetGetterAndSetterModifiers(getteraccess, setteraccess, out setter, out getter);
                navProp.Annotations.Add(new PropertyAccessModifierAnnotation(setter, getter));
            }

            return navProp;
        }

        private void GetGetterAndSetterModifiers(string getteraccess, string setteraccess, out AccessModifier setter, out AccessModifier getter)
        {
            setter = setteraccess == null ? AccessModifier.Unspecified : this.GetAccessModifier(setteraccess);
            getter = getteraccess == null ? AccessModifier.Unspecified : this.GetAccessModifier(getteraccess);
        }

        private AccessModifier GetAccessModifier(string accessModifier)
        {
            ExceptionUtilities.Assert(accessModifier == "Internal" || accessModifier == "Private" || accessModifier == "Protected" || accessModifier == "Public", "Unhandles Access Modifier: " + accessModifier);
            return (AccessModifier)Enum.Parse(typeof(AccessModifier), accessModifier, false);
        }

        private SpatialDataType GetSpatialDataTypeWithSrid(SpatialDataType spatialDataType, IEnumerable<XAttribute> facets)
        {
            if (facets != null)
            {
                var attribute = facets.SingleOrDefault(c => c.Name == "SRID");
                if (attribute != null)
                {
                    return spatialDataType.WithSrid(attribute.Value);
                }
            }

            return spatialDataType;
        }

        private FunctionImport ParseFunctionImport(XElement functionImportElement)
        {
            string functionImportName = functionImportElement.GetRequiredAttributeValue("Name");

            var functionImport = new FunctionImport(functionImportName);

            bool isComposable = XmlConvert.ToBoolean(functionImportElement.GetOptionalAttributeValue("IsComposable", "false"));
            functionImport.IsComposable = isComposable;

            bool isBindable = XmlConvert.ToBoolean(functionImportElement.GetOptionalAttributeValue("IsBindable", "false"));
            functionImport.IsBindable = isBindable;

            bool isSideEffecting = XmlConvert.ToBoolean(functionImportElement.GetOptionalAttributeValue("IsSideEffecting", "true"));
            functionImport.IsSideEffecting = isSideEffecting;

            string entitySetPath = functionImportElement.GetOptionalAttributeValue("EntitySetPath", null);
            if (entitySetPath != null)
            {
                functionImport.Annotations.Add(new EntitySetPathAnnotation(entitySetPath));
            }

            foreach (var parameterElement in functionImportElement.Elements().Where(el => this.IsXsdlElement(el, "Parameter")))
            {
                functionImport.Parameters.Add(this.ParseFunctionParameter(parameterElement));
            }

            string returnTypeName = functionImportElement.GetOptionalAttributeValue("ReturnType", null);

            if (returnTypeName != null)
            {
                bool isNullable = XmlConvert.ToBoolean(functionImportElement.GetOptionalAttributeValue("Nullable", "true"));
                var returnType = new FunctionImportReturnType(this.ParseType(returnTypeName, isNullable, null));
                string entitySetName = functionImportElement.GetOptionalAttributeValue("EntitySet", null);
                if (entitySetName != null)
                {
                    returnType.EntitySet = new EntitySetReference(entitySetName);
                }

                functionImport.Add(returnType);
            }

            foreach (var returnTypeElement in functionImportElement.Elements().Where(el => this.IsXsdlElement(el, "ReturnType")))
            {
                var type = returnTypeElement.GetRequiredAttributeValue("Type");
                var returnType = new FunctionImportReturnType(this.ParseType(type, true, null));
                var entitySet = returnTypeElement.GetOptionalAttributeValue("EntitySet", null);
                if (entitySet != null)
                {
                    returnType.EntitySet = new EntitySetReference(entitySet);
                }

                functionImport.ReturnTypes.Add(returnType);
            }

            string methodaccess = functionImportElement.GetOptionalAttributeValue(EdmConstants.CodegenNamespace + "MethodAccess", null);
            if (methodaccess != null)
            {
                functionImport.Annotations.Add(new MethodAccessModifierAnnotation(this.GetAccessModifier(methodaccess)));
            }

            this.ParseAnnotations(functionImport, functionImportElement);
            return functionImport;
        }

        private object ParseDefaultValueString(string defaultValueString, DataType dataType)
        {
            var primitiveDataType = dataType as PrimitiveDataType;
            ExceptionUtilities.Assert(primitiveDataType != null, "Cannot Parse default value for non-primitive: {0}.", dataType);
            ExceptionUtilities.Assert(primitiveDataType.HasFacet<PrimitiveClrTypeFacet>(), "Cannot Parse default value since PrimitiveDataType does not have ClrType facet defined.");

            Type clrType = primitiveDataType.GetFacet<PrimitiveClrTypeFacet>().Value;
            if (clrType == typeof(string))
            {
                return defaultValueString;
            }
            else if (clrType == typeof(byte[]))
            {
                return this.ParseStringIntoByteArray(defaultValueString);
            }
            else if (clrType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(defaultValueString, CultureInfo.InvariantCulture);
            }
            else if (clrType == typeof(Guid))
            {
                return Guid.Parse(defaultValueString);
            }
            else if (clrType == typeof(TimeSpan))
            {
                return TimeSpan.Parse(defaultValueString, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(defaultValueString, clrType, CultureInfo.InvariantCulture);
        }

        private byte[] ParseStringIntoByteArray(string value)
        {
            // '0xA12B6' -> byte[] { 0x0A, 0x12, 0xB6 }
            ExceptionUtilities.Assert(value.StartsWith("0x", StringComparison.OrdinalIgnoreCase), "Binary value string must start with '0x', e.g. '0xFA89C'.");

            string digits = value.Substring(2);
            if (digits.Length % 2 == 1)
            {
                digits = "0" + digits;
            }

            int byteCount = digits.Length / 2;
            byte[] result = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                var byteString = new string(new char[] { digits[i * 2], digits[(i * 2) + 1] });
                result[i] = byte.Parse(byteString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return result;
        }

        private void ProcessAllContainersWithExtends(EntityModelSchema model, IEnumerable<XElement> containerElementsWithExtends)
        {
            IList<XElement> processedContainerElements = new List<XElement>();
            foreach (XElement containerElement in containerElementsWithExtends.ToArray())
            {
                this.ProcessSingleContainerWithExtends(model, containerElement, containerElementsWithExtends, processedContainerElements);
            }
        }

        private void ProcessSingleContainerWithExtends(
            EntityModelSchema model,
            XElement containerElement,
            IEnumerable<XElement> containerElementsWithExtends,
            IList<XElement> processedContainerElements)
        {
            if (!processedContainerElements.Contains(containerElement))
            {
                string baseContainerName = containerElement.Attribute("Extends").Value;
                var baseContainerElement = containerElementsWithExtends.SingleOrDefault(e => e.Attribute("Name").Value == baseContainerName);
                if (baseContainerElement != null)
                {
                    this.ProcessSingleContainerWithExtends(model, baseContainerElement, containerElementsWithExtends, processedContainerElements);
                }

                var baseContainer = model.EntityContainers.SingleOrDefault(c => c.Name == baseContainerName);
                var extendedContainer = model.EntityContainers.SingleOrDefault(c => c.Name == containerElement.Attribute("Name").Value);

                this.CloneContainerContents(baseContainer, extendedContainer);

                processedContainerElements.Add(containerElement);
            }
        }

        private void CloneContainerContents(EntityContainer baseContainer, EntityContainer extendedContainer)
        {
            foreach (var entitySet in baseContainer.EntitySets)
            {
                var clonedEntitySet = new EntitySet(entitySet.Name, entitySet.EntityType);
                extendedContainer.Add(clonedEntitySet);
            }

            foreach (var associationSet in baseContainer.AssociationSets)
            {
                var clonedAssociationSet = new AssociationSet(associationSet.Name, associationSet.AssociationType);
                foreach (var setEnd in associationSet.Ends)
                {
                    clonedAssociationSet.Add(new AssociationSetEnd(setEnd.AssociationEnd, setEnd.EntitySet.Name));
                }

                extendedContainer.Add(clonedAssociationSet);
            }

            foreach (var functionImport in baseContainer.FunctionImports)
            {
                var clonedFunctionImport = new FunctionImport(functionImport.Name);
                foreach (var returnType in functionImport.ReturnTypes)
                {
                    clonedFunctionImport.ReturnTypes.Add(returnType);
                }

                foreach (var parameter in functionImport.Parameters)
                {
                    clonedFunctionImport.Add(new FunctionParameter(parameter.Name, parameter.DataType, parameter.Mode));
                }

                extendedContainer.Add(clonedFunctionImport);
            }
        }
    }
}
