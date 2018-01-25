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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>
    [TestClass]
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

        [TestMethod()]
        public void AssertEmptyVocabularyAnnotations()
        {
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                Assert.IsFalse(clientEdmModel.VocabularyAnnotations.Any(), "No vocabulary annotations should be defined in the client edm model for Protocol Version : {0}", dataServiceProtocolVersion.ToString());
            }
        }

        [TestMethod()]
        public void AssertNullablePrimitiveTypeReferencesAreNullable()
        {
            bool makeNullable = true;
            string errorMessage = "Primitive type references should be nullable";
            AssertPrimitiveTypeReferenceGeneration(makeNullable, errorMessage);
        }

        [TestMethod()]
        public void AssertNonNullablePrimitiveTypeReferencesAreNonNullable()
        {
            bool makeNullable = false;
            string errorMessage = "Primitive type references should not be nullable";
            AssertPrimitiveTypeReferenceGeneration(makeNullable, errorMessage);
        }

        [TestMethod]
        public void GenericEntityTypesShouldHaveUniqueEdmTypeNames()
        {
            // Regression test: [Client-ODataLib-Integration] In Client, while generating edmType for generic types, we do not take generic arguments in account
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                IEdmEntityType stringPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<string>));
                IEdmEntityType integerPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<int>));

                Assert.AreNotEqual(GetFullName(stringPropertyGenericArgType), GetFullName(integerPropertyGenericArgType), "generic types should have unique names based on generic args");
            }
        }

        [TestMethod]
        public void GenericEntityTypesShouldHaveUniqueEdmTypeNames_2()
        {
            // Regression test: [Client-ODataLib-Integration] In Client, while generating edmType for generic types, we do not take generic arguments in account
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                IEdmEntityType stringPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<GenericEntityType<string>>));
                IEdmEntityType integerPropertyGenericArgType = (IEdmEntityType)clientEdmModel.GetOrCreateEdmType(typeof(GenericEntityType<GenericEntityType<int>>));

                Assert.AreNotEqual(GetFullName(stringPropertyGenericArgType), GetFullName(integerPropertyGenericArgType), "generic types should have unique names based on generic args");
            }
        }

        [TestMethod]
        public void GenericComplexTypesShouldHaveUniqueEdmTypeNames()
        {
            // Regression test: [Client-ODataLib-Integration] In Client, while generating edmType for generic types, we do not take generic arguments in account
            foreach (var dataServiceProtocolVersion in new[] { ODataProtocolVersion.V4 })
            {
                ClientEdmModel clientEdmModel = new ClientEdmModel(dataServiceProtocolVersion);
                IEdmComplexType stringPropertyGenericArgType = (IEdmComplexType)clientEdmModel.GetOrCreateEdmType(typeof(GenericComplexType<string, int>));
                IEdmComplexType integerPropertyGenericArgType = (IEdmComplexType)clientEdmModel.GetOrCreateEdmType(typeof(GenericComplexType<int, string>));

                Assert.AreNotEqual(GetFullName(stringPropertyGenericArgType), GetFullName(integerPropertyGenericArgType), "generic types should have unique names based on generic args");
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
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

            Assert.AreNotSame(entityType1, entityType2);
            Assert.AreNotSame(entityType2, entityType3);
            Assert.AreEqual(entityType1.ToString(), entityType2.ToString());
            Assert.AreNotEqual(entityType1.AssemblyQualifiedName, entityType2.AssemblyQualifiedName);
            Assert.AreEqual(entityType2.AssemblyQualifiedName, entityType3.AssemblyQualifiedName);

            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);

            var edmEntityType1 = clientModel.GetOrCreateEdmType(entityType1);
            var edmEntityType2 = clientModel.GetOrCreateEdmType(entityType2);

            Action test = () => clientModel.GetOrCreateEdmType(entityType3);
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_MultipleTypesWithSameName(entityType3.AssemblyQualifiedName));

            Assert.AreSame(edmEntityType1, clientModel.FindDeclaredType(entityType1.ToString()));
            Assert.AreSame(edmEntityType2, clientModel.FindDeclaredType(entityType2.AssemblyQualifiedName));

            var edmComplexType1 = clientModel.GetOrCreateEdmType(complexType1);
            var edmComplexType2 = clientModel.GetOrCreateEdmType(complexType2);

            test = () => clientModel.GetOrCreateEdmType(complexType3);
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_MultipleTypesWithSameName(complexType3.AssemblyQualifiedName));

            Assert.AreSame(edmComplexType1, clientModel.FindDeclaredType(complexType1.ToString()));
            Assert.AreSame(edmComplexType2, clientModel.FindDeclaredType(complexType2.AssemblyQualifiedName));
        }
#endif

        [TestMethod]
        public void GetOrCreateEdmTypeShouldThrowIfTypeIsObject()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            Action test = () => clientModel.GetOrCreateEdmType(typeof(object));
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_NoSettableFields("System.Object"));
        }

        [TestMethod]
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

        [TestMethod]
        public void GetOrCreateEdmTypeShouldThrowIfTypeHasObjectProperty()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            IEdmComplexType complexType = (IEdmComplexType)clientModel.GetOrCreateEdmType(typeof(TypeWithObjectProperty));
            Action test = () => complexType.DeclaredProperties.Count();
            test.ShouldThrow<InvalidOperationException>().WithMessage(Strings.ClientType_NoSettableFields("System.Object"));
        }

        [TestMethod]
        public void GetOrCreateEdmTypeShouldWorkForEnumTypes()
        {
            var clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var enumType = clientModel.GetOrCreateEdmType(typeof(EdmEnumType));
            Assert.IsNotNull(enumType);
            Assert.IsNotNull(enumType.FullName());
        }

        private class TypeWithCollectionOfObjectProperty
        {
            public ICollection<object> Objects { get; set; }
        }

        [TestMethod]
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

        [TestMethod]
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
                    Assert.AreEqual(primitiveEdmType.Namespace, "Edm", "Namespace was incorrect for clr type :{0}", nullableType.FullName);
                    Assert.AreEqual(GetFullName(primitiveEdmType), primitivePropertyType.Key, "FullName was incorrect for clr type :{0}", nullableType.FullName);
                    Assert.AreEqual(primitiveEdmType.TypeKind, EdmTypeKind.Primitive, "Type kind was incorrect for clr type :{0}", nullableType.FullName);

                    IEdmTypeReference primitiveTypeReference = primitiveEdmType.ToEdmTypeReference(makeNullable);
                    Assert.AreEqual(primitiveTypeReference.IsNullable, makeNullable, errorMessage);
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
