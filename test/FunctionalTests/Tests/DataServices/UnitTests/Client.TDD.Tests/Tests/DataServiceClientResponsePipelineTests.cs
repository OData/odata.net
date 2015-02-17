//---------------------------------------------------------------------
// <copyright file="DataServiceClientResponsePipelineTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataServiceClientResponsePipelineTests
    {
        [TestMethod]
        public void AllPublicConfigurationMethodsShouldThrowOnNullInput()
        {
            var pipeline = new DataServiceClientResponsePipelineConfiguration("foo");

            foreach (var publicMethodWithConfigurationArg in pipeline.GetType().GetMethods().Where(mi => mi.ReturnType == typeof(DataServiceClientResponsePipelineConfiguration)))
            {
                MethodInfo methodUnderTest = publicMethodWithConfigurationArg;
                var parameter = publicMethodWithConfigurationArg.GetParameters()[0];
                Action test = () => methodUnderTest.Invoke(pipeline, BindingFlags.Instance | BindingFlags.Public, null, new object[] { null }, CultureInfo.InvariantCulture);
                var fluentException = test.ShouldThrow<TargetInvocationException>().WithInnerException<ArgumentNullException>();
                fluentException.And.As<TargetInvocationException>().InnerException.As<ArgumentNullException>().ParamName.Should().Be(parameter.Name);
            }
        }
    }
}
