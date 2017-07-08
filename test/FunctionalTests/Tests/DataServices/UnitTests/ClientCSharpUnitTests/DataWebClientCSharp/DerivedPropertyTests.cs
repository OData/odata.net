//---------------------------------------------------------------------
// <copyright file="DerivedPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.DerivedProperty
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Edm;
    using System.IO;
    using System.Linq;
    using AstoriaUnitTests.ClientExtensions;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using t = System.Data.Test.Astoria;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [TestClass]
    public class DerivedPropertyClientTests
    {
        private Version V4 = new Version(4, 0);

        #region Linq Tests

        #region TypeFilter tests

        [Ignore] // Remove Atom
        // [TestMethod]
        public void TypeFilterTests()
        {
            SendRequestAndVerifyUriAndContext(ModelWithDerivedNavigationProperties(), GetTypeFilterTests);
        }

        IEnumerable<Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>> GetTypeFilterTests()
        {
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>() select p,
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>().OfType<Employee>() select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(e => e.ID == 2).SelectMany(e => (e as Employee).Manager.DirectReports),
                    c => c.CreateQuery<Person>("People").Where(e => e.ID == 2).Select(e => (e as Employee).Manager).SelectMany(m => m.DirectReports),
                },
                c => c.BaseUri + "People(2)/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(e => e.ID == 2).Select(e => e).OfType<Employee>().SelectMany(e => e.Manager.DirectReports),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Where(e => e.ID == 2).SelectMany(e => e.Manager.DirectReports),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Where(e => e.ID == 2).Select(e => e.Manager).SelectMany(m => m.DirectReports)
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee(2)/Manager/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 2).SelectMany(p => (p as Employee).Manager.DirectReports).Where(e => e.ID == 2).SelectMany(e => (e as PeopleManager).DirectReports),
                },
                c => c.BaseUri + "People(2)/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/DirectReports(2)/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 2).SelectMany(p => (p as Employee).Manager.DirectReports).OfType<PeopleManager>().Where(m => m.ID == 2).SelectMany(m => m.DirectReports),
                },
                c => c.BaseUri + "People(2)/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/DirectReports/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager(2)/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Where(e => e.ID == 2).SelectMany(e => e.Manager.DirectReports).Where(e => e.ID == 2).SelectMany(e => (e as PeopleManager).DirectReports),
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee(2)/Manager/DirectReports(2)/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Where(e => e.ID == 2).SelectMany(e => e.Manager.DirectReports).OfType<PeopleManager>().Where(e => e.ID == 2).SelectMany(m => m.DirectReports),
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee(2)/Manager/DirectReports/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager(2)/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 5).SelectMany(p => (p as Employee).Manager.DirectReports).Where(e => e.ID == 2).SelectMany(e => (e as PeopleManager).DirectReports),
                },
                c => c.BaseUri + "People(5)/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/DirectReports(2)/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 5).SelectMany(p => (p as Employee).Manager.DirectReports).OfType<PeopleManager>().Where(m => m.ID == 2).SelectMany(m => m.DirectReports),
                },
                c => c.BaseUri + "People(5)/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/DirectReports/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager(2)/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 5).SelectMany(p => (((p.BestFriend as Employee).Manager as PeopleManager).BestFriend as Employee).Manager.DirectReports),
                },
                c => c.BaseUri + "People(5)/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/DirectReports",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 5).SelectMany(p => (((p.BestFriend as Employee).Manager as PeopleManager).BestFriend as Employee).Manager.DirectReports).OfType<PeopleManager>(),
                },
                c => c.BaseUri + "People(5)/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/DirectReports/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager",
                (c, q, o) =>
                {
                    Assert.AreEqual(1, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People")
                         where p.ID == 1
                         from pp in p.Aquaintances.OfType<PeopleManager>()
                         select pp,
                },
                c => c.BaseUri + "People(1)/Aquaintances/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People")
                         where p.ID == 1
                         from pp in p.Aquaintances.OfType<Employee>()
                         select pp,
                },
                c => c.BaseUri + "People(1)/Aquaintances/AstoriaUnitTests.Tests.DerivedProperty.Employee",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People")
                         where p.ID == 1
                         from pp in p.Aquaintances.OfType<PeopleManager>().Cast<Employee>()
                         select pp,
                },
                c => c.BaseUri + "People(1)/Aquaintances/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People")
                         where p.ID == 1
                         from pp in p.Aquaintances.OfType<PeopleManager>().Select(ppp => ppp)
                            .Cast<Employee>().Select(ppp => ppp)
                            .OfType<Employee>().Select(ppp => ppp)
                            .Cast<Person>().Select(ppp => ppp)
                            .OfType<Person>().Select(ppp => ppp)
                         select pp,
                },
                c => c.BaseUri + "People(1)/Aquaintances/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );
        }

        #endregion TypeFilter tests

        #region OrderBy tests

        [Ignore] // Remove Atom
        // [TestMethod]
        public void OrderByTests()
        {
            SendRequestAndVerifyUriAndContext(ModelWithDerivedNavigationProperties(), GetOrderByTests);
        }

        IEnumerable<Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>> GetOrderByTests()
        {
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() orderby (p as Employee).Manager.ID select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$orderby=Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() orderby (p as Employee).Manager.ID, (p as Employee).Manager.Name select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$orderby=Manager/ID,Manager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") orderby (p as Employee).Manager.ID select p,
                },
                c => c.BaseUri + "People?$orderby=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") orderby (p as Employee).Manager.ID, (p as Employee).Manager.Name select p,
                },
                c => c.BaseUri + "People?$orderby=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/ID,AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() orderby (p as PeopleManager).Manager.ID select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$orderby=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                            {
                                c => from p in c.CreateQuery<Person>("People").OfType<Employee>() orderby (p as PeopleManager).Manager.ID, (p as PeopleManager).Manager.Name select p,
                            },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$orderby=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") orderby ((p as Employee).Manager as PeopleManager).Manager.ID select p,
                },
                c => c.BaseUri + "People?$orderby=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") orderby ((p as Employee).Manager as PeopleManager).Manager.ID, ((p as Employee).Manager as PeopleManager).Manager.Name select p,
                },
                c => c.BaseUri + "People?$orderby=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/Manager/ID,AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/Manager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") orderby ((p as PeopleManager).Manager as Employee).Manager.ID select p,
                },
                c => c.BaseUri + "People?$orderby=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") orderby ((p as PeopleManager).Manager as Employee).Manager.ID, ((p as PeopleManager).Manager as Employee).Manager.Name select p,
                },
                c => c.BaseUri + "People?$orderby=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/Manager/ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/Manager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() orderby ((p as Employee).Manager as PeopleManager).Manager.ID select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$orderby=Manager/Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() orderby ((p as Employee).Manager as PeopleManager).Manager.ID, ((p as Employee).Manager as PeopleManager).Manager.Name select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$orderby=Manager/Manager/ID,Manager/Manager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            // Server: Null propogation not handled when handling segments after a navigation reference property, resulting in NRE
            // Resolved as Won't Fix
            //yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
            //    new Func<DataServiceContext, IQueryable>[]
            //    {
            //        c => from p in c.CreateQuery<Person>("People").OfType<Employee>() orderby (((p as Employee).BestFriend as Employee).Manager.BestFriend as PeopleManager).Manager.ID select p,
            //    },
            //    c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$orderby=BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/ID",
            //    (c, q, o) =>
            //    {
            //        Assert.AreEqual(5, c.Entities.Count);
            //        Assert.AreEqual(0, c.Links.Count);
            //        VerifyNonProjectedLinkInfoCount(c);
            //    }
            //);

            // Server: Null propogation not handled when handling segments after a navigation reference property, resulting in NRE
            // Resolved as Won't Fix
            //yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
            //    new Func<DataServiceContext, IQueryable>[]
            //    {
            //        c => c.CreateQuery<Person>("People").OfType<Employee>().Where(e => e.ID == 2).SelectMany(e => e.Manager.DirectReports).OfType<PeopleManager>().Where(e => e.ID == 2).SelectMany(m => m.DirectReports).OrderBy(e => (((e as Employee).BestFriend as Employee).Manager.BestFriend as PeopleManager).Manager.ID),
            //    },
            //    c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee(2)/Manager/DirectReports/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager(2)/DirectReports?$orderby=BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/ID",
            //    (c, q, o) =>
            //    {
            //        Assert.AreEqual(2, c.Entities.Count);
            //        Assert.AreEqual(0, c.Links.Count);
            //        VerifyNonProjectedLinkInfoCount(c);
            //    }
            //);
        }

        #endregion OrderBy tests

        #region Filter tests
        [Ignore] // Remove Atom
        // [TestMethod]
        public void FilterTests()
        {
            SendRequestAndVerifyUriAndContext(ModelWithDerivedNavigationProperties(), GetFilterTests);
        }

        IEnumerable<Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>> GetFilterTests()
        {
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>() where (p as Employee).ID == 2 select p,
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>() where (p as Person).ID == 2 select p,
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>() where (p as PeopleManager as Employee as Person).ID == 2 select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager(2)",
                (c, q, o) =>
                {
                    Assert.AreEqual(1, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() where (p as PeopleManager).ID == 2 select p,
                    c => from p in c.CreateQuery<Person>("People").OfType<Employee>() where (p as Employee as Person as PeopleManager).ID == 2 select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$filter=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/ID eq 2",
                (c, q, o) =>
                {
                    Assert.AreEqual(1, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") where (p as Employee).Manager.ID > 0 select p,
                    c => from p in c.CreateQuery<Person>("People") where (p as Employee as Person as Employee).Manager.ID > 0 select p
                },
                c => c.BaseUri + "People?$filter=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/ID gt 0",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People") where (p as PeopleManager).ID > 0 && (p as Employee).Name != "Nil the Nameless" orderby (p as Employee).Manager.ID select p
                },
                c => c.BaseUri + "People?$filter=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/ID gt 0 and AstoriaUnitTests.Tests.DerivedProperty.Employee/Name ne 'Nil the Nameless'&$orderby=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>() where (p as PeopleManager).ID > 0 && (p as PeopleManager).Name != "Nil the Nameless" orderby (p as PeopleManager).Manager.ID select p,
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>() where (p as Employee).ID > 0 && (p as Employee).Name != "Nil the Nameless" orderby (p as Employee).Manager.ID select p,
                    c => from p in c.CreateQuery<Person>("People").OfType<PeopleManager>() where (p as Person).ID > 0 && (p as Person).Name != "Nil the Nameless" orderby (p as Employee).Manager.ID select p,
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager?$filter=ID gt 0 and Name ne 'Nil the Nameless'&$orderby=Manager/ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            // Server: Null propogation not handled when handling segments after a navigation reference property, resulting in NRE
            // Resolved as Won't Fix
            //yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
            //    new Func<DataServiceContext, IQueryable>[]
            //    {
            //        c => from p in c.CreateQuery<Person>("People") where (((p as PeopleManager).Manager as Employee).BestFriend as Employee).ID > 0 orderby ((p as PeopleManager).Manager as Employee).Manager.ID select p,
            //    },
            //    c => c.BaseUri + "People?$filter=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/ID gt 0&$orderby=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/Manager/ID",
            //    (c, q, o) =>
            //    {
            //        Assert.AreEqual(2, c.Entities.Count);
            //        Assert.AreEqual(0, c.Links.Count);
            //        VerifyNonProjectedLinkInfoCount(c);
            //    }
            //);

            // Server: Null propogation not handled when handling segments after a navigation reference property, resulting in NRE
            // Resolved as Won't Fix
            //yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
            //    new Func<DataServiceContext, IQueryable>[]
            //    {
            //        c => from p in c.CreateQuery<Person>("People") where (((p as Employee).BestFriend as Employee).Manager.BestFriend as PeopleManager).Manager.ID > 0 orderby (((p as Employee).BestFriend as Employee).Manager.BestFriend as PeopleManager).Manager.ID select p as Employee,
            //    },
            //    c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$filter=AstoriaUnitTests.Tests.DerivedProperty.Employee/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/ID gt 0&$orderby=AstoriaUnitTests.Tests.DerivedProperty.Employee/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager/BestFriend/AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Manager/ID",
            //    (c, q, o) =>
            //    {
            //        Assert.AreEqual(1, c.Entities.Count);
            //        Assert.AreEqual(0, c.Links.Count);
            //        VerifyNonProjectedLinkInfoCount(c);
            //    }
            //);
        }

        #endregion Filter tests

        #region Expand tests
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ExpandTests()
        {
            SendRequestAndVerifyUriAndContext(ModelWithDerivedNavigationProperties(), GetExpandTests);
        }

        IEnumerable<Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>> GetExpandTests()
        {
            // expand resource reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Expand(p => (p as Employee).Manager),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(5, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            // expand resource set reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Expand(p => ((p as Employee).Manager.BestFriend as PeopleManager).DirectReports).OfType<Employee>(),
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($expand=BestFriend($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports))",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(9, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            // multiple expands
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Expand(p => ((p as Employee).Manager.BestFriend as PeopleManager).DirectReports).Expand(p => (p as Employee).Manager),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($expand=BestFriend($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(9, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            // mixing v1 expand with new expand
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Expand(p => ((p as Employee).Manager.BestFriend as PeopleManager).DirectReports).Expand("BestFriend").Expand(p => (p as Employee).Manager),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($expand=BestFriend($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports)),BestFriend,AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(13, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );

            // mixing v1 expand with new expand on different part of the tree
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => ((DataServiceQuery<PeopleManager>)((DataServiceQuery<Employee>)c.CreateQuery<Person>("People").OfType<Employee>().Where(e => e.ID == 2)).Expand(p => p.Manager).Expand("BestFriend").Select(e => e.Manager)).Expand(p => ((p as Employee).Manager.BestFriend as PeopleManager).DirectReports).Expand("BestFriend"),
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee(2)/Manager?$expand=Manager,BestFriend,Manager($expand=BestFriend($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports))",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(2, c.Links.Count);
                    VerifyNonProjectedLinkInfoCount(c);
                }
            );
        }

        #endregion Expand tests

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ProjectionTests()
        {
            SendRequestAndVerifyUriAndContext(ModelWithDerivedNavigationProperties(), GetEntityProjectionTests);
            SendRequestAndVerifyUriAndContext(ModelWithDerivedNavigationProperties(), GetNonEntityProjectionTests);
        }

        IEnumerable<Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>> GetEntityProjectionTests()
        {
            // Projecting whole entity reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 1 || p.ID == 2 || p.ID == 6).Select(p => new Employee { ID = p.ID, Manager = (p as Employee).Manager}),
                },
                c => c.BaseUri + "People?$filter=ID eq 1 or ID eq 2 or ID eq 6&$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(4, c.Entities.Count);
                    Assert.AreEqual(2, c.Links.Count);
                }
            );

            // Projecting whole entity set reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 1 || p.ID == 2 || p.ID == 6).Select(p => new PeopleManager { ID = p.ID, DirectReports = (p as PeopleManager).DirectReports}),
                },
                c => c.BaseUri + "People?$filter=ID eq 1 or ID eq 2 or ID eq 6&$expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(2, c.Links.Count);
                }
            );

            // Projecting narrow entity reference property, TypeAs after the nav prop.
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new MyEmployeeEntity { ID = e.ID, Manager = e.Manager == null ? null : new MyEmployeeEntity { ID = (e.Manager as Person).ID, Name = (e.Manager as Employee).Name }}),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new MyEmployeeEntity { ID = e.ID, Manager = e.Manager == null ? null : new MyEmployeeEntity { ID = (e.Manager as Person as PeopleManager as Employee).ID, Name = (e.Manager as Employee as Person).Name }}),
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$expand=Manager($select=ID),Manager($select=Name)&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(5, c.Entities.Count);
                    Assert.AreEqual(5, c.Links.Count);
                }
            );

            // Projecting narrow entity reference property, TypeAs before the nav prop.
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Manager = (e as Employee).Manager == null ? null : new MyEmployeeEntity { ID = (e as Employee).Manager.ID, Name = (e as Employee).Manager.Name }}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Manager = (e as Employee as Person as Employee).Manager == null ? null : new MyEmployeeEntity { ID = (e as Employee as Person as Employee).Manager.ID, Name = (e as Employee as Person as Employee).Manager.Name }}),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($select=Name)&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(5, c.Links.Count);
                }
            );

            // Projecting narrow entity set reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new MyEmployeeEntity { ID = cw.ID, Name = cw.Name }).ToList() }),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=Name)&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(4, c.Links.Count);
                }
            );

            // Projecting narrow entity set reference property, TypeAs in the nested select
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new MyEmployeeEntity { ID = cw.ID, Name = (cw as PeopleManager).Name }).ToList() }),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Name)&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(4, c.Links.Count);
                }
            );

            // Projecting narrow entity set reference property, TypeAs in the nested select
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new MyEmployeeEntity { ID = cw.ID, Name = (cw as PeopleManager).Name, DirectReports = (cw as PeopleManager).DirectReports == null ? null : (cw as PeopleManager).DirectReports.Select(dr => new MyEmployeeEntity { ID = dr.ID, Name = (dr as Employee).Name, Manager = new MyEmployeeEntity { ID = (dr as Employee).Manager.ID, Name = (dr as Employee).Manager.Name} }).ToList() }).ToList() }),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Name),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($select=ID)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($select=Name)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($expand=Manager($select=ID))),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($expand=Manager($select=Name)))&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(8, c.Links.Count);
                }
            );

            // TypeAs PeopleManager before a key and a primitive property so they would be null for Person and Employee
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = (e as PeopleManager).ID, Name = (e as PeopleManager).Name}),
                },
                c => c.BaseUri + "People?$select=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in c.Entities)
                    {
                        if (p.ServerTypeName == typeof(PeopleManager).FullName)
                        {
                            Assert.IsNotNull(((MyEmployeeEntity)p.Entity).Name);
                            Assert.IsTrue(((MyEmployeeEntity)p.Entity).ID > 0);
                        }
                        else if (p.ServerTypeName != typeof(PeopleManager).FullName)
                        {
                            Assert.IsNull(((MyEmployeeEntity)p.Entity).Name);
                            Assert.AreEqual(0, ((MyEmployeeEntity)p.Entity).ID);
                        }
                        else
                        {
                            Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before complex property so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(p => new Employee { ID = p.ID, Office = (p as PeopleManager).Office}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in c.Entities)
                    {
                        if (p.ServerTypeName == typeof(PeopleManager).FullName)
                        {
                            Assert.IsNotNull(((Employee)p.Entity).Office.Building);
                            Assert.IsFalse(((Employee)p.Entity).Office.OfficeNumber == 0);
                        }
                        else if (p.ServerTypeName != typeof(PeopleManager).FullName)
                        {
                            Assert.IsNull(((Employee)p.Entity).Office);
                        }
                        else
                        {
                            Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before complex property so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Office = (e as PeopleManager).Office}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in c.Entities)
                    {
                        if (p.ServerTypeName == typeof(PeopleManager).FullName)
                        {
                            Assert.IsNotNull(((MyEmployeeEntity)p.Entity).Office.Building);
                            Assert.IsFalse(((MyEmployeeEntity)p.Entity).Office.OfficeNumber == 0);
                        }
                        else if (p.ServerTypeName != typeof(PeopleManager).FullName)
                        {
                            Assert.IsNull(((MyEmployeeEntity)p.Entity).Office);
                        }
                        else
                        {
                            Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before bag of primitives so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Skills = (e as PeopleManager).Skills}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Skills",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in c.Entities)
                    {
                        if (p.ServerTypeName == typeof(PeopleManager).FullName)
                        {
                            Assert.IsNotNull(((MyEmployeeEntity)p.Entity).Skills);
                        }
                        else if (p.ServerTypeName != typeof(PeopleManager).FullName)
                        {
                            Assert.IsNull(((MyEmployeeEntity)p.Entity).Skills);
                        }
                        else
                        {
                            Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before bag of complex so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new Employee { ID = e.ID, Vacations = (e as PeopleManager).Vacations}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Vacations",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in c.Entities)
                    {
                        if (p.ServerTypeName == typeof(PeopleManager).FullName)
                        {
                            Assert.IsNotNull(((Employee)p.Entity).Vacations);
                        }
                        else if (p.ServerTypeName != typeof(PeopleManager).FullName)
                        {
                            Assert.IsNull(((Employee)p.Entity).Vacations);
                        }
                        else
                        {
                            Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before named stream so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new Employee { ID = e.ID, Photo = (e as PeopleManager).Photo}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Photo = (e as PeopleManager).Photo}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Photo",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in c.Entities)
                    {
                        DataServiceStreamLink photo;
                        if (p.Entity.GetType() == typeof(Employee))
                        {
                            photo = ((Employee)p.Entity).Photo;
                        }
                        else
                        {
                            photo = ((MyEmployeeEntity)p.Entity).Photo;
                        }

                        if (p.ServerTypeName == typeof(PeopleManager).FullName)
                        {
                            Assert.IsNotNull(photo);
                            t.TestUtil.AssertContains(photo.EditLink.OriginalString, p.ServerTypeName);
                            if (photo.SelfLink != null)
                            {
                                t.TestUtil.AssertContains(photo.SelfLink.OriginalString, p.ServerTypeName);
                            }
                        }
                        else if (p.ServerTypeName != typeof(PeopleManager).FullName)
                        {
                            Assert.IsNull(photo);
                        }
                        else
                        {
                            Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before bag of complex so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeEntity { ID = e.ID, Vacations = (e as PeopleManager).Vacations == null ? null : (e as PeopleManager).Vacations}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Vacations",
                (c, q, o) =>
                {
                    Assert.AreEqual(6, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in c.Entities)
                    {
                        if (p.ServerTypeName == typeof(PeopleManager).FullName)
                        {
                            Assert.IsNotNull(((MyEmployeeEntity)p.Entity).Vacations);
                        }
                        else if (p.ServerTypeName != typeof(PeopleManager).FullName)
                        {
                            Assert.IsNull(((MyEmployeeEntity)p.Entity).Vacations);
                        }
                        else
                        {
                            Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        }
                    }
                }
            );
        }

        IEnumerable<Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>> GetNonEntityProjectionTests()
        {
            // Projecting whole entity reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 1 || p.ID == 2 || p.ID == 6).Select(p => new { ID = p.ID, Manager = (p as Employee).Manager}),
                },
                c => c.BaseUri + "People?$filter=ID eq 1 or ID eq 2 or ID eq 6&$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(1, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    PeopleManager shyam = (PeopleManager)c.Entities.Single().Entity;
                    Assert.AreEqual("Shyam", shyam.Name);
                    Assert.AreEqual(3, o.Count);
                    Assert.AreEqual(2, o.Where(p => p.GetType().GetProperty("Manager").GetValue(p, null) == null ? false : p.GetType().GetProperty("Manager").GetValue(p, null) == shyam).Count());
                }
            );

            // Projecting whole entity set reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Where(p => p.ID == 1 || p.ID == 2 || p.ID == 6).Select(p => new { ID = p.ID, DirectReports = (p as PeopleManager).DirectReports}),
                },
                c => c.BaseUri + "People?$filter=ID eq 1 or ID eq 2 or ID eq 6&$expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(2, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Employee pratik = (Employee)c.Entities.Single(e => ((Employee)e.Entity).Name == "Pratik").Entity;
                    Employee jimmy = (Employee)c.Entities.Single(e => ((Employee)e.Entity).Name == "Jimmy").Entity;
                    Assert.AreEqual(3, o.Count);
                    var o1 = o.Single(p => (int)p.GetType().GetProperty("ID").GetValue(p, null) == 1);
                    var o2 = o.Single(p => (int)p.GetType().GetProperty("ID").GetValue(p, null) == 2);
                    var o6 = o.Single(p => (int)p.GetType().GetProperty("ID").GetValue(p, null) == 6);
                    Assert.IsNull(o1.GetType().GetProperty("DirectReports").GetValue(o1, null));
                    Assert.AreEqual(2, ((ICollection<Employee>)o2.GetType().GetProperty("DirectReports").GetValue(o2, null)).Count);
                    Assert.IsNull(o6.GetType().GetProperty("DirectReports").GetValue(o6, null));
                }
            );

            // Projecting narrow entity reference property, TypeAs after the nav prop.
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new { ID = e.ID, Manager = e.Manager == null ? null : new { ID = (e.Manager as Person).ID, Name = (e.Manager as Employee).Name }}),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new { ID = e.ID, Manager = e.Manager == null ? null : new { ID = (e.Manager as Person as PeopleManager as Employee).ID, Name = (e.Manager as Employee as Person).Name }}),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new { MyID = e.ID, Manager = new { MyID = (e.Manager as Person).ID, Name = (e.Manager as Employee).Name }}),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new { MyID = e.ID, Manager = new { MyID = (e.Manager as Person as PeopleManager as Employee).ID, Name = (e.Manager as Employee as Person).Name }}),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new MyEmployeeNonEntity { MyID = e.ID, Manager = e.Manager == null ? null : new MyEmployeeNonEntity { MyID = e.Manager.ID, Name = (e.Manager as Employee).Name }}),
                    c => c.CreateQuery<Person>("People").OfType<Employee>().Select(e => new MyEmployeeNonEntity { MyID = e.ID, Manager = e.Manager == null ? null : new MyEmployeeNonEntity { MyID = (e.Manager as Person as PeopleManager as Employee).ID, Name = (e.Manager as Employee as Person).Name }}),
                },
                c => c.BaseUri + "People/AstoriaUnitTests.Tests.DerivedProperty.Employee?$expand=Manager($select=ID),Manager($select=Name)&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(5, o.Count);
                    foreach (var p in o)
                    {
                        Assert.IsNotNull(p.GetType().GetProperty("Manager").GetValue(p, null));
                    }
                }
            );

            // Projecting narrow entity reference property, TypeAs before the nav prop.
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Manager = (e as Employee).Manager == null ? null : new { MyID = (e as Employee).Manager.ID, Name = (e as Employee).Manager.Name }}),
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Manager = (e as Employee as Person as Employee).Manager == null ? null : new { MyID = (e as Employee as Person as Employee).Manager.ID, Name = (e as Employee as Person as Employee).Manager.Name }}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Manager = (e as Employee).Manager == null ? null : new MyEmployeeNonEntity { MyID = (e as Employee).Manager.ID, Name = (e as Employee).Manager.Name }}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Manager = (e as Employee as Person as Employee).Manager == null ? null : new MyEmployeeNonEntity { MyID = (e as Employee as Person as Employee).Manager.ID, Name = (e as Employee as Person as Employee).Manager.Name }}),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($select=Name)&$select=ID",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (var p in o)
                    {
                        int myId = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        if (myId > 1)
                        {
                            var manager = p.GetType().GetProperty("Manager").GetValue(p, null);
                            var managerName = manager.GetType().GetProperty("Name").GetValue(manager, null);
                            Assert.IsTrue(manager != null && myId > 1 && managerName != null);
                        }
                    }
                }
            );

            // Projecting narrow entity set reference property
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new { MyID = cw.ID, Name = cw.Name }).ToList() }),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new MyEmployeeNonEntity { MyID = cw.ID, Name = cw.Name }).ToList() }),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=Name)&$select=ID",
                (ctx, q, o) =>
                {
                    Assert.AreEqual(0, ctx.Entities.Count);
                    Assert.AreEqual(0, ctx.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        IEnumerable<object> colleagues = (IEnumerable<object>)p.GetType().GetProperty("Colleagues").GetValue(p, null);
                        if (id > 1)
                        {
                            foreach (object c in colleagues)
                            {
                                int cId = (int)c.GetType().GetProperty("MyID").GetValue(c, null);
                                Assert.IsTrue(cId > 1);
                                string cName = (string)c.GetType().GetProperty("Name").GetValue(c, null);
                                Assert.IsNotNull(cName);
                            }
                        }
                        else
                        {
                            Assert.IsNull(colleagues);
                        }
                    }
                }
            );

            // Projecting narrow entity set reference property, TypeAs in the nested select
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new { MyID = cw.ID, Name = (cw as PeopleManager).Name }).ToList() }),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new MyEmployeeNonEntity { MyID = cw.ID, Name = (cw as PeopleManager).Name }).ToList() }),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Name)&$select=ID",
                (ctx, q, o) =>
                {
                    Assert.AreEqual(0, ctx.Entities.Count);
                    Assert.AreEqual(0, ctx.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        IEnumerable<object> colleagues = (IEnumerable<object>)p.GetType().GetProperty("Colleagues").GetValue(p, null);
                        if (id > 1)
                        {
                            foreach (object c in colleagues)
                            {
                                int cId = (int)c.GetType().GetProperty("MyID").GetValue(c, null);
                                string cName = (string)c.GetType().GetProperty("Name").GetValue(c, null);
                                if (cId == 2) Assert.AreEqual("Andy", cName);
                                else if (cId == 5) Assert.AreEqual("Shyam", cName);
                                else Assert.IsNull(cName);
                            }
                        }
                        else
                        {
                            Assert.IsNull(colleagues);
                        }
                    }
                }
            );

            // Projecting narrow entity set reference property, TypeAs in the nested select
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new { MyID = cw.ID, Name = (cw as PeopleManager).Name, DirectReports = (cw as PeopleManager).DirectReports == null ? null : (cw as PeopleManager).DirectReports.Select(dr => new { MyID = dr.ID, Name = (dr as Employee).Name, Manager = new { MyID = (dr as Employee).Manager.ID, Name = (dr as Employee).Manager.Name} }).ToList() }).ToList() }),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Colleagues = (e as Employee).Colleagues == null ? null : (e as Employee).Colleagues.Select(cw => new MyEmployeeNonEntity { MyID = cw.ID, Name = (cw as PeopleManager).Name, DirectReports = (cw as PeopleManager).DirectReports == null ? null : (cw as PeopleManager).DirectReports.Select(dr => new MyEmployeeNonEntity { MyID = dr.ID, Name = (dr as Employee).Name, Manager = new MyEmployeeNonEntity { MyID = (dr as Employee).Manager.ID, Name = (dr as Employee).Manager.Name} }).ToList() }).ToList() }),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=ID),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($select=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Name),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($select=ID)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($select=Name)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($expand=Manager($select=ID))),AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues($expand=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports($expand=Manager($select=Name)))&$select=ID",
                (ctx, q, o) =>
                {
                    Assert.AreEqual(0, ctx.Entities.Count);
                    Assert.AreEqual(0, ctx.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        IEnumerable<object> colleagues = (IEnumerable<object>)p.GetType().GetProperty("Colleagues").GetValue(p, null);
                        if (id > 1)
                        {
                            foreach (object c in colleagues)
                            {
                                int cId = (int)c.GetType().GetProperty("MyID").GetValue(c, null);

                                string cName = (string)c.GetType().GetProperty("Name").GetValue(c, null);
                                if (cId == 2) Assert.AreEqual("Andy", cName);
                                else if (cId == 5) Assert.AreEqual("Shyam", cName);
                                else Assert.IsNull(cName);

                                IEnumerable<object> drs = (IEnumerable<object>)c.GetType().GetProperty("DirectReports").GetValue(c, null);
                                if (cId == 2 || cId == 5)
                                {
                                    foreach (object dr in drs)
                                    {
                                        int drId = (int)dr.GetType().GetProperty("MyID").GetValue(dr, null);
                                        Assert.IsTrue(drId > 1);
                                        string drName = (string)dr.GetType().GetProperty("Name").GetValue(dr, null);
                                        Assert.IsNotNull(drName);
                                        object drManager = dr.GetType().GetProperty("Manager").GetValue(dr, null);
                                        Assert.IsNotNull(drManager);
                                        int drManagerId = (int)drManager.GetType().GetProperty("MyID").GetValue(drManager, null);
                                        Assert.AreEqual(cId, drManagerId);
                                        string drManagerName = (string)drManager.GetType().GetProperty("Name").GetValue(drManager, null);
                                        Assert.AreEqual(cName, drManagerName);
                                    }
                                }
                                else
                                {
                                    Assert.IsNull(drs);
                                }
                            }
                        }
                        else
                        {
                            Assert.IsNull(colleagues);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before a key and a primitive property so they would be null for Person and Employee
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = (e as PeopleManager).ID, Name = (e as PeopleManager).Name}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = (e as PeopleManager).ID, Name = (e as PeopleManager).Name}),
                },
                c => c.BaseUri + "People?$select=AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Name",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        string name = (string)p.GetType().GetProperty("Name").GetValue(p, null);
                        Assert.IsTrue((id == 0 && name == null) || (id == 2 && name == "Andy") || (id == 5 && name == "Shyam"));
                    }
                }
            );

            // TypeAs PeopleManager before complex property so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(p => new { MyID = p.ID, Office = (p as PeopleManager).Office}),
                    c => c.CreateQuery<Person>("People").Select(p => new MyEmployeeNonEntity { MyID = p.ID, Office = (p as PeopleManager).Office}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        Office office = (Office)p.GetType().GetProperty("Office").GetValue(p, null);
                        Assert.IsTrue((id == 2 && office != null) || (id == 5 && office != null) || (id > 0 && office == null));
                    }
                }
            );

            // TypeAs PeopleManager before complex property so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Office = (e as PeopleManager).Office == null ? null : new { BuildingName = (e as PeopleManager).Office.Building, OfficeNumber = (e as PeopleManager).Office.OfficeNumber}}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, MyOffice = (e as PeopleManager).Office == null ? null : new MyOfficeComplexType { BuildingName = (e as PeopleManager).Office.Building, OfficeNumber = (e as PeopleManager).Office.OfficeNumber}}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        object office = (object)p.GetType().GetProperty(p is MyEmployeeNonEntity ? "MyOffice" : "Office").GetValue(p, null);
                        Assert.IsTrue((id == 2 && office != null) || (id == 5 && office != null) || (id > 0 && office == null));
                    }
                }
            );

            // TypeAs Employee before entity reference and complex property and flatten the object
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new
                    {
                        MyID = e.ID,
                        OfficeBuilding = (e as PeopleManager).Office == null ? null : (e as PeopleManager).Office.Building,
                        OfficeNumber = (e as PeopleManager).Office == null ? 0 : (e as PeopleManager).Office.OfficeNumber,
                        Colleagues = (e as Employee).Manager == null ? null : ((e as Employee).Manager.DirectReports == null ? null : (e as Employee).Manager.DirectReports.Select(co => new { MyID = co.ID, Name = co.Name }).ToList()),
                        SkipLevelManager = (e as Employee).Manager == null ? null : ((e as Employee).Manager.Manager == null ? null : new { MyID = (e as Employee).Manager.Manager.ID, Name = (e as Employee).Manager.Manager.Name }),
                    }),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity
                    {
                        MyID = e.ID,
                        OfficeBuilding = (e as PeopleManager).Office == null ? null : (e as PeopleManager).Office.Building,
                        OfficeNumber = (e as PeopleManager).Office == null ? 0 : (e as PeopleManager).Office.OfficeNumber,
                        Colleagues = (e as Employee).Manager == null ? null : ((e as Employee).Manager.DirectReports == null ? null : (e as Employee).Manager.DirectReports.Select(co => new MyEmployeeNonEntity { MyID = co.ID, Name = co.Name }).ToList()),
                        SkipLevelManager = (e as Employee).Manager == null ? null : ((e as Employee).Manager.Manager == null ? null : new MyEmployeeNonEntity { MyID = (e as Employee).Manager.Manager.ID, Name = (e as Employee).Manager.Manager.Name }),
                    }),
                },
                c => c.BaseUri + "People?$expand=AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($expand=DirectReports($select=ID)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($expand=DirectReports($select=Name)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($expand=Manager($select=ID)),AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager($expand=Manager($select=Name))&$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Office",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        string officeBuilding = (string)p.GetType().GetProperty("OfficeBuilding").GetValue(p, null);
                        int officeNumber = (int)p.GetType().GetProperty("OfficeNumber").GetValue(p, null);
                        Assert.IsTrue((id == 2 && officeBuilding != null && officeNumber > 0) || (id == 5 && officeBuilding != null && officeNumber > 0) || (id > 0 && officeBuilding == null && officeNumber == 0));
                        IEnumerable<object> colleagues = (IEnumerable<object>)p.GetType().GetProperty("Colleagues").GetValue(p, null);
                        var skipLevelManager = p.GetType().GetProperty("SkipLevelManager").GetValue(p, null);
                        if (id > 1)
                        {
                            Assert.AreEqual(2, colleagues.Count());
                            Assert.IsNotNull(skipLevelManager);
                            int skipLevelManagerID = (int)skipLevelManager.GetType().GetProperty("MyID").GetValue(skipLevelManager, null);
                            Assert.AreEqual(5, skipLevelManagerID);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before bag of primitives so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Skills = (e as PeopleManager).Skills}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Skills = (e as PeopleManager).Skills}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Skills",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        ICollection<string> skills = (ICollection<string>)p.GetType().GetProperty("Skills").GetValue(p, null);
                        Assert.IsTrue((id == 2 && skills != null) || (id == 5 && skills != null) || (id > 0 && skills == null));
                    }
                }
            );

            // TypeAs PeopleManager before bag of complex so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Vacations = (e as PeopleManager).Vacations}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Vacations = (e as PeopleManager).Vacations}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Vacations",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    Assert.AreEqual(6, o.Count);
                    foreach (object p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        ICollection<Vacation> vacations = (ICollection<Vacation>)p.GetType().GetProperty("Vacations").GetValue(p, null);
                        Assert.IsTrue((id == 2 && vacations != null) || (id == 5 && vacations != null) || (id > 0 && vacations == null));
                    }
                }
            );

            // TypeAs PeopleManager before bag of complex so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, MyVacations = default(Vacation), Vacations = (e as PeopleManager).Vacations }),
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, MyVacations = default(Vacation), Vacations = (e as PeopleManager).Vacations == null ? null : (e as PeopleManager).Vacations.Select(v => new { Description = v.Description, StartDate = v.StartDate, EndDate = v.EndDate }).ToList()}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Vacations = (e as PeopleManager).Vacations }),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Vacations = (e as PeopleManager).Vacations == null ? null : (e as PeopleManager).Vacations.Select(v => new Vacation { Description = v.Description, StartDate = v.StartDate, EndDate = v.EndDate }).ToList()}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, MyVacations = (e as PeopleManager).Vacations == null ? null : (e as PeopleManager).Vacations.Select(v => new MyVacationComplexType { Description = v.Description, StartDate = v.StartDate, EndDate = v.EndDate }).ToList()}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Vacations",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        var vacationsProp = p.GetType().GetProperty("Vacations");
                        var myVacationsProp = p.GetType().GetProperty("MyVacations");
                        object vacations = vacationsProp.GetValue(p, null) ?? myVacationsProp.GetValue(p, null);
                        if (id == 2 || id == 5)
                        {
                            Assert.IsNotNull(vacations, "ID = " + id.ToString());
                        }
                        else
                        {
                            Assert.IsNull(vacations);
                        }
                    }
                }
            );

            // TypeAs PeopleManager before named stream so it'll be null for Employee and Person
            yield return new Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>(
                new Func<DataServiceContext, IQueryable>[]
                {
                    c => c.CreateQuery<Person>("People").Select(e => new { MyID = e.ID, Photo = (e as PeopleManager).Photo}),
                    c => c.CreateQuery<Person>("People").Select(e => new MyEmployeeNonEntity { MyID = e.ID, Photo = (e as PeopleManager).Photo}),
                },
                c => c.BaseUri + "People?$select=ID,AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/Photo",
                (c, q, o) =>
                {
                    Assert.AreEqual(0, c.Entities.Count);
                    Assert.AreEqual(0, c.Links.Count);
                    foreach (var p in o)
                    {
                        int id = (int)p.GetType().GetProperty("MyID").GetValue(p, null);
                        DataServiceStreamLink photo = (DataServiceStreamLink)p.GetType().GetProperty("Photo").GetValue(p, null);
                        Assert.IsTrue(id > 0);
                        if (id == 2 || id == 5)
                        {
                            Assert.IsNotNull(photo);
                            t.TestUtil.AssertContains(photo.EditLink.OriginalString, typeof(PeopleManager).FullName);
                            if (photo.SelfLink != null)
                            {
                                t.TestUtil.AssertContains(photo.SelfLink.OriginalString, typeof(PeopleManager).FullName);
                            }
                        }
                        else
                        {
                            Assert.IsNull(photo);
                        }
                    }
                }
            );
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void NegativeQueryTestsForMissingProperty_JsonTracking()
        {
            VerifyNegativeQueryErrorMessage(
                ModelWithDerivedNavigationProperties(),
                this.GetNegativeQueryTestsForMissingProperty,
                context =>
                {
                    JsonLightTestUtil.ConfigureContextForJsonLight(context);
                    context.MergeOption = MergeOption.AppendOnly;
                });
        }

        // [TestMethod]
        public void NegativeQueryTestsForMissingProperty_JsonNoTracking()
        {
            VerifyNegativeQueryErrorMessage(
                ModelWithDerivedNavigationProperties(),
                this.GetNegativeQueryTestsForMissingProperty,
                context =>
                {
                    JsonLightTestUtil.ConfigureContextForJsonLight(context);
                    context.MergeOption = MergeOption.NoTracking;
                });
        }

        IEnumerable<Tuple<IQueryable[], string, Type>> GetNegativeQueryTests(DataServiceContext context)
        {
            var people = context.CreateQuery<Person>("People");

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    from p in people.OfType<Employee>().OfType<PeopleManager>() select p,
                    from p in people.OfType<Employee>().Select(pp => pp).OfType<PeopleManager>() select p,
                    from p in people select (p as Employee as PeopleManager).DirectReports,
                    from p in people.Where(e => (e as Employee as PeopleManager).Name == "Bobby") select p,
                    from p in people orderby (p as Employee as PeopleManager).Name select p,
                    from p in people.Expand(pp => (pp as Employee as PeopleManager).DirectReports) select p,
                    from p in people where p.ID == 1 from e in p.Aquaintances.OfType<Employee>().OfType<PeopleManager>() select e,
                    from p in people where p.ID == 1 from e in p.Aquaintances.OfType<Employee>().Cast<Person>().OfType<PeopleManager>() select e,
                    from p in people where p.ID == 1 from e in p.Aquaintances.OfType<Employee>().OfType<Person>().OfType<PeopleManager>() select e,
                    from p in people where p.ID == 1 from e in p.Aquaintances.OfType<Employee>().Select(pp => pp).OfType<PeopleManager>() select e,
                },
                "Non-redundant type filters (OfType<T>, C# 'as' and VB 'TryCast') can only be used once per resource set.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    from p in people select p as Employee,
                },
                "Unsupported expression '(p As Employee)' in 'Select' method. Expression cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    from p in people select p.BestFriend as PeopleManager,
                },
                "Unsupported expression '(p.BestFriend As PeopleManager)' in 'Select' method. Expression cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.SelectMany(p => p.Friends as ICollection<Employee>),
                },
                "Unsupported expression '(p.Friends As ICollection`1)' in 'SelectMany' method. Expression cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    from p in people orderby p as Employee select p,
                },
                "Unsupported expression '(p As Employee)' in 'OrderBy' method. Expression cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    from p in people orderby p.BestFriend as Employee select p,
                },
                "Unsupported expression '(p.BestFriend As Employee)' in 'OrderBy' method. Expression cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    from p in people where (p.BestFriend as Employee) == null && (p.BestFriend.Name as string) != string.Empty select p,
                    from p in people where (p.BestFriend as Employee) == null select p,
                },
                "Unsupported expression '(p.BestFriend As Employee)' in 'Where' method. Expression cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Expand(p => p as PeopleManager),
                },
                "The expression 'p => (p As PeopleManager)' is not a valid expression for navigation path. The only supported operations inside the lambda expression body are MemberAccess and TypeAs. The expression must contain at least one MemberAccess and it cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Expand(p => (p as Employee).BestFriend as PeopleManager),
                },
                "The expression 'p => ((p As Employee).BestFriend As PeopleManager)' is not a valid expression for navigation path. The only supported operations inside the lambda expression body are MemberAccess and TypeAs. The expression must contain at least one MemberAccess and it cannot end with TypeAs.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Expand(p => ((p as Employee).BestFriend as PeopleManager).Office.Building),
                },
                "The expression 'p => ((p As Employee).BestFriend As PeopleManager).Office.Building' is not a valid expression for navigation path. The only supported operations inside the lambda expression body are MemberAccess and TypeAs. The expression must contain at least one MemberAccess and it cannot end with TypeAs.",
                typeof(NotSupportedException)
            );


            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    ((DataServiceQuery<PeopleManager>)((DataServiceQuery<Employee>)people.Expand(p => (p as Employee).Manager).OfType<Employee>().Where(e => e.ID == 2)).Expand("BestFriend").Select(e => e.Manager)).Expand(p => ((p as Employee).Manager.BestFriend as PeopleManager).DirectReports).Expand("BestFriend"),
                },
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><m:error xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:code /><m:message>The type name 'AstoriaUnitTests.Tests.DerivedProperty.Employee' specified in the URI is not a valid type. Please make sure that the type name is valid and that it derives from the type 'AstoriaUnitTests.Tests.DerivedProperty.PeopleManager'.</m:message></m:error>",
                typeof(DataServiceQueryException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { ID = e.ID, Vacations = (e as Employee).Vacations.Select(v => new { Desc = v.Description, Start = v.StartDate, End = v.EndDate }).ToList()}),
                    people.Select(e => new MyEmployeeNonEntity { MyID = e.ID, MyVacations = (e as Employee).Vacations.Select(v => new MyVacationComplexType { Description= v.Description, StartDate = v.StartDate, EndDate = v.EndDate }).ToList()}),
                },
                "Value cannot be null.\r\nParameter name: source",
                typeof(ArgumentNullException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { ID = e.ID, BestFriend = e.BestFriend as Employee }),
                },
                "Constructing or initializing instances of the type <>f__AnonymousType19`2[System.Int32,AstoriaUnitTests.Tests.DerivedProperty.Employee] with the expression (e.BestFriend As Employee) is not supported.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { ID = e.ID, BestFriend = new { ID = e.BestFriend.ID, BestFriend = e.BestFriend.BestFriend as Employee }}),
                },
                "Constructing or initializing instances of the type <>f__AnonymousType19`2[System.Int32,<>f__AnonymousType19`2[System.Int32,AstoriaUnitTests.Tests.DerivedProperty.Employee]] with the expression (e.BestFriend.BestFriend As Employee) is not supported.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new Person { ID = e.ID, BestFriend = e.BestFriend as Employee }),
                },
                "Initializing instances of the entity type AstoriaUnitTests.Tests.DerivedProperty.Person with the expression (e.BestFriend As Employee) is not supported.",
                typeof(NotSupportedException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.OfType<Employee>().Select(e => new MyEmployeeEntity { ID = e.ID, Vacations = e.Vacations.Select(v => new Vacation { Description = v.Description, StartDate = v.StartDate, EndDate = v.EndDate }).ToList()}),
                },
                "Constructing or initializing instances of the type AstoriaUnitTests.Tests.DerivedProperty.MyEmployeeEntity with the expression e.Vacations.Select(v => new Vacation() {Description = v.Description, StartDate = v.StartDate, EndDate = v.EndDate}).ToList() is not supported.",
                typeof(NotSupportedException)
            );
        }

        IEnumerable<Tuple<IQueryable[], string, Type>> GetNegativeQueryTestsForMissingProperty(DataServiceContext context)
        {
            var people = context.CreateQuery<Person>("People");

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { ID = e.ID, Manager = new { ID = (e as Employee).Manager.ID, Name = (e as Employee).Manager.Name }}),
                    people.Select(e => new { ID = e.ID, Manager = new { ID = (e as Employee as Person as Employee).Manager.ID, Name = (e as Employee as Person as Employee).Manager.Name }}),
                    people.Select(e => new MyEmployeeNonEntity { MyID = e.ID, Manager = new MyEmployeeNonEntity { MyID = (e as Employee).Manager.ID, Name = (e as Employee).Manager.Name }}),
                    people.Select(e => new MyEmployeeEntity { ID = e.ID, Manager = new MyEmployeeEntity { ID = (e as Employee).Manager.ID, Name = (e as Employee).Manager.Name }}),
                    people.Select(e => new Employee { ID = e.ID, Manager = new PeopleManager { ID = (e as Employee).Manager.ID, Name = (e as Employee).Manager.Name }}),

                },
                DataServicesClientResourceUtil.GetString("AtomMaterializer_PropertyMissing", "Manager"),
                typeof(InvalidOperationException)
            );

            // Same as above, but no key properties included in projection
            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { Manager = new { Name = (e as Employee).Manager.Name }}),
                    people.Select(e => new { Manager = new { Name = (e as Employee as Person as Employee).Manager.Name }}),
                    people.Select(e => new MyEmployeeNonEntity { Manager = new MyEmployeeNonEntity { Name = (e as Employee).Manager.Name }}),
                    people.Select(e => new MyEmployeeEntity { Manager = new MyEmployeeEntity { Name = (e as Employee).Manager.Name }}),
                    people.Select(e => new Employee { Manager = new PeopleManager { Name = (e as Employee).Manager.Name }}),

                },
                DataServicesClientResourceUtil.GetString("AtomMaterializer_PropertyMissing", "Manager"),
                typeof(InvalidOperationException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { ID = e.ID, Colleagues = (e as Employee).Colleagues.Select(c => new { ID = c.ID, Name = c.Name }).ToList()}),
                    people.Select(e => new MyEmployeeNonEntity { MyID = e.ID, Colleagues = (e as Employee).Colleagues.Select(c => new MyEmployeeNonEntity { MyID = c.ID, Name = c.Name }).ToList()}),
                    people.Select(e => new MyEmployeeEntity { ID = e.ID, Colleagues = (e as Employee).Colleagues.Select(c => new MyEmployeeEntity { ID = c.ID, Name = c.Name }).ToList()}),
                    people.Select(e => new Employee { ID = e.ID, Colleagues = (e as Employee).Colleagues.Select(c => new Employee { ID = c.ID, Name = c.Name }).ToList()}),
                },
                DataServicesClientResourceUtil.GetString("AtomMaterializer_PropertyMissing", "Colleagues"),
                typeof(InvalidOperationException)
            );

            // Same as above, but no key properties included in projection
            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { Colleagues = (e as Employee).Colleagues.Select(c => new { Name = c.Name }).ToList()}),
                    people.Select(e => new MyEmployeeNonEntity { Colleagues = (e as Employee).Colleagues.Select(c => new MyEmployeeNonEntity {Name = c.Name }).ToList()}),
                    people.Select(e => new MyEmployeeEntity { Colleagues = (e as Employee).Colleagues.Select(c => new MyEmployeeEntity { Name = c.Name }).ToList()}),
                    people.Select(e => new Employee { Colleagues = (e as Employee).Colleagues.Select(c => new Employee { Name = c.Name }).ToList()}),
                },
                DataServicesClientResourceUtil.GetString("AtomMaterializer_PropertyMissing", "Colleagues"),
                typeof(InvalidOperationException)
            );

            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { ID = e.ID, OfficeBuilding = (e as Employee).Office.Building, OfficeNumber = (e as Employee).Office.OfficeNumber }),
                    people.Select(e => new { ID = e.ID, Office = new { Building = (e as Employee).Office.Building, OfficeNumber = (e as Employee).Office.OfficeNumber }}),
                    people.Select(e => new MyEmployeeNonEntity { MyID = e.ID, MyOffice = new MyOfficeComplexType { BuildingName = (e as Employee).Office.Building, OfficeNumber = (e as Employee).Office.OfficeNumber }}),
                },
                DataServicesClientResourceUtil.GetString("AtomMaterializer_PropertyMissing", "Office"),
                typeof(InvalidOperationException)
            );

            // Same as above, but no key properties included in projection
            yield return new Tuple<IQueryable[], string, Type>(
                new IQueryable[]
                {
                    people.Select(e => new { OfficeBuilding = (e as Employee).Office.Building, OfficeNumber = (e as Employee).Office.OfficeNumber }),
                    people.Select(e => new { Office = new { Building = (e as Employee).Office.Building, OfficeNumber = (e as Employee).Office.OfficeNumber }}),
                    people.Select(e => new MyEmployeeNonEntity { MyOffice = new MyOfficeComplexType { BuildingName = (e as Employee).Office.Building, OfficeNumber = (e as Employee).Office.OfficeNumber }}),
                },
                DataServicesClientResourceUtil.GetString("AtomMaterializer_PropertyMissing", "Office"),
                typeof(InvalidOperationException)
            );
        }

        private static void VerifyNegativeQueryErrorMessage(DSPServiceDefinition service, Func<DataServiceContext, IEnumerable<Tuple<IQueryable[], string, Type>>> getTestCases, Action<DataServiceContext> configureContext = null)
        {
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();

                DataServiceContext context = new DataServiceContext(new Uri(request.BaseUri), ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();

                if (configureContext != null)
                {
                    configureContext(context);
                }

                t.TestUtil.RunCombinations(getTestCases(context), testCase =>
                {
                    var queries = testCase.Item1;
                    string expectedError = testCase.Item2;
                    Type expectedExceptionType = testCase.Item3;

                    foreach (var query in queries)
                    {
                        try
                        {
                            foreach (var p in query) { }
                            Assert.Fail("Exception expected but received none.");
                        }
                        catch (Exception e)
                        {
                            Assert.AreEqual(expectedExceptionType, e.GetType());
                            string errorMsg = e.InnerException == null ? e.Message : e.InnerException.Message;
                            Assert.AreEqual(AstoriaUnitTests.Tests.LinqTests.Normalize(expectedError), AstoriaUnitTests.Tests.LinqTests.Normalize(errorMsg));
                        }
                    }
                });
            }
        }

        private void VerifyNonProjectedLinkInfoCount(DataServiceContext ctx)
        {
            foreach (var p in ctx.Entities)
            {
                switch (p.ServerTypeName)
                {
                    case "AstoriaUnitTests.Tests.DerivedProperty.Person":
                        Assert.AreEqual(3, p.LinkInfos.Count());
                        Assert.AreEqual(0, p.StreamDescriptors.Count());
                        break;

                    case "AstoriaUnitTests.Tests.DerivedProperty.Employee":
                        Assert.AreEqual(5, p.LinkInfos.Count());
                        Assert.AreEqual(1, p.StreamDescriptors.Count());
                        break;

                    case "AstoriaUnitTests.Tests.DerivedProperty.PeopleManager":
                        Assert.AreEqual(6, p.LinkInfos.Count());
                        Assert.AreEqual(1, p.StreamDescriptors.Count());
                        break;

                    default:
                        Assert.Fail("Unexpected type: " + p.ServerTypeName);
                        break;
                }
            }
        }

        private static void SendRequestAndVerifyUriAndContext(DSPServiceDefinition service,
            Func<IEnumerable<Tuple<Func<DataServiceContext, IQueryable>[], Func<DataServiceContext, string>, Action<DataServiceContext, IQueryable, List<object>>>>> queryUrisAndValidations)
        {
            using (InProcessWebRequest request = (InProcessWebRequest)service.CreateForInProcess())
            {
                request.StartService();
                ODataProtocolVersion[] versions = new ODataProtocolVersion[] { ODataProtocolVersion.V4 };
                t.TestUtil.RunCombinations(queryUrisAndValidations(), versions, (testCase, version) =>
                {
                    var getQueries = testCase.Item1;
                    var getExpectedUri = testCase.Item2;
                    var validate = testCase.Item3;
                    foreach (var getQuery in getQueries)
                    {
                        var host = new t.TestServiceHost2();
                        var requestMessage = new DataServiceHostRequestMessage(host);
                        var ctx = new DataServiceContextWithCustomTransportLayer(request.ServiceRoot, version, () => requestMessage, () =>
                        {
                            request.SendRequest(host);
                            return new DataServiceHostResponseMessage(host);
                        });
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();

                        if (version < ODataProtocolVersion.V4)
                        {
                            var query = getQuery(ctx);
                            try
                            {
                                foreach (var p in query) { }
                                Assert.Fail("Exception expected but received none.");
                            }
                            catch (NotSupportedException e)
                            {
                                Assert.IsTrue(
                                    "The expression 'TypeAs' is not supported when MaxProtocolVersion is less than '4.0'." == e.Message ||
                                    "The method 'OfType' is not supported when MaxProtocolVersion is less than '4.0'." == e.Message);
                            }
                        }
                        else
                        {
                            #region Setup resolvers and events

                            ctx.ResolveType = (typeName) =>
                            {
                                if (typeName == typeof(Employee).FullName)
                                {
                                    return typeof(Employee);
                                }

                                if (typeName == typeof(PeopleManager).FullName)
                                {
                                    return typeof(PeopleManager);
                                }

                                return null;
                            };

                            ctx.SendingRequest2 += (sender, e) =>
                            {
                                Assert.AreEqual("4.0", e.RequestMessage.GetHeader("OData-Version"), "OData-Version mismatch.");
                                Assert.AreEqual("4.0", e.RequestMessage.GetHeader("OData-MaxVersion"), "OData-MaxVersion mismatch.");
                            };

                            #endregion Setup resolvers and events

                            // Validate Uri
                            var query = getQuery(ctx);
                            string clientGeneratedUri = query.ToString();
                            Assert.AreEqual(getExpectedUri(ctx), clientGeneratedUri);

                            #region Validate entities

                            List<object> materializedObjects = new List<object>();

                            // Materialize and validate LinkInfos
                            foreach (var e in query)
                            {
                                EntityDescriptor descriptor = ctx.GetEntityDescriptor(e);
                                if (descriptor != null)
                                {
                                    foreach (var link in descriptor.LinkInfos)
                                    {
                                        switch (link.Name)
                                        {
                                            case "Aquaintances":
                                            case "DirectReports":
                                            case "Manager":
                                            case "Colleagues":
                                            case "BestFriend":
                                            case "Friends":
                                                // If the entity is not of the base type (Person), then expect navigation links to have a type segment.
                                                if (descriptor.ServerTypeName != "AstoriaUnitTests.Tests.DerivedProperty.Person")
                                                {
                                                    t.TestUtil.AssertContains(link.NavigationLink.OriginalString, descriptor.ServerTypeName);
                                                    if (link.AssociationLink != null)
                                                    {
                                                        t.TestUtil.AssertContains(link.AssociationLink.OriginalString, descriptor.ServerTypeName);
                                                    }
                                                }
                                                break;
                                            default:
                                                Assert.Fail("Unexpected link: " + link.Name);
                                                return;
                                        }
                                    }
                                }

                                materializedObjects.Add(e);
                            }

                            #endregion Validate entities

                            #region Validate Links

                            // Validate LinkDescriptors
                            foreach (LinkDescriptor link in ctx.Links)
                            {
                                string identity = ctx.GetEntityDescriptor(link.Source).Identity.AbsoluteUri;
                                int startIdx = identity.IndexOf('(') + 1;
                                int endIdx = identity.IndexOf(')');
                                int sourceId = int.Parse(identity.Substring(startIdx, endIdx - startIdx));

                                identity = ctx.GetEntityDescriptor(link.Target).Identity.AbsoluteUri;
                                startIdx = identity.IndexOf('(') + 1;
                                endIdx = identity.IndexOf(')');
                                int targetId = int.Parse(identity.Substring(startIdx, endIdx - startIdx));

                                switch (link.SourceProperty)
                                {
                                    case "DirectReports":
                                        switch (sourceId)
                                        {
                                            case 1: //"Foo":
                                                Assert.Fail("DirectReports link not expected for Foo");
                                                break;
                                            case 2: //"Andy":
                                                //Assert.IsTrue(targetName == "Pratik" || targetName == "Jimmy");
                                                Assert.IsTrue(targetId == 3 || targetId == 4);
                                                break;
                                            case 3: //"Pratik":
                                                Assert.Fail("DirectReports link not expected for Pratik");
                                                break;
                                            case 4: //"Jimmy":
                                                Assert.Fail("DirectReports link not expected for Jimmy");
                                                break;
                                            case 5: //"Shyam":
                                                //Assert.IsTrue(targetName == "Andy" || targetName == "Marcelo");
                                                Assert.IsTrue(targetId == 2 || targetId == 6);
                                                break;
                                            case 6: //"Marcelo":
                                                Assert.Fail("DirectReports link not expected for Marcelo");
                                                break;
                                            default:
                                                Assert.Fail("Unrecognized id: " + sourceId);
                                                break;
                                        }
                                        break;
                                    case "Manager":
                                        switch (sourceId)
                                        {
                                            case 1: //"Foo":
                                                Assert.Fail("Manager link not expected for Foo");
                                                break;
                                            case 2: //"Andy":
                                                //Assert.AreEqual("Shyam", targetName);
                                                Assert.AreEqual(5, targetId);
                                                break;
                                            case 3: //"Pratik":
                                                //Assert.AreEqual("Andy", targetName);
                                                Assert.AreEqual(2, targetId);
                                                break;
                                            case 4: //"Jimmy":
                                                //Assert.AreEqual("Andy", targetName);
                                                Assert.AreEqual(2, targetId);
                                                break;
                                            case 5: //"Shyam":
                                                //Assert.AreEqual("Shyam", targetName);
                                                Assert.AreEqual(5, targetId);
                                                break;
                                            case 6: //"Marcelo":
                                                //Assert.AreEqual("Shyam", targetName);
                                                Assert.AreEqual(5, targetId);
                                                break;
                                            default:
                                                Assert.Fail("Unrecognized id: " + sourceId);
                                                break;
                                        }
                                        break;
                                    case "Colleagues":
                                        switch (sourceId)
                                        {
                                            case 1: //"Foo":
                                                Assert.Fail("Colleagues link not expected for Foo");
                                                break;
                                            case 2: //"Andy":
                                                //Assert.AreEqual("Marcelo", targetName);
                                                Assert.AreEqual(6, targetId);
                                                break;
                                            case 3: //"Pratik":
                                                //Assert.AreEqual("Jimmy", targetName);
                                                Assert.AreEqual(4, targetId);
                                                break;
                                            case 4: //"Jimmy":
                                                //Assert.AreEqual("Pratik", targetName);
                                                Assert.AreEqual(3, targetId);
                                                break;
                                            case 5: //"Shyam":
                                                Assert.Fail("Colleagues link not expected for Shyam");
                                                break;
                                            case 6: //"Marcelo":
                                                //Assert.AreEqual("Andy", targetName);
                                                Assert.AreEqual(2, targetId);
                                                break;
                                            default:
                                                Assert.Fail("Unrecognized id: " + sourceId);
                                                break;
                                        }
                                        break;
                                    case "BestFriend":
                                        switch (sourceId)
                                        {
                                            case 1: //"Foo":
                                                //Assert.AreEqual("Pratik", targetName);
                                                Assert.AreEqual(3, targetId);
                                                break;
                                            case 2: //"Andy":
                                                //Assert.AreEqual("Shyam", targetName);
                                                Assert.AreEqual(5, targetId);
                                                break;
                                            case 3: //"Pratik":
                                                //Assert.AreEqual("Andy", targetName);
                                                Assert.AreEqual(2, targetId);
                                                break;
                                            case 4: //"Jimmy":
                                                //Assert.AreEqual("Foo", targetName);
                                                Assert.AreEqual(1, targetId);
                                                break;
                                            case 5: //"Shyam":
                                                //Assert.AreEqual("Marcelo", targetName);
                                                Assert.AreEqual(6, targetId);
                                                break;
                                            case 6: //"Marcelo":
                                                //Assert.AreEqual("Jimmy", targetName);
                                                Assert.AreEqual(4, targetId);
                                                break;
                                            default:
                                                Assert.Fail("Unrecognized id: " + sourceId);
                                                break;
                                        }
                                        break;
                                    case "Friends":
                                        break;
                                    default:
                                        Assert.Fail("Unexpected link descriptor: " + link.SourceProperty);
                                        return;
                                }
                            }

                            #endregion Validate Links

                            // Validation specific to the test case.
                            validate(ctx, query, materializedObjects);
                        }
                    }
                });
            }
        }

        #endregion Linq Tests

        #region DataServiceContext Tests

        #region AddLink
        [Ignore] // Remove Atom
        // [TestMethod]
        public void AddLinkTests()
        {
            SendSaveChangesAndVerifyContext(GetAddLinkTests());
        }

        IEnumerable<Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>> GetAddLinkTests()
        {
            // add link to attached objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                Employee pratik = new Employee { ID = 3 };
                Employee marcelo = new Employee { ID = 6 };
                ctx.AttachTo("People", pratik);
                ctx.AttachTo("People", marcelo);
                ctx.AddLink(pratik, "Friends", marcelo);

                ctx.SaveChanges(saveChangeOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => p.Friends).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.AreEqual(6, marcelo.ID);
                Assert.IsNotNull(pratik.Friends.Single(f => f == marcelo));

                Assert.AreEqual(2, ctx.Links.Where(l => l.Source == pratik).Count());
                Assert.IsNotNull(ctx.Links.Single(l => l.Target == marcelo));
            },
            new List<Version>() { V4, V4 },
            null);

            // add link to retrieved objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                Employee marcelo = new Employee { ID = 6 };
                ctx.AttachTo("People", marcelo);
                ctx.AddLink(pratik, "Friends", marcelo);

                ctx.SaveChanges(saveChangeOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => p.Friends).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.AreEqual(6, marcelo.ID);
                Assert.IsNotNull(pratik.Friends.Single(f => f == marcelo));

                Assert.AreEqual(2, ctx.Links.Where(l => l.Source == pratik).Count());
                Assert.IsNotNull(ctx.Links.Single(l => l.Target == marcelo));
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // add link to newly added objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Person p100 = new Person { ID = 100 };
                Person p101 = new Person { ID = 101 };
                ctx.AddObject("People", p100);
                ctx.AddObject("People", p101);
                ctx.AddLink(p100, "Friends", p101);

                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => p.Friends).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.Friends.Single());

                Assert.AreEqual(p100, (Person)ctx.Links.Single().Source);
                Assert.AreEqual(p101, (Person)ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4, V4, V4 },
            null);

            // add link to existing objects, for derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                PeopleManager shyam = null;
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 5);
                foreach (Person p in query)
                {
                    shyam = (PeopleManager)p;
                    Assert.IsTrue(ctx.GetEntityDescriptor(p).EditLink.OriginalString.EndsWith(typeof(PeopleManager).FullName), "href for edit link did not end with type name");
                }

                Employee marcelo = new Employee { ID = 6 };
                ctx.AttachTo("People", marcelo);
                ctx.AddLink(shyam, "Colleagues", marcelo);

                ctx.SaveChanges(saveChangesOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Colleagues).Where(p => p.ID == 5);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(5, person.ID);
                Assert.AreEqual(shyam, person);
                Assert.AreEqual(6, marcelo.ID);
                Assert.AreEqual(marcelo, shyam.Colleagues.Single(e => e.ID == 6));

                Assert.IsNotNull(ctx.Links.SingleOrDefault(l => l.SourceProperty == "Colleagues" && l.Source == shyam && l.Target == marcelo));
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // add link to newly added objects, for derived property with link folding
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                ctx.Format.UseJson(this.LoadModelFromString());
                PeopleManager shyam = new PeopleManager { ID = 5 };
                Employee pratik = new Employee { ID = 3 };

                ctx.AttachTo("People", shyam);
                ctx.AttachTo("People", pratik);

                ctx.AddLink(shyam, "DirectReports", pratik);

                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => (p as PeopleManager).DirectReports).Where(p => p.ID == 5);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(shyam.ID, person.ID);
                Assert.AreEqual(shyam, person);
                Assert.AreEqual(3, pratik.ID);
                Assert.AreEqual(pratik, shyam.DirectReports.FirstOrDefault(l => l.ID == 3));

                Assert.IsNotNull(ctx.Links.SingleOrDefault(l => l.SourceProperty == "DirectReports" && l.Source == shyam && l.Target == pratik));
            },
            new List<Version>() { V4, V4 },
            null);

            // add link to newly added objects, for derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                PeopleManager p100 = new PeopleManager { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                Employee p101 = new Employee { ID = 101 };
                p101.Skills = new List<string>();
                p101.Vacations = new List<Vacation>();

                ctx.AddObject("People", p100);
                ctx.AddObject("People", p101);
                ctx.AddLink(p100, "Colleagues", p101);

                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Colleagues).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.Colleagues.Single());

                Assert.IsNotNull(ctx.Links.SingleOrDefault(l => l.SourceProperty == "Colleagues" && l.Source == p100 && l.Target == p101));
            },
            new List<Version>() { V4 /*b/c of collection properties*/, V4 /*b/c of collection properties*/, V4, V4 },
            null);

            // add link to newly added objects, for derived property with link folding
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                PeopleManager p100 = new PeopleManager { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                Employee p101 = new Employee { ID = 101 };
                p101.Skills = new List<string>();
                p101.Vacations = new List<Vacation>();

                ctx.AddObject("People", p101);
                ctx.AddObject("People", p100);
                ctx.AddLink(p100, "Colleagues", p101);

                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Colleagues).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.Colleagues.Single());

                Assert.IsNotNull(ctx.Links.SingleOrDefault(l => l.SourceProperty == "Colleagues" && l.Source == p100 && l.Target == p101));
            },
            new List<Version>() { V4 /*b/c of collection properties*/, V4 /*b/c of collection properties*/, V4 },
            null);
        }

        #endregion AddLink

        #region SetLink
        [Ignore] // Remove Atom
        // [TestMethod]
        public void SetLinkTests()
        {
            SendSaveChangesAndVerifyContext(GetSetLinkTests());
        }

        IEnumerable<Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>> GetSetLinkTests()
        {
            // set link to attached objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                Employee pratik = new Employee { ID = 3 };
                Employee marcelo = new Employee { ID = 6 };
                ctx.AttachTo("People", pratik);
                ctx.AttachTo("People", marcelo);
                ctx.SetLink(pratik, "BestFriend", marcelo);

                ctx.SaveChanges(saveChangeOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => p.BestFriend).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(person, pratik);
                Assert.AreEqual(6, marcelo.ID);
                Assert.AreEqual(marcelo, pratik.BestFriend);

                Assert.AreEqual(pratik, (Person)ctx.Links.Single().Source);
                Assert.AreEqual(marcelo, (Person)ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4 },
            null);

            // set link to retrieved objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                Employee marcelo = new Employee { ID = 6 };
                ctx.AttachTo("People", marcelo);
                ctx.SetLink(pratik, "BestFriend", marcelo);

                ctx.SaveChanges(saveChangeOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => p.BestFriend).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.AreEqual(6, marcelo.ID);
                Assert.AreEqual(marcelo, pratik.BestFriend);

                Assert.AreEqual(pratik, (Person)ctx.Links.Single().Source);
                Assert.AreEqual(marcelo, (Person)ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // set link to newly added objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Person p100 = new Person { ID = 100 };
                Person p101 = new Person { ID = 101 };
                ctx.AddObject("People", p100);
                ctx.AddObject("People", p101);
                ctx.SetLink(p100, "BestFriend", p101);

                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => p.BestFriend).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.BestFriend);

                Assert.AreEqual(p100, (Person)ctx.Links.Single().Source);
                Assert.AreEqual(p101, (Person)ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4, V4, V4 },
            null);

            // set link to existing objects, for derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Employee pratik = null;
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                foreach (Person p in query)
                {
                    pratik = (Employee)p;
                    Assert.IsTrue(ctx.GetEntityDescriptor(p).EditLink.OriginalString.EndsWith(typeof(Employee).FullName), "href for edit link did not end with type name");
                }

                PeopleManager shyam = new PeopleManager { ID = 5 };
                ctx.AttachTo("People", shyam);
                ctx.SetLink(pratik, "Manager", shyam);

                ctx.SaveChanges(saveChangesOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Manager).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.AreEqual(5, shyam.ID);
                Assert.AreEqual(shyam, pratik.Manager);

                Assert.AreEqual(pratik, ctx.Links.Single().Source);
                Assert.AreEqual(shyam, ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // set link to existing objects, for derived property, should fail for attach
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Employee pratik = new Employee { ID = 3 };
                PeopleManager shyam = new PeopleManager { ID = 5 };
                ctx.AttachTo("People", pratik);
                ctx.AttachTo("People", shyam);
                ctx.SetLink(pratik, "Manager", shyam);

                try
                {
                    ctx.SaveChanges(saveChangesOption);
                    Assert.Fail("Exception expected but received none.");
                }
                catch (DataServiceRequestException e)
                {
                    t.TestUtil.AssertContains(e.InnerException.Message, "Resource not found for the segment 'Manager'.");
                }
            },
            new List<Version>() { V4 },
            null);

            // set link to null, for derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Employee pratik = null;
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                foreach (Person p in query)
                {
                    pratik = (Employee)p;
                    Assert.IsTrue(ctx.GetEntityDescriptor(p).EditLink.OriginalString.EndsWith(typeof(Employee).FullName), "href for edit link did not end with type name");
                }

                ctx.SetLink(pratik, "Manager", null);
                ctx.SaveChanges(saveChangesOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Manager).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.IsNull(pratik.Manager);

                Assert.AreEqual(pratik, ctx.Links.Single().Source);
                Assert.IsNull(ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // set link to newly added objects, for derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Employee p100 = new Employee { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                PeopleManager p101 = new PeopleManager { ID = 101 };
                p101.Skills = new List<string>();
                p101.Vacations = new List<Vacation>();

                ctx.AddObject("People", p100);
                ctx.AddObject("People", p101);
                ctx.SetLink(p100, "Manager", p101);

                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Manager).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.Manager);

                Assert.AreEqual(p100, ctx.Links.Single().Source);
                Assert.AreEqual(p101, ctx.Links.Single().Target);
            },
            new List<Version>() { V4 /*b/c of collection properties*/, V4 /*b/c of collection properties*/, V4, V4 },
            null);

            // set link to newly added objects, for derived property with link folding
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Employee p100 = new Employee { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                PeopleManager p101 = new PeopleManager { ID = 101 };
                p101.Skills = new List<string>();
                p101.Vacations = new List<Vacation>();

                ctx.AddObject("People", p101);
                ctx.AddObject("People", p100);
                ctx.SetLink(p100, "Manager", p101);

                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Manager).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.Manager);

                Assert.AreEqual(p100, ctx.Links.Single().Source);
                Assert.AreEqual(p101, ctx.Links.Single().Target);
            },
            new List<Version>() { V4 /*b/c of collection properties*/, V4 /*b/c of collection properties*/, V4 },
            null);
        }

        #endregion SetLink

        #region DeleteLink
        [Ignore] // Remove Atom
        // [TestMethod]
        public void DeleteLinkTests()
        {
            SendSaveChangesAndVerifyContext(GetDeleteLinkTests());
        }

        IEnumerable<Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>> GetDeleteLinkTests()
        {
            // delete link to attached objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                Employee pratik = new Employee { ID = 3 };
                Person p1 = new Person { ID = 1 };
                ctx.AttachTo("People", pratik);
                ctx.AttachTo("People", p1);
                ctx.DeleteLink(pratik, "Friends", p1);

                ctx.SaveChanges(saveChangeOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => p.Friends).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.IsFalse(pratik.Friends.Any());
                Assert.AreEqual(0, ctx.Links.Count());
            },
            new List<Version>() { V4, V4 },
            null);

            // delete link to retrieved objects, for non-derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                Person p1 = new Person { ID = 1 };
                ctx.AttachTo("People", p1);
                ctx.DeleteLink(pratik, "Friends", p1);

                ctx.SaveChanges(saveChangeOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => p.Friends).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.IsFalse(pratik.Friends.Any());
                Assert.AreEqual(0, ctx.Links.Count());
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // delete link to retrieved object, for derived property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                Employee marcelo = null;
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 6);
                foreach (Person p in query)
                {
                    marcelo = (Employee)p;
                    Assert.IsTrue(ctx.GetEntityDescriptor(p).EditLink.OriginalString.EndsWith(typeof(Employee).FullName), "href for edit link did not end with type name");
                }

                PeopleManager andy = new PeopleManager { ID = 2 };
                ctx.AttachTo("People", andy);
                ctx.DeleteLink(marcelo, "Colleagues", andy);

                ctx.SaveChanges(saveChangesOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Colleagues).Where(p => p.ID == 6);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(6, person.ID);
                Assert.AreEqual(marcelo, person);
                Assert.IsFalse(marcelo.Colleagues.Any());
                Assert.AreEqual(0, ctx.Links.Count());
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // Delete newly added links, for derived property with link folding
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangesOption) =>
            {
                ctx.Format.UseJson(this.LoadModelFromString());
                PeopleManager shyam = new PeopleManager { ID = 5 };
                Employee marcelo = new Employee { ID = 6 };

                ctx.AttachTo("People", shyam);
                ctx.AttachTo("People", marcelo);

                ctx.DeleteLink(shyam, "DirectReports", marcelo);
                ctx.SaveChanges(saveChangesOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => (p as PeopleManager).DirectReports).Where(p => p.ID == 5);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(5, person.ID);
                Assert.AreEqual(shyam, person);
                Assert.AreEqual(6, marcelo.ID);
                Assert.AreEqual(1, ctx.Links.Count());
            },
            new List<Version>() { V4, V4 },
            null);
        }

        #endregion DeleteLink

        #region AddRelatedObject
        [Ignore] // Remove Atom
        // [TestMethod]
        public void AddRelatedObjectTests()
        {
            SendSaveChangesAndVerifyContext(GetAddRelatedObjectTests());
        }

        IEnumerable<Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>> GetAddRelatedObjectTests()
        {
            // Add related object to retrieved object, for non-derived reference set property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                Employee p100 = new Employee { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                ctx.AddRelatedObject(pratik, "Friends", p100);

                ctx.SaveChanges(saveChangeOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => p.Friends).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.AreEqual(100, p100.ID);
                Assert.IsNotNull(pratik.Friends.Single(f => f == p100));

                Assert.AreEqual(2, ctx.Links.Where(l => l.Source == pratik).Count());
                Assert.IsNotNull(ctx.Links.Single(l => l.Target == p100));
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // Add related object to newly added object, for non-derived reference set property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                Employee p100 = new Employee { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                Employee p101 = new Employee { ID = 101 };
                p101.Skills = new List<string>();
                p101.Vacations = new List<Vacation>();

                ctx.AddObject("People", p100);
                ctx.AddRelatedObject(p100, "Friends", p101);

                ctx.SaveChanges(saveChangeOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => p.Friends).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.Friends.Single());

                Assert.AreEqual(p100, (Person)ctx.Links.Single().Source);
                Assert.AreEqual(p101, (Person)ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // Add related object to retrieved object, for derived reference set property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                Employee p100 = new Employee { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                ctx.AddRelatedObject(pratik, "Colleagues", p100);

                ctx.SaveChanges(saveChangeOption);

                query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Colleagues).Where(p => p.ID == 3);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(3, person.ID);
                Assert.AreEqual(pratik, person);
                Assert.AreEqual(100, p100.ID);
                Assert.IsNotNull(pratik.Colleagues.Single(f => f == p100));

                Assert.AreEqual(2, ctx.Links.Where(l => l.Source == pratik).Count());
                Assert.IsNotNull(ctx.Links.Single(l => l.Target == p100));
            },
            new List<Version>() { V4, V4, V4 },
            null);

            // Add related object to newly added object, for derived reference set property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                Employee p100 = new Employee { ID = 100 };
                p100.Skills = new List<string>();
                p100.Vacations = new List<Vacation>();

                Employee p101 = new Employee { ID = 101 };
                p101.Skills = new List<string>();
                p101.Vacations = new List<Vacation>();

                ctx.AddObject("People", p100);
                ctx.AddRelatedObject(p100, "Colleagues", p101);

                ctx.SaveChanges(saveChangeOption);

                var query = ctx.CreateQuery<Person>("People").Expand(p => (p as Employee).Colleagues).Where(p => p.ID == 100);
                ctx.MergeOption = MergeOption.OverwriteChanges;

                Person person = query.Single();
                Assert.AreEqual(100, person.ID);
                Assert.AreEqual(p100, person);
                Assert.AreEqual(101, p101.ID);
                Assert.AreEqual(p101, p100.Colleagues.Single());

                Assert.AreEqual(p100, (Person)ctx.Links.Single().Source);
                Assert.AreEqual(p101, (Person)ctx.Links.Single().Target);
            },
            new List<Version>() { V4, V4, V4 },
            null);
        }

        #endregion AddRelatedObject

        #region LoadProperty
        [Ignore] // Remove Atom
        // [TestMethod]
        public void LoadPropertyTests()
        {
            SendSaveChangesAndVerifyContext(GetLoadPropertyTests());
        }

        IEnumerable<Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>> GetLoadPropertyTests()
        {
            // attach and load non-derived reference property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                Employee pratik = new Employee { ID = 3 };
                ctx.AttachTo("People", pratik);
                var response = ctx.LoadProperty(pratik, "BestFriend");
                t.TestUtil.AssertContainsFalse(response.Query.RequestUri.OriginalString, typeof(Employee).FullName);
                Assert.IsTrue(response.Query.RequestUri.OriginalString.EndsWith("/BestFriend"));
                Person bestFriend = pratik.BestFriend;
                Assert.IsTrue(ctx.Entities.Contains(ctx.GetEntityDescriptor(bestFriend)));
                Assert.IsNotNull(ctx.Links.Single(l => l.Source == pratik && l.Target == bestFriend && l.SourceProperty == "BestFriend"));
            },
            new List<Version>() { V4 },
            null);

            // retrive entity and load non-derived reference property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                var response = ctx.LoadProperty(pratik, "BestFriend");
                Assert.IsTrue(response.Query.RequestUri.OriginalString.EndsWith(typeof(Employee).FullName + "/BestFriend"));
                Person bestFriend = pratik.BestFriend;
                Assert.IsTrue(ctx.Entities.Contains(ctx.GetEntityDescriptor(bestFriend)));
                Assert.IsNotNull(ctx.Links.Single(l => l.Source == pratik && l.Target == bestFriend && l.SourceProperty == "BestFriend"));
            },
            new List<Version>() { V4, V4 },
            null);

            // attach and load derived reference property, expect failure
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                Employee pratik = new Employee { ID = 3 };
                ctx.AttachTo("People", pratik);
                var response = ctx.LoadProperty(pratik, "Manager");
                Assert.Fail("Expected exception but received none.");
            },
            new List<Version>() { V4 },
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><m:error xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:code /><m:message>Resource not found for the segment 'Manager'.</m:message></m:error>");

            // retrive entity and load non-derived reference property
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                var response = ctx.LoadProperty(pratik, "Manager");
                Assert.IsTrue(response.Query.RequestUri.OriginalString.EndsWith(typeof(Employee).FullName + "/Manager"));
                Person manager = pratik.Manager;
                Assert.IsTrue(ctx.Entities.Contains(ctx.GetEntityDescriptor(manager)));
                Assert.IsNotNull(ctx.Links.Single(l => l.Source == pratik && l.Target == manager && l.SourceProperty == "Manager"));
            },
            new List<Version>() { V4, V4 },
            null);
        }

        #endregion LoadProperty

        #region NamedStreams
        [Ignore] // Remove Atom
        // [TestMethod]
        public void NamedStreamTests()
        {
            // NamedStreams operations are not supported in batch
            SendSaveChangesAndVerifyContext(GetNamedStreamTests(), new SaveChangesOptions[] { SaveChangesOptions.None });
        }

        IEnumerable<Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>> GetNamedStreamTests()
        {
            // attach and save named stream, expect failure
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                PeopleManager andy = new PeopleManager { ID = 2 };
                ctx.AttachTo("People", andy);
                ctx.SetSaveStream(andy, "Photo", new MemoryStream(), true, "abc/pqr");
                ctx.SaveChanges(saveChangeOption);
            },
            new List<Version>() { V4 },
            "The stream named 'Photo' cannot be modified because it does not have an edit-media link. Make sure that the stream name is correct and that an edit-media link for this stream is included in the entry element in the response.");

            // attach and load named stream, expect failure
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                PeopleManager andy = new PeopleManager { ID = 2 };
                ctx.AttachTo("People", andy);
                ctx.GetReadStream(andy, "Photo", new DataServiceRequestArgs());
                ctx.SaveChanges(saveChangeOption);
            },
            new List<Version>() { V4 },
            "The entity does not have a stream named 'Photo'. Make sure that the name of the stream is correct.\r\nParameter name: name");

            // retrive entity and save/load named stream
            yield return new Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>(
            (ctx, saveChangeOption) =>
            {
                var query = ctx.CreateQuery<Person>("People").Where(p => p.ID == 3);
                Employee pratik = (Employee)query.Single();
                ctx.SetSaveStream(pratik, "Photo", new MemoryStream(new byte[] { 1, 2, 3 }), true, "abc/pqr");
                ctx.SaveChanges(saveChangeOption);

                DataServiceRequestArgs arg = new DataServiceRequestArgs() { AcceptContentType = "abc/pqr" };
                var response = ctx.GetReadStream(pratik, "Photo", new DataServiceRequestArgs() { AcceptContentType = "abc/pqr" });
                byte[] buffer = new byte[4];
                Assert.AreEqual(3, response.Stream.Read(buffer, 0, 4));
                Assert.IsTrue(buffer[0] == 1 && buffer[1] == 2 && buffer[2] == 3);
            },
            new List<Version>() { V4, V4, V4 },
            null);
        }

        #endregion NamedStreams

        private void SendSaveChangesAndVerifyContext(IEnumerable<Tuple<Action<DataServiceContext, SaveChangesOptions>, List<Version>, string>> testCases, SaveChangesOptions[] saveChangesOptions = null)
        {
            t.TestUtil.RunCombinations(testCases, saveChangesOptions ?? new SaveChangesOptions[] { SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.None }, (testCase, saveChangesOption) =>
            {
                var testMethod = testCase.Item1;
                var dataServiceVersions = testCase.Item2;
                var expectedError = testCase.Item3;

                var service = ModelWithDerivedNavigationProperties();
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.StartService();
                    DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri, UriKind.Absolute), ODataProtocolVersion.V4);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.ResolveType = (typeName) =>
                    {
                        if (typeName == typeof(Employee).FullName)
                        {
                            return typeof(Employee);
                        }

                        if (typeName == typeof(PeopleManager).FullName)
                        {
                            return typeof(PeopleManager);
                        }

                        return null;
                    };

                    ctx.ResolveName = (type) =>
                    {
                        if (type == typeof(Person) || type == typeof(Employee) || type == typeof(PeopleManager))
                        {
                            return type.FullName;
                        }

                        return null;
                    };

                    ctx.SendingRequest2 += (sender, e) =>
                    {
                        if (saveChangesOption != SaveChangesOptions.BatchWithSingleChangeset)
                        {
                            if (dataServiceVersions.Count <= 0)
                            {
                                Assert.Fail("Number of actual requests greater than number of expected requests.");
                            }
                            Version dsv = dataServiceVersions[0];
                            dataServiceVersions.RemoveAt(0);

                            Assert.AreEqual(dsv == null ? null : dsv.Major + "." + dsv.Minor, e.RequestMessage.GetHeader("OData-Version"), "OData-Version mismatch.");
                            Assert.AreEqual("4.0", e.RequestMessage.GetHeader("OData-MaxVersion"), "OData-MaxVersion mismatch.");
                        }
                    };

                    try
                    {
                        ctx.MergeOption = MergeOption.OverwriteChanges;
                        testMethod(ctx, saveChangesOption);
                        if (!string.IsNullOrEmpty(expectedError))
                        {
                            Assert.Fail("Exception expected but received none.");
                        }

                        if (saveChangesOption != SaveChangesOptions.BatchWithSingleChangeset)
                        {
                            Assert.AreEqual(0, dataServiceVersions.Count, "Actual number of requests less than the expected number of requests.");
                        }
                    }
                    catch (Exception e)
                    {
                        string errorMsg = e.InnerException == null ? e.Message : e.InnerException.Message;
                        Assert.IsNotNull(expectedError, "Did not expect exception but received: " + errorMsg);
                        Assert.AreEqual(expectedError, errorMsg);
                    }
                }
            });
        }

        #endregion DataServiceContext Tests

        private static DSPServiceDefinition ModelWithDerivedNavigationProperties()
        {
            // Navigation Collection Property: Client - Entity, Server - NonEntity
            DSPMetadata metadata = new DSPMetadata("ModelWithDerivedNavProperties", "AstoriaUnitTests.Tests.DerivedProperty");

            var peopleType = metadata.AddEntityType("Person", null, null, false);
            metadata.AddKeyProperty(peopleType, "ID", typeof(int));
            metadata.AddPrimitiveProperty(peopleType, "Name", typeof(string));
            var bestFriendProperty = metadata.AddResourceReferenceProperty(peopleType, "BestFriend", peopleType);
            var friendsProperty = metadata.AddResourceSetReferenceProperty(peopleType, "Friends", peopleType);
            var aquaintancesProperty = metadata.AddResourceSetReferenceProperty(peopleType, "Aquaintances", peopleType);

            var peopleSet = metadata.AddResourceSet("People", peopleType);

            var officeType = metadata.AddComplexType("Office", null, null, false);
            metadata.AddPrimitiveProperty(officeType, "Building", typeof(string));
            metadata.AddPrimitiveProperty(officeType, "OfficeNumber", typeof(int));

            var vacationType = metadata.AddComplexType("Vacation", null, null, false);
            metadata.AddPrimitiveProperty(vacationType, "Description", typeof(string));
            metadata.AddPrimitiveProperty(vacationType, "StartDate", typeof(DateTimeOffset));
            metadata.AddPrimitiveProperty(vacationType, "EndDate", typeof(DateTimeOffset));

            var employeeType = metadata.AddEntityType("Employee", null, peopleType, false);
            metadata.AddCollectionProperty(employeeType, "Vacations", vacationType);
            metadata.AddComplexProperty(employeeType, "Office", officeType);
            metadata.AddCollectionProperty(employeeType, "Skills", ResourceType.GetPrimitiveResourceType(typeof(string)));
            metadata.AddNamedStreamProperty(employeeType, "Photo");

            var managerType = metadata.AddEntityType("PeopleManager", null, employeeType, false);

            var drProperty = metadata.AddResourceSetReferenceProperty(managerType, "DirectReports", employeeType);
            var managerProperty = metadata.AddResourceReferenceProperty(employeeType, "Manager", managerType);
            var colleaguesProperty = metadata.AddResourceSetReferenceProperty(employeeType, "Colleagues", employeeType);

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "Manager_DirectReports",
                new ResourceAssociationSetEnd(peopleSet, employeeType, managerProperty),
                new ResourceAssociationSetEnd(peopleSet, managerType, drProperty)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "BestFriend",
                new ResourceAssociationSetEnd(peopleSet, peopleType, bestFriendProperty),
                new ResourceAssociationSetEnd(peopleSet, peopleType, null)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "Friends",
                new ResourceAssociationSetEnd(peopleSet, peopleType, friendsProperty),
                new ResourceAssociationSetEnd(peopleSet, peopleType, null)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "Colleagues",
                new ResourceAssociationSetEnd(peopleSet, employeeType, colleaguesProperty),
                new ResourceAssociationSetEnd(peopleSet, employeeType, null)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "Aquaintances",
                new ResourceAssociationSetEnd(peopleSet, peopleType, aquaintancesProperty),
                new ResourceAssociationSetEnd(peopleSet, peopleType, null)));


            metadata.SetReadOnly();

            DSPContext context = new DSPContext();

            DSPResource people1 = new DSPResource(peopleType);
            people1.SetValue("ID", 1);
            people1.SetValue("Name", "Foo");
            people1.SetValue("Friends", new List<DSPResource>());

            DSPResource thanksgivingVacation = new DSPResource(vacationType);
            thanksgivingVacation.SetValue("Description", "Thanksgiving");
            thanksgivingVacation.SetValue("StartDate", new DateTime(2011, 11, 19));
            thanksgivingVacation.SetValue("EndDate", new DateTime(2011, 11, 27));

            DSPResource christmasVacation = new DSPResource(vacationType);
            christmasVacation.SetValue("Description", "Christmas");
            christmasVacation.SetValue("StartDate", new DateTime(2011, 12, 24));
            christmasVacation.SetValue("EndDate", new DateTime(2012, 1, 2));

            DSPResource andy = new DSPResource(managerType);
            andy.SetValue("ID", 2);
            andy.SetValue("Name", "Andy");
            andy.SetValue("Vacations", new List<DSPResource>() { thanksgivingVacation, christmasVacation });
            var office = new DSPResource(officeType);
            office.SetValue("Building", "Building 18");
            office.SetValue("OfficeNumber", 100);
            andy.SetValue("Office", office);
            andy.SetValue("Skills", new List<string>() { "CSharp", "VB", "SQL" });
            andy.SetValue("Friends", new List<DSPResource>() { people1 });
            andy.SetValue("Aquaintences", new List<DSPResource>());

            DSPResource pratik = new DSPResource(employeeType);
            pratik.SetValue("ID", 3);
            pratik.SetValue("Name", "Pratik");
            pratik.SetValue("Manager", andy);
            pratik.SetValue("Vacations", new List<DSPResource>() { christmasVacation });
            office = new DSPResource(officeType);
            office.SetValue("Building", "Building 18");
            office.SetValue("OfficeNumber", 101);
            pratik.SetValue("Office", office);
            pratik.SetValue("Skills", new List<string>() { "CSharp", "VB", "SQL" });
            pratik.SetValue("Friends", new List<DSPResource>() { people1 });
            pratik.SetValue("Aquaintences", new List<DSPResource>());

            DSPResource jimmy = new DSPResource(employeeType);
            jimmy.SetValue("ID", 4);
            jimmy.SetValue("Name", "Jimmy");
            jimmy.SetValue("Manager", andy);
            jimmy.SetValue("Vacations", new List<DSPResource>() { thanksgivingVacation, christmasVacation });
            office = new DSPResource(officeType);
            office.SetValue("Building", "Building 18");
            office.SetValue("OfficeNumber", 102);
            jimmy.SetValue("Office", office);
            jimmy.SetValue("Skills", new List<string>() { "CSharp", "SQL" });
            jimmy.SetValue("Friends", new List<DSPResource>() { people1 });
            jimmy.SetValue("Aquaintences", new List<DSPResource>());

            andy.SetValue("DirectReports", new List<DSPResource>() { pratik, jimmy });

            DSPResource shyam = new DSPResource(managerType);
            shyam.SetValue("ID", 5);
            shyam.SetValue("Name", "Shyam");
            shyam.SetValue("Manager", shyam);
            shyam.SetValue("Vacations", new List<DSPResource>() { thanksgivingVacation, christmasVacation });
            office = new DSPResource(officeType);
            office.SetValue("Building", "Building 18");
            office.SetValue("OfficeNumber", 103);
            shyam.SetValue("Office", office);
            shyam.SetValue("Skills", new List<string>());
            shyam.SetValue("Friends", new List<DSPResource>() { people1 });
            shyam.SetValue("Aquaintences", new List<DSPResource>());

            DSPResource marcelo = new DSPResource(employeeType);
            marcelo.SetValue("ID", 6);
            marcelo.SetValue("Name", "Marcelo");
            marcelo.SetValue("Manager", shyam);
            marcelo.SetValue("Vacations", new List<DSPResource>());
            office = new DSPResource(officeType);
            office.SetValue("Building", "Building 18");
            office.SetValue("OfficeNumber", 104);
            marcelo.SetValue("Office", office);
            marcelo.SetValue("Skills", new List<string>() { "CSharp", "VB", "SQL" });
            marcelo.SetValue("Friends", new List<DSPResource>() { people1 });
            marcelo.SetValue("Aquaintences", new List<DSPResource>());

            andy.SetValue("Manager", shyam);
            shyam.SetValue("DirectReports", new List<DSPResource>() { andy, marcelo });

            pratik.SetValue("BestFriend", andy);
            andy.SetValue("BestFriend", shyam);
            shyam.SetValue("BestFriend", marcelo);
            marcelo.SetValue("BestFriend", jimmy);
            jimmy.SetValue("BestFriend", people1);
            people1.SetValue("BestFriend", pratik);

            andy.SetValue("Colleagues", new List<DSPResource>() { marcelo });
            pratik.SetValue("Colleagues", new List<DSPResource>() { jimmy });
            jimmy.SetValue("Colleagues", new List<DSPResource>() { pratik });
            marcelo.SetValue("Colleagues", new List<DSPResource>() { andy });
            shyam.SetValue("Colleagues", new List<DSPResource>());

            people1.SetValue("Aquaintances", new List<DSPResource>() { pratik, andy, jimmy, shyam, marcelo });

            var people = context.GetResourceSetEntities("People");
            people.Add(people1);
            people.Add(andy);
            people.Add(pratik);
            people.Add(jimmy);
            people.Add(shyam);
            people.Add(marcelo);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true },
            };

            return service;

        }

        private IEdmModel LoadModelFromString()
        {
            string metadata = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
	<edmx:DataServices>
		<Schema Namespace=""AstoriaUnitTests.Tests.DerivedProperty"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
			<EntityType Name=""Person"">
				<Key>
					<PropertyRef Name=""ID"" />
				</Key>
				<Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
				<Property Name=""Name"" Type=""Edm.String"" />
				<NavigationProperty Name=""BestFriend"" Type=""AstoriaUnitTests.Tests.DerivedProperty.Person"" />
				<NavigationProperty Name=""Friends"" Type=""Collection(AstoriaUnitTests.Tests.DerivedProperty.Person)"" />
				<NavigationProperty Name=""Aquaintances"" Type=""Collection(AstoriaUnitTests.Tests.DerivedProperty.Person)"" />
			</EntityType>
			<EntityType Name=""Employee"" BaseType=""AstoriaUnitTests.Tests.DerivedProperty.Person"">
				<Property Name=""Vacations"" Type=""Collection(AstoriaUnitTests.Tests.DerivedProperty.Vacation)"" Nullable=""false"" />
				<Property Name=""Office"" Type=""AstoriaUnitTests.Tests.DerivedProperty.Office"" />
				<Property Name=""Skills"" Type=""Collection(Edm.String)"" Nullable=""false"" />
				<Property Name=""Photo"" Type=""Edm.Stream"" Nullable=""false"" />
				<NavigationProperty Name=""Manager"" Type=""AstoriaUnitTests.Tests.DerivedProperty.PeopleManager"" Partner=""DirectReports"" />
				<NavigationProperty Name=""Colleagues"" Type=""Collection(AstoriaUnitTests.Tests.DerivedProperty.Employee)"" />
			</EntityType>
			<ComplexType Name=""Vacation"">
				<Property Name=""Description"" Type=""Edm.String"" />
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
			</ComplexType>
			<ComplexType Name=""Office"">
				<Property Name=""Building"" Type=""Edm.String"" />
				<Property Name=""OfficeNumber"" Type=""Edm.Int32"" Nullable=""false"" />
			</ComplexType>
			<EntityType Name=""PeopleManager"" BaseType=""AstoriaUnitTests.Tests.DerivedProperty.Employee"">
				<NavigationProperty Name=""DirectReports"" Type=""Collection(AstoriaUnitTests.Tests.DerivedProperty.Employee)"" Partner=""Manager"" />
			</EntityType>
			<EntityContainer Name=""ModelWithDerivedNavProperties"">
				<EntitySet Name=""People"" EntityType=""AstoriaUnitTests.Tests.DerivedProperty.Person"">
					<NavigationPropertyBinding Path=""AstoriaUnitTests.Tests.DerivedProperty.Employee/Manager"" Target=""People"" />
					<NavigationPropertyBinding Path=""AstoriaUnitTests.Tests.DerivedProperty.PeopleManager/DirectReports"" Target=""People"" />
					<NavigationPropertyBinding Path=""AstoriaUnitTests.Tests.DerivedProperty.Employee/Colleagues"" Target=""People"" />
					<NavigationPropertyBinding Path=""BestFriend"" Target=""People"" />
					<NavigationPropertyBinding Path=""Friends"" Target=""People"" />
					<NavigationPropertyBinding Path=""Aquaintances"" Target=""People"" />
				</EntitySet>
			</EntityContainer>
		</Schema>
	</edmx:DataServices>
</edmx:Edmx>";
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(new StringReader(metadata));
            IEdmModel model = Microsoft.OData.Edm.Csdl.CsdlReader.Parse(reader);
            return model;
        }
    }

    class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Person BestFriend { get; set; }
        public ICollection<Person> Friends { get; set; }
        public ICollection<Person> Aquaintances { get; set; }
    }

    class Vacation
    {
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }

    class Employee : Person
    {
        public Office Office { get; set; }  // complex property
        public ICollection<Vacation> Vacations { get; set; } // collection complex property
        public ICollection<string> Skills { get; set; } // collection primitive property
        public PeopleManager Manager { get; set; }
        public ICollection<Employee> Colleagues { get; set; }
        public DataServiceStreamLink Photo { get; set; }
    }

    class PeopleManager : Employee
    {
        public ICollection<Employee> DirectReports { get; set; }
    }

    class Office
    {
        public string Building { get; set; }
        public int OfficeNumber { get; set; }
    }

    class MyPersonEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public MyPersonEntity BestFriend { get; set; }
    }

    class MyOfficeComplexType
    {
        public string BuildingName { get; set; }
        public int OfficeNumber { get; set; }
    }

    class MyVacationComplexType
    {
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }

    class MyEmployeeEntity : MyPersonEntity
    {
        public Office Office { get; set; }  // complex property
        public ICollection<Vacation> Vacations { get; set; } // collection complex property
        public ICollection<string> Skills { get; set; } // collection primitive property
        public MyEmployeeEntity Manager { get; set; }
        public ICollection<MyEmployeeEntity> Colleagues { get; set; }
        public ICollection<MyEmployeeEntity> DirectReports { get; set; }
        public DataServiceStreamLink Photo { get; set; }
    }

    class MyPersonNonEntity
    {
        public string Name { get; set; }
        public MyPersonNonEntity BestFriend { get; set; }
        public ICollection<MyPersonNonEntity> Friends { get; set; }
    }

    class MyEmployeeNonEntity : MyPersonNonEntity
    {
        public int MyID { get; set; }
        public Office Office { get; set; }
        public string OfficeBuilding { get; set; }
        public int OfficeNumber { get; set; }
        public MyOfficeComplexType MyOffice { get; set; }  // complex property
        public ICollection<MyVacationComplexType> MyVacations { get; set; } // collection complex property
        public ICollection<Vacation> Vacations { get; set; } // collection complex property
        public ICollection<string> Skills { get; set; } // collection primitive property
        public MyEmployeeNonEntity Manager { get; set; }
        public ICollection<MyEmployeeNonEntity> Colleagues { get; set; }
        public ICollection<MyEmployeeNonEntity> DirectReports { get; set; }
        public MyEmployeeNonEntity SkipLevelManager { get; set; }
        public DataServiceStreamLink Photo { get; set; }
    }
}
