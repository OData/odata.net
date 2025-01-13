//-----------------------------------------------------------------------------
// <copyright file="PrimitiveKeyValuesDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.PrimitiveKeys
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
            this.EdmBinaries?.Clear();
            this.EdmBooleans?.Clear();
            this.EdmBytes?.Clear();
            this.EdmDecimals?.Clear();
            this.EdmDoubles?.Clear();
            this.EdmSingles?.Clear();
            this.EdmGuids?.Clear();
            this.EdmInt16s?.Clear();
            this.EdmInt32s?.Clear();
            this.EdmInt64s?.Clear();
            this.EdmStrings?.Clear();
            this.EdmTimes?.Clear();
            this.EdmDateTimeOffsets?.Clear();
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
            this.EdmBinaries =
            [
                new EdmBinary { Id = new Byte[] { } },
                new EdmBinary { Id = new Byte[] { 1 } },
                new EdmBinary { Id = new Byte[] { 2, 3, 4 } },
            ];
        }

        private void PopulateEdmBooleans()
        {
            this.EdmBooleans =
            [
                new EdmBoolean { Id = true },
                new EdmBoolean { Id = false },
            ];
        }

        private void PopulateEdmBytes()
        {
            this.EdmBytes =
            [
                new EdmByte { Id = Byte.MinValue },
                new EdmByte { Id = Byte.MaxValue },
                new EdmByte { Id = 0 },
                new EdmByte { Id = 1 },
            ];
        }

        private void PopulateEdmDecimals()
        {
            this.EdmDecimals =
            [
                new EdmDecimal { Id = Decimal.MinValue },
                new EdmDecimal { Id = Decimal.MaxValue },
                new EdmDecimal { Id = Decimal.MinusOne },
                new EdmDecimal { Id = Decimal.One },
                new EdmDecimal { Id = Decimal.Zero },
            ];
        }

        private void PopulateEdmDoubles()
        {
            this.EdmDoubles =
            [
                new EdmDouble { Id = Double.Epsilon },
                new EdmDouble { Id = Double.MaxValue },
                new EdmDouble { Id = Double.MinValue },
                new EdmDouble { Id = Double.NaN },
                new EdmDouble { Id = Double.NegativeInfinity },
                new EdmDouble { Id = Double.PositiveInfinity },
                new EdmDouble { Id = 0 },
                new EdmDouble { Id = -1 },
                new EdmDouble { Id = 1 },
            ];
        }

        private void PopulateEdmSingles()
        {
            this.EdmSingles =
            [
                new EdmSingle { Id = Single.Epsilon },
                new EdmSingle { Id = Single.MaxValue },
                new EdmSingle { Id = Single.MinValue },
                new EdmSingle { Id = Single.NaN },
                new EdmSingle { Id = Single.NegativeInfinity },
                new EdmSingle { Id = Single.PositiveInfinity },
                new EdmSingle { Id = 0 },
                new EdmSingle { Id = -1 },
                new EdmSingle { Id = 1 },
            ];
        }

        private void PopulateEdmGuids()
        {
            this.EdmGuids =
            [
                new EdmGuid { Id = Guid.NewGuid() },
                new EdmGuid { Id = Guid.Empty },
            ];
        }

        private void PopulateEdmInt16s()
        {
            this.EdmInt16s =
            [
                new EdmInt16 { Id = Int16.MinValue },
                new EdmInt16 { Id = Int16.MaxValue },
                new EdmInt16 { Id = 0 },
                new EdmInt16 { Id = -1 },
                new EdmInt16 { Id = 1 },
            ];
        }

        private void PopulateEdmInt32s()
        {
            this.EdmInt32s =
            [
                new EdmInt32 { Id = Int32.MinValue },
                new EdmInt32 { Id = Int32.MaxValue },
                new EdmInt32 { Id = 0 },
                new EdmInt32 { Id = -1 },
                new EdmInt32 { Id = 1 },
            ];
        }

        private void PopulateEdmInt64s()
        {
            this.EdmInt64s =
            [
                new EdmInt64 { Id = Int64.MinValue },
                new EdmInt64 { Id = Int64.MaxValue },
                new EdmInt64 { Id = 0 },
                new EdmInt64 { Id = -1 },
                new EdmInt64 { Id = 1 },
            ];
        }

        private void PopulateEdmStrings()
        {
            this.EdmStrings =
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
            ];
        }

        private void PopulateEdmTimes()
        {
            this.EdmTimes =
            [
                new EdmTime { Id = TimeSpan.MinValue },
                new EdmTime { Id = TimeSpan.MaxValue },
                new EdmTime { Id = TimeSpan.Zero },
            ];
        }

        private void PopulateEdmDateTimeOffsets()
        {
            this.EdmDateTimeOffsets =
            [
                new EdmDateTimeOffset { Id = DateTimeOffset.MinValue },
                new EdmDateTimeOffset { Id = DateTimeOffset.MaxValue },
            ];
        }
    }
}
