//---------------------------------------------------------------------
// <copyright file="CommunityVocabularyTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Xunit;
using System.Threading.Tasks;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    public partial class CommunityVocabularyTests
    {

        [Fact]
        public async Task TestCommunityVocabularyModel_Async()
        {
            const string expectedUrlEscape = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Org.OData.Community.V1"" Alias=""Community"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""UrlEscapeFunction"" Type=""Core.Tag"" AppliesTo=""Function"">
    <Annotation Term=""Core.Description"" String=""Annotates a function to be substituted for a colon-escaped segment in a Url path"" />
  </Term>
</Schema>";

            var sw = new StringWriter();
            IEnumerable<EdmError> errors;
            using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, Async = true }))
            {
                var (result, errorsAsync) = await model.TryWriteSchemaAsync(xw).ConfigureAwait(false);
                Assert.True(result);

                errors = errorsAsync;
            }

            Assert.False(errors.Any(), "No Errors");

            string output = sw.ToString();
            Assert.True(expectedUrlEscape == output, "expected Community schema not matching");
        }
    }
}
