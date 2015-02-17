//---------------------------------------------------------------------
// <copyright file="AtomResourceCollectionMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the AtomResourceCollectionMetadata object model type.
    /// </summary>
    [TestClass, TestCase]
    public class AtomResourceCollectionMetadataTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of the resource collection.")]
        public void DefaultValuesTest()
        {
            AtomResourceCollectionMetadata resourceCollection = new AtomResourceCollectionMetadata();
            this.Assert.IsNull(resourceCollection.Title, "Expected null default value for property 'Title'.");
            this.Assert.IsNull(resourceCollection.Accept, "Expected null default value for property 'Accept'.");
            this.Assert.IsNull(resourceCollection.Categories, "Expected null default value for property 'Categories'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of the resource collection.")]
        public void PropertyGettersAndSettersTest()
        {
            AtomTextConstruct title = new AtomTextConstruct();
            string accept = "mime/type";
            AtomCategoriesMetadata categories = new AtomCategoriesMetadata();

            AtomResourceCollectionMetadata resourceCollection = new AtomResourceCollectionMetadata()
            {
                Title = title,
                Accept = accept,
                Categories = categories
            };

            this.Assert.AreSame(title, resourceCollection.Title, "Expected reference equal values for property 'Title'.");
            this.Assert.AreSame(accept, resourceCollection.Accept, "Expected reference equal values for property 'Accept'.");
            this.Assert.AreSame(categories, resourceCollection.Categories, "Expected reference equal values for property 'Categories'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of the resource collection.")]
        public void PropertySettersNullTest()
        {
            AtomResourceCollectionMetadata resourceCollection = new AtomResourceCollectionMetadata()
            {
                Title = null,
                Accept = null,
                Categories = null,
            };

            this.Assert.IsNull(resourceCollection.Title, "Expected null value for property 'Title'.");
            this.Assert.IsNull(resourceCollection.Accept, "Expected null value for property 'Accept'.");
            this.Assert.IsNull(resourceCollection.Categories, "Expected null value for property 'Categories'.");
        }
    }
}
