//---------------------------------------------------------------------
// <copyright file="Dev10TypeTestsClient.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Reflection;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Dev10TypeDef = AstoriaUnitTests.Tests.UnitTestModule.Dev10TypeTests;

    /// <summary>This is a test class for new types and language constructs in dev 10.</summary>
    [TestClass]
    public class Dev10TypeTests
    {
        [TestMethod]
        public void Dev10Type_ClientPost()
        {
            Tuple<Type, bool, string>[] typeList = new Tuple<Type, bool, string>[]
            {
                Tuple.Create(typeof(Dev10TypeDef.EntityWithDynamicComplexProperty), true, "Contains:The type 'AstoriaUnitTests.Tests.UnitTestModule+Dev10TypeTests+ComplexTypeWithDynamicInterface' is not supported by the client library."),
                Tuple.Create(typeof(Dev10TypeDef.EntityWithDynamicInterface),       true, "Contains:The type 'AstoriaUnitTests.Tests.UnitTestModule+Dev10TypeTests+EntityWithDynamicInterface' is not supported by the client library."),
                Tuple.Create(typeof(Dev10TypeDef.EntityWithDynamicProperties),      true, "Contains:The complex type 'System.Object' has no settable properties."),
                Tuple.Create(typeof(Dev10TypeDef.EntityWithTupleProperty),          true, "Contains:The type 'System.Tuple`2[System.String,System.String]' is not supported by the client library.")                
            };

            TestUtil.RunCombinations(typeList, (typedef) =>
            {
                this.GetType().GetMethod("PostToDev10TypeService", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typedef.Item1).Invoke(this, new object[] { typedef.Item2, typedef.Item3 });
            });
        }

        // Materialize from Dev 10 Types
        [TestMethod]
        public void Dev10Type_ClientQuery()
        {
            Tuple<Type, bool, string>[] typeList = new Tuple<Type, bool, string>[]
            {
                Tuple.Create(typeof(Dev10TypeDef.EntityWithBigIntProperty), true, "Contains:The property 'BigInt' on type 'AstoriaUnitTests.Tests.UnitTestModule_Dev10TypeTests_EntityWithBigIntProperty' is not a valid property. Make sure that the type of the property is a public type and a supported primitive type or a entity type with a valid key or a complex type."),

                // for this one we can't find any "static" properties since it will go through the dynamic interface
                Tuple.Create(typeof(Dev10TypeDef.EntityWithDynamicComplexProperty), true, "Contains:Internal Server Error. The property 'DynamicComplexProperty' is of type 'AstoriaUnitTests.Tests.UnitTestModule_Dev10TypeTests_EntityWithDynamicComplexProperty' which is an unsupported type."),
                                                                                                     
                Tuple.Create(typeof(Dev10TypeDef.EntityWithDynamicInterface), true,"Contains:"),
                Tuple.Create(typeof(Dev10TypeDef.EntityWithDynamicProperties), true,"Contains:"),
                Tuple.Create(typeof(Dev10TypeDef.EntityWithTupleProperty), true, "Contains:")
            };

            foreach (Tuple<Type, bool, string> typedef in typeList)
            {
                this.GetType().GetMethod("QueryDev10TypeService", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typedef.Item1).Invoke(this, new object[] { typedef.Item2, typedef.Item3 });
            }
        }

        [TestMethod]
        public void Dev10Type_ClientDynamicExpand()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(Dev10TypeDef.Dev10TypeEntitySet_Expand);
                web.StartService();
                string baseUri = web.BaseUri;

                DataServiceContext context = new DataServiceContext(new Uri(baseUri));
                var query = context.CreateQuery<Dev10TypeDef.EntityWithDynamicNavigation>("Parents").Expand("Children");
                try
                {
                    var results = query.ToList();
                    Assert.Fail("Exception failed to be thrown.");
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex;
                    while (innerEx.InnerException != null)
                    {
                        innerEx = innerEx.InnerException;
                    }

                    Assert.IsTrue(innerEx.Message.Contains("Internal Server Error. The type 'AstoriaUnitTests.Tests.UnitTestModule+Dev10TypeTests+EntityWithDynamicInterface' is not supported."), ex.InnerException.Message);
                }
            }
        }

        [TestMethod]
        public void Dev10Type_ClientQueryTupleWithALinq()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(Dev10TypeDef.Dev10TypeEntitySet<Dev10TypeDef.EntityWithTupleProperty>);
                web.StartService();
                string baseUri = web.BaseUri;

                DataServiceContext context = new DataServiceContext(new Uri(baseUri));
                var query = from t in context.CreateQuery<Dev10TypeDef.EntityWithTupleProperty>("Entities")
                            where t.ComplexTuple.Item1 == "value 1"
                            select t;
                try
                {
                    string queryUri = query.ToString();
                    Assert.Fail("Client ALINQ with Tuple failed to throw");
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex;
                    while (innerEx.InnerException != null)
                    {
                        innerEx = innerEx.InnerException;
                    }
                    Assert.AreEqual("The type 'System.Tuple`2[System.String,System.String]' is not supported by the client library.", innerEx.Message);
                }
            }
        }

        private void QueryDev10TypeService<T>(bool expectException, string exceptionMessage)
            where T : new()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(Dev10TypeDef.Dev10TypeEntitySet<T>);
                web.StartService();
                string baseUri = web.BaseUri;

                DataServiceContext context = new DataServiceContext(new Uri(baseUri));

                try
                {
                    var query = context.CreateQuery<T>("Entities");
                    var result = query.Execute();
                    Assert.IsFalse(expectException);
                    Assert.AreEqual(result.Count(), 3);
                }
                catch (Exception ex)
                {
                    // is exception expected?
                    Assert.IsTrue(expectException, "exception was not expected");
                    if (!String.IsNullOrEmpty(exceptionMessage))
                    {
                        Exception innerEx = ex;
                        while (innerEx.InnerException != null)
                        {
                            innerEx = innerEx.InnerException;
                        }

                        if (exceptionMessage.StartsWith("Contains:"))
                        {
                            exceptionMessage = exceptionMessage.Substring(9);
                            Assert.IsTrue(innerEx.Message.Contains(exceptionMessage), innerEx.Message);
                        }
                        else
                        {
                            Assert.AreEqual(ex.Message, exceptionMessage);
                        }
                    }
                }
            }
        }

        private void PostToDev10TypeService<T>(bool exceptionExpected, string exceptionMessage)
            where T : new()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(Dev10TypeDef.Dev10TypeEntitySet<T>);
                web.StartService();
                string baseUri = web.BaseUri;

                DataServiceContext context = new DataServiceContext(new Uri(baseUri));
                try
                {
                    context.AddObject("Entities", new T());
                    context.SaveChanges();
                    Assert.IsFalse(exceptionExpected);
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(exceptionExpected);

                    if (exceptionMessage != string.Empty)
                    {
                        Exception innerEx = ex;
                        while (innerEx.InnerException != null)
                        {
                            innerEx = innerEx.InnerException;
                        }

                        if (exceptionMessage.StartsWith("Contains:"))
                        {
                            exceptionMessage = exceptionMessage.Substring(9);
                            Assert.IsTrue(innerEx.Message.Contains(exceptionMessage));
                        }
                        else
                        {
                            Assert.AreEqual(ex.Message, exceptionMessage);
                        }
                    }
                }
            }
        }
    }
}