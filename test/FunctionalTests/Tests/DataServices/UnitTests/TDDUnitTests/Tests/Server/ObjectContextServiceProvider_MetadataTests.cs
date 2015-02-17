//---------------------------------------------------------------------
// <copyright file="ObjectContextServiceProvider_MetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Caching;
using Microsoft.OData.Service.Providers;
using System.Linq;
using System.Reflection;
using AstoriaUnitTests.Tests.Server.Simulators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class ObjectContextServiceProvider_MetadataTests
    {

        #region Test Cases

        [TestMethod]
        public void VerifyPopulateMemberMetadataReturnsValidResourceProperties()
        {
            Type entityClrType = typeof(TestEntityType);
            Type[] primitiveTypes = entityClrType.GetProperties().Select(p => p.PropertyType).ToArray();

            PrimitiveResourceTypeMap typeMap;
            ResourceType entityResourceType;
            ProviderMetadataSimulator workspace;
            SetupTestMetadata(entityClrType, primitiveTypes, out entityResourceType, out workspace, out typeMap);
            
            ObjectContextServiceProvider.PopulateMemberMetadata(new ResourceTypeCacheItem(entityResourceType), workspace, null, typeMap);
            VerifyResourceProperties(entityClrType, entityResourceType);
        }

        [TestMethod]
        public void VerifyPopulateMemberMetadataThrowsWhenUnRecognizedPropertyExists()
        {
            Type entityClrType = typeof(TestEntityTypeWithInteger);
            Type[] primitiveTypes = new Type[] { typeof(string) };

            PrimitiveResourceTypeMap typeMap;
            ResourceType entityResourceType;
            ProviderMetadataSimulator workspace;
            SetupTestMetadata(entityClrType, primitiveTypes, out entityResourceType, out workspace, out typeMap);
            Action test = () => ObjectContextServiceProvider.PopulateMemberMetadata(new ResourceTypeCacheItem(entityResourceType), workspace, null, typeMap);
            test.ShouldThrow<NotSupportedException>().WithMessage(Strings.ObjectContext_PrimitiveTypeNotSupported("IntProperty", entityClrType.Name, ProviderMemberSimulator.GetEdmTypeName(typeof(int))));
        }

        [TestMethod]
        public void VerifyPopulateMemberMetadataFailsOnEnumPropertyOnEntityType()
        {
            Type entityClrType = typeof(TestEntityTypeWithEnum);
            Type[] primitiveTypes = new Type[] { typeof(string) };

            PrimitiveResourceTypeMap typeMap;
            ResourceType entityResourceType;
            ProviderMetadataSimulator workspace;
            SetupTestMetadata(entityClrType, primitiveTypes, out entityResourceType, out workspace, out typeMap);
            Action test = () => ObjectContextServiceProvider.PopulateMemberMetadata(new ResourceTypeCacheItem(entityResourceType), workspace, null, typeMap);
            test.ShouldThrow<NotSupportedException>().WithMessage(Strings.ObjectContext_PrimitiveTypeNotSupported("EnumProperty", entityClrType.Name, ProviderMemberSimulator.GetEdmTypeName(typeof(Enum))));
        }

        #endregion

        #region Setup Methods

        private static void SetupTestMetadata(Type entityClrType, Type[] primitiveTypes, out ResourceType entityResourceType, out ProviderMetadataSimulator workspace, out PrimitiveResourceTypeMap primitiveTypeMap)
        {
            KeyValuePair<Type, string>[] mappedPrimitiveTypes = BuildTypeMap(primitiveTypes);
            primitiveTypeMap = new PrimitiveResourceTypeMap(mappedPrimitiveTypes.ToArray());
            entityResourceType = new ResourceType(entityClrType, ResourceTypeKind.EntityType, null, entityClrType.Namespace, entityClrType.Name, false);
            workspace = new ProviderMetadataSimulator(new List<Type>() { entityClrType });
        }

        private static KeyValuePair<Type, string>[] BuildTypeMap(Type[] types)
        {
            Assert.IsTrue(types != null && types.Length > 0, "Expected a non-null and non-empty set of types to build the type map for the test.");

            KeyValuePair<Type, string>[] mappedPrimitiveTypes = new KeyValuePair<Type, string>[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                mappedPrimitiveTypes[i] = new KeyValuePair<Type, string>(types[i], ProviderMemberSimulator.GetEdmTypeName(types[i]));
            }
            return mappedPrimitiveTypes;
        }

        #endregion

        #region Verification

        private static void VerifyResourceProperties(Type entityClrType, ResourceType entityResourceType)
        {
            PropertyInfo[] expectedProperties = entityClrType.GetProperties();
            ReadOnlyCollection<ResourceProperty> actualProperties = entityResourceType.Properties;

            Assert.AreEqual(expectedProperties.Length, actualProperties.Count, "Expected resource type to have the same number of properties as the CLR type provided by the test.");
            foreach (PropertyInfo expectedProperty in expectedProperties)
            {
                string expectedPropertyName = expectedProperty.Name;

                ResourceProperty actualResourceProperty = GetResourceProperty(actualProperties, expectedPropertyName);
                VerifyResourceProperty(expectedPropertyName, expectedProperty.PropertyType, actualResourceProperty);
            }
        }

        private static ResourceProperty GetResourceProperty(ReadOnlyCollection<ResourceProperty> actualProperties, string expectedPropertyName)
        {
            ResourceProperty resourceProperty = actualProperties.SingleOrDefault(p => p.Name == expectedPropertyName);
            Assert.IsNotNull(resourceProperty, "ResourceType does not contain the expected property {0}.", expectedPropertyName);
            return resourceProperty;
        }

        private static void VerifyResourceProperty(string expectedPropertyName, Type expectedPropertyType, ResourceProperty actualResourceProperty)
        {
            ResourcePropertyKind expectedPropertyKind = ResourcePropertyKind.Primitive;
            if (expectedPropertyName == ProviderMemberSimulator.KeyPropertyName)
            {
                expectedPropertyKind |= ResourcePropertyKind.Key;
            }

            string expectedMimeType = ProviderMemberSimulator.IsMimeType(expectedPropertyName) ? ProviderMemberSimulator.SimulatedMimeType : null;

            Assert.AreEqual(expectedPropertyKind, actualResourceProperty.Kind, "Expected ResourceProperty {0} to be a primitive.", expectedPropertyName);
            Assert.AreEqual(expectedPropertyType, actualResourceProperty.Type, "ResourceProperty.Type is not the expected type.");
            Assert.AreEqual(expectedMimeType, actualResourceProperty.MimeType, "Expected ResourceProperty {0} to have the simulated MimeType.", expectedPropertyName);
        }

        #endregion

        #region Test Types

        public class TestEntityType
        {
            public int KeyProperty { get; set; }
            public int IntProperty { get; set; }            
            public int? NullableIntProperty { get; set; }
            public string StringProperty { get; set; }
            public string StringPropertyWithMimeType { get; set; }
            public PrimitiveResourceTypeMapTests.TestPrimitiveType TestPrimitiveProperty { get; set; }
        }

        public class TestEntityTypeWithInteger
        {
            public int IntProperty { get; set; }
        }

        public enum TestEnum
        {
            A,
            B
        };

        public class TestEntityTypeWithEnum
        {
            public TestEnum EnumProperty { get; set; }
        }

        #endregion
    }
}
