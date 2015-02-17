//---------------------------------------------------------------------
// <copyright file="Aruba.NonClr.Workspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;

    [WorkspaceAttribute("Aruba", DataLayerProviderKind.NonClr, Priority = 0, Standard = true)]
    public class ArubaNonClrWorkspace : System.Data.Test.Astoria.NonClrWorkspace
    {
        public ArubaNonClrWorkspace()
            : base("Aruba", "Aruba.NonClr", "ArubaContainer")
        {
            this.Language = WorkspaceLanguage.CSharp;
        }

        public override ServiceContainer ServiceContainer
        {
            set
            {
                _serviceContainer = value;
            }

            get
            {
                if (_serviceContainer == null)
                {

                    //Complex types code here
                    ComplexType StatusInfo = Resource.ComplexType("StatusInfo", ContextNamespace,
                            Resource.Property("Started", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                            Resource.Property("StartedBy", Clr.Types.String, NodeFacet.Nullable(true)),
                            Resource.Property("Completed", Clr.Types.DateTime, NodeFacet.Nullable(true))
                        );
                    ComplexType ContactInfo = Resource.ComplexType("ContactInfo", ContextNamespace,
                            Resource.Property("Email", Clr.Types.String, NodeFacet.Nullable(true)),
                            Resource.Property("WorkPhone", Clr.Types.String, NodeFacet.Nullable(true))
                        );
                    ComplexType AddressInfo = Resource.ComplexType("AddressInfo", ContextNamespace,
                            Resource.Property("StreetAddress", Clr.Types.String, NodeFacet.Nullable(true)),
                            Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true)),
                            Resource.Property("State", Clr.Types.String, NodeFacet.Nullable(true))
                        );
                    ComplexType ZipCode = Resource.ComplexType("ZipCode", ContextNamespace,
                            Resource.Property("MainZip", Clr.Types.String, NodeFacet.Nullable(true)),
                            Resource.Property("ExtendedZip", Clr.Types.String, NodeFacet.Nullable(true))
                        );
                    ComplexType AllTypesComplexType = Resource.ComplexType("AllTypesComplexType", ContextNamespace,
                            Resource.Property("c1_int", Clr.Types.Int32),
                            Resource.Property("c2_smallint", Clr.Types.Int16),
                            Resource.Property("c3_tinyint", Clr.Types.Byte),
                            Resource.Property("c4_bit", Clr.Types.Boolean),
                            Resource.Property("c5_datetime", Clr.Types.DateTime),
                            Resource.Property("c6_smalldatetime", Clr.Types.DateTime),
                            Resource.Property("c7_decimal_28_4_", Clr.Types.Decimal),
                            Resource.Property("c8_numeric_28_4_", Clr.Types.Decimal),
                            Resource.Property("c9_real", Clr.Types.Single),
                            Resource.Property("c10_float", Clr.Types.Double),
                            Resource.Property("c11_money", Clr.Types.Decimal),
                            Resource.Property("c12_smallmoney", Clr.Types.Decimal),
                            Resource.Property("c13_varchar_512_", Clr.Types.String),
                            Resource.Property("c14_char_512_", Clr.Types.String),
                            Resource.Property("c15_text", Clr.Types.String),
                            Resource.Property("c16_binary_512_", Clr.Types.Binary),
                            Resource.Property("c17_varbinary_512_", Clr.Types.Binary),
                            Resource.Property("c18_image", Clr.Types.Binary),
                            Resource.Property("c19_nvarchar_512_", Clr.Types.String),
                            Resource.Property("c20_nchar_512_", Clr.Types.String),
                            Resource.Property("c21_ntext", Clr.Types.String),
                            Resource.Property("c22_uniqueidentifier", Clr.Types.Guid),
                            Resource.Property("c23_bigint", Clr.Types.Int64),
                            Resource.Property("c24_sbyte", Clr.Types.SByte)
                        );

                    //Complex types that contain other complextypes code here
                    ContactInfo.Properties.Add(Resource.Property("AddressInfo", AddressInfo));
                    AddressInfo.Properties.Add(Resource.Property("ZipCode", ZipCode));


                    //Resource types here
                    ResourceType DefectBug = Resource.ResourceType("DefectBug", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Number", Clr.Types.Int32 ),
                        Resource.Property("Comment", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType Owner = Resource.ResourceType("Owner", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("FirstName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("LastName", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Alias", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType Run = Resource.ResourceType("Run", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Purpose", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("StatusInfo", StatusInfo),
                        Resource.Property("RequestStatusInfo", StatusInfo)
                    );
                    ResourceType Task = Resource.ResourceType("Task", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Deleted", Clr.Types.Boolean, NodeFacet.Nullable(true)),
                        Resource.Property("StatusInfo", StatusInfo),
                        Resource.Property("Passed", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("Failed", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("Investigates", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("Improvements", Clr.Types.Int64, NodeFacet.Nullable(true))
                    );
                    ResourceType OwnerDetail = Resource.ResourceType("OwnerDetail", ContextNamespace,
                        Resource.Property("DetailId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Level", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("HomeAddress1", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("HomeAddress2", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType ProjectBug = Resource.ResourceType("ProjectBug", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Number", Clr.Types.Int32),
                        Resource.Property("Comment", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType Scenario = Resource.ResourceType("Scenario", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType NonDefaultMappings = Resource.ResourceType("NonDefaultMappings", ContextNamespace,
                        Resource.Property("c1_int", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("c_int_AS_decimal", Clr.Types.Int32, NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_int_AS_numeric", Clr.Types.Int32, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_int_AS_float", Clr.Types.Int32),
                        Resource.Property("c_int_AS_money", Clr.Types.Int32, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("c_int_AS_bigint", Clr.Types.Int32),
                        Resource.Property("c_smallint_AS_int", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("c_smallint_AS_decimal", Clr.Types.Int16, NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_smallint_AS_numeric", Clr.Types.Int16, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_smallint_AS_real", Clr.Types.Int16),
                        Resource.Property("c_smallint_AS_float", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("c_smallint_AS_money", Clr.Types.Int16, NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("c_smallint_AS_smallmoney", Clr.Types.Int16, NodeFacet.Nullable(true), NodeFacet.Precision(10), NodeFacet.Scale(4)),
                        Resource.Property("c_smallint_AS_bigint", Clr.Types.Int16),
                        Resource.Property("c_tinyint_AS_int", Clr.Types.Byte, NodeFacet.Nullable(true)),
                        Resource.Property("c_tinyint_AS_smallint", Clr.Types.Byte),
                        Resource.Property("c_tinyint_AS_decimal", Clr.Types.Byte, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_tinyint_AS_numeric", Clr.Types.Byte, NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_tinyint_AS_real", Clr.Types.Byte, NodeFacet.Nullable(true)),
                        Resource.Property("c_tinyint_AS_float", Clr.Types.Byte),
                        Resource.Property("c_tinyint_AS_money", Clr.Types.Byte, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("c_tinyint_AS_smallmoney", Clr.Types.Byte, NodeFacet.Precision(10), NodeFacet.Scale(4)),
                        Resource.Property("c_tinyint_AS_bigint", Clr.Types.Byte, NodeFacet.Nullable(true)),
                        Resource.Property("c_smalldatetime_AS_datetime", Clr.Types.DateTime),
                        Resource.Property("c_varchar_AS_nvarchar", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c_char_AS_nchar", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c_bigint_AS_decimal", Clr.Types.Int64, NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_bigint_AS_numeric", Clr.Types.Int64, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4))
                    );
                    ResourceType Project = Resource.ResourceType("Project", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType ConfigBase = Resource.ResourceType("ConfigBase", ContextNamespace, NodeFacet.AbstractType(),
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("OS", Clr.Types.String, NodeFacet.Nullable(true)),
                        Resource.Property("Lang", Clr.Types.String, NodeFacet.Nullable(true))
                    );
                    ResourceType Config = Resource.ResourceType("Config", ContextNamespace, ConfigBase,
                        Resource.Property("Arch", Clr.Types.String, NodeFacet.Nullable(true))
                    );
                    ResourceType AllTypes = Resource.ResourceType("AllTypes", ContextNamespace,
                        Resource.Property("c1_int", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("c2_int", Clr.Types.Int32),
                        Resource.Property("c3_smallint", Clr.Types.Int16),
                        Resource.Property("c4_tinyint", Clr.Types.Byte),
                        Resource.Property("c5_bit", Clr.Types.Boolean),
                        Resource.Property("c6_datetime", Clr.Types.DateTime),
                        Resource.Property("c7_smalldatetime", Clr.Types.DateTime),
                        Resource.Property("c8_decimal_28_4_", Clr.Types.Decimal, NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c9_numeric_28_4_", Clr.Types.Decimal, NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c10_real", Clr.Types.Single),
                        Resource.Property("c11_float", Clr.Types.Double),
                        Resource.Property("c12_money", Clr.Types.Decimal, NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("c13_smallmoney", Clr.Types.Decimal, NodeFacet.Precision(10), NodeFacet.Scale(4)),
                        Resource.Property("c14_varchar_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c15_char_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c16_text", Clr.Types.String, NodeFacet.Nullable(true)),
                        Resource.Property("c17_binary_512_", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c18_varbinary_512_", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c19_image", Clr.Types.Binary, NodeFacet.Nullable(true)),
                        Resource.Property("c20_nvarchar_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c21_nchar_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c22_ntext", Clr.Types.String, NodeFacet.Nullable(true)),
                        Resource.Property("c23_uniqueidentifier", Clr.Types.Guid),
                        Resource.Property("c24_bigint", Clr.Types.Int64),
                        Resource.Property("c25_int", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("c26_smallint", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("c27_tinyint", Clr.Types.Byte, NodeFacet.Nullable(true)),
                        Resource.Property("c28_bit", Clr.Types.Boolean, NodeFacet.Nullable(true)),
                        Resource.Property("c29_datetime", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("c30_smalldatetime", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("c31_decimal_28_4_", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c32_numeric_28_4_", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c33_real", Clr.Types.Single, NodeFacet.Nullable(true)),
                        Resource.Property("c34_float", Clr.Types.Double, NodeFacet.Nullable(true)),
                        Resource.Property("c35_money", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("c36_smallmoney", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(10), NodeFacet.Scale(4)),
                        Resource.Property("c37_varchar_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c38_char_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c39_text", Clr.Types.String, NodeFacet.Nullable(true)),
                        Resource.Property("c40_binary_512_", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c41_varbinary_512_", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c42_image", Clr.Types.Binary, NodeFacet.Nullable(true)),
                        Resource.Property("c43_nvarchar_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c44_nchar_512_", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c45_ntext", Clr.Types.String, NodeFacet.Nullable(true)),
                        Resource.Property("c46_uniqueidentifier", Clr.Types.Guid, NodeFacet.Nullable(true)),
                        Resource.Property("c47_bigint", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("c48_sbyte", Clr.Types.SByte, NodeFacet.Nullable(true))
                    );
                    ResourceType OwnerContactInfo = Resource.ResourceType("OwnerContactInfo", ContextNamespace,
                        Resource.Property("ContactInfoId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ContactInfo", ContactInfo)
                    );
                    ResourceType Failure = Resource.ResourceType("Failure", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("TestId", Clr.Types.Int32),
                        Resource.Property("TestCase", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Variation", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Changed", Clr.Types.DateTime),
                        Resource.Property("Log", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType AllTypesComplexEntity = Resource.ResourceType("AllTypesComplexEntity", ContextNamespace,
                        Resource.Property("c1_int", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("MemberAllTypesComplex", AllTypesComplexType)
                    );
                    ResourceType NonDefaultFacets = Resource.ResourceType("NonDefaultFacets", ContextNamespace,
                        Resource.Property("c1_int", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("c_decimal27_3_AS_decimal28_4", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_decimal24_0_AS_decimal26_2", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(26), NodeFacet.Scale(2)),
                        Resource.Property("c_numeric24_0_AS_numeric28_4", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("c_numeric24_0_AS_numeric25_1", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(25), NodeFacet.Scale(1)),
                        Resource.Property("c_varchar230_AS_varchar512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c_varchar17_AS_varchar98", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(98)),
                        Resource.Property("c_varbinary60_AS_varbinary512", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c_varbinary31_AS_varbinary365", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(365)),
                        Resource.Property("c_varchar80_AS_nvarchar512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("c_varchar185_AS_nvarchar285", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(285))
                    );
                    ResourceType DataKey_BigInt = Resource.ResourceType("DataKey_BigInt", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int64, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10))
                    );
                    ResourceType ThirteenNavigations = Resource.ResourceType("ThirteenNavigations", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10))
                    );
                    ResourceType DataKey_Bit = Resource.ResourceType("DataKey_Bit", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Boolean, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true))
                    );
                    ResourceType DataKey_DateTime = Resource.ResourceType("DataKey_DateTime", ContextNamespace,
                        Resource.Property("Id", Clr.Types.DateTime, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))
                    );
                    ResourceType DataKey_Decimal = Resource.ResourceType("DataKey_Decimal", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Precision(16), NodeFacet.Scale(2)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10))
                    );
                    ResourceType DataKey_Float = Resource.ResourceType("DataKey_Float", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Double, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.Binary, NodeFacet.Nullable(true))
                    );
                    ResourceType DataKey_GUID = Resource.ResourceType("DataKey_GUID", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Guid, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.Binary, NodeFacet.Nullable(true))
                    );
                    ResourceType DataKey_Money = Resource.ResourceType("DataKey_Money", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10))
                    );
                    ResourceType DataKey_Numeric = Resource.ResourceType("DataKey_Numeric", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Precision(10), NodeFacet.Scale(0)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))
                    );
                    ResourceType DataKey_Real = Resource.ResourceType("DataKey_Real", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Single, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true))
                    );
                    ResourceType DataKey_SmallDateTime = Resource.ResourceType("DataKey_SmallDateTime", ContextNamespace,
                        Resource.Property("Id", Clr.Types.DateTime, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10))
                    );
                    ResourceType DataKey_SmallMoney = Resource.ResourceType("DataKey_SmallMoney", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Precision(10), NodeFacet.Scale(4)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true))
                    );
                    ResourceType DataKey_TinyInt = Resource.ResourceType("DataKey_TinyInt", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Byte, Resource.Key()),
                        Resource.Property("DataColumn", Clr.Types.Guid, NodeFacet.Nullable(true))
                    );
                    ResourceType DataKey_VarChar50 = Resource.ResourceType("DataKey_VarChar50", ContextNamespace,
                        Resource.Property("Id", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(50)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true))
                    );
                    ResourceType Person = Resource.ResourceType("Person", ContextNamespace,
                        Resource.Property("FirstName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(30)),
                        Resource.Property("LastName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(30)),
                        Resource.Property("MiddleName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(30))
                    );
                    ResourceType Vehicle = Resource.ResourceType("Vehicle", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Make", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("Model", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("Year", Clr.Types.Int32)
                    );
                    ResourceType College = Resource.ResourceType("College", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("State", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30))
                    );
                    ResourceType Student = Resource.ResourceType("Student", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("FirstName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("LastName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("MiddleName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("Major", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30))
                    );
                    ResourceType Computer = Resource.ResourceType("Computer", ContextNamespace,
                        Resource.Property("MachineName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(30)),
                        Resource.Property("Manufacturer", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("Model", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30))
                    );
                    ResourceType ComputerDetails = Resource.ResourceType("ComputerDetails", ContextNamespace,
                        Resource.Property("MachineName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(30)),
                        Resource.Property("OperatingSystem", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("OperatingSystemVersion", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("Status", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30))
                    );
                    ResourceType Worker = Resource.ResourceType("Worker", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("FirstName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("LastName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("MiddleName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30))
                    );
                    ResourceType Office = Resource.ResourceType("Office", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key() ),
                        Resource.Property("OfficeNumber", Clr.Types.Int32),
                        Resource.Property("FloorNumber", Clr.Types.Int16),
                        Resource.Property("BuildingName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("State", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("IsWindowOffice", Clr.Types.Boolean)
                    );
                    ResourceType DeepTree_A = Resource.ResourceType("DeepTree_A", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key() )
                    );
                    ResourceType WideTree_A = Resource.ResourceType("WideTree_A", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key() )
                    );
                    ResourceType Albums = Resource.ResourceType("Albums", ContextNamespace,
                        Resource.Property("albumId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("AlbumName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(100))
                    );
                    ResourceType Albums2 = Resource.ResourceType("Albums2", ContextNamespace,
                        Resource.Property("albumId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("AlbumName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(100))
                    );
                    ResourceType Albums3 = Resource.ResourceType("Albums3", ContextNamespace,
                        Resource.Property("albumId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("AlbumName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(100))
                    );
                    ResourceType Artists = Resource.ResourceType("Artists", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ArtistName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(100))
                    );
                    ResourceType Artists2 = Resource.ResourceType("Artists2", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ArtistName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(100))
                    );
                    ResourceType Artists3 = Resource.ResourceType("Artists3", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ArtistName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(100))
                    );
                    ResourceType Builds = Resource.ResourceType("Builds", ContextNamespace,
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType LabIssues = Resource.ResourceType("LabIssues", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("IssueType", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("Description", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))
                    );
                    ResourceType LabOwners = Resource.ResourceType("LabOwners", ContextNamespace,
                        Resource.Property("ownerAlias", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(10)),
                        Resource.Property("FirstName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("LastName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("Changed", Clr.Types.Binary, NodeFacet.MaxSize(8))
                    );
                    ResourceType Songs = Resource.ResourceType("Songs", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("SongName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(300))
                    );
                    ResourceType Songs2 = Resource.ResourceType("Songs2", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("SongName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(300))
                    );
                    ResourceType Songs3 = Resource.ResourceType("Songs3", ContextNamespace,
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("SongName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(300))
                    );
                    ResourceType Recordings = Resource.ResourceType("Recordings", ContextNamespace,
                        Resource.Property("SongId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ArtistId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("DateOccurred", Clr.Types.DateTime),
                        Resource.Property("OriginalSongId", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("OriginalArtistId", Clr.Types.Int32, NodeFacet.Nullable(true))
                    );
                    ResourceType Recordings2 = Resource.ResourceType("Recordings2", ContextNamespace,
                        Resource.Property("SongId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ArtistId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("DateOccurred", Clr.Types.DateTime)
                    );
                    ResourceType Recordings3 = Resource.ResourceType("Recordings3", ContextNamespace,
                        Resource.Property("SongId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ArtistId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("DateOccurred", Clr.Types.DateTime),
                        Resource.Property("OriginalSongId", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("OriginalArtistId", Clr.Types.Int32, NodeFacet.Nullable(true))
                    );
                    ResourceType Run1s = Resource.ResourceType("Run1s", ContextNamespace,
                        Resource.Property("RunId1", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Run2s = Resource.ResourceType("Run2s", ContextNamespace,
                        Resource.Property("RunId2", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Run3s = Resource.ResourceType("Run3s", ContextNamespace,
                        Resource.Property("RunId3", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(false)
                    );

                    ResourceType Test1s = Resource.ResourceType("Test1s", ContextNamespace,
                        Resource.Property("TestId1", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId1", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Test2s = Resource.ResourceType("Test2s", ContextNamespace,
                        Resource.Property("TestId2", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId1", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Test3s = Resource.ResourceType("Test3s", ContextNamespace,
                        Resource.Property("TestId3", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId1", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true) // should be false, but service cannot be started then
                    );
                    ResourceType Test4s = Resource.ResourceType("Test4s", ContextNamespace,
                        Resource.Property("TestId4", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId2", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Test5s = Resource.ResourceType("Test5s", ContextNamespace,
                        Resource.Property("TestId5", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId2", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Test6s = Resource.ResourceType("Test6s", ContextNamespace,
                        Resource.Property("TestId6", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId2", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(false)
                    );
                    ResourceType Test7s = Resource.ResourceType("Test7s", ContextNamespace,
                        Resource.Property("TestId7", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId3", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Test8s = Resource.ResourceType("Test8s", ContextNamespace,
                        Resource.Property("TestId8", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId3", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(true)
                    );
                    ResourceType Test9s = Resource.ResourceType("Test9s", ContextNamespace,
                        Resource.Property("TestId9", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RunId3", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("BuildId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50))//,
                        //NodeFacet.TopLevelAccess(false)
                    );
                    ResourceType Baseline = Resource.ResourceType("Baseline", ContextNamespace, Failure,
                        Resource.Property("Comment", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512))
                    );
                    ResourceType Uninvestigated = Resource.ResourceType("Uninvestigated", ContextNamespace, Failure
                    );
                    ResourceType PropertyOwner = Resource.ResourceType("PropertyOwner", ContextNamespace, Person,
                        Resource.Property("PropertyValue", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(18), NodeFacet.Scale(3))
                    );
                    ResourceType Car = Resource.ResourceType("Car", ContextNamespace, Vehicle
                    );
                    ResourceType Suv = Resource.ResourceType("Suv", ContextNamespace, Vehicle
                    );
                    ResourceType Truck = Resource.ResourceType("Truck", ContextNamespace, Vehicle
                    );
                    ResourceType GradStudent = Resource.ResourceType("GradStudent", ContextNamespace, Student
                    );
                    ResourceType University = Resource.ResourceType("University", ContextNamespace, College
                    );
                    ResourceType DeepTree_B = Resource.ResourceType("DeepTree_B", ContextNamespace, DeepTree_A,
                        Resource.Property("B_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_C = Resource.ResourceType("DeepTree_C", ContextNamespace, DeepTree_B,
                        Resource.Property("C_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_D = Resource.ResourceType("DeepTree_D", ContextNamespace, DeepTree_C,
                        Resource.Property("D_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_E = Resource.ResourceType("DeepTree_E", ContextNamespace, DeepTree_D,
                        Resource.Property("E_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_F = Resource.ResourceType("DeepTree_F", ContextNamespace, DeepTree_E,
                        Resource.Property("F_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_G = Resource.ResourceType("DeepTree_G", ContextNamespace, DeepTree_F,
                        Resource.Property("G_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_H = Resource.ResourceType("DeepTree_H", ContextNamespace, DeepTree_G,
                        Resource.Property("H_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_I = Resource.ResourceType("DeepTree_I", ContextNamespace, DeepTree_H,
                        Resource.Property("I_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_J = Resource.ResourceType("DeepTree_J", ContextNamespace, DeepTree_I,
                        Resource.Property("J_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_K = Resource.ResourceType("DeepTree_K", ContextNamespace, DeepTree_J,
                        Resource.Property("K_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_L = Resource.ResourceType("DeepTree_L", ContextNamespace, DeepTree_K,
                        Resource.Property("L_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_M = Resource.ResourceType("DeepTree_M", ContextNamespace, DeepTree_L,
                        Resource.Property("M_Int", Clr.Types.Int32)
                    );
                    ResourceType DeepTree_N = Resource.ResourceType("DeepTree_N", ContextNamespace, DeepTree_M,
                        Resource.Property("N_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_B = Resource.ResourceType("WideTree_B", ContextNamespace, WideTree_A,
                        Resource.Property("B_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_C = Resource.ResourceType("WideTree_C", ContextNamespace, WideTree_A,
                        Resource.Property("C_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_D = Resource.ResourceType("WideTree_D", ContextNamespace, WideTree_A,
                        Resource.Property("D_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_E = Resource.ResourceType("WideTree_E", ContextNamespace, WideTree_A,
                        Resource.Property("E_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_F = Resource.ResourceType("WideTree_F", ContextNamespace, WideTree_A,
                        Resource.Property("F_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_G = Resource.ResourceType("WideTree_G", ContextNamespace, WideTree_A,
                        Resource.Property("G_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_H = Resource.ResourceType("WideTree_H", ContextNamespace, WideTree_A,
                        Resource.Property("H_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_I = Resource.ResourceType("WideTree_I", ContextNamespace, WideTree_A,
                        Resource.Property("I_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_J = Resource.ResourceType("WideTree_J", ContextNamespace, WideTree_A,
                        Resource.Property("J_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_K = Resource.ResourceType("WideTree_K", ContextNamespace, WideTree_A,
                        Resource.Property("K_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_L = Resource.ResourceType("WideTree_L", ContextNamespace, WideTree_A,
                        Resource.Property("L_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_M = Resource.ResourceType("WideTree_M", ContextNamespace, WideTree_A,
                        Resource.Property("M_Int", Clr.Types.Int32)
                    );
                    ResourceType WideTree_N = Resource.ResourceType("WideTree_N", ContextNamespace, WideTree_A,
                        Resource.Property("N_Int", Clr.Types.Int32)
                    );
                    ResourceType TestFailure = Resource.ResourceType("TestFailure", ContextNamespace, Baseline
                    );
                    ResourceType MachineConfig = Resource.ResourceType("MachineConfig", ContextNamespace, Config,
                        Resource.Property("Host", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Address", Clr.Types.Guid, NodeFacet.Nullable(true))
                    );

                    // MEST - simple
                    ResourceType Porridge = Resource.ResourceType("Porridge", ContextNamespace,
                        Resource.Property("PorridgeId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("PorridgeName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(128)),
                        Resource.Property("Temperature", Clr.Types.Double, NodeFacet.Nullable(true))
                    );

                    ResourceType Oatmeal = Resource.ResourceType("Oatmeal", ContextNamespace, Porridge,
                        Resource.Property("WithMilk", Clr.Types.Boolean, NodeFacet.Nullable(true))
                    );

                    // MEST - associations
                    ResourceType Pet = Resource.ResourceType("Pet", ContextNamespace,
                        Resource.Property("Name", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(25))
                    );

                    ResourceType PetType_1Way = Resource.ResourceType("PetType_1Way", ContextNamespace,
                        Resource.Property("PetTypeId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("PetTypeName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType Trait_1Way = Resource.ResourceType("Trait_1Way", ContextNamespace,
                        Resource.Property("TraitId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("TraitName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType TraitDetail_1Way = Resource.ResourceType("TraitDetail_1Way", ContextNamespace,
                        Resource.Property("TraitDetailId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("TraitDetailName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType PetType = Resource.ResourceType("PetType", ContextNamespace,
                        Resource.Property("PetTypeId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("PetTypeName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType Trait = Resource.ResourceType("Trait", ContextNamespace,
                        Resource.Property("TraitId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("TraitName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType TraitDetail = Resource.ResourceType("TraitDetail", ContextNamespace,
                        Resource.Property("TraitDetailId", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("TraitDetailName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    // 1-Way assoc
                    ResourceAssociationEnd PetRole = Resource.End("Pet", Pet, Multiplicity.Zero);
                    ResourceAssociationEnd PetType_1WayRole = Resource.End("PetType_1Way", PetType_1Way, Multiplicity.Zero);
                    ResourceAssociation FK_Pet_PetType_1Way = Resource.Association("FK_Pet_Traits_1Way", PetRole, PetType_1WayRole);

                    Pet.Properties.Add(Resource.Property("PetType_1Way", PetType_1Way, FK_Pet_PetType_1Way, PetRole, PetType_1WayRole));

                    ResourceAssociationEnd PetRole2 = Resource.End("Pet", Pet, Multiplicity.Zero);
                    ResourceAssociationEnd Traits_1WayRole = Resource.End("Trait_1Way", Trait_1Way, Multiplicity.Many);
                    ResourceAssociation FK_Pet_Traits_1Way = Resource.Association("FK_Pet_Traits_1Way", PetRole2, Traits_1WayRole);

                    Pet.Properties.Add(Resource.Property("Traits_1Way", Resource.Collection(Trait_1Way), FK_Pet_Traits_1Way, PetRole, Traits_1WayRole));

                    ResourceAssociationEnd Trait_1WayRole = Resource.End("Trait_1Way", Trait_1Way, Multiplicity.Zero);
                    ResourceAssociationEnd TraitDetails_1WayRole = Resource.End("TraitDetails_1Way", TraitDetail_1Way, Multiplicity.Many);
                    ResourceAssociation FK_Trait_TraitDetails_1Way = Resource.Association("FK_Trait_TraitDetails_1Way", Trait_1WayRole, TraitDetails_1WayRole);

                    Trait_1Way.Properties.Add(Resource.Property("TraitDetails_1Way", Resource.Collection(TraitDetail_1Way), FK_Trait_TraitDetails_1Way, Trait_1WayRole, TraitDetails_1WayRole));

                    // 2-way assoc
                    ResourceAssociationEnd PetRole3 = Resource.End("Pet", Pet, Multiplicity.Zero);
                    ResourceAssociationEnd PetTypeRole = Resource.End("PetType", PetType, Multiplicity.Zero);
                    ResourceAssociation FK_Pet_PetType = Resource.Association("FK_Pet_PetType", PetRole3, PetTypeRole);

                    Pet.Properties.Add(Resource.Property("PetType", PetType, FK_Pet_PetType, PetRole, PetTypeRole));
                    PetType.Properties.Add(Resource.Property("Pet", Pet, FK_Pet_PetType, PetTypeRole, PetRole));

                    ResourceAssociationEnd PetRole4 = Resource.End("Pet", Pet, Multiplicity.Zero);
                    ResourceAssociationEnd TraitsRole = Resource.End("Trait", Trait, Multiplicity.Many);
                    ResourceAssociation FK_Pet_Traits = Resource.Association("FK_Pet_Traits", PetRole4, TraitsRole);

                    Pet.Properties.Add(Resource.Property("Traits", Resource.Collection(Trait), FK_Pet_Traits, PetRole4, TraitsRole));
                    Trait.Properties.Add(Resource.Property("Pet", Pet, FK_Pet_Traits, TraitsRole, PetRole4));

                    ResourceAssociationEnd TraitRole = Resource.End("Trait", Trait, Multiplicity.Zero);
                    ResourceAssociationEnd TraitDetailsRole = Resource.End("TraitDetails", TraitDetail, Multiplicity.Many);
                    ResourceAssociation FK_Trait_TraitDetails = Resource.Association("FK_Trait_TraitDetails", TraitRole, TraitDetailsRole);

                    Trait.Properties.Add(Resource.Property("TraitDetails", Resource.Collection(TraitDetail), FK_Trait_TraitDetails, TraitRole, TraitDetailsRole));
                    TraitDetail.Properties.Add(Resource.Property("Trait", Trait, FK_Trait_TraitDetails, TraitDetailsRole, TraitRole ));
                    // End Mest

                    //Explicity define Many to many relationships here
                    ResourceAssociationEnd FailureRole = Resource.End("Failure", Failure, Multiplicity.Many);
                    ResourceAssociationEnd ConfigRole = Resource.End("Config", Config, Multiplicity.Many);
                    ResourceAssociation FailureConfig = Resource.Association("FailureConfig", FailureRole, ConfigRole);

                    ResourceAssociationEnd TaskRole = Resource.End("Task", Task, Multiplicity.Many);
                    ResourceAssociationEnd RunRole = Resource.End("Run", Run, Multiplicity.Zero);
                    ResourceAssociation RunTasks = Resource.Association("RunTasks", TaskRole, RunRole);

                    ResourceAssociationEnd DefectBugRole = Resource.End("DefectBug", DefectBug, Multiplicity.Many);
                    ResourceAssociationEnd OwnerRole = Resource.End("Owner", Owner, Multiplicity.Zero);
                    ResourceAssociation OwnerBugsDefect_AssignedTo = Resource.Association("OwnerBugsDefect_AssignedTo", DefectBugRole, OwnerRole);

                    ResourceAssociationEnd DefectBugRole11 = Resource.End("DefectBug", DefectBug, Multiplicity.Many);
                    ResourceAssociationEnd OwnerRole11 = Resource.End("Owner", Owner, Multiplicity.Zero);
                    ResourceAssociation OwnerBugsDefect_ResolvedBy = Resource.Association("OwnerBugsDefect_ResolvedBy", DefectBugRole11, OwnerRole11);

                    ResourceAssociationEnd ProjectBugRole = Resource.End("ProjectBug", ProjectBug, Multiplicity.Many);
                    ResourceAssociationEnd OwnerRole21 = Resource.End("Owner", Owner, Multiplicity.Zero);
                    ResourceAssociation OwnerBugsProject_AssignedTo = Resource.Association("OwnerBugsProject_AssignedTo", ProjectBugRole, OwnerRole21);

                    ResourceAssociationEnd ProjectBugRole11 = Resource.End("ProjectBug", ProjectBug, Multiplicity.Many);
                    ResourceAssociationEnd OwnerRole31 = Resource.End("Owner", Owner, Multiplicity.Zero);
                    ResourceAssociation OwnerBugsProject_ResolvedBy = Resource.Association("OwnerBugsProject_ResolvedBy", ProjectBugRole11, OwnerRole31);

                    ResourceAssociationEnd DefectBugRole21 = Resource.End("DefectBug", DefectBug, Multiplicity.Many);
                    ResourceAssociationEnd FailureRole11 = Resource.End("Failure", Failure, Multiplicity.Zero);
                    ResourceAssociation FailureBugDefectTracking = Resource.Association("FailureBugDefectTracking", DefectBugRole21, FailureRole11);

                    ResourceAssociationEnd ProjectBugRole21 = Resource.End("ProjectBug", ProjectBug, Multiplicity.Many);
                    ResourceAssociationEnd FailureRole21 = Resource.End("Failure", Failure, Multiplicity.Zero);
                    ResourceAssociation FailureBugProjectTracking = Resource.Association("FailureBugProjectTracking", ProjectBugRole21, FailureRole21);

                    ResourceAssociationEnd RunRole11 = Resource.End("Run", Run, Multiplicity.Many);
                    ResourceAssociationEnd OwnerRole41 = Resource.End("Owner", Owner, Multiplicity.Zero);
                    ResourceAssociation OwnerRuns = Resource.Association("OwnerRuns", RunRole11, OwnerRole41);

                    ResourceAssociationEnd OwnerDetailRole = Resource.End("OwnerDetail", OwnerDetail, Multiplicity.Zero);
                    ResourceAssociationEnd OwnerRole51 = Resource.End("Owner", Owner, Multiplicity.Zero);
                    ResourceAssociation OwnerOwnerDetails = Resource.Association("OwnerOwnerDetails", OwnerDetailRole, OwnerRole51);

                    ResourceAssociationEnd ScenarioRole = Resource.End("Scenario", Scenario, Multiplicity.Many);
                    ResourceAssociationEnd ProjectRole = Resource.End("Project", Project, Multiplicity.Zero);
                    ResourceAssociation ProjectTestScenario = Resource.Association("ProjectTestScenario", ScenarioRole, ProjectRole);

                    ResourceAssociationEnd ScenarioRole11 = Resource.End("Scenario", Scenario, Multiplicity.Many);
                    ResourceAssociationEnd ProjectRole11 = Resource.End("Project", Project, Multiplicity.Zero);
                    ResourceAssociation ProjectDeploymentScenario = Resource.Association("ProjectDeploymentScenario", ScenarioRole11, ProjectRole11);

                    ResourceAssociationEnd DataKey_BigIntRole = Resource.End("DataKey_BigInt", DataKey_BigInt, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_BigInt = Resource.Association("FK_ThirteenNavigations_DataKey_BigInt", DataKey_BigIntRole, ThirteenNavigationsRole);

                    ResourceAssociationEnd DataKey_BitRole = Resource.End("DataKey_Bit", DataKey_Bit, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole11 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_Bit = Resource.Association("FK_ThirteenNavigations_DataKey_Bit", DataKey_BitRole, ThirteenNavigationsRole11);

                    ResourceAssociationEnd DataKey_DateTimeRole = Resource.End("DataKey_DateTime", DataKey_DateTime, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole21 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_DateTime = Resource.Association("FK_ThirteenNavigations_DataKey_DateTime", DataKey_DateTimeRole, ThirteenNavigationsRole21);

                    ResourceAssociationEnd DataKey_DecimalRole = Resource.End("DataKey_Decimal", DataKey_Decimal, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole31 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_Decimal = Resource.Association("FK_ThirteenNavigations_DataKey_Decimal", DataKey_DecimalRole, ThirteenNavigationsRole31);

                    ResourceAssociationEnd DataKey_FloatRole = Resource.End("DataKey_Float", DataKey_Float, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole41 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_Float = Resource.Association("FK_ThirteenNavigations_DataKey_Float", DataKey_FloatRole, ThirteenNavigationsRole41);

                    ResourceAssociationEnd DataKey_GUIDRole = Resource.End("DataKey_GUID", DataKey_GUID, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole51 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_GUID = Resource.Association("FK_ThirteenNavigations_DataKey_GUID", DataKey_GUIDRole, ThirteenNavigationsRole51);

                    ResourceAssociationEnd DataKey_MoneyRole = Resource.End("DataKey_Money", DataKey_Money, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole61 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_Money = Resource.Association("FK_ThirteenNavigations_DataKey_Money", DataKey_MoneyRole, ThirteenNavigationsRole61);

                    ResourceAssociationEnd DataKey_NumericRole = Resource.End("DataKey_Numeric", DataKey_Numeric, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole71 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_Numeric = Resource.Association("FK_ThirteenNavigations_DataKey_Numeric", DataKey_NumericRole, ThirteenNavigationsRole71);

                    ResourceAssociationEnd DataKey_RealRole = Resource.End("DataKey_Real", DataKey_Real, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole81 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_Real = Resource.Association("FK_ThirteenNavigations_DataKey_Real", DataKey_RealRole, ThirteenNavigationsRole81);

                    ResourceAssociationEnd DataKey_SmallDateTimeRole = Resource.End("DataKey_SmallDateTime", DataKey_SmallDateTime, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole91 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_SmallDateTime = Resource.Association("FK_ThirteenNavigations_DataKey_SmallDateTime", DataKey_SmallDateTimeRole, ThirteenNavigationsRole91);

                    ResourceAssociationEnd DataKey_SmallMoneyRole = Resource.End("DataKey_SmallMoney", DataKey_SmallMoney, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole101 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_SmallMoney = Resource.Association("FK_ThirteenNavigations_DataKey_SmallMoney", DataKey_SmallMoneyRole, ThirteenNavigationsRole101);

                    ResourceAssociationEnd DataKey_TinyIntRole = Resource.End("DataKey_TinyInt", DataKey_TinyInt, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole111 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_TinyInt = Resource.Association("FK_ThirteenNavigations_DataKey_TinyInt", DataKey_TinyIntRole, ThirteenNavigationsRole111);

                    ResourceAssociationEnd DataKey_VarChar50Role = Resource.End("DataKey_VarChar50", DataKey_VarChar50, Multiplicity.Zero);
                    ResourceAssociationEnd ThirteenNavigationsRole121 = Resource.End("ThirteenNavigations", ThirteenNavigations, Multiplicity.Many);
                    ResourceAssociation FK_ThirteenNavigations_DataKey_VarChar50 = Resource.Association("FK_ThirteenNavigations_DataKey_VarChar50", DataKey_VarChar50Role, ThirteenNavigationsRole121);

                    ResourceAssociationEnd PersonRole = Resource.End("Person", Person, Multiplicity.Zero);
                    ResourceAssociationEnd VehicleRole = Resource.End("Vehicle", Vehicle, Multiplicity.Zero);
                    ResourceAssociation FK_Person_Vehicle = Resource.Association("FK_Person_Vehicle", PersonRole, VehicleRole);

                    ResourceAssociationEnd PersonRole11 = Resource.End("Person", Person, Multiplicity.Zero);
                    ResourceAssociationEnd OldVehiclesRole = Resource.End("OldVehicles", Vehicle, Multiplicity.Many);
                    ResourceAssociation FK_Person_OldVehicles = Resource.Association("FK_Person_OldVehicles", PersonRole11, OldVehiclesRole);

                    ResourceAssociationEnd ComputerRole = Resource.End("Computer", Computer, Multiplicity.Zero);
                    ResourceAssociationEnd ComputerDetailsRole = Resource.End("ComputerDetails", ComputerDetails, Multiplicity.Zero);
                    ResourceAssociation FK_Computer_ComputerDetails = Resource.Association("FK_Computer_ComputerDetails", ComputerRole, ComputerDetailsRole);

                    ResourceAssociationEnd StudentsRole = Resource.End("Students", Student, Multiplicity.Many);
                    ResourceAssociationEnd CollegeRole = Resource.End("College", College, Multiplicity.Zero);
                    ResourceAssociation FK_Students_College = Resource.Association("FK_Students_College", StudentsRole, CollegeRole);

                    ResourceAssociationEnd WorkerRole = Resource.End("Worker", Worker, Multiplicity.Zero);
                    ResourceAssociationEnd OfficeRole = Resource.End("Office", Office, Multiplicity.Zero);
                    ResourceAssociation FK_Worker_Office = Resource.Association("FK_Worker_Office", WorkerRole, OfficeRole);

                    ResourceAssociationEnd WorkerRole11 = Resource.End("Worker", Worker, Multiplicity.Zero);
                    ResourceAssociationEnd MentorRole = Resource.End("Mentor", Worker, Multiplicity.Zero);
                    ResourceAssociation FK_Worker_Mentor = Resource.Association("FK_Worker_Mentor", WorkerRole11, MentorRole);

                    ResourceAssociationEnd ArtistsRole = Resource.End("Artists", Artists, Multiplicity.Zero);
                    ResourceAssociationEnd RecordingsRole = Resource.End("Recordings", Recordings, Multiplicity.Many);
                    ResourceAssociation FK_Recordings_Artists = Resource.Association("FK_Recordings_Artists", ArtistsRole, RecordingsRole);

                    ResourceAssociationEnd Artists2Role = Resource.End("Artists2", Artists2, Multiplicity.Zero);
                    ResourceAssociationEnd Recordings2Role = Resource.End("Recordings2", Recordings2, Multiplicity.Many);
                    ResourceAssociation FK_Recordings2_Artists = Resource.Association("FK_Recordings2_Artists", Artists2Role, Recordings2Role);

                    ResourceAssociationEnd Artists3Role = Resource.End("Artists3", Artists3, Multiplicity.Zero);
                    ResourceAssociationEnd Recordings3Role = Resource.End("Recordings3", Recordings3, Multiplicity.Many);
                    ResourceAssociation FK_Recordings3_Artists3 = Resource.Association("FK_Recordings3_Artists3", Artists3Role, Recordings3Role);

                    ResourceAssociationEnd LabOwnersRole = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd BuildsRole = Resource.End("Builds", Builds, Multiplicity.Many);
                    ResourceAssociation FK_Builds_LabOwners = Resource.Association("FK_Builds_LabOwners", LabOwnersRole, BuildsRole);

                    ResourceAssociationEnd BuildsRole11 = Resource.End("Builds", Builds, Multiplicity.Zero);
                    ResourceAssociationEnd Run1sRole = Resource.End("Run1s", Run1s, Multiplicity.Many);
                    ResourceAssociation FK_Run1s_Builds = Resource.Association("FK_Run1s_Builds", BuildsRole11, Run1sRole);

                    ResourceAssociationEnd BuildsRole21 = Resource.End("Builds", Builds, Multiplicity.Zero);
                    ResourceAssociationEnd Run2sRole = Resource.End("Run2s", Run2s, Multiplicity.Many);
                    ResourceAssociation FK_Run2s_Builds = Resource.Association("FK_Run2s_Builds", BuildsRole21, Run2sRole);

                    ResourceAssociationEnd BuildsRole31 = Resource.End("Builds", Builds, Multiplicity.Zero);
                    ResourceAssociationEnd Run3sRole = Resource.End("Run3s", Run3s, Multiplicity.Many);
                    ResourceAssociation FK_Run3s_Builds = Resource.Association("FK_Run3s_Builds", BuildsRole31, Run3sRole);

                    ResourceAssociationEnd LabOwnersRole11 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd LabIssuesRole = Resource.End("LabIssues", LabIssues, Multiplicity.Many);
                    ResourceAssociation FK_LabIssues_LabOwners = Resource.Association("FK_LabIssues_LabOwners", LabOwnersRole11, LabIssuesRole);

                    ResourceAssociationEnd LabOwnersRole21 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Run1sRole11 = Resource.End("Run1s", Run1s, Multiplicity.Many);
                    ResourceAssociation FK_Run1s_LabOwners = Resource.Association("FK_Run1s_LabOwners", LabOwnersRole21, Run1sRole11);

                    ResourceAssociationEnd LabOwnersRole31 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Run2sRole11 = Resource.End("Run2s", Run2s, Multiplicity.Many);
                    ResourceAssociation FK_Run2s_LabOwners = Resource.Association("FK_Run2s_LabOwners", LabOwnersRole31, Run2sRole11);

                    ResourceAssociationEnd LabOwnersRole41 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Run3sRole11 = Resource.End("Run3s", Run3s, Multiplicity.Many);
                    ResourceAssociation FK_Run3s_LabOwners = Resource.Association("FK_Run3s_LabOwners", LabOwnersRole41, Run3sRole11);

                    ResourceAssociationEnd LabOwnersRole51 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test1sRole = Resource.End("Test1s", Test1s, Multiplicity.Many);
                    ResourceAssociation FK_Test1s_LabOwners = Resource.Association("FK_Test1s_LabOwners", LabOwnersRole51, Test1sRole);

                    ResourceAssociationEnd LabOwnersRole61 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test2sRole = Resource.End("Test2s", Test2s, Multiplicity.Many);
                    ResourceAssociation FK_Test2s_LabOwners = Resource.Association("FK_Test2s_LabOwners", LabOwnersRole61, Test2sRole);

                    ResourceAssociationEnd LabOwnersRole71 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test3sRole = Resource.End("Test3s", Test3s, Multiplicity.Many);
                    ResourceAssociation FK_Test3s_LabOwners = Resource.Association("FK_Test3s_LabOwners", LabOwnersRole71, Test3sRole);

                    ResourceAssociationEnd LabOwnersRole81 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test4sRole = Resource.End("Test4s", Test4s, Multiplicity.Many);
                    ResourceAssociation FK_Test4s_LabOwners = Resource.Association("FK_Test4s_LabOwners", LabOwnersRole81, Test4sRole);

                    ResourceAssociationEnd LabOwnersRole91 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test5sRole = Resource.End("Test5s", Test5s, Multiplicity.Many);
                    ResourceAssociation FK_Test5s_LabOwners = Resource.Association("FK_Test5s_LabOwners", LabOwnersRole91, Test5sRole);

                    ResourceAssociationEnd LabOwnersRole101 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test6sRole = Resource.End("Test6s", Test6s, Multiplicity.Many);
                    ResourceAssociation FK_Test6s_LabOwners = Resource.Association("FK_Test6s_LabOwners", LabOwnersRole101, Test6sRole);

                    ResourceAssociationEnd LabOwnersRole111 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test7sRole = Resource.End("Test7s", Test7s, Multiplicity.Many);
                    ResourceAssociation FK_Test7s_LabOwners = Resource.Association("FK_Test7s_LabOwners", LabOwnersRole111, Test7sRole);

                    ResourceAssociationEnd LabOwnersRole121 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test8sRole = Resource.End("Test8s", Test8s, Multiplicity.Many);
                    ResourceAssociation FK_Test8s_LabOwners = Resource.Association("FK_Test8s_LabOwners", LabOwnersRole121, Test8sRole);

                    ResourceAssociationEnd LabOwnersRole131 = Resource.End("LabOwners", LabOwners, Multiplicity.Zero);
                    ResourceAssociationEnd Test9sRole = Resource.End("Test9s", Test9s, Multiplicity.Many);
                    ResourceAssociation FK_Test9s_LabOwners = Resource.Association("FK_Test9s_LabOwners", LabOwnersRole131, Test9sRole);

                    ResourceAssociationEnd SongsRole = Resource.End("Songs", Songs, Multiplicity.Zero);
                    ResourceAssociationEnd RecordingsRole11 = Resource.End("Recordings", Recordings, Multiplicity.Many);
                    ResourceAssociation FK_Recordings_Songs = Resource.Association("FK_Recordings_Songs", SongsRole, RecordingsRole11);

                    ResourceAssociationEnd Recordings2Role11 = Resource.End("Recordings2", Recordings2, Multiplicity.Zero);
                    ResourceAssociationEnd Recordings21Role = Resource.End("Recordings21", Recordings2, Multiplicity.Many);
                    ResourceAssociation FK_Recordings2_Recordings2 = Resource.Association("FK_Recordings2_Recordings2", Recordings2Role11, Recordings21Role);

                    ResourceAssociationEnd Songs2Role = Resource.End("Songs2", Songs2, Multiplicity.Zero);
                    ResourceAssociationEnd Recordings2Role21 = Resource.End("Recordings2", Recordings2, Multiplicity.Many);
                    ResourceAssociation FK_Recordings2_Songs2 = Resource.Association("FK_Recordings2_Songs2", Songs2Role, Recordings2Role21);

                    ResourceAssociationEnd Songs3Role = Resource.End("Songs3", Songs3, Multiplicity.Zero);
                    ResourceAssociationEnd Recordings3Role11 = Resource.End("Recordings3", Recordings3, Multiplicity.Many);
                    ResourceAssociation FK_Recordings3_Songs3 = Resource.Association("FK_Recordings3_Songs3", Songs3Role, Recordings3Role11);

                    ResourceAssociationEnd Run1sRole21 = Resource.End("Run1s", Run1s, Multiplicity.Zero);
                    ResourceAssociationEnd Test1sRole11 = Resource.End("Test1s", Test1s, Multiplicity.Many);
                    ResourceAssociation FK_Test1s_Run1s = Resource.Association("FK_Test1s_Run1s", Run1sRole21, Test1sRole11);

                    ResourceAssociationEnd Run1sRole31 = Resource.End("Run1s", Run1s, Multiplicity.Zero);
                    ResourceAssociationEnd Test2sRole11 = Resource.End("Test2s", Test2s, Multiplicity.Many);
                    ResourceAssociation FK_Test2s_Run1s = Resource.Association("FK_Test2s_Run1s", Run1sRole31, Test2sRole11);

                    ResourceAssociationEnd Run1sRole41 = Resource.End("Run1s", Run1s, Multiplicity.Zero);
                    ResourceAssociationEnd Test3sRole11 = Resource.End("Test3s", Test3s, Multiplicity.Many);
                    ResourceAssociation FK_Test3s_Run1s = Resource.Association("FK_Test3s_Run1s", Run1sRole41, Test3sRole11);

                    ResourceAssociationEnd Run2sRole21 = Resource.End("Run2s", Run2s, Multiplicity.Zero);
                    ResourceAssociationEnd Test4sRole11 = Resource.End("Test4s", Test4s, Multiplicity.Many);
                    ResourceAssociation FK_Test4s_Run2s = Resource.Association("FK_Test4s_Run2s", Run2sRole21, Test4sRole11);

                    ResourceAssociationEnd Run2sRole31 = Resource.End("Run2s", Run2s, Multiplicity.Zero);
                    ResourceAssociationEnd Test5sRole11 = Resource.End("Test5s", Test5s, Multiplicity.Many);
                    ResourceAssociation FK_Test5s_Run2s = Resource.Association("FK_Test5s_Run2s", Run2sRole31, Test5sRole11);

                    ResourceAssociationEnd Run2sRole41 = Resource.End("Run2s", Run2s, Multiplicity.Zero);
                    ResourceAssociationEnd Test6sRole11 = Resource.End("Test6s", Test6s, Multiplicity.Many);
                    ResourceAssociation FK_Test6s_Run2s = Resource.Association("FK_Test6s_Run2s", Run2sRole41, Test6sRole11);

                    ResourceAssociationEnd Run3sRole21 = Resource.End("Run3s", Run3s, Multiplicity.Zero);
                    ResourceAssociationEnd Test7sRole11 = Resource.End("Test7s", Test7s, Multiplicity.Many);
                    ResourceAssociation FK_Test7s_Run3s = Resource.Association("FK_Test7s_Run3s", Run3sRole21, Test7sRole11);

                    ResourceAssociationEnd Run3sRole31 = Resource.End("Run3s", Run3s, Multiplicity.Zero);
                    ResourceAssociationEnd Test8sRole11 = Resource.End("Test8s", Test8s, Multiplicity.Many);
                    ResourceAssociation FK_Test8s_Run3s = Resource.Association("FK_Test8s_Run3s", Run3sRole31, Test8sRole11);

                    ResourceAssociationEnd Run3sRole41 = Resource.End("Run3s", Run3s, Multiplicity.Zero);
                    ResourceAssociationEnd Test9sRole11 = Resource.End("Test9s", Test9s, Multiplicity.Many);
                    ResourceAssociation FK_Test9s_Run3s = Resource.Association("FK_Test9s_Run3s", Run3sRole41, Test9sRole11);

                    ResourceAssociationEnd AlbumsRole = Resource.End("Albums", Albums, Multiplicity.Many);
                    ResourceAssociationEnd RecordingsRole21 = Resource.End("Recordings", Recordings, Multiplicity.Many);
                    ResourceAssociation RecordingAlbumsLinkTable = Resource.Association("RecordingAlbumsLinkTable", AlbumsRole, RecordingsRole21);

                    ResourceAssociationEnd Albums2Role = Resource.End("Albums2", Albums2, Multiplicity.Many);
                    ResourceAssociationEnd Recordings2Role31 = Resource.End("Recordings2", Recordings2, Multiplicity.Many);
                    ResourceAssociation RecordingAlbumsLinkTable2 = Resource.Association("RecordingAlbumsLinkTable2", Albums2Role, Recordings2Role31);

                    ResourceAssociationEnd Albums3Role = Resource.End("Albums3", Albums3, Multiplicity.Many);
                    ResourceAssociationEnd Recordings3Role21 = Resource.End("Recordings3", Recordings3, Multiplicity.Many);
                    ResourceAssociation RecordingAlbumsLinkTable3 = Resource.Association("RecordingAlbumsLinkTable3", Albums3Role, Recordings3Role21);


                    //Resource navigation properties added here
                    Task.Properties.Add(Resource.Property("Run", Run, RunTasks, TaskRole, RunRole));

                    ResourceProperty newProperty = Resource.Property("TestScenario", Resource.Collection(Scenario), ProjectTestScenario, ProjectRole, ScenarioRole);
                    newProperty.Facets.MestTag = "TestScenarioSet";
                    Project.Properties.Add(newProperty);

                    newProperty = Resource.Property("DeploymentScenario", Resource.Collection(Scenario), ProjectDeploymentScenario, ProjectRole11, ScenarioRole11);
                    newProperty.Facets.MestTag = "DeploymentScenarioSet";
                    Project.Properties.Add(newProperty);

                    Run.Properties.Add(Resource.Property("Tasks", Resource.Collection(Task), RunTasks, RunRole, TaskRole));
                    Run.Properties.Add(Resource.Property("Owner", Owner, OwnerRuns, RunRole11, OwnerRole41));

                    OwnerDetail.Properties.Add(Resource.Property("Owner", Owner, OwnerOwnerDetails, OwnerDetailRole, OwnerRole51));

                    Owner.Properties.Add(Resource.Property("Runs", Resource.Collection(Run), OwnerRuns, OwnerRole41, RunRole11));
                    Owner.Properties.Add(Resource.Property("AssignedToBugsDefect", Resource.Collection(DefectBug), OwnerBugsDefect_AssignedTo, OwnerRole, DefectBugRole));
                    Owner.Properties.Add(Resource.Property("ResolvedBugsDefect", Resource.Collection(DefectBug), OwnerBugsDefect_ResolvedBy, OwnerRole11, DefectBugRole11));
                    Owner.Properties.Add(Resource.Property("OwnerDetail", OwnerDetail, NodeFacet.Nullable(true), OwnerOwnerDetails, OwnerRole51, OwnerDetailRole));
                    Owner.Properties.Add(Resource.Property("AssignedToBugsProject", Resource.Collection(ProjectBug), OwnerBugsProject_AssignedTo, OwnerRole21, ProjectBugRole));
                    Owner.Properties.Add(Resource.Property("ResolvedBugsProject", Resource.Collection(ProjectBug), OwnerBugsProject_ResolvedBy, OwnerRole31, ProjectBugRole11));

                    DefectBug.Properties.Add(Resource.Property("AssignedToOwnerBugsDefect", Owner, OwnerBugsDefect_AssignedTo, DefectBugRole, OwnerRole));
                    DefectBug.Properties.Add(Resource.Property("ResolvedOwnerBugsDefect", Owner, NodeFacet.Nullable(true), OwnerBugsDefect_ResolvedBy, DefectBugRole11, OwnerRole11));

                    ProjectBug.Properties.Add(Resource.Property("AssignedToOwnerBugsProject", Owner, OwnerBugsProject_AssignedTo, ProjectBugRole, OwnerRole21));
                    ProjectBug.Properties.Add(Resource.Property("ResolvedOwnerBugsProject", Owner, NodeFacet.Nullable(true), OwnerBugsProject_ResolvedBy, ProjectBugRole11, OwnerRole31));

                    Config.Properties.Add(Resource.Property("Failures", Resource.Collection(Failure), FailureConfig, ConfigRole, FailureRole));

                    Failure.Properties.Add(Resource.Property("BugsDefect", Resource.Collection(DefectBug), FailureBugDefectTracking, FailureRole11, DefectBugRole21));
                    Failure.Properties.Add(Resource.Property("Configs", Resource.Collection(Config), FailureConfig, FailureRole, ConfigRole));
                    Failure.Properties.Add(Resource.Property("BugsProject", Resource.Collection(ProjectBug), FailureBugProjectTracking, FailureRole21, ProjectBugRole21));

                    DataKey_BigInt.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_BigInt, DataKey_BigIntRole, ThirteenNavigationsRole));

                    DataKey_Bit.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_Bit, DataKey_BitRole, ThirteenNavigationsRole11));

                    DataKey_DateTime.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_DateTime, DataKey_DateTimeRole, ThirteenNavigationsRole21));

                    DataKey_Decimal.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_Decimal, DataKey_DecimalRole, ThirteenNavigationsRole31));

                    DataKey_Float.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_Float, DataKey_FloatRole, ThirteenNavigationsRole41));

                    DataKey_GUID.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_GUID, DataKey_GUIDRole, ThirteenNavigationsRole51));

                    DataKey_Money.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_Money, DataKey_MoneyRole, ThirteenNavigationsRole61));

                    DataKey_Numeric.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_Numeric, DataKey_NumericRole, ThirteenNavigationsRole71));

                    DataKey_Real.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_Real, DataKey_RealRole, ThirteenNavigationsRole81));

                    DataKey_SmallDateTime.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_SmallDateTime, DataKey_SmallDateTimeRole, ThirteenNavigationsRole91));

                    DataKey_SmallMoney.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_SmallMoney, DataKey_SmallMoneyRole, ThirteenNavigationsRole101));

                    DataKey_TinyInt.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_TinyInt, DataKey_TinyIntRole, ThirteenNavigationsRole111));

                    DataKey_VarChar50.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations), FK_ThirteenNavigations_DataKey_VarChar50, DataKey_VarChar50Role, ThirteenNavigationsRole121));

                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_BigInt", DataKey_BigInt, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_BigInt, ThirteenNavigationsRole, DataKey_BigIntRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Bit", DataKey_Bit, FK_ThirteenNavigations_DataKey_Bit, ThirteenNavigationsRole11, DataKey_BitRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_DateTime", DataKey_DateTime, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_DateTime, ThirteenNavigationsRole21, DataKey_DateTimeRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Decimal", DataKey_Decimal, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_Decimal, ThirteenNavigationsRole31, DataKey_DecimalRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Float", DataKey_Float, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_Float, ThirteenNavigationsRole41, DataKey_FloatRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_GUID", DataKey_GUID, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_GUID, ThirteenNavigationsRole51, DataKey_GUIDRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Money", DataKey_Money, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_Money, ThirteenNavigationsRole61, DataKey_MoneyRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Numeric", DataKey_Numeric, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_Numeric, ThirteenNavigationsRole71, DataKey_NumericRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Real", DataKey_Real, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_Real, ThirteenNavigationsRole81, DataKey_RealRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_SmallDateTime", DataKey_SmallDateTime, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_SmallDateTime, ThirteenNavigationsRole91, DataKey_SmallDateTimeRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_SmallMoney", DataKey_SmallMoney, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_SmallMoney, ThirteenNavigationsRole101, DataKey_SmallMoneyRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_TinyInt", DataKey_TinyInt, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_TinyInt, ThirteenNavigationsRole111, DataKey_TinyIntRole));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_VarChar50", DataKey_VarChar50, NodeFacet.Nullable(true), FK_ThirteenNavigations_DataKey_VarChar50, ThirteenNavigationsRole121, DataKey_VarChar50Role));

                    Person.Properties.Add(Resource.Property("PrimaryVehicle", Vehicle, FK_Person_Vehicle, PersonRole, VehicleRole));
                    Person.Properties.Add(Resource.Property("OldVehicles", Resource.Collection(Vehicle), FK_Person_OldVehicles, PersonRole11, OldVehiclesRole));

                    Vehicle.Properties.Add(Resource.Property("PrimaryDriver", Person, NodeFacet.Nullable(true), FK_Person_Vehicle, VehicleRole, PersonRole));
                    Vehicle.Properties.Add(Resource.Property("OldDrivers", Person, NodeFacet.Nullable(true), FK_Person_OldVehicles, OldVehiclesRole, PersonRole11));

                    College.Properties.Add(Resource.Property("Students", Resource.Collection(Student), FK_Students_College, CollegeRole, StudentsRole));

                    Student.Properties.Add(Resource.Property("College", College, FK_Students_College, StudentsRole, CollegeRole));

                    Computer.Properties.Add(Resource.Property("ExtraDetails", ComputerDetails, FK_Computer_ComputerDetails, ComputerRole, ComputerDetailsRole));

                    ComputerDetails.Properties.Add(Resource.Property("MainRecord", Computer, FK_Computer_ComputerDetails, ComputerDetailsRole, ComputerRole));

                    Worker.Properties.Add(Resource.Property("Mentor", Worker, FK_Worker_Mentor, WorkerRole11, MentorRole));
                    Worker.Properties.Add(Resource.Property("Office", Office, NodeFacet.Nullable(true), FK_Worker_Office, WorkerRole, OfficeRole));

                    Albums.Properties.Add(Resource.Property("Recordings", Resource.Collection(Recordings), RecordingAlbumsLinkTable, AlbumsRole, RecordingsRole21));

                    Albums2.Properties.Add(Resource.Property("Recordings2", Resource.Collection(Recordings2), RecordingAlbumsLinkTable2, Albums2Role, Recordings2Role31));

                    Albums3.Properties.Add(Resource.Property("Recordings3", Resource.Collection(Recordings3), RecordingAlbumsLinkTable3, Albums3Role, Recordings3Role21));

                    Artists.Properties.Add(Resource.Property("Recordings", Resource.Collection(Recordings), FK_Recordings_Artists, ArtistsRole, RecordingsRole));

                    Artists2.Properties.Add(Resource.Property("Recordings2", Resource.Collection(Recordings2), FK_Recordings2_Artists, Artists2Role, Recordings2Role));

                    Artists3.Properties.Add(Resource.Property("Recordings3", Resource.Collection(Recordings3), FK_Recordings3_Artists3, Artists3Role, Recordings3Role));

                    Builds.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Builds_LabOwners, BuildsRole, LabOwnersRole));
                    Builds.Properties.Add(Resource.Property("Run1s", Resource.Collection(Run1s), /*NodeFacet.CanonicalAccessPath(true),*/ FK_Run1s_Builds, BuildsRole11, Run1sRole));
                    Builds.Properties.Add(Resource.Property("Run2s", Resource.Collection(Run2s), /*NodeFacet.CanonicalAccessPath(false),*/ FK_Run2s_Builds, BuildsRole21, Run2sRole));
                    Builds.Properties.Add(Resource.Property("Run3s", Resource.Collection(Run3s), /*NodeFacet.CanonicalAccessPath(true),*/ FK_Run3s_Builds, BuildsRole31, Run3sRole));

                    LabIssues.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_LabIssues_LabOwners, LabIssuesRole, LabOwnersRole11));

                    LabOwners.Properties.Add(Resource.Property("Builds", Resource.Collection(Builds), FK_Builds_LabOwners, LabOwnersRole, BuildsRole));
                    LabOwners.Properties.Add(Resource.Property("LabIssues", Resource.Collection(LabIssues), FK_LabIssues_LabOwners, LabOwnersRole11, LabIssuesRole));
                    LabOwners.Properties.Add(Resource.Property("Run1s", Resource.Collection(Run1s), FK_Run1s_LabOwners, LabOwnersRole21, Run1sRole11));
                    LabOwners.Properties.Add(Resource.Property("Run2s", Resource.Collection(Run2s), FK_Run2s_LabOwners, LabOwnersRole31, Run2sRole11));
                    LabOwners.Properties.Add(Resource.Property("Run3s", Resource.Collection(Run3s), FK_Run3s_LabOwners, LabOwnersRole41, Run3sRole11));
                    LabOwners.Properties.Add(Resource.Property("Test1s", Resource.Collection(Test1s), FK_Test1s_LabOwners, LabOwnersRole51, Test1sRole));
                    LabOwners.Properties.Add(Resource.Property("Test2s", Resource.Collection(Test2s), FK_Test2s_LabOwners, LabOwnersRole61, Test2sRole));
                    LabOwners.Properties.Add(Resource.Property("Test3s", Resource.Collection(Test3s), FK_Test3s_LabOwners, LabOwnersRole71, Test3sRole));
                    LabOwners.Properties.Add(Resource.Property("Test4s", Resource.Collection(Test4s), FK_Test4s_LabOwners, LabOwnersRole81, Test4sRole));
                    LabOwners.Properties.Add(Resource.Property("Test5s", Resource.Collection(Test5s), FK_Test5s_LabOwners, LabOwnersRole91, Test5sRole));
                    LabOwners.Properties.Add(Resource.Property("Test6s", Resource.Collection(Test6s), FK_Test6s_LabOwners, LabOwnersRole101, Test6sRole));
                    LabOwners.Properties.Add(Resource.Property("Test7s", Resource.Collection(Test7s), FK_Test7s_LabOwners, LabOwnersRole111, Test7sRole));
                    LabOwners.Properties.Add(Resource.Property("Test8s", Resource.Collection(Test8s), FK_Test8s_LabOwners, LabOwnersRole121, Test8sRole));
                    LabOwners.Properties.Add(Resource.Property("Test9s", Resource.Collection(Test9s), FK_Test9s_LabOwners, LabOwnersRole131, Test9sRole));

                    Recordings.Properties.Add(Resource.Property("Artists", Artists, FK_Recordings_Artists, RecordingsRole, ArtistsRole));
                    Recordings.Properties.Add(Resource.Property("Songs", Songs, FK_Recordings_Songs, RecordingsRole11, SongsRole));
                    Recordings.Properties.Add(Resource.Property("Albums", Resource.Collection(Albums), RecordingAlbumsLinkTable, RecordingsRole21, AlbumsRole));

                    Recordings2.Properties.Add(Resource.Property("Artists2", Artists2, FK_Recordings2_Artists, Recordings2Role, Artists2Role));
                    Recordings2.Properties.Add(Resource.Property("Recordings21", Resource.Collection(Recordings2), FK_Recordings2_Recordings2, Recordings2Role11, Recordings21Role));
                    Recordings2.Properties.Add(Resource.Property("Recordings22", Recordings2, NodeFacet.Nullable(true), FK_Recordings2_Recordings2, Recordings21Role, Recordings2Role11));
                    Recordings2.Properties.Add(Resource.Property("Songs2", Songs2, FK_Recordings2_Songs2, Recordings2Role21, Songs2Role));
                    Recordings2.Properties.Add(Resource.Property("Albums2", Resource.Collection(Albums2), RecordingAlbumsLinkTable2, Recordings2Role31, Albums2Role));

                    Recordings3.Properties.Add(Resource.Property("Artists3", Artists3, FK_Recordings3_Artists3, Recordings3Role, Artists3Role));
                    Recordings3.Properties.Add(Resource.Property("Songs3", Songs3, FK_Recordings3_Songs3, Recordings3Role11, Songs3Role));
                    Recordings3.Properties.Add(Resource.Property("Albums3", Resource.Collection(Albums3), RecordingAlbumsLinkTable3, Recordings3Role21, Albums3Role));

                    Run1s.Properties.Add(Resource.Property("Builds", Builds, FK_Run1s_Builds, Run1sRole, BuildsRole11));
                    Run1s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Run1s_LabOwners, Run1sRole11, LabOwnersRole21));
                    Run1s.Properties.Add(Resource.Property("Test1s", Resource.Collection(Test1s), /*NodeFacet.CanonicalAccessPath(true),*/ FK_Test1s_Run1s, Run1sRole21, Test1sRole11));
                    Run1s.Properties.Add(Resource.Property("Test2s", Resource.Collection(Test2s), /*NodeFacet.CanonicalAccessPath(false),*/ FK_Test2s_Run1s, Run1sRole31, Test2sRole11));
                    Run1s.Properties.Add(Resource.Property("Test3s", Resource.Collection(Test3s), /*NodeFacet.CanonicalAccessPath(false),*/ FK_Test3s_Run1s, Run1sRole41, Test3sRole11));

                    Run2s.Properties.Add(Resource.Property("Builds", Builds, FK_Run2s_Builds, Run2sRole, BuildsRole21));
                    Run2s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Run2s_LabOwners, Run2sRole11, LabOwnersRole31));
                    Run2s.Properties.Add(Resource.Property("Test4s", Resource.Collection(Test4s), /*NodeFacet.CanonicalAccessPath(true),*/ FK_Test4s_Run2s, Run2sRole21, Test4sRole11));
                    Run2s.Properties.Add(Resource.Property("Test5s", Resource.Collection(Test5s), /*NodeFacet.CanonicalAccessPath(false),*/ FK_Test5s_Run2s, Run2sRole31, Test5sRole11));
                    Run2s.Properties.Add(Resource.Property("Test6s", Resource.Collection(Test6s), /*NodeFacet.CanonicalAccessPath(true),*/ FK_Test6s_Run2s, Run2sRole41, Test6sRole11));

                    Run3s.Properties.Add(Resource.Property("Builds", Builds, FK_Run3s_Builds, Run3sRole, BuildsRole31));
                    Run3s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Run3s_LabOwners, Run3sRole11, LabOwnersRole41));
                    Run3s.Properties.Add(Resource.Property("Test7s", Resource.Collection(Test7s), /*NodeFacet.CanonicalAccessPath(true),*/ FK_Test7s_Run3s, Run3sRole21, Test7sRole11));
                    Run3s.Properties.Add(Resource.Property("Test8s", Resource.Collection(Test8s), /*NodeFacet.CanonicalAccessPath(false),*/ FK_Test8s_Run3s, Run3sRole31, Test8sRole11));
                    Run3s.Properties.Add(Resource.Property("Test9s", Resource.Collection(Test9s), /*NodeFacet.CanonicalAccessPath(true),*/ FK_Test9s_Run3s, Run3sRole41, Test9sRole11));

                    Songs.Properties.Add(Resource.Property("Recordings", Resource.Collection(Recordings), FK_Recordings_Songs, SongsRole, RecordingsRole11));

                    Songs2.Properties.Add(Resource.Property("Recordings2", Resource.Collection(Recordings2), FK_Recordings2_Songs2, Songs2Role, Recordings2Role21));

                    Songs3.Properties.Add(Resource.Property("Recordings3", Resource.Collection(Recordings3), FK_Recordings3_Songs3, Songs3Role, Recordings3Role11));

                    Test1s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test1s_LabOwners, Test1sRole, LabOwnersRole51));
                    Test1s.Properties.Add(Resource.Property("Run1s", Run1s, FK_Test1s_Run1s, Test1sRole11, Run1sRole21));

                    Test2s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test2s_LabOwners, Test2sRole, LabOwnersRole61));
                    Test2s.Properties.Add(Resource.Property("Run1s", Run1s, FK_Test2s_Run1s, Test2sRole11, Run1sRole31));

                    Test3s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test3s_LabOwners, Test3sRole, LabOwnersRole71));
                    Test3s.Properties.Add(Resource.Property("Run1s", Run1s, FK_Test3s_Run1s, Test3sRole11, Run1sRole41));

                    Test4s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test4s_LabOwners, Test4sRole, LabOwnersRole81));
                    Test4s.Properties.Add(Resource.Property("Run2s", Run2s, FK_Test4s_Run2s, Test4sRole11, Run2sRole21));

                    Test5s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test5s_LabOwners, Test5sRole, LabOwnersRole91));
                    Test5s.Properties.Add(Resource.Property("Run2s", Run2s, FK_Test5s_Run2s, Test5sRole11, Run2sRole31));

                    Test6s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test6s_LabOwners, Test6sRole, LabOwnersRole101));
                    Test6s.Properties.Add(Resource.Property("Run2s", Run2s, FK_Test6s_Run2s, Test6sRole11, Run2sRole41));

                    Test7s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test7s_LabOwners, Test7sRole, LabOwnersRole111));
                    Test7s.Properties.Add(Resource.Property("Run3s", Run3s, FK_Test7s_Run3s, Test7sRole11, Run3sRole21));

                    Test8s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test8s_LabOwners, Test8sRole, LabOwnersRole121));
                    Test8s.Properties.Add(Resource.Property("Run3s", Run3s, FK_Test8s_Run3s, Test8sRole11, Run3sRole31));

                    Test9s.Properties.Add(Resource.Property("LabOwners", LabOwners, FK_Test9s_LabOwners, Test9sRole, LabOwnersRole131));
                    Test9s.Properties.Add(Resource.Property("Run3s", Run3s, FK_Test9s_Run3s, Test9sRole11, Run3sRole41));

                    Baseline.Properties.Add(Resource.Property("BugsDefect", Resource.Collection(DefectBug), FailureBugDefectTracking, FailureRole11, DefectBugRole21));
                    Baseline.Properties.Add(Resource.Property("Configs", Resource.Collection(Config), FailureConfig, FailureRole, ConfigRole));
                    Baseline.Properties.Add(Resource.Property("BugsProject", Resource.Collection(ProjectBug), FailureBugProjectTracking, FailureRole21, ProjectBugRole21));

                    Uninvestigated.Properties.Add(Resource.Property("BugsDefect", Resource.Collection(DefectBug), FailureBugDefectTracking, FailureRole11, DefectBugRole21));
                    Uninvestigated.Properties.Add(Resource.Property("Configs", Resource.Collection(Config), FailureConfig, FailureRole, ConfigRole));
                    Uninvestigated.Properties.Add(Resource.Property("BugsProject", Resource.Collection(ProjectBug), FailureBugProjectTracking, FailureRole21, ProjectBugRole21));

                    PropertyOwner.Properties.Add(Resource.Property("PrimaryVehicle", Vehicle, FK_Person_Vehicle, PersonRole, VehicleRole));
                    PropertyOwner.Properties.Add(Resource.Property("OldVehicles", Resource.Collection(Vehicle), FK_Person_OldVehicles, PersonRole11, OldVehiclesRole));

                    Car.Properties.Add(Resource.Property("PrimaryDriver", Person, NodeFacet.Nullable(true), FK_Person_Vehicle, VehicleRole, PersonRole));
                    Car.Properties.Add(Resource.Property("OldDrivers", Person, NodeFacet.Nullable(true), FK_Person_OldVehicles, OldVehiclesRole, PersonRole11));

                    Suv.Properties.Add(Resource.Property("PrimaryDriver", Person, NodeFacet.Nullable(true), FK_Person_Vehicle, VehicleRole, PersonRole));
                    Suv.Properties.Add(Resource.Property("OldDrivers", Person, NodeFacet.Nullable(true), FK_Person_OldVehicles, OldVehiclesRole, PersonRole11));

                    Truck.Properties.Add(Resource.Property("PrimaryDriver", Person, NodeFacet.Nullable(true), FK_Person_Vehicle, VehicleRole, PersonRole));
                    Truck.Properties.Add(Resource.Property("OldDrivers", Person, NodeFacet.Nullable(true), FK_Person_OldVehicles, OldVehiclesRole, PersonRole11));

                    GradStudent.Properties.Add(Resource.Property("College", College, FK_Students_College, StudentsRole, CollegeRole));

                    University.Properties.Add(Resource.Property("Students", Resource.Collection(Student), FK_Students_College, CollegeRole, StudentsRole));

                    TestFailure.Properties.Add(Resource.Property("BugsDefect", Resource.Collection(DefectBug), FailureBugDefectTracking, FailureRole11, DefectBugRole21));
                    TestFailure.Properties.Add(Resource.Property("Configs", Resource.Collection(Config), FailureConfig, FailureRole, ConfigRole));
                    TestFailure.Properties.Add(Resource.Property("BugsProject", Resource.Collection(ProjectBug), FailureBugProjectTracking, FailureRole21, ProjectBugRole21));

                    MachineConfig.Properties.Add(Resource.Property("Failures", Resource.Collection(Failure), FailureConfig, ConfigRole, FailureRole));


                    //Resource Containers added here
                    _serviceContainer = Resource.ServiceContainer(this, "Aruba",
                            Resource.ResourceContainer("BugDefectTrackingSet", DefectBug),
                            Resource.ResourceContainer("TaskSet", Task),
                            Resource.ResourceContainer("TestScenarioSet", Scenario),
                            Resource.ResourceContainer("NonDefaultMappingsSet", NonDefaultMappings),
                            Resource.ResourceContainer("ProjectSet", Project),
                            Resource.ResourceContainer("RunSet", Run),
                            Resource.ResourceContainer("OwnerDetailSet", OwnerDetail),
                            Resource.ResourceContainer("DeploymentScenarioSet", Scenario),
                            Resource.ResourceContainer("OwnerSet", Owner),
                            Resource.ResourceContainer("ConfigSet", Config, MachineConfig),
                            Resource.ResourceContainer("AllTypesSet", AllTypes),
                            Resource.ResourceContainer("OwnerContactInfoSet", OwnerContactInfo),
                            Resource.ResourceContainer("BugProjectTrackingSet", ProjectBug),
                            Resource.ResourceContainer("FailureSet", Failure, Baseline, Uninvestigated, TestFailure),
                            Resource.ResourceContainer("AllTypesComplexEntitySet", AllTypesComplexEntity),
                            Resource.ResourceContainer("NonDefaultFacetsSet", NonDefaultFacets),
                            Resource.ResourceContainer("DataKey_BigInt", DataKey_BigInt),
                            Resource.ResourceContainer("DataKey_Bit", DataKey_Bit),
                            Resource.ResourceContainer("DataKey_DateTime", DataKey_DateTime),
                            Resource.ResourceContainer("DataKey_Decimal", DataKey_Decimal),
                            Resource.ResourceContainer("DataKey_Float", DataKey_Float),
                            Resource.ResourceContainer("DataKey_GUID", DataKey_GUID),
                            Resource.ResourceContainer("DataKey_Money", DataKey_Money),
                            Resource.ResourceContainer("DataKey_Numeric", DataKey_Numeric),
                            Resource.ResourceContainer("DataKey_Real", DataKey_Real),
                            Resource.ResourceContainer("DataKey_SmallDateTime", DataKey_SmallDateTime),
                            Resource.ResourceContainer("DataKey_SmallMoney", DataKey_SmallMoney),
                            Resource.ResourceContainer("DataKey_TinyInt", DataKey_TinyInt),
                            Resource.ResourceContainer("DataKey_VarChar50", DataKey_VarChar50),
                            Resource.ResourceContainer("ThirteenNavigations", ThirteenNavigations),
                            Resource.ResourceContainer("People", Person, PropertyOwner),
                            Resource.ResourceContainer("Vehicles", Vehicle, Car, Suv, Truck),
                            Resource.ResourceContainer("Colleges", College, University),
                            Resource.ResourceContainer("Students", Student, GradStudent),
                            Resource.ResourceContainer("Computers", Computer),
                            Resource.ResourceContainer("ComputerDetails", ComputerDetails),
                            Resource.ResourceContainer("Workers", Worker),
                            Resource.ResourceContainer("Offices", Office),
                            Resource.ResourceContainer("DeepInheritanceSet", DeepTree_A, DeepTree_B, DeepTree_C, DeepTree_D, DeepTree_E, DeepTree_F, DeepTree_G, DeepTree_H, DeepTree_I, DeepTree_J, DeepTree_K, DeepTree_L, DeepTree_M, DeepTree_N),
                            Resource.ResourceContainer("WideInheritanceSet", WideTree_A, WideTree_B, WideTree_C, WideTree_D, WideTree_E, WideTree_F, WideTree_G, WideTree_H, WideTree_I, WideTree_J, WideTree_K, WideTree_L, WideTree_M, WideTree_N),
                            Resource.ResourceContainer("Albums", Albums),
                            Resource.ResourceContainer("Albums2", Albums2),
                            Resource.ResourceContainer("Albums3", Albums3),
                            Resource.ResourceContainer("Artists", Artists),
                            Resource.ResourceContainer("Artists2", Artists2),
                            Resource.ResourceContainer("Artists3", Artists3),
                            Resource.ResourceContainer("Builds", Builds),
                            Resource.ResourceContainer("LabIssues", LabIssues),
                            Resource.ResourceContainer("LabOwners", LabOwners),
                            Resource.ResourceContainer("Recordings", Recordings),
                            Resource.ResourceContainer("Recordings2", Recordings2),
                            Resource.ResourceContainer("Recordings3", Recordings3),
                            Resource.ResourceContainer("Run1s", Run1s),
                            Resource.ResourceContainer("Run2s", Run2s),
                            Resource.ResourceContainer("Run3s", Run3s),
                            Resource.ResourceContainer("Songs", Songs),
                            Resource.ResourceContainer("Songs2", Songs2),
                            Resource.ResourceContainer("Songs3", Songs3),
                            Resource.ResourceContainer("Test1s", Test1s),
                            Resource.ResourceContainer("Test2s", Test2s),
                            Resource.ResourceContainer("Test3s", Test3s),
                            Resource.ResourceContainer("Test4s", Test4s),
                            Resource.ResourceContainer("Test5s", Test5s),
                            Resource.ResourceContainer("Test6s", Test6s),
                            Resource.ResourceContainer("Test7s", Test7s),
                            Resource.ResourceContainer("Test8s", Test8s),
                            Resource.ResourceContainer("Test9s", Test9s),
                            Resource.ResourceContainer("TooHot", Porridge, Oatmeal),
                            Resource.ResourceContainer("TooCold", Porridge, Oatmeal),
                            Resource.ResourceContainer("JustRight", Porridge, Oatmeal),
                            Resource.ResourceContainer("Cats", Pet),
                            Resource.ResourceContainer("CatTraits", Trait),
                            Resource.ResourceContainer("CatTraitDetail", TraitDetail),
                            Resource.ResourceContainer("CatPetType", PetType),
                            Resource.ResourceContainer("CatTraits_1Way", Trait_1Way),
                            Resource.ResourceContainer("CatTraitDetail_1Way", TraitDetail_1Way),
                            Resource.ResourceContainer("CatPetType_1Way", PetType_1Way),
                            Resource.ResourceContainer("Dogs", Pet),
                            Resource.ResourceContainer("DogTraits", Trait),
                            Resource.ResourceContainer("DogTraitDetail", TraitDetail),
                            Resource.ResourceContainer("DogPetType", PetType),
                            Resource.ResourceContainer("DogTraits_1Way", Trait_1Way),
                            Resource.ResourceContainer("DogTraitDetail_1Way", TraitDetail_1Way),
                            Resource.ResourceContainer("DogPetType_1Way", PetType_1Way)
                    );

                    _serviceContainer.ResourceContainers["Cats"].Facets.MestTag = "Cats";
                    _serviceContainer.ResourceContainers["CatTraits"].Facets.MestTag = "Cats";
                    _serviceContainer.ResourceContainers["CatTraitDetail"].Facets.MestTag = "Cats";
                    _serviceContainer.ResourceContainers["CatPetType"].Facets.MestTag = "Cats";
                    _serviceContainer.ResourceContainers["CatTraits_1Way"].Facets.MestTag = "Cats";
                    _serviceContainer.ResourceContainers["CatTraitDetail_1Way"].Facets.MestTag = "Cats";
                    _serviceContainer.ResourceContainers["CatPetType_1Way"].Facets.MestTag = "Cats";

                    _serviceContainer.ResourceContainers["Dogs"].Facets.MestTag = "Dogs";
                    _serviceContainer.ResourceContainers["DogTraits"].Facets.MestTag = "Dogs";
                    _serviceContainer.ResourceContainers["DogTraitDetail"].Facets.MestTag = "Dogs";
                    _serviceContainer.ResourceContainers["DogPetType"].Facets.MestTag = "Dogs";
                    _serviceContainer.ResourceContainers["DogTraits_1Way"].Facets.MestTag = "Dogs";
                    _serviceContainer.ResourceContainers["DogTraitDetail_1Way"].Facets.MestTag = "Dogs";
                    _serviceContainer.ResourceContainers["DogPetType_1Way"].Facets.MestTag = "Dogs";

                    _serviceContainer.ResourceContainers["TestScenarioSet"].Facets.MestTag = "TestScenarioSet";
                    _serviceContainer.ResourceContainers["DeploymentScenarioSet"].Facets.MestTag = "DeploymentScenarioSet";

                    foreach (ResourceContainer rc in _serviceContainer.ResourceContainers)
                    {
                        foreach (ResourceType t in rc.ResourceTypes)
                        {
                            t.InferAssociations();
                        }
                    }
                }
                return _serviceContainer;
            }
        }

        public override ResourceType LanguageDataResource()
        {
            return this.ServiceContainer.ResourceContainers["DataKey_Money"].BaseType;
        }
    }
}

