//---------------------------------------------------------------------
// <copyright file="CsdlContentGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Class to generate Csdl contents from <see cref="EntityModelSchema"/>.
    /// </summary>
    [ImplementationName(typeof(ICsdlContentGenerator), "Default")]
    public class CsdlContentGenerator : XsdlContentGeneratorBase, ICsdlContentGenerator
    {
        private EdmVersion edmVersion;

        /// <summary>
        /// Initializes a new instance of the CsdlContentGenerator class.
        /// </summary>
        public CsdlContentGenerator()
            : base(".csdl")
        {
            this.CsdlDataTypeGenerator = new CsdlDataTypeGenerator(this.NamespaceAliasManager);
        }

        /// <summary>
        /// Gets or sets the csdl data type generator.
        /// </summary>
        public ICsdlDataTypeGenerator CsdlDataTypeGenerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to output the fully qualified name for primitive data type.
        /// </summary>
        public bool OutputFullNameForPrimitiveDataType
        {
            get { return this.CsdlDataTypeGenerator.OutputFullNameForPrimitiveDataType; }
            set { this.CsdlDataTypeGenerator.OutputFullNameForPrimitiveDataType = value; }
        }

        /// <summary>
        /// Generates csdl contents, given a fully resolved <see cref="EntityModelSchema"/>
        /// </summary>
        /// <param name="csdlVersion">The CSDL version.</param>
        /// <param name="model">Input model</param>
        /// <returns>Generated file contents.</returns>
        public IEnumerable<FileContents<XElement>> Generate(EdmVersion csdlVersion, EntityModelSchema model)
        {
            this.edmVersion = csdlVersion;
            return base.Generate(this.DetermineXmlNamespace(csdlVersion), model);
        }

        /// <summary>
        /// Generates additional root-level attributes.
        /// </summary>
        /// <returns>Empty sequence, can be overridded in derived classes</returns>
        protected override IEnumerable<XAttribute> GenerateTopLevelAttributes()
        {
            var attributes = new List<XAttribute>();
            attributes.AddRange(base.GenerateTopLevelAttributes());
            if (this.edmVersion >= EdmVersion.V40)
            {
                // don't know how to force XLinq to use a prefix with a particular name
                attributes.Add(new XAttribute(XName.Get("UseStrongSpatialTypes", EdmConstants.AnnotationNamespace.NamespaceName), "false"));
            }

            return attributes;
        }

        /// <summary>
        /// Generates the top-level namespace declarations.
        /// </summary>
        /// <param name="model">The entity schema model.</param>
        /// <returns>
        /// Sequence of top-level namespace declaration attributes.
        /// </returns>
        protected override IEnumerable<XAttribute> GenerateNamespaceDeclarations(EntityModelSchema model)
        {
            foreach (var declaration in base.GenerateNamespaceDeclarations(model))
            {
                yield return declaration;
            }

            if (this.NeedsCodegenNamespace(model))
            {
                yield return new XAttribute(XNamespace.Xmlns + "cg", EdmConstants.CodegenNamespace.NamespaceName);
            }

            if (this.NeedsAnnotationNamespace(model))
            {
                yield return new XAttribute(XNamespace.Xmlns + "annotation", EdmConstants.AnnotationNamespace.NamespaceName);
            }
        }

        /// <summary>
        /// Generates the <see cref="XAttribute"/> that contains the default value for a <see cref="MemberProperty"/>.
        /// </summary>
        /// <param name="defaultValue">The default value of the <see cref="MemberProperty"/>.</param>
        /// <returns>
        /// The <see cref="XAttribute"/> that contains the default value for a <see cref="MemberProperty"/>.
        /// </returns>
        protected override XAttribute GenerateDefaultValue(object defaultValue)
        {
            if (defaultValue == null)
            {
                return null;
            }

            Type originalType = defaultValue.GetType();
            Type typeToCheck = Nullable.GetUnderlyingType(originalType);

            typeToCheck = typeToCheck ?? originalType;

            string defaultValueText = null;

            if (typeToCheck == typeof(DateTime))
            {
                defaultValueText = ((DateTime)defaultValue).ToString("yyyy-MM-dd HH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            }
            else if (typeToCheck == typeof(DateTimeOffset))
            {
                defaultValueText = ((DateTimeOffset)defaultValue).ToString("yyyy-MM-dd HH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
            }
            else if (typeToCheck == typeof(Guid))
            {
                defaultValueText = ((Guid)defaultValue).ToString("D", CultureInfo.InvariantCulture);
            }
            else if (typeToCheck == typeof(byte[]))
            {
                var value = (byte[])defaultValue;

                if (value.Length == 0)
                {
                    return null;
                }

                StringBuilder sb = new StringBuilder("0x", 2 + (value.Length * 2));

                for (int i = 0; i < value.Length; i++)
                {
                    sb.Append(value[i].ToString("X2", CultureInfo.InvariantCulture));
                }

                defaultValueText = sb.ToString();
            }
            else if (typeToCheck == typeof(TimeSpan))
            {
                defaultValueText = ((TimeSpan)defaultValue).ToString(@"hh\:mm\:ss\.fffffff\Z", CultureInfo.InvariantCulture);
            }
            else
            {
                return base.GenerateDefaultValue(defaultValue);
            }

            return new XAttribute("DefaultValue", defaultValueText);
        }

        /// <summary>
        /// Generates the navigation property.
        /// </summary>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <param name="prop">The Navigation property.</param>
        /// <returns>IEnumerable of XElement</returns>
        protected override XElement GenerateNavigationProperty(XNamespace xmlNamespace, NavigationProperty prop)
        {
            var content = base.GenerateNavigationProperty(xmlNamespace, prop);
            PropertyAccessModifierAnnotation accessModifierAnnotation = prop.Annotations.OfType<PropertyAccessModifierAnnotation>().SingleOrDefault();
            if (accessModifierAnnotation != null)
            {
                content.Add(GeneratePropertyAccessModifier(accessModifierAnnotation));
            }

            return content;
        }

        /// <summary>
        /// Generates 'EntityType' XElement based on <see cref="EntityType"/>.
        /// </summary>
        /// <param name="xmlNamespace">XML namespace to use</param>
        /// <param name="entityType">Entity type</param>
        /// <returns><see cref="XElement"/> representing EntityType.</returns>
        protected override XElement GenerateEntityType(XNamespace xmlNamespace, EntityType entityType)
        {
            var element = base.GenerateEntityType(xmlNamespace, entityType);
            TypeAccessModifierAnnotation annotation = entityType.Annotations.OfType<TypeAccessModifierAnnotation>().SingleOrDefault();
            element.Add(GenerateTypeAccessModifier(annotation));
            return element;
        }

        /// <summary>
        /// Generates EntityContainer element from <see cref="EntityContainer"/>
        /// </summary>
        /// <param name="xmlNamespace">XML namespace to use</param>
        /// <param name="container">Entity container</param>
        /// <returns>'EntityContainer' <see cref="XElement"/></returns>
        protected override XElement GenerateEntityContainer(XNamespace xmlNamespace, EntityContainer container)
        {
            var element = base.GenerateEntityContainer(xmlNamespace, container);
            TypeAccessModifierAnnotation annotation = container.Annotations.OfType<TypeAccessModifierAnnotation>().SingleOrDefault();
            element.Add(GenerateTypeAccessModifier(annotation));
            return element;
        }

        /// <summary>
        /// Generates the complex types.
        /// </summary>
        /// <param name="items">The collection of Complex Types.</param>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <returns>Complex Type XElement</returns>
        protected override IEnumerable<XElement> GenerateComplexTypes(IEnumerable<ComplexType> items, XNamespace xmlNamespace)
        {
            var content = base.GenerateComplexTypes(items, xmlNamespace).ToList();
            foreach (ComplexType ct in items.Where(ct => ct.Annotations.OfType<TypeAccessModifierAnnotation>().Any()))
            {
                TypeAccessModifierAnnotation annotation = ct.Annotations.OfType<TypeAccessModifierAnnotation>().SingleOrDefault();
                content.Single(e => e.Attribute("Name").Value == ct.Name).Add(GenerateTypeAccessModifier(annotation));
            }

            return content;
        }

        /// <summary>
        /// Generates Property element for a given <see cref="MemberProperty"/>
        /// </summary>
        /// <param name="xmlNamespace">XML namespace to use</param>
        /// <param name="prop">Entity or ComplexType property</param>
        /// <returns>Property XElement</returns>
        protected override XElement GenerateProperty(XNamespace xmlNamespace, MemberProperty prop)
        {
            var element = base.GenerateProperty(xmlNamespace, prop);
            PropertyAccessModifierAnnotation accessModifierAnnotation = prop.Annotations.OfType<PropertyAccessModifierAnnotation>().SingleOrDefault();
            if (accessModifierAnnotation != null)
            {
                element.Add(GeneratePropertyAccessModifier(accessModifierAnnotation));
            }

            CollectionKindAnnotation collectionKindAnnotation = prop.Annotations.OfType<CollectionKindAnnotation>().SingleOrDefault();
            if (collectionKindAnnotation != null)
            {
                element.Add(new XAttribute("CollectionKind", collectionKindAnnotation.CollectionKind.ToString()));
            }

            return element;
        }

        /// <summary>
        /// Generates additional description for function
        /// </summary>
        /// <param name="modelFunction">the function</param>
        /// <param name="xmlNamespace">the xml namespace</param>
        /// <returns>some XElements and XAttributes for additional description</returns>
        protected override IEnumerable<XObject> GenerateFunctionAdditionalDescription(Function modelFunction, XNamespace xmlNamespace)
        {
            if (modelFunction.Annotations.OfType<FunctionInModelAnnotation>().Any())
            {
                var annotation = modelFunction.Annotations.OfType<FunctionInModelAnnotation>().Single();
                if (annotation.DefiningExpression != null)
                {
                    return new XObject[] { new XElement(xmlNamespace + "DefiningExpression", annotation.DefiningExpression) };
                }
            }

            return Enumerable.Empty<XObject>();
        }

        /// <summary>
        /// Generates the enum types
        /// </summary>
        /// <param name="enumTypes">the collection of Enum types</param>
        /// <param name="xmlNamespace">the XML namespace</param>
        /// <returns>the Enum type XElements</returns>
        protected override IEnumerable<XElement> GenerateEnumTypes(IEnumerable<EnumType> enumTypes, XNamespace xmlNamespace)
        {
            var content = from e in enumTypes
                          select this.GenerateEnumType(e, xmlNamespace);
            return content;
        }

        /// <summary>
        /// Generates addition items in entity container specific to csdl (e.g. FunctionImport)
        /// </summary>
        /// <param name="container">the container</param>
        /// <param name="xmlNamespace">the xml namespace</param>
        /// <returns>some XElements or XAttributes for items specific to csdl</returns>
        protected override IEnumerable<XObject> GenerateAdditionalEntityContainerItems(EntityContainer container, XNamespace xmlNamespace)
        {
            return this.GenerateFunctionImports(container, xmlNamespace).Cast<XObject>();
        }

        /// <summary>
        /// Determines whether the specified annotation is a special annotation (handled by the generator
        /// rather than custom annotation).
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns>
        /// Value <c>true</c> if the specified annotation is a special annotation; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsSpecialAnnotation(Annotation annotation)
        {
            if (annotation is ConcurrencyTokenAnnotation)
            {
                return true;
            }

            if (annotation is FunctionInModelAnnotation)
            {
                return true;
            }

            if (annotation is PropertyAccessModifierAnnotation)
            {
                return true;
            }

            if (annotation is TypeAccessModifierAnnotation)
            {
                return true;
            }

            if (annotation is MethodAccessModifierAnnotation)
            {
                return true;
            }

            return base.IsSpecialAnnotation(annotation);
        }

        /// <summary>
        /// Generates the concurrency token attribute.
        /// </summary>
        /// <param name="memberProperty">The property.</param>
        /// <returns>Generated attribute</returns>
        protected override XAttribute GenerateStoreGeneratedPattern(MemberProperty memberProperty)
        {
            StoreGeneratedPatternAnnotation annotation = memberProperty.Annotations.OfType<StoreGeneratedPatternAnnotation>().SingleOrDefault();
            if (annotation == null)
            {
                return null;
            }

            return new XAttribute(EdmConstants.AnnotationNamespace.GetName("StoreGeneratedPattern"), annotation.Name);
        }

        /// <summary>
        /// Gets the data type generator.
        /// </summary>
        /// <returns>The data type generator.</returns>
        protected override IXsdlDataTypeGenerator GetDataTypeGenerator()
        {
            return this.CsdlDataTypeGenerator;
        }

        private static XAttribute GenerateTypeAccessModifier(TypeAccessModifierAnnotation annotation)
        {
            if (annotation == null)
            {
                return null;
            }

            AccessModifier typeAccessModifier = annotation.TypeAccessModifier;
            if (typeAccessModifier == AccessModifier.Unspecified)
            {
                return null;
            }

            return new XAttribute(EdmConstants.CodegenNamespace + "TypeAccess", annotation.TypeAccessModifier.ToString());
        }

        private static XAttribute GenerateMethodAccessModifier(MethodAccessModifierAnnotation annotation)
        {
            if (annotation == null)
            {
                return null;
            }

            AccessModifier methodAccessModifier = annotation.MethodAccessModifier;
            if (methodAccessModifier == AccessModifier.Unspecified)
            {
                return null;
            }

            return new XAttribute(EdmConstants.CodegenNamespace + "MethodAccess", methodAccessModifier.ToString());
        }

        private static IEnumerable<XAttribute> GeneratePropertyAccessModifier(PropertyAccessModifierAnnotation annotation)
        {
            List<XAttribute> visibilities = new List<XAttribute>();
            AccessModifier getterVisibility = annotation.GetterAccessModifier;
            AccessModifier setterVisibility = annotation.SetterAccessModifier;

            if (getterVisibility != AccessModifier.Unspecified)
            {
                visibilities.Add(new XAttribute(EdmConstants.CodegenNamespace + "GetterAccess", annotation.GetterAccessModifier.ToString()));
            }

            if (setterVisibility != AccessModifier.Unspecified)
            {
                visibilities.Add(new XAttribute(EdmConstants.CodegenNamespace + "SetterAccess", annotation.SetterAccessModifier.ToString()));
            }

            return visibilities.Count > 0 ? visibilities : null;
        }

        private bool NeedsAnnotationNamespace(EntityModelSchema schema)
        {
            return schema.EntityTypes.Cast<NamedStructuralType>()
                .Concat(schema.ComplexTypes.Cast<NamedStructuralType>())
                .SelectMany(e => e.Properties)
                .Any(p => p.Annotations.OfType<StoreGeneratedPatternAnnotation>().Any());
        }

        private bool NeedsCodegenNamespace(EntityModelSchema model)
        {
            foreach (EntityContainer container in model.EntityContainers)
            {
                if (container.Annotations.OfType<TypeAccessModifierAnnotation>().Any())
                {
                    return true;
                }

                foreach (FunctionImport function in container.FunctionImports)
                {
                    if (function.Annotations.OfType<MethodAccessModifierAnnotation>().Any())
                    {
                        return true;
                    }
                }
            }

            foreach (EntityType entity in model.EntityTypes)
            {
                if (entity.Annotations.OfType<TypeAccessModifierAnnotation>().Any())
                {
                    return true;
                }

                if (entity.Properties.Any(p => p.Annotations.OfType<PropertyAccessModifierAnnotation>().Any()))
                {
                    return true;
                }

                if (entity.AllNavigationProperties.Any(p => p.Annotations.OfType<PropertyAccessModifierAnnotation>().Any()))
                {
                    return true;
                }
            }

            foreach (ComplexType complex in model.ComplexTypes)
            {
                if (complex.Annotations.OfType<TypeAccessModifierAnnotation>().Any())
                {
                    return true;
                }

                if (complex.Properties.Any(p => p.Annotations.OfType<PropertyAccessModifierAnnotation>().Any()))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<XElement> GenerateFunctionImports(EntityContainer container, XNamespace xmlNamespace)
        {
            var content = from f in container.FunctionImports
                          select this.GenerateFunctionImport(xmlNamespace, f);
            return content;
        }

        private XElement GenerateFunctionImport(XNamespace xmlNamespace, FunctionImport functionImport)
        {
            MethodAccessModifierAnnotation annotation = functionImport.Annotations.OfType<MethodAccessModifierAnnotation>().SingleOrDefault();
            EntitySetPathAnnotation entitySetPathAnnotation = functionImport.Annotations.OfType<EntitySetPathAnnotation>().SingleOrDefault();

            return new XElement(
                xmlNamespace + "FunctionImport",
                new XAttribute("Name", functionImport.Name),
                this.GenerateDocumentation(xmlNamespace, functionImport),
                this.CsdlDataTypeGenerator.GenerateReturnTypeForFunctionImport(functionImport.ReturnTypes, xmlNamespace),
                functionImport.IsComposable ? new XAttribute("IsComposable", true) : null,
                functionImport.IsSideEffecting ? null : new XAttribute("IsSideEffecting", false),
                functionImport.IsBindable ? new XAttribute("IsBindable", true) : null,
                entitySetPathAnnotation != null ? new XAttribute("EntitySetPath", entitySetPathAnnotation.EntitySetPath) : null,
                this.GenerateFunctionImportParameters(functionImport.Parameters, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, functionImport),
                GenerateMethodAccessModifier(annotation));
        }

        private IEnumerable<XElement> GenerateFunctionImportParameters(IEnumerable<FunctionParameter> parameters, XNamespace xmlNamespace)
        {
            var content = from parameter in parameters
                          select new XElement(
                                xmlNamespace + "Parameter",
                                new XAttribute("Name", parameter.Name),
                                this.GenerateDocumentation(xmlNamespace, parameter),
                                this.CsdlDataTypeGenerator.GenerateParameterTypeForFunctionImport(parameter, xmlNamespace),
                                this.GenerateAnnotations(xmlNamespace, parameter));
            return content;
        }

        private XElement GenerateEnumType(EnumType enumType, XNamespace xmlNamespace)
        {
            return new XElement(
                xmlNamespace + "EnumType",
                new XAttribute("Name", enumType.Name),
                enumType.IsFlags == null ? null : new XAttribute("IsFlags", enumType.IsFlags),
                enumType.UnderlyingType == null ? null : new XAttribute("UnderlyingType", this.GetEdmNameForUnderlyingType(enumType.UnderlyingType)),
                this.GenerateDocumentation(xmlNamespace, enumType),
                this.GenerateEnumMembers(enumType.Members, xmlNamespace),
                this.GenerateAnnotations(xmlNamespace, enumType));
        }

        private string GetEdmNameForUnderlyingType(Type underlyingType)
        {
            var edmPrimitiveDataType = EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.Latest)
                                                   .SingleOrDefault(t => t.HasFacet<PrimitiveClrTypeFacet>() && t.GetFacet<PrimitiveClrTypeFacet>().Value == underlyingType);
            if (edmPrimitiveDataType != null)
            {
                if (this.OutputFullNameForPrimitiveDataType)
                {
                    return EdmDataTypes.GetEdmFullName(edmPrimitiveDataType);
                }

                return EdmDataTypes.GetEdmName(edmPrimitiveDataType);
            }

            return underlyingType.Name;
        }

        private IEnumerable<XElement> GenerateEnumMembers(IEnumerable<EnumMember> members, XNamespace xmlNamespace)
        {
            var content = from m in members
                          select new XElement(
                              xmlNamespace + "Member",
                              this.GenerateDocumentation(xmlNamespace, m),
                              new XAttribute("Name", m.Name),
                              m.Value == null ? null : new XAttribute("Value", m.Value.ToString()),
                              this.GenerateAnnotations(xmlNamespace, m));
            return content;
        }

        /// <summary>
        /// Determines XML namespace to be used.
        /// </summary>
        /// <param name="csdlVersion">The CSDL version.</param>
        /// <returns>
        /// CSDL namespace based on specified csdl version.
        /// </returns>
        private XNamespace DetermineXmlNamespace(EdmVersion csdlVersion)
        {
            if (csdlVersion == EdmVersion.V40)
            {
                return EdmConstants.CsdlOasisNamespace;
            }
            else
            {
                throw new TaupoNotSupportedException("CSDL Schema Version is not supported: " + csdlVersion.ToString());
            }
        }
    }
}
