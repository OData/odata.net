//---------------------------------------------------------------------
// <copyright file="XsdlContentGeneratorBase.cs" company="Microsoft">
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
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Base class for CSDL and SSDL generator
    /// </summary>
    public abstract class XsdlContentGeneratorBase
    {
        private string outputExtension;

        /// <summary>
        /// Initializes a new instance of the XsdlContentGeneratorBase class for given output extension.
        /// </summary>
        /// <param name="outputExtension">Output extension with a leading period (".csdl" or ".ssdl")</param>
        protected XsdlContentGeneratorBase(string outputExtension)
        {
            this.outputExtension = outputExtension;
            this.ContainerDefaultNamespace = "DefaultNamespace";
            this.NamespaceAliasManager = new NamespaceAliasManager();
        }

        /// <summary>
        /// Gets or sets the NamespaceAliasManager.
        /// </summary>
        public NamespaceAliasManager NamespaceAliasManager { get; set; }

        /// <summary>
        /// Gets or sets default namespace to be used for EntityContainers if the model defines no types.
        /// </summary>
        public string ContainerDefaultNamespace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to generate Taupo-specific annotations.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if Taupo annotations should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateTaupoAnnotations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore unsupported annotations.
        /// </summary>
        /// <value>
        /// Value <c>true</c> if unsupported annotations should be ignored; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreUnsupportedAnnotations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use namespace alias "Self" in the generated csdl 
        /// </summary>
        public bool UseSelfAlias { get; set; }

        /// <summary>
        /// Adds a namespace alias in the generated csdls
        /// </summary>
        /// <param name="namespaceName">the namespace name</param>
        /// <param name="alias">the specified alias</param>
        public void AddNamespaceAlias(string namespaceName, string alias)
        {
            this.NamespaceAliasManager.AddNamespaceAlias(namespaceName, alias);
        }

        /// <summary>
        /// Gets the data type generator.
        /// </summary>
        /// <returns>The data type generator.</returns>
        protected abstract IXsdlDataTypeGenerator GetDataTypeGenerator();

        /// <summary>
        /// Generates CSDL/SSDL content for a given model.
        /// </summary>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <param name="model">Input model</param>
        /// <returns>List of generated file contents.</returns>
        protected IEnumerable<FileContents<XElement>> Generate(XNamespace xmlNamespace, EntityModelSchema model)
        {
            List<FileContents<XElement>> contents = new List<FileContents<XElement>>();
            Dictionary<string, IList<object>> namespace2Items = this.GroupModelItemsIntoNamespaces(model);
            foreach (var kvp in namespace2Items)
            {
                string currentNamespace = kvp.Key;
                IList<object> items = kvp.Value;

                this.NamespaceAliasManager.NamespaceForSelfAlias = this.UseSelfAlias ? currentNamespace : null;
                
                XElement content = new XElement(
                    xmlNamespace + "Schema",
                    new XAttribute("Namespace", currentNamespace),
                    this.GenerateNamespaceDeclarations(model),
                    this.GenerateTopLevelAttributes(),
                    this.GenerateUsingAliases(xmlNamespace),
                    this.GenerateEntityContainers(items.OfType<EntityContainer>(), xmlNamespace),
                    this.GenerateEntityTypes(items.OfType<EntityType>(), xmlNamespace),
                    this.GenerateComplexTypes(items.OfType<ComplexType>(), xmlNamespace),
                    this.GenerateAssociations(items.OfType<AssociationType>(), xmlNamespace),
                    this.GenerateFunctions(items.OfType<Function>(), xmlNamespace),
                    this.GenerateEnumTypes(items.OfType<EnumType>(), xmlNamespace));

                contents.Add(new FileContents<XElement>(currentNamespace + this.outputExtension, content));
            }

            return contents;
        }

        /// <summary>
        /// Generates the top-level namespace declarations.
        /// </summary>
        /// <param name="model">The entity schema model.</param>
        /// <returns>
        /// Sequence of top-level namespace declaration attributes.
        /// </returns>
        protected virtual IEnumerable<XAttribute> GenerateNamespaceDeclarations(EntityModelSchema model)
        {
            if (this.GenerateTaupoAnnotations)
            {
                yield return new XAttribute(XNamespace.Xmlns + "taupo", EdmConstants.TaupoAnnotationsNamespace.NamespaceName);
            }
        }

        /// <summary>
        /// Generates additional root-level attributes.
        /// </summary>
        /// <returns>Empty sequence, can be overridded in derived classes</returns>
        protected virtual IEnumerable<XAttribute> GenerateTopLevelAttributes()
        {
            if (this.UseSelfAlias)
            {
                yield return new XAttribute("Alias", "Self");
            }
        }

        /// <summary>
        /// Generates EntityContainer element from <see cref="EntityContainer"/>
        /// </summary>
        /// <param name="xmlNamespace">XML namespace to use</param>
        /// <param name="container">Entity container</param>
        /// <returns>'EntityContainer' <see cref="XElement"/></returns>
        protected virtual XElement GenerateEntityContainer(XNamespace xmlNamespace, EntityContainer container)
        {
            return new XElement(
                xmlNamespace + "EntityContainer",
                (container.Name != null ? new XAttribute("Name", container.Name) : null),
                this.GenerateDocumentation(xmlNamespace, container),
                this.GenerateEntitySets(container, xmlNamespace),
                this.GenerateAssociationSets(container, xmlNamespace),
                this.GenerateAdditionalEntityContainerItems(container, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, container));
        }

        /// <summary>
        /// Generates the additional entity container items.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <returns>Empty sequence, can be overridden in derived classes.</returns>
        protected virtual IEnumerable<XObject> GenerateAdditionalEntityContainerItems(EntityContainer container, XNamespace xmlNamespace)
        {
            return Enumerable.Empty<XObject>();
        }

        /// <summary>
        /// Generates the additional description for entity set
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>Empty sequence, can be overriden in derived classes.</returns>
        protected virtual IEnumerable<XObject> GenerateEntitySetAdditionalDescription(EntitySet entitySet, XNamespace xmlNamespace)
        {
            return Enumerable.Empty<XObject>();
        }

        /// <summary>
        /// Generates the documentation element for given annotated item.
        /// </summary>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <param name="annotatedItem">The annotated item.</param>
        /// <returns>Documentation XElement or null if <see cref="DocumentationAnnotation" /> is not present on the item.</returns>
        protected virtual XElement GenerateDocumentation(XNamespace xmlNamespace, AnnotatedItem annotatedItem)
        {
            var documentationAnnotation = annotatedItem.Annotations.OfType<DocumentationAnnotation>().SingleOrDefault();
            if (documentationAnnotation == null)
            {
                return null;
            }

            var element = new XElement(xmlNamespace + "Documentation");
            if (documentationAnnotation.Summary != null)
            {
                element.Add(new XElement(xmlNamespace + "Summary", documentationAnnotation.Summary));
            }

            if (documentationAnnotation.LongDescription != null)
            {
                element.Add(new XElement(xmlNamespace + "LongDescription", documentationAnnotation.LongDescription));
            }

            return element;
        }

        /// <summary>
        /// Generates annotations for a given item.
        /// </summary>
        /// <param name="outputNamespace">The output namespace.</param>
        /// <param name="annotatedItem">Item with structural annotations.</param>
        /// <returns>
        /// Sequence of <see cref="XElement"/> that represent structural annotations.
        /// </returns>
        protected virtual IEnumerable<XObject> GenerateAnnotations(XNamespace outputNamespace, IAnnotatedItem annotatedItem)
        {
            var results = new List<XObject>();

            foreach (Annotation annotation in annotatedItem.Annotations)
            {
                // if this annotation is handled elsewhere, skip it here
                if (this.IsSpecialAnnotation(annotation))
                {
                    continue;
                }

                // now this can be structural, attribute or custom
                var structuralAnnotation = annotation as StructuralAnnotation;
                if (structuralAnnotation != null)
                {
                    results.Add(structuralAnnotation.Content);
                    continue;
                }

                var attributeAnnotation = annotation as AttributeAnnotation;
                if (attributeAnnotation != null)
                {
                    results.Add(attributeAnnotation.Content);
                    continue;
                }

                if (this.GenerateTaupoAnnotations)
                {
                    var customAnnotationSerializer = annotation as ICustomAnnotationSerializer;
                    if (customAnnotationSerializer != null)
                    {
                        results.Add(customAnnotationSerializer.GetXObject());
                        continue;
                    }

                    // simple annotation without payload represent as taupo:Annotation="true"
                    if (annotation is TagAnnotation)
                    {
                        results.Add(new XAttribute(EdmConstants.TaupoAnnotationsNamespace + annotation.GetType().Name, true));
                        continue;
                    }

                    // composite annotations are serailized as:
                    // <taupo:Annotation>
                    //    <taupo:Property1>value</taupo:Property1>
                    //    <taupo:Property2>value</taupo:Property2>
                    // ...
                    //    <taupo:PropertyN>value</taupo:PropertyN>
                    // </taupo:Annotation>
                    if (annotation is CompositeAnnotation)
                    {
                        var element = new XElement(EdmConstants.TaupoAnnotationsNamespace + annotation.GetType().Name);
                        foreach (var prop in annotation.GetType().GetPublicProperties(true))
                        {
                            object value = prop.GetValue(annotation, null);
                            if (value != null)
                            {
                                element.Add(new XElement(EdmConstants.TaupoAnnotationsNamespace + prop.Name, Convert.ToString(value, CultureInfo.InvariantCulture)));
                            }
                        }

                        results.Add(element);
                        continue;
                    }

                    if (this.IgnoreUnsupportedAnnotations)
                    {
                        continue;
                    }

                    throw new TaupoNotSupportedException("Annotation '" + annotation.GetType().Name + "' cannot be serialized. Supported annotations must derive from: " + typeof(TagAnnotation).Name + ", " + typeof(CompositeAnnotation).Name + " or implement " + typeof(ICustomAnnotationSerializer).Name + ".");
                }
            }

            return results;
        }

        /// <summary>
        /// Generates the <see cref="XAttribute"/> that contains the default value for a <see cref="MemberProperty"/>.
        /// </summary>
        /// <param name="defaultValue">The default value of the <see cref="MemberProperty"/>.</param>
        /// <returns>The <see cref="XAttribute"/> that contains the default value for a <see cref="MemberProperty"/>.</returns>
        protected virtual XAttribute GenerateDefaultValue(object defaultValue)
        {
            if (defaultValue == null)
            {
                return null;
            }

            return new XAttribute("DefaultValue", defaultValue.ToString());
        }

        /// <summary>        
        /// Generates 'EntityType' XElement based on <see cref="EntityType"/>.
        /// </summary>
        /// <param name="xmlNamespace">XML namespace to use</param>
        /// <param name="entityType">Entity type</param>
        /// <returns><see cref="XElement"/> representing EntityType.</returns>
        protected virtual XElement GenerateEntityType(XNamespace xmlNamespace, EntityType entityType)
        {
            return new XElement(
                xmlNamespace + "EntityType",
                (entityType.IsAbstract ? new XAttribute("Abstract", "true") : null),
                (entityType.IsOpen ? new XAttribute("OpenType", "true") : null),
                (entityType.Name != null ? new XAttribute("Name", entityType.Name) : null),
                this.GenerateDocumentation(xmlNamespace, entityType),
                this.GenerateKeyOrBaseType(entityType, xmlNamespace),
                this.GenerateUniqueConstraints(entityType, xmlNamespace),
                this.GenerateProperties(entityType, xmlNamespace),
                this.GenerateNavigationProperties(entityType, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, entityType));
        }

        /// <summary>
        /// Generates Property element for a given <see cref="MemberProperty"/>
        /// </summary>
        /// <param name="xmlNamespace">XML namespace to use</param>
        /// <param name="prop">Entity or ComplexType property</param>
        /// <returns>Property XElement</returns>
        protected virtual XElement GenerateProperty(XNamespace xmlNamespace, MemberProperty prop)
        {
            return new XElement(
                xmlNamespace + "Property",
                prop.Name != null ? new XAttribute("Name", prop.Name) : null,
                this.GenerateDocumentation(xmlNamespace, prop),
                this.GetDataTypeGenerator().GeneratePropertyType(prop, xmlNamespace),
                this.GenerateDefaultValue(prop.DefaultValue),
                this.GenerateStoreGeneratedPattern(prop),
                this.GenerateAnnotations(xmlNamespace, prop));
        }

        /// <summary>
        /// Generates the store generated pattern attribute.
        /// </summary>
        /// <param name="memberProperty">The property.</param>
        /// <returns>Generated attribute</returns>
        protected abstract XAttribute GenerateStoreGeneratedPattern(MemberProperty memberProperty);

        /// <summary>
        /// Generates additional description for function
        /// </summary>
        /// <param name="modelFunction">the function</param>
        /// <param name="xmlNamespace">the xml namespace</param>
        /// <returns>some XElements and XAttributes for additional description</returns>
        protected abstract IEnumerable<XObject> GenerateFunctionAdditionalDescription(Function modelFunction, XNamespace xmlNamespace);

        /// <summary>
        /// Determines whether the specified annotation is a special annotation (handled by the generator
        /// rather than custom annotation).
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns>
        /// Value <c>true</c> if the specified annotation is a special annotation; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsSpecialAnnotation(Annotation annotation)
        {
            if (annotation is DocumentationAnnotation)
            {
                return true;
            }

            if (annotation is StoreGeneratedPatternAnnotation)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates the navigation property.
        /// </summary>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <param name="prop">The Navigation property.</param>
        /// <returns>IEnumerable of XElement</returns>
        protected virtual XElement GenerateNavigationProperty(XNamespace xmlNamespace, NavigationProperty prop)
        {
            return new XElement(
                xmlNamespace + "NavigationProperty",
                new XAttribute("Name", prop.Name),
                new XAttribute("Relationship", this.GetFullyQualifiedName(prop.Association)),
                new XAttribute("FromRole", prop.FromAssociationEnd.RoleName),
                new XAttribute("ToRole", prop.ToAssociationEnd.RoleName),
                this.GenerateDocumentation(xmlNamespace, prop),
                this.GenerateAnnotations(xmlNamespace, prop));
        }

        /// <summary>
        /// Generates the complex types.
        /// </summary>
        /// <param name="items">The collection of Complex Types.</param>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <returns>Complex Type XElement</returns>
        protected virtual IEnumerable<XElement> GenerateComplexTypes(IEnumerable<ComplexType> items, XNamespace xmlNamespace)
        {
            var content = new List<XElement>();
            foreach (var ct in items)
            {
                var element = new XElement(xmlNamespace + "ComplexType");
                element.Add((ct.Name != null ? new XAttribute("Name", ct.Name) : null));
                element.Add((ct.IsAbstract ? new XAttribute("Abstract", "true") : null));
                element.Add((ct.IsOpen ? new XAttribute("OpenType", "true") : null));
                element.Add((ct.BaseType != null ? new XAttribute("BaseType", this.GetFullyQualifiedName(ct.BaseType)) : null));
                element.Add(this.GenerateDocumentation(xmlNamespace, ct));
                element.Add(this.GenerateProperties(ct, xmlNamespace));
                var annotationElements = this.GenerateAnnotations(xmlNamespace, ct);
                element.Add(annotationElements);
                content.Add(element);
            }

            return content;
        }

        /// <summary>
        /// Generates the enum types
        /// </summary>
        /// <param name="enumTypes">the collection of Enum types</param>
        /// <param name="xmlNamespace">the XML namespace</param>
        /// <returns>the Enum type XElements</returns>
        protected virtual IEnumerable<XElement> GenerateEnumTypes(IEnumerable<EnumType> enumTypes, XNamespace xmlNamespace)
        {
            return Enumerable.Empty<XElement>();
        }

        /// <summary>
        /// Gets the fully qualified name
        /// </summary>
        /// <param name="type">the type name</param>
        /// <returns>the fully qualified name</returns>
        private string GetFullyQualifiedName(INamedItem type)
        {
            return this.NamespaceAliasManager.GetFullyQualifiedName(type);
        }

        private IEnumerable<XElement> GenerateUsingAliases(XNamespace xmlNamespace)
        {
            foreach (var namespaceToAlias in this.NamespaceAliasManager.GetNamespaceToAliasMappings())
            {
                if (namespaceToAlias.Value != "Self")
                {
                    yield return new XElement(
                        xmlNamespace + "Using",
                        new XAttribute("Namespace", namespaceToAlias.Key),
                        new XAttribute("Alias", namespaceToAlias.Value));
                }
            }
        }

        private XElement GenerateEntitySet(XNamespace xmlNamespace, EntitySet entitySet)
        {
            return new XElement(
                xmlNamespace + "EntitySet",
                (entitySet.Name != null ? new XAttribute("Name", entitySet.Name) : null),
                (entitySet.EntityType != null ? new XAttribute("EntityType", this.GetFullyQualifiedName(entitySet.EntityType)) : null),
                this.GenerateDocumentation(xmlNamespace, entitySet),
                this.GenerateEntitySetAdditionalDescription(entitySet, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, entitySet));
        }

        private XElement GenerateAssociationSet(XNamespace xmlNamespace, AssociationSet associationSet)
        {
            return new XElement(
                xmlNamespace + "AssociationSet",
                new XAttribute("Name", associationSet.Name),
                new XAttribute("Association", this.GetFullyQualifiedName(associationSet.AssociationType)),
                this.GenerateDocumentation(xmlNamespace, associationSet),
                this.GenerateAssociationSetEnds(xmlNamespace, associationSet),
                this.GenerateAnnotations(xmlNamespace, associationSet));
        }

        private IEnumerable<XElement> GenerateAssociationSetEnds(XNamespace xmlNamespace, AssociationSet associationSet)
        {
            var content = from setEnd in associationSet.Ends
                          select this.GenerateAssociationSetEnd(xmlNamespace, setEnd);
            return content;
        }

        private XElement GenerateAssociationSetEnd(XNamespace xmlNamespace, AssociationSetEnd setEnd)
        {
            return new XElement(
                xmlNamespace + "End",
                new XAttribute("Role", setEnd.AssociationEnd.RoleName),
                new XAttribute("EntitySet", setEnd.EntitySet.Name),
                this.GenerateDocumentation(xmlNamespace, setEnd),
                this.GenerateAnnotations(xmlNamespace, setEnd));
        }

        private Dictionary<string, IList<object>> GroupModelItemsIntoNamespaces(EntityModelSchema model)
        {
            var namespace2Items = new Dictionary<string, IList<object>>();

            this.GroupIntoNamespaces(model.EntityTypes.Cast<INamedItem>(), namespace2Items);
            this.GroupIntoNamespaces(model.ComplexTypes.Cast<INamedItem>(), namespace2Items);
            this.GroupIntoNamespaces(model.Associations.Cast<INamedItem>(), namespace2Items);
            this.GroupIntoNamespaces(model.Functions.Cast<INamedItem>(), namespace2Items);
            this.GroupIntoNamespaces(model.EnumTypes.Cast<INamedItem>(), namespace2Items);
           
            string containerNamespace = this.ContainerDefaultNamespace;
            if (namespace2Items.Keys.Count > 0)
            {
                containerNamespace = namespace2Items.Keys.First();
            }
            else
            {
                namespace2Items[containerNamespace] = new List<object>();
            }

            foreach (EntityContainer container in model.EntityContainers)
            {
                namespace2Items[containerNamespace].Add(container);
            }

            return namespace2Items;
        }

        private void GroupIntoNamespaces(IEnumerable<INamedItem> types, Dictionary<string, IList<object>> namespace2Items)
        {
            foreach (INamedItem nt in types)
            {
                if (nt.NamespaceName == null)
                {
                    throw new TaupoInvalidOperationException("Namespace for " + nt.GetType().Name + " cannot be null.");
                }

                IList<object> itemsForNamespace;

                if (!namespace2Items.TryGetValue(nt.NamespaceName, out itemsForNamespace))
                {
                    itemsForNamespace = new List<object>();
                    namespace2Items.Add(nt.NamespaceName, itemsForNamespace);
                }

                itemsForNamespace.Add(nt);
            }
        }

        private IEnumerable<XElement> GenerateEntityContainers(IEnumerable<EntityContainer> items, XNamespace xmlNamespace)
        {
            var content = from container in items
                          select this.GenerateEntityContainer(xmlNamespace, container);
            return content;
        }

        private IEnumerable<XElement> GenerateEntitySets(EntityContainer container, XNamespace xmlNamespace)
        {
            var content = from set in container.EntitySets
                          select this.GenerateEntitySet(xmlNamespace, set);
            return content;
        }

        private IEnumerable<XElement> GenerateAssociationSets(EntityContainer container, XNamespace xmlNamespace)
        {
            var content = from set in container.AssociationSets
                          select this.GenerateAssociationSet(xmlNamespace, set);
            return content;
        }

        private IEnumerable<XElement> GenerateFunctions(IEnumerable<Function> functions, XNamespace xmlNamespace)
        {
            var content = from f in functions
                          select this.GenerateFunction(f, xmlNamespace);
            return content;
        }

        private XElement GenerateFunction(Function function, XNamespace xmlNamespace)
        {
            return new XElement(
                xmlNamespace + "Function",
                (function.Name != null ? new XAttribute("Name", function.Name) : null),
                this.GenerateDocumentation(xmlNamespace, function),
                this.GetDataTypeGenerator().GenerateReturnTypeForFunction(function.ReturnType, xmlNamespace),                
                this.GenerateFunctionAdditionalDescription(function, xmlNamespace),
                this.GenerateFunctionParameters(function.Parameters, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, function));
        }

        private IEnumerable<XElement> GenerateFunctionParameters(IEnumerable<FunctionParameter> parameters, XNamespace xmlNamespace)
        {
            var content = from parameter in parameters
                          select new XElement(
                                xmlNamespace + "Parameter",
                                new XAttribute("Name", parameter.Name),
                                this.GenerateDocumentation(xmlNamespace, parameter),
                                this.GetDataTypeGenerator().GenerateParameterTypeForFunction(parameter, xmlNamespace),
                                (parameter.Mode != FunctionParameterMode.In ? new XAttribute("Mode", parameter.Mode.ToString()) : null),
                                this.GenerateAnnotations(xmlNamespace, parameter));
            return content;
        }

        private IEnumerable<XElement> GenerateEntityTypes(IEnumerable<EntityType> items, XNamespace xmlNamespace)
        {
            var content = from et in items
                          select this.GenerateEntityType(xmlNamespace, et);
            return content;
        }

        private XObject GenerateKeyOrBaseType(EntityType entityType, XNamespace xmlNamespace)
        {
            if (entityType.BaseType != null)
            {
                return new XAttribute("BaseType", this.GetFullyQualifiedName(entityType.BaseType));
            }

            var keyProperties = entityType.Properties
                .Where(p => p.IsPrimaryKey && p.Name != null)
                .Select(p => new XElement(xmlNamespace + "PropertyRef", new XAttribute("Name", p.Name)))
                .ToArray();

            if (keyProperties.Length != 0)
            {
                return new XElement(xmlNamespace + "Key", keyProperties);
            }

            return null;
        }

        private IEnumerable<XElement> GenerateUniqueConstraints(EntityType entityType, XNamespace xmlNamespace)
        {
            List<XElement> uniqueConstraints = new List<XElement>();
            foreach (EdmUniqueConstraint edmUniqueConstraint in entityType.EdmUniqueConstraints)
            {
                var uniqueProperties = edmUniqueConstraint.Properties
                    .Select(p => new XElement(xmlNamespace + "PropertyRef", new XAttribute("Name", p.Name)))
                    .ToArray();

                var name = new XAttribute("Name", edmUniqueConstraint.Name);
                uniqueConstraints.Add(new XElement(xmlNamespace + "UniqueConstraint", name, uniqueProperties));
            }

            return uniqueConstraints;
        }

        private IEnumerable<XElement> GenerateProperties(StructuralType structuralType, XNamespace xmlNamespace)
        {
            var content = from prop in structuralType.Properties
                          select this.GenerateProperty(xmlNamespace, prop);
            return content;
        }

        private IEnumerable<XElement> GenerateNavigationProperties(EntityType entityType, XNamespace xmlNamespace)
        {
            var content = from prop in entityType.NavigationProperties
                          select this.GenerateNavigationProperty(xmlNamespace, prop);
            return content;
        }

        private IEnumerable<XElement> GenerateAssociations(IEnumerable<AssociationType> items, XNamespace xmlNamespace)
        {
            var content = from association in items
                          select this.GenerateAssociation(xmlNamespace, association);
            return content;
        }

        private XElement GenerateAssociation(XNamespace xmlNamespace, AssociationType association)
        {
            return new XElement(
                xmlNamespace + "Association",
                new XAttribute("Name", association.Name),
                this.GenerateDocumentation(xmlNamespace, association),
                this.GenerateAssociationEnds(association, xmlNamespace),
                association.ReferentialConstraint == null ? null : this.GenerateReferentialConstrait(association.ReferentialConstraint, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, association));
        }

        private IEnumerable<XElement> GenerateAssociationEnds(AssociationType association, XNamespace xmlNamespace)
        {
            var content = from end in association.Ends
                          select this.GenerateAssociationEnd(xmlNamespace, end);
            return content;
        }

        private XElement GenerateAssociationEnd(XNamespace xmlNamespace, AssociationEnd end)
        {
            return new XElement(
                xmlNamespace + "End",
                new XAttribute("Role", end.RoleName),
                new XAttribute("Type", this.GetFullyQualifiedName(end.EntityType)),
                new XAttribute("Multiplicity", this.GetMultiplicityString(end.Multiplicity)),
                this.GenerateDocumentation(xmlNamespace, end),
                this.GenerateOnDelete(end, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, end));
        }

        private string GetMultiplicityString(EndMultiplicity endMultiplicity)
        {
            switch (endMultiplicity)
            {
                case EndMultiplicity.Many:
                    return "*";
                case EndMultiplicity.One:
                    return "1";
                case EndMultiplicity.ZeroOne:
                    return "0..1";
                default:
                    throw new TaupoNotSupportedException("End multiplicity: " + endMultiplicity + " is not supported by current implementation.");
            }
        }

        private XElement GenerateReferentialConstrait(ReferentialConstraint rc, XNamespace xmlNamespace)
        {
            return new XElement(
                xmlNamespace + "ReferentialConstraint",
                this.GenerateDocumentation(xmlNamespace, rc),
                this.GenerateReferentialEnd("Principal", rc.PrincipalAssociationEnd, rc.PrincipalProperties, xmlNamespace),
                this.GenerateReferentialEnd("Dependent", rc.DependentAssociationEnd, rc.DependentProperties, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, rc));
        }

        private XElement GenerateReferentialEnd(string endTypeName, AssociationEnd end, IEnumerable<MemberProperty> properties, XNamespace xmlNamespace)
        {
            var propRefs = from p in properties
                           select new XElement(
                                    xmlNamespace + "PropertyRef",
                                    new XAttribute("Name", p.Name));
            return new XElement(
                    xmlNamespace + endTypeName,
                    new XAttribute("Role", end.RoleName),
                    propRefs);
        }

        private XElement GenerateOnDelete(AssociationEnd end, XNamespace xmlNamespace)
        {
            if (end.DeleteBehavior == OperationAction.Cascade)
            {
                return new XElement(
                    xmlNamespace + "OnDelete",
                    new XAttribute("Action", "Cascade"));
            }
            else
            {
                return null;
            }
        }
    }
}
