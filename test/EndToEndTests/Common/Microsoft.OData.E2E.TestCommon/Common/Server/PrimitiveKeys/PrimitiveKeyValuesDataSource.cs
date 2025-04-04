//-----------------------------------------------------------------------------
// <copyright file="PrimitiveKeyValuesDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveKeys
{
    public class PrimitiveKeyValuesDataSource
    {
        public static PrimitiveKeyValuesDataSource CreateInstance()
        {
            return new PrimitiveKeyValuesDataSource();
        }

        public PrimitiveKeyValuesDataSource()
        {
            ResetData();
            InitializeData();
        }

        private void InitializeData()
        {
            PopulateEdmBinaries();
            PopulateEdmBooleans();
            PopulateEdmBytes();
            PopulateEdmDecimals();
            PopulateEdmDoubles();
            PopulateEdmSingles();
            PopulateEdmGuids();
            PopulateEdmInt16s();
            PopulateEdmInt32s();
            PopulateEdmInt64s();
            PopulateEdmStrings();
            PopulateEdmTimes();
            PopulateEdmDateTimeOffsets();
        }

        private void ResetData()
        {
            EdmBinaries?.Clear();
            EdmBooleans?.Clear();
            EdmBytes?.Clear();
            EdmDecimals?.Clear();
            EdmDoubles?.Clear();
            EdmSingles?.Clear();
            EdmGuids?.Clear();
            EdmInt16s?.Clear();
            EdmInt32s?.Clear();
            EdmInt64s?.Clear();
            EdmStrings?.Clear();
            EdmTimes?.Clear();
            EdmDateTimeOffsets?.Clear();
        }

        public IList<EdmBinary>? EdmBinaries { get; set; }
        public IList<EdmBoolean>? EdmBooleans { get; set; }
        public IList<EdmByte>? EdmBytes { get; set; }
        public IList<EdmDecimal>? EdmDecimals { get; set; }
        public IList<EdmDouble>? EdmDoubles { get; set; }
        public IList<EdmSingle>? EdmSingles { get; set; }
        public IList<EdmGuid>? EdmGuids { get; set; }
        public IList<EdmInt16>? EdmInt16s { get; set; }
        public IList<EdmInt32>? EdmInt32s { get; set; }
        public IList<EdmInt64>? EdmInt64s { get; set; }
        public IList<EdmString>? EdmStrings { get; set; }
        public IList<EdmTime>? EdmTimes { get; set; }
        public IList<EdmDateTimeOffset>? EdmDateTimeOffsets { get; set; }

        private void PopulateEdmBinaries()
        {
            EdmBinaries =
            [
                new EdmBinary { Id = new byte[] { } },
                new EdmBinary { Id = new byte[] { 1 } },
                new EdmBinary { Id = new byte[] { 2, 3, 4 } },
            ];
        }

        private void PopulateEdmBooleans()
        {
            EdmBooleans =
            [
                new EdmBoolean { Id = true },
                new EdmBoolean { Id = false },
            ];
        }

        private void PopulateEdmBytes()
        {
            EdmBytes =
            [
                new EdmByte { Id = byte.MinValue },
                new EdmByte { Id = byte.MaxValue },
                new EdmByte { Id = 0 },
                new EdmByte { Id = 1 },
            ];
        }

        private void PopulateEdmDecimals()
        {
            EdmDecimals =
            [
                new EdmDecimal { Id = decimal.MinValue },
                new EdmDecimal { Id = decimal.MaxValue },
                new EdmDecimal { Id = decimal.MinusOne },
                new EdmDecimal { Id = decimal.One },
                new EdmDecimal { Id = decimal.Zero },
            ];
        }

        private void PopulateEdmDoubles()
        {
            EdmDoubles =
            [
                new EdmDouble { Id = double.Epsilon },
                new EdmDouble { Id = double.MaxValue },
                new EdmDouble { Id = double.MinValue },
                new EdmDouble { Id = double.NaN },
                new EdmDouble { Id = double.NegativeInfinity },
                new EdmDouble { Id = double.PositiveInfinity },
                new EdmDouble { Id = 0 },
                new EdmDouble { Id = -1 },
                new EdmDouble { Id = 1 },
            ];
        }

        private void PopulateEdmSingles()
        {
            EdmSingles =
            [
                new EdmSingle { Id = float.Epsilon },
                new EdmSingle { Id = float.MaxValue },
                new EdmSingle { Id = float.MinValue },
                new EdmSingle { Id = float.NaN },
                new EdmSingle { Id = float.NegativeInfinity },
                new EdmSingle { Id = float.PositiveInfinity },
                new EdmSingle { Id = 0 },
                new EdmSingle { Id = -1 },
                new EdmSingle { Id = 1 },
            ];
        }

        private void PopulateEdmGuids()
        {
            EdmGuids =
            [
                new EdmGuid { Id = Guid.NewGuid() },
                new EdmGuid { Id = Guid.Empty },
            ];
        }

        private void PopulateEdmInt16s()
        {
            EdmInt16s =
            [
                new EdmInt16 { Id = short.MinValue },
                new EdmInt16 { Id = short.MaxValue },
                new EdmInt16 { Id = 0 },
                new EdmInt16 { Id = -1 },
                new EdmInt16 { Id = 1 },
            ];
        }

        private void PopulateEdmInt32s()
        {
            EdmInt32s =
            [
                new EdmInt32 { Id = int.MinValue },
                new EdmInt32 { Id = int.MaxValue },
                new EdmInt32 { Id = 0 },
                new EdmInt32 { Id = -1 },
                new EdmInt32 { Id = 1 },
            ];
        }

        private void PopulateEdmInt64s()
        {
            EdmInt64s =
            [
                new EdmInt64 { Id = long.MinValue },
                new EdmInt64 { Id = long.MaxValue },
                new EdmInt64 { Id = 0 },
                new EdmInt64 { Id = -1 },
                new EdmInt64 { Id = 1 },
            ];
        }

        private void PopulateEdmStrings()
        {
            EdmStrings =
            [
                new EdmString { Id = "!" },
                new EdmString { Id = "!!" },
                new EdmString { Id = "!!!" },
                new EdmString { Id = "*" },
                new EdmString { Id = "**" },
                new EdmString { Id = "***" },
                new EdmString { Id = "(" },
                new EdmString { Id = "((" },
                new EdmString { Id = "(((" },
                new EdmString { Id = ")" },
                new EdmString { Id = "))" },
                new EdmString { Id = ")))" },
                new EdmString { Id = ";" },
                new EdmString { Id = ";;" },
                new EdmString { Id = ";;;" },
                new EdmString { Id = ":" },
                new EdmString { Id = "::" },
                new EdmString { Id = ":::" },
                new EdmString { Id = "@" },
                new EdmString { Id = "@@" },
                new EdmString { Id = "@@@" },
                new EdmString { Id = "&" },
                new EdmString { Id = "&&" },
                new EdmString { Id = "&&&" },
                new EdmString { Id = "=" },
                new EdmString { Id = "==" },
                new EdmString { Id = "===" },
                new EdmString { Id = "+" },
                new EdmString { Id = "++" },
                new EdmString { Id = "+++" },
                new EdmString { Id = "$" },
                new EdmString { Id = "$$" },
                new EdmString { Id = "$$$" },
                new EdmString { Id = "," },
                new EdmString { Id = ",," },
                new EdmString { Id = ",,," },
                new EdmString { Id = "[" },
                new EdmString { Id = "[[" },
                new EdmString { Id = "[[[" },
                new EdmString { Id = "]" },
                new EdmString { Id = "]]" },
                new EdmString { Id = "]]]" },
                new EdmString { Id = "{" },
                new EdmString { Id = "{{" },
                new EdmString { Id = "{{{" },
                new EdmString { Id = "}" },
                new EdmString { Id = "}}" },
                new EdmString { Id = "}}}" },
                new EdmString { Id = "?" },
                new EdmString { Id = "??" },
                new EdmString { Id = "???" },
                new EdmString { Id = "'" },
                new EdmString { Id = "''" },
                new EdmString { Id = "'''" },
                new EdmString { Id = "/" },
                new EdmString { Id = "//" },
                new EdmString { Id = "///" },
                new EdmString { Id = "\"" },
                new EdmString { Id = "\"\"" },
                new EdmString { Id = "\"\"\"" },
                new EdmString { Id = "SomeID" }
            ];
        }

        private void PopulateEdmTimes()
        {
            EdmTimes =
            [
                new EdmTime { Id = TimeSpan.MinValue },
                new EdmTime { Id = TimeSpan.MaxValue },
                new EdmTime { Id = TimeSpan.Zero },
            ];
        }

        private void PopulateEdmDateTimeOffsets()
        {
            EdmDateTimeOffsets =
            [
                new EdmDateTimeOffset { Id = DateTimeOffset.MinValue },
                new EdmDateTimeOffset { Id = DateTimeOffset.MaxValue },
            ];
        }
    }
}
