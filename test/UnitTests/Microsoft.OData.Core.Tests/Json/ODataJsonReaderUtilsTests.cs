//---------------------------------------------------------------------
// <copyright file="ODataJsonReaderUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonReaderUtilsTests
    {
        private readonly Type typeReaderUtils;
        private readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic ;

        public ODataJsonReaderUtilsTests()
        {
           this.typeReaderUtils =
                typeof(ODataReader).GetTypeInfo().Assembly.GetType("Microsoft.OData.Json.ODataJsonReaderUtils");
        }

        [Fact]
        public void TesConvertValueNull()
        {
            object nullValue = typeReaderUtils.GetMethod("ConvertValue", bindingFlags)
                .Invoke(null, /*static method*/
                    new object[] {
                        null,
                        EdmCoreModel.Instance.GetInt64(false),
                        new ODataMessageReaderSettings(), /*ODataMessageReaderSettings*/
                        false, /*validateNullValue*/
                        "custome.nullablePropertyName",
                        null /*ODataPayloadValueConverter*/
                    }
                );
            Assert.Null(nullValue);
        }

        [Fact]
        public void TestGetPayloadTypeName()
        {
            Dictionary<object[], string> payloadTypeToNames = new Dictionary<object[], string>();
            payloadTypeToNames.Add(new object[] {null}, null);
            payloadTypeToNames.Add(new object[] {true}, OData.Metadata.EdmConstants.EdmBooleanTypeName);
            payloadTypeToNames.Add(new object[] { "myPayloadItem" }, OData.Metadata.EdmConstants.EdmStringTypeName);
            payloadTypeToNames.Add(new object[] { Int32.MinValue }, OData.Metadata.EdmConstants.EdmInt32TypeName);
            payloadTypeToNames.Add(new object[] { Double.Epsilon }, OData.Metadata.EdmConstants.EdmDoubleTypeName);

            ODataCollectionValue collectionValue = new ODataCollectionValue();
            payloadTypeToNames.Add(new object[] { collectionValue }, EdmLibraryExtensions.GetCollectionTypeFullName(collectionValue.TypeName));

            ODataResource resourceValue = new ODataResource();
            payloadTypeToNames.Add(new object[] { resourceValue }, resourceValue.TypeName);

            foreach (KeyValuePair<object[], string> pair in payloadTypeToNames)
            {
                object result = typeReaderUtils.GetMethod("GetPayloadTypeName", bindingFlags)
                    .Invoke(null, /*static method*/
                        pair.Key);
                Assert.Equal(pair.Value, result);
            }
        }

        [Fact]
        public void TestGetPayloadTypeNameError()
        {
            bool exceptionThrown = true;
            try
            {
                typeReaderUtils.GetMethod("GetPayloadTypeName", bindingFlags)
                    .Invoke(null, /*static method*/
                        new object[] {Guid.NewGuid()});
                exceptionThrown = false;
            }
            catch (TargetInvocationException e)
            {
                Assert.IsType<ODataException>(e.InnerException);
                Assert.Equal("An internal error 'ODataJsonReader_ReadResourceStart' occurred.", e.InnerException.Message);
                return;
            }

            Assert.True(exceptionThrown);
        }
    }
}
