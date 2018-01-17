//---------------------------------------------------------------------
// <copyright file="ActionParametersTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTest = System.Data.Test.Astoria;

    #endregion

    /// <summary>
    /// Test the serialization of action parameters from the client.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [Ignore] // Remove Atom
    // [TestClass]
    public class ActionParametersTests
    {
        #region  Properties
        private enum BasicParamValues
        {
            EnumPara1,
            EnumPara2,
            EnumPara3,
            IntParam1,
            IntParam2,
            StringParam1,
            StringParam2,
            ComplexParam1,
            ComplexParam2,
            ComplexParam3,
            ComplexParam4,
            ComplexWithComplex,
            UntypedNullParam,
            EmptyStringParam,
            IntCollParam1,
            IntCollParam2,
            EmptyPrimitiveCollParam,
            EmptyComplexCollParam,
            ListOfComplexTypesParam,
            ComplexCollParam,
            DateTimeOffsetParam,
            NullableParam,
        }

        private static readonly PayloadGeneratorSettings payloadGeneratorSettings = new PayloadGeneratorSettings() { IncludeWhitespaceInJson = false, QuotePropertyNamesInJson = true };

        private IEnumerable<TestCase> bodyOperationParametersPositiveTestCases;
        private IEnumerable<TestCase> uriOperationParametersPositiveTestCases;
        private IEnumerable<TestCase> negativeTestCases;
        private string defaultUriString = "/ActionWithParameters";

        #region PositiveTestScenarios
        /// <summary>
        /// Collection of all positive tests for BodyOperationParameters.
        /// </summary>
        private IEnumerable<TestCase> BodyOperationParameterPositiveTestCases
        {
            get
            {
                if (this.bodyOperationParametersPositiveTestCases == null)
                {
                    this.bodyOperationParametersPositiveTestCases = new TestCase[]
                    {
                        #region BodyOperationParameter Tests
                        
                        #region Enum
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { new BodyOperationParameter("p1", BasicParamValues.IntParam1 ) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddProperty("p1", "IntParam1"),
                        },
                        #endregion

                        #region primitive

                        // primitive
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddProperty("IntParam1", 5),
                        },
                    
                        // two primitives
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1), this.GetParam<BodyOperationParameter>(BasicParamValues.StringParam1) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddProperty("IntParam1", 5).AddProperty("StringParam1", "StringValue1"),
                        },

                        #endregion

                        #region entity
                    
                        // entity
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new [] { new BodyOperationParameter("p1" , new MyCustomerEntity() { ID = 1, Name= "Customer1" })},
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddComplexProperty("p1",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+MyCustomerEntity" }
                                        .AddProperty("ID", "1")
                                        .AddProperty("Name", "Customer1")),
                        },

                        #endregion

                        #region complex

                        // complex containing a collection of primitive type
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { new BodyOperationParameter("p1" , new ComplexType2() { City="Redmond", Numbers = new List<int> { 1, 2 ,3} })},
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddComplexProperty("p1",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+ComplexType2" }
                                        .AddProperty("City", "Redmond")
                                        .AddCollectionProperty("Numbers", "Edm.Int32", new List<int> { 1, 2, 3 })),
                        },

                        // complex containing a collection of complex type.
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexParam3) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddComplexProperty("ComplexParam3",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+ComplexType3" }
                                        .AddCollectionProperty("Addresses", "AstoriaServer.ActionParametersTests+AddressComplexType",
                                            new List<PayloadBuilder>()
                                            {
                                                new PayloadBuilder() { IsComplex=true }.AddProperty("City", "Redmond").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98052),
                                                new PayloadBuilder() { IsComplex=true }.AddProperty("City", "Bellevue").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98007),
                                            })
                                        .AddProperty("Key", 3))
                        },
   
                        // Complex
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexParam1) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddComplexProperty("ComplexParam1",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                        .AddProperty("City", "Redmond")
                                        .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                        .AddProperty("ZipCode", 98052))
                        },

                        // Complex containing another complex
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexWithComplex) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddComplexProperty("ComplexWithComplexParam",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+ComplexTypeWithComplex" }
                                        .AddComplexProperty("Address", new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType"}
                                                .AddProperty("City", "Woodinville")
                                                .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                                .AddProperty("ZipCode", 98077))
                                        .AddProperty("Name", "Allie"))
                        },

                        #endregion

                        #region Empty or null parameters
                        
                        // null value
                       new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.UntypedNullParam) } ,
                            ExpectedPayloadBuilder = new PayloadBuilder().AddProperty("UntypedNullParam", null)
                        },
                    
                        // empty string
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.EmptyStringParam) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddProperty("EmptyStringParam", String.Empty),
                        },

                        // empty collection of primitive
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.EmptyPrimitiveCollParam) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddCollectionProperty("EmptyPrimitiveCollection", null, new List<object>()),
                        },

                        // empty collection of complex
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.EmptyComplexCollParam) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddCollectionProperty("EmptyComplexCollection", null, new List<object>()),
                        },

                        // TODO:: 
                        // This currently fails because the playback service sets the content-type (which is null in this case) to be application/ATOM+XML.
                        // empty array
                        //new TestCase() 
                        // {
                        //      ActualUriString = this.defaultUriString,
                        //      ExpectedBaseUriString = this.defaultUriString,
                        //      HttpMethod = "POST",
                        //      Parameters = new BodyOperationParameter[]  {  },
                        //      ExpectedPayloadBuilder="",
                        //},

                        #endregion

                        #region misc

                        // primitive and complex
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1), this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexParam1) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddProperty("IntParam1", 5)
                                .AddComplexProperty("ComplexParam1",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                        .AddProperty("City", "Redmond")
                                        .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                        .AddProperty("ZipCode", 98052))
                        },

                        // complex and primitive
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexParam1), this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddComplexProperty("ComplexParam1",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                        .AddProperty("City", "Redmond")
                                        .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                        .AddProperty("ZipCode", 98052))
                                .AddProperty("IntParam1", 5)
                        },

                        // complex, primitive, complex collection, primitive
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexParam1), this.GetParam<BodyOperationParameter>(BasicParamValues.StringParam1),
                                this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexCollParam), this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddComplexProperty("ComplexParam1",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                        .AddProperty("City", "Redmond")
                                        .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                        .AddProperty("ZipCode", 98052))
                                .AddProperty("StringParam1", "StringValue1")
                                .AddCollectionProperty("ComplexCollection1", null,
                                    new List<PayloadBuilder>()
                                    {
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }.AddProperty("City", "Redmond").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98052),
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }.AddProperty("City", "Bellevue").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98007),
                                    })
                                .AddProperty("IntParam1", 5)
                        },
                        
                        // TODO:: Add some end-to-end tests once this code is ported to the OData branch.

                        #endregion
                        
                        #region primitive collection

                        // primitive collection (array)
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.IntCollParam1) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddCollectionProperty("IntCollection1", null, new List<int> { 1 }),
                        },

                        // primitive collection (array)
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.IntCollParam2) },
                            ExpectedPayloadBuilder = new PayloadBuilder().AddCollectionProperty("IntCollection2", null, new List<int> { 1, 2 }),
                        },

                        #endregion      
                        
                        #region  Enum colleciton
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { new BodyOperationParameter("EnumCollection1", new List<BasicParamValues>(){ BasicParamValues.EnumPara1, BasicParamValues.EnumPara2, BasicParamValues.EnumPara3 })},
                            ExpectedPayloadBuilder = new PayloadBuilder().AddCollectionProperty("EnumCollection1", null, new List<BasicParamValues>(){ BasicParamValues.EnumPara1, BasicParamValues.EnumPara2, BasicParamValues.EnumPara3 }),
                        },
                        #endregion

                        #region complex collection

                        // complex collection (array)
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[]  { this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexCollParam) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddCollectionProperty("ComplexCollection1", null,
                                    new List<PayloadBuilder>()
                                    {
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }.AddProperty("City", "Redmond").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98052),
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }.AddProperty("City", "Bellevue").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98007),
                                    }),
                        },

                        // List of complex types
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.ListOfComplexTypesParam) },
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddCollectionProperty("ListOfComplexTypes", null,
                                    new List<PayloadBuilder>()
                                    {
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }.AddProperty("City", "Redmond").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98052),
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }.AddProperty("City", "Bellevue").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98007),
                                    }),
                        },

                        // collection of complex types with inheritance. 
                        new  TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "POST",
                            Parameters = new OperationParameter[] { new BodyOperationParameter("p1", new ComplexType2[]
                                { new ComplexType2() { City="Redmond", Numbers=new List<int>() { 1,2} },
                                  new ComplexType4() { City="Redmond", State="WA", Numbers=new List<int>() { 1,2 }}})},
                           ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddCollectionProperty("p1", null,
                                    new List<PayloadBuilder>()
                                    {
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+ComplexType2" }.AddProperty("City", "Redmond").AddCollectionProperty("Numbers", "Edm.Int32", new List<int> { 1, 2 }),
                                        new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+ComplexType4" }.AddProperty("City", "Redmond").AddCollectionProperty("Numbers", "Edm.Int32", new List<int> { 1, 2 }).AddProperty("State", "WA"),
                                    }),
                        },

                        #endregion

                        #endregion
                    };
                }
                return this.bodyOperationParametersPositiveTestCases;
            }
        }

        /// <summary>
        /// Collection of all positive tests for UriOperationParameters.
        /// </summary>
        private IEnumerable<TestCase> UriOperationParameterPositiveTestCases
        {
            get
            {
                if (this.uriOperationParametersPositiveTestCases == null)
                {
                    this.uriOperationParametersPositiveTestCases = new TestCase[]
                   {
                        #region UriOperationParameter Tests
                        
                        #region Enum
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(p1=AstoriaServer.ActionParametersTests+BasicParamValues'IntParam1')",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  { new UriOperationParameter("p1", BasicParamValues.IntParam1 ) },
                            ExpectedPayloadBuilder = null,
                        },
                        #endregion

                        #region primitive

                        // primitive - int
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(IntParam1=5)",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.IntParam1) },
                            ExpectedPayloadBuilder = null,
                        },

                        // primitive - string
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(StringParam1='StringValue1')",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.StringParam1)},
                            ExpectedPayloadBuilder = null,
                        },

                        // primitive - string with quotes
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(param1='some''s''tring')",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("param1", "some's'tring" )},
                            ExpectedPayloadBuilder = null,
                        },

                        // primitive - empty string.
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(EmptyStringParam='')",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.EmptyStringParam) },
                            ExpectedPayloadBuilder = null,
                        },

                        // datetimeoffset
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(DateTimeParam=2011-01-01T00%3A00%3A00Z)",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.DateTimeOffsetParam) },
                            ExpectedPayloadBuilder = null,
                        },
      
                        // two primitives - int and string
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(IntParam1=5,StringParam1='StringValue1')",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.IntParam1), this.GetParam<UriOperationParameter>(BasicParamValues.StringParam1) },
                            ExpectedPayloadBuilder = null,
                        },              
                                        
                        // nullable bool.
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = Uri.EscapeUriString("/ActionWithParameters(NullableBool=5)"),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.NullableParam) },
                            ExpectedPayloadBuilder = null,
                        },

                        #endregion
     
                        #region complex
                        
                        // complex type
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("ComplexParam1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                    .AddProperty("City", "Redmond")
                                    .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                    .AddProperty("ZipCode", 98052)),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.ComplexParam1) },
                            ExpectedPayloadBuilder = null,
                        },


                        // TODO: need more investigation
                        // complex type with complex type
                        //new TestCase()  
                        //{
                        //    ActualUriString = this.defaultUriString,
                        //    ExpectedBaseUriString = this.defaultUriString,
                        //    ExpectedUriBuilder = new OperationUriBuilder()
                        //        .AddParameter("ComplexWithComplexParam", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+ComplexTypeWithComplex" }
                        //            .AddComplexProperty("Address", new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                        //                .AddProperty("City", "Woodinville")
                        //                .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                        //                .AddProperty("ZipCode", 98077))
                        //            .AddProperty("Name", "Allie")),
                        //    HttpMethod = "GET",
                        //    Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.ComplexWithComplex) },    
                        //    ExpectedPayloadBuilder = null,
                        //},
                    
                        // complex containing a collection of primitive type
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("p1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+ComplexType2" }
                                        .AddProperty("City", "Redmond")
                                        .AddCollectionProperty("Numbers", "Edm.Int32", new List<int> { 1, 2, 3 })),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("p1" , new ComplexType2() { City="Redmond", Numbers = new List<int> { 1, 2 ,3} })},
                            ExpectedPayloadBuilder = null,
                        },

                        // TODO: need more investigation
                        // complex containing a collection of complex type.
                        //new TestCase() 
                        //{
                        //    ActualUriString = this.defaultUriString,
                        //    ExpectedBaseUriString = this.defaultUriString,
                        //    ExpectedUriBuilder = new OperationUriBuilder()
                        //        .AddParameter("p1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+ComplexType3" }
                        //            .AddCollectionProperty("Addresses", "AstoriaServer.ActionParametersTests+AddressComplexType",
                        //                new List<PayloadBuilder>()
                        //                        {
                        //                            new PayloadBuilder() { IsComplex=true }.AddProperty("City", "Redmond").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98052),
                        //                            new PayloadBuilder() { IsComplex=true }.AddProperty("City", "Bellevue").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98007),
                        //                        })
                        //            .AddProperty("Key", 3)),
                        //    HttpMethod = "GET",
                        //    Parameters = new UriOperationParameter[] { new UriOperationParameter( "p1" ,new ComplexType3() { Key=3, Addresses = GetListOfAddressComplexType()})},
                        //    ExpectedPayloadBuilder = null,
                        //},
                    
                        #endregion
    
                        #region collection of enum

                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder().AddParameter("EnumCollection1", new CollectionPropertyPayloadBuilder(null, "AstoriaServer.ActionParametersTests+BasicParamValues" /*server side metadata name*/, new List<string> { BasicParamValues.EnumPara2 + "", BasicParamValues.EnumPara2 + "", BasicParamValues.EnumPara3 + "" })),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("EnumCollection1", new List<BasicParamValues> { BasicParamValues.EnumPara2, BasicParamValues.EnumPara2, BasicParamValues.EnumPara3 }) },
                            ExpectedPayloadBuilder = null,
                        },

                        #endregion

                        #region collection of primitive/complex

                        // collection of complex type (list)
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("ListOfComplexTypes",
                                    new CollectionPropertyPayloadBuilder(null, "AstoriaServer.ActionParametersTests+AddressComplexType", new List<PayloadBuilder>
                                    {
                                        new PayloadBuilder() { IsComplex = true }
                                            .AddProperty("City", "Redmond")
                                            .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                            .AddProperty("ZipCode", 98052),
                                        new PayloadBuilder() { IsComplex = true}
                                            .AddProperty("City", "Bellevue")
                                            .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                            .AddProperty("ZipCode", 98007)
                                    })),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.ListOfComplexTypesParam) },
                            ExpectedPayloadBuilder = null,
                        },
                    
                        // complex collection (array)
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("ComplexCollection1",
                                    new CollectionPropertyPayloadBuilder(null, "AstoriaServer.ActionParametersTests+AddressComplexType", new List<PayloadBuilder>
                                    {
                                        new PayloadBuilder() { IsComplex = true }
                                            .AddProperty("City", "Redmond")
                                            .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                            .AddProperty("ZipCode", 98052),
                                        new PayloadBuilder() { IsComplex = true}
                                            .AddProperty("City", "Bellevue")
                                            .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                            .AddProperty("ZipCode", 98007)
                                    })),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  { this.GetParam<UriOperationParameter>(BasicParamValues.ComplexCollParam) },
                            ExpectedPayloadBuilder = null,
                        },

                        // primitive collection (array) - 1 element
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder().AddParameter("IntCollection1", new CollectionPropertyPayloadBuilder(null, "Edm.Int32", new List<int> { 1 })),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  { this.GetParam<UriOperationParameter>(BasicParamValues.IntCollParam1) },
                            ExpectedPayloadBuilder = null,
                        },

                        // primitive collection (array) - 2 element
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder().AddParameter("IntCollection2", new CollectionPropertyPayloadBuilder(null, "Edm.Int32", new List<int> { 1, 2 })),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  { this.GetParam<UriOperationParameter>(BasicParamValues.IntCollParam2) },
                            ExpectedPayloadBuilder = null,
                        },

                        #endregion
     
                        #region Empty and null parameters
                
                        // untyped null parameter
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(UntypedNullParam=null)",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.UntypedNullParam) },
                            ExpectedPayloadBuilder = null,
                        },

                        // empty string
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = "/ActionWithParameters(EmptyStringParam='')",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  { this.GetParam<UriOperationParameter>(BasicParamValues.EmptyStringParam) },
                            ExpectedPayloadBuilder = null,
                        },

                        // empty collection of primitive
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("EmptyPrimitiveCollection", new CollectionPropertyPayloadBuilder(null, "Edm.Int32", new List<object>())),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  { this.GetParam<UriOperationParameter>(BasicParamValues.EmptyPrimitiveCollParam) },
                            ExpectedPayloadBuilder = null,
                        },

                        // empty collection of complex
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("EmptyComplexCollection", new CollectionPropertyPayloadBuilder(null, "AstoriaServer.ActionParametersTests+AddressComplexType", new List<object>())),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  { this.GetParam<UriOperationParameter>(BasicParamValues.EmptyComplexCollParam) },
                            ExpectedPayloadBuilder = null,
                        },

                        // empty parameter array
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[]  {  },
                            ExpectedPayloadBuilder = null,
                        },

                        #endregion 

                        #region entity

                         // entity type
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("ComplexParam1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                    .AddProperty("City", "Redmond")
                                    .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                    .AddProperty("ZipCode", 98052)),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.ComplexParam1) },
                            ExpectedPayloadBuilder = null,
                        },

                         // entity collection type
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new OperationParameter[] { new UriOperationParameter("p1", new MyCustomerEntity() { ID = 1, Name= "Customer1" } )},
                            HttpMethod = "GET",
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("ListOfComplexTypes",
                                    new CollectionPropertyPayloadBuilder(null, "AstoriaServer.ActionParametersTests+AddressComplexType", new List<PayloadBuilder>
                                    {
                                        new PayloadBuilder() { IsComplex = true }
                                            .AddProperty("City", "Redmond")
                                            .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                            .AddProperty("ZipCode", 98052),
                                        new PayloadBuilder() { IsComplex = true}
                                            .AddProperty("City", "Bellevue")
                                            .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                            .AddProperty("ZipCode", 98007)
                                    })),
                            ExpectedPayloadBuilder = null,
                        },


                        #endregion
 
                        #region Alias 

                        // alias with regular params
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString + "(param1=@p1,param2=@p2)",
                           ExpectedBaseUriString = Uri.EscapeUriString("/ActionWithParameters(param1=@p1,param2=@p2,p3=3)?@p1=5&@p2='stringparam'"),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("@p1", 5), new UriOperationParameter("@p2", "stringparam"), new UriOperationParameter("p3",3)},
                            ExpectedPayloadBuilder = null,
                        },
                    
                        // alias
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString + "/func(param1=@p1,param2=@p2)",
                            ExpectedBaseUriString = Uri.EscapeUriString("/ActionWithParameters/func(param1=@p1,param2=@p2)?@p1=5&@p2='stringparam'"),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("@p1", 5), new UriOperationParameter("@p2", "stringparam")},
                            ExpectedPayloadBuilder = null,
                        },

                        #endregion
                    
                        #region misc
                        // primitive and complex
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("IntParam1", 5)
                                .AddParameter("ComplexParam1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                    .AddProperty("City", "Redmond")
                                    .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                    .AddProperty("ZipCode", 98052)),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.IntParam1), this.GetParam<UriOperationParameter>(BasicParamValues.ComplexParam1) },
                            ExpectedPayloadBuilder = null,
                        },

                        // complex and primitive
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("ComplexParam1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                    .AddProperty("City", "Redmond")
                                    .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                    .AddProperty("ZipCode", 98052))
                                .AddParameter("IntParam1", 5),
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.ComplexParam1), this.GetParam<UriOperationParameter>(BasicParamValues.IntParam1) },
                            ExpectedPayloadBuilder = null,
                        },

                        // TODO: need more investigation
                        // complex, primitive, complex collection, primitive
                        //new TestCase()
                        //{
                        //    ActualUriString = this.defaultUriString,
                        //    ExpectedBaseUriString = this.defaultUriString,
                        //    ExpectedUriBuilder = new OperationUriBuilder()
                        //        //.AddParameter("ComplexParam1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                        //        //    .AddProperty("City", "Redmond")
                        //        //    .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                        //        //    .AddProperty("ZipCode", 98052))
                        //        //.AddParameter("StringParam1", "StringValue1")
                        //        .AddParameter("ComplexCollection1", new CollectionPropertyPayloadBuilder(null, "AstoriaServer.ActionParametersTests+AddressComplexType",
                        //            new List<PayloadBuilder>
                        //            {
                        //                new PayloadBuilder() { IsComplex = true }.AddProperty("City", "Redmond").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98052),
                        //                new PayloadBuilder() { IsComplex = true }.AddProperty("City", "Bellevue").AddProperty("Enum1", BasicParamValues.EnumPara1 + "").AddProperty("ZipCode", 98007),
                        //            })),
                        //        //.AddParameter("IntParam1", 5),
                        //    HttpMethod = "GET",
                        //    Parameters = new UriOperationParameter[] { /*this.GetParam<UriOperationParameter>(BasicParamValues.ComplexParam1), this.GetParam<UriOperationParameter>(BasicParamValues.StringParam1), */
                        //        this.GetParam<UriOperationParameter>(BasicParamValues.ComplexCollParam),/* this.GetParam<UriOperationParameter>(BasicParamValues.IntParam1) */},
                        //    ExpectedPayloadBuilder = null,
                        //},

                        // Uri parameters with HttpMethod = POST
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("ComplexParam1", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+AddressComplexType" }
                                    .AddProperty("City", "Redmond")
                                    .AddProperty("Enum1", BasicParamValues.EnumPara1 + "")
                                    .AddProperty("ZipCode", 98052))
                                .AddParameter("StringParam1", "StringValue1"),
                            HttpMethod = "POST",
                            Parameters = new UriOperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.ComplexParam1), this.GetParam<UriOperationParameter>(BasicParamValues.StringParam1)},
                            ExpectedPayloadBuilder = null,
                        },
                    
                        // combination of uri and body parameters with HttpMethod = POST.
                        new  TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            ExpectedBaseUriString = this.defaultUriString,
                            ExpectedUriBuilder = new OperationUriBuilder()
                                .AddParameter("IntParam1", 5)
                                .AddParameter("ComplexParam2", new PayloadBuilder() { IsComplex = true, TypeName = "AstoriaServer.ActionParametersTests+ComplexType2" }
                                    .AddProperty("City", "Redmond")
                                    .AddCollectionProperty("Numbers", "Edm.Int32", new List<int> { 1, 2 })),
                            HttpMethod = "POST",
                            Parameters = new OperationParameter[] { this.GetParam<UriOperationParameter>(BasicParamValues.IntParam1), this.GetParam<BodyOperationParameter>(BasicParamValues.StringParam1),
                                this.GetParam<UriOperationParameter>(BasicParamValues.ComplexParam2), this.GetParam<BodyOperationParameter>(BasicParamValues.ComplexParam2)},
                            ExpectedPayloadBuilder = new PayloadBuilder()
                                .AddProperty("StringParam1", "StringValue1")
                                .AddComplexProperty("ComplexParam2",
                                    new PayloadBuilder() { TypeName = "AstoriaServer.ActionParametersTests+ComplexType2" }
                                        .AddProperty("City", "Redmond")
                                        .AddCollectionProperty("Numbers", "Edm.Int32", new List<int> { 1, 2 })),
                        },
                        
                        #endregion

                        #region uri with existing query string containing escaped or un-escaped characters.
                     
                        // TODO: need more investigation
                        // uri with existing unescaped query string.
                        //new TestCase()  
                        //{
                        //    ActualUriString = this.defaultUriString + "?What=A B C",
                        //    ExpectedBaseUriString = "/ActionWithParameters(p1='This%20is%20with%2520space')?What=A%20B%20C",
                        //    HttpMethod = "GET",
                        //    Parameters = new UriOperationParameter[] { new UriOperationParameter("p1", "This is with%20space") },    
                        //},

                        // uri with existing escaped query string.
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString + "?What=A%20B%20C",
                            ExpectedBaseUriString = "/ActionWithParameters(p1='This%20is%20with%20space')?What=A%20B%20C",
                            HttpMethod = "GET",
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("p1", "This is with space") },
                        },

                        // TODO: need more investigation
                        // uri with existing esacaped query string containing '%3F' (?) and '%26' (&); 
                        // Expected%3A The '%' sign in the value of UriOperationParameter (%3F%26) should be escaped with %25.
                        //new TestCase()  
                        //{
                        //    ActualUriString = this.defaultUriString + "?p1=%3F%26",
                        //    ExpectedBaseUriString = "/ActionWithParameters(p2='%253F%2526')?p1=%3F%26",
                        //    HttpMethod = "GET",
                        //    Parameters = new UriOperationParameter[] { new UriOperationParameter("p2", "%3F%26") },    
                        //},

                        #endregion

                        #endregion
                    };
                }
                return this.uriOperationParametersPositiveTestCases;
            }
        }
        #endregion

        #region NegativeTestScenarios
        /// <summary>
        /// Collection of all negative tests.
        /// </summary>
        private IEnumerable<TestCase> NegativeTestCases
        {
            get
            {
                if (this.negativeTestCases == null)
                {
                    this.negativeTestCases = new TestCase[]
                    {  
                        #region BodyOperationParameter tests

                        // duplicate parameter name for body operation parameter
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new OperationParameter[] { new BodyOperationParameter("p1", 5), new BodyOperationParameter("p1", 6)},
                            HttpMethod = "POST",
                            ExpectedErrorMessage= "Multiple body operation parameters were found with the same name. Body operation parameter names must be unique.",
                        },
   
                        // loop in complex type for body parameters.
                        new  TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new OperationParameter[] { new BodyOperationParameter("p1", GetComplexTypeWithLoop())},
                            HttpMethod = "POST",
                            ExpectedErrorMessage = "A circular loop was detected while serializing the property 'ComplexType'. You must make sure that loops are not present in properties that return a collection or complex type.",
                        },
                       
                        // body operation parameter with GET.
                        new  TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new OperationParameter[] { new BodyOperationParameter("p1", 5)},
                            HttpMethod = "GET",
                            ExpectedErrorMessage = "OperationParameter of type BodyOperationParameter cannot be specified when the HttpMethod is set to GET.",
                        },
                        
                        // Arraylist
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new BodyOperationParameter[]  { new BodyOperationParameter("p1", new ArrayList() { 1 } ) },
                            HttpMethod = "POST",
                            ExpectedErrorMessage = "The complex type 'System.Object' has no settable properties.",
                        },

                        #endregion

                        #region UriOperationParameter tests

                        // duplicate parameter name for uri operation parameter
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new OperationParameter[] { new UriOperationParameter("p1", 5), new UriOperationParameter("p1", 6)},
                            HttpMethod = "GET",
                            ExpectedErrorMessage= "Multiple uri operation parameters were found with the same name. Uri operation parameter names must be unique.",
                        },

                        // null element inside array.
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("p1",5), null, new UriOperationParameter("p3", 6)},
                            HttpMethod = "GET",
                            ExpectedErrorMessage = "The operation parameters array contains a null element which is not allowed.",
                        },

                        // loop in complex type for uri parameters.
                        new  TestCase()
                        {
                            ActualUriString = this.defaultUriString,
                            Parameters = new OperationParameter[] { new UriOperationParameter("p1", GetComplexTypeWithLoop())},
                            HttpMethod = "GET",
                            ExpectedErrorMessage = "A circular loop was detected while serializing the property 'ComplexType'. You must make sure that loops are not present in properties that return a collection or complex type.",
                        },

                        // parameter alias not contained in the uri.
                        new TestCase()
                        {
                            ActualUriString = this.defaultUriString + "/func(param1=@p2,param2=@p3)",
                            Parameters = new UriOperationParameter[] { new UriOperationParameter("@p1", 5), new UriOperationParameter("@p2", "stringparam") },
                            HttpMethod = "GET",
                            ExpectedErrorMessage = "The parameter alias '@p1' was not present in the request URI. All parameters passed as alias must be present in the request URI.",
                        },
                        
                        #endregion
                    };
                }

                return this.negativeTestCases;
            }
        }
        #endregion

        #endregion

        #region Types

        private T GetParam<T>(BasicParamValues paramType) where T : OperationParameter
        {
            switch (paramType)
            {
                case BasicParamValues.IntParam1:
                    return (T)Activator.CreateInstance(typeof(T), "IntParam1", 5);

                case BasicParamValues.IntParam2:
                    return (T)Activator.CreateInstance(typeof(T), "IntParam2", 6);

                case BasicParamValues.StringParam1:
                    return (T)Activator.CreateInstance(typeof(T), "StringParam1", "StringValue1");

                case BasicParamValues.StringParam2:
                    return (T)Activator.CreateInstance(typeof(T), "StringParam2", "StringValue2");

                case BasicParamValues.ComplexParam1:
                    return (T)Activator.CreateInstance(typeof(T), "ComplexParam1", new AddressComplexType() { City = "Redmond", ZipCode = 98052, Enum1 = BasicParamValues.EnumPara1 });

                case BasicParamValues.ComplexParam2:
                    return (T)Activator.CreateInstance(typeof(T), "ComplexParam2", new ComplexType2() { City = "Redmond", Numbers = new List<int>() { 1, 2 } });

                case BasicParamValues.ComplexParam3:
                    return (T)Activator.CreateInstance(typeof(T), "ComplexParam3", new ComplexType3() { Key = 3, Addresses = GetListOfAddressComplexType() });

                case BasicParamValues.ComplexParam4:
                    return (T)Activator.CreateInstance(typeof(T), "ComplexParam4", new ComplexType4() { City = "Bellevue", State = "WA", Numbers = new List<int>() { 1, 2, 3 } });

                case BasicParamValues.ComplexWithComplex:
                    return (T)Activator.CreateInstance(typeof(T), "ComplexWithComplexParam", new ComplexTypeWithComplex() { Name = "Allie", Address = new AddressComplexType() { City = "Woodinville", ZipCode = 98077 } });

                case BasicParamValues.UntypedNullParam:
                    return (T)Activator.CreateInstance(typeof(T), "UntypedNullParam", null);

                case BasicParamValues.EmptyStringParam:
                    return (T)Activator.CreateInstance(typeof(T), "EmptyStringParam", "");

                case BasicParamValues.IntCollParam1:
                    return (T)Activator.CreateInstance(typeof(T), "IntCollection1", new Int32[] { 1 });

                case BasicParamValues.IntCollParam2:
                    return (T)Activator.CreateInstance(typeof(T), "IntCollection2", new Int32[] { 1, 2 });

                case BasicParamValues.EmptyPrimitiveCollParam:
                    return (T)Activator.CreateInstance(typeof(T), "EmptyPrimitiveCollection", new Int32[] { });

                case BasicParamValues.EmptyComplexCollParam:
                    return (T)Activator.CreateInstance(typeof(T), "EmptyComplexCollection", new AddressComplexType[] { });

                case BasicParamValues.ListOfComplexTypesParam:
                    return (T)Activator.CreateInstance(typeof(T), "ListOfComplexTypes", GetListOfAddressComplexType());

                case BasicParamValues.ComplexCollParam:
                    return (T)Activator.CreateInstance(typeof(T), "ComplexCollection1", new AddressComplexType[] { new AddressComplexType() { City = "Redmond", ZipCode = 98052, Enum1 = BasicParamValues.EnumPara1 }, new AddressComplexType() { City = "Bellevue", ZipCode = 98007, Enum1 = BasicParamValues.EnumPara1 } });

                case BasicParamValues.DateTimeOffsetParam:
                    return (T)Activator.CreateInstance(typeof(T), "DateTimeParam", new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero));

                case BasicParamValues.NullableParam:
                    return (T)Activator.CreateInstance(typeof(T), "NullableBool", (int?)5);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns a list of complex type.
        /// </summary>
        /// <returns>list of AddressComplexType.</returns>
        private static List<AddressComplexType> GetListOfAddressComplexType()
        {
            List<AddressComplexType> list = new List<AddressComplexType>();
            list.Add(new AddressComplexType() { City = "Redmond", ZipCode = 98052, Enum1 = BasicParamValues.EnumPara1 });
            list.Add(new AddressComplexType() { City = "Bellevue", ZipCode = 98007, Enum1 = BasicParamValues.EnumPara1 });
            return list;
        }

        /// <summary>
        /// Returns a complex type with loop.
        /// </summary>
        /// <returns>ComplexType5 with loop</returns>
        private static ComplexType5 GetComplexTypeWithLoop()
        {
            ComplexType5 ct = new ComplexType5();
            ct.City = "Redmond";
            ct.ComplexType = ct;
            return ct;
        }

        /// <summary>
        /// Represents a unit test.
        /// </summary>
        private class TestCase
        {
            public string ActualUriString = string.Empty;
            public OperationParameter[] Parameters;
            public PayloadBuilder ExpectedPayloadBuilder = null;
            public string ExpectedBaseUriString = String.Empty;
            public OperationUriBuilder ExpectedUriBuilder = null;
            public string ExpectedErrorMessage = String.Empty;
            public string HttpMethod = "POST";

            public virtual Func<DataServiceContext, Uri, bool, OperationParameter[], OperationResponse> ExecuteMethodWithParams
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Represents an entity
        /// </summary>
        private class MyCustomerEntity
        {
            public int ID
            {
                set;
                get;
            }

            public string Name
            {
                set;
                get;
            }
        }

        /// <summary>
        /// Represents a complex type.
        /// </summary>
        private class AddressComplexType
        {
            public int ZipCode
            {
                set;
                get;
            }

            public string City
            {
                set;
                get;
            }

            public BasicParamValues Enum1
            {
                set;
                get;
            }
        }

        /// <summary>
        /// Represents a complex type that contains a collection of primitives.
        /// </summary>
        private class ComplexType2
        {
            public string City
            {
                set;
                get;
            }

            public List<int> Numbers
            {
                get;
                set;
            }

        }

        /// <summary>
        /// Represents a complex type that contains a collection of complex types.
        /// </summary>        
        private class ComplexType3
        {
            public int Key
            {
                set;
                get;
            }
            public List<AddressComplexType> Addresses
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Represents a complex type that contains another complex type
        /// </summary>
        private class ComplexTypeWithComplex
        {
            public string Name
            {
                set;
                get;
            }

            public AddressComplexType Address
            {
                get;
                set;
            }

        }

        public delegate IAsyncResult MyBeginExecute(DataServiceContext ctx, Uri uri, OperationParameter[] parameters);
        public delegate void MyEndExecute(DataServiceContext ctx, IAsyncResult result);

        /// <summary>
        /// A complex type that inherits from ComplexType2
        /// </summary>
        private class ComplexType4 : ComplexType2
        {
            public string State
            {
                get;
                set;
            }
        }

        /// <summary>
        /// A complex type containing a complex type as a property.
        /// </summary>
        private class ComplexType5
        {
            public string City
            {
                set;
                get;
            }

            public ComplexType5 ComplexType
            {
                set;
                get;
            }
        }

        #endregion

        #region Reflection Provider

        public static Dictionary<string, object> ExpectedResults = new Dictionary<string, object>();

        [KeyAttribute("OrderId")]
        public class Order
        {
            public int OrderId { get; set; }
        }

        public partial class OrderItemData
        {
            static IList<Order> orders;
            static OrderItemData()
            {
                orders = new Order[] { new Order() { OrderId = 1 } };
            }

            public IQueryable<Order> Orders
            {
                get { return orders.AsQueryable<Order>(); }
            }
        }

        public class OrderItems : DataService<OrderItemData>
        {
            [System.ServiceModel.Web.WebGet]
            public void ServiceOpTest(int p1, string p2)
            {
                Assert.AreEqual(p1, (int)ExpectedResults["p1"]);
                Assert.AreEqual(p2, (string)ExpectedResults["p2"]);
            }

            public static void InitializeService(IDataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("Orders", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            }
        }

        #endregion

        #region TestMethods

        #region Error Tests

        [TestMethod]
        public void OperationParameterConstructorErrorTests()
        {
            try
            {
                OperationParameter op = new BodyOperationParameter(null, 5);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "The Name property of an OperationParameter must be set to a non-null, non-empty string.");
            }

            try
            {
                OperationParameter op = new UriOperationParameter("", 4);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "The Name property of an OperationParameter must be set to a non-null, non-empty string.");
            }
        }

        [TestMethod]
        public void ExecuteActionParametersErrorTestsWithJsonLight()
        {
            RunExecuteActionParametersErrorTests(ODataFormat.Json, (ctx, uri, testCase) => ctx.Execute(uri, testCase.HttpMethod, testCase.Parameters));
        }


        [TestMethod]
        public void ExecuteActionParametersErrorTestsAsyncWithJsonLight()
        {
            RunExecuteActionParametersErrorTests(ODataFormat.Json, (ctx, uri, testCase) => ctx.BeginExecute<object>(uri, asyncResult => ctx.EndExecute<object>(asyncResult), null, testCase.HttpMethod, false, testCase.Parameters));
        }

        private void RunExecuteActionParametersErrorTests(ODataFormat contextFormat, Action<DataServiceContext, Uri, TestCase> executeAction)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                request.StartService();

                AstoriaTest.TestUtil.RunCombinations(this.NegativeTestCases, (testCase) =>
                {
                    DataServiceContext ctx = CreateContext(request, contextFormat);
                    Uri uri = new Uri(request.ServiceRoot + testCase.ActualUriString);
                    Exception exception = AstoriaTest.TestUtil.RunCatching(() => executeAction(ctx, uri, testCase));
                    Assert.IsNotNull(exception, "exception != null");
                    Assert.AreEqual(testCase.ExpectedErrorMessage, exception.Message);
                });
            }
        }

        #endregion

        #region Positive Tests
        [Ignore]
        [TestMethod]
        public void ExecuteActionParametersTestsWithJsonLight()
        {
            // Test all positive test cases for action body parameter serialization with JSON Light.
            this.RunExecuteActionParameterTests(true, ODataFormat.Json);
        }

        [Ignore]
        [TestMethod]
        public void ExecuteActionParametersTestsAsyncWithJsonLight()
        {
            // Test all positive test cases for action parameter serialization in async mode with JSON Light.
            RunExecuteActionParameterTests(false, ODataFormat.Json);
        }

        [Ignore]
        [TestMethod]
        public void ExecuteUriWithExistingEscapedCollectionValueWithJsonLight()
        {
            var testCases = new TestCase[]
            {
                new TestCase()
                {
                    // Original query string = IntCollection1={"odata.type"%3A"Collection(Edm.Int32)","value"%3A[1]}
                    ActualUriString = this.defaultUriString + "?IntCollection1=%7B%22odata.type%22%3A%22Collection(Edm.Int32)%22,%22value%22%3A%5B1%5D%7D",
                    //"/ActionWithParameters(p1='%7B%2526*%7D')IntCollection1=%7B%22odata.type%22%3A%22Collection(Edm.Int32)%22,%22value%22%3A%5B1%5D%7D"
                    ExpectedBaseUriString = "/ActionWithParameters?IntCollection1=%7B%22odata.type%22%3A%22Collection(Edm.Int32)%22,%22value%22%3A%5B1%5D%7D&p1='%7B%2526*%7D'",
                    HttpMethod = "GET",
                    Parameters = new UriOperationParameter[] { new UriOperationParameter("p1", "{%26*}") },
                },
            };

            this.RunExecuteActionParameterTests(testCases, true, ODataFormat.Json);
        }

        private void RunExecuteActionParameterTests(bool runSynchronous, ODataFormat contextFormat)
        {
            this.RunExecuteActionParameterTests(this.BodyOperationParameterPositiveTestCases, runSynchronous, contextFormat);
            this.RunExecuteActionParameterTests(this.UriOperationParameterPositiveTestCases, runSynchronous, contextFormat);
        }

        private void RunExecuteActionParameterTests(IEnumerable<TestCase> testCases, bool runSynchronous, ODataFormat contextFormat)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.ProcessRequestOverride.Restore())
            {
                request.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                request.StartService();

                AstoriaTest.TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    PlaybackService.ProcessRequestOverride.Value = (req) =>
                    {
                        // Parameters are sent using JSON when context format is Atom
                        var expectedParameterFormat = contextFormat;

                        string expectedUriString = testCase.ExpectedBaseUriString;
                        OperationUriBuilder operationUriBuilder = testCase.ExpectedUriBuilder;
                        if (operationUriBuilder != null)
                        {
                            var parameterQueryString = UriOperationParameterGenerator.Generate(operationUriBuilder, expectedParameterFormat, payloadGeneratorSettings);
                            expectedUriString = testCase.ExpectedBaseUriString + "(" + parameterQueryString + ")";
                        }

                        Assert.AreEqual(Uri.UnescapeDataString(expectedUriString), Uri.UnescapeDataString(req.RequestUriString));
                        StreamReader reader = new StreamReader(req.GetRequestStream());
                        string actualPayload = reader.ReadToEnd();

                        string expectedPayload = GetExpectedPayload(testCase, expectedParameterFormat);

                        Assert.AreEqual(expectedPayload, actualPayload);
                        if (req.RequestContentLength > 0)
                        {
                            string contentType = "application/json;odata.metadata=minimal";
                            Assert.AreEqual(contentType, req.RequestContentType);
                        }

                        req.SetResponseStatusCode(200);
                        return req;
                    };


                    DataServiceContext ctx = CreateContext(request, contextFormat);

                    ctx.ResolveName = (clientType) =>
                    {
                        return clientType.FullName.Replace("AstoriaUnitTests", "AstoriaServer");
                    };

                    Uri uri = new Uri(request.ServiceRoot + testCase.ActualUriString);
                    if (runSynchronous)
                    {
                        ctx.Execute(uri, testCase.HttpMethod, testCase.Parameters);
                    }
                    else
                    {
                        IAsyncResult result = ctx.BeginExecute<object>(uri, null, null, testCase.HttpMethod, false, testCase.Parameters);
                        ctx.EndExecute<object>(result);
                    }
                });
            }
        }
        #endregion

        #region Async Cancel Tests
        [TestMethod]
        public void ExecuteActionParametersTestsAsyncCancelWithJsonLight()
        {
            // Test that cancelling an async execute works with JSON Light.
            this.RunCancelRequestTest(ODataFormat.Json);
        }

        private void RunCancelRequestTest(ODataFormat format)
        {
            var cancelTestCase = new TestCase()
            {
                Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1) },
                ExpectedErrorMessage = "The operation has been canceled.",
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                request.StartService();

                DataServiceContext ctx = CreateContext(request, format);
                Uri uri = new Uri(request.ServiceRoot + this.defaultUriString);
                IAsyncResult result = ctx.BeginExecute<object>(uri, null, null, "POST", false, cancelTestCase.Parameters);
                ctx.CancelRequest(result);
                Exception exception = AstoriaTest.TestUtil.RunCatching(delegate ()
                {
                    ctx.EndExecute<object>(result);
                });

                Assert.IsNotNull(exception, "exception != null");
                Assert.AreEqual(cancelTestCase.ExpectedErrorMessage, exception.Message);
            }
        }

        #endregion

        #region Mismatch between BeginExecute And EndExecute calls tests.

        private class BeginAndEndExecuteTestCase
        {
            internal BodyOperationParameter[] Parameters { get; set; }
            internal string ExpectedErrorMessage { get; set; }
            internal MyBeginExecute BeginExecuteMethod { get; set; }
            internal MyEndExecute EndExecuteMethod { get; set; }
        }

        [TestMethod]
        public void BeginAndEndExecuteErrorTestsWithJsonLight()
        {
            // Test that incorrectly calling EndExecute throws error with JSON Light.
            this.RunBeginAndEndExecuteErrorTests(ODataFormat.Json);
        }

        private void RunBeginAndEndExecuteErrorTests(ODataFormat format)
        {
            var testCases = new BeginAndEndExecuteTestCase[]
            {
                // Call EndExecute<T> on BeginExecute.
                new BeginAndEndExecuteTestCase()
                {
                    Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1) },
                    ExpectedErrorMessage = "The current object did not originate the async result.\r\nParameter name: asyncResult",
                    BeginExecuteMethod = new MyBeginExecute((DataServiceContext ctx, Uri uri, OperationParameter[] parameters) =>
                        {
                            return ctx.BeginExecute(uri, null, null, "POST", parameters);
                        }),

                    EndExecuteMethod = new MyEndExecute((DataServiceContext ctx, IAsyncResult result) =>
                        {
                            ctx.EndExecute<object>(result);
                        }),
                },
                    
                // Call EndExecute on BeginExecute<T>. 
                new BeginAndEndExecuteTestCase()
                {
                    Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1) },
                    ExpectedErrorMessage = "The current object did not originate the async result.\r\nParameter name: asyncResult",
                    BeginExecuteMethod = new MyBeginExecute((DataServiceContext ctx, Uri uri, OperationParameter[] parameters) =>
                        {
                            return ctx.BeginExecute<object>(uri, null, null, "POST", false, parameters);
                        }),

                    EndExecuteMethod = new MyEndExecute((DataServiceContext ctx, IAsyncResult result) =>
                        {
                            ctx.EndExecute(result);
                        }),
                },

                // Call EndExecute on another BeginExecute<T> overload. 
                new BeginAndEndExecuteTestCase()
                {
                    Parameters = new BodyOperationParameter[] { this.GetParam<BodyOperationParameter>(BasicParamValues.IntParam1) },
                    ExpectedErrorMessage = "The current object did not originate the async result.\r\nParameter name: asyncResult",
                    BeginExecuteMethod = new MyBeginExecute((DataServiceContext ctx, Uri uri, OperationParameter[] parameters) =>
                        {
                            return ctx.BeginExecute<object>(uri, null, null);
                        }),

                    EndExecuteMethod = new MyEndExecute((DataServiceContext ctx, IAsyncResult result) =>
                        {
                            ctx.EndExecute(result);
                        }),
                },
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                request.StartService();

                AstoriaTest.TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    DataServiceContext ctx = CreateContext(request, format);

                    Uri uri = new Uri(request.ServiceRoot + this.defaultUriString);
                    IAsyncResult result = testCase.BeginExecuteMethod(ctx, uri, testCase.Parameters);
                    Exception exception = AstoriaTest.TestUtil.RunCatching(delegate ()
                    {
                        testCase.EndExecuteMethod(ctx, result);
                    });

                    Assert.IsNotNull(exception, "exception != null");
                    Assert.AreEqual(testCase.ExpectedErrorMessage, exception.Message);
                });
            }
        }
        #endregion

        #region Uri parameters end-to-end test (uses service operation)
        //TODO: Need to update server for serviceoperation parameter
        [Ignore]
        [TestMethod]
        public void ExecuteServiceOperationTestsWithUriParametersWithJsonLight()
        {
            // End-to-End test for uri parameters using service operation with JSON Light.
            RunServiceOperationTestWithUriParameters(ODataFormat.Json);
        }

        private static void RunServiceOperationTestWithUriParameters(ODataFormat contextFormat)
        {
            TestCase[] positiveTestCases = new TestCase[]
            {   
                // a uri parameter containing spaces. (Client should escape the uri parameter containing spaces)
                new TestCase()
                {
                    ActualUriString = "/ServiceOpTest",
                    Parameters = new OperationParameter[] { new UriOperationParameter("p1", 5), new UriOperationParameter("p2", "This is a string with space") },
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        ExpectedResults.Clear();
                        ExpectedResults.Add("p1", 5);
                        ExpectedResults.Add("p2", "This is a string with space");
                        return ctx.Execute(uri, "GET", operationParameters);
                    },
                },

                // a uri parameter containing spaces and special characters which should be escaped.
                new TestCase()
                {
                    ActualUriString = "/ServiceOpTest",
                    Parameters = new OperationParameter[] { new UriOperationParameter("p1", 5), new UriOperationParameter("p2", "{\"__metadata\":{\"type\":\"Collection(Edm.Int32)\"}, \"results\":[1]}") },
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        ExpectedResults.Clear();
                        ExpectedResults.Add("p1", 5);
                        ExpectedResults.Add("p2", "{\"__metadata\":{\"type\":\"Collection(Edm.Int32)\"}, \"results\":[1]}");
                        return ctx.Execute(uri, "GET", operationParameters);
                    },
                },

                // a uri parameter containing '?' and '&' in escaped form. 
                new TestCase()
                {
                    ActualUriString = "/ServiceOpTest",
                    Parameters = new OperationParameter[] { new UriOperationParameter("p1", 5), new UriOperationParameter("p2", "%3F%26") },
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        ExpectedResults.Clear();
                        ExpectedResults.Add("p1", 5);
                        ExpectedResults.Add("p2", "%3F%26");
                        return ctx.Execute(uri, "GET", operationParameters);
                    },
                },

                // a uri parameter containing reserved characters: "?, #, $, %". Also contains %20 which represents a space.
                new TestCase()
                {
                    ActualUriString = "/ServiceOpTest",
                    Parameters = new OperationParameter[] { new UriOperationParameter("p1", 5), new UriOperationParameter("p2", "This#Has$Reserved#?Characters  %20") },
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        ExpectedResults.Clear();
                        ExpectedResults.Add("p1", 5);
                        ExpectedResults.Add("p2", "This#Has$Reserved#?Characters  %20");
                        return ctx.Execute(uri, "GET", operationParameters);
                    },
                },
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(OrderItems);
                request.StartService();

                AstoriaTest.TestUtil.RunCombinations(positiveTestCases, (testCase) =>
                {
                    DataServiceContext ctx = CreateContext(request, contextFormat);
                    Uri uri = new Uri(request.ServiceRoot + testCase.ActualUriString);

                    OperationResponse operationResponse = testCase.ExecuteMethodWithParams.Invoke(ctx, uri, false, testCase.Parameters);

                    Assert.IsNotNull(operationResponse);
                    Assert.IsNull(operationResponse.Error);
                });
            }
        }

        [TestMethod]
        public void PassUriParameterWithoutResolveNameWithJsonLight()
        {
            RunPassUriParameterWithoutResolveNameTest(ODataFormat.Json);
        }

        private void RunPassUriParameterWithoutResolveNameTest(ODataFormat contextFormat)
        {
            var testCase = new TestCase()
            {
                ActualUriString = "/ComplexTypesInUri",
                Parameters = new OperationParameter[] { new UriOperationParameter("p1", new AddressComplexType() { City = "Seattle", ZipCode = 98101, Enum1 = BasicParamValues.EnumPara1 }) }
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(OrderItems);
                request.StartService();

                DataServiceContext ctx = CreateContext(request, contextFormat);
                Uri uri = new Uri(request.ServiceRoot + testCase.ActualUriString);

                try
                {
                    ctx.Execute(uri, "GET", testCase.Parameters);
                    Assert.Fail("Test Failed");
                }
                catch (InvalidOperationException ioException)
                {
                    Assert.AreEqual(ioException.Message, DataServicesClientResourceUtil.GetString("DataServiceException_GeneralError"), "Incorrect error message");
                }
            }
        }

        #endregion Uri parameters end-to-end test (uses service operation)

        #region Null values for UriOperationParameter
        [TestMethod]
        public void ExecuteActionNullUriParameterValueTestsWithJsonLight()
        {
            // Test null values for uri parameter values with JSON Light
            RunExecuteActionNullUriParameterValueTests(ODataFormat.Json);
        }

        private void RunExecuteActionNullUriParameterValueTests(ODataFormat contextFormat)
        {
            TestCase[] positiveTestCases = new TestCase[]
            {   
                // null parameter value without typename
                new TestCase()
                {
                    ActualUriString = this.defaultUriString,
                    ExpectedBaseUriString = "/ActionWithParameters(p1=null)",
                    HttpMethod = "GET",
                    Parameters = new UriOperationParameter[] { new UriOperationParameter("p1", new ODataNullValue()) },
                    ExpectedPayloadBuilder = null,
                },
  
                // multiple null parameter value
                new TestCase()
                {
                    ActualUriString = this.defaultUriString,
                    ExpectedBaseUriString = "/ActionWithParameters(p1=null,p2=null,p3=null,p4=null)",
                    HttpMethod = "GET",
                    Parameters = new UriOperationParameter[] { new UriOperationParameter("p1", new ODataNullValue()), new UriOperationParameter("p2", new ODataNullValue()),
                        new UriOperationParameter("p3", new ODataNullValue()), new UriOperationParameter("p4", null) },
                    ExpectedPayloadBuilder = null,
                },
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.ProcessRequestOverride.Restore())
            {
                request.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                request.StartService();

                AstoriaTest.TestUtil.RunCombinations(positiveTestCases, (testCase) =>
                {
                    PlaybackService.ProcessRequestOverride.Value = (req) =>
                    {
                        Assert.AreEqual(testCase.ExpectedBaseUriString, req.RequestUriString);
                        StreamReader reader = new StreamReader(req.GetRequestStream());
                        String actualPayload = reader.ReadToEnd();
                        var expectedPayload = GetExpectedPayload(testCase, null);
                        Assert.AreEqual(expectedPayload, actualPayload);
                        req.SetResponseStatusCode(200);
                        return req;
                    };

                    DataServiceContext ctx = CreateContext(request, contextFormat);
                    Uri uri = new Uri(request.ServiceRoot + testCase.ActualUriString);
                    ctx.Execute(uri, testCase.HttpMethod, testCase.Parameters);
                });
            }
        }
        #endregion

        private static DataServiceContext CreateContext(TestWebRequest request, ODataFormat contextFormat)
        {
            DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);

            if (contextFormat == ODataFormat.Json)
            {
                ctx.Format.UseJson(new EdmModel());
            }
            return ctx;
        }

        private static string GetExpectedPayload(TestCase testCase, ODataFormat expectedPayloadFormat)
        {
            string expectedPayload;
            if (testCase.ExpectedPayloadBuilder == null)
            {
                expectedPayload = String.Empty;
            }
            else
            {
                expectedPayload = PayloadGenerator.Generate(
                    testCase.ExpectedPayloadBuilder,
                    expectedPayloadFormat,
                    payloadGeneratorSettings);
            }

            return expectedPayload;
        }
        #endregion
    }
}
