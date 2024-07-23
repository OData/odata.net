//---------------------------------------------------------------------
// <copyright file="OasisRelationshipChangesAcceptanceTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xunit;
using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm.Tests.ScenarioTests
{
    public partial class OasisRelationshipChangesAcceptanceTests
    {
        [Fact]
        public async Task WriterShouldContinueToWork_Async()
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, new XmlWriterSettings() { Async = true }))
            {
                Tuple<bool, IEnumerable<Edm.Validation.EdmError>> result = await CsdlWriter.TryWriteCsdlAsync(this.representativeModel, writer, CsdlTarget.OData).ConfigureAwait(false);
                bool success = result.Item1;
                IEnumerable<Edm.Validation.EdmError> errors = result.Item2;
                Assert.True(success);
                Assert.Empty(errors);
                await writer.FlushAsync().ConfigureAwait(false);
            }

            string actual = builder.ToString();
            var actualXml = XElement.Parse(actual);
            var actualNormalized = actualXml.ToString();

            Assert.Equal(RepresentativeEdmxDocument, actualNormalized);
        }
    }
}
