//---------------------------------------------------------------------
// <copyright file="aruba.linqtosql.workspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;

    [WorkspaceAttribute("Aruba", DataLayerProviderKind.LinqToSql, Priority = 3, Standard = true)]
    public class LinqToSqlArubaWorkspace : LinqToSqlWorkspace
    {
        public LinqToSqlArubaWorkspace()
            : base("Aruba", "Aruba", "ArubaDataContext")
        {
            this.Language = WorkspaceLanguage.CSharp;
        }
        public override ServiceContainer ServiceContainer
        {
            get
            {
                if (_serviceContainer == null)
                {
                    ResourceType AllTypes = Resource.ResourceType("AllTypes", "Aruba",
                        Resource.Property("C1_int", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("C2_int", Clr.Types.Int32, NodeFacet.Nullable(false)),
                        Resource.Property("C3_smallint", Clr.Types.Int16, NodeFacet.Nullable(false)),
                        Resource.Property("C4_tinyint", Clr.Types.Byte, NodeFacet.Nullable(false)),
                        Resource.Property("C5_bit", Clr.Types.Boolean, NodeFacet.Nullable(false)),
                        Resource.Property("C6_datetime", Clr.Types.DateTime, NodeFacet.Nullable(false)),
                        Resource.Property("C7_smalldatetime", Clr.Types.DateTime, NodeFacet.Nullable(false)),
                        Resource.Property("C8_decimal28_4", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C9_numeric28_4", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C10_real", SqlTypes.Real, NodeFacet.Nullable(false)),
                        Resource.Property("C11_float", SqlTypes.Float, NodeFacet.Nullable(false)),
                        Resource.Property("C12_money", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C13_smallmoney", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C14_varchar512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C15_char512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512)),
                        Resource.Property("C16_text", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.Sortable(false)),
                        Resource.Property("C17_binary512", LinqToSqlTypes.Binary, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C18_varbinary512", LinqToSqlTypes.Binary, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C19_image", LinqToSqlTypes.Binary, NodeFacet.Nullable(false), NodeFacet.Sortable(false), NodeFacet.UnderlyingType(UnderlyingType.Image)),
                        Resource.Property("C20_nvarchar512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C21_nchar512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C22_ntext", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.Sortable(false)),
                        Resource.Property("C23_uniqueidentifier", Clr.Types.Guid, NodeFacet.Nullable(false), NodeFacet.Sortable(false)),
                        Resource.Property("C24_bigint", Clr.Types.Int64, NodeFacet.Nullable(false)),
                        Resource.Property("C25_int", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("C26_smallint", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("C27_tinyint", Clr.Types.Byte, NodeFacet.Nullable(true)),
                        Resource.Property("C28_bit", Clr.Types.Boolean, NodeFacet.Nullable(true)),
                        Resource.Property("C29_datetime", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("C30_smalldatetime", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("C31_decimal28_4", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C32_numeric28_4", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C33_real", SqlTypes.Real, NodeFacet.Nullable(true)),
                        Resource.Property("C34_float", SqlTypes.Float, NodeFacet.Nullable(true)),
                        Resource.Property("C35_money", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C36_smallmoney", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C37_varchar512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("C38_char512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("C39_text", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)),
                        Resource.Property("C40_binary512", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C41_varbinary512", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C42_image", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.Sortable(false), NodeFacet.UnderlyingType(UnderlyingType.Image)),
                        Resource.Property("C43_nvarchar512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C44_nchar512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C45_ntext", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)),
                        Resource.Property("C46_uniqueidentifier", Clr.Types.Guid, NodeFacet.Nullable(true)),
                        Resource.Property("C47_bigint", Clr.Types.Int64, NodeFacet.Nullable(true)));

                    ResourceType AllTypesComplex = Resource.ResourceType("AllTypesComplex", "Aruba",
                        Resource.Property("C1_int", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("C2_int", Clr.Types.Int32, NodeFacet.Nullable(false)),
                        Resource.Property("C3_smallint", Clr.Types.Int16, NodeFacet.Nullable(false)),
                        Resource.Property("C4_tinyint", Clr.Types.Byte, NodeFacet.Nullable(false)),
                        Resource.Property("C5_bit", Clr.Types.Boolean, NodeFacet.Nullable(false)),
                        Resource.Property("C6_datetime", Clr.Types.DateTime, NodeFacet.Nullable(false)),
                        Resource.Property("C7_smalldatetime", Clr.Types.DateTime, NodeFacet.Nullable(false)),
                        Resource.Property("C8_decimal28_4", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C9_numeric28_4", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C10_real", SqlTypes.Real, NodeFacet.Nullable(false)),
                        Resource.Property("C11_float", SqlTypes.Float, NodeFacet.Nullable(false)),
                        Resource.Property("C12_money", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C13_smallmoney", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C14_varchar512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C15_char512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C16_text", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.Sortable(false)),
                        Resource.Property("C17_binary512", LinqToSqlTypes.Binary, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C18_varbinary512", LinqToSqlTypes.Binary, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C19_image", LinqToSqlTypes.Binary, NodeFacet.Nullable(false), NodeFacet.Sortable(false), NodeFacet.UnderlyingType(UnderlyingType.Image)),
                        Resource.Property("C20_nvarchar512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C21_nchar512", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C22_ntext", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.Sortable(false)),
                        Resource.Property("C23_uniqueidentifier", Clr.Types.Guid, NodeFacet.Nullable(false), NodeFacet.Sortable(false)),
                        Resource.Property("C24_bigint", Clr.Types.Int64, NodeFacet.Nullable(false)));

                    ResourceType BugsDefectTracking = Resource.ResourceType("BugsDefectTracking", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Number", Clr.Types.Int32, NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("FailureId", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("AssignedToId", Clr.Types.Int32, NodeFacet.Nullable(false)),
                        Resource.Property("ResolvedById", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Comment", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)));

                    ResourceType BugsProjectTracking = Resource.ResourceType("BugsProjectTracking", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Number", Clr.Types.Int32, NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("FailureId", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("AssignedToId", Clr.Types.Int32, NodeFacet.Nullable(false)),
                        Resource.Property("ResolvedById", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Comment", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)));

                    ResourceType ConfigFailures = Resource.ResourceType("ConfigFailures", "Aruba",
                        Resource.Property("ConfigId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("FailureId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)));

                    ResourceType Configs = Resource.ResourceType("Configs", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("OS", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Language", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Architecture", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType DataKey_BigInt = Resource.ResourceType("DataKey_BigInt", "Aruba",
                        Resource.Property("Id", Clr.Types.Int64, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10), NodeFacet.Sortable(false)));

                    ResourceType DataKey_Bit = Resource.ResourceType("DataKey_Bit", "Aruba",
                        Resource.Property("Id", Clr.Types.Boolean, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", LinqToSqlTypes.XElement, NodeFacet.Nullable(true), NodeFacet.Sortable(false)));

                    ResourceType DataKey_DateTime = Resource.ResourceType("DataKey_DateTime", "Aruba",
                        Resource.Property("Id", Clr.Types.DateTime, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)));

                    ResourceType DataKey_Decimal = Resource.ResourceType("DataKey_Decimal", "Aruba",
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)));

                    ResourceType DataKey_Float = Resource.ResourceType("DataKey_Float", "Aruba",
                        Resource.Property("Id", SqlTypes.Float, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.Sortable(false), NodeFacet.UnderlyingType(UnderlyingType.Image)));

                    ResourceType DataKey_GUID = Resource.ResourceType("DataKey_GUID", "Aruba",
                        Resource.Property("Id", Clr.Types.Guid, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.Sortable(false)));

                    ResourceType DataKey_Money = Resource.ResourceType("DataKey_Money", "Aruba",
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)));

                    ResourceType DataKey_Numeric = Resource.ResourceType("DataKey_Numeric", "Aruba",
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)));

                    ResourceType DataKey_Real = Resource.ResourceType("DataKey_Real", "Aruba",
                        Resource.Property("Id", SqlTypes.Real, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true)));

                    ResourceType DataKey_SmallDateTime = Resource.ResourceType("DataKey_SmallDateTime", "Aruba",
                        Resource.Property("Id", Clr.Types.DateTime, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)));

                    ResourceType DataKey_SmallMoney = Resource.ResourceType("DataKey_SmallMoney", "Aruba",
                        Resource.Property("Id", Clr.Types.Decimal, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true)));

                    ResourceType DataKey_TinyInt = Resource.ResourceType("DataKey_TinyInt", "Aruba",
                        Resource.Property("Id", Clr.Types.Byte, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DataColumn", Clr.Types.Guid, NodeFacet.Nullable(true)));

                    ResourceType DataKey_VarChar50 = Resource.ResourceType("DataKey_VarChar50", "Aruba",
                        Resource.Property("Id", Clr.Types.String, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.MaxSize(50)),
                        Resource.Property("DataColumn", LinqToSqlTypes.XElement, NodeFacet.Nullable(true), NodeFacet.Sortable(false)));

                    ResourceType DeploymentScenarios = Resource.ResourceType("DeploymentScenarios", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("ProjectId", Clr.Types.Int32, NodeFacet.Nullable(true)));

                    ResourceType FailureDetails = Resource.ResourceType("FailureDetails", "Aruba",
                        Resource.Property("FailureId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Log", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType Failures = Resource.ResourceType("Failures", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("TestId", Clr.Types.Int32, NodeFacet.Nullable(false)),
                        Resource.Property("Baseline", Clr.Types.Boolean, NodeFacet.Nullable(true)),
                        Resource.Property("TypeId", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("TestCase", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Variation", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Comment", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Changed", Clr.Types.DateTime, NodeFacet.Nullable(false)));

                    ResourceType FailureTypes = Resource.ResourceType("FailureTypes", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType MachineConfigs = Resource.ResourceType("MachineConfigs", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Host", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("IPAddress", Clr.Types.Guid, NodeFacet.Nullable(true)));

                    ResourceType NonDefaultFacets = Resource.ResourceType("NonDefaultFacets", "Aruba",
                        Resource.Property("C1_int", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("C_decimal27_3_AS_decimal28_4", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_decimal24_0_AS_decimal26_2", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_numeric24_0_AS_numeric28_4", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_numeric24_0_AS_numeric25_1", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_varchar230_AS_varchar512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("C_varchar17_AS_varchar98", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(98)),
                        Resource.Property("C_varbinary60_AS_varbinary512", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C_varbinary31_AS_varbinary365", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.MaxSize(365), NodeFacet.Sortable(false)),
                        Resource.Property("C_varchar80_AS_nvarchar512", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C_varchar185_AS_nvarchar285", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(285), NodeFacet.Sortable(false)));

                    ResourceType NonDefaultMappings = Resource.ResourceType("NonDefaultMappings", "Aruba",
                        Resource.Property("C1_int", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("C_int_AS_decimal", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C_int_AS_numeric", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_int_AS_float", SqlTypes.Float, NodeFacet.Nullable(false)),
                        Resource.Property("C_int_AS_money", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_int_AS_bigint", Clr.Types.Int64, NodeFacet.Nullable(false)),
                        Resource.Property("C_smallint_AS_int", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("C_smallint_AS_decimal", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C_smallint_AS_numeric", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_smallint_AS_real", SqlTypes.Real, NodeFacet.Nullable(false)),
                        Resource.Property("C_smallint_AS_float", SqlTypes.Float, NodeFacet.Nullable(true)),
                        Resource.Property("C_smallint_AS_money", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C_smallint_AS_smallmoney", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_smallint_AS_bigint", Clr.Types.Int64, NodeFacet.Nullable(false)),
                        Resource.Property("C_tinyint_AS_int", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("C_tinyint_AS_smallint", Clr.Types.Int16, NodeFacet.Nullable(false)),
                        Resource.Property("C_tinyint_AS_decimal", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_tinyint_AS_numeric", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C_tinyint_AS_real", SqlTypes.Real, NodeFacet.Nullable(true)),
                        Resource.Property("C_tinyint_AS_float", SqlTypes.Float, NodeFacet.Nullable(false)),
                        Resource.Property("C_tinyint_AS_money", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("C_tinyint_AS_smallmoney", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C_tinyint_AS_bigint", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("C_smalldatetime_AS_datetime", Clr.Types.DateTime, NodeFacet.Nullable(false)),
                        Resource.Property("C_varchar_AS_nvarchar", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("C_char_AS_nchar", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512), NodeFacet.Sortable(false)),
                        Resource.Property("C_nvarchar_AS_ntext", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)),
                        Resource.Property("C_bigint_AS_decimal", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("C_bigint_AS_numeric", Clr.Types.Decimal, NodeFacet.Nullable(true)));

                    ResourceType OwnerContactInfos = Resource.ResourceType("OwnerContactInfos", "Aruba",
                        Resource.Property("ContactInfoId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Email", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("WorkPhone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("StreetAddress", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("State", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("MainZip", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("ExtendedZip", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType OwnerDetails = Resource.ResourceType("OwnerDetails", "Aruba",
                        Resource.Property("DetailId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("HomeAddress1", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("HomeAddress2", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("Level", Clr.Types.Int32, NodeFacet.Nullable(true)));

                    ResourceType OwnerOwnerDetails = Resource.ResourceType("OwnerOwnerDetails", "Aruba",
                        Resource.Property("OwnerId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("DetailId", Clr.Types.Int32, NodeFacet.Nullable(false)));

                    ResourceType Owners = Resource.ResourceType("Owners", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("FirstName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("LastName", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Alias", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512)));

                    ResourceType Projects = Resource.ResourceType("Projects", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType RunOwners = Resource.ResourceType("RunOwners", "Aruba",
                        Resource.Property("RunId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("OwnerId", Clr.Types.Int32, NodeFacet.Nullable(false)));

                    ResourceType Runs = Resource.ResourceType("Runs", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(512)),
                        Resource.Property("Purpose", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Started", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("Completed", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("StartedBy", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("RequestStarted", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("RequestCompleted", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("RequestStartedBy", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType TaskInvestigates = Resource.ResourceType("TaskInvestigates", "Aruba",
                        Resource.Property("TaskId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Investigates", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("Improvements", Clr.Types.Int64, NodeFacet.Nullable(true)));

                    ResourceType TaskResults = Resource.ResourceType("TaskResults", "Aruba",
                        Resource.Property("TaskId", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Passed", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("Failed", Clr.Types.Int64, NodeFacet.Nullable(true)));

                    ResourceType Tasks = Resource.ResourceType("Tasks", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("ConfigId", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Deleted", Clr.Types.Boolean, NodeFacet.Nullable(true)),
                        Resource.Property("RunId", Clr.Types.Int32, NodeFacet.Nullable(false)),
                        Resource.Property("Started", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("Completed", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("StartedBy", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType Testers = Resource.ResourceType("Testers", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)));

                    ResourceType TestScenarios = Resource.ResourceType("TestScenarios", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("Name", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(512)),
                        Resource.Property("ProjectId", Clr.Types.Int32, NodeFacet.Nullable(true)));

                    ResourceType ThirteenNavigations = Resource.ResourceType("ThirteenNavigations", "Aruba",
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("BigInt_Id", Clr.Types.Int64, NodeFacet.Nullable(true)),
                        Resource.Property("Bit_Id", Clr.Types.Boolean, NodeFacet.Nullable(false)),
                        Resource.Property("DateTime_Id", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("Decimal_Id", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("Float_Id", SqlTypes.Float, NodeFacet.Nullable(true)),
                        Resource.Property("Money_Id", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("Numeric_Id", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("Real_Id", SqlTypes.Real, NodeFacet.Nullable(true)),
                        Resource.Property("SmallDateTime_Id", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("TinyInt_Id", Clr.Types.Byte, NodeFacet.Nullable(true)),
                        Resource.Property("GUID_Id", Clr.Types.Guid, NodeFacet.Nullable(true)),
                        Resource.Property("Varchar_Id", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(50)),
                        Resource.Property("SmallMoney_Id", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("DataColumn", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)));

                    BugsDefectTracking.Properties.Add(Resource.Property("Owners", Owners));
                    BugsDefectTracking.Properties.Add(Resource.Property("Failures", Failures));
                    BugsDefectTracking.Properties.Add(Resource.Property("ResolvedBy", Owners));

                    BugsProjectTracking.Properties.Add(Resource.Property("Owners", Owners));
                    BugsProjectTracking.Properties.Add(Resource.Property("Failures", Failures));
                    BugsProjectTracking.Properties.Add(Resource.Property("ResolvedBy", Owners));

                    ConfigFailures.Properties.Add(Resource.Property("Configs", Configs));
                    ConfigFailures.Properties.Add(Resource.Property("Failures", Failures));

                    Configs.Properties.Add(Resource.Property("ConfigFailures", Resource.Collection(ConfigFailures)));
                    Configs.Properties.Add(Resource.Property("MachineConfigs", MachineConfigs));

                    DataKey_BigInt.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_Bit.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_DateTime.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_Decimal.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_Float.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_GUID.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_Money.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_Numeric.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_Real.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_SmallDateTime.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_SmallMoney.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_TinyInt.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));
                    DataKey_VarChar50.Properties.Add(Resource.Property("ThirteenNavigations", Resource.Collection(ThirteenNavigations)));

                    DeploymentScenarios.Properties.Add(Resource.Property("Projects", Projects));
                    
                    FailureDetails.Properties.Add(Resource.Property("Failures", Failures));

                    Failures.Properties.Add(Resource.Property("BugsDefectTracking", Resource.Collection(BugsDefectTracking)));
                    Failures.Properties.Add(Resource.Property("BugsProjectTracking", Resource.Collection(BugsProjectTracking)));
                    Failures.Properties.Add(Resource.Property("ConfigFailures", Resource.Collection(ConfigFailures)));
                    Failures.Properties.Add(Resource.Property("FailureDetails", FailureDetails));

                    MachineConfigs.Properties.Add(Resource.Property("Configs", Configs));

                    OwnerDetails.Properties.Add(Resource.Property("OwnerOwnerDetails", Resource.Collection(OwnerOwnerDetails)));

                    OwnerOwnerDetails.Properties.Add(Resource.Property("OwnerDetails", OwnerDetails));
                    OwnerOwnerDetails.Properties.Add(Resource.Property("Owners", Owners));

                    Owners.Properties.Add(Resource.Property("BugsDefectTracking", Resource.Collection(BugsDefectTracking)));
                    Owners.Properties.Add(Resource.Property("Bugs_ResolvedByOwners", Resource.Collection(BugsDefectTracking)));
                    Owners.Properties.Add(Resource.Property("BugsProjectTracking", Resource.Collection(BugsProjectTracking)));
                    Owners.Properties.Add(Resource.Property("BugsProject_ResolvedByOwners", Resource.Collection(BugsProjectTracking)));
                    Owners.Properties.Add(Resource.Property("OwnerOwnerDetails", OwnerOwnerDetails));
                    Owners.Properties.Add(Resource.Property("RunOwners", Resource.Collection(RunOwners)));

                    Projects.Properties.Add(Resource.Property("DeploymentScenarios", Resource.Collection(DeploymentScenarios)));
                    Projects.Properties.Add(Resource.Property("TestScenarios", Resource.Collection(TestScenarios)));

                    RunOwners.Properties.Add(Resource.Property("Owners", Owners));
                    RunOwners.Properties.Add(Resource.Property("Runs", Runs));

                    Runs.Properties.Add(Resource.Property("RunOwners", RunOwners));
                    Runs.Properties.Add(Resource.Property("Tasks", Resource.Collection(Tasks)));

                    TaskInvestigates.Properties.Add(Resource.Property("Tasks", Tasks));

                    TaskResults.Properties.Add(Resource.Property("Tasks", Tasks));

                    Tasks.Properties.Add(Resource.Property("TaskInvestigates", TaskInvestigates));
                    Tasks.Properties.Add(Resource.Property("TaskResults", TaskResults));
                    Tasks.Properties.Add(Resource.Property("Runs", Runs));
                    
                    TestScenarios.Properties.Add(Resource.Property("Projects", Projects));

                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_BigInt", DataKey_BigInt));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Bit", DataKey_Bit));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_DateTime", DataKey_DateTime));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Decimal", DataKey_Decimal));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Float", DataKey_Float));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_GUID", DataKey_GUID));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Money", DataKey_Money));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Numeric", DataKey_Numeric));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_Real", DataKey_Real));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_SmallDateTime", DataKey_SmallDateTime));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_SmallMoney", DataKey_SmallMoney));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_TinyInt", DataKey_TinyInt));
                    ThirteenNavigations.Properties.Add(Resource.Property("DataKey_VarChar50", DataKey_VarChar50));

                    _serviceContainer = Resource.ServiceContainer(this, "Aruba",
                    Resource.ResourceContainer("AllTypes", AllTypes),
                    Resource.ResourceContainer("AllTypesComplex", AllTypesComplex),
                    Resource.ResourceContainer("BugsDefectTracking", BugsDefectTracking),
                    Resource.ResourceContainer("BugsProjectTracking", BugsProjectTracking),
                    Resource.ResourceContainer("ConfigFailures", ConfigFailures),
                    Resource.ResourceContainer("Configs", Configs),
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
                    Resource.ResourceContainer("DeploymentScenarios", DeploymentScenarios),
                    Resource.ResourceContainer("FailureDetails", FailureDetails),
                    Resource.ResourceContainer("Failures", Failures),
                    Resource.ResourceContainer("FailureTypes", FailureTypes),
                    Resource.ResourceContainer("MachineConfigs", MachineConfigs),
                    Resource.ResourceContainer("NonDefaultFacets", NonDefaultFacets),
                    Resource.ResourceContainer("NonDefaultMappings", NonDefaultMappings),
                    Resource.ResourceContainer("OwnerContactInfos", OwnerContactInfos),
                    Resource.ResourceContainer("OwnerDetails", OwnerDetails),
                    Resource.ResourceContainer("OwnerOwnerDetails", OwnerOwnerDetails),
                    Resource.ResourceContainer("Owners", Owners),
                    Resource.ResourceContainer("Projects", Projects),
                    Resource.ResourceContainer("RunOwners", RunOwners),
                    Resource.ResourceContainer("Runs", Runs),
                    Resource.ResourceContainer("TaskInvestigates", TaskInvestigates),
                    Resource.ResourceContainer("TaskResults", TaskResults),
                    Resource.ResourceContainer("Tasks", Tasks),
                    Resource.ResourceContainer("Testers", Testers),
                    Resource.ResourceContainer("TestScenarios", TestScenarios),
                    Resource.ResourceContainer("ThirteenNavigations", ThirteenNavigations));

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
            return this.ServiceContainer.ResourceContainers["OwnerDetails"].BaseType;
        }
    }
}
