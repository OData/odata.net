//---------------------------------------------------------------------
// <copyright file="Dev10TypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Numerics;
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        /// <summary>This is a test class for querying functionality.</summary>
        [TestClass, TestCase]
        public class Dev10TypeTests : AstoriaTestCase
        {
            #region Supporting Classes

            static int idGenerator = 0;

            public class EntityWithDynamicInterface : IDynamicMetaObjectProvider
            {
                public int ID { get; set; }
                public EntityWithDynamicInterface()
                {
                    this.ID = idGenerator++;
                }
                public DynamicMetaObject GetMetaObject(Expression parameter)
                {
                    return new MetaEntity(parameter);
                }
            }

            public class EntityWithDynamicAncestor : EntityWithDynamicInterface
            {
                public string ChildProp { get; set; }
            }

            public class MetaEntity : DynamicMetaObject
            {
                public MetaEntity(Expression parameters)
                    : base(parameters, BindingRestrictions.Empty)
                {
                }
            }

            public class EntityWithDynamicProperties
            {
                public int ID { get; set; }
                public dynamic DynamicProperty { get; set; }
                public EntityWithDynamicProperties()
                {
                    this.ID = idGenerator++; ;
                    this.DynamicProperty = 5;
                }
            }

            public class EntityWithDynamicNavigation
            {
                public int ID { get; set; }
                public EntityWithDynamicInterface Children { get; set; }
                public EntityWithDynamicNavigation()
                {
                    this.ID = idGenerator++;
                    this.Children = new EntityWithDynamicInterface();
                }
            }

            public class ComplexTypeWithDynamicInterface : IDynamicMetaObjectProvider
            {
                public DynamicMetaObject GetMetaObject(Expression parameter)
                {
                    return new MetaEntity(parameter);
                }
            }

            public class EntityWithDynamicComplexProperty
            {
                public int ID { get; set; }
                public ComplexTypeWithDynamicInterface DynamicComplexProperty { get; set; }
                public EntityWithDynamicComplexProperty()
                {
                    this.ID = idGenerator++;
                    this.DynamicComplexProperty = new ComplexTypeWithDynamicInterface();
                }
            }

            public class EntityWithTupleProperty
            {
                public int ID { get; set; }
                public Tuple<string, string> ComplexTuple { get; set; }
                public EntityWithTupleProperty()
                {
                    this.ID = idGenerator++;
                    this.ComplexTuple = new Tuple<string, string>("value 1", "value 2");
                }
            }

            public class EntityWithBigIntProperty
            {
                public int ID { get; set; }
                public BigInteger BigInt { get; set; }
                public EntityWithBigIntProperty()
                {
                    this.ID = idGenerator++;
                    this.BigInt = new BigInteger(1e10);
                }
            }

            #endregion

            public class Dev10TypeEntitySet_DynamicProperty
            {
                public static Func<EntityWithDynamicProperties> GetEntityInstance = null;
                public IQueryable<EntityWithDynamicProperties> Entities
                {
                    get
                    {
                        if (GetEntityInstance != null)
                        {
                            return new EntityWithDynamicProperties[] { GetEntityInstance() }.AsQueryable();
                        }
                        else
                        {
                            return new EntityWithDynamicProperties[] { new EntityWithDynamicProperties() }.AsQueryable();
                        }
                    }
                }
            }

            [TestMethod]
            public void Dev10Type_DynamicPropertyTypes()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("TestValues", new object[]{
                        Tuple.Create(5, 0, typeof(NotSupportedException), "The CLR Type 'System.Object' has no public properties and is not a supported resource type."),           // primitive value type
                        Tuple.Create(new EntityWithDynamicProperties(), 0, typeof(NotSupportedException), "The CLR Type 'System.Object' has no public properties and is not a supported resource type."), 
                        Tuple.Create("string", 0, typeof(NotSupportedException), "The CLR Type 'System.Object' has no public properties and is not a supported resource type."),     // primitive string type
                        Tuple.Create(new ComplexTypeWithDynamicInterface(), 0, typeof(NotSupportedException), "The CLR Type 'System.Object' has no public properties and is not a supported resource type."), 
                        Tuple.Create(new Tuple<int>(5), 0, typeof(NotSupportedException), "The CLR Type 'System.Object' has no public properties and is not a supported resource type."), 
                        Tuple.Create(new ExpandoObject(), 0, typeof(NotSupportedException), "The CLR Type 'System.Object' has no public properties and is not a supported resource type."), 
                        
                    }));

                using (TestWebRequest r = TestWebRequest.CreateForInProcess())
                {
                    r.DataServiceType = typeof(Dev10TypeEntitySet_DynamicProperty);
                    r.RequestUriString = "/Entities";

                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        dynamic value = values["TestValues"];

                        Dev10TypeEntitySet_DynamicProperty.GetEntityInstance =
                            () =>
                            {
                                return new EntityWithDynamicProperties()
                                {
                                    ID = idGenerator++,
                                    DynamicProperty = value.Item1
                                };
                            };

                        Exception ex = TestUtil.RunCatching(r.SendRequest);

                        TestUtil.AssertExceptionExpected(ex, true);
                        Assert.IsNotNull(ex, "Should throw if using type without any public properties");
                        Exception innerEx = ex.GetBaseException();
                        Assert.AreEqual(innerEx.Message, value.Item4, "Incorrect error message on using type without public properties");
                    });
                }
            }

            public class Dev10TypeEntitySet_Expand
            {
                private EntityWithDynamicNavigation[] parents;
                private EntityWithDynamicInterface[] children;
                public IQueryable<EntityWithDynamicNavigation> Parents
                {
                    get { return this.parents.AsQueryable(); }
                }
                public IQueryable<EntityWithDynamicInterface> Children
                {
                    get { return this.children.AsQueryable(); }
                }
                public Dev10TypeEntitySet_Expand()
                {
                    this.children = new EntityWithDynamicInterface[]{
                        new EntityWithDynamicInterface(), 
                        new EntityWithDynamicInterface()
                    };

                    this.parents = new EntityWithDynamicNavigation[]{
                        new EntityWithDynamicNavigation() { Children=this.children[0]},
                        new EntityWithDynamicNavigation() { Children=this.children[1]}
                    };
                }
            }

            [TestMethod]
            public void Dev10Type_DynamicEntityServiceExpand()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(Dev10TypeEntitySet_Expand);
                    request.RequestUriString = "/Parents?$expand=Children";
                    Exception ex = TestUtil.RunCatching(request.SendRequest);
                    Exception innerEx = ex;
                    while (innerEx.InnerException != null)
                    {
                        innerEx = innerEx.InnerException;
                    }
                    Assert.IsNotNull(innerEx);
                    Assert.AreEqual(innerEx.Message, "Internal Server Error. The type 'AstoriaUnitTests.Tests.UnitTestModule+Dev10TypeTests+EntityWithDynamicInterface' is not supported.");
                }
            }

            public class Dev10TypeEntitySet<T>
                where T : new()
            {
                private T[] entities;
                public IQueryable<T> Entities
                {
                    get { return entities.AsQueryable(); }
                }
                public Dev10TypeEntitySet()
                {
                    entities = new T[] { new T(), new T(), new T() };
                }
            }

            // disabled on port of OOB to Dev10
            [TestMethod]
            public void Dev10Type_DynamicEntityService()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("EntityType", new object[]
                    {
                        // entity with a property defined by 'dynamic' keyword (eq. of System.Object)
                        Tuple.Create(typeof(Dev10TypeEntitySet<EntityWithDynamicProperties>), 0, typeof(NotSupportedException), "The CLR Type 'System.Object' has no public properties and is not a supported resource type."),

                        // entity that implements IDMOP interface
                        Tuple.Create(typeof(Dev10TypeEntitySet<EntityWithDynamicInterface>), 0, typeof(InvalidOperationException), "Internal Server Error. The type 'AstoriaUnitTests.Tests.UnitTestModule+Dev10TypeTests+EntityWithDynamicInterface' is not supported."),
                        
                        // entity that inherits from another entity which implements IDMOP interface
                        Tuple.Create(typeof(Dev10TypeEntitySet<EntityWithDynamicAncestor>), 0, typeof(InvalidOperationException), "Internal Server Error. The type 'AstoriaUnitTests.Tests.UnitTestModule+Dev10TypeTests+EntityWithDynamicAncestor' is not supported."),
                        
                        // entity with a complex type that implements IDMOP
                        Tuple.Create(typeof(Dev10TypeEntitySet<EntityWithDynamicComplexProperty>), 0, typeof(InvalidOperationException), "Internal Server Error. The property 'DynamicComplexProperty' is of type 'AstoriaUnitTests.Tests.UnitTestModule_Dev10TypeTests_EntityWithDynamicComplexProperty' which is an unsupported type."),
                        
                        // entity with BigInt property
                        Tuple.Create(typeof(Dev10TypeEntitySet<EntityWithBigIntProperty>),0, typeof(InvalidOperationException), "The property 'BigInt' on type 'AstoriaUnitTests.Tests.UnitTestModule_Dev10TypeTests_EntityWithBigIntProperty' is not a valid property. Make sure that the type of the property is a public type and a supported primitive type or a entity type with a valid key or a complex type."),
                        
                        // entity with Tuple property
                        Tuple.Create(typeof(Dev10TypeEntitySet<EntityWithTupleProperty>), 0, typeof(InvalidOperationException), "Internal Server Error. The property 'ComplexTuple' is of type 'AstoriaUnitTests.Tests.UnitTestModule_Dev10TypeTests_EntityWithTupleProperty' which is an unsupported type.")
                    }));

                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    using (TestWebRequest r = TestWebRequest.CreateForInProcess())
                    {
                        dynamic value = values["EntityType"];

                        r.DataServiceType = value.Item1;
                        r.RequestUriString = "/Entities";
                        Exception ex = TestUtil.RunCatching(r.SendRequest);
                        TestUtil.AssertExceptionExpected(ex, true);
                        Assert.IsNotNull(ex, "Should throw if using type without any public properties");
                        Exception innerEx = ex.GetBaseException();
                        Assert.AreEqual(innerEx.Message, value.Item4, "Incorrect error message on using type without public properties");
                    }
                });
            }
        }
    }
}