//---------------------------------------------------------------------
// <copyright file="SchemaGenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro.Test
{
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SchemaGenTests
    {
        [TestMethod]
        public void SimpleODataEntrySchema()
        {
            ODataResource entry = new ODataResource
            {
                TypeName = "NS.SimpleEntry",
                Properties = new[]
                {
                    new ODataProperty{  Name = "TNull"      , Value = null,             },
                    new ODataProperty{  Name = "TBoolean"   , Value = true,             },
                    new ODataProperty{  Name = "TInt32"     , Value = 30,               },
                    new ODataProperty{  Name = "TLong"      , Value = 31L,              },
                    new ODataProperty{  Name = "TFloat"     , Value = 3.2f,             },
                    new ODataProperty{  Name = "TDouble"    , Value = 3.3d,             },
                    new ODataProperty{  Name = "TBytes"     , Value = new byte[] {4},   },
                    new ODataProperty{  Name = "TString"    , Value = "35"              },
                }
            };

            string actual = ODataAvroSchemaGen.GetSchema(entry).ToString();
            string expected = TestHelper.GetResourceString("Microsoft.Test.OData.PluggableFormat.Tests.Avro.Test.SimpleODataEntrySchema.json");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SimpleODataEntrySchemaFromModel()
        {
            EdmEntityType type = new EdmEntityType("NS","SimpleEntry");
            type.AddStructuralProperty("TNull", EdmPrimitiveTypeKind.Boolean, true);
            type.AddStructuralProperty("TBoolean", EdmPrimitiveTypeKind.Boolean, false);
            type.AddStructuralProperty("TInt32", EdmPrimitiveTypeKind.Int32);
            type.AddStructuralProperty("TLong", EdmPrimitiveTypeKind.Int64, false);
            type.AddStructuralProperty("TFloat", EdmPrimitiveTypeKind.Single);
            type.AddStructuralProperty("TDouble", EdmPrimitiveTypeKind.Double, false);
            type.AddStructuralProperty("TBytes", EdmPrimitiveTypeKind.Binary);
            type.AddStructuralProperty("TString", EdmPrimitiveTypeKind.String, false);

            string actual = ODataAvroSchemaGen.GetSchemaFromModel(type).ToString();
            string expected = TestHelper.GetResourceString("Microsoft.Test.OData.PluggableFormat.Tests.Avro.Test.SimpleODataEntrySchemaFromModel.json");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ODataErrorSchema()
        {
            string actual = ODataAvroSchemaGen.GetSchema(new ODataError()).ToString();
            string expected = TestHelper.GetResourceString("Microsoft.Test.OData.PluggableFormat.Tests.Avro.Test.ODataErrorSchema.json");
            Assert.AreEqual(expected, actual);
        }
    }
}
#endif