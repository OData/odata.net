//---------------------------------------------------------------------
// <copyright file="ODataEntryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataResource object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataEntryTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of an entry.")]
        public void DefaultValuesTest()
        {
            ODataResource entry = new ODataResource();
            this.Assert.IsNull(entry.ETag, "Expected null default value for property 'ETag'.");
            this.Assert.IsNull(entry.Id, "Expected null default value for property 'Id'.");
            this.Assert.IsNull(entry.EditLink, "Expected null default value for property 'EditLink'.");
            this.Assert.IsNull(entry.ReadLink, "Expected null default value for property 'ReadLink'.");
            this.Assert.IsNull(entry.Properties, "Expected null default value for property 'Properties'.");
            this.Assert.IsNull(entry.TypeName, "Expected null default value for property 'TypeName'.");
            this.Assert.IsNull(entry.MediaResource, "Expected null default value for property 'MediaResource'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of an entry.")]
        public void PropertyGettersAndSettersTest()
        {
            string etag = "ETag";
            Uri id = new Uri("http://odatalib.org/id");
            Uri readlink = new Uri("http://odatalib.org/readlink");
            Uri editlink = new Uri("http://odatalib.org/editlink");
            ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();

            ODataProperty primitiveProperty = new ODataProperty();
            ODataProperty complexProperty = new ODataProperty();
            ODataProperty multiValueProperty = new ODataProperty();
            ODataProperty namedStreamProperty = new ODataProperty();
            List<ODataProperty> properties = new List<ODataProperty>()
            {
                primitiveProperty,
                complexProperty,
                multiValueProperty,
                namedStreamProperty
            };

            string typeName = "ODataLibSample.DummyType";

            ODataResource entry = new ODataResource()
            {
                ETag = etag,
                Id = id,
                EditLink = editlink,
                Properties = properties,
                TypeName = typeName,
                MediaResource = mediaResource
            };

            this.Assert.IsNull(entry.ReadLink, "Expect ReadLink to be null if it was not set.");

            entry.ReadLink = readlink;

            this.Assert.AreSame(etag, entry.ETag, "Expected reference equal values for property 'ETag'.");
            this.Assert.AreSame(id, entry.Id, "Expected reference equal values for property 'Id'.");
            this.Assert.AreSame(readlink, entry.ReadLink, "Expected reference equal values for property 'ReadLink'.");
            this.Assert.AreSame(editlink, entry.EditLink, "Expected reference equal values for property 'EditLink'.");
            this.Assert.AreSame(properties, entry.Properties, "Expected reference equal values for property 'Properties'.");
            this.Assert.AreSame(typeName, entry.TypeName, "Expected reference equal values for property 'TypeName'.");
            this.Assert.AreSame(mediaResource, entry.MediaResource, "Expected reference equals for property 'MediaResource'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of an entry.")]
        public void PropertySettersNullTest()
        {
            ODataResource entry = new ODataResource()
            {
                ETag = null,
                Id = null,
                ReadLink = null,
                EditLink = null,
                Properties = null,
                TypeName = null,
                MediaResource = null,
            };

            this.Assert.IsNull(entry.ETag, "Expected null value for property 'ETag'.");
            this.Assert.IsNull(entry.Id, "Expected null value for property 'Id'.");
            this.Assert.IsNull(entry.ReadLink, "Expected null value for property 'ReadLink'.");
            this.Assert.IsNull(entry.EditLink, "Expected null value for property 'EditLink'.");
            this.Assert.IsNull(entry.Properties, "Expected null value for property 'Properties'.");
            this.Assert.IsNull(entry.TypeName, "Expected null value for property 'TypeName'.");
            this.Assert.IsNull(entry.MediaResource, "Expected null default value for property 'MediaResource'.");
        }
    }
}
