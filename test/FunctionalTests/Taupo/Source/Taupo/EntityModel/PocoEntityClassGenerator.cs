//---------------------------------------------------------------------
// <copyright file="PocoEntityClassGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Types;

    /// <summary>
    /// Generates POCO entities based on the supplied model
    /// </summary>
    [ImplementationName(typeof(IEntityClassGenerator), "POCO", HelpText = "Generate POCO (persistance-ignorant) classes")]
    public class PocoEntityClassGenerator : IEntityClassGenerator
    {
        private static DataTypeToCodeTypeReferenceResolver codeTypeReferenceResolver = new DataTypeToCodeTypeReferenceResolver();
        private IProgrammingLanguageStrategy language;

        /// <summary>
        /// Initializes a new instance of the PocoEntityClassGenerator class
        /// </summary>
        /// <param name="language">The language.</param>
        public PocoEntityClassGenerator(IProgrammingLanguageStrategy language)
        {
            ExceptionUtilities.CheckArgumentNotNull(language, "language");
            this.language = language;
            this.PocoOption = PocoOption.None;
        }

        /// <summary>
        /// Gets or sets the Poco Option
        /// </summary>
        /// <value>The poco option.</value>
        [InjectTestParameter("PocoOption", DefaultValueDescription = "None", HelpText = "The type of POCO class to be generated. Example: for generating POCO classes with all properties virtual, specify 'AllPropertiesVirtual'")]
        public PocoOption PocoOption { get; set; }

        /// <summary>
        /// Generates POCO entities based on the supplied model
        /// </summary>
        /// <param name="model">Model to generate classes from</param>
        /// <returns>Generated code files</returns>
        public IEnumerable<FileContents<string>> GenerateEntityClasses(EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            PocoAnnotator.Annotate(model, this.PocoOption);
            List<FileContents<string>> results = new List<FileContents<string>>();

            foreach (var ns in model.EntityTypes.Select(e => e.NamespaceName).Concat(model.EnumTypes.Select(e => e.NamespaceName)).Concat(model.ComplexTypes.Select(e => e.NamespaceName)).Distinct())
            {
                var codeUnit = new CodeCompileUnit();
                CodeNamespace codeNamespace = codeUnit.AddNamespace(ns);
                codeNamespace.ImportNamespace("System.Collections.Generic");

                foreach (var type in model.ComplexTypes.Where(e => e.NamespaceName == ns))
                {
                    codeNamespace.Types.Add(this.BuildType(type));
                }

                foreach (var type in model.EntityTypes.Where(e => e.NamespaceName == ns))
                {
                    codeNamespace.Types.Add(this.BuildType(type));
                }

                foreach (var type in model.EnumTypes.Where(e => e.NamespaceName == ns))
                {
                    codeNamespace.Types.Add(this.BuildType(type));
                }

                string code = this.GenerateCodeFromCompileUnit(codeUnit);
                results.Add(new FileContents<string>(ns + this.language.FileExtension, code));
            }

            return results;
        }

        /// <summary>
        /// Method reads all the CodeAttributeAnnotation defined as a part of EntityType and MemberProperty on an EntityModelSchema
        /// and generates appropiate codedom snippet for it.
        /// </summary>
        /// <param name="codeClass">CodeDom class to apply annotation on</param>
        /// <param name="codeAttributeAnnotations">Annotateditem which is to be read and interpreted</param>
        protected virtual void AddCodeAttributeAnnotationAsCustomAttribute(CodeTypeMember codeClass, IEnumerable<CodeAttributeAnnotation> codeAttributeAnnotations)
        {
            foreach (CodeAttributeAnnotation caa in codeAttributeAnnotations)
            {
                codeClass.AddCustomAttribute(caa.TypeOfAttribute, Code.CustomAttributeArguments(caa.Arguments));
            }
        }

        /// <summary>
        /// Adds navigation properties to generated type class.
        /// </summary>
        /// <param name="codeClass">The <see cref="CodeTypeDeclaration"/> to which to add the navigation properties.</param>
        /// <param name="type">The <see cref="EntityType"/> from which to find navigation properties.</param>
        /// <param name="defaultConstructor">The <see cref="CodeConstructor"/>, used to initialize navigation properties to their default values.</param>
        /// <remarks>
        /// Because CodeDom only supports property-level attributes, the get access modifier will be used for the entire property
        /// </remarks>
        protected virtual void AddNavigationProperties(CodeTypeDeclaration codeClass, EntityType type, CodeConstructor defaultConstructor)
        {
            foreach (var prop in type.NavigationProperties)
            {
                string propTypeName = prop.ToAssociationEnd.EntityType.FullName;
                bool isVirtual = prop.Annotations.Any(a => a is VirtualAnnotation);

                CodeTypeReference propType;
                if (prop.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
                {
                    // Instantiate collection in constructor if it's not overrideable
                    if (!isVirtual)
                    {
                        var instantiateType = Code.GenericType("List", propTypeName);
                        defaultConstructor.Statements.Add(Code.This().Property(prop.Name).Assign(Code.New(instantiateType)));
                    }

                    propType = Code.GenericType("ICollection", propTypeName);
                }
                else
                {
                    propType = new CodeTypeReference(propTypeName);
                }

                var codeProp = codeClass.AddAutoImplementedProperty(propType, prop.Name);

                ApplyPropertyAccessModifier(codeProp, prop.Annotations.OfType<PropertyAccessModifierAnnotation>().SingleOrDefault());

                if (prop.Annotations.Any(a => a is CodeAttributeAnnotation))
                {
                    this.AddCodeAttributeAnnotationAsCustomAttribute(codeProp, prop.Annotations.OfType<CodeAttributeAnnotation>());
                }

                if (isVirtual)
                {
                    codeProp.SetVirtual();
                }
            }
        }

        /// <summary>
        /// Adds enum members to the generated type class.
        /// </summary>
        /// <param name="codeEnum">The <see cref="CodeTypeDeclaration"/> to which to add the enum members.</param>
        /// <param name="type">The definition of the enum type.</param>
        protected virtual void AddEnumMembers(CodeTypeDeclaration codeEnum, EnumType type)
        {
            foreach (var member in type.Members)
            {
                var codeMember = new CodeMemberField(codeEnum.Name, member.Name);
                if (member.Value != null)
                {
                    codeMember.InitExpression = new CodeSnippetExpression(member.Value.ToString());
                }

                codeEnum.Members.Add(codeMember);
            }
        }

        /// <summary>
        /// Adds scalar and complex properties to the generated type class.
        /// </summary>
        /// <param name="codeClass">The <see cref="CodeTypeDeclaration"/> to which to add the properties.</param>
        /// <param name="type">The <see cref="StructuralType"/> from which to find properties.</param>
        protected virtual void AddProperties(CodeTypeDeclaration codeClass, StructuralType type)
        {
            foreach (var prop in type.Properties)
            {
                CodeTypeReference propertyType = null;
                var genericPropertyTypeAnnotation = prop.Annotations.OfType<GenericPropertyTypeAnnotation>().FirstOrDefault();
                if (genericPropertyTypeAnnotation != null)
                {
                    propertyType = Code.TypeRef(genericPropertyTypeAnnotation.GenericTypeParameterName);
                }
                else
                {
                    propertyType = codeTypeReferenceResolver.Resolve(prop.PropertyType);
                }

                var codeProp = codeClass.AddAutoImplementedProperty(propertyType, prop.Name);

                ApplyPropertyAccessModifier(codeProp, prop.Annotations.OfType<PropertyAccessModifierAnnotation>().SingleOrDefault());

                if (prop.Annotations.Any(a => a is VirtualAnnotation))
                {
                    codeProp.SetVirtual();
                }

                if (prop.Annotations.Any(a => a is CodeAttributeAnnotation))
                {
                    this.AddCodeAttributeAnnotationAsCustomAttribute(codeProp, prop.Annotations.OfType<CodeAttributeAnnotation>());
                }
            }
        }

        /// <summary>
        /// Builds a code representation of an <see cref="EntityType"/>.
        /// </summary>
        /// <param name="type">The <see cref="EntityType"/> from which to generate code.</param>
        /// <returns>A <see cref="CodeTypeDeclaration"/> which represents the <see cref="EntityType"/>.</returns>
        protected virtual CodeTypeDeclaration BuildType(EntityType type)
        {
            var codeClass = new CodeTypeDeclaration(type.Name);
            var defaultConstructor = codeClass.AddConstructor();

            ApplyTypeAccessModifier(codeClass, type.Annotations.OfType<TypeAccessModifierAnnotation>().SingleOrDefault());

            if (type.Annotations.Any(a => a is SerializableAnnotation))
            {
                codeClass.AddCustomAttribute(typeof(SerializableAttribute));
            }

            if (type.Annotations.Any(a => a is CodeAttributeAnnotation))
            {
                this.AddCodeAttributeAnnotationAsCustomAttribute(codeClass, type.Annotations.OfType<CodeAttributeAnnotation>());
            }

            var genericTypeAnnotation = type.Annotations.OfType<GenericTypeAnnotation>().FirstOrDefault();
            if (genericTypeAnnotation != null)
            {
                foreach (var typeParameter in genericTypeAnnotation.TypeParameters)
                {
                    codeClass.TypeParameters.Add(new CodeTypeParameter(typeParameter));
                }
            }

            if (type.BaseType != null)
            {
                var baseType = new CodeTypeReference(type.BaseType.FullName);

                var baseGenericTypeAnnotation = type.BaseType.Annotations.OfType<GenericTypeAnnotation>().SingleOrDefault();
                var genericArgumentsAnnotation = type.Annotations.OfType<GenericArgumentsAnnotation>().SingleOrDefault();
                if (genericArgumentsAnnotation != null)
                {
                    foreach (var typeParameter in baseGenericTypeAnnotation.TypeParameters)
                    {
                        var typeRef = Code.TypeRef(typeParameter);
                        var argument = genericArgumentsAnnotation.GenericArguments.SingleOrDefault(g => g.TypeParameterName == typeParameter);
                        if (argument != null)
                        {
                            typeRef = codeTypeReferenceResolver.Resolve(argument.DataType);
                        }
                        else
                        {
                            if (genericTypeAnnotation == null || !genericTypeAnnotation.TypeParameters.Contains(typeParameter))
                            {
                                throw new TaupoArgumentException(
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        "Entity type {0} cannot derive from entity type {1} because it does not specify a {2} or {3} to fill in {1}'s generic parameter {4}.",
                                        type.Name,
                                        type.BaseType.Name,
                                        typeof(GenericTypeAnnotation).Name,
                                        typeof(GenericArgumentsAnnotation).Name,
                                        typeParameter));
                            }
                        }

                        baseType.TypeArguments.Add(typeRef);
                    }
                }
                else
                {
                    if (baseGenericTypeAnnotation != null)
                    {
                        throw new TaupoArgumentException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Entity type {0} cannot derive from entity type {1} because it does not specify a {2} or {3} to fill in {1}'s generic parameter {4}.",
                                type.Name,
                                type.BaseType.Name,
                                typeof(GenericTypeAnnotation).Name,
                                typeof(GenericArgumentsAnnotation).Name,
                                baseGenericTypeAnnotation.TypeParameters.First()));
                    }
                }

                codeClass.InheritsFrom(baseType);
            }

            if (type.IsAbstract)
            {
                codeClass.SetAbstract();
            }

            this.AddProperties(codeClass, type);
            this.AddNavigationProperties(codeClass, type, defaultConstructor);

            return codeClass;
        }

        /// <summary>
        /// Builds a code representation of an <see cref="ComplexType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ComplexType"/> from which to generate code.</param>
        /// <returns>A <see cref="CodeTypeDeclaration"/> which represents the <see cref="ComplexType"/>.</returns>
        protected virtual CodeTypeDeclaration BuildType(ComplexType type)
        {
            var codeClass = new CodeTypeDeclaration(type.Name);
            codeClass.AddConstructor();

            ApplyTypeAccessModifier(codeClass, type.Annotations.OfType<TypeAccessModifierAnnotation>().SingleOrDefault());

            if (type.Annotations.Any(a => a is SerializableAnnotation))
            {
                codeClass.AddCustomAttribute(typeof(SerializableAttribute));
            }

            if (type.Annotations.Any(a => a is CodeAttributeAnnotation))
            {
                this.AddCodeAttributeAnnotationAsCustomAttribute(codeClass, type.Annotations.OfType<CodeAttributeAnnotation>());
            }

            this.AddProperties(codeClass, type);
            return codeClass;
        }

        /// <summary>
        /// Builds a code representation of an <see cref="EnumType"/>.
        /// </summary>
        /// <param name="type">The <see cref="EnumType"/> from which to generate code.</param>
        /// <returns>A <see cref="CodeTypeDeclaration"/> which represents the <see cref="EnumType"/>.</returns>
        protected virtual CodeTypeDeclaration BuildType(EnumType type)
        {
            var codeEnum = new CodeTypeDeclaration(type.Name);

            codeEnum.IsEnum = true;

            if (type.UnderlyingType != null)
            {
                codeEnum.BaseTypes.Add(type.UnderlyingType);
            }

            if (type.IsFlags == true)
            {
                codeEnum.AddCustomAttribute(typeof(FlagsAttribute));
            }

            ApplyTypeAccessModifier(codeEnum, type.Annotations.OfType<TypeAccessModifierAnnotation>().SingleOrDefault());

            if (type.Annotations.Any(a => a is SerializableAnnotation))
            {
                codeEnum.AddCustomAttribute(typeof(SerializableAttribute));
            }

            this.AddEnumMembers(codeEnum, type);

            return codeEnum;
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) compilation unit and
        /// outputs it to the specified a string.
        /// </summary>
        /// <param name="codeUnit">A <see cref="CodeCompileUnit"/> to generate code for.</param>
        /// <returns>The generated code.</returns>
        protected virtual string GenerateCodeFromCompileUnit(CodeCompileUnit codeUnit)
        {
            ExtendedCodeGenerator generator = this.language.CreateCodeGenerator();
            StringBuilder sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                var options = new SafeCodeGeneratorOptions()
                {
                    BracingStyle = "C",
                    IndentString = "    ",
                };

                generator.GenerateCodeFromCompileUnit(codeUnit, stringWriter, options);
            }

            return sb.ToString();
        }

        private static void ApplyPropertyAccessModifier(CodeMemberAutoImplementedProperty codeProp, PropertyAccessModifierAnnotation propertyAccessModifierAnnotation)
        {
            if (propertyAccessModifierAnnotation != null)
            {
                var maxVisibility = (AccessModifier)Math.Max(
                    (int)propertyAccessModifierAnnotation.GetterAccessModifier,
                    (int)propertyAccessModifierAnnotation.SetterAccessModifier);

                codeProp.Attributes &= ~MemberAttributes.AccessMask;
                codeProp.Attributes |= ConvertAccessModifierToMemberAttributes(maxVisibility);

                if (maxVisibility != propertyAccessModifierAnnotation.GetterAccessModifier)
                {
                    codeProp.GetAttributes =
                        ConvertAccessModifierToMemberAttributes(propertyAccessModifierAnnotation.GetterAccessModifier);
                }

                if (maxVisibility != propertyAccessModifierAnnotation.SetterAccessModifier)
                {
                    codeProp.SetAttributes =
                        ConvertAccessModifierToMemberAttributes(propertyAccessModifierAnnotation.SetterAccessModifier);
                }
            }
        }

        private static MemberAttributes ConvertAccessModifierToMemberAttributes(AccessModifier accessModifier)
        {
            switch (accessModifier)
            {
                case AccessModifier.Internal:
                    return MemberAttributes.Assembly;

                case AccessModifier.Private:
                    return MemberAttributes.Private;

                case AccessModifier.Protected:
                    return MemberAttributes.Family;

                default:
                    return MemberAttributes.Public;
            }
        }

        private static void ApplyTypeAccessModifier(CodeTypeDeclaration codeClass, TypeAccessModifierAnnotation typeAccessModifierAnnotation)
        {
            // Clear visibility attributes
            codeClass.TypeAttributes &= ~TypeAttributes.VisibilityMask;

            if (typeAccessModifierAnnotation != null && typeAccessModifierAnnotation.TypeAccessModifier == AccessModifier.Internal)
            {
                codeClass.TypeAttributes |= TypeAttributes.NotPublic;
            }
            else
            {
                // Default to public
                codeClass.TypeAttributes |= TypeAttributes.Public;
            }
        }
    }
}
