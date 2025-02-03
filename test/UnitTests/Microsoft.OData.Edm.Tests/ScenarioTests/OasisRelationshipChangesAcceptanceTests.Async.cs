//---------------------------------------------------------------------
// <copyright file="OasisRelationshipChangesAcceptanceTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xunit;

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
                var (result, errors) = await CsdlWriter.TryWriteCsdlAsync(this.representativeModel, writer, CsdlTarget.OData).ConfigureAwait(false);
                Assert.True(result);
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
