//---------------------------------------------------------------------
// <copyright file="ODataEdmStructuredValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.Test.Taupo.Common;
using Microsoft.Test.Taupo.Execution;
using Microsoft.Test.Taupo.OData.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    /// <summary>
    /// Tests for the ODataEdmStructuredValue object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataEdmStructuredValueTests : ODataTestCase
    {
        // TODO: add tests for invalid property values
        // TODO: add tests for undeclared properties on open types
        // TODO: add tests for undeclared properties on non-open types
        // TODO: [JsonLight] Add support for spatial values in ODataEdmStructuredValue
        //       Add tests for spatial values 

        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        private static readonly ODataComplexValue simpleComplexValue = new ODataComplexValue
        {
            Properties = new[]
            {
                new ODataProperty { Name = "IntProp", Value = 1 },
                new ODataProperty { Name = "StringProp", Value = "One" }
            }
        };
        private static readonly ODataComplexValue nestedComplexValue = new ODataComplexValue
        {
            Properties = new[]
            {
                new ODataProperty { Name = "IntProp", Value = 1 },
                new ODataProperty { Name = "StringProp", Value = "One" },
                new ODataProperty { Name = "ComplexProp", Value = simpleComplexValue  },
            }
        };

        private static readonly ODataCollectionValue primitiveCollectionValue = new ODataCollectionValue { Items = new object[] { 1, 2, 3, 4 } };
        private static readonly ODataCollectionValue complexCollectionValue = new ODataCollectionValue { Items = new object[] { simpleComplexValue, simpleComplexValue, nestedComplexValue, nestedComplexValue } };

        private static readonly ODataProperty[] singlePrimitiveProperty = new[] { new ODataProperty { Name = "Int32Prop", Value = 1 } };
        private static readonly ODataProperty[] singleNullProperty = new[] { new ODataProperty { Name = "Int32Prop", Value = null } };
        private static readonly ODataProperty[] allPrimitiveProperties = new[] 
        { 
            new ODataProperty { Name = "BoolProp", Value = true },
            new ODataProperty { Name = "Int16Prop", Value = (Int16)1 },
            new ODataProperty { Name = "Int32Prop", Value = (Int32)2 },
            new ODataProperty { Name = "Int64Prop", Value = (Int64)3 },
            new ODataProperty { Name = "ByteProp", Value = (Byte)4 },
            new ODataProperty { Name = "SByteProp", Value = (SByte)5 },
            new ODataProperty { Name = "SingleProp", Value = (Single)0.1 },
            new ODataProperty { Name = "DoubleProp", Value = (Double)0.2 },
            new ODataProperty { Name = "DecimalProp", Value = (Decimal)0.3 },
            new ODataProperty { Name = "DateTimeOffsetProp", Value = new DateTimeOffset(2012, 2, 15, 10, 10, 10, new TimeSpan(1, 1, 0)) },
            new ODataProperty { Name = "TimeProp", Value = new TimeSpan(1, 1, 1) },
            new ODataProperty { Name = "GuidProp", Value = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee") },
            new ODataProperty { Name = "StringProp", Value = "SomeValue" },
            new ODataProperty { Name = "BinaryProp", Value = new byte[] { 0, 1, 2, 3 } },
        };
        private static readonly ODataProperty[] complexProperty = new[] 
        { 
            new ODataProperty { Name = "ComplexProp", Value = simpleComplexValue }
        };
        private static readonly ODataProperty[] nestedComplexProperty = new[] 
        { 
            new ODataProperty { Name = "ComplexProp", Value = nestedComplexValue } 
        };
        private static readonly ODataProperty[] primitiveCollectionProperty = new[] 
        { 
            new ODataProperty { Name = "PrimitiveCollectionProp", Value = primitiveCollectionValue } 
        };
        private static readonly ODataProperty[] complexCollectionProperty = new[] 
        { 
            new ODataProperty { Name = "ComplexCollectionProp", Value = complexCollectionValue }
        };
        private static readonly ODataProperty[] differentPropertyKinds = new[] 
        { 
            new ODataProperty { Name = "ComplexProp", Value = simpleComplexValue  },
            new ODataProperty { Name = "PrimitiveCollectionProp", Value = primitiveCollectionValue },
            new ODataProperty { Name = "Int32Prop", Value = 1 },
            new ODataProperty { Name = "ComplexCollectionProp", Value = complexCollectionValue },
        };

#if !SILVERLIGHT && !WINDOWS_PHONE
        // These tests use private reflection and thus cannot run on Silverlight or the phone.
        [TestMethod, Variation(Description = "Test the IEdmValue implementation for an ODataResource.")]
        public void ODataEntryEdmValueTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildEdmValueModel();

            ODataResource[] entries = new ODataResource[]
            {
                // Entry with a single primitive property
                new ODataResource { TypeName = "TestModel.SinglePrimitivePropertyEntityType", Properties = singlePrimitiveProperty },

                // Entry with a single null property
                new ODataResource { TypeName = "TestModel.SinglePrimitivePropertyEntityType", Properties = singleNullProperty },

                // Entry with all primitive typed properties
                new ODataResource { TypeName = "TestModel.AllPrimitivePropertiesEntityType", Properties = allPrimitiveProperties },

                // Entry with complex property
                new ODataResource { TypeName = "TestModel.SingleComplexPropertyEntityType", Properties = complexProperty },

                // Entry with nested complex property
                new ODataResource { TypeName = "TestModel.SingleComplexPropertyEntityType", Properties = nestedComplexProperty },

                // Entry with primitive collection property
                new ODataResource { TypeName = "TestModel.SinglePrimitiveCollectionPropertyEntityType", Properties = primitiveCollectionProperty },

                // Entry with complex collection property
                new ODataResource { TypeName = "TestModel.SingleComplexCollectionPropertyEntityType", Properties = complexCollectionProperty },

                // Entry with different kinds of properties
                new ODataResource { TypeName = "TestModel.DifferentPropertyKindsEntityType", Properties = differentPropertyKinds },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                entries,
                new bool[] { true, false},
                (entry, includeTypeReferences) =>
                {
                    IEdmEntityTypeReference typeReference = null;
                    IEdmEntitySet entitySet = null;
                    if (includeTypeReferences)
                    {
                        IEdmEntityType schemaType = (IEdmEntityType)model.FindType(entry.TypeName);
                        this.Assert.IsNotNull(schemaType, "Expected to find type in the model.");
                        typeReference = (IEdmEntityTypeReference)schemaType.ToTypeReference( /*nullable*/ false);

                        IEdmEntityContainer container = model.EntityContainer;
                        if (container != null)
                        {
                            entitySet = container.FindEntitySet(schemaType.Name + "Set");
                        }
                    }

                    IEdmValue entryEdmValue = ODataEdmValueUtils.CreateStructuredEdmValue(entry, entitySet, typeReference);
                    ODataEdmValueUtils.CompareValue(entryEdmValue, entry, this.Assert);
                });
        }

        [TestMethod, Variation(Description = "Test the IEdmValue implementation for an ODataComplexValue.")]
        public void ODataComplexValueEdmValueTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildEdmValueModel();

            ODataComplexValue[] complexValues = new ODataComplexValue[]
            {
                // Empty complex value
                new ODataComplexValue() { TypeName = "TestModel.EmptyComplexType" },

                // Complex value with a single primitive property
                new ODataComplexValue { TypeName = "TestModel.SinglePrimitivePropertyComplexType", Properties = singlePrimitiveProperty },

                // Complex value with a single null property
                new ODataComplexValue { TypeName = "TestModel.SinglePrimitivePropertyComplexType", Properties = singleNullProperty },

                // Complex value with all primitive typed properties
                new ODataComplexValue { TypeName = "TestModel.AllPrimitivePropertiesComplexType", Properties = allPrimitiveProperties },

                // Complex value with complex property
                new ODataComplexValue { TypeName = "TestModel.SingleComplexPropertyComplexType", Properties = complexProperty },

                // Complex value with nested complex property
                new ODataComplexValue { TypeName = "TestModel.SingleComplexPropertyComplexType", Properties = nestedComplexProperty },

                // Complex value with primitive collection property
                new ODataComplexValue { TypeName = "TestModel.SinglePrimitiveCollectionPropertyComplexType", Properties = primitiveCollectionProperty },

                // Complex value with complex collection property
                new ODataComplexValue { TypeName = "TestModel.SingleComplexCollectionPropertyComplexType", Properties = complexCollectionProperty },

                // Complex value with different kinds of properties
                new ODataComplexValue { TypeName = "TestModel.DifferentPropertyKindsComplexType", Properties = differentPropertyKinds },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                complexValues,
                new bool[] { true, false },
                (complexValue, includeTypeReferences) =>
                {
                    IEdmComplexTypeReference typeReference = null;
                    if (includeTypeReferences)
                    {
                        IEdmType schemaType = model.FindType(complexValue.TypeName);
                        this.Assert.IsNotNull(schemaType, "Expected to find type in the model.");
                        typeReference = (IEdmComplexTypeReference)schemaType.ToTypeReference(/*nullable*/ false);
                    }

                    IEdmValue complexEdmValue = ODataEdmValueUtils.CreateStructuredEdmValue(complexValue, typeReference);
                    ODataEdmValueUtils.CompareValue(complexEdmValue, complexValue, this.Assert);
                });
        }
#endif
    }
}
