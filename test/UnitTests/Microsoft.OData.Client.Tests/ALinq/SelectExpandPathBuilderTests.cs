//---------------------------------------------------------------------
// <copyright file="SelectExpandPathBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>
    public class SelectExpandPathBuilderTests
    {
        private DataServiceContext dsc;
 
        public SelectExpandPathBuilderTests()
        {
            this.dsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
        }

        [Fact]
        public void SingleProjectionBecomesSelect()
        {
            PropertyInfo testProperty1Info = typeof(TestEntity).GetProperties().Single(x => x.Name == "TestProperty1");
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            pathBuilder.PushParamExpression(pe);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(testProperty1Info, null, this.dsc);
            Assert.Equal("TestProperty1", pathBuilder.ProjectionPaths.Single());
        }

        [Fact]
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

            Assert.Equal(2, pathBuilder.ProjectionPaths.Count());
            Assert.Contains("TestProperty1", pathBuilder.ProjectionPaths);
            Assert.Contains("TestProperty2", pathBuilder.ProjectionPaths);
        }

        [Fact]
        public void SingleExpansionBecomesExpand()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter((typeof(TestEntity)));
            pathBuilder.PushParamExpression(pe);
            PropertyInfo navPropPropertyInfo = typeof(TestEntity).GetProperties().Single(x => x.PropertyType == typeof(SubTestEntity1));
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(navPropPropertyInfo, null, dsc);
            Assert.Equal("NavProp1", pathBuilder.ExpandPaths.Single());
        }

        [Fact]
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

            Assert.Equal(2, pathBuilder.ExpandPaths.Count());
            Assert.Contains("NavProp1", pathBuilder.ExpandPaths);
            Assert.Contains("NavProp2", pathBuilder.ExpandPaths);
        }

        [Fact]
        public void TypeIsPrePendedAsATypeSegment()
        {
            SelectExpandPathBuilder pathBuilder = new SelectExpandPathBuilder();
            ParameterExpression pe = Expression.Parameter(typeof(TestEntity));
            PropertyInfo testProperty1Info = typeof(TestEntity).GetProperties().Single(x => x.Name == "TestProperty1");
            pathBuilder.PushParamExpression(pe);
            pathBuilder.StartNewPath();
            pathBuilder.AppendPropertyToPath(testProperty1Info, typeof(TestEntity), dsc);
            Assert.Equal("Microsoft.OData.Client.Tests.ALinq.SelectExpandPathBuilderTests+TestEntity/TestProperty1",
                pathBuilder.ProjectionPaths.Single());
        }

        [Fact]
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
            Assert.Equal("NavProp1($select=SubTestProperty)", pathBuilder.ExpandPaths.Single());
        }

        [Fact]
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
            Assert.Equal("NavProp1($expand=NavProp3)", pathBuilder.ExpandPaths.Single());
        }

        [Fact]
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
            Assert.Equal("NavProp1($expand=NavProp3($select=SubTestProperty))", pathBuilder.ExpandPaths.Single());

        }

        [Fact]
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
            Assert.Equal("NavProp1($expand=NavProp3($select=SubTestProperty))", pathBuilder.ExpandPaths.Single());
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
