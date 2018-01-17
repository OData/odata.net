//---------------------------------------------------------------------
// <copyright file="ODataStreamReferenceValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataStreamReferenceValue object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataStreamReferenceValueTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of ODataStreamReferenceValue.")]
        public void DefaultValuesTest()
        {
            ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();
            this.Assert.IsNull(mediaResource.ContentType, "Expected null default value for property 'ContentType'.");
            this.Assert.IsNull(mediaResource.EditLink, "Expected null default value for property 'EditLink'.");
            this.Assert.IsNull(mediaResource.ETag, "Expected null default value for property 'ETag'.");
            this.Assert.IsNull(mediaResource.ReadLink, "Expected null default value for property 'ReadLink'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of ODataStreamReferenceValue.")]
        public void PropertyGettersAndSettersTest()
        {
            Uri readLink = new Uri("http://odata.org/read");
            Uri editLink = new Uri("http://odata.org/edit");
            string etag = "MyETagValue";
            string contentType = "application/binary";

            ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue()
            {
                ContentType = contentType,
                EditLink = editLink,
                ETag = etag,
                ReadLink = readLink,
            };

            this.Assert.AreSame(contentType, mediaResource.ContentType, "Expected reference equal values for property 'ContentType'.");
            this.Assert.AreSame(editLink, mediaResource.EditLink, "Expected reference equal values for property 'EditLink'.");
            this.Assert.AreSame(etag, mediaResource.ETag, "Expected reference equal values for property 'ETag'.");
            this.Assert.AreSame(readLink, mediaResource.ReadLink, "Expected reference equal values for property 'ReadLink'.");
        }

        [TestMethod, Variation(Description = "Test setting properties to null.")]
        public void PropertySettersNullTest()
        {
            ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue()
            {
                ReadLink = new Uri("http://odata.org/read"),
                EditLink = new Uri("http://odata.org/edit"),
                ETag = "MyETagValue",
                ContentType = "application/binary"
            };

            mediaResource.ReadLink = null;
            mediaResource.EditLink = null;
            mediaResource.ETag = null;
            mediaResource.ContentType = null;

            this.Assert.IsNull(mediaResource.ContentType, "Expected null value for property 'ContentType'.");
            this.Assert.IsNull(mediaResource.EditLink, "Expected null value for property 'EditLink'.");
            this.Assert.IsNull(mediaResource.ETag, "Expected null value for property 'ETag'.");
            this.Assert.IsNull(mediaResource.ReadLink, "Expected null value for property 'ReadLink'.");
        }
    }
}
