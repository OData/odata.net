//---------------------------------------------------------------------
// <copyright file="VCardReaderTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard.Test
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VCardReaderTest
    {
        [TestMethod]
        public void ReadItemTest()
        {
            var cases = new[]
            {
                new ItemTestCase()
                {
                    Name = "a",
                    Value = "b"
                },
                new ItemTestCase()
                {
                    Name = "X-DL",
                    ParamList = "Design Work Group",
                    Value = "List Item 1;List Item 2;List Item 3"
                },
                new ItemTestCase()
                {
                    Groups = "A0",
                    Name = "ADR",
                    ParamList = "DOM;HOME",
                    Value = "P.O. Box 101;Suite 101; 123 Main Street"
                },
            };

            foreach (var @case in cases)
            {
                var itemLine = @case.ToString();

                using (var sr = new StringReader(itemLine))
                {
                    var vr = new VCardReader(sr);
                    vr.Read();
                    Assert.AreEqual(VCardReaderState.Item, vr.State, string.Format("Case [{0}], State not match.", @case));
                    Assert.AreEqual(@case.Groups, vr.Groups, string.Format("Case [{0}], Groups not match.", @case));
                    Assert.AreEqual(@case.Name, vr.Name, string.Format("Case [{0}], Name not match.", @case));
                    Assert.AreEqual(@case.ParamList, vr.Params, string.Format("Case [{0}], ParamList not match.", @case));
                    Assert.AreEqual(@case.Value, vr.Value, string.Format("Case [{0}], Value not match.", @case));
                }
            }
        }

        private class ItemTestCase
        {
            public string Groups { get; set; }
            public string Name { get; set; }
            public string ParamList { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Value))
                {
                    throw new ApplicationException("Name and value are required");
                }

                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrEmpty(Groups))
                {
                    builder.Append(Groups);
                    builder.Append('.');
                }

                builder.Append(Name);

                if (!string.IsNullOrEmpty(this.ParamList))
                {
                    builder.Append(';');
                    builder.Append(this.ParamList);
                }

                builder.Append(':');
                builder.Append(Value);

                return builder.ToString();
            }
        }
    }
}
