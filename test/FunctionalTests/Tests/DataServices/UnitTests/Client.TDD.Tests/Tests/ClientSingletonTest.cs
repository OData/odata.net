//---------------------------------------------------------------------
// <copyright file="ClientSingletonTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientSingletonTest
    {
        private TestQueryable<Person> personQueryable;
        private TestQueryable<Customer> customerQueryable;
        private Uri baseUri;
        private DataServiceContext context;

        [TestInitialize]
        public void Init()
        {
            baseUri = new Uri("http://base.org/");
            context = new DataServiceContext(baseUri);
            personQueryable = this.CreateSingletonQuery<Person>("Vip");
            customerQueryable = this.CreateSingletonQuery<Customer>("Vip");
        }

        private TestQueryable<TElement> CreateSingletonQuery<TElement>(string singletonName)
        {
            SingletonResourceExpression exp = new SingletonResourceExpression(typeof(IOrderedQueryable<TElement>), null, Expression.Constant(singletonName), typeof(TElement), null, CountOption.None, null, null, null, null);
            DataServiceQueryProvider provider = new DataServiceQueryProvider(context);
            return new TestQueryable<TElement>(exp, provider);
        }

        [TestMethod]
        public void TestBase()
        {
            Assert.AreEqual("http://base.org/Vip", personQueryable.RootSingleton);
            Assert.AreEqual("http://base.org/Vip", customerQueryable.RootSingleton);
        }

        [TestMethod]
        public void TestWhere()
        {
            // Test Key
            Assert.AreEqual("http://base.org/Vip?$filter=PersonId eq 1", personQueryable.Where(p => p.PersonId == 1).ToString());
            Assert.AreEqual("http://base.org/Vip?$filter=PersonId eq 1", customerQueryable.Where(p => p.PersonId == 1).ToString());
            
            // Test NonKey
            Assert.AreEqual("http://base.org/Vip?$filter=FirstName eq 'Alex'", personQueryable.Where(p => p.FirstName == "Alex").ToString());
            Assert.AreEqual("http://base.org/Vip?$filter=FirstName eq 'Alex'", customerQueryable.Where(p => p.FirstName == "Alex").ToString());
            Assert.AreEqual("http://base.org/Vip?$filter=CustomerNumber eq '10086'", customerQueryable.Where(p => p.CustomerNumber == "10086").ToString());
            Assert.AreEqual("http://base.org/Vip?$filter=Card/CardNumber eq '10086'", customerQueryable.Where(p => p.Card.CardNumber == "10086").ToString());

            Assert.AreEqual("http://base.org/Vip?$filter=Microsoft.OData.Client.TDDUnitTests.Tests.Customer/CustomerNumber eq '1024'", personQueryable.Where(p => (p as Customer).CustomerNumber == "1024").ToString());
        }

        [TestMethod]
        public void TestExpand()
        {
            Assert.AreEqual("http://base.org/Vip?$expand=Order", personQueryable.Translate(personQueryable.Expand("Order")));
            Assert.AreEqual("http://base.org/Vip?$expand=FirstName", personQueryable.Translate(personQueryable.Expand(p => p.FirstName)));
            
            Assert.AreEqual("http://base.org/Vip?$expand=Microsoft.OData.Client.TDDUnitTests.Tests.Customer/Card", personQueryable.Translate(personQueryable.Expand(p => (p as Customer).Card)));
            Assert.AreEqual("http://base.org/Vip?$expand=Card", customerQueryable.Translate(customerQueryable.Expand("Card")));
            Assert.AreEqual("http://base.org/Vip?$expand=Card", customerQueryable.Translate(customerQueryable.Expand(p => p.Card)));
        }

        [TestMethod]
        public void TestSelect()
        {
            Assert.AreEqual("http://base.org/Vip/FirstName", personQueryable.Translate(personQueryable.Select(p => p.FirstName)));
            Assert.AreEqual("http://base.org/Vip?$select=PersonId,FirstName", personQueryable.Translate(personQueryable.Select(p => new Person { PersonId = p.PersonId, FirstName = p.FirstName })));
            Assert.AreEqual("http://base.org/Vip/Microsoft.OData.Client.TDDUnitTests.Tests.Customer/Card", personQueryable.Translate(personQueryable.Select(p => (p as Customer).Card)));

            Assert.AreEqual("http://base.org/Vip/PersonId", customerQueryable.Translate(customerQueryable.Select(p => p.PersonId)));
            Assert.AreEqual("http://base.org/Vip/CustomerNumber", customerQueryable.Translate(customerQueryable.Select(p => p.CustomerNumber)));
            Assert.AreEqual("http://base.org/Vip?$select=PersonId,FirstName,CustomerNumber,Card", 
                customerQueryable.Translate(customerQueryable.Select(p => new Customer()
                {
                    PersonId = p.PersonId, 
                    FirstName = p.FirstName,
                    CustomerNumber = p.CustomerNumber,
                    Card = p.Card
                })));
        }

        [TestMethod]
        public void TestSkip()
        {
            Assert.AreEqual("http://base.org/Vip?$skip=2", personQueryable.Skip(2).ToString());

            Assert.AreEqual("http://base.org/Vip?$skip=2", customerQueryable.Skip(2).ToString());
        }

        [TestMethod]
        public void TestExpandAndSkip()
        {
            Assert.AreEqual("http://base.org/Vip?$skip=2&$expand=Order", personQueryable.Expand("Order").Skip(2).ToString());

            Assert.AreEqual("http://base.org/Vip?$skip=2&$expand=Order", customerQueryable.Expand("Order").Skip(2).ToString());
        }

        [TestMethod]
        public void TestWhereAndExpand()
        {
            Assert.AreEqual("http://base.org/Vip?$filter=PersonId eq 1&$expand=Order", personQueryable.Expand("Order").Where(p => p.PersonId == 1).ToString());
            
            Assert.AreEqual("http://base.org/Vip?$filter=PersonId eq 1&$expand=Order", customerQueryable.Expand("Order").Where(p => p.PersonId == 1).ToString());
            Assert.AreEqual("http://base.org/Vip?$filter=CustomerNumber eq '100'&$expand=Order", customerQueryable.Expand("Order").Where(p => p.CustomerNumber == "100").ToString());
        }

        [TestMethod]
        public void TestSingleOrDefalut()
        {
            Assert.AreEqual("http://base.org/Vip", personQueryable.Translate(CreateExpression("SingleOrDefault", personQueryable)));

            Assert.AreEqual("http://base.org/Vip", customerQueryable.Translate(CreateExpression("SingleOrDefault", customerQueryable)));
        }

        [TestMethod]
        public void TestSingle()
        {
            Assert.AreEqual("http://base.org/Vip", personQueryable.Translate(CreateExpression("Single", personQueryable)));

            Assert.AreEqual("http://base.org/Vip", customerQueryable.Translate(CreateExpression("Single", customerQueryable)));
        }

        [TestMethod]
        public void TestFirst()
        {
            Assert.AreEqual("http://base.org/Vip", personQueryable.Translate(CreateExpression("First", personQueryable)));
            Assert.AreEqual("http://base.org/Vip", customerQueryable.Translate(CreateExpression("First", customerQueryable)));
        }

        [TestMethod]
        public void TestFirstOrDefault()
        {
            Assert.AreEqual("http://base.org/Vip", personQueryable.Translate(CreateExpression("FirstOrDefault", personQueryable)));

            Assert.AreEqual("http://base.org/Vip", customerQueryable.Translate(CreateExpression("FirstOrDefault", customerQueryable)));
        }

        [TestMethod]
        public void TestCount()
        {
            Assert.AreEqual("http://base.org/Vip/$count", personQueryable.Translate(CreateExpression("Count", personQueryable)));

            Assert.AreEqual("http://base.org/Vip/$count", customerQueryable.Translate(CreateExpression("Count", customerQueryable)));
        }

        [TestMethod]
        public void TestCreateSingletonQuery()
        {
            DataServiceContext context = new DataServiceContext(new Uri("http://base.org/"));
            var query = context.CreateSingletonQuery<Customer>("Vip");
            Assert.IsTrue(query.Expression is SingletonResourceExpression);
            Assert.AreEqual(typeof(Customer), query.ElementType);
        }

        private Expression CreateExpression<TElement>(string methodName, TestQueryable<TElement> queryable)
        {
            return Expression.Call(typeof(Queryable), methodName, new[] { queryable.ElementType }, queryable.Expression);
        }

        internal class TestQueryable<TElement> : IQueryable<TElement>
        {
            private readonly MethodInfo expandMethodInfo = typeof(DataServiceQuery<TElement>).GetMethod("Expand", new Type[] { typeof(string) });

            private readonly MethodInfo expandGenericMethodInfo = (MethodInfo)typeof(DataServiceQuery<TElement>).GetMember("Expand*").Single(m => ((MethodInfo)m).GetGenericArguments().Count() == 1);

            internal TestQueryable(Expression ex, DataServiceQueryProvider provider)
            {
                this.Expression = ex;
                this.queryProvider = provider;
            }

            public Expression Expression { get; private set; }

            private readonly DataServiceQueryProvider queryProvider;

            public IQueryProvider Provider
            {
                get { return this.queryProvider; }
            }

            public Type ElementType
            {
                get { return typeof(TElement); }
            }

            public IQueryable<TElement> Expand(string path)
            {
                // To Test Expand, we create the full expression manually and test CreateSingleQuery separately
                return this.queryProvider.CreateQuery<TElement>(Expression.Call(Expression.Convert(this.Expression, typeof(DataServiceQuery<TElement>.DataServiceOrderedQuery)), expandMethodInfo, new Expression[] { Expression.Constant(path) }));
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
            public IQueryable<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
            {
                MethodInfo mi = expandGenericMethodInfo.MakeGenericMethod(typeof(TTarget));
                return this.queryProvider.CreateQuery<TElement>(
                    Expression.Call(
                        Expression.Convert(this.Expression, typeof(DataServiceQuery<TElement>.DataServiceOrderedQuery)), 
                        mi, 
                        new Expression[] {navigationPropertyAccessor}));
            }

            public string RootSingleton
            {
                get
                {
                    return this.queryProvider.Translate(this.Expression).Uri.ToString();
                }
            }

            public string Translate(IQueryable iQueryable)
            {
                return this.Translate(iQueryable.Expression);
            }

            public string Translate(Expression expression)
            {
                return this.queryProvider.Translate(expression).Uri.ToString();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }

    class Customer : Person
    {
        public string CustomerNumber { get; set; }

        public Card Card { get; set; }
    }

    [Key("PersonId")]
    class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    class Card
    {
        public string CardNumber { get; set; }
    }
}
