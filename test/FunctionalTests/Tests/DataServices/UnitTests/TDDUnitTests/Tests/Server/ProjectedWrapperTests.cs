//---------------------------------------------------------------------
// <copyright file="ProjectedWrapperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Internal;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProjectedWrapperTests
    {
        private readonly ProjectedWrapper0 p0 = new ProjectedWrapper0();
        private readonly ProjectedWrapper1 p1 = new ProjectedWrapper1();
        private readonly ProjectedWrapper2 p2 = new ProjectedWrapper2();
        private readonly ProjectedWrapper3 p3 = new ProjectedWrapper3();
        private readonly ProjectedWrapper4 p4 = new ProjectedWrapper4();
        private readonly ProjectedWrapper5 p5 = new ProjectedWrapper5();
        private readonly ProjectedWrapper6 p6 = new ProjectedWrapper6();
        private readonly ProjectedWrapper7 p7 = new ProjectedWrapper7();
        private readonly ProjectedWrapperMany pm = new ProjectedWrapperMany();

        [TestMethod]
        public void ProjectedWrapperShouldAllowNullResourceTypeName()
        {
            // should not throw
            this.p0.ResourceTypeName = null;
            this.p0.ResourceTypeName.Should().BeNull();

            this.p1.ResourceTypeName = null;
            this.p1.ResourceTypeName.Should().BeNull();

            this.p2.ResourceTypeName = null;
            this.p2.ResourceTypeName.Should().BeNull();

            this.p3.ResourceTypeName = null;
            this.p3.ResourceTypeName.Should().BeNull();

            this.p4.ResourceTypeName = null;
            this.p4.ResourceTypeName.Should().BeNull();

            this.p5.ResourceTypeName = null;
            this.p5.ResourceTypeName.Should().BeNull();

            this.p6.ResourceTypeName = null;
            this.p6.ResourceTypeName.Should().BeNull();

            this.p7.ResourceTypeName = null;
            this.p7.ResourceTypeName.Should().BeNull();

            this.pm.ResourceTypeName = null;
            this.pm.ResourceTypeName.Should().BeNull();
        }

        public void ProjectedWrapperShouldAllowEmptyResourceTypeName()
        {
            // should not throw
            this.p0.ResourceTypeName = string.Empty;
            this.p0.ResourceTypeName.Should().BeEmpty();

            this.p1.ResourceTypeName = string.Empty;
            this.p1.ResourceTypeName.Should().BeEmpty();

            this.p2.ResourceTypeName = string.Empty;
            this.p2.ResourceTypeName.Should().BeEmpty();

            this.p3.ResourceTypeName = string.Empty;
            this.p3.ResourceTypeName.Should().BeEmpty();

            this.p4.ResourceTypeName = string.Empty;
            this.p4.ResourceTypeName.Should().BeEmpty();

            this.p5.ResourceTypeName = string.Empty;
            this.p5.ResourceTypeName.Should().BeEmpty();

            this.p6.ResourceTypeName = string.Empty;
            this.p6.ResourceTypeName.Should().BeEmpty();

            this.p7.ResourceTypeName = string.Empty;
            this.p7.ResourceTypeName.Should().BeEmpty();

            this.pm.ResourceTypeName = string.Empty;
            this.pm.ResourceTypeName.Should().BeEmpty();
        }

        [TestMethod]
        public void ProjectedWrapperShouldAllowNullPropertyNameList()
        {
            // should not throw
            this.p0.PropertyNameList = null;
            this.p0.PropertyNameList.Should().BeNull();

            this.p1.PropertyNameList = null;
            this.p1.PropertyNameList.Should().BeNull();

            this.p2.PropertyNameList = null;
            this.p2.PropertyNameList.Should().BeNull();

            this.p3.PropertyNameList = null;
            this.p3.PropertyNameList.Should().BeNull();

            this.p4.PropertyNameList = null;
            this.p4.PropertyNameList.Should().BeNull();

            this.p5.PropertyNameList = null;
            this.p5.PropertyNameList.Should().BeNull();

            this.p6.PropertyNameList = null;
            this.p6.PropertyNameList.Should().BeNull();

            this.p7.PropertyNameList = null;
            this.p7.PropertyNameList.Should().BeNull();

            this.pm.PropertyNameList = null;
            this.pm.PropertyNameList.Should().BeNull();

            this.ValidateEmptyPropertyNames();
        }

        [TestMethod]
        public void ProjectedWrapperShouldAllowEmptyPropertyNameList()
        {
            // should not throw
            this.p0.PropertyNameList = string.Empty;
            this.p0.PropertyNameList.Should().BeEmpty();

            this.p1.PropertyNameList = string.Empty;
            this.p1.PropertyNameList.Should().BeEmpty();

            this.p2.PropertyNameList = string.Empty;
            this.p2.PropertyNameList.Should().BeEmpty();

            this.p3.PropertyNameList = string.Empty;
            this.p3.PropertyNameList.Should().BeEmpty();

            this.p4.PropertyNameList = string.Empty;
            this.p4.PropertyNameList.Should().BeEmpty();

            this.p5.PropertyNameList = string.Empty;
            this.p5.PropertyNameList.Should().BeEmpty();

            this.p6.PropertyNameList = string.Empty;
            this.p6.PropertyNameList.Should().BeEmpty();

            this.p7.PropertyNameList = string.Empty;
            this.p7.PropertyNameList.Should().BeEmpty();

            this.pm.PropertyNameList = string.Empty;
            this.pm.PropertyNameList.Should().BeEmpty();

            this.ValidateEmptyPropertyNames();
        }

        private void ValidateEmptyPropertyNames()
        {
            PrivateObject pp0 = new PrivateObject(this.p0, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp0.GetField("propertyNames"));

            PrivateObject pp1 = new PrivateObject(this.p1, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp1.GetField("propertyNames"));

            PrivateObject pp2 = new PrivateObject(this.p2, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp2.GetField("propertyNames"));

            PrivateObject pp3 = new PrivateObject(this.p3, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp3.GetField("propertyNames"));

            PrivateObject pp4 = new PrivateObject(this.p4, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp4.GetField("propertyNames"));

            PrivateObject pp5 = new PrivateObject(this.p5, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp5.GetField("propertyNames"));

            PrivateObject pp6 = new PrivateObject(this.p6, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp6.GetField("propertyNames"));

            PrivateObject pp7 = new PrivateObject(this.p7, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, pp7.GetField("propertyNames"));

            PrivateObject ppm = new PrivateObject(this.pm, new PrivateType(typeof(ProjectedWrapper).Assembly.FullName, typeof(ProjectedWrapper).FullName));
            Assert.AreEqual(WebUtil.EmptyStringArray, ppm.GetField("propertyNames"));
        }
    }
}