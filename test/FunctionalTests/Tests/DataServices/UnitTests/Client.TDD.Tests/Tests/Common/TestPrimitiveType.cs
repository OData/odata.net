//---------------------------------------------------------------------
// <copyright file="TestPrimitiveType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client;

namespace AstoriaUnitTests.Tests
{
    public class TestPrimitiveType
    {
        public string Data { get; set; }

        public override string ToString()
        {
            return Data;
        }
    }

    internal class TestTypeConverter : PrimitiveTypeConverter
    {
        public int ParseCall { get; set; }
        public int ToStringCall { get; set; }
        public int WriteAtomCall { get; set; }

        public TestTypeConverter()
        {
            WriteAtomCall = ParseCall = ToStringCall = 0;
        }

        internal override object Parse(string text)
        {
            ParseCall++;
            return new TestPrimitiveType() { Data = text };
        }

        internal override string ToString(object instance)
        {
            ToStringCall++;
            return instance.ToString();
        }
    }
}
