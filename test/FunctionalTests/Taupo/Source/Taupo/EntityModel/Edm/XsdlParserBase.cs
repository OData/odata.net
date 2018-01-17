//---------------------------------------------------------------------
// <copyright file="XsdlParserBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Base class for CSDL and SSDL parser
    /// </summary>
    public abstract class XsdlParserBase
    {
        private List<string> entityTypeFullNames;
        private List<string> complexTypeFullNames;
        private List<string> enumTypeFullNames;

        private Dictionary<string, string> alias2namespace;

        /// <summary>
        /// Gets the namespace applied to the current entity model schema
        /// </summary>
        protected string CurrentNamespace { get; private set; }

        /// <summary>
        /// Parses the specified CSDL/SSDL content.
        /// </summary>
        /// <param name="xsdlContent">Content of multiple CSDL or multiple SSDL files.</param>
        /// <returns>
        /// Instance of <see cref="EntityModelSchema"/> which represents Entity Model parsed from the files.
        /// </returns>
        public EntityModelSchema Parse(params XElement[] xsdlContent)
        {
            EntityModelSchema model = new EntityModelSchema();

            this.entityTypeFullNames = new List<string>();
            this.complexTypeFullNames = new List<string>();
            this.enumTypeFullNames = new List<string>();

            foreach (XElement xsdl in xsdlContent)
            {
                this.RegisterNominalTypes(xsdl);
            }

            foreach (XElement xsdl in xsdlContent)
            {
                this.ParseSingleXsdl(model, xsdl);
            }

            return model.Resolve();
        }

        /// <summary>
        /// Parses a single csdl/ssdl file.
        /// </summary>
        /// <param name="model">the entity model schema which the csdl/ssdl file parses to</param>
        /// <param name="schemaElement">the top level schema element in the csdl/ssdl file</param>
        protected virtual void ParseSingleXsdl(EntityModelSchema model, XElement schemaElement)
        {
            this.AssertXsdlElement(schemaElement, "Schema");

            this.SetupNamespaceAndAliases(schemaElement);

            foreach (var entityContainerElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "EntityContainer")))
            {
                model.Add(this.ParseEntityContainer(entityContainerElement));
            }

            foreach (var entityTypeElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "EntityType")))
            {
                model.Add(this.ParseEntityType(entityTypeElement));
            }

            foreach (var associationTypeElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "Association")))
            {
                model.Add(this.ParseAssociation(associationTypeElement));
            }

            foreach (var functionElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "Function")))
            {
                model.Add(this.ParseFunction(functionElement));
            }
        }

        /// <summary>
        /// Parses an entity container element in the csdl/ssdl file.
        /// </summary>
        /// <param name="entityContainerElement">the entity container element to parse</param>
        /// <returns>the parsed entity container object in the entity model schema</returns>
        protected virtual EntityContainer ParseEntityContainer(XElement entityContainerElement)
        {
            string name = entityContainerElement.GetRequiredAttributeValue("Name");
            var entityContainer = new EntityContainer(name);
            foreach (var entitySetElement in entityContainerElement.Elements().Where(el => this.IsXsdlElement(el, "EntitySet")))
            {
                entityContainer.Add(this.ParseEntitySet(entitySetElement));
            }

            foreach (var entitySetElement in entityContainerElement.Elements().Where(el => this.IsXsdlElement(el, "AssociationSet")))
            {
                entityContainer.Add(this.ParseAssociationSet(entitySetElement));
            }

            this.ParseAnnotations(entityContainer, entityContainerElement);
            return entityContainer;
        }

        /// <summary>
        /// Parses an entity type element in the csdl/ssdl file.
        /// </summary>
        /// <param name="entityTypeElement">the entity type element to parse</param>
        /// <returns>the parsed entity type object in the entity model schema</returns>
        protected virtual EntityType ParseEntityType(XElement entityTypeElement)
        {
            string name = entityTypeElement.GetRequiredAttributeValue("Name");
            var entityType = new EntityType(this.CurrentNamespace, name);

            foreach (var propertyElement in entityTypeElement.Elements().Where(el => this.IsXsdlElement(el, "Property")))
            {
                entityType.Properties.Add(this.ParseProperty(propertyElement));
            }

            var keyElement = entityTypeElement.Elements().Where(c => this.IsXsdlElement(c, "Key")).SingleOrDefault();
            if (keyElement != null)
            {
                foreach (var propertyRefElement in keyElement.Elements().Where(c => this.IsXsdlElement(c, "PropertyRef")))
                {
                    string propertyName = propertyRefElement.GetRequiredAttributeValue("Name");
                    var property = entityType.Properties.Single(p => p.Name == propertyName);
                    property.IsPrimaryKey = true;
                }
            }

            this.ParseAnnotations(entityType, entityTypeElement);
            return entityType;
        }

        /// <summary>
        /// Parses a full type name
        /// </summary>
        /// <param name="fullTypeName">the full name of the type</param>
        /// <param name="isNullable">whether the type is nullable or not</param>
        /// <param name="facets">the facets attributes associated with the type</param>
        /// <returns>the data type representation in the entity mdoel schema</returns>
        protected DataType ParseType(string fullTypeName, bool isNullable, IEnumerable<XAttribute> facets)
        {
            if (fullTypeName.StartsWith("Collection(", StringComparison.Ordinal))
            {
                string elementTypeName = this.ExtractElementTypeName(fullTypeName);
                return DataTypes.CollectionType.WithElementDataType(this.ParseType(elementTypeName, isNullable, facets));
            }
            else if (fullTypeName.StartsWith("Ref(", StringComparison.Ordinal))
            {
                string elementTypeName = this.ExtractElementTypeName(fullTypeName);
                return DataTypes.ReferenceType.WithEntityType((this.ParseType(elementTypeName, true, null) as EntityDataType).Definition);
            }
            else
            {
                return this.ParseDataTypeFromQualifiedTypeName(fullTypeName, isNullable, facets);
            }
        }

        /// <summary>
        /// Parses the property Element to determine the DataType of the Property
        /// </summary>
        /// <param name="propertyElement">Property Element used as input to find the DataType</param>
        /// <returns>DataType that is determined from the property element</returns>
        protected abstract DataType ParsePropertyDataType(XElement propertyElement);

        /// <summary>
        /// Parses an annotation element/attribute in the csdl/ssdl file.
        /// </summary>
        /// <param name="annotatedItem">the annotated item to add annotations to</param>
        /// <param name="xmlElement">the element to parse</param>
        protected void ParseAnnotations(AnnotatedItem annotatedItem, XElement xmlElement)
        {
            foreach (XElement element in xmlElement.Elements())
            {
                if (this.IsXsdlElement(element, "Documentation"))
                {
                    annotatedItem.Annotations.Add(this.ParseDocumentationAnnotation(element));
                    continue;
                }

                if (!this.IsXsdlNamespace(element.Name.NamespaceName))
                {
                    annotatedItem.Annotations.Add(new StructuralAnnotation { Content = element });
                }
            }

            foreach (XAttribute attrib in xmlElement.Attributes())
            {
                if (attrib.Name.NamespaceName == EdmConstants.CodegenNamespace)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(attrib.Name.NamespaceName))
                {
                    annotatedItem.Annotations.Add(new AttributeAnnotation { Content = attrib });
                    continue;
                }
            }
        }

        /// <summary>
        /// Parses an Edm type
        /// </summary>
        /// <param name="qualifiedTypeName">the qualified Edm type name</param>
        /// <returns>the list of 2 strings with EntityTypeNamespace the first, EntityTypeName the second.</returns>
        protected string[] ParseEdmTypeName(string qualifiedTypeName)
        {
            string entityTypeName = string.Empty;
            string entityTypeNamespace = string.Empty;
            string[] parsedEdmTypeName = new string[2];

            int lastDot = qualifiedTypeName.LastIndexOf('.');
            if (lastDot < 0)
            {
                entityTypeName = qualifiedTypeName;
                entityTypeNamespace = this.GetDefaultEdmTypeNamespace();
            }
            else
            {
                entityTypeNamespace = qualifiedTypeName.Substring(0, lastDot);
                entityTypeName = qualifiedTypeName.Substring(lastDot + 1);
                if (this.alias2namespace.ContainsKey(entityTypeNamespace))
                {
                    entityTypeNamespace = this.alias2namespace[entityTypeNamespace];
                }
            }

            parsedEdmTypeName[0] = entityTypeNamespace;
            parsedEdmTypeName[1] = entityTypeName;
            return parsedEdmTypeName;
        }

        /// <summary>
        /// Parses a function element
        /// </summary>
        /// <param name="parameterElement">the function element in the schema file</param>
        /// <returns>the function parameter representation in the entity model schema</returns>
        protected FunctionParameter ParseFunctionParameter(XElement parameterElement)
        {
            string parameterName = parameterElement.GetRequiredAttributeValue("Name");

            DataType parameterType;
            string parameterTypeName = parameterElement.GetOptionalAttributeValue("Type", null);
            bool isNullable = XmlConvert.ToBoolean(parameterElement.GetOptionalAttributeValue("Nullable", "true"));
            if (parameterTypeName != null)
            {
                parameterType = this.ParseType(parameterTypeName, isNullable, parameterElement.Attributes());
            }
            else
            {
                var parameterTypeElement = parameterElement.Elements().Single(e => this.IsXsdlNamespace(e.Name.NamespaceName));
                parameterType = this.ParseType(parameterTypeElement);
            }

            FunctionParameter parameter = new FunctionParameter()
            {
                Name = parameterName,
                DataType = parameterType,
            };

            string parameterMode = parameterElement.GetOptionalAttributeValue("Mode", null);
            if (parameterMode != null)
            {
                parameter.Mode = (FunctionParameterMode)Enum.Parse(typeof(FunctionParameterMode), parameterMode, true);
            }

            this.ParseAnnotations(parameter, parameterElement);
            return parameter;
        }

        /// <summary>
        /// Gets an integer facet value.
        /// </summary>
        /// <param name="facets">a list of facet xattributes</param>
        /// <param name="facetName">the name of the facet</param>
        /// <returns>the integer value of the facet</returns>
        protected int? GetIntFacetValue(IEnumerable<XAttribute> facets, string facetName)
        {
            if (facets == null)
            {
                return null;
            }

            var attribute = facets.SingleOrDefault(c => c.Name == facetName);
            if (attribute == null)
            {
                return null;
            }

            if (attribute.Value == "Max")
            {
                return -1;
            }

            return Convert.ToInt32(attribute.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets an boolean facet value.
        /// </summary>
        /// <param name="facets">a list of facet xattributes</param>
        /// <param name="facetName">the name of the facet</param>
        /// <returns>the boolean value of the facet</returns>
        protected bool? GetBoolFacetValue(IEnumerable<XAttribute> facets, string facetName)
        {
            if (facets == null)
            {
                return null;
            }

            var attribute = facets.SingleOrDefault(c => c.Name == facetName);
            if (attribute == null)
            {
                return null;
            }

            return Convert.ToBoolean(attribute.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Throws if the element does not belong to the namespace or the namespace of the element is not expected
        /// </summary>
        /// <param name="element">the element</param>
        /// <param name="localName">the local name of the element</param>
        protected void AssertXsdlElement(XElement element, string localName)
        {
            if (element.Name.LocalName != localName)
            {
                throw new TaupoInvalidOperationException("Invalid CSDL/SSDL data. Expected: '<" + localName + " />' got " + element + ".");
            }

            if (!this.IsXsdlNamespace(element.Name.NamespaceName))
            {
                throw new TaupoInvalidOperationException("Expected CSDL/SSDL namespace.");
            }
        }

        /// <summary>
        /// Returns a value indicating whether the element belongs to a namespace.
        /// </summary>
        /// <param name="el">the element</param>
        /// <param name="localName">the local name of the element</param>
        /// <returns>true if the element belongs to the namespace, false otherwise.</returns>
        protected abstract bool IsXsdlElement(XElement el, string localName);

        /// <summary>
        /// Returns a value indicating whether the namespacename is the expected namespace.
        /// </summary>
        /// <param name="namespaceName">the namespace name</param>
        /// <returns>true if the namespacename is one of csdl namespaces, false otherwise.</returns>
        protected abstract bool IsXsdlNamespace(string namespaceName);

        /// <summary>
        /// Returns the default namespace used by csdl/ssdl
        /// </summary>
        /// <returns>The default namespace used by csdl/ssdl</returns>
        protected abstract string GetDefaultEdmTypeNamespace();

        /// <summary>
        /// Parses the primitive type 
        /// </summary>
        /// <param name="typeName">the name of the primitive type</param>
        /// <param name="facets">the facets attributes associated with the type</param>
        /// <returns>the primitive data type representation in the entity mdoel schema</returns>
        protected abstract PrimitiveDataType ParsePrimitiveType(string typeName, IEnumerable<XAttribute> facets);

        /// <summary>
        /// Parses the entity set
        /// </summary>
        /// <param name="entitySetElement">the entityset element to parse</param>
        /// <returns>the entityset presentation in the entity model schema</returns>
        protected virtual EntitySet ParseEntitySet(XElement entitySetElement)
        {
            string entitySetName = entitySetElement.GetRequiredAttributeValue("Name");
            string entityTypeNamespace = this.ParseEdmTypeName(entitySetElement.GetRequiredAttributeValue("EntityType"))[0];
            string entityTypeName = this.ParseEdmTypeName(entitySetElement.GetRequiredAttributeValue("EntityType"))[1];
            var entitySet = new EntitySet(entitySetName, new EntityTypeReference(entityTypeNamespace, entityTypeName));
            this.ParseAnnotations(entitySet, entitySetElement);
            return entitySet;
        }

        /// <summary>
        /// Parses a function element
        /// </summary>
        /// <param name="functionElement">the XElement to represent a Function</param>
        /// <returns>the Function representation in entity data model</returns>
        protected virtual Function ParseFunction(XElement functionElement)
        {
            string name = functionElement.GetRequiredAttributeValue("Name");
            var function = new Function(this.CurrentNamespace, name);

            string returnTypeName = functionElement.GetOptionalAttributeValue("ReturnType", null);
            if (returnTypeName != null)
            {
                bool isNullable = XmlConvert.ToBoolean(functionElement.GetOptionalAttributeValue("Nullable", "true"));
                function.ReturnType = this.ParseType(returnTypeName, isNullable, null);
            }

            var returnTypeElement = functionElement.Elements().SingleOrDefault(el => this.IsXsdlElement(el, "ReturnType"));
            if (returnTypeElement != null)
            {
                returnTypeName = returnTypeElement.GetOptionalAttributeValue("Type", null);
                bool isNullable = XmlConvert.ToBoolean(returnTypeElement.GetOptionalAttributeValue("Nullable", "true"));
                if (returnTypeName != null)
                {
                    function.ReturnType = this.ParseType(returnTypeName, isNullable, returnTypeElement.Attributes());
                }
                else
                {
                    function.ReturnType = this.ParseType(returnTypeElement.Elements().Single(e => this.IsXsdlNamespace(e.Name.NamespaceName)));
                }
            }

            foreach (var parameterElement in functionElement.Elements().Where(el => this.IsXsdlElement(el, "Parameter")))
            {
                function.Parameters.Add(this.ParseFunctionParameter(parameterElement));
            }

            this.ParseAnnotations(function, functionElement);
            return function;
        }

        /// <summary>
        /// Parses a XElement that contains type information
        ///  Accepts an element that is a CollectionType, RowType, ReferenceType, and TypeRef
        /// </summary>
        /// <param name="typeElement">XElement that contains a Type element</param>
        /// <returns>DataType represented by the XElement</returns>
        protected DataType ParseType(XElement typeElement)
        {
            if (typeElement.Name.LocalName == "CollectionType")
            {
                string elementTypeName = typeElement.GetOptionalAttributeValue("ElementType", null);
                if (elementTypeName != null)
                {
                    bool isNullable = XmlConvert.ToBoolean(typeElement.GetOptionalAttributeValue("Nullable", "true"));
                    DataType dataType = this.ParseType(elementTypeName, isNullable, typeElement.Attributes());
                    return DataTypes.CollectionType.WithElementDataType(dataType);
                }
                else
                {
                    var elementType = typeElement.Elements().Single(e => this.IsXsdlNamespace(e.Name.NamespaceName));
                    return DataTypes.CollectionType.WithElementDataType(this.ParseType(elementType));
                }
            }
            else if (typeElement.Name.LocalName == "RowType")
            {
                var row = new RowType();
                foreach (var propertyElement in typeElement.Elements().Where(el => this.IsXsdlElement(el, "Property")))
                {
                    row.Properties.Add(this.ParseProperty(propertyElement));
                }

                return DataTypes.RowType.WithDefinition(row);
            }
            else if (typeElement.Name.LocalName == "ReferenceType")
            {
                DataType dataType = this.ParseType(typeElement.GetRequiredAttributeValue("Type"), true, null);
                return DataTypes.ReferenceType.WithEntityType((dataType as EntityDataType).Definition);
            }
            else if (typeElement.Name.LocalName == "TypeRef")
            {
                bool isNullable = XmlConvert.ToBoolean(typeElement.GetOptionalAttributeValue("Nullable", "true"));
                DataType dataType = this.ParseType(typeElement.GetRequiredAttributeValue("Type"), isNullable, typeElement.Attributes());
                return dataType;
            }
            else
            {
                throw new TaupoNotSupportedException("Unsupported data type element: " + typeElement.Name.LocalName);
            }
        }

        /// <summary>
        /// Parse a Property from its XElement
        /// </summary>
        /// <param name="propertyElement">XElement to parse Property from</param>
        /// <returns>A memberProperty</returns>
        protected virtual MemberProperty ParseProperty(XElement propertyElement)
        {
            var name = propertyElement.GetRequiredAttributeValue("Name");
            DataType dataType = this.ParsePropertyDataType(propertyElement);

            var memberProperty = new MemberProperty(name, dataType);

            this.ParseAnnotations(memberProperty, propertyElement);
            return memberProperty;
        }

        private AssociationSet ParseAssociationSet(XElement associationSetElement)
        {
            string associationSetName = associationSetElement.GetRequiredAttributeValue("Name");

            string associationTypeNamespace = this.ParseEdmTypeName(associationSetElement.GetRequiredAttributeValue("Association"))[0];
            string associationTypeName = this.ParseEdmTypeName(associationSetElement.GetRequiredAttributeValue("Association"))[1];

            var associationSet = new AssociationSet(associationSetName, new AssociationTypeReference(associationTypeNamespace, associationTypeName));

            foreach (var associationSetEndElement in associationSetElement.Elements().Where(el => this.IsXsdlElement(el, "End")))
            {
                associationSet.Ends.Add(this.ParseAssociationSetEnd(associationSetEndElement));
            }

            this.ParseAnnotations(associationSet, associationSetElement);
            return associationSet;
        }

        private AssociationSetEnd ParseAssociationSetEnd(XElement associationSetEndElement)
        {
            string roleName = associationSetEndElement.GetRequiredAttributeValue("Role");
            var entitySetName = associationSetEndElement.GetRequiredAttributeValue("EntitySet");

            var associationSetEnd = new AssociationSetEnd(roleName, entitySetName);
            this.ParseAnnotations(associationSetEnd, associationSetEndElement);
            return associationSetEnd;
        }

        private void RegisterNominalTypes(XElement schemaElement)
        {
            this.AssertXsdlElement(schemaElement, "Schema");

            string namespaceName = schemaElement.GetRequiredAttributeValue("Namespace");
            foreach (var entityTypeElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "EntityType")))
            {
                string name = entityTypeElement.GetRequiredAttributeValue("Name");
                this.entityTypeFullNames.Add(namespaceName + "." + name);
            }

            foreach (var complexTypeElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "ComplexType")))
            {
                string name = complexTypeElement.GetRequiredAttributeValue("Name");
                this.complexTypeFullNames.Add(namespaceName + "." + name);
            }

            foreach (var enumTypeElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "EnumType")))
            {
                string name = enumTypeElement.GetRequiredAttributeValue("Name");
                this.enumTypeFullNames.Add(namespaceName + "." + name);
            }
        }

        private void SetupNamespaceAndAliases(XElement schemaElement)
        {
            string namespaceName = schemaElement.GetRequiredAttributeValue("Namespace");
            string alias = schemaElement.GetOptionalAttributeValue("Alias", null);

            this.CurrentNamespace = namespaceName;
            this.alias2namespace = new Dictionary<string, string>();
            if (alias != null)
            {
                this.alias2namespace.Add(alias, namespaceName);
            }

            foreach (var usingElement in schemaElement.Elements().Where(el => this.IsXsdlElement(el, "Using")))
            {
                alias = usingElement.GetRequiredAttributeValue("Alias");
                namespaceName = usingElement.GetRequiredAttributeValue("Namespace");

                this.alias2namespace[alias] = namespaceName;
            }
        }

        private AssociationType ParseAssociation(XElement associationTypeElement)
        {
            string name = associationTypeElement.GetRequiredAttributeValue("Name");
            var association = new AssociationType(this.CurrentNamespace, name);

            foreach (var associationEndElement in associationTypeElement.Elements().Where(el => this.IsXsdlElement(el, "End")))
            {
                association.Ends.Add(this.ParseAssociationEnd(associationEndElement));
            }

            var constraintElement = associationTypeElement.Elements().Where(el => this.IsXsdlElement(el, "ReferentialConstraint")).SingleOrDefault();
            if (constraintElement != null)
            {
                association.ReferentialConstraint = this.ParseReferentialConstraint(constraintElement);
            }

            this.ParseAnnotations(association, associationTypeElement);
            return association;
        }

        private ReferentialConstraint ParseReferentialConstraint(XElement constraintElement)
        {
            var constraint = new ReferentialConstraint();
            var principalElement = constraintElement.Elements().Single(el => this.IsXsdlElement(el, "Principal"));
            var dependentElement = constraintElement.Elements().Single(el => this.IsXsdlElement(el, "Dependent"));

            constraint.PrincipalAssociationEnd = new AssociationEndReference(principalElement.GetRequiredAttributeValue("Role"));
            constraint.DependentAssociationEnd = new AssociationEndReference(dependentElement.GetRequiredAttributeValue("Role"));

            this.ParseConstraintProperties(constraint.PrincipalProperties, principalElement);
            this.ParseConstraintProperties(constraint.DependentProperties, dependentElement);

            this.ParseAnnotations(constraint, constraintElement);
            return constraint;
        }

        private void ParseConstraintProperties(IList<MemberProperty> propertyList, XElement element)
        {
            foreach (var propertyRefElement in element.Elements().Where(el => this.IsXsdlElement(el, "PropertyRef")))
            {
                propertyList.Add(new MemberPropertyReference(propertyRefElement.GetRequiredAttributeValue("Name")));
            }
        }

        private AssociationEnd ParseAssociationEnd(XElement associationEndElement)
        {
            var roleName = associationEndElement.GetRequiredAttributeValue("Role");

            string entityTypeNamespace = this.ParseEdmTypeName(associationEndElement.GetRequiredAttributeValue("Type"))[0];
            string entityTypeName = this.ParseEdmTypeName(associationEndElement.GetRequiredAttributeValue("Type"))[1];

            EndMultiplicity multiplicity = this.ParseEndMultiplicity(associationEndElement.GetRequiredAttributeValue("Multiplicity"));
            OperationAction onDeleteAction = associationEndElement.Descendants().Any(e => e.Name.LocalName.Equals("OnDelete") && e.Attribute("Action").Value.Equals("Cascade")) ? OperationAction.Cascade : OperationAction.None;
            AssociationEnd end = new AssociationEnd(roleName, new EntityTypeReference(entityTypeNamespace, entityTypeName), multiplicity, onDeleteAction);
            this.ParseAnnotations(end, associationEndElement);
            return end;
        }

        private EndMultiplicity ParseEndMultiplicity(string multiplicityString)
        {
            switch (multiplicityString)
            {
                case "1":
                    return EndMultiplicity.One;

                case "0..1":
                    return EndMultiplicity.ZeroOne;

                case "*":
                    return EndMultiplicity.Many;

                default:
                    throw new TaupoNotSupportedException("Not supported multiplicity: " + multiplicityString);
            }
        }

        private Annotation ParseDocumentationAnnotation(XElement element)
        {
            var summaryElement = element.Elements().Where(c => this.IsXsdlElement(c, "Summary")).SingleOrDefault();
            var longDescriptionElement = element.Elements().Where(c => this.IsXsdlElement(c, "LongDescription")).SingleOrDefault();

            DocumentationAnnotation doc = new DocumentationAnnotation();
            if (summaryElement != null)
            {
                doc.Summary = summaryElement.Value;
            }

            if (longDescriptionElement != null)
            {
                doc.LongDescription = longDescriptionElement.Value;
            }

            return doc;
        }

        private string ExtractElementTypeName(string fullTypeName)
        {
            int openingBracketIndex = fullTypeName.IndexOf("(", StringComparison.OrdinalIgnoreCase);
            int closingBracketIndex = fullTypeName.LastIndexOf(")", StringComparison.OrdinalIgnoreCase);
            string elementTypeName = fullTypeName.Substring(openingBracketIndex + 1, closingBracketIndex - openingBracketIndex - 1);

            return elementTypeName;
        }

        private DataType ParseDataTypeFromQualifiedTypeName(string qualifiedTypeName, bool isNullable, IEnumerable<XAttribute> facets)
        {
            string typeNamespace = this.ParseEdmTypeName(qualifiedTypeName)[0];
            string typeName = this.ParseEdmTypeName(qualifiedTypeName)[1];

            if (typeNamespace != this.GetDefaultEdmTypeNamespace())
            {
                if (this.complexTypeFullNames.Contains(typeNamespace + "." + typeName))
                {
                    return DataTypes.ComplexType.WithName(typeNamespace, typeName).Nullable(isNullable);
                }
                else if (this.entityTypeFullNames.Contains(typeNamespace + "." + typeName))
                {
                    return DataTypes.EntityType.WithName(typeNamespace, typeName).Nullable(isNullable);
                }
                else if (this.enumTypeFullNames.Contains(typeNamespace + "." + typeName))
                {
                    return DataTypes.EnumType.WithName(typeNamespace, typeName).Nullable(isNullable);
                }
                else
                {
                    throw new TaupoInvalidOperationException("Type " + typeNamespace + "." + typeName + " cannot be resolved.");
                }
            }
            else
            {
                return this.ParsePrimitiveType(typeName, facets).Nullable(isNullable);
            }
        }
    }
}
