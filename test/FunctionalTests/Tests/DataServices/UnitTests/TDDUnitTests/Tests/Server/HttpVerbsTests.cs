//---------------------------------------------------------------------
// <copyright file="HttpVerbsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using Microsoft.OData.Service;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpVerbsTests
    {
        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsQueryVerbReturnsTrueForHeadAndGet()
        {
            HttpVerbs.GET.IsQuery().Should().BeTrue();
        }

        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsQueryVerbReturnsFalseForAllOtherVerbs()
        {
            HttpVerbs.DELETE.IsQuery().Should().BeFalse();
            HttpVerbs.POST.IsQuery().Should().BeFalse();
            HttpVerbs.PATCH.IsQuery().Should().BeFalse();
            HttpVerbs.PUT.IsQuery().Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsChangeVerbReturnsFalseForHeadAndGet()
        {
            HttpVerbs.GET.IsChange().Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsChangeVerbReturnsTrueForAllOtherVerbs()
        {
            HttpVerbs.DELETE.IsChange().Should().BeTrue();
            HttpVerbs.POST.IsChange().Should().BeTrue();
            HttpVerbs.PATCH.IsChange().Should().BeTrue();
            HttpVerbs.PUT.IsChange().Should().BeTrue();
        }

        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsUpdateVerbReturnsTrueForPutMergeAndPatch()
        {
            HttpVerbs.PUT.IsUpdate().Should().BeTrue();
            HttpVerbs.PATCH.IsUpdate().Should().BeTrue();
        }

        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsUpdateVerbReturnsFalseForAllOtherVerbs()
        {
            HttpVerbs.GET.IsUpdate().Should().BeFalse();
            HttpVerbs.POST.IsUpdate().Should().BeFalse();
            HttpVerbs.DELETE.IsUpdate().Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsDeleteVerbReturnsTrueForDelete()
        {
            HttpVerbs.DELETE.IsDelete().Should().BeTrue();
        }

        [TestMethod]
        [TestCategory("Partition1")]
        public void VerifyIsDeleteVerbReturnsFalseForAllOtherVerbs()
        {
            HttpVerbs.GET.IsDelete().Should().BeFalse();
            HttpVerbs.POST.IsDelete().Should().BeFalse();
            HttpVerbs.PUT.IsDelete().Should().BeFalse();
            HttpVerbs.PATCH.IsDelete().Should().BeFalse();
        }
    }
}
