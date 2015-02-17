//---------------------------------------------------------------------
// <copyright file="SelectExpandPathBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.OData.Client;
using Microsoft.OData.Client.Metadata;
using System.Linq;
using Microsoft.CSharp;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AstoriaUnitTests.Tests.ALinq
{
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>

    [TestClass]
    public class SelectExpandPathBuilderTests
    {
        private DataServiceContext dsc;
 
        public SelectExpandPathBuilderTests()
        {
            this.dsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
        }

        [TestMethod]
        public void SingleProjectionBecomesSelect()
        {
            PropertyInfo testProperty1Info = typeof(TestEntity).GetProperties().Single(x => x.Name == "TestProperty1");
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            pathBuilder.PushParamExpression(pe);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(testProperty1Info, null, this.dsc);
            pathBuilder.ProjectionPaths.Single().Should().Be("TestProperty1");
        }

        [TestMethod]
        public void MultipleSingleLevelProjectionsBecomeSelectClauses()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            pathBuilder.PushParamExpression(pe);
            foreach (PropertyInfo propertyInfo in typeof(TestEntity).GetProperties().Where(x => x.PropertyType == typeof(string)))
            {
                pathBuilder.StartNewPath();
                pathBuilder.AppendPropertyToPath(propertyInfo, null, this.dsc);
            }
            pathBuilder.ProjectionPaths.Should().HaveCount(2);
            pathBuilder.ProjectionPaths.Should().Contain(x => x == "TestProperty1")
                .And.Contain(x => x == "TestProperty2");
        }

        [TestMethod]
        public void SingleExpansionBecomesExpand()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter((typeof(TestEntity)));
            pathBuilder.PushParamExpression(pe);
            PropertyInfo navPropPropertyInfo = typeof(TestEntity).GetProperties().Single(x => x.PropertyType == typeof(SubTestEntity1));
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(navPropPropertyInfo, null, dsc);
            pathBuilder.ExpandPaths.Single().Should().Be("NavProp1");
        }

        [TestMethod]
        public void MultipleSingleLevelExpansionsBecomeExpandClauses()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            pathBuilder.PushParamExpression(pe);
            foreach (PropertyInfo propertyInfo in typeof(TestEntity).GetProperties().Where(x => x.PropertyType != typeof(string)))
            {
                pathBuilder.StartNewPath();
                pathBuilder.AppendPropertyToPath(propertyInfo, null, dsc);
            }
            pathBuilder.ExpandPaths.Should().HaveCount(2);
            pathBuilder.ExpandPaths.Should().Contain(x => x == "NavProp1")
                .And.Contain(x => x == "NavProp2");
        }

        [TestMethod]
        public void TypeIsPrePendedAsATypeSegment()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            PropertyInfo testProperty1Info = typeof(TestEntity).GetProperties().Single(x => x.Name == "TestProperty1");
            pathBuilder.PushParamExpression(pe);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(testProperty1Info, typeof(TestEntity), dsc);
            pathBuilder.ProjectionPaths.Single().Should().Be("AstoriaUnitTests.Tests.ALinq.SelectExpandPathBuilderTests+TestEntity/TestProperty1");
        }

        [TestMethod]
        public void ProjectionThroughMultipleNavPropsBecomeExpandOptions()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            PropertyInfo subTestEntityProperty = typeof(SubTestEntity1).GetProperties().Single(x => x.Name == "SubTestProperty");
            PropertyInfo navPropInfo = typeof(TestEntity).GetProperties().Single(x => x.Name == "NavProp1");
            pathBuilder.PushParamExpression(pe);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(navPropInfo, null, dsc);
            pathBuilder.AppendPropertyToPath(subTestEntityProperty, null, dsc);
            pathBuilder.ExpandPaths.Single().Should().Be("NavProp1($select=SubTestProperty)");
        }

        [TestMethod]
        public void ExpansionThroughMultipleNavPropsIsExpressedAsExpandOptions()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            PropertyInfo navPropInfo = typeof(TestEntity).GetProperties().Single(x => x.Name == "NavProp1");
            PropertyInfo subNavPropInfo = typeof(SubTestEntity1).GetProperties().Single(x => x.Name == "NavProp3");
            pathBuilder.PushParamExpression(pe);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(navPropInfo, null, dsc);
            pathBuilder.AppendPropertyToPath(subNavPropInfo, null, dsc);
            pathBuilder.ExpandPaths.Single().Should().Be("NavProp1($expand=NavProp3)");
        }

        [TestMethod]
        public void SelectingLowerLevelPropertyIsTranslatedIntoExpandOption()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            PropertyInfo navPropInfo = typeof(TestEntity).GetProperties().Single(x => x.Name == "NavProp1");
            PropertyInfo subNavPropInfo = typeof(SubTestEntity1).GetProperties().Single(x => x.Name == "NavProp3");
            PropertyInfo subTestProperty = typeof(SubTestEntity2).GetProperties().Single();
            pathBuilder.PushParamExpression(pe);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(navPropInfo, null, dsc);
            pathBuilder.AppendPropertyToPath(subNavPropInfo, null, dsc);
            pathBuilder.AppendPropertyToPath(subTestProperty, null, dsc);
            pathBuilder.ExpandPaths.Single().Should().Be("NavProp1($expand=NavProp3($select=SubTestProperty))");
        }

        [TestMethod]
        public void PushingANewParameterExpressionResetsTheBasePath()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe1 = Expression.Parameter(typeof(TestEntity), "pe1");
            ParameterExpression pe2 = Expression.Parameter(typeof(SubTestEntity1), "pe2");
            PropertyInfo navPropInfo = typeof(TestEntity).GetProperties().Single(x => x.Name == "NavProp1");
            PropertyInfo subNavPropInfo = typeof(SubTestEntity1).GetProperties().Single(x => x.Name == "NavProp3");
            PropertyInfo subTestProperty = typeof(SubTestEntity2).GetProperties().Single();
            pathBuilder.PushParamExpression(pe1);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(navPropInfo, null, dsc);
            pathBuilder.AppendPropertyToPath(subNavPropInfo, null, dsc);
            pathBuilder.PushParamExpression(pe2);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(subTestProperty, null, dsc);
            pathBuilder.ExpandPaths.Single().Should().Be("NavProp1($expand=NavProp3($select=SubTestProperty))");
        }

        [EntityType]
        [Key("testKey")]
        private class TestEntity
        {
            public string TestProperty1 { get; set; }
            public string TestProperty2 { get; set; }
            public SubTestEntity1 NavProp1 { get; set; }
            public SubTestEntity2 NavProp2 { get; set; }
        }

        [EntityType]
        [Key("testSubKey1")]
        private class SubTestEntity1
        {
            public string SubTestProperty { get; set; }
            public SubTestEntity2 NavProp3 { get; set; }
        }

        [EntityType]
        [Key("testSubKey2")]
        private class SubTestEntity2
        {
            public string SubTestProperty { get; set; }
        }
    }
}
