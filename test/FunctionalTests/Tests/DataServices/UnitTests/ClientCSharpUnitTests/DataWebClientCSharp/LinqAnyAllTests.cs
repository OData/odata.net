//---------------------------------------------------------------------
// <copyright file="LinqAnyAllTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client;
using System.Data.Test.Astoria;
using System.Linq;
using AstoriaUnitTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace AstoriaUnitTests.Tests
{
    [TestClass]
    public class LinqAnyAllTests
    {
        #region collection model
        public class MVComplexType
        {
            public string Name { get; set; }
            public List<int> Numbers { get; set; }
        }

        public class EntityWithCollections
        {
            public int ID { get; set; }
            public List<int> CollectionOfInt { get; set; }
            public List<string> CollectionOfString { get; set; }
            public List<MVComplexType> CollectionOfComplexType { get; set; }
        }
        #endregion

        [TestMethod]
        public void FilterCollectionWithAnyAll()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"), ODataProtocolVersion.V4);

            var values = ctx.CreateQuery<EntityWithCollections>("Values");
            var testCases = new[]
            {
                new{
                    q = from e in values
                        where e.CollectionOfInt.Any()
                        select e,
                    url = "Values?$filter=CollectionOfInt/any()"
                },
                new{
                    q = from e in values
                        where e.CollectionOfInt.Any() && e.ID == 0
                        select e,
                    url = "Values?$filter=CollectionOfInt/any() and ID eq 0"
                },
                new{
                    q = from e in values
                        where e.CollectionOfInt.Any(mv => mv == 2 )
                        select e,
                    url = "Values?$filter=CollectionOfInt/any(mv:mv eq 2)"
                },
                new{
                    q = from e in values
                        where e.CollectionOfInt.Any(mv => mv > e.ID ) && e.ID <100
                        select e,
                    url = "Values?$filter=CollectionOfInt/any(mv:mv gt $it/ID) and ID lt 100"
                },
                new{
                    q = from e in values
                        where e.CollectionOfComplexType.Any(mv => e.CollectionOfString.All(s => s.StartsWith(mv.Name)) || e.ID <100) && e.ID > 50
                        select e,
                    url = "Values?$filter=CollectionOfComplexType/any(mv:$it/CollectionOfString/all(s:startswith(s,mv/Name)) or $it/ID lt 100) and ID gt 50"
                },
                new{
                    q = from e in values
                        where e.CollectionOfComplexType.All(mv => mv.Name.StartsWith("a") || e.ID <100) && e.ID > 50
                        select e,
                    url = "Values?$filter=CollectionOfComplexType/all(mv:startswith(mv/Name,'a') or $it/ID lt 100) and ID gt 50"
                },
                new{
                    q = from e in values
                        where e.CollectionOfComplexType.All(mv => mv.Name.Contains("a") || mv.Numbers.All(n=>n % 2 == 0)) && e.ID/5 == 3
                        select e,
                    url = "Values?$filter=CollectionOfComplexType/all(mv:contains(mv/Name,'a') or mv/Numbers/all(n:n mod 2 eq 0)) and ID div 5 eq 3"
                },
            };

            TestUtil.RunCombinations(testCases, (testCase) =>
            {
                Assert.AreEqual(ctx.BaseUri.AbsoluteUri + testCase.url,testCase.q.ToString(), "url == q.ToString()");
            });
        }

        #region Movie model
        public class Movie
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTimeOffset ReleaseYear { get; set; }
            public Person Director { get; set; }
            public List<Person> Actors { get; set; }
            public List<Award> Awards { get; set; }
            public List<string> Titles { get; set; }
        }

        public class Award
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTimeOffset AwardDate { get; set; }
            public Movie Movie { get; set; }
            public Person Recepient { get; set; }
        }

        public class Person
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTimeOffset DateOfBirth { get; set; }
            public List<Movie> DirectedMovies { get; set; }
            public List<Award> Awards { get; set; }
        }

        public class Actor : Person
        {
            public List<Movie> Movies { get; set; }
        }

        public class MegaStar : Actor
        {
            public string MegaStartProp { get; set; }
        }
        #endregion 

        [TestMethod]
        public void FilterNavigationWithAnyAll()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"), ODataProtocolVersion.V4);
            ctx.ResolveName = (type) =>
                                  {
                                      return "NS." + type.Name;
                                  };

            var movies = ctx.CreateQuery<Movie>("Movies");
            var testCases = new[]
            {
                new{
                    q = from m in movies
                        where m.Awards.Any()
                        select m,
                    url = "Movies?$filter=Awards/any()"
                },
                new{
                    q = from m in movies
                        where m.Awards.Any() && m.ID == 0
                        select m,
                    url = "Movies?$filter=Awards/any() and ID eq 0"
                },
                new{
                    q = from m in movies
                        where m.Awards.Any(a => a.ID == 2)
                        select m,
                    url = "Movies?$filter=Awards/any(a:a/ID eq 2)"
                },
                new{
                    q = from m in movies
                        where m.Awards.Any(a => a.ID == m.ID)
                        select m,
                    url = "Movies?$filter=Awards/any(a:a/ID eq $it/ID)"
                },
                new{
                    q = from m in movies
                        where m.Director.Awards.All(a => a.Movie == m)
                        select m,
                    url = "Movies?$filter=Director/Awards/all(a:a/Movie eq $it)"
                },
                new{
                    q = from m in movies
                        where m.Actors.Any(a => a.DirectedMovies.All(dm => dm == m))
                        select m,
                    url = "Movies?$filter=Actors/any(a:a/DirectedMovies/all(dm:dm eq $it))"
                },
                new{
                    q = from m in movies
                        where m.Actors.Any(a => a.DirectedMovies.All(dm => dm == m && m.Awards.All(aw=>aw.Movie.Director == dm.Director)))
                        select m,
                    url = "Movies?$filter=Actors/any(a:a/DirectedMovies/all(dm:dm eq $it and $it/Awards/all(aw:aw/Movie/Director eq dm/Director)))"
                },
                new{
                    q = from m in movies
                        where m.Actors.Any(a => a is MegaStar)
                        select m,
                    url = "Movies?$filter=Actors/any(a:isof(a, 'NS.MegaStar'))"
                },
                new{
                    q = from m in movies
                        where m.Awards.All(aw => aw.Movie.Director is MegaStar)
                        select m,
                    url = "Movies?$filter=Awards/all(aw:isof(aw/Movie/Director, 'NS.MegaStar'))"
                },
                new{
                    q = from m in movies
                        where m.Awards.All(aw => m.Director is MegaStar && !aw.Movie.Actors.Any(a=> a is MegaStar))
                        select m,
                    url = "Movies?$filter=Awards/all(aw:isof($it/Director, 'NS.MegaStar') and not aw/Movie/Actors/any(a:isof(a, 'NS.MegaStar')))"
                },
                new{
                    q = from m in movies
                        where m.Awards.All(aw => m.Director.FirstName.StartsWith("Hus") && !aw.Movie.Actors.Any(a=> a is MegaStar))
                        select m,
                    url = "Movies?$filter=Awards/all(aw:startswith($it/Director/FirstName,'Hus') and not aw/Movie/Actors/any(a:isof(a, 'NS.MegaStar')))"
                },
                new{
                    q = from m in movies
                        where m.Awards.All(aw => m.Director is MegaStar && ((MegaStar)m.Director).MegaStartProp.StartsWith("Hus") && aw.Recepient == m.Director)
                        select m,
                    url = "Movies?$filter=Awards/all(aw:isof($it/Director, 'NS.MegaStar') and startswith(cast($it/Director,'NS.MegaStar')/MegaStartProp,'Hus') and aw/Recepient eq $it/Director)"
                },
                new{
                    q = from m in movies
                        where m.Actors.OfType<MegaStar>().Any()
                        select m,
                    url = "Movies?$filter=Actors/NS.MegaStar/any()"
                },
                new{
                    q = from m in movies
                        where m.Actors.OfType<MegaStar>().Any( ms=> ms.Awards.Any())
                        select m,
                    url = "Movies?$filter=Actors/NS.MegaStar/any(ms:ms/Awards/any())"
                },
                new{
                    q = from m in movies
                        where m.Actors.OfType<MegaStar>().All( ms=> ms.Awards.Any())
                        select m,
                    url = "Movies?$filter=Actors/NS.MegaStar/all(ms:ms/Awards/any())"
                },
                new{
                    q = from m in movies
                        where m.Actors.All(a=> a is MegaStar && a.Awards.Any())
                        select m,
                    url = "Movies?$filter=Actors/all(a:isof(a, 'NS.MegaStar') and a/Awards/any())"
                },
                new{
                    q = from m in movies
                        where m.Actors.OfType<MegaStar>().All( ms=> ms.Awards.Any(a=> a.AwardDate > new DateTime(0, DateTimeKind.Utc) && ms.DirectedMovies.Any() && m.Director == ms))
                        select m,
                    url = "Movies?$filter=Actors/NS.MegaStar/all(ms:ms/Awards/any(a:a/AwardDate gt 0001-01-01T00:00:00Z and ms/DirectedMovies/any() and $it/Director eq ms))"
                },
                new{
                    q = from m in movies
                        where m.Actors.OfType<MegaStar>().All( ms=> ms.Awards.Any(a=> a.AwardDate > new DateTime(0, DateTimeKind.Utc) && ms.DirectedMovies.Any(dm=> dm.Awards.All(aw=> aw.Recepient.FirstName == ms.FirstName )) && m.Director == ms))
                        select m,
                    url = "Movies?$filter=Actors/NS.MegaStar/all(ms:ms/Awards/any(a:a/AwardDate gt 0001-01-01T00:00:00Z and ms/DirectedMovies/any(dm:dm/Awards/all(aw:aw/Recepient/FirstName eq ms/FirstName)) and $it/Director eq ms))"
                },
                new{
                    q = from m in movies
                        where m.Awards.Any(aw => aw.Recepient is MegaStar && m.Actors.OfType<MegaStar>().All(a=>a.DateOfBirth > new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
                        select m,
                    url = "Movies?$filter=Awards/any(aw:isof(aw/Recepient, 'NS.MegaStar') and $it/Actors/NS.MegaStar/all(a:a/DateOfBirth gt 2010-01-01T00:00:00Z))"
                },
            };

            TestUtil.RunCombinations(testCases, (testCase) =>
            {
                Assert.AreEqual(ctx.BaseUri.AbsoluteUri + testCase.url, testCase.q.ToString(), "url == q.ToString()");
            });
        }

        [TestMethod]
        public void InvalidAnyAllUsage()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"), ODataProtocolVersion.V4);
            var moviesQuery = ctx.CreateQuery<Movie>("Movies");
            var collectionsQuery = ctx.CreateQuery<EntityWithCollections>("Values");
            var testCases = new Tuple<IQueryable, string>[]
                    {
                        // in $filter, onyl navigation and collection properties are supported.
                        new Tuple<IQueryable, string>(
                            moviesQuery.Where(m=>m.Name.Any()),
                            "The source parameter for the 'Any' method has to be either a navigation or a collection property."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.Where(m=>m.Name.Any(c=>c == 'a')),
                            "The source parameter for the 'Any' method has to be either a navigation or a collection property."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.Where(m=>m.Name.All(c=>c == 'a')),
                            "The source parameter for the 'All' method has to be either a navigation or a collection property."),

                        // in $orderby
                        new Tuple<IQueryable, string>(
                            moviesQuery.OrderBy(m=>m.Actors.Any()),
                            "The method 'Any' is not supported by the 'orderby' query option."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.OrderBy(m=>m.Actors.Any(a=>a.FirstName.StartsWith("a"))),
                            "The method 'Any' is not supported by the 'orderby' query option."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.OrderBy(m=>m.Actors.All(a=>a.FirstName.StartsWith("a"))),
                            "The method 'All' is not supported by the 'orderby' query option."),

                        new Tuple<IQueryable, string>(
                            moviesQuery.OrderBy(m=>m.Name).ThenBy(m=>m.Actors.Any()),
                            "The method 'Any' is not supported by the 'orderby' query option."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.OrderBy(m=>m.Name).ThenBy(m=>m.Actors.Any()),
                            "The method 'Any' is not supported by the 'orderby' query option."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.OrderByDescending(m=>m.Name).ThenBy(m=>m.Actors.Any()),
                            "The method 'Any' is not supported by the 'orderby' query option."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.OrderByDescending(m=>m.Name).ThenBy(m=>m.Actors.Any()),
                            "The method 'Any' is not supported by the 'orderby' query option."),
                        new Tuple<IQueryable, string>(
                            collectionsQuery.OrderByDescending(m=>m.CollectionOfInt.Any()),
                            "The method 'Any' is not supported by the 'orderby' query option."),
                        new Tuple<IQueryable, string>(
                            collectionsQuery.OrderByDescending(m=>m.ID).ThenBy(m=>m.CollectionOfInt.Any()),
                            "The method 'Any' is not supported by the 'orderby' query option."),

                       // in $select
                        new Tuple<IQueryable, string>(
                            moviesQuery.Select(m=>m.Awards.Any()),
                            "The method 'Select' is not supported."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.Select(m=>new Movie() { ID = m.ID, Awards = m.Awards.Any() ? m.Awards : new List<Award>()}),
                            "Initializing instances of the entity type AstoriaUnitTests.Tests.LinqAnyAllTests+Movie with the expression IIF(m.Awards.Any(), m.Awards, value(System.Collections.Generic.List`1[AstoriaUnitTests.Tests.LinqAnyAllTests+Award])) is not supported."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.Select(m=>new Movie() { ID = m.ID, Actors = m.Actors.OrderBy(a=>a.Awards.Any()).ToList()}),
                            "Initializing instances of the entity type AstoriaUnitTests.Tests.LinqAnyAllTests+Movie with the expression m.Actors.OrderBy(a => a.Awards.Any()) is not supported."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.Select(m=>new { ID = m.ID, Actors = m.Actors.Any()}),
                            "Constructing or initializing instances of the type <>f__AnonymousType1b`2[System.Int32,System.Boolean] with the expression m.Actors.Any() is not supported."),
                        new Tuple<IQueryable, string>(
                            moviesQuery.Select(m=>new { ID = m.ID, Actors = m.Actors.OrderBy(a=>a.Awards.Any()).ToList()}),
                            "Constructing or initializing instances of the type <>f__AnonymousType1b`2[System.Int32,System.Collections.Generic.List`1[AstoriaUnitTests.Tests.LinqAnyAllTests+Person]] with the expression m.Actors.OrderBy(a => a.Awards.Any()) is not supported."),
                    };

            TestUtil.RunCombinations(testCases, (testCase)=>
            {
                LinqTests.VerifyNotSupportedQuery(testCase.Item1, testCase.Item2);
            });
        }

        [TestMethod]
        public void AnyAllClientVersionTests()
        {
            TestUtil.RunCombinations(ServiceVersion.DataServiceProtocolVersions, maxProtocolVersion =>
            {
                DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"), maxProtocolVersion);

                var movies = ctx.CreateQuery<Movie>("Movies");
                var testCases = new[]
                {
                    new{
                        q = from m in movies
                            where m.Awards.Any()
                            select m,
                        ErrorMessage = "Error translating Linq expression to URI: The method 'Any' is not supported when MaxProtocolVersion is less than '4.0'."
                    },
                    new{
                        q = from m in movies
                            where m.Awards.Any(a => a.ID == 2)
                            select m,
                        ErrorMessage = "Error translating Linq expression to URI: The method 'Any' is not supported when MaxProtocolVersion is less than '4.0'."
                    },
                    new{
                        q = from m in movies
                            where m.Titles.Any(t => t.Contains("Space"))
                            select m,
                        ErrorMessage = "Error translating Linq expression to URI: The method 'Any' is not supported when MaxProtocolVersion is less than '4.0'."
                    },
                    new{
                        q = from m in movies
                            where m.Actors.OfType<MegaStar>().Any()
                            select m,
                        ErrorMessage = "Error translating Linq expression to URI: The method 'OfType' is not supported when MaxProtocolVersion is less than '4.0'."
                    },
                    new{
                        q = from m in movies
                            where m.Director.Awards.All(a => a.Movie == m)
                            select m,
                        ErrorMessage = "Error translating Linq expression to URI: The method 'All' is not supported when MaxProtocolVersion is less than '4.0'."
                    },
                };

                TestUtil.RunCombinations(testCases, testCase =>
                {
                    string qstr = testCase.q.ToString();
                    if (maxProtocolVersion < ODataProtocolVersion.V4)
                    {
                        Assert.AreEqual(testCase.ErrorMessage, qstr, "ErrorMessage == qstr");
                    }
                    else
                    {
                        Assert.IsTrue(qstr.Contains("any(") || qstr.Contains("all("), "qstr should contain any(...) or all(...)");
                    }
                });
            });
        }
    }
}
