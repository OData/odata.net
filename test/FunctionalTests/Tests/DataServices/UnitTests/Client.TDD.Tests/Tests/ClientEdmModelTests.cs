//---------------------------------------------------------------------
// <copyright file="ClientEdmModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
    using System.CodeDom.Compiler;
#endif
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
    using Microsoft.CSharp;
#endif
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using Xunit;

    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>
    public class ClientEdmModelTests
    {
        private IDictionary<string, Type> clrTypeToEdmTypeMapping =
            new Dictionary<string, Type>()
            {
                {"Edm.Boolean", typeof(Boolean)}, 
                {"Edm.Byte", typeof(Byte)},
                {"Edm.Date", typeof(Date)},
                {"Edm.DateTimeOffset", typeof(DateTimeOffset)},
                {"Edm.Decimal", typeof(Decimal)},
                {"Edm.Double", typeof(Double)},
                {"Edm.Guid", typeof(Guid)}, 
                {"Edm.Int16", typeof(Int16)},
                {"Edm.Int32", typeof(Int32)}, 
                {"Edm.Int64", typeof(Int64)}, 
                {"Edm.Single", typeof(Single)},
                {"Edm.SByte", typeof(SByte)}, 
                {"Edm.TimeOfDay", typeof(TimeOfDay)},
            };

        [Fact]
        public void AssertEmptyVocabularyAnnotations()
        {
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                Assert.False(clientEdmModel.VocabularyAnnotations.Any());
            }
        }

        [Fact]
        public void AssertNullablePrimitiveTypeReferencesAreNullable()
        {
            bool makeNullable = true;
            string errorMessage = "Primitive type references should be nullable";
            AssertPrimitiveTypeReferenceGeneration(makeNullable, errorMessage);
        }

        [Fact]
        public void AssertNonNullablePrimitiveTypeReferencesAreNonNullable()
        {
            bool makeNullable = false;
            string errorMessage = "Primitive type references should not be nullable";
            AssertPrimitiveTypeReferenceGeneration(makeNullable, errorMessage);
        }

        [Fact]
        public void GenericEntityTypesShouldHaveUniqueEdmTypeNames()
        {
            // Regression test: [Client-ODataLib-Integration] In Client, while generating edmType for generic types, we do not take generic arguments in account
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                IEdmEntityType stringPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<string>));
                IEdmEntityType integerPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<int>));

                Assert.NotEqual(GetFullName(stringPropertyGenericArgType), GetFullName(integerPropertyGenericArgType));
            }
        }

        [Fact]
        public void GenericEntityTypesShouldHaveUniqueEdmTypeNames_2()
        {
            // Regression test: [Client-ODataLib-Integration] In Client, while generating edmType for generic types, we do not take generic arguments in account
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                IEdmEntityType stringPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<GenericEntityType<string>>));
                IEdmEntityType integerPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<GenericEntityType<int>>));

                Assert.NotEqual(GetFullName(stringPropertyGenericArgType), GetFullName(integerPropertyGenericArgType));
            }
        }

        [Fact]
        public void GenericComplexTypesShouldHaveUniqueEdmTypeNames()
        {
            // Regression test: [Client-ODataLib-Integration] In Client, while generating edmType for generic types, we do not take generic arguments in account
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                IEdmComplexType stringPropertyGenericArgType = (IEdmComplexType)clientEdmModel.GetOrCreateEdmType(typeof(GenericComplexType<string, int>));
                IEdmComplexType integerPropertyGenericArgType = (IEdmComplexType)clientEdmModel.GetOrCreateEdmType(typeof(GenericComplexType<int, string>));

                Assert.NotEqual(GetFullName(stringPropertyGenericArgType), GetFullName(integerPropertyGenericArgType));
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [Fact]
        public void ClientEdmModel_GetOrCreateEdmType_SupportsTypesWithTheSameNamesFromDifferentAssemblies()
        {
            var provider = new CSharpCodeProvider();
            var compilerOptions1 = this.CreateCompilerOptions();
            var compilerOptions2 = this.CreateCompilerOptions();

            string sourceCode = "namespace Namespace1 { public class EntityType { public int ID {get;set;} } public class ComplexType { public int Info {get;set;} } }";
            var compilerResults1 = provider.CompileAssemblyFromSource(compilerOptions1, sourceCode);
            var compilerResults2 = provider.CompileAssemblyFromSource(compilerOptions2, sourceCode);
            var compilerResults3 = provider.CompileAssemblyFromSource(compilerOptions2, sourceCode);

            var entityType1 = compilerResults1.CompiledAssembly.GetType("Namespace1.EntityType", true);
            var entityType2 = compilerResults2.CompiledAssembly.GetType("Namespace1.EntityType", true);
            var entityType3 = compilerResults3.CompiledAssembly.GetType("Namespace1.EntityType", true);

            var complexType1 = compilerResults1.CompiledAssembly.GetType("Namespace1.ComplexType", true);
            var complexType2 = compilerResults2.CompiledAssembly.GetType("Namespace1.ComplexType", true);
            var complexType3 = compilerResults3.CompiledAssembly.GetType("Namespace1.ComplexType", true);

            Assert.NotSame(entityType1, entityType2);
            Assert.NotSame(entityType2, entityType3);
            Assert.Equal(entityType1.ToString(), entityType2.ToString());
            Assert.NotEqual(entityType1.AssemblyQualifiedName, entityType2.AssemblyQualifiedName);
            Assert.Equal(entityType2.AssemblyQualifiedName, entityType3.AssemblyQualifiedName);

            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);

            var edmEntityType1 = clientModel.GetOrCreateEdmType(entityType1);
            var edmEntityType2 = clientModel.GetOrCreateEdmType(entityType2);

            Action test = () => clientModel.GetOrCreateEdmType(entityType3);
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_MultipleTypesWithSameName(entityType3.AssemblyQualifiedName));

            Assert.Same(edmEntityType1, clientModel.FindDeclaredType(entityType1.ToString()));
            Assert.Same(edmEntityType2, clientModel.FindDeclaredType(entityType2.AssemblyQualifiedName));

            var edmComplexType1 = clientModel.GetOrCreateEdmType(complexType1);
            var edmComplexType2 = clientModel.GetOrCreateEdmType(complexType2);

            test = () => clientModel.GetOrCreateEdmType(complexType3);
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_MultipleTypesWithSameName(complexType3.AssemblyQualifiedName));

            Assert.Same(edmComplexType1, clientModel.FindDeclaredType(complexType1.ToString()));
            Assert.Same(edmComplexType2, clientModel.FindDeclaredType(complexType2.AssemblyQualifiedName));
        }
#endif

        [Fact]
        public void GetOrCreateEdmTypeShouldThrowIfTypeIsObject()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            Action test = () => clientModel.GetOrCreateEdmType(typeof(object));
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_NoSettableFields("System.Object"));
        }

        [Fact]
        public void GetOrCreateEdmTypeShouldThrowIfTypeIsIEnumerableOfT()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            Action test = () => clientModel.GetOrCreateEdmType(typeof(IEnumerable<object>));
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_NoSettableFields("System.Collections.Generic.IEnumerable`1[System.Object]"));
        }

        private class TypeWithObjectProperty
        {
            public object Obj { get; set; }
        }

        [Fact]
        public void GetOrCreateEdmTypeShouldThrowIfTypeHasObjectProperty()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            IEdmComplexType complexType = (IEdmComplexType)clientModel.GetOrCreateEdmType(typeof(TypeWithObjectProperty));
            Action test = () => complexType.DeclaredProperties.Count();
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_NoSettableFields("System.Object"));
        }

        [Fact]
        public void GetOrCreateEdmTypeShouldWorkForEnumTypes()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var enumType = clientModel.GetOrCreateEdmType(typeof(EdmEnumType));
            Assert.NotNull(enumType);
            Assert.NotNull(enumType.FullName());
        }

        private class TypeWithCollectionOfObjectProperty
        {
            public ICollection<object> Objects { get; set; }
        }

        [Fact]
        public void GetOrCreateEdmTypeShouldThrowIfTypeContainsAPropertyOfCollectionOfObjects()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            IEdmComplexType complexType = (IEdmComplexType)clientModel.GetOrCreateEdmType(typeof(TypeWithCollectionOfObjectProperty));
            Action test = () => complexType.DeclaredProperties.Count();
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_NoSettableFields("System.Object"));
        }

        private class TypeWithIEnumerableProperty
        {
            public IEnumerable<int> IE { get; set; }
        }

        [Fact]
        public void GetOrCreateEdmTypeShouldThrowIfTypeContainsAPropertyOfIEnumerableType()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            IEdmComplexType complexType = (IEdmComplexType)clientModel.GetOrCreateEdmType(typeof(TypeWithIEnumerableProperty));
            Action test = () => complexType.DeclaredProperties.Count();
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_NoSettableFields("System.Collections.Generic.IEnumerable`1[System.Int32]"));
        }

        private void AssertPrimitiveTypeReferenceGeneration(bool makeNullable, string errorMessage)
        {
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                foreach (var primitivePropertyType in clrTypeToEdmTypeMapping)
                {
                    Type nullableType = primitivePropertyType.Value;
                    if (makeNullable)
                    {
                        nullableType = typeof(Nullable<>).MakeGenericType(primitivePropertyType.Value);
                    }

                    var primitiveEdmType = clientEdmModel.GetOrCreateEdmType(nullableType) as IEdmPrimitiveType;
                    Assert.Equal("Edm", primitiveEdmType.Namespace);
                    Assert.Equal(GetFullName(primitiveEdmType), primitivePropertyType.Key);
                    Assert.Equal(EdmTypeKind.Primitive, primitiveEdmType.TypeKind);

                    IEdmTypeReference primitiveTypeReference = primitiveEdmType.ToEdmTypeReference(makeNullable);
                    Assert.Equal(primitiveTypeReference.IsNullable, makeNullable);
                }
            }
        }

        private static string GetFullName(IEdmSchemaElement schemaElement)
        {
            return schemaElement.FullName();
        }

        public class GenericEntityType<TProperty>
        {
            public int ID { get; set; }

            public TProperty Property { get; set; }
        }

        public class GenericComplexType<TProperty1, TProperty2>
        {
            public TProperty1 Property1 { get; set; }

            public TProperty2 Property2 { get; set; }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        private CompilerParameters CreateCompilerOptions()
        {
            var options = new System.CodeDom.Compiler.CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                TreatWarningsAsErrors = true,
                WarningLevel = 4,
            };

            return options;
        }
#endif
    }
}
