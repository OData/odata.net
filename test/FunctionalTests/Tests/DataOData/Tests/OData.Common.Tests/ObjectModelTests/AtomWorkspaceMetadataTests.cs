//---------------------------------------------------------------------
// <copyright file="AtomWorkspaceMetadataTests.cs" company="Microsoft">
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
    /// Tests for the AtomWorkspaceMetadata object model type.
    /// </summary>
    [TestClass, TestCase]
    public class AtomWorkspaceMetadataTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of the workspace.")]
        public void DefaultValuesTest()
        {
            AtomWorkspaceMetadata workspace = new AtomWorkspaceMetadata();
            this.Assert.IsNull(workspace.Title, "Expected null default value for property 'Title'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of the workspace.")]
        public void PropertyGettersAndSettersTest()
        {
            AtomTextConstruct title = new AtomTextConstruct();

            AtomWorkspaceMetadata workspace = new AtomWorkspaceMetadata()
            {
                Title = title,
            };

            this.Assert.AreSame(title, workspace.Title, "Expected reference equal values for property 'Title'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of the workspace.")]
        public void PropertySettersNullTest()
        {
            AtomWorkspaceMetadata workspace = new AtomWorkspaceMetadata()
            {
                Title = null,
            };

            this.Assert.IsNull(workspace.Title, "Expected null value for property 'Title'.");
        }
    }
}
