//---------------------------------------------------------------------
// <copyright file="AtomCategoriesMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the AtomCategoriesMetadata object model type.
    /// </summary>
    [TestClass, TestCase]
    public class AtomCategoriesMetadataTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of the categories.")]
        public void DefaultValuesTest()
        {
            AtomCategoriesMetadata categories = new AtomCategoriesMetadata();
            this.Assert.IsNull(categories.Fixed, "Expected null default value for property 'Fixed'.");
            this.Assert.IsNull(categories.Scheme, "Expected null default value for property 'Scheme'.");
            this.Assert.IsNull(categories.Href, "Expected null default value for property 'HRef'.");
            this.Assert.IsNull(categories.Categories, "Expected null default value for property 'Categories'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of the categories.")]
        public void PropertyGettersAndSettersTest()
        {
            bool fixedValue = true;
            string scheme = "http://odatalib.org/scheme";
            Uri href = new Uri("http://odatalib.org/href");
            List<AtomCategoryMetadata> categoriesList = new List<AtomCategoryMetadata> { new AtomCategoryMetadata() };

            AtomCategoriesMetadata categories = new AtomCategoriesMetadata()
            {
                Fixed = fixedValue,
                Scheme = scheme,
                Href = href,
                Categories = categoriesList
            };

            this.Assert.AreEqual(fixedValue, categories.Fixed, "Expected equal values for property 'Fixed'.");
            this.Assert.AreSame(scheme, categories.Scheme, "Expected reference equal values for property 'Scheme'.");
            this.Assert.AreSame(href, categories.Href, "Expected reference equal values for property 'HRef'.");
            this.Assert.AreSame(categoriesList, categories.Categories, "Expected reference equal values for property 'Categories'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of the categories.")]
        public void PropertySettersNullTest()
        {
            AtomCategoriesMetadata categories = new AtomCategoriesMetadata()
            {
                Fixed = null,
                Scheme = null,
                Href = null,
                Categories = null,
            };

            this.Assert.IsNull(categories.Fixed, "Expected null value for property 'Fixed'.");
            this.Assert.IsNull(categories.Scheme, "Expected null value for property 'Scheme'.");
            this.Assert.IsNull(categories.Href, "Expected null value for property 'Href'.");
            this.Assert.IsNull(categories.Categories, "Expected null value for property 'Categories'.");
        }
    }
}
