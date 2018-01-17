//---------------------------------------------------------------------
// <copyright file="LinqTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.ServiceModel.Web;
    using System.Text.RegularExpressions;
    using AstoriaUnitTests.DataWebClientCSharp.AlternativeNS;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NorthwindModel;
    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [DeploymentItem("Workspaces", "Workspaces")]
    [TestClass]
    public class LinqTests
    {
        private static DataServiceContext context;
        private static Uri _baseUri;
        private static LocalWebRequest request;
        private static ReadOnlyTestContext baseLineContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // An in-memory web request is required, because results are comparing
            // in-depth using references, which won't work with "real" unwired
            // objects coming from a different domain.
            OpenWebDataServiceHelper.ForceVerboseErrors = true;
            request = (LocalWebRequest)TestWebRequest.CreateForLocal();
            request.AddToInitializeService(new DataServicesFeaturesSection() { ReplaceFunction = new DataServicesReplaceFunctionFeature() { Enable = true } });
            request.DataServiceType = typeof(ReadOnlyTestContext);
            request.ForceVerboseErrors = true;
            request.StartService();
            _baseUri = request.ServiceRoot;

            context = new DataServiceContext(_baseUri);
            //context.EnableAtom = true;
            //context.Format.UseAtom();
            context.MergeOption = MergeOption.NoTracking;
            context.SendingRequest2 += LinqTests.VerifyMimeTypeForCountRequests;
            baseLineContext = ReadOnlyTestContext.CreateBaseLineContext();
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (request != null)
            {
                request.Dispose();
                request = null;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqSimpleNavigation()
        {
            var baseline = baseLineContext.Teams;

            var queryable = context.CreateQuery<Team>("Teams");
            RunTest(baseline, queryable);

            queryable = context.CreateQuery<Team>("/Teams");
            RunTest(baseline, queryable);

            queryable = context.CreateQuery<Team>("Teams/");
            RunTest(baseline, queryable);

            queryable = context.CreateQuery<Team>("/Teams/");
            RunTest(baseline, queryable);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void TestContinuation()
        {
            using (System.Data.Test.Astoria.TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
            {
                OpenWebDataServiceHelper.ForceVerboseErrors = true;
                OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                {
                    config.SetEntitySetPageSize("*", 2);
                };

                using (HttpBasedWebRequest request = (HttpBasedWebRequest)TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(ReadOnlyTestContext);
                    request.StartService();
                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.MergeOption = MergeOption.NoTracking;

                    var teams = from cs in ctx.CreateQuery<Team>("Teams")
                                select new Team()
                                {
                                    TeamID = cs.TeamID,
                                    TeamName = cs.TeamName
                                };

                    DataServiceCollection<Team> sdpTeams = new DataServiceCollection<Team>(teams, TrackingMode.None);

                    Assert.IsNotNull(sdpTeams.Continuation, "sdpTeams.Continuation should not be null");

                    sdpTeams.Load(teams.ToList());

                    Assert.IsNull(sdpTeams.Continuation, "sdpTeams.Continuation should be null");
                }
            }
        }

        // [TestMethod]
        public void TestNestedQuery()
        {
            using (System.Data.Test.Astoria.TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
            {
                OpenWebDataServiceHelper.ForceVerboseErrors = true;

                using (LocalWebRequest request = (LocalWebRequest)TestWebRequest.CreateForLocal())
                {
                    request.DataServiceType = typeof(NorthwindContext);
                    request.StartService();
                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.MergeOption = MergeOption.NoTracking;

                    var q = from c in ctx.CreateQuery<northwindClient.Categories>("Categories")
                            select new { PN = (from p in c.Products select p), Cat = c };

                    foreach (var c in q)
                    {
                        Assert.IsTrue(c.ToString().Contains("Cat"));
                        Assert.IsTrue(c.ToString().Contains("PN"));
                    }
                }
            }
        }

        [TestMethod]
        public void VerifyQueriesNotSupported()
        {
            var q = from t in context.CreateQuery<Team>("Teams")
                    select new
                    {
                        nonentity = from x in
                                        (from p in t.Players
                                         select new { field = p.ID }
                                         )
                                    select x
                    };

            VerifyNotSupportedQuery(q);

            var q1 = from t in context.CreateQuery<Team>("Teams")
                     select new
                     {
                         nonentity = from x in
                                         (from p in t.Players
                                          select new { field = p.Team.Players, field1 = p.ID }
                                          )
                                     from a in x.field
                                     select a
                     };

            VerifyNotSupportedQuery(q1);

            var q2 = from t in context.CreateQuery<Team>("Teams")
                     from p in t.Players
                     select new
                     {
                         nonentity = p.Lastname
                     };
            VerifyNotSupportedQuery(q2);

            var q3 = context.CreateQuery<Team>("Teams").Where(t => t.TeamID > 1).Select(t => new { field = t.Players.Select(p => new { id = p.ID }).Select(a => a) });
            VerifyNotSupportedQuery(q3);

            var q4 = context.CreateQuery<Team>("Teams").Where(t => t.TeamID > 1).Select(t => new { field = t.Players.SelectMany(p => p.Team.Players, (a, p) => new { id = p.ID }).Select(a => a) });
            VerifyNotSupportedQuery(q4);

            var q5 = context.CreateQuery<Team>("Teams").Where(t => t.TeamID > 1).Select(t => new { field = t.Players.Select(p => new { id = p.ID }).ToList().Select(a => a) });
            VerifyNotSupportedQuery(q5);
        }

        // [TestMethod]
        public void ShouldThrowNullExceptionForEntryReturnedByNavigationProperty()
        {

            var q = from t in context.CreateQuery<Team>("Teams")
                    where t.TeamName == "Cubs" //team without a stadium
                    select new
                    {
                        stadiumname = t.HomeStadium.Name
                    };

            try
            {
                q.First();
                Assert.Fail("Null reference exception was expected");
            }
            catch (NullReferenceException nre)
            {
                Assert.AreEqual("An entry returned by the navigation property 'HomeStadium' is null and cannot be initialized. You should check for a null value before accessing this property.", nre.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Null reference exception was expected but the following is received:\n", e.Message);
            }
        }

        /// <summary>
        /// This method helps normalize the magic names generated by the compiler for anonymous class names.
        /// The compiler can choose to change the names of anonymous classes and we cannot rely on them to
        /// decide on test case success.
        /// According to ericli, any change to the code can cause the order in which the compiler requests unique 
        /// numbers to change from run to run.
        /// </summary>
        /// <param name="selector">String to normalize.</param>
        /// <returns>Normalized string.</returns>
        public static string Normalize(string selector)
        {
            // Normalizing closure classes - Naming convention - P<N>C__SI
            // P - class instance name or empty
            // N - original name, if any
            // C - character 1 through s. 'c' for anonymous type
            // S - descriptive suffix
            // I - an optional unique number
            string pattern = @"<[\w|_|@|$]*>\w?_{2}\w*";
            Regex rx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.Replace(selector.ToString(), delegate(Match m) { return Regex.Replace(m.Value, pattern, "$"); });
        }

        public static void VerifyNotSupportedQuery(IQueryable q, string error = null)
        {
            Exception e = null;
            try
            {
                foreach (var o in q)
                {
                }
                Debug.WriteLine("Query: " + q.ToString());
            }
            catch (NotSupportedException nse)
            {
                e = nse;
                if (error != null)
                {
                    Assert.AreEqual(Normalize(error), Normalize(nse.Message));
                }
            }
            Assert.IsInstanceOfType(e, typeof(NotSupportedException));
        }

        // [TestMethod]
        public void LinqFindByKeySimple()
        {
            // no projections            

            // check uri here also since need to be using key syntax.

            var baseline = baseLineContext.Teams.Where(t => t.TeamID == 3);
            var queryable = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 3);
            RunTest(baseline, queryable);

            var writer = TestURIWriter.CreateURIWriter<Team>(context.BaseUri.ToString(), "Teams").
                FindByKey("TeamID", 3);
            CheckURI(writer, queryable);

            int x = 2;
            var baseline2 = baseLineContext.Teams.Where(t => t.TeamID == 2);
            var queryable2 = context.CreateQuery<Team>("Teams").Where(c => c.TeamID == x);
            RunTest(baseline2, queryable2);

            var writer2 = TestURIWriter.CreateURIWriter<Team>(context.BaseUri.ToString(), "Teams").
                FindByKey("TeamID", 2);
            CheckURI(writer2, queryable2);

            x = 1;
            var baseline3 = baseLineContext.Teams.Where(t => t.TeamID == 1);
            var queryable3 = context.CreateQuery<Team>("Teams").Where(c => c.TeamID == x);
            RunTest(baseline3, queryable3);

            var writer3 = TestURIWriter.CreateURIWriter<Team>(context.BaseUri.ToString(), "Teams").
                FindByKey("TeamID", 1);
            CheckURI(writer3, queryable3);

            var baseline4 = baseLineContext.Teams.Where(t => t.TeamID == 4);
            var queryable4 = context.CreateQuery<Team>("Teams").Where(c => x * 2 == c.TeamID);
            x = 2;
            RunTest(baseline4, queryable4);

            var writer4 = TestURIWriter.CreateURIWriter<Team>(context.BaseUri.ToString(), "Teams").
                FindByKey("TeamID", 4);
            CheckURI(writer4, queryable4);

        }

        // [TestMethod]
        public void LinqFindByKeyAndNavigate()
        {
            // check uri here also since need to be using key syntax.
            ReadOnlyTestContext.ClearBaselineIncludes();

            //identity projection
            var writer = TestURIWriter.CreateURIWriter<League>(_baseUri.OriginalString, "Leagues").FindByKey("ID", 1);
            var queryable = context.CreateQuery<League>("Leagues").Where(l => l.ID == 1).Select(l => l);
            var baseline = baseLineContext.Leagues.Where(l => l.ID == 1).Select(l => l);
            RunTest(baseline, queryable);
            CheckURI(writer, queryable);

            // navigate to collection
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var writer2 = TestURIWriter.CreateURIWriter<League>(_baseUri.OriginalString, "Leagues").FindByKey("ID", 1).
                NavigateToCollection(l => l.Teams, true);
            var queryable2 = context.CreateQuery<League>("Leagues").Where(l => l.ID == 1).SelectMany(l => l.Teams);
            var baseline2 = baseLineContext.Leagues.Where(l => l.ID == 1).SelectMany(l => l.Teams);
            RunTest(baseline2, queryable2);
            CheckURI(writer2, queryable2);

            // navigate to entity
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var writer3 = TestURIWriter.CreateURIWriter<Team>(_baseUri.OriginalString, "Teams").FindByKey("ID", 1).Navigate(t => t.HomeStadium);
            var queryable3 = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).Select(t => t.HomeStadium);
            var baseline3 = baseLineContext.Teams.Where(t => t.TeamID == 1).Select(t => t.HomeStadium);

            RunTest(baseline3, queryable3);
            CheckURI(writer3, queryable3);

            // navigate to value
            var writer4 = TestURIWriter.CreateURIWriter<Team>(_baseUri.OriginalString, "Teams").FindByKey("TeamID", 3).Navigate(t => t.City);
            var queryable4 = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 3).Select(t => t.City);
            var baseline4 = baseLineContext.Teams.Where(t => t.TeamID == 3).Select(t => t.City);

            RunTest(baseline4, queryable4);
            CheckURI(writer4, queryable4);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqComplexPaths()
        {
            // navigate to property on entity

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var queryable = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).Select(t => t.HomeStadium).Select(s => s.Name);

            var baseline = baseLineContext.Teams.Where(t => t.TeamID == 1).Select(t => t.HomeStadium).Select(s => s.Name);

            RunTest(baseline, queryable);

            // SelectMany - no projection

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable2 = context.CreateQuery<League>("Leagues").Where(l => l.ID == 1).SelectMany(l => l.Teams);

            var baseline2 = baseLineContext.Leagues.Where(l => l.ID == 1).SelectMany(l => l.Teams);
            RunTest(baseline2, queryable2);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // SelectMany from comprehension

            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable3 = from l in context.CreateQuery<League>("Leagues")
                             where l.ID == 1
                             from t in l.Teams
                             select t;

            var baseline3 = from l in baseLineContext.Leagues
                            where l.ID == 1
                            from t in l.Teams
                            select t;

            RunTest(baseline3, queryable3);

            // Select many from comprehension 2
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable4 = from l in context.CreateQuery<League>("Leagues")
                             from t in l.Teams
                             where l.ID == 1
                             select t;

            var baseline4 = from l in baseLineContext.Leagues
                            from t in l.Teams
                            where l.ID == 1
                            select t;

            RunTest(baseline4, queryable4);

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            var queryable5 = from l in context.CreateQuery<League>("Leagues")
                             where l.ID == 1
                             from t in l.Teams
                             where t.TeamID == 1
                             from p in t.Players
                             select p;

            var baseline5 = from l in baseLineContext.Leagues
                            where l.ID == 1
                            from t in l.Teams
                            where t.TeamID == 1
                            from p in t.Players
                            select p;

            RunTest(baseline5, queryable5);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTakeAndSkip()
        {
            //take

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            {
                var queryable = (from l in context.CreateQuery<League>("Leagues")
                                 where l.ID == 1
                                 from t in l.Teams
                                 select t).Take(1);

                var baseline = (from l in baseLineContext.Leagues
                                where l.ID == 1
                                from t in l.Teams
                                select t).Take(1);

                RunTest(baseline, queryable);
            }

            //take over root
            var baseline2 = baseLineContext.Teams.Take(3);
            var queryable2 = context.CreateQuery<Team>("Teams").Take(3);

            RunTest(baseline2, queryable2);

            // Take multiple times.
            {
                var baseline = baseLineContext.Teams.Take(2).Take(3).Take(4);
                var queryable = context.CreateQuery<Team>("Teams").Take(2).Take(3).Take(4);
                RunTest(baseline, queryable);
            }

            // Negative take.
            {
                var baseline = baseLineContext.Teams.Take(-1);
                var queryable = context.CreateQuery<Team>("Teams").Take(-1);
                RunTest(baseline, queryable);
            }

            //skip
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable3 = (from l in context.CreateQuery<League>("Leagues")
                              where l.ID == 0
                              from t in l.Teams
                              select t).Skip(2);

            var baseline3 = (from l in baseLineContext.Leagues
                             where l.ID == 0
                             from t in l.Teams
                             select t).Skip(2);

            RunTest(baseline3, queryable3);

            // Skip multiple times.
            {
                var baseline = baseLineContext.Teams.Skip(2).Skip(3);
                var queryable = context.CreateQuery<Team>("Teams").Skip(2).Skip(3);
                RunTest(baseline, queryable);
            }

            // Negative skip.
            {
                var baseline = baseLineContext.Teams.Skip(-1);
                var queryable = context.CreateQuery<Team>("Teams").Skip(-1);
                RunTest(baseline, queryable);
            }

            //skip over root
            var baseline4 = baseLineContext.Teams.Skip(3);
            var queryable4 = context.CreateQuery<Team>("Teams").Skip(3);

            RunTest(baseline4, queryable4);


            //combo
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable5 = (from l in context.CreateQuery<League>("Leagues")
                              where l.ID == 1
                              from t in l.Teams
                              select t
                              ).Skip(1).Take(1);

            var baseline5 = (from l in baseLineContext.Leagues
                             where l.ID == 1
                             from t in l.Teams
                             select t
                             ).Skip(1).Take(1);

            RunTest(baseline5, queryable5);

            // Interleave take in skip.
            {
                var baseline = baseLineContext.Teams.Skip(1).Take(5).Skip(1);
                var queryable = context.CreateQuery<Team>("Teams").Skip(1).Take(5).Skip(1);
                Exception exception = System.Data.Test.Astoria.TestUtil.RunCatching(delegate { RunTest(baseline, queryable); });
                System.Data.Test.Astoria.TestUtil.AssertExceptionExpected(exception, true);
                Assert.IsInstanceOfType(exception, typeof(NotSupportedException));
            }

            // Interleave skip in take.
            {
                var baseline = baseLineContext.Teams.Take(5).Skip(1).Take(1);
                var queryable = context.CreateQuery<Team>("Teams").Take(5).Skip(1).Take(1);
                Exception exception = System.Data.Test.Astoria.TestUtil.RunCatching(delegate { RunTest(baseline, queryable); });
                System.Data.Test.Astoria.TestUtil.AssertExceptionExpected(exception, true);
                Assert.IsInstanceOfType(exception, typeof(NotSupportedException));
            }

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqOrderBy()
        {
            //Simple OrderBy

            var queryable = from t in context.CreateQuery<Team>("Teams")
                            orderby t.TeamName
                            select t;

            var baseline = from t in baseLineContext.Teams
                           orderby t.TeamName
                           select t;

            RunTest(baseline, queryable);

            //Order By after navigation

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable2 = context.CreateQuery<League>("Leagues").Where(l => l.ID == 1)
                .SelectMany(l => l.Teams).
                OrderBy(l => l.TeamName).Select(l => l);

            var baseline2 = from l in baseLineContext.Leagues
                            where l.ID == 1
                            from t in l.Teams
                            orderby t.TeamName
                            select t;

            RunTest(baseline2, queryable2);

            ReadOnlyTestContext.ClearBaselineIncludes();

            //simple OrderBy descending
            var queryable3 = from t in context.CreateQuery<Team>("Teams")
                             orderby t.TeamName descending
                             select t;

            var baseline3 = from t in baseLineContext.Teams
                            orderby t.TeamName descending
                            select t;

            RunTest(baseline3, queryable3);
        }

        // [TestMethod]
        public void LinqThenBy()
        {
            //Simple ThenBy

            var queryable = from t in context.CreateQuery<Team>("Teams")
                            orderby t.City, t.TeamName
                            select t;

            var baseline = from t in baseLineContext.Teams
                           orderby t.City, t.TeamName
                           select t;

            RunTest(baseline, queryable);

            // mix ascending and descending

            var queryable2 = from t in context.CreateQuery<Team>("Teams")
                             orderby t.City ascending, t.TeamName descending
                             select t;

            var baseline2 = from t in baseLineContext.Teams
                            orderby t.City ascending, t.TeamName descending
                            select t;

            RunTest(baseline2, queryable2);

            // pathological

            var queryable3 = from t in context.CreateQuery<Team>("Teams")
                             orderby t.City ascending, t.TeamName descending,
                                     t.City descending, t.TeamName ascending
                             select t;

            var baseline3 = from t in baseLineContext.Teams
                            orderby t.City ascending, t.TeamName descending,
                                    t.City descending, t.TeamName ascending
                            select t;

            RunTest(baseline3, queryable3);
        }

        // [TestMethod]
        public void LinqSelectMany()
        {
            // Queryable.SelectMany comes in these forms:
            // - SelectMany(IQ<TSource>source, Exp<Func<TSource, IEnum<TResult>>> selector)
            // - SelectMany(IQ<TSource>source, Exp<Func<TSource, IEnum<TCollection>>> collection, Exp<Func<TSource, TCollection, TResult>> selector)
            //
            // And the "int position" versions, which aren't supported (see LinqSelectManyWithPosition unit test):
            // - SelectMany(IQ<TSource>source, Exp<Func<TSource, int, IEnum<TResult>>> selector)
            // - SelectMany(IQ<TSource>source, Exp<Func<TSource, int, IEnum<TCollection>>> collection, Exp<Func<TSource, TCollection, TResult>> selector)
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Player), "Team");

            {
                Trace.WriteLine("Basic success case");
                var q = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).SelectMany(t => t.Players);
                var b = baseLineContext.Teams.Where(t => t.TeamID == 1).SelectMany(t => t.Players);
                RunTest(b, q);
            }

            {
                Trace.WriteLine("Basic success case with collector (not considered a projection)");
                var q = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => p);
                var b = baseLineContext.Teams.Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => p);
                RunTest(b, q);
            }

            {
                Trace.WriteLine("Basic success case with collector (as projection)");
                var q = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { p });
                var b = baseLineContext.Teams.Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { p });
                RunTest(b, q);
            }

            {
                Trace.WriteLine("Transparent identifier in leaf of projection (not supported, projection doesn't implement SelectMany-style projection).");
                var q = context.CreateQuery<Team>("Teams")
                    .Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { t, p });
                VerifyNotSupportedQuery(q, null);
            }

            {
                Trace.WriteLine("Transparent identifier (not supported, trailed with .Where() but both members still in scope for projection).");
                var q = context.CreateQuery<Team>("Teams")
                    .Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { t, p })
                    .Where(anon => anon.p.ID == 1001);
                VerifyNotSupportedQuery(q, null);
            }

            {
                Trace.WriteLine("Transparent identifier (success - drops the unused identifier)");
                var q = context.CreateQuery<Team>("Teams")
                    .Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { t, p })
                    .Where(anon => anon.p.ID == 1001).Select(anon => new { anon.p.Team });
                var b = baseLineContext.Teams
                    .Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { t, p })
                    .Where(anon => anon.p.ID == 1001).Select(anon => new { anon.p.Team });
                Trace.WriteLine(q.ToString());
                RunTest(b, q);
            }

#if FALSE
            // This *could* work, except that the InputBinder isn't recognizing
            // the t.Team.TeamID access as a transparent identifier, so the
            // wrong tree is getting formed (leading to an ArgumentException
            // in BuildKeyPredicateFilter because the property is declared
            // on the Team but it's being applied to an anon<Team> type).
            {
                Trace.WriteLine("Transparent identifier (success - drops the unused identifier)");
                var q = context.CreateQuery<Team>("Teams")
                    .Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { t, p })
                    .Where(anon => anon.p.ID == 1001).Select(anon => new { anon.p.Team })
                    .Where(t => t.Team.TeamID == 1);
                var b = baseLineContext.Teams
                    .Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { t, p })
                    .Where(anon => anon.p.ID == 1001).Select(anon => new { anon.p.Team })
                    .Where(t => t.Team.TeamID == 1);
                RunTest(b, q);
            }
#endif
        }

        // [TestMethod]
        public void LinqSelectManyWithPosition()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            {
                Trace.WriteLine("Basic success case");
                var q = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).SelectMany(t => t.Players);
                var b = baseLineContext.Teams.Where(t => t.TeamID == 1).SelectMany(t => t.Players);
                RunTest(b, q);
            }

            {
                Trace.WriteLine("Select many with position");
                var q = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).SelectMany((t, n) => t.Players);
                var b = baseLineContext.Teams.Where(t => t.TeamID == 1).SelectMany((t, n) => t.Players);
                try
                {
                    RunTest(b, q);
                    Assert.Fail("Failure expected");
                }
                catch (NotSupportedException e)
                {
                    System.Data.Test.Astoria.TestUtil.AssertContains(e.ToString(), "System.NotSupportedException: The method 'SelectMany' is not supported.");
                }
            }

            // It's a limitation that primitive properties can't be projected directly,
            // but instead need to be projected into a structured (member-init / new'ed) type.
            {
                Trace.WriteLine("Basic success case with collector");
                var q = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { fn = p.FirstName });
                var b = baseLineContext.Teams.Where(t => t.TeamID == 1).SelectMany(t => t.Players, (t, p) => new { fn = p.FirstName });
                RunTest(b, q);
            }

            {
                Trace.WriteLine("Select many with position with collector");
                var q = context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1).SelectMany((t, n) => t.Players, (t, p) => new { fn = p.FirstName });
                var b = baseLineContext.Teams.Where(t => t.TeamID == 1).SelectMany((t, n) => t.Players, (t, p) => new { fn = p.FirstName });
                try
                {
                    RunTest(b, q);
                    Assert.Fail("Failure expected");
                }
                catch (NotSupportedException e)
                {
                    System.Data.Test.Astoria.TestUtil.AssertContains(e.ToString(), "System.NotSupportedException: The method 'SelectMany' is not supported.");
                }
            }
        }

        // [TestMethod]
        public void LinqSimpleFilter()
        {
            // simple filter
            var queryable = from t in context.CreateQuery<Team>("Teams")
                            where t.City == "New York"
                            select t;

            var baseline = from t in baseLineContext.Teams
                           where t.City == "New York"
                           select t;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable2 = context.CreateQuery<League>("Leagues").
                             Where(l => l.ID == 1).
                             SelectMany(l => l.Teams).
                             Where(t => t.City == "Chicago");

            var baseline2 = baseLineContext.Leagues.
                            Where(l => l.ID == 1).
                            SelectMany(l => l.Teams).
                            Where(t => t.City == "Chicago");

            RunTest(baseline2, queryable2);

            //multiple wheres
            var queryable3 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where s.Capacity > 50000
                             where s.City == "Seattle"
                             select s;


            var baseline3 = from s in baseLineContext.Stadiums
                            where s.Capacity > 50000
                            where s.City == "Seattle"
                            select s;

            RunTest(baseline3, queryable3);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }


        // [TestMethod]
        public void LinqBinaryExpressionsInFilter()
        {
            //AndAlso
            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            where s.Capacity < 50000
                            && s.City == "Seattle"
                            select s;


            var baseline = from s in baseLineContext.Stadiums
                           where s.Capacity < 50000
                           && s.City == "Seattle"
                           select s;

            RunTest(baseline, queryable);

            //OrElse
            var queryable2 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where s.Capacity < 50000
                             || s.City == "Seattle"
                             select s;


            var baseline2 = from s in baseLineContext.Stadiums
                            where s.Capacity < 50000
                            || s.City == "Seattle"
                            select s;

            RunTest(baseline2, queryable2);

            //Equal
            var queryable3 = from t in context.CreateQuery<Team>("Teams")
                             where t.City == "Seattle"
                             select t;


            var baseline3 = from t in baseLineContext.Teams
                            where t.City == "Seattle"
                            select t;

            RunTest(baseline3, queryable3);

            //NotEqual
            var queryable4 = from t in context.CreateQuery<Team>("Teams")
                             where t.City != "Seattle"
                             select t;


            var baseline4 = from t in baseLineContext.Teams
                            where t.City != "Seattle"
                            select t;

            RunTest(baseline4, queryable4);

            //LessThan
            var queryable5 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where s.Capacity < 50000
                             select s;


            var baseline5 = from s in baseLineContext.Stadiums
                            where s.Capacity < 50000
                            select s;

            RunTest(baseline5, queryable5);

            //LessThanOrEqual
            var queryable6 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where s.Capacity <= 50000
                             select s;


            var baseline6 = from s in baseLineContext.Stadiums
                            where s.Capacity <= 50000
                            select s;

            RunTest(baseline6, queryable6);

            //GreaterThan
            var queryable7 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where s.Capacity > 50000
                             select s;


            var baseline7 = from s in baseLineContext.Stadiums
                            where s.Capacity > 50000
                            select s;

            RunTest(baseline7, queryable7);

            //GreaterThanOrEqual
            var queryable8 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where s.Capacity >= 50000
                             select s;


            var baseline8 = from s in baseLineContext.Stadiums
                            where s.Capacity >= 50000
                            select s;

            RunTest(baseline8, queryable8);

            //Add
            var queryable9 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where s.Capacity + 20000 > 50000
                             select s;


            var baseline9 = from s in baseLineContext.Stadiums
                            where s.Capacity + 20000 > 50000
                            select s;

            RunTest(baseline9, queryable9);

            //Subtract
            var queryable10 = from s in context.CreateQuery<Stadium>("Stadiums")
                              where s.Capacity - 20000 > 30000
                              select s;


            var baseline10 = from s in baseLineContext.Stadiums
                             where s.Capacity - 20000 > 30000
                             select s;

            RunTest(baseline10, queryable10);

            //Multiply
            var queryable11 = from s in context.CreateQuery<Stadium>("Stadiums")
                              where s.Capacity * 2 > 100000
                              select s;


            var baseline11 = from s in baseLineContext.Stadiums
                             where s.Capacity * 2 > 100000
                             select s;

            RunTest(baseline11, queryable11);

            //Divide
            var queryable12 = from s in context.CreateQuery<Stadium>("Stadiums")
                              where s.Capacity / 2 < 20000
                              select s;


            var baseline12 = from s in baseLineContext.Stadiums
                             where s.Capacity / 2 < 20000
                             select s;

            RunTest(baseline12, queryable12);

            //Modulo
            var queryable13 = from s in context.CreateQuery<Stadium>("Stadiums")
                              where s.Capacity % 2 == 0
                              select s;


            var baseline13 = from s in baseLineContext.Stadiums
                             where s.Capacity % 2 == 0
                             select s;

            RunTest(baseline13, queryable13);

            //AddChecked
            var queryable14 = from s in context.CreateQuery<Stadium>("Stadiums")
                              where checked(s.Capacity + 20000) > 70000
                              select s;


            var baseline14 = from s in baseLineContext.Stadiums
                             where checked(s.Capacity + 20000) > 70000
                             select s;

            RunTest(baseline14, queryable14);

            //SubtractChecked
            var queryable15 = from s in context.CreateQuery<Stadium>("Stadiums")
                              where checked(s.Capacity - 20000) > 30000
                              select s;


            var baseline15 = from s in baseLineContext.Stadiums
                             where checked(s.Capacity - 20000) > 30000
                             select s;

            RunTest(baseline15, queryable15);

            //Multiply
            var queryable16 = from s in context.CreateQuery<Stadium>("Stadiums")
                              where checked(s.Capacity * 2) > 100000
                              select s;


            var baseline16 = from s in baseLineContext.Stadiums
                             where checked(s.Capacity * 2) > 100000
                             select s;

            RunTest(baseline16, queryable16);
        }

        // [TestMethod]
        public void LinqBinaryExpressionsInOrderBy()
        {
            //AndAlso
            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            orderby s.Capacity < 50000
                            && s.City == "Seattle"
                            select s;


            var baseline = from s in baseLineContext.Stadiums
                           orderby s.Capacity < 50000
                           && s.City == "Seattle"
                           select s;

            RunTest(baseline, queryable);

            //OrElse
            var queryable2 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby s.Capacity < 50000
                             || s.City == "Seattle"
                             select s;


            var baseline2 = from s in baseLineContext.Stadiums
                            orderby s.Capacity < 50000
                            || s.City == "Seattle"
                            select s;

            RunTest(baseline2, queryable2);

            //Equal
            var queryable3 = from t in context.CreateQuery<Team>("Teams")
                             orderby t.City == "Seattle"
                             select t;


            var baseline3 = from t in baseLineContext.Teams
                            orderby t.City == "Seattle"
                            select t;

            RunTest(baseline3, queryable3);

            //NotEqual
            var queryable4 = from t in context.CreateQuery<Team>("Teams")
                             orderby t.City != "Seattle"
                             select t;


            var baseline4 = from t in baseLineContext.Teams
                            orderby t.City != "Seattle"
                            select t;

            RunTest(baseline4, queryable4);

            //LessThan
            var queryable5 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby s.Capacity < 50000
                             select s;


            var baseline5 = from s in baseLineContext.Stadiums
                            orderby s.Capacity < 50000
                            select s;

            RunTest(baseline5, queryable5);

            //LessThanOrEqual
            var queryable6 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby s.Capacity <= 50000
                             select s;


            var baseline6 = from s in baseLineContext.Stadiums
                            orderby s.Capacity <= 50000
                            select s;

            RunTest(baseline6, queryable6);

            //GreaterThan
            var queryable7 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby s.Capacity > 50000
                             select s;


            var baseline7 = from s in baseLineContext.Stadiums
                            orderby s.Capacity > 50000
                            select s;

            RunTest(baseline7, queryable7);

            //GreaterThanOrEqual
            var queryable8 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby s.Capacity >= 50000
                             select s;


            var baseline8 = from s in baseLineContext.Stadiums
                            orderby s.Capacity >= 50000
                            select s;

            RunTest(baseline8, queryable8);

            //Add
            var queryable9 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby s.Capacity + 20000
                             select s;


            var baseline9 = from s in baseLineContext.Stadiums
                            orderby s.Capacity + 20000
                            select s;

            RunTest(baseline9, queryable9);

            //Subtract
            var queryable10 = from s in context.CreateQuery<Stadium>("Stadiums")
                              orderby s.Capacity - 20000
                              select s;


            var baseline10 = from s in baseLineContext.Stadiums
                             orderby s.Capacity - 20000
                             select s;

            RunTest(baseline10, queryable10);

            //Multiply
            var queryable11 = from s in context.CreateQuery<Stadium>("Stadiums")
                              orderby s.Capacity * 2
                              select s;


            var baseline11 = from s in baseLineContext.Stadiums
                             orderby s.Capacity * 2
                             select s;

            RunTest(baseline11, queryable11);

            //Divide
            var queryable12 = from s in context.CreateQuery<Stadium>("Stadiums")
                              orderby s.Capacity / 2
                              select s;


            var baseline12 = from s in baseLineContext.Stadiums
                             orderby s.Capacity / 2
                             select s;

            RunTest(baseline12, queryable12);

            //Modulo
            var queryable13 = from s in context.CreateQuery<Stadium>("Stadiums")
                              orderby s.Capacity % 1000 == 0
                              select s;


            var baseline13 = from s in baseLineContext.Stadiums
                             orderby s.Capacity % 1000 == 0
                             select s;

            RunTest(baseline13, queryable13);

            //AddChecked
            var queryable14 = from s in context.CreateQuery<Stadium>("Stadiums")
                              orderby checked(s.Capacity + 20000)
                              select s;


            var baseline14 = from s in baseLineContext.Stadiums
                             orderby checked(s.Capacity + 20000)
                             select s;

            RunTest(baseline14, queryable14);

            //SubtractChecked
            var queryable15 = from s in context.CreateQuery<Stadium>("Stadiums")
                              orderby checked(s.Capacity - 20000)
                              select s;


            var baseline15 = from s in baseLineContext.Stadiums
                             orderby checked(s.Capacity - 20000)
                             select s;

            RunTest(baseline15, queryable15);

            //Multiply
            var queryable16 = from s in context.CreateQuery<Stadium>("Stadiums")
                              orderby checked(s.Capacity * 2)
                              select s;


            var baseline16 = from s in baseLineContext.Stadiums
                             orderby s.Capacity * 2
                             select s;

            RunTest(baseline16, queryable16);
        }

        // [TestMethod]
        public void LinqUnaryExpressionsInFilter()
        {
            //Not
            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            where !(s.City == "Seattle")
                            select s;


            var baseline = from s in baseLineContext.Stadiums
                           where !(s.City == "Seattle")
                           select s;

            RunTest(baseline, queryable);

            //Convert
            var queryable2 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where (long)s.ID > 0L
                             select s;


            var baseline2 = from s in baseLineContext.Stadiums
                            where (long)s.ID > 0L
                            select s;

            RunTest(baseline2, queryable2);

            //ConvertChecked
            var queryable3 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where checked((long)s.ID > 0L)
                             select s;


            var baseline3 = from s in baseLineContext.Stadiums
                            where checked((long)s.ID) > 0L
                            select s;

            RunTest(baseline3, queryable3);

            //Negate

            var queryable4 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where -s.Capacity < -50000
                             select s;


            var baseline4 = from s in baseLineContext.Stadiums
                            where -s.Capacity < -50000
                            select s;

            RunTest(baseline4, queryable4);

            //NegateChecked

            var queryable5 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where checked(-s.Capacity) < -50000
                             select s;


            var baseline5 = from s in baseLineContext.Stadiums
                            where checked(-s.Capacity) < -50000
                            select s;

            RunTest(baseline5, queryable5);

            //UnaryPlus
            var queryable6 = from s in context.CreateQuery<Stadium>("Stadiums")
                             where +s.Capacity < +50000
                             select s;


            var baseline6 = from s in baseLineContext.Stadiums
                            where +s.Capacity < +50000
                            select s;

            RunTest(baseline6, queryable6);
        }

        // [TestMethod]
        public void LinqUnaryExpressionsInOrderBy()
        {
            //Not
            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            orderby !(s.City == "Seattle")
                            select s;


            var baseline = from s in baseLineContext.Stadiums
                           orderby !(s.City == "Seattle")
                           select s;

            RunTest(baseline, queryable);

            // Convert 
            var queryable2 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby (long)s.ID
                             select s;


            var baseline2 = from s in baseLineContext.Stadiums
                            orderby (long)s.ID
                            select s;

            RunTest(baseline2, queryable2);

            //ConvertChecked
            var queryable3 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby checked((long)s.ID)
                             select s;


            var baseline3 = from s in baseLineContext.Stadiums
                            orderby checked((long)s.ID)
                            select s;

            RunTest(baseline3, queryable3);

            //Negate
            var queryable4 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby -s.Capacity
                             select s;


            var baseline4 = from s in baseLineContext.Stadiums
                            orderby -s.Capacity
                            select s;

            RunTest(baseline4, queryable4);

            //NegateChecked

            var queryable5 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby checked(-s.Capacity)
                             select s;


            var baseline5 = from s in baseLineContext.Stadiums
                            orderby checked(-s.Capacity)
                            select s;

            RunTest(baseline5, queryable5);

            //UnaryPlus
            var queryable6 = from s in context.CreateQuery<Stadium>("Stadiums")
                             orderby +s.Capacity
                             select s;


            var baseline6 = from s in baseLineContext.Stadiums
                            orderby +s.Capacity
                            select s;

            RunTest(baseline6, queryable6);
        }

        [TestMethod]
        public void LinqKeyPredicate_Negative_1()
        {
            var queryable = from le in context.CreateQuery<League>("Leagues")
                            where (le.ID == 1 && le.ID == 2)
                            select le;

            bool passed = false;

            try
            {
                queryable.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Multiple key predicates cannot be specified for the same entity set.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");
        }

        [TestMethod]
        public void LinqKeyPredicate_Negative_2()
        {
            var queryable = (from le in context.CreateQuery<League>("Leagues")
                             where (le.ID == 1)
                             select le).Where(le => le.ID == 2);

            bool passed = false;

            try
            {
                queryable.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Multiple key predicates cannot be specified for the same entity set.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");
        }

        [TestMethod]
        public void LinqKeyPredicate_Negative_3()
        {
            var queryable = from le in context.CreateQuery<League>("Leagues")
                            where le != null
                            select le;

            bool passed = false;

            try
            {
                queryable.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "The expression ([10008] != null) is not supported.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");
        }

        // ALinq - SelectMany navigation paths where no key predicate is present should be detected earlier
        [TestMethod]
        public void LinqKeyPredicate_Negative_4()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            var queryable = from t in context.CreateQuery<Team>("Teams")
                            select t.HomeStadium;

            VerifyNotSupportedQuery(queryable, DataServicesClientResourceUtil.GetString("ALinq_CantNavigateWithoutKeyPredicate"));
        }

        [TestMethod]
        public void LinqKeyPredicate_Negative_5()
        {
            var queryable = from le in context.CreateQuery<League>("Leagues")
                            from t in le.Teams
                            select t.HomeStadium;

            bool passed = false;

            try
            {
                queryable.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "The method 'SelectMany' is not supported.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");
        }

        // ALinq: with multiple predicates and the key in the predicate fails
        // [TestMethod]
        public void LinqKeyPredicate_Positive()
        {
            // Within the same clause:
            // Non-filter query option before key predicate
            var queryable = from t in context.CreateQuery<Team>("Teams")
                            orderby t.TeamID
                            where t.TeamID == 1
                            select t;

            var baseline = from t in baseLineContext.Teams
                           orderby t.TeamID
                           where t.TeamID == 1
                           select t;
            RunTest(baseline, queryable);

            // Filter before key predicate
            queryable = from t in context.CreateQuery<Team>("Teams")
                        where t.TeamName != "" && t.TeamID == 1
                        select t;

            baseline = from t in baseLineContext.Teams
                       where t.TeamName != "" && t.TeamID == 1
                       select t;
            RunTest(baseline, queryable);

            // Key predicate before filter
            queryable = from t in context.CreateQuery<Team>("Teams")
                        where t.TeamID == 1 && t.TeamName != ""
                        select t;

            baseline = from t in baseLineContext.Teams
                       where t.TeamID == 1 && t.TeamName != ""
                       select t;
            RunTest(baseline, queryable);

            // In subsequent clauses:
            // Non-filter query option before key predicate
            queryable = (from t in context.CreateQuery<Team>("Teams")
                         orderby t.TeamID
                         select t).Where(t => t.TeamID == 1);

            baseline = (from t in baseLineContext.Teams
                        orderby t.TeamID
                        select t).Where(t => t.TeamID == 1);
            RunTest(baseline, queryable);

            // Filter before key predicate
            queryable = (from t in context.CreateQuery<Team>("Teams")
                         where t.TeamName != ""
                         select t).Where(t => t.TeamID == 1);

            baseline = (from t in baseLineContext.Teams
                        where t.TeamName != ""
                        select t).Where(t => t.TeamID == 1);
            RunTest(baseline, queryable);

            // Key predicate before filter
            queryable = (from t in context.CreateQuery<Team>("Teams")
                         where t.TeamID == 1
                         select t).Where(t => t.TeamName != "");

            baseline = (from t in baseLineContext.Teams
                        where t.TeamID == 1
                        select t).Where(t => t.TeamName != "");
            RunTest(baseline, queryable);

            // Multiple key predicates separated by a filter in the same clause as the first key predicate
            queryable = (from t in context.CreateQuery<Team>("Teams")
                         where t.TeamID == 1 && t.TeamName != ""
                         select t).Where(t => t.TeamID == 2);

            baseline = (from t in baseLineContext.Teams
                        where t.TeamID == 1 && t.TeamName != ""
                        select t).Where(t => t.TeamID == 2);
            RunTest(baseline, queryable);

            // Multiple key predicates separated by a filter in an intervening clause
            queryable = (from t in context.CreateQuery<Team>("Teams")
                         where t.TeamID == 1
                         select t)
                        .Where(t => t.TeamName != "")
                        .Where(t => t.TeamID == 2);

            baseline = (from t in baseLineContext.Teams
                        where t.TeamID == 1 && t.TeamName != ""
                        select t)
                        .Where(t => t.TeamName != "")
                        .Where(t => t.TeamID == 2);
            RunTest(baseline, queryable);
        }

        // [TestMethod]
        public void LinqKeyPredicate_NonFilterQueryOption()
        {
            // Applying key predicate before non-filter query option - converts key predicate to filter query option
            LinqTestCase[] testCases =
            {
                new LinqTestCase()
                {
                    Query = (from t in context.CreateQuery<Team>("Teams")
                             where t.TeamID == 1
                             select t).OrderBy(t => t.TeamID).Skip(0).Take(1),
                    ExpectedUri = "?$filter=TeamID eq 1&$orderby=TeamID&$skip=0&$top=1",
                    ExpectedResult =  (from t in baseLineContext.Teams
                                       where t.TeamID == 1
                                       select t).OrderBy(t => t.TeamID).Skip(0).Take(1)
                },
                new LinqTestCase()
                {
                    Query = (from t in context.CreateQuery<Team>("Teams")
                             where t.TeamID == 1
                             select t).OrderByDescending(t => t.TeamID).Skip(0).Take(1),
                    ExpectedUri = "?$filter=TeamID eq 1&$orderby=TeamID desc&$skip=0&$top=1",
                    ExpectedResult =  (from t in baseLineContext.Teams
                                       where t.TeamID == 1
                                       select t).OrderByDescending(t => t.TeamID).Skip(0).Take(1)
                }
            };

            TestLinqQueries(testCases);
        }

        // [TestMethod]
        public void LinqKeyPredicate_Order()
        {
            // ALinq: order of the predicates should not matter for a composite key
            {
                var queryable = (from t in context.CreateQuery<MultipleKeyType>("MoreVar1")
                                 where t.Key1 == 1 && t.Key2 == "23" && t.Key3 == 37
                                 select t.Data1);
                var baseline = (from t in baseLineContext.MoreVar1
                                where t.Key1 == 1 && t.Key2 == "23" && t.Key3 == 37
                                select t.Data1);
                RunTest(baseline, queryable);
            }

            {
                var queryable = (from t in context.CreateQuery<MultipleKeyType>("MoreVar1")
                                 where t.Key2 == "23" && t.Key1 == 1 && t.Key3 == 37
                                 select t.Data1);
                var baseline = (from t in baseLineContext.MoreVar1
                                where t.Key2 == "23" && t.Key1 == 1 && t.Key3 == 37
                                select t.Data1);
                RunTest(baseline, queryable);
            }
        }

        [TestMethod]
        public void LinqTransparentIdentifier_Negative_1()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            // Not supported because no key predicate is applied to Leagues
            var queryable = from l in context.CreateQuery<League>("Leagues")
                            from t in l.Teams
                            select t;

            bool passed = false;

            try
            {
                queryable.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == DataServicesClientResourceUtil.GetString("ALinq_CantNavigateWithoutKeyPredicate"))
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_DirectProjection1()
        {
            {
                Trace.WriteLine("Projecting from identifier");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

                int leagueId = baseLineContext.Leagues.First(le => le.Teams.Count > 0).ID;

                var queryable = from l in context.CreateQuery<League>("Leagues")
                                where l.ID == leagueId
                                from t in l.Teams
                                select t;

                var baseline = from l in baseLineContext.Leagues
                               where l.ID == leagueId
                               from t in l.Teams
                               select t;

                RunTest(baseline, queryable);
            }

            {
                Trace.WriteLine("Projecting from identifier with initializer");
                var queryable = from c in context.CreateQuery<northwindClient.Customers>("Customers")
                                where c.CustomerID == "ALFKI"
                                from o in c.Orders
                                where o.OrderID == 10643
                                from od in o.Order_Details
                                where (od.OrderID == 10643 && od.ProductID == 28)
                                select new { OrderID = od.OrderID, ProductID = od.ProductID, Orders = od.Orders, Products = od.Products };

                Trace.WriteLine(queryable.Expression.ToString());
                string zz = queryable.ToString();
                System.Data.Test.Astoria.TestUtil.AssertContains(zz, "/Customers('ALFKI')/Orders(10643)/Order_Details(OrderID=10643,ProductID=28)?$expand=Orders,Products&$select=OrderID,ProductID");
            }

            {
                Trace.WriteLine("Projecting from identifier with a single property");
                var queryable = from l in context.CreateQuery<League>("Leagues")
                                where l.ID == 1
                                from t in l.Teams
                                where t.TeamID == 1
                                from p in t.Players
                                select new { name = p.FirstName };

                string zz = queryable.ToString();
                System.Data.Test.Astoria.TestUtil.AssertContains(zz, "/Leagues(1)/Teams(1)/Players?$select=FirstName");
            }

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_DirectProjection2()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            League league = baseLineContext.Leagues.First(le => le.Teams.Count > 0);
            int teamId = league.Teams.First(t => t.Players.Count > 0).TeamID;

            var queryable = from l in context.CreateQuery<League>("Leagues")
                            where l.ID == league.ID
                            from t in l.Teams
                            where t.TeamID == teamId
                            from p in t.Players
                            select p;

            var baseline = from l in baseLineContext.Leagues
                           where l.ID == league.ID
                           from t in l.Teams
                           where t.TeamID == teamId
                           from p in t.Players
                           select p;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_DirectProjection3()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            League league = baseLineContext.Leagues.First(le => le.Teams.Count > 0);
            int teamId = league.Teams.First().TeamID;

            var queryable = from l in context.CreateQuery<League>("Leagues")
                            from t in l.Teams
                            from p in t.Players
                            where l.ID == league.ID && t.TeamID == teamId
                            select p;

            var baseline = from l in context.CreateQuery<League>("Leagues")
                           from t in l.Teams
                           from p in t.Players
                           where l.ID == league.ID && t.TeamID == teamId
                           select p;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_OrderBy1()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            League league = baseLineContext.Leagues.First(le => le.Teams.Count > 0);
            int teamId = league.Teams.First().TeamID;

            var queryable = from l in context.CreateQuery<League>("Leagues")
                            from t in l.Teams
                            from p in t.Players
                            orderby p.Lastname
                            where l.ID == league.ID && t.TeamID == teamId
                            select p;

            var baseline = from l in context.CreateQuery<League>("Leagues")
                           from t in l.Teams
                           from p in t.Players
                           orderby p.Lastname
                           where l.ID == league.ID && t.TeamID == teamId
                           select p;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_NestedSelect_Collection()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "FormerNames");

            int teamId = baseLineContext.Teams.First(t => t.HomeStadium != null && t.HomeStadium.FormerNames != null && t.HomeStadium.FormerNames.Count > 0).TeamID;

            var queryable =
                from s in
                    (
                        from t in context.CreateQuery<Team>("Teams")
                        where t.TeamID == teamId
                        select t.HomeStadium
                    )
                from n in s.FormerNames
                select n;

            var baseline =
                from s in
                    (
                        from t in baseLineContext.Teams
                        where t.TeamID == teamId
                        select t.HomeStadium
                    )
                from n in s.FormerNames
                select n;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        [TestMethod]
        public void LinqTransparentIdentifier_NestedSelect_Navigation_1()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "FormerNames");

            int stadiumId = baseLineContext.Stadiums.First(s => s.FormerNames != null && s.FormerNames.Count() > 0).ID;
            int teamId = baseLineContext.Teams.First(t => t.HomeStadium != null && t.HomeStadium.ID == stadiumId).TeamID;
            int leagueId = baseLineContext.Leagues.First(l => l.Teams != null && l.Teams.Where(t => t.TeamID == teamId).Count() > 0).ID;

            // Not supported: can be rewritten as follows:
            //var queryable =
            //    from s in
            //        (
            //            from l in context.CreateQuery<League>("Leagues")
            //            where l.ID == leagueId
            //            from t in l.Teams
            //            where t.TeamID == teamId
            //            select t.HomeStadium
            //        )
            //    from n in s.FormerNames
            //    select n;

            var queryable =
                from s in
                    (
                        from l in context.CreateQuery<League>("Leagues")
                        where l.ID == leagueId
                        from x in (l.Teams.Where(t => t.TeamID == teamId).Select(t => t.HomeStadium))
                        select x
                    )
                from n in s.FormerNames
                select n;

            bool passed = false;

            try
            {
                queryable.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "The method 'SelectMany' is not supported.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_NestedSelect_Navigation_2()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "FormerNames");

            int stadiumId = baseLineContext.Stadiums.First(s => s.FormerNames != null && s.FormerNames.Count > 0).ID;
            int teamId = baseLineContext.Teams.First(t => t.HomeStadium != null && t.HomeStadium.ID == stadiumId).TeamID;
            int leagueId = baseLineContext.Leagues.First(l => l.Teams != null && l.Teams.Where(t => t.TeamID == teamId).Count() > 0).ID;

            var queryable =
                from s in
                    (
                        from l in context.CreateQuery<League>("Leagues")
                        where l.ID == leagueId
                        from t in l.Teams
                        where t.TeamID == teamId
                        select t.HomeStadium
                    )
                from n in s.FormerNames
                select n;

            var baseline =
                from s in
                    (
                        from l in baseLineContext.Leagues
                        where l.ID == leagueId
                        from t in l.Teams
                        where t.TeamID == teamId
                        select t.HomeStadium
                    )
                from n in s.FormerNames
                select n;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_NestedSelect_Navigation_3()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "FormerNames");

            int stadiumId = baseLineContext.Stadiums.First(s => s.FormerNames != null && s.FormerNames.Count() > 1).ID;
            int teamId = baseLineContext.Teams.First(t => t.HomeStadium != null && t.HomeStadium.ID == stadiumId).TeamID;
            int leagueId = baseLineContext.Leagues.First(l => l.Teams != null && l.Teams.Where(t => t.TeamID == teamId).Count() > 0).ID;

            var queryable =
                from t in
                    (
                        from l in context.CreateQuery<League>("Leagues")
                        where l.ID == leagueId
                        from t in l.Teams
                        where t.TeamID == teamId
                        select t
                    )
                from n in t.HomeStadium.FormerNames
                select n;

            var baseline =
                from t in
                    (
                        from l in baseLineContext.Leagues
                        where l.ID == leagueId
                        from t in l.Teams
                        where t.TeamID == teamId
                        select t
                    )
                from n in t.HomeStadium.FormerNames
                select n;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_NestedSelect_Navigation_4()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            League league = baseLineContext.Leagues.First(le => le.Teams != null && le.Teams.Count > 0);
            int teamId = league.Teams.First(t => t.HomeStadium != null).TeamID;

            var queryable =
                from l in context.CreateQuery<League>("Leagues")
                where l.ID == league.ID
                from t in l.Teams
                where t.TeamID == teamId
                select t.HomeStadium;


            var baseline =
                from l in baseLineContext.Leagues
                where l.ID == league.ID
                from t in l.Teams
                where t.TeamID == teamId
                select t.HomeStadium;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_NestedSelect_Navigation_5()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");

            int stadiumId = baseLineContext.Stadiums.First(s => s.Sponsor != null).ID;
            int teamId = baseLineContext.Teams.First(t => t.HomeStadium != null && t.HomeStadium.ID == stadiumId).TeamID;
            int leagueId = baseLineContext.Leagues.First(l => l.Teams != null && l.Teams.Where(t => t.TeamID == teamId).Count() > 0).ID;

            var queryable =
                from l in context.CreateQuery<League>("Leagues")
                where l.ID == leagueId
                from t in l.Teams
                where t.TeamID == teamId
                select t.HomeStadium.Sponsor;


            var baseline =
                from l in baseLineContext.Leagues
                where l.ID == leagueId
                from t in l.Teams
                where t.TeamID == teamId
                select t.HomeStadium.Sponsor;

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_Positive1()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            League league = baseLineContext.Leagues.First(l => l.Teams != null && l.Teams.Count > 0);
            int teamId = league.Teams.First().TeamID;

            var queryable = (from l in context.CreateQuery<League>("Leagues")
                             where l.ID == league.ID
                             from t in l.Teams
                             where true
                             select t).Where(t => t.TeamID == teamId);

            var baseline = (from l in baseLineContext.Leagues
                            where l.ID == league.ID
                            from t in l.Teams
                            where true
                            select t).Where(t => t.TeamID == teamId);

            queryable = ((DataServiceQuery<Team>)queryable).Expand("Players");

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqTransparentIdentifier_Positive2()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            League league = baseLineContext.Leagues.First(l => l.Teams != null && l.Teams.Count > 0);
            int teamId = league.Teams.First().TeamID;

            var queryable = (from l in context.CreateQuery<League>("Leagues")
                             where l.ID == league.ID
                             from t in l.Teams
                             where t.TeamID == teamId
                             select t).Where(t => t.TeamName != "");

            var baseline = (from l in baseLineContext.Leagues
                            where l.ID == league.ID
                            from t in l.Teams
                            where t.TeamID == teamId
                            select t).Where(t => t.TeamName != "");

            queryable = ((DataServiceQuery<Team>)queryable).Expand("Players");

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqComplexExpressionsInQueryOptions()
        {
            // several expressions
            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            where (s.Capacity > 30000 && s.Capacity < 60000) ||
                            s.City == "Chicago" || s.ID != 1
                            orderby s.City
                            select s;


            var baseline = from s in baseLineContext.Stadiums
                           where (s.Capacity > 30000 && s.Capacity < 60000) ||
                           s.City == "Chicago" || s.ID != 1
                           orderby s.City
                           select s;

            RunTest(baseline, queryable);

            // Multiple Where query options seperated by other query options
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable2 = context.CreateQuery<League>("Leagues").
                              Where(l => l.ID == 1).
                              SelectMany(l => l.Teams).
                              Where(t => t.City == "New York").
                              OrderBy(t => t.TeamName).
                              Where(tt => tt.TeamName == "Giants");

            var baseline2 = baseLineContext.Leagues.
                            Where(l => l.ID == 1).
                              SelectMany(l => l.Teams).
                              Where(t => t.City == "New York").
                              OrderBy(t => t.TeamName).
                              Where(tt => tt.TeamName == "Giants");

            RunTest(baseline2, queryable2);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // multiple different expressions in then by

            var queryable3 = from t in context.CreateQuery<Stadium>("Stadiums")
                             orderby t.City, t.Capacity * 2, t.Capacity * 3
                             select t;

            var baseline3 = from t in baseLineContext.Stadiums
                            orderby t.City, t.Capacity * 2, t.Capacity * 3
                            select t;

            RunTest(baseline3, queryable3);

            // constants - don't use lamdbda parameters
            var queryable4 = from t in context.CreateQuery<Stadium>("Stadiums")
                             where true && TestMethod2()
                             orderby TestMethod1() + TestMethod1()
                             select t;

            var baseline4 = from t in baseLineContext.Stadiums
                            where true && TestMethod2()
                            orderby TestMethod1() + TestMethod1()
                            select t;

            RunTest(baseline4, queryable4);
        }

        internal int TestMethod1()
        {
            return 5;
        }

        internal bool TestMethod2()
        {
            return true;
        }

        // [TestMethod]
        public void LinqPathsInQueryOptions()
        {
            // paths in filter and order

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var queryable = from t in context.CreateQuery<Team>("Teams")
                            where t.HomeStadium != null &&
                                t.HomeStadium.Capacity > 10000
                            orderby t.HomeStadium.City
                            select t;


            var baseline = from t in baseLineContext.Teams
                           where t.HomeStadium != null &&
                                t.HomeStadium.Capacity > 10000
                           orderby t.HomeStadium.City
                           select t;

            RunTest(baseline, ((DataServiceQuery<Team>)queryable).Expand("HomeStadium"));
            ReadOnlyTestContext.ClearBaselineIncludes();

            // circular navigation

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Employee), "Manager");


            DataServiceQuery<Employee> queryable2 = (DataServiceQuery<Employee>)
                            from e in context.CreateQuery<Employee>("Employees")
                            where e.Manager.Manager.Name == "Big Boss"
                            orderby e.Manager.EmployeeID
                            select e;

            queryable2 = queryable2.Expand("Manager").Expand("Manager($expand=Manager)");

            var baseline2 = from e in baseLineContext.Employees
                            where e.Manager.Manager.Name == "Big Boss"
                            orderby e.Manager.EmployeeID
                            select e;

            RunTest(baseline2, queryable2);
            ReadOnlyTestContext.ClearBaselineIncludes();

            // muliple paths in a expression

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var queryable3 = from t in context.CreateQuery<Team>("Teams")
                             where t.HomeStadium != null &&
                                 t.HomeStadium.Name != t.City
                             orderby t.HomeStadium.City
                             select t;


            var baseline3 = from t in baseLineContext.Teams
                            where t.HomeStadium != null &&
                                 t.HomeStadium.Name != t.City
                            orderby t.HomeStadium.City
                            select t;

            RunTest(baseline3, ((DataServiceQuery<Team>)queryable3).Expand("HomeStadium"));
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqDataTypes()
        {
            // bool
            var queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                            where t.Boolean == false
                            orderby t.Boolean
                            select t;


            var baseline = from t in baseLineContext.Var1
                           where t.Boolean == false
                           orderby t.Boolean
                           select t;

            RunTest(baseline, queryable);

            // byte
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Byte > 0xA
                        orderby t.Byte
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Byte > 0xA
                       orderby t.Byte
                       select t;

            RunTest(queryable, baseline);

            // Decimal
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Decimal != 5.57M
                        orderby t.Decimal
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Decimal != 5.57M
                       orderby t.Decimal
                       select t;

            RunTest(queryable, baseline);

            // Double
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Double != 1.1112d
                        orderby t.Double
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Double != 1.1112d
                       orderby t.Double
                       select t;

            RunTest(queryable, baseline);

            // float
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Float != 1.1112f
                        orderby t.Float
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Float != 1.1112f
                       orderby t.Float
                       select t;

            RunTest(queryable, baseline);

            // Guid
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Guid != new Guid(1, 1, 3, new byte[] { 4, 6, 4, 1, 7, 3, 3, 3 })
                        orderby t.Guid
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Guid != new Guid(1, 1, 3, new byte[] { 4, 6, 4, 1, 7, 3, 3, 3 })
                       orderby t.Guid
                       select t;

            RunTest(queryable, baseline);

            // short
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Short > -3
                        orderby t.Short
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Short > -3
                       orderby t.Short
                       select t;

            // int
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Int != -3
                        orderby t.Int
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Int != -3
                       orderby t.Int
                       select t;

            RunTest(queryable, baseline);

            // long
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.Long > -3
                        orderby t.Long
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.Long > -3
                       orderby t.Long
                       select t;

            RunTest(queryable, baseline);

            // string
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.String == "foo"
                        orderby t.String
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.String == "foo"
                       orderby t.String
                       select t;

            RunTest(queryable, baseline);

            // DateTimeOffset
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.DateTimeOffset != new DateTimeOffset(new DateTime(1985, 5, 24), TimeSpan.FromHours(-8.0))
                        orderby t.DateTimeOffset
                        select t;

            baseline = from t in baseLineContext.Var1
                       where t.DateTimeOffset != new DateTimeOffset(new DateTime(1985, 5, 24), TimeSpan.FromHours(-8.0))
                       orderby t.DateTimeOffset
                       select t;

            RunTest(queryable, baseline);

            // TimeSpan
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.TimeSpan != TimeSpan.FromHours(2.0)
                        orderby t.TimeSpan
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.TimeSpan != TimeSpan.FromHours(2.0)
                       orderby t.TimeSpan
                       select t;

            RunTest(queryable, baseline);

            // NullableBoolean
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableBoolean == null
                        orderby t.NullableBoolean
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableBoolean == null
                       orderby t.NullableBoolean
                       select t;

            RunTest(queryable, baseline);

            // NullableByte
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableByte == null
                        orderby t.NullableByte
                        select t;

            baseline = from t in baseLineContext.Var1
                       where t.NullableByte == null
                       orderby t.NullableByte
                       select t;

            RunTest(queryable, baseline);

            // NullableDecimal
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableDecimal == null
                        orderby t.NullableDecimal
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableDecimal == null
                       orderby t.NullableDecimal
                       select t;

            RunTest(queryable, baseline);

            // NullableDouble
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableDouble == null
                        orderby t.NullableDouble
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableDouble == null
                       orderby t.NullableDouble
                       select t;

            RunTest(queryable, baseline);

            // NullableFloat
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableFloat == null
                        orderby t.NullableFloat
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableFloat == null
                       orderby t.NullableFloat
                       select t;

            RunTest(queryable, baseline);

            // NullableGuid
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableGuid == null
                        orderby t.NullableGuid
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableGuid == null
                       orderby t.NullableGuid
                       select t;

            RunTest(queryable, baseline);

            // NullableInt
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableInt == null
                        orderby t.NullableInt
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableInt == null
                       orderby t.NullableInt
                       select t;

            RunTest(queryable, baseline);

            // NullableLong
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableLong == null
                        orderby t.NullableLong
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableLong == null
                       orderby t.NullableLong
                       select t;

            RunTest(queryable, baseline);

            // NullableShort
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableShort == null
                        orderby t.NullableShort
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableShort == null
                       orderby t.NullableShort
                       select t;

            RunTest(queryable, baseline);

            // NullableDateTimeOffset
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableDateTimeOffset == null
                        orderby t.NullableDateTimeOffset
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableDateTimeOffset == null
                       orderby t.NullableDateTimeOffset
                       select t;

            RunTest(queryable, baseline);

            // TimeSpan
            queryable = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where t.NullableTimeSpan == null
                        orderby t.NullableTimeSpan
                        select t;


            baseline = from t in baseLineContext.Var1
                       where t.NullableTimeSpan == null
                       orderby t.NullableTimeSpan
                       select t;

            RunTest(queryable, baseline);
        }

        [TestMethod]
        public void LinqPrecendenceOfQueryOptions()
        {
            // TODO
        }

        [TestMethod]
        public void LinqSpecialCharactersInURI()
        {
            // TODO
        }

        // [TestMethod]
        public void LinqSpecialVBAndOr()
        {

            //And – always mapped to ‘and’ in the query operator syntax because of VB.

            ParameterExpression pe = Expression.Parameter(typeof(Stadium), "s");
            MemberInfo mi = typeof(Stadium).GetProperty("City");

            BinaryExpression be1 = Expression.MakeBinary(ExpressionType.Equal,
                Expression.MakeMemberAccess(pe, mi), Expression.Constant("Seattle"));

            BinaryExpression be2 = Expression.MakeBinary(ExpressionType.And, be1, Expression.Constant(true));
            Expression<Func<Stadium, bool>> le = (Expression<Func<Stadium, bool>>)
                Expression.Lambda(be2, Expression.Parameter(typeof(Stadium), "s"));

            var queryable = context.CreateQuery<Stadium>("Stadiums").
                            Where(le).
                            OrderBy(le).
                            Select(s => s);

            var baseline = baseLineContext.Stadiums.
                           Where(s => s.City == "Seattle" && true).
                           OrderBy(s => true).
                           Select(s => s);

            RunTest(baseline, queryable);

            //Or – always mapped to ‘or’ in the query operator syntax because of VB.

            be2 = Expression.MakeBinary(ExpressionType.Or, be1, Expression.Constant(true));
            le = (Expression<Func<Stadium, bool>>)
                Expression.Lambda(be2, Expression.Parameter(typeof(Stadium), "s"));

            var queryable2 = context.CreateQuery<Stadium>("Stadiums").
                            Where(le).
                            OrderBy(le).
                            Select(s => s);

            var baseline2 = baseLineContext.Stadiums.
                           Where(s => s.City == "Seattle" || true).
                           OrderBy(s => true).
                           Select(s => s);

            RunTest(baseline2, queryable2);

            // Coalesce

            ParameterExpression cPe = Expression.Parameter(typeof(YetAnotherAllPrimitiveTypesType), "x");
            MemberInfo cMi = typeof(YetAnotherAllPrimitiveTypesType).GetProperty("NullableInt");
            int? y = 63;
            BinaryExpression cBe = Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(cPe, cMi),
                   Expression.Constant(y, typeof(int?)), true, null);

            Expression cCoalesce = Expression.Coalesce(cBe, Expression.Constant(false));
            Expression<Func<YetAnotherAllPrimitiveTypesType, bool>> cLe = (Expression<Func<YetAnotherAllPrimitiveTypesType, bool>>)
                Expression.Lambda(cCoalesce, new ParameterExpression[] { cPe });


            var queryable3 = context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1").
                             Where(cLe).
                             Select(s => s);

            var baseline3 = baseLineContext.Var1.
                           Where(cLe).
                           Select(s => s);

            RunTest(baseline3, queryable3);

        }

        // [TestMethod]
        public void LinqV1Projections()
        {
            // project primitive
            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            where s.ID == 1
                            select s.City;


            var baseline = from s in baseLineContext.Stadiums
                           where s.ID == 1
                           select s.City;

            RunTest(baseline, queryable);

            // project into tuple
            var queryable2 = from x in
                                 (from s in context.CreateQuery<Stadium>("Stadiums")
                                  select s).AsEnumerable()
                             select new { x.City, x.Capacity };


            var baseline2 = from s in baseLineContext.Stadiums
                            select new { s.City, s.Capacity };

            RunTest(baseline2, queryable2.AsQueryable());

            // project into constructor
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable4 = from x in
                                 (from l in context.CreateQuery<League>("Leagues")
                                  where l.ID == 1
                                  from t in l.Teams
                                  select t).AsEnumerable()
                             select new Team() { City = x.City, Players = null, HomeStadium = new Stadium() { Name = "KingDome" }, TeamID = x.TeamID, TeamName = x.TeamName };


            var baseline4 = from l in baseLineContext.Leagues
                            where l.ID == 1
                            from t in l.Teams
                            select new Team() { City = t.City, Players = null, HomeStadium = new Stadium() { Name = "KingDome" }, TeamID = t.TeamID, TeamName = t.TeamName };

            RunTest(baseline4, queryable4.AsQueryable());

            ReadOnlyTestContext.ClearBaselineIncludes();

            // project with query options

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable5 = from x in
                                 (from l in context.CreateQuery<League>("Leagues")
                                  where l.ID == 1
                                  from t in l.Teams
                                  select t).AsEnumerable()
                             select new { x.City, x.TeamName, Stadium = x.HomeStadium, constant = 5 };


            var baseline5 = from l in baseLineContext.Leagues
                            where l.ID == 1
                            from t in l.Teams
                            select new { t.City, t.TeamName, Stadium = t.HomeStadium, constant = 5 };

            RunTest(baseline5, queryable5.AsQueryable());

            ReadOnlyTestContext.ClearBaselineIncludes();

            // project with query options
            var queryable6 = from x in
                                 (from s in context.CreateQuery<Stadium>("Stadiums")
                                  where s.Capacity > 30000
                                  orderby s.Name
                                  select s).AsEnumerable()
                             select new { x.City, x.Capacity };


            var baseline6 = from s in baseLineContext.Stadiums
                            where s.Capacity > 30000
                            orderby s.Name
                            select new { s.City, s.Capacity };

            RunTest(baseline6, queryable6.AsQueryable());

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var queryable7 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                      orderby t.TeamName
                                                      select t).Expand("HomeStadium").AsEnumerable().
                             Select(t => new { t.City, t.TeamName, t.HomeStadium, constant = 5 });


            var baseline7 = from t in baseLineContext.Teams
                            orderby t.TeamName
                            select new { t.City, t.TeamName, t.HomeStadium, constant = 5 };

            RunTest(baseline7, queryable7.AsQueryable());

            ReadOnlyTestContext.ClearBaselineIncludes();

        }

        // [TestMethod]
        public void LinqV2SimpleProjections()
        {
            // simple projection into Anonymous type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            select new { A = s.ID, City = s.City, Capacity = s.Capacity };


            var baseline = from s in baseLineContext.Stadiums
                           select new { A = s.ID, City = s.City, Capacity = s.Capacity };

            RunTest(baseline, queryable, true);

            // project into narrow type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable2 = from s in context.CreateQuery<Stadium>("Stadiums")
                             select new NarrowStadium { City = s.City, ID = s.ID };

            var baseline2 = from s in baseLineContext.Stadiums
                            select new NarrowStadium { City = s.City, ID = s.ID };

            RunTest(baseline2, queryable2);

            Trace.WriteLine("projection into nested narrow types");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(NarrowStadium), "Sponsor");

            var queryable3 = from s in context.CreateQuery<Stadium>("Stadiums")
                             select new NarrowStadium { ID = s.ID, City = s.City, Sponsor = new NarrowStadiumSponsor { StadiumSponsorID = s.Sponsor.StadiumSponsorID } };

            var baseline3 = from s in baseLineContext.Stadiums
                            select new NarrowStadium { ID = s.ID, City = s.City, Sponsor = new NarrowStadiumSponsor { StadiumSponsorID = s.Sponsor.StadiumSponsorID } };

            RunTest(baseline3, queryable3);

            // projection into nested anouymous types
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");

            var queryable4 = from s in context.CreateQuery<Stadium>("Stadiums")
                             select new { A = s.ID, City = s.City + "City", Sponsor = new { StadiumSponsorID = s.Sponsor.StadiumSponsorID * 2 } };

            var baseline4 = from s in baseLineContext.Stadiums
                            select new { A = s.ID, City = s.City + "City", Sponsor = new { StadiumSponsorID = s.Sponsor.StadiumSponsorID * 2 } };

            RunTest(baseline4, queryable4, true);

            // multi-step nav prop.
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");

            var queryable5 = from s in context.CreateQuery<Stadium>("Stadiums")
                             select new { B = s.ID, SponserId = s.Sponsor.StadiumSponsorID, Sponsor = new NarrowStadiumSponsor { StadiumSponsorID = s.Sponsor.StadiumSponsorID } };

            var baseline5 = from s in baseLineContext.Stadiums
                            select new { B = s.ID, SponserId = s.Sponsor.StadiumSponsorID, Sponsor = new NarrowStadiumSponsor { StadiumSponsorID = s.Sponsor.StadiumSponsorID } };

            RunTest(baseline5, queryable5, true);

            {
                Trace.WriteLine("SelectMany");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

                var queryable6 = from s in context.CreateQuery<BigState>("BigStates")
                                 where s.StateName == "Oregon"
                                 from c in s.Cities
                                 select new LittleCity
                                 {
                                     Mayor = c.Mayor,
                                     BigCityID = c.BigCityID,
                                 };

                var baseline6 = from s in baseLineContext.BigStates
                                where s.StateName == "Oregon"
                                from c in s.Cities
                                select new LittleCity
                                {
                                    Mayor = c.Mayor,
                                    BigCityID = c.BigCityID
                                };

                RunTest(baseline6, queryable6, true);
            }
        }

        // [TestMethod]
        public void LinqV2ProjectionsQueryOptions()
        {
            // all query options before projection - entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            var queryable1 = from x in
                                 (from s in context.CreateQuery<BigState>("BigStates").AddQueryOption("foo", "bar")
                                  where s.StateName == "Oregon"
                                  from c in s.Cities
                                  where c.HasBaseballTeam != null && (bool)c.HasBaseballTeam
                                  orderby c.Population descending
                                  select c).Skip(1).Take(1)
                             select new LittleCity
                             {
                                 Mayor = x.Mayor,
                                 BigCityID = x.BigCityID,
                             };

            var baseline1 = from x in
                                (from s in baseLineContext.BigStates
                                 where s.StateName == "Oregon"
                                 from c in s.Cities
                                 where c.HasBaseballTeam != null && (bool)c.HasBaseballTeam
                                 orderby c.Population descending
                                 select c).Skip(1).Take(1)
                            select new LittleCity
                            {
                                Mayor = x.Mayor,
                                BigCityID = x.BigCityID,
                            };

            RunTest(baseline1, queryable1, true);

            // all query options before projection - non-entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable2 = from x in
                                 (from s in context.CreateQuery<BigState>("BigStates").AddQueryOption("foo", "bar")
                                  where s.StateName == "Oregon"
                                  from c in s.Cities
                                  where c.HasBaseballTeam != null && (bool)c.HasBaseballTeam
                                  orderby c.Population descending
                                  select c).Skip(1).Take(1)
                             select new
                             {
                                 Mayor = x.Mayor,
                                 BigCityID = x.BigCityID,
                             };

            var baseline2 = from x in
                                (from s in baseLineContext.BigStates
                                 where s.StateName == "Oregon"
                                 from c in s.Cities
                                 where c.HasBaseballTeam != null && (bool)c.HasBaseballTeam
                                 orderby c.Population descending
                                 select c).Skip(1).Take(1)
                            select new
                            {
                                Mayor = x.Mayor,
                                BigCityID = x.BigCityID,
                            };

            RunTest(baseline2, queryable2, true);

            // Take, Skip, Expand, AddCustomQueryOption after projection - entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            var queryable3 = ((DataServiceQuery<LittleCity>)(from s in context.CreateQuery<BigCity>("BigCities")
                                                             select new LittleCity
                                                             {
                                                                 Mayor = s.Mayor,
                                                                 BigCityID = s.BigCityID,
                                                             }).Skip(2).Take(2)).AddQueryOption("foo", 5);

            var baseline3 = (from s in baseLineContext.BigCities
                             select new LittleCity
                             {
                                 Mayor = s.Mayor,
                                 BigCityID = s.BigCityID,
                             }).Skip(2).Take(2);

            RunTest(baseline3, queryable3, true);

            // Take, Skip, Expand, AddCustomQueryOption after projection - non entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            var queryable4 = ((DataServiceQuery<LittleCityNotAnEntity>)(from s in context.CreateQuery<BigCity>("BigCities")
                                                                        select new LittleCityNotAnEntity
                                                                        {
                                                                            Mayor = s.Mayor,
                                                                            BigCityID = s.BigCityID.ToString(),
                                                                        }).Skip(2).Take(2)).AddQueryOption("foo", 5);

            var baseline4 = (from s in baseLineContext.BigCities
                             select new LittleCityNotAnEntity
                             {
                                 Mayor = s.Mayor,
                                 BigCityID = s.BigCityID.ToString(),
                             }).Skip(2).Take(2);

            RunTest(baseline4, queryable4, true);

            // illegal case, filter after projection - Entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable5 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new LittleCity
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                              }).Where(lc => lc.Population > 1);

            bool passed = false;

            try
            {
                queryable5.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message == "The filter query option cannot be specified after the select query option.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed!");

            // illegal case, filter after projection - non Entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable6 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                              }).Where(lc => lc.Mayor != "Mayor Nickels");

            passed = false;

            try
            {
                queryable6.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message == "The filter query option cannot be specified after the select query option.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed!");

            // illegal case, orderby after projection - Entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable7 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new LittleCity
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                              }).OrderBy(lc => lc.Mayor);

            passed = false;

            try
            {
                queryable7.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message == "The orderby query option cannot be specified after the select query option.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed!");

            // illegal case, orderby after projection - non Entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable8 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                              }).OrderBy(lc => lc.Mayor);

            passed = false;

            try
            {
                queryable8.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message == "The orderby query option cannot be specified after the select query option.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed!");

        }

        // [TestMethod]
        public void LinqV2ProjectionsNavigationAfterProjection()
        {
            bool passed;

            {
                Trace.WriteLine("illegal - navigating after projection - Non entity type.");
                ReadOnlyTestContext.ClearBaselineIncludes();

                var queryable1 = (from s in context.CreateQuery<BigCity>("BigCities")
                                  select new
                                  {
                                      Mayor = s.Mayor,
                                      BigCityID = s.BigCityID,
                                  }).Select(p => p.Mayor);

                passed = false;
                try
                {
                    queryable1.GetEnumerator();
                }
                catch (NotSupportedException nse)
                {
                    if (nse.Message == "Can only specify 'select' query option after last navigation.")
                        passed = true;
                }

                if (!passed)
                    throw new Exception("Test Failed!");
            }

            Trace.WriteLine("illegal - navigating after projection - entity type.");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable2 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new LittleCity
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                              }).Select(p => p.Mayor);

            passed = false;

            try
            {
                queryable2.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message == "Can only specify 'select' query option after last navigation.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed!");

            Trace.WriteLine("identity projection - non-entity type");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable3 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                              }).Select(p => p);

            var baseline3 = (from s in baseLineContext.BigCities
                             select new
                             {
                                 Mayor = s.Mayor,
                                 BigCityID = s.BigCityID,
                             }).Select(p => p);

            RunTest(baseline3, queryable3, true);

            Trace.WriteLine("identity projection - entity type");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable4 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new LittleCity
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                              }).Select(p => p);

            var baseline4 = (from s in baseLineContext.BigCities
                             select new LittleCity
                             {
                                 Mayor = s.Mayor,
                                 BigCityID = s.BigCityID,
                             }).Select(p => p);

            RunTest(baseline4, queryable4, true);

            Trace.WriteLine("illegal - navigating to singleton after projection - Non entity type.");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable5 = (from s in context.CreateQuery<BigCity>("BigCities")
                              select new
                              {
                                  Mayor = s.Mayor,
                                  BigCityID = s.BigCityID,
                                  State = s.State
                              }).Select(p => p.State);

            passed = false;

            try
            {
                queryable5.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message == "Can only specify 'select' query option after last navigation.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed!");

        }

        // [TestMethod]
        public void LinqV2ProjectionsProjectingSingletons()
        {
            Trace.WriteLine("projection on singleton, non-entity type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            var queryable1 = from x in
                                 (from c in context.CreateQuery<BigCity>("BigCities")
                                  where c.BigCityID == 1
                                  select c.State)
                             select new
                             {
                                 a = x.Population,
                                 b = x.StateName
                             };

            var baseline1 = from x in
                                (from c in baseLineContext.BigCities
                                 where c.BigCityID == 1
                                 select c.State)
                            select new
                            {
                                a = x.Population,
                                b = x.StateName
                            };

            RunTest(baseline1, queryable1, true);

            Trace.WriteLine("projection on singleton, entity type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            var queryable2 = from x in
                                 (from c in context.CreateQuery<BigCity>("BigCities")
                                  where c.BigCityID == 1
                                  select c.State)
                             select new LittleState
                             {
                                 StateName = x.StateName
                             };

            var baseline2 = from x in
                                (from c in baseLineContext.BigCities
                                 where c.BigCityID == 1
                                 select c.State)
                            select new LittleState
                            {
                                StateName = x.StateName
                            };

            RunTest(baseline2, queryable2, true);

            // projection on singleton with query options, entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            var queryable3 = from x in
                                 (from c in context.CreateQuery<BigCity>("BigCities").AddQueryOption("gggg", "hhhh")
                                  where c.BigCityID == 1
                                  select c.State)
                             select new LittleState
                             {
                                 StateName = x.StateName
                             };

            var baseline3 = from x in
                                (from c in baseLineContext.BigCities
                                 where c.BigCityID == 1
                                 select c.State)
                            select new LittleState
                            {
                                StateName = x.StateName
                            };

            RunTest(baseline3, queryable3, true);
        }

        // [TestMethod]
        public void LinqV2ProjectionsSingleValueReturningOperators()
        {
            // Single with non-entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val1 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        where s.Name == "Brewer Stadium"
                        select new
                        {
                            Name = s.Name,
                            B = s.City
                        }).Single();

            var baseline1 = (from s in baseLineContext.Stadiums
                             where s.Name == "Brewer Stadium"
                             select new
                             {
                                 Name = s.Name,
                                 B = s.City
                             }).Single();

            Assert.AreEqual(val1, baseline1);

            // Single with entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val2 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        where s.Name == "Brewer Stadium"
                        select new NarrowStadium
                        {
                            City = s.City,
                            ID = s.ID
                        }).Single();


            var baseline2 = (from s in baseLineContext.Stadiums
                             where s.Name == "Brewer Stadium"
                             select new NarrowStadium
                             {
                                 City = s.City,
                                 ID = s.ID
                             }).Single();

            Assert.AreEqual(val2, baseline2);

            // SingleOrDefault with non-entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val3 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        where false
                        select new
                        {
                            Name = s.Name,
                            B = s.City
                        }).SingleOrDefault();

            var baseline3 = (from s in baseLineContext.Stadiums
                             where false
                             select new
                             {
                                 Name = s.Name,
                                 B = s.City
                             }).SingleOrDefault();

            Assert.AreEqual(val3, baseline3);

            // SingleorDefault with entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val4 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        where 1 == 0
                        select new NarrowStadium
                        {
                            City = s.City,
                            ID = s.ID
                        }).SingleOrDefault();


            var baseline4 = (from s in baseLineContext.Stadiums
                             where 1 == 0
                             select new NarrowStadium
                             {
                                 City = s.City,
                                 ID = s.ID
                             }).SingleOrDefault();

            Assert.AreEqual(val4, baseline4);

            // First with non-entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val5 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        orderby s.City
                        select new
                        {
                            Name = s.Name,
                            B = s.City
                        }).First();

            var baseline5 = (from s in baseLineContext.Stadiums
                             orderby s.City
                             select new
                             {
                                 Name = s.Name,
                                 B = s.City
                             }).First();

            Assert.AreEqual(val5, baseline5);

            // First with entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val6 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        orderby s.Name
                        select new NarrowStadium
                        {
                            City = s.City,
                            ID = s.ID
                        }).First();


            var baseline6 = (from s in baseLineContext.Stadiums
                             orderby s.Name
                             select new NarrowStadium
                             {
                                 City = s.City,
                                 ID = s.ID
                             }).First();

            Assert.AreEqual(val6, baseline6);

            // FirstOrDefault with non-entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val7 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        where false
                        select new
                        {
                            Name = s.Name,
                            B = s.City
                        }).FirstOrDefault();

            var baseline7 = (from s in baseLineContext.Stadiums
                             where false
                             select new
                             {
                                 Name = s.Name,
                                 B = s.City
                             }).FirstOrDefault();

            Assert.AreEqual(val7, baseline7);

            // FirstOrDefault with entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val8 = (from s in context.CreateQuery<Stadium>("Stadiums")
                        where true
                        select new NarrowStadium
                        {
                            City = s.City,
                            ID = s.ID
                        }).FirstOrDefault();


            var baseline8 = (from s in baseLineContext.Stadiums
                             where true
                             select new NarrowStadium
                             {
                                 City = s.City,
                                 ID = s.ID
                             }).FirstOrDefault();

            Assert.AreEqual(val8, baseline8);

            // with inline count
            ReadOnlyTestContext.ClearBaselineIncludes();

            var val9 = (from s in context.CreateQuery<Stadium>("Stadiums").IncludeTotalCount()
                        where true
                        select new NarrowStadium
                        {
                            City = s.City,
                            ID = s.ID
                        }).FirstOrDefault();


            var baseline9 = (from s in baseLineContext.Stadiums
                             where true
                             select new NarrowStadium
                             {
                                 City = s.City,
                                 ID = s.ID
                             }).FirstOrDefault();

            Assert.AreEqual(val9, baseline9);
        }

        // [TestMethod]
        public void LinqV2ProjectionsSelectingSingleProperty()
        {
            Trace.WriteLine("selecting single primitve property - non EntityType");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var query1 = from item in context.CreateQuery<BigState>("BigStates")
                         where item.StateName == "Oregon"
                         select new
                         {
                             a = item.StateName,
                             c = item.ID,
                             d = from b in item.Cities select b.Name
                         };

            var baseline1 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            select new
                            {
                                a = item.StateName,
                                c = item.ID,
                                d = from b in item.Cities select b.Name
                            };

            RunTest(baseline1, query1, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

            Trace.WriteLine("selecting single property - EntityType");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var query2 = from item in context.CreateQuery<BigState>("BigStates")
                         where item.StateName == "Oregon"
                         select new LittleState4
                         {
                             ID = item.ID,
                             StateName = item.StateName,
                             Cities = (from c in item.Cities select c).ToList()
                         };

            var baseline2 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            select new LittleState4
                            {
                                ID = item.ID,
                                StateName = item.StateName,
                                Cities = (from c in item.Cities select c).ToList()
                            };

            RunTest(baseline2, query2, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

            Trace.WriteLine("selecting single entity property - non EntityType");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var query3 = from item in context.CreateQuery<BigState>("BigStates")
                         where item.StateName == "Oregon"
                         select new
                         {
                             ID = item.ID,
                             StateName = item.StateName,
                             Cities = (from c in item.Cities select c).ToList()
                         };

            var baseline3 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            select new
                            {
                                ID = item.ID,
                                StateName = item.StateName,
                                Cities = (from c in item.Cities select c).ToList()
                            };

            RunTest(baseline3, query3, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

            Trace.WriteLine("selecting single primitive property - EntityType");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var query4 = from item in context.CreateQuery<BigState>("BigStates")
                         where item.StateName == "Oregon"
                         select new
                         {
                             ID = item.ID,
                             StateName = item.StateName,
                             CityNames = (from c in item.Cities select c.Name).ToList()
                         };

            var baseline4 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            select new
                            {
                                ID = item.ID,
                                StateName = item.StateName,
                                CityNames = (from c in item.Cities select c.Name).ToList()
                            };

            RunTest(baseline4, query4, true);
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqV2ProjectionsCount()
        {
            // Count on projection, non entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var count1 = (from c in context.CreateQuery<BigCity>("BigCities")
                          select new
                          {
                              Id = c.BigCityID / 2,
                              Name = c.Name + "ghghghg"
                          }).Count();

            var baselineCount1 = (from c in baseLineContext.BigCities
                                  select new
                                  {
                                      Id = c.BigCityID / 2,
                                      Name = c.Name + "ghghghg"
                                  }).Count();

            if ((count1 != baselineCount1) || (count1 != baseLineContext.BigCities.Count()))
            {
                throw new Exception("Test failed");
            }
            ReadOnlyTestContext.ClearBaselineIncludes();

            // Count on projection, entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var count2 = (from c in context.CreateQuery<BigCity>("BigCities")
                          select new LittleCity
                          {
                              BigCityID = c.BigCityID,
                              Name = c.Name
                          }).Count();

            var baselineCount2 = (from c in baseLineContext.BigCities
                                  select new LittleCity
                                  {
                                      BigCityID = c.BigCityID,
                                      Name = c.Name
                                  }).Count();

            if ((count2 != baselineCount2) || (count2 != baseLineContext.BigCities.Count()))
            {
                throw new Exception("Test failed");
            }
            ReadOnlyTestContext.ClearBaselineIncludes();

            // LongCount on projection, non entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var count3 = (from c in context.CreateQuery<BigCity>("BigCities")
                          select new
                          {
                              Id = c.BigCityID / 2,
                              Name = c.Name + "ghghghg"
                          }).LongCount();

            var baselineCount3 = (from c in baseLineContext.BigCities
                                  select new
                                  {
                                      Id = c.BigCityID / 2,
                                      Name = c.Name + "ghghghg"
                                  }).LongCount();

            if ((count3 != baselineCount3) || (count3 != baseLineContext.BigCities.LongCount()))
            {
                throw new Exception("Test failed");
            }
            ReadOnlyTestContext.ClearBaselineIncludes();

            // LongCount on projection, entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var count4 = (from c in context.CreateQuery<BigCity>("BigCities")
                          select new LittleCity
                          {
                              BigCityID = c.BigCityID,
                              Name = c.Name
                          }).Count();

            var baselineCount4 = (from c in baseLineContext.BigCities
                                  select new LittleCity
                                  {
                                      BigCityID = c.BigCityID,
                                      Name = c.Name
                                  }).Count();

            if ((count4 != baselineCount4) || (count4 != baseLineContext.BigCities.Count()))
            {
                throw new Exception("Test failed");
            }
            ReadOnlyTestContext.ClearBaselineIncludes();

            //IncludeTotalCount - Entity Type.
            ReadOnlyTestContext.ClearBaselineIncludes();
            var queryable = from c in context.CreateQuery<BigCity>("BigCities").IncludeTotalCount()
                            select new LittleCity
                            {
                                BigCityID = c.BigCityID,
                                Name = c.Name
                            };

            var baseline = from c in baseLineContext.BigCities
                           select new LittleCity
                            {
                                BigCityID = c.BigCityID,
                                Name = c.Name
                            };


            var results = ((DataServiceQuery<LittleCity>)queryable).Execute();

            QueryOperationResponse qor = (QueryOperationResponse)(results);

            if ((qor.TotalCount != baseline.Count()) || (qor.TotalCount != baseLineContext.BigCities.Count()))
            {
                throw new Exception("Test failed");
            }

            RunTest(baseline, queryable, true);

            //IncludeTotalCount - non Entity Type.
            ReadOnlyTestContext.ClearBaselineIncludes();
            var queryable2 = from c in context.CreateQuery<BigCity>("BigCities").IncludeTotalCount()
                             select new
                             {
                                 BigCityID = c.BigCityID,
                                 Name = c.Name
                             };

            var baseline2 = from c in baseLineContext.BigCities
                            select new
                            {
                                BigCityID = c.BigCityID,
                                Name = c.Name
                            };


            var results2 = queryable2.GetType().GetMethod("Execute").Invoke(queryable2, null);

            QueryOperationResponse qor2 = (QueryOperationResponse)(results2);

            if ((qor2.TotalCount != baseline.Count()) || (qor2.TotalCount != baseLineContext.BigCities.Count()))
            {
                throw new Exception("Test failed");
            }

            RunTest(baseline2, queryable2, true);

            //IncludeTotalCount - Entity Type, after projection
            ReadOnlyTestContext.ClearBaselineIncludes();
            var queryable3 = ((DataServiceQuery<LittleCity>)from c in context.CreateQuery<BigCity>("BigCities")
                                                            select new LittleCity
                                                            {
                                                                BigCityID = c.BigCityID,
                                                                Name = c.Name
                                                            }).IncludeTotalCount();

            var baseline3 = from c in baseLineContext.BigCities
                            select new LittleCity
                            {
                                BigCityID = c.BigCityID,
                                Name = c.Name
                            };


            var results3 = ((DataServiceQuery<LittleCity>)queryable3).Execute();

            QueryOperationResponse qor3 = (QueryOperationResponse)(results3);

            if ((qor3.TotalCount != baseline3.Count()) || (qor3.TotalCount != baseLineContext.BigCities.Count()))
            {
                throw new Exception("Test failed");
            }

            RunTest(baseline3, queryable3, true);

            // IncludeTotalCount with query options
            ReadOnlyTestContext.ClearBaselineIncludes();
            var queryable4 = from c in context.CreateQuery<BigCity>("BigCities").IncludeTotalCount()
                             where c.BigCityID > 0
                             orderby c.Population
                             select new LittleCity
                             {
                                 BigCityID = c.BigCityID,
                                 Name = c.Name
                             };

            var baseline4 = from c in baseLineContext.BigCities
                            where c.BigCityID > 0
                            orderby c.Population
                            select new LittleCity
                            {
                                BigCityID = c.BigCityID,
                                Name = c.Name
                            };


            var results4 = ((DataServiceQuery<LittleCity>)queryable4).Execute();

            QueryOperationResponse qor4 = (QueryOperationResponse)(results);

            if (qor4.TotalCount != baseline.Count())
            {
                throw new Exception("Test failed");
            }

            RunTest(baseline4, queryable4, true);
        }

        // [TestMethod]
        public void LinqV2ProjectionsKnownEntityTypes()
        {
            {
                Trace.WriteLine("basic narrowing");
                ReadOnlyTestContext.ClearBaselineIncludes();

                var queryable1 = from s in context.CreateQuery<BigCity>("BigCities")
                                 select new LittleCity() { BigCityID = s.BigCityID, Mayor = s.Mayor, Population = s.Population };

                var baseline1 = from s in baseLineContext.BigCities
                                select new LittleCity() { BigCityID = s.BigCityID, Mayor = s.Mayor, Population = s.Population };

                RunTest(baseline1, queryable1, true);
            }

            // TODO: Client generating incorrect query for anonymous lookups
            // TODO: When the client sees s.Name here it adds a select clause which
            // TODO: in V4 overrides the select all (and there is no implicit recursive select all anymore to recover).
            // TODO: The client needs to be changed to add a wildcard to supercede the select item
            // TODO: in this case.

            // TODO: for now I'm removing the select clause so that this passes and 
            // TODO: well need to fix the client later.
            //{
            //    Trace.WriteLine("original entity and projection");
            //    ReadOnlyTestContext.ClearBaselineIncludes();

            //    var query = from s in context.CreateQuery<BigCity>("BigCities")
            //                select new { city = s, city_name = s.Name };

            //    var baseline = from s in baseLineContext.BigCities
            //                   select new { city = s, city_name = s.Name };

            //    RunTest(baseline, query, true);
            //}

            // project into same type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable2 = from s in context.CreateQuery<BigCity>("BigCities")
                             select new BigCity()
                             {
                                 BigCityID = s.BigCityID,
                                 Mayor = s.Mayor,
                                 Population = s.Population,
                             };

            var baseline2 = from s in baseLineContext.BigCities
                            select new BigCity()
                            {
                                BigCityID = s.BigCityID,
                                Mayor = s.Mayor,
                                Population = s.Population,
                            };

            RunTest(baseline2, queryable2, true);

            Trace.WriteLine("multilevel narrow case");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable4 = from s in context.CreateQuery<BigState>("BigStates")
                             select new LittleState
                             {
                                 StateName = s.StateName,
                                 TopCity = new LittleCity() { Mayor = s.TopCity.Mayor, Name = s.TopCity.Name, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                 Cities = s.Cities.Select(bc => new LittleCity() { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                             };

            var baseline4 = from s in baseLineContext.BigStates
                            select new LittleState
                            {
                                StateName = s.StateName,
                                TopCity = new LittleCity() { Mayor = s.TopCity.Mayor, Name = s.TopCity.Name, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                Cities = s.Cities.Select(bc => new LittleCity() { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                            };

            RunTest(baseline4, queryable4, true);
            ReadOnlyTestContext.ClearBaselineIncludes();


            // with filter and orderby
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable5 = from s in context.CreateQuery<BigState>("BigStates")
                             where s.Population > 1
                             orderby s.TopCity.Mayor
                             select new LittleState
                             {
                                 StateName = s.StateName,
                                 TopCity = new LittleCity() { Mayor = s.TopCity.Mayor, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                 Cities = s.Cities.Select(bc => new LittleCity() { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                             };

            var baseline5 = from s in baseLineContext.BigStates
                            where s.Population > 1
                            orderby s.TopCity.Mayor
                            select new LittleState
                            {
                                StateName = s.StateName,
                                TopCity = new LittleCity() { Mayor = s.TopCity.Mayor, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                Cities = s.Cities.Select(bc => new LittleCity() { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                            };

            RunTest(baseline5, queryable5, true);

            Uri uri = new Uri(queryable5.ToString());
            string[] segments = uri.Segments;
            Assert.AreEqual("BigStates", segments[segments.Length - 1]);
            Assert.AreEqual("?$filter=Population%20gt%201&$orderby=TopCity/Mayor&$expand=TopCity($select=Mayor),TopCity($select=Population),TopCity($select=BigCityID),Cities($select=Mayor),Cities($select=Population),Cities($select=BigCityID)&$select=StateName",
                uri.Query, "uri.Query (for queryable5)");

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqV2ProjectionsKnownEntityTypesLegalCasts()
        {
            Trace.WriteLine("casts in projection");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable1 = from s in context.CreateQuery<BigState>("BigStates")
                             select new LittleState3
                             {
                                 ID = s.ID,  // implicit cast short to long
                                 StateName = (string)s.StateName,
                                 TopCity = new LittleCity3() { BigCityID = s.TopCity.BigCityID, Mayor = (string)s.TopCity.Mayor, Population = (int)s.TopCity.Population }

                             };

            var baseline1 = from s in baseLineContext.BigStates
                            select new LittleState3
                            {
                                ID = s.ID,  // implicit cast short to long
                                StateName = (string)s.StateName,
                                TopCity = new LittleCity3() { BigCityID = s.TopCity.BigCityID, Mayor = (string)s.TopCity.Mayor, Population = (int)s.TopCity.Population }
                            };

            RunTest(baseline1, queryable1, true);

            Trace.WriteLine("casts - datatypes and nullables");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable2 = from p in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                             select new BigCityVar1
                             {
                                 ID = (int)(float)p.ID,
                                 NullableBoolean = (bool?)p.NullableBoolean,
                                 NullableDateTimeOffset = (DateTimeOffset?)p.NullableDateTimeOffset,
                                 Int = (int)p.Int,
                                 Long = (int)p.Long,
                                 Short = (int)p.Short,
                                 NullableInt = (long?)p.NullableInt,
                                 NullableShort = (long?)p.NullableShort,
                                 NullableLong = (long?)p.NullableLong,
                                 NullableTimeSpan = (TimeSpan?)p.NullableTimeSpan
                             };

            var baseline2 = from p in baseLineContext.Var1
                            select new BigCityVar1
                            {
                                ID = (int)(float)p.ID,
                                NullableBoolean = (bool?)p.NullableBoolean,
                                NullableDateTimeOffset = (DateTimeOffset?)p.NullableDateTimeOffset,
                                Int = (int)p.Int,
                                Long = (int)p.Long,
                                Short = (int)p.Short,
                                NullableInt = (long?)p.NullableInt,
                                NullableShort = (long?)p.NullableShort,
                                NullableLong = (long?)p.NullableLong,
                                NullableTimeSpan = (TimeSpan?)p.NullableTimeSpan
                            };

            RunTest(baseline2, queryable2, true);

            Trace.WriteLine("checked");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable3 = from p in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                             select new BigCityVar1
                             {
                                 ID = checked((int)(float)p.ID),
                                 NullableBoolean = checked((bool?)p.NullableBoolean),
                                 NullableLong = checked((long?)p.NullableLong),
                                 NullableDateTimeOffset = checked((DateTimeOffset?)p.NullableDateTimeOffset),
                                 NullableTimeSpan = checked((TimeSpan?)p.NullableTimeSpan)
                             };

            var baseline3 = from p in baseLineContext.Var1
                            select new BigCityVar1
                            {
                                ID = checked((int)(float)p.ID),
                                NullableBoolean = checked((bool?)p.NullableBoolean),
                                NullableLong = checked((long?)p.NullableLong),
                                NullableDateTimeOffset = checked((DateTimeOffset?)p.NullableDateTimeOffset),
                                NullableTimeSpan = checked((TimeSpan?)p.NullableTimeSpan)
                            };

            RunTest(baseline3, queryable3, true);

            Trace.WriteLine("illegal casts -> entity type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");

            var queryable4 = from s in context.CreateQuery<BigState>("BigStates")
                             select new LittleState3
                             {
                                 TopCity = (LittleCity3)s.TopCity
                             };

            bool passed = false;

            try
            {
                queryable4.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Initializing instances of the entity type AstoriaUnitTests.LittleState3 with the expression Convert(s.TopCity) is not supported.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");

            Trace.WriteLine("illegal cast -> complex type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");

            var queryable5 = from s in context.CreateQuery<BigState>("BigStates")
                             select new BigCity
                             {
                                 MiscInfo = (BigCityComplexType)s.TopCity
                             };

            passed = false;

            try
            {
                queryable5.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Initializing instances of the entity type AstoriaUnitTests.BigCity with the expression Convert(s.TopCity) is not supported.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // I don't think this is relevant anymore... do we care about V2?
        // [TestMethod]
        public void LinqV2ProjectionsProjectingEntireEntity()
        {
            // selecting entire entity into non-Entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable1 = from p in context.CreateQuery<BigCity>("BigCities")
                             select new
                             {
                                 BC = p
                             };

            var baseline1 = from p in baseLineContext.BigCities
                            select new
                            {
                                BC = p
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable1.ToString(), "BigCities()");
            RunTest(baseline1, queryable1, true);



            // selecting entire entity + some properties into non-Entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable2 = from p in context.CreateQuery<BigCity>("BigCities")
                             select new
                             {
                                 BC = p,
                                 Name = p.Name,
                                 Streets = p.Streets
                             };

            var baseline2 = from p in baseLineContext.BigCities
                            select new
                            {
                                BC = new BigCity { Name = p.Name, BigCityID = p.BigCityID, HasBaseballTeam = p.HasBaseballTeam, Population = p.Population, MiscInfo = p.MiscInfo, Mayor = p.Mayor, Streets = null },
                                Name = p.Name,
                                Streets = p.Streets
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable2.ToString(), "$expand=Streets&$select=*,Name,Streets");
            RunTest(baseline2, queryable2, true);

            // selecting entire entity + some fields of nav property into non-entitytype
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable3 = from p in context.CreateQuery<BigCity>("BigCities")
                             select new
                             {
                                 BC = p,
                                 Name = p.Name,
                                 SmallStreets = (from s in p.Streets select new { s.StreetName }).ToList()
                             };

            var baseline3 = from p in baseLineContext.BigCities
                            select new
                            {
                                BC = new BigCity { Name = p.Name, BigCityID = p.BigCityID, HasBaseballTeam = p.HasBaseballTeam, Population = p.Population, MiscInfo = p.MiscInfo, Mayor = p.Mayor, Streets = null },
                                Name = p.Name,
                                SmallStreets = (from s in p.Streets select new { s.StreetName }).ToList()
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable3.ToString(), "$expand=Streets($select=StreetName)&$select=*,Name");
            RunTest(baseline3, queryable3, true);

            // selecting entire entity + single field of nav property
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable4 = from p in context.CreateQuery<BigCity>("BigCities")
                             select new
                             {
                                 BC = p,
                                 Name = p.Name,
                                 SmallStreets = (from s in p.Streets select s.StreetName).ToList()
                             };

            var baseline4 = from p in baseLineContext.BigCities
                            select new
                            {
                                BC = new BigCity { Name = p.Name, BigCityID = p.BigCityID, HasBaseballTeam = p.HasBaseballTeam, Population = p.Population, MiscInfo = p.MiscInfo, Mayor = p.Mayor, Streets = null },
                                Name = p.Name,
                                SmallStreets = (from s in p.Streets select s.StreetName).ToList()
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable4.ToString(), "$expand=Streets($select=StreetName)&$select=*,Name");
            RunTest(baseline4, queryable4, true);

            // selecting entire entity + fields of nav property into entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable5 = from p in context.CreateQuery<BigCity>("BigCities")
                             select new
                             {
                                 BC = p,
                                 Name = p.Name,
                                 SmallStreets = (from s in p.Streets select new Street { StreetName = s.StreetName, NumberOfBlocks = s.NumberOfBlocks }).ToList()
                             };

            var baseline5 = from p in baseLineContext.BigCities
                            select new
                            {
                                BC = new BigCity { Name = p.Name, BigCityID = p.BigCityID, HasBaseballTeam = p.HasBaseballTeam, Population = p.Population, MiscInfo = p.MiscInfo, Mayor = p.Mayor, Streets = null },
                                Name = p.Name,
                                SmallStreets = (from s in p.Streets select new Street { StreetName = s.StreetName, NumberOfBlocks = s.NumberOfBlocks }).ToList()
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable5.ToString(), "$expand=Streets($select=StreetName,NumberOfBlocks)&$select=*,Name");
            RunTest(baseline5, queryable5, true);

            // selecting entire nav prop entity + some fields of nav property
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");

            var queryable6 = from p in context.CreateQuery<BigState>("BigStates")
                             select new
                             {
                                 X = p.TopCity,
                                 Y = p.TopCity.Name,
                                 Z = p.TopCity.Population
                             };

            var baseline6 = from p in baseLineContext.BigStates
                            select new
                            {
                                X = p.TopCity,
                                Y = p.TopCity.Name,
                                Z = p.TopCity.Population
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable6.ToString(), "$expand=TopCity($select=*)");
            RunTest(baseline6, queryable6, true);

            // selecting entire nav prop for collection 
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable7 = from p in context.CreateQuery<BigState>("BigStates")
                             where p.TopCity != null
                             select new
                             {
                                 A = p.TopCity,
                                 B = p.Cities,
                                 C = p.Cities.Select(c => c.Streets).ToList()
                             };

            var baseline7 = from p in baseLineContext.BigStates
                            where p.TopCity != null
                            select new
                            {
                                A = new BigCity { Name = p.TopCity.Name, BigCityID = p.TopCity.BigCityID, HasBaseballTeam = p.TopCity.HasBaseballTeam, Population = p.TopCity.Population, MiscInfo = p.TopCity.MiscInfo, Mayor = p.TopCity.Mayor, Streets = null },
                                B = (from bc in p.Cities select new BigCity { Name = bc.Name, BigCityID = bc.BigCityID, HasBaseballTeam = bc.HasBaseballTeam, Population = bc.Population, MiscInfo = bc.MiscInfo, Mayor = bc.Mayor, Streets = null }).ToList(),
                                C = p.Cities.Select(c => c.Streets).ToList()
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable7.ToString(), "$expand=TopCity, $expand=Cities($expand=Streets)");

            // selecting entire complext type - there should be no *
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable8 = from p in context.CreateQuery<BigCity>("BigCities")
                             select new
                             {
                                 X = p.MiscInfo,
                                 Y = p
                             };

            var baseline8 = from p in baseLineContext.BigCities
                            select new
                            {
                                X = p.MiscInfo,
                                Y = p
                            };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable8.ToString(), "$select=MiscInfo,*");
            RunTest(baseline8, queryable8, true);
        }


        [TestMethod]
        public void LinqV2ProjectionsIllegalCases()
        {
            // illegal case, sequence operator in query - Entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable1 = from b in context.CreateQuery<BigState>("BigStares")
                             select new LittleState { Cities = b.Cities.OrderBy(x => x.Name).Select(y => new LittleCity { }).ToList() };

            VerifyNotSupportedQuery(queryable1, "Initializing instances of the entity type AstoriaUnitTests.LittleState with the expression b.Cities.OrderBy(x => x.Name) is not supported.");

            // illegal case, sequence operator in query - non Entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable2 = from b in context.CreateQuery<BigState>("BigStares")
                             select new { Cities = b.Cities.OrderBy(x => x.Var1.DateTimeOffset).Select(y => new LittleCity { }).ToList() };

            //TODO: need to find way to check error without relying on anonymous type name
            //VerifyNotSupportedQuery(queryable2, "Constructing or initializing instances of the type <>f__AnonymousType15`1[System.Collections.Generic.List`1[AstoriaUnitTests.LittleCity]] with the expression b.Cities.OrderBy(x => x.Var1.DateTime) is not supported.");
            VerifyNotSupportedQuery(queryable2, null);

            // illegal case, calling a constructor of Entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable3 = from b in context.CreateQuery<BigState>("BigStares")
                             select new LittleState(b.ID);

            VerifyNotSupportedQuery(queryable3, "Construction of entity type instances must use object initializer with default constructor.");

            // illegal case, calling a constructor of Entity Type inside construction of non Entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable4 = from b in context.CreateQuery<BigState>("BigStares")
                             select new { a = new LittleState(b.ID) };

            //TODO: need to find way to check error without relying on anonymous type name
            //VerifyNotSupportedQuery(queryables4, "Constructing or initializing instances of the type <>f__AnonymousType16`1[AstoriaUnitTests.LittleState] with the expression new LittleState(b.ID) is not supported.");
            VerifyNotSupportedQuery(queryable4, null);

            // illegal case, referencing constant entity
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable5 = from b in context.CreateQuery<BigState>("BigStates")
                             select new LittleState { };

            VerifyNotSupportedQuery(queryable5, "Referencing of local entity type instances not supported when projecting results.");

            // illegal case, member names don't match
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable6 = from b in context.CreateQuery<BigState>("BigStates")
                             select new LittleState { ID = b.Population };

            VerifyNotSupportedQuery(queryable6, "Cannot assign the value from the Population property to the ID property.  When projecting results into a entity type, the property names of the source type and the target type must match for the properties being projected.");

            // illegal case, member names don't match
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable7 = from b in context.CreateQuery<BigState>("BigStates")
                             from c in b.Cities
                             where c.BigCityID == 1
                             select new { A = b.Population, B = c.HasBaseballTeam };

            VerifyNotSupportedQuery(queryable7, "Can only project the last entity type in the query being translated.");

            // selecting in complex type (illegal)
            var queryable8 = from b in context.CreateQuery<BigCity>("BigCities")
                             select new LittleCity { Name = b.MiscInfo.A };

            VerifyNotSupportedQuery(queryable8, "Initializing instances of the entity type AstoriaUnitTests.LittleCity with the expression b.MiscInfo.A is not supported.");

            // illegal level shift in Entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable9 = from b in context.CreateQuery<BigCity>("BigCities")
                             select new LittleCity
                             {
                                 BigCityID = b.BigCityID,
                                 State = new LittleState { StateName = b.State.TopCity.State.StateName }
                             };

            VerifyNotSupportedQuery(queryable9, "Cannot initialize an instance of entity type 'AstoriaUnitTests.LittleCity' because 'b' and 'b.State.TopCity' do not refer to the same source entity.");

            // cannot project single property with wrapping in type - scalar
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable10 = from b in context.CreateQuery<BigCity>("BigCities")
                              where b.BigCityID == 1
                              select (int)b.Population;

            VerifyNotSupportedQuery(queryable10, "The method 'Select' is not supported.");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // cannot project single property with wrapping in type - entity
            var queryable11 = from b in context.CreateQuery<BigCity>("BigCities")
                              select b.State;

            VerifyNotSupportedQuery(queryable11, DataServicesClientResourceUtil.GetString("ALinq_CantNavigateWithoutKeyPredicate"));

            var queryable12 = from b in context.CreateQuery<BigCity>("BigCities")
                              select b.State.ToString();

            VerifyNotSupportedQuery(queryable12, "The method 'Select' is not supported.");


            // cannot project single property with wrapping in type - collection
            var queryable13 = from b in context.CreateQuery<BigCity>("BigCities")
                              select b.Streets;

            VerifyNotSupportedQuery(queryable13, "The method 'Select' is not supported.");

        }

        // [TestMethod]
        public void LinqV2ProjectionsNonEntityTypes()
        {
            // basic narrowing
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable1 = from s in context.CreateQuery<BigCity>("BigCities")
                             select new { BigCityID = s.BigCityID, Mayor = s.Mayor, Population = s.Population };

            var baseline1 = from s in baseLineContext.BigCities
                            select new { BigCityID = s.BigCityID, Mayor = s.Mayor, Population = s.Population };

            RunTest(baseline1, queryable1, true);

            // multilevel narrow case
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable4 = from s in context.CreateQuery<BigState>("BigStates")
                             select new
                             {
                                 StateName = s.StateName,
                                 TopCity = new { Mayor = s.TopCity.Mayor, Name = s.TopCity.Name, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                 Cities = s.Cities.Select(bc => new { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                             };

            var baseline4 = from s in baseLineContext.BigStates
                            select new
                            {
                                StateName = s.StateName,
                                TopCity = new { Mayor = s.TopCity.Mayor, Name = s.TopCity.Name, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                Cities = s.Cities.Select(bc => new { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                            };

            RunTest(baseline4, queryable4, true);
            ReadOnlyTestContext.ClearBaselineIncludes();


            // with filter and orderby
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "TopCity");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable5 = from s in context.CreateQuery<BigState>("BigStates")
                             where s.Population > 1
                             orderby s.TopCity.Mayor
                             select new
                             {
                                 StateName = s.StateName,
                                 TopCity = new { Mayor = s.TopCity.Mayor, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                 Cities = s.Cities.Select(bc => new { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                             };

            var baseline5 = from s in baseLineContext.BigStates
                            where s.Population > 1
                            orderby s.TopCity.Mayor
                            select new
                            {
                                StateName = s.StateName,
                                TopCity = new { Mayor = s.TopCity.Mayor, Population = s.TopCity.Population, BigCityID = s.TopCity.BigCityID },
                                Cities = s.Cities.Select(bc => new { Mayor = bc.Mayor, Population = bc.Population, BigCityID = bc.BigCityID }).ToList()
                            };

            RunTest(baseline5, queryable5, true);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqV2ProjectionsNonEntityTypesShapes()
        {
            // narrowing case into anonymous type which happens to include key
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable3 = from c in context.CreateQuery<BigCity>("BigCities")
                             select new { BigCityId = c.BigCityID, Population = c.Population };

            var baseline3 = from c in baseLineContext.BigCities
                            select new { BigCityId = c.BigCityID, Population = c.Population };

            RunTest(baseline3, queryable3, true);

        }

        // [TestMethod]
        public void LinqV2ProjectionsWithExpand()
        {
            // Expand Option with entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "CoolestCity");

            var queryable1 = from item in context.CreateQuery<BigState>("BigStates")
                             select new LittleState { ID = item.ID, StateName = item.StateName };

            var baseline1 = from item in baseLineContext.BigStates
                            select new LittleState { ID = item.ID, StateName = item.StateName };

            RunTest(baseline1, queryable1, true);

            // Expand Option with non entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "CoolestCity");

            var queryable2 = from item in context.CreateQuery<BigState>("BigStates")
                             select new { ID = item.ID, StateName = item.StateName };

            var baseline2 = from item in baseLineContext.BigStates
                            select new { ID = item.ID, StateName = item.StateName };

            RunTest(baseline2, queryable2, true);

            // expanding after navigating to collection - entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable3 = from item in context.CreateQuery<BigState>("BigStates")
                             where item.StateName == "Oregon"
                             from bc in item.Cities
                             select new LittleCity { BigCityID = bc.BigCityID, Mayor = bc.Mayor };

            var baseline3 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            from bc in item.Cities
                            select new LittleCity { BigCityID = bc.BigCityID, Mayor = bc.Mayor };

            RunTest(baseline3, queryable3, true);

            // expanding after navigating to collection - non entity Type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable4 = from item in context.CreateQuery<BigState>("BigStates")
                             where item.StateName == "Oregon"
                             from bc in item.Cities
                             select new { bID = bc.BigCityID, A = bc.HasBaseballTeam ?? false, B = bc.Streets };

            var baseline4 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            from bc in item.Cities
                            select new { bID = bc.BigCityID, A = bc.HasBaseballTeam ?? false, B = bc.Streets };

            RunTest(baseline4, queryable4, true);

            // redundant expands - Implicit and Explicit
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Var1");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable5 = from item in context.CreateQuery<BigState>("BigStates")
                             where item.StateName == "Oregon"
                             from bc in item.Cities
                             select new { bID = bc.BigCityID, A = bc.Streets, B = bc.Streets, C = bc.Var1, D = bc.Var1 };

            var baseline5 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            from bc in item.Cities
                            select new { bID = bc.BigCityID, A = bc.Streets, B = bc.Streets, C = bc.Var1, D = bc.Var1 };

            RunTest(baseline5, queryable5, true);

            // redundant expands. - AddQueryOption, Implicit, and Explicit.
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Var1");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable6 = from item in context.CreateQuery<BigState>("BigStates")
                             where item.StateName == "Oregon"
                             from bc in item.Cities
                             select new { bID = bc.BigCityID, A = bc.Streets, B = bc.Streets, C = bc.Var1, D = bc.Var1 };

            var ts = queryable6.ToList();

            var baseline6 = from item in baseLineContext.BigStates
                            where item.StateName == "Oregon"
                            from bc in item.Cities
                            select new { bID = bc.BigCityID, A = bc.Streets, B = bc.Streets, C = bc.Var1, D = bc.Var1 };

            RunTest(baseline6, queryable6, true);
        }

        public delegate string f(long x, string y);
        public delegate string f2(BigCity x);
        public delegate int? f3(long? x, long? y);

        internal int? FuncThatTakesNullables(long? x, long? y)
        {
            return null;
        }

        internal bool FuncThatTakesBigCity(BigCity bc)
        {
            throw new NotSupportedException();
        }

        // [TestMethod]
        public void LinqV2ProjectionsOperatorsInProjection()
        {
            Trace.WriteLine("operations over properties of primitive types in non-Entity Type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            var queryable1 = from c in context.CreateQuery<BigCity>("BigCities")
                             where c.Name == "Salem"
                             select new
                             {
                                 a = c.BigCityID + c.BigCityID,   // add
                                 b = checked(c.BigCityID + c.BigCityID), // add checked
                                 c = c.Name != null && c.State.StateName != null,  // and
                                 d = c.Name != null || c.State.StateName != null,  // or
                                 e = (new string[] { c.Name }[0]), // array init, array index
                                 f = (new string[] { c.Name }).Length, // array length
                                 h = c.Name.EndsWith(c.Name), // method call - object and param.
                                 i = (int)c.BigCityID, // cast
                                 j = c.BigCityID > 0 ? c.Name : c.State.StateName, // conditional
                                 k = 5, // constant
                                 l = (c.BigCityID * c.BigCityID) / c.BigCityID, // multiply, divide
                                 m = checked((checked(c.BigCityID * c.BigCityID)) / c.BigCityID), // multiply, divide checked
                                 n = (c.BigCityID > 1) && (c.BigCityID < 5) && (c.BigCityID <= 4) && (c.BigCityID >= 1), // <, >, <=, >=
                                 o = (object)(c.BigCityID) is long, // type is
                                 p = c.Name as string, // type as
                                 q = c.BigCityID - c.BigCityID, // subtraction
                                 r = checked(c.BigCityID - c.BigCityID), //subtraction checked
                                 s = c.BigCityID % c.BigCityID, // modula
                                 t = checked(c.BigCityID % c.BigCityID), // modula checked
                                 u = -c.BigCityID, // negate
                                 v = checked(-c.BigCityID), // negate checked
                                 w = !c.HasBaseballTeam, // not
                                 x = c.BigCityID == c.BigCityID, // equal
                                 y = c.BigCityID != c.BigCityID, // not equal
                                 z = (int)c.BigCityID, // cast
                                 aa = c.BigCityID & c.BigCityID, // &
                                 bb = c.BigCityID | c.BigCityID, // |
                                 cc = c.BigCityID << 5, // <<
                                 dd = c.BigCityID >> 3, // >>
                                 ee = c.Mayor ?? c.Name // Coalesce
                             };

            var baseline1 = from c in baseLineContext.BigCities
                            where c.Name == "Salem"
                            select new
                            {
                                a = c.BigCityID + c.BigCityID,   // add
                                b = checked(c.BigCityID + c.BigCityID), // add checked
                                c = c.Name != null && c.State != null,  // and
                                d = c.Name != null || c.State != null,  // or
                                e = (new string[] { c.Name, c.Name, c.Name }[c.BigCityID]), // array init, array index
                                f = (new string[] { c.Name }).Length, // array length
                                h = c.Name.EndsWith(c.Name), // method call - object and param.
                                i = (int)c.BigCityID, // cast
                                j = c.BigCityID > 0 ? c.Name : c.State.StateName, // conditional
                                k = 5, // constant
                                l = (c.BigCityID * c.BigCityID) / c.BigCityID, // multiply, divide
                                m = checked((checked(c.BigCityID * c.BigCityID)) / c.BigCityID), // multiply, divide checked
                                n = (c.BigCityID > 1) && (c.BigCityID < 5) && (c.BigCityID <= 4) && (c.BigCityID >= 1), // <, >, <=, >=
                                o = (object)(c.BigCityID) is long, // type is
                                p = c.Name as string, // type as
                                q = c.BigCityID - c.BigCityID, // subtraction
                                r = checked(c.BigCityID - c.BigCityID), //subtraction checked
                                s = c.BigCityID % c.BigCityID, // modula
                                t = checked(c.BigCityID % c.BigCityID), // modula checked
                                u = -c.BigCityID, // negate
                                v = checked(-c.BigCityID), // negate checked
                                w = !c.HasBaseballTeam, // not
                                x = c.BigCityID == c.BigCityID, // equal
                                y = c.BigCityID != c.BigCityID, // not equal
                                z = (int)c.BigCityID, // cast 
                                aa = c.BigCityID & c.BigCityID, // &
                                bb = c.BigCityID | c.BigCityID, // |
                                cc = c.BigCityID << 5, // <<
                                dd = c.BigCityID >> 3, // >>
                                ee = c.Mayor ?? c.Name // Coalesce
                            };

            RunTest(baseline1, queryable1, true);

            Trace.WriteLine("operations over Entity Types (error) in non Entity Type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            f2 func2 = delegate(BigCity bc) { return bc.Mayor; };
            BigCity localBigCity = new BigCity();
            localBigCity.Mayor = "Mr. Smith";
            List<BigCity> cities = new List<BigCity>();

            List<Expression<Func<BigCity, object>>> selectors = new List<Expression<Func<BigCity, object>>>() 
            { 
                bc => new { a = bc + bc },  // addition
                bc => new { a = checked( bc + bc) }, // addition checked
                bc => new { a = bc - bc }, // subtraction
                bc => new { a = checked( bc - bc) }, // subtraction checked
                bc => new { a = bc * bc },  // multiplication
                bc => new { a = checked( bc * bc) }, // multiplication checked
                bc => new { a = bc / bc }, // division
                bc => new { a = checked( bc / bc) }, // division checked
                bc => new { a = bc % bc },  // modula
                bc => new { a = checked( bc % bc) }, // modula checked
                bc => new { a = (LittleCity3)bc }, // cast
                bc => new { a = (BigCityComplexType)bc }, // cast
                bc => new { a = bc ? 1 : 0 }, // conditional
                bc => new { a = true ? bc : null }, // conditional
                bc => new { a = false ? null : bc }, // conditional
                bc => new { a = bc < new BigCity() }, // <
                bc => new { a = new BigCity() <= bc }, // <=
                bc => new { a = new BigCity() < bc }, // >
                bc => new { a = bc <= new BigCity() }, // >=
                bc => new { a = bc is DerivedBigCity }, // type is
                bc => new { a = bc as DerivedBigCity }, // type as
                bc => new { a = - bc  }, // negate
                bc => new { a = checked (-bc)}, // negate checked
                bc => new { a = !bc  }, // not
                bc => new { a = new BigCity() == bc }, // equal
                bc => new { a = bc != new BigCity() }, // not equal
                bc => new { a = bc&bc }, // &
                bc => new { a = bc|bc }, // |
                bc => new { a = bc << 5 }, // <<
                bc => new { a = bc >> 3 }, // >>
                bc => new { a = ~bc }, // ~
                bc => new { a = bc.GetType() }, // method call
                bc => new { a = FuncThatTakesBigCity(bc) },  // parameter to method
                bc => new { a = func2(bc) }, // delegate
                bc => new { a = bc ?? bc } // ??
            };

            object queryable2 = null;
            bool passed = false;

            foreach (var e in selectors)
            {
                queryable2 = context.CreateQuery<BigCity>("BigCities").Select(e);

                try
                {
                    passed = false;
                    ((IEnumerable)queryable2).GetEnumerator();
                }
                catch (NotSupportedException ex)
                {
                    if (ex.Message == "Projection not supported") //TODO, put in individual error messages
                    {
                    }
                    passed = true;
                }

                if (!passed)
                    throw new Exception("Test Failed");

            }

            Trace.WriteLine("operations over Entity Types (error) in known Entity Type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            List<Expression<Func<BigCity, DummyEntity>>> selectors2 = new List<Expression<Func<BigCity, DummyEntity>>>() 
            { 
                bc => new DummyEntity{ a = bc + bc },  // addition
                bc => new DummyEntity{ a = checked( bc + bc) }, // addition checked
                bc => new DummyEntity{ a = bc - bc }, // subtraction
                bc => new DummyEntity{ a = checked( bc - bc) }, // subtraction checked
                bc => new DummyEntity{ a = bc * bc },  // multiplication
                bc => new DummyEntity{ a = checked( bc * bc) }, // multiplication checked
                bc => new DummyEntity{ a = bc / bc }, // division
                bc => new DummyEntity{ a = checked( bc / bc) }, // division checked
                bc => new DummyEntity{ a = bc % bc },  // modula
                bc => new DummyEntity{ a = checked( bc % bc) }, // modula checked
                bc => new DummyEntity{ a = (LittleCity3)bc }, // cast
                bc => new DummyEntity{ a = (BigCityComplexType)bc }, // cast
                bc => new DummyEntity{ a = bc ? 1 : 0 }, // conditional
                bc => new DummyEntity{ a = true ? bc : null }, // conditional
                bc => new DummyEntity{ a = false ? null : bc }, // conditional
                bc => new DummyEntity{ a = bc < new BigCity() }, // <
                bc => new DummyEntity{ a = new BigCity() <= bc }, // <=
                bc => new DummyEntity{ a = new BigCity() < bc }, // >
                bc => new DummyEntity{ a = bc <= new BigCity() }, // >=
                bc => new DummyEntity{ a = bc is DerivedBigCity }, // type is
                bc => new DummyEntity{ a = bc as DerivedBigCity }, // type as
                bc => new DummyEntity{ a = - bc  }, // negate
                bc => new DummyEntity{ a = checked (-bc)}, // negate checked
                bc => new DummyEntity{ a = !bc  }, // not
                bc => new DummyEntity{ a = new BigCity() == bc }, // equal
                bc => new DummyEntity{ a = bc != new BigCity() }, // not equal
                bc => new DummyEntity{ a = bc&bc }, // &
                bc => new DummyEntity{ a = bc|bc }, // |
                bc => new DummyEntity{ a = bc << 5 }, // <<
                bc => new DummyEntity{ a = bc >> 3 }, // >>
                bc => new DummyEntity{ a = ~bc }, // ~
                bc => new DummyEntity{ a = bc.GetType() }, // method call
                bc => new DummyEntity{ a = FuncThatTakesBigCity(bc) },  // parameter to method
                bc => new DummyEntity{ a = func2(bc) },  // parameter to delegate
                bc => new DummyEntity{ a = bc ?? bc.State.TopCity } // ??
            };

            passed = false;

            foreach (var e in selectors2)
            {
                var queryable2a = context.CreateQuery<BigCity>("BigCities").Select(e);

                try
                {
                    passed = false;
                    ((IEnumerable)queryable2a).GetEnumerator();
                }
                catch (NotSupportedException ex)
                {
                    if (ex.Message == "Projection not supported") //TODO, put in individual error messages
                    { }
                    passed = true;
                }

                if (!passed)
                    throw new Exception("Test Failed");

            }

            Trace.WriteLine("operations over primitives (error) in known Entity Type");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            f func = delegate(long xx, string yy) { return xx.ToString() + yy; };

            List<Expression<Func<BigCity, DummyEntity>>> selectors3 = new List<Expression<Func<BigCity, DummyEntity>>>() 
            { 
                bc => new DummyEntity{ a = bc.BigCityID + bc.BigCityID },  // addition
                bc => new DummyEntity{ a = checked( bc.BigCityID + bc.BigCityID) }, // addition checked
                bc => new DummyEntity{ a = bc.BigCityID - bc.BigCityID }, // subtraction
                bc => new DummyEntity{ a = checked( bc.BigCityID - bc.BigCityID) }, // subtraction checked
                bc => new DummyEntity{ a = bc.BigCityID * bc.BigCityID },  // multiplication
                bc => new DummyEntity{ a = checked( bc.BigCityID * bc.BigCityID) }, // multiplication checked
                bc => new DummyEntity{ a = bc.BigCityID / bc.BigCityID }, // division
                bc => new DummyEntity{ a = checked( bc.BigCityID / bc.BigCityID) }, // division checked
                bc => new DummyEntity{ a = bc.BigCityID % bc.BigCityID },  // modula
                bc => new DummyEntity{ a = checked( bc.BigCityID % bc.BigCityID) }, // modula checked
                bc => new DummyEntity{ a = (bool) bc.HasBaseballTeam ? 1 : 0 }, // conditional
                bc => new DummyEntity{ a = true ? bc.HasBaseballTeam : null }, // conditional
                bc => new DummyEntity{ a = false ? null : bc.HasBaseballTeam }, // conditional
                bc => new DummyEntity{ a = bc.BigCityID < new BigCity().BigCityID }, // <
                bc => new DummyEntity{ a = new BigCity().BigCityID <= bc.BigCityID }, // <=
                bc => new DummyEntity{ a = new BigCity().BigCityID < bc.BigCityID }, // >
                bc => new DummyEntity{ a = bc.BigCityID <= new BigCity().BigCityID }, // >=
                bc => new DummyEntity{ a = (object) bc.BigCityID is short }, // type is
                bc => new DummyEntity{ a = - bc.BigCityID  }, // negate
                bc => new DummyEntity{ a = checked (-bc.BigCityID)}, // negate checked
                bc => new DummyEntity{ a = !bc.HasBaseballTeam  }, // not
                bc => new DummyEntity{ a = new BigCity().BigCityID == bc.BigCityID }, // equal
                bc => new DummyEntity{ a = bc.HasBaseballTeam != new BigCity().HasBaseballTeam }, // not equal
                bc => new DummyEntity{ a = bc.BigCityID&bc.BigCityID }, // &
                bc => new DummyEntity{ a = bc.BigCityID|bc.BigCityID }, // |
                bc => new DummyEntity{ a = bc.BigCityID << 5 }, // <<
                bc => new DummyEntity{ a = bc.BigCityID >> 3 }, // >>
                bc => new DummyEntity{ a = ~bc.BigCityID }, // ~
                bc => new DummyEntity{ a = bc.BigCityID.GetType() }, // method call
                bc => new DummyEntity{ a = string.Concat(bc.Mayor, bc.Mayor) },  // parameter to method
                bc => new DummyEntity{ a = func(bc.BigCityID, bc.Mayor)},  // parameter to delegate
                bc => new DummyEntity{ a = bc.Name ?? bc.Mayor } // ??
            };

            passed = false;

            foreach (var e in selectors3)
            {
                var queryable2b = context.CreateQuery<BigCity>("BigCities").Select(e);

                try
                {
                    passed = false;
                    ((IEnumerable)queryable2b).GetEnumerator();
                }
                catch (NotSupportedException ex)
                {
                    if (ex.Message == "Projection not supported") //TODO, put in individual error messages
                    { }
                    passed = true;
                }

                if (!passed)
                    throw new Exception("Test Failed");

            }

            Trace.WriteLine("non entity type - referecing locals, constants in the closure, Invocation - no paths for primitives, complex types.");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "State");

            int x = 1;

            var queryable4 = from c in context.CreateQuery<BigCity>("BigCities")
                             where c.Name == "Salem"
                             select new
                             {
                                 a = "constant string",   // constant string
                                 b = x, // local
                                 c = x + x + c.BigCityID,  // local 
                                 d = new DateTime().Year,  // create non-entity
                                 e = func(c.BigCityID, c.Name)  // invoke
                             };

            var baseline4 = from c in baseLineContext.BigCities
                            where c.Name == "Salem"
                            select new
                            {
                                a = "constant string",   // constant string
                                b = x, // local
                                c = x + x + c.BigCityID,  // local 
                                d = new DateTime().Year,  // create non-entity
                                e = func(c.BigCityID, c.Name)  // invoke
                            };

            RunTest(baseline4, queryable4, true);

            Trace.WriteLine("operators over nullables, IEnum. in non entity type");
            ReadOnlyTestContext.ClearBaselineIncludes();

            f3 func3 = FuncThatTakesNullables;

            var queryable6 = from c in context.CreateQuery<BigCityVar1>("BigCityVar1")
                             select new
                             {
                                 nid = c.ID,
                                 a = c.NullableInt + c.NullableInt,   // binary operation over nullable
                                 b = -c.NullableLong, // unary operation over nullable
                                 c = FuncThatTakesNullables(c.NullableLong, c.NullableLong), // method call
                                 d = func3(c.NullableLong, c.NullableLong), // delegate invocation
                                 e = c.NullableInt == null ? c.NullableBoolean : !c.NullableBoolean // conditional
                             };


            var baseline6 = from c in baseLineContext.BigCityVar1
                            select new
                            {
                                nid = c.ID,
                                a = c.NullableInt + c.NullableInt,   // binary operation over nullable
                                b = -c.NullableLong, // unary operation over nullable
                                c = FuncThatTakesNullables(c.NullableLong, c.NullableLong), // method call
                                d = func3(c.NullableLong, c.NullableLong), // delegate invocation
                                e = c.NullableInt == null ? c.NullableBoolean : !c.NullableBoolean // conditional
                            };

            RunTest(baseline6, queryable6, true);

            {
                Trace.WriteLine("legal operations for entities in non entity type");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

                // b is commented out because it conflicts with 'a' at change tracking time
                var queryable7 = from s in context.CreateQuery<BigState>("BigStates")
                                 select new
                                 {
                                     nid = s.StateName,
                                     a = s.Cities.ToList(), //ToList
                                     //b = s.Cities.Select(c => new LittleCity { BigCityID = c.BigCityID }), // Select
                                     c = localBigCity.Mayor.Length, // referencing local + member access + methodCall.
                                 };

                var baseline7 = from s in baseLineContext.BigStates
                                select new
                                {
                                    nid = s.StateName,
                                    a = s.Cities.ToList(),  // ToList
                                    //b = s.Cities.Select(c => new LittleCity { BigCityID = c.BigCityID }), // Select
                                    c = localBigCity.Mayor.Length, // referencing local + member access + methodCall.
                                };

                RunTest(baseline7, queryable7, true);
            }

            {
                Trace.WriteLine("legal operations for entities in non entity type");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

                // a is commented out because it conflicts with 'b' at change tracking time
                var queryable7 = from s in context.CreateQuery<BigState>("BigStates")
                                 select new
                                 {
                                     nid = s.StateName,
                                     //a = s.Cities.ToList(), //ToList
                                     b = s.Cities.Select(c => new LittleCity { BigCityID = c.BigCityID }), // Select
                                     c = localBigCity.Mayor.Length, // referencing local + member access + methodCall.
                                 };

                var baseline7 = from s in baseLineContext.BigStates
                                select new
                                {
                                    nid = s.StateName,
                                    //a = s.Cities.ToList(),  // ToList
                                    b = s.Cities.Select(c => new LittleCity { BigCityID = c.BigCityID }), // Select
                                    c = localBigCity.Mayor.Length, // referencing local + member access + methodCall.
                                };

                RunTest(baseline7, queryable7, true);
            }

            Trace.WriteLine("operations over complex types in non-entity type");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable8 = from c in context.CreateQuery<BigCity>("BigCities")
                             where c.Name == "Salem"
                             select new
                             {
                                 nid = c.BigCityID,
                                 a = c.MiscInfo + c.MiscInfo,   // add
                                 b = checked(c.MiscInfo + c.MiscInfo), // add checked
                                 c = c.MiscInfo != null && c.MiscInfo == null,  // and
                                 d = c.MiscInfo == null || c.MiscInfo != null,  // or
                                 e = (new BigCityComplexType[] { c.MiscInfo }[0]), // array init, array index
                                 f = (new BigCityComplexType[] { c.MiscInfo }).Length, // array length
                                 h = c.MiscInfo.GetType(), // method call
                                 i = (int)c.MiscInfo, // cast
                                 j = c.MiscInfo > new BigCityComplexType() ? c.MiscInfo == null : c.MiscInfo != null, // conditional
                                 k = new BigCityComplexType(), // constant
                                 l = (c.MiscInfo * c.MiscInfo) / c.MiscInfo, // multiply, divide
                                 m = checked((checked(c.MiscInfo * c.MiscInfo)) / c.MiscInfo), // multiply, divide checked
                                 n = (c.MiscInfo > c.MiscInfo) && (c.MiscInfo < c.MiscInfo) && (c.MiscInfo <= c.MiscInfo) && (c.MiscInfo >= c.MiscInfo), // <, >, <=, >=
                                 o = c.MiscInfo is object, // type is
                                 p = c.MiscInfo as BigCityComplexType2, // type as
                                 q = c.MiscInfo - c.MiscInfo, // subtraction
                                 r = checked(c.MiscInfo - c.MiscInfo), //subtraction checked
                                 s = c.MiscInfo % c.MiscInfo, // modula
                                 t = checked(c.MiscInfo % c.MiscInfo), // modula checked
                                 u = -c.MiscInfo, // negate
                                 v = checked(-c.MiscInfo), // negate checked
                                 w = !c.MiscInfo, // not
                                 x = c.MiscInfo == c.MiscInfo, // equal
                                 y = c.MiscInfo != c.MiscInfo, // not equal
                                 z = (int)c.MiscInfo, // cast
                                 aa = c.MiscInfo & c.MiscInfo, // &
                                 bb = c.MiscInfo | c.MiscInfo, // |
                                 cc = c.MiscInfo << 5, // <<
                                 dd = c.MiscInfo >> 3, // >>
                                 ee = c.MiscInfo ?? c.MiscInfo // Coalesce
                             };

            var baseline8 = from c in baseLineContext.BigCities
                            where c.Name == "Salem"
                            select new
                            {
                                nid = c.BigCityID,
                                a = c.MiscInfo + c.MiscInfo,   // add
                                b = checked(c.MiscInfo + c.MiscInfo), // add checked
                                c = c.MiscInfo != null && c.MiscInfo == null,  // and
                                d = c.MiscInfo == null || c.MiscInfo != null,  // or
                                e = (new BigCityComplexType[] { c.MiscInfo }[0]), // array init, array index
                                f = (new BigCityComplexType[] { c.MiscInfo }).Length, // array length
                                h = c.MiscInfo.GetType(), // method call
                                i = (int)c.MiscInfo, // cast
                                j = c.MiscInfo > new BigCityComplexType() ? c.MiscInfo == null : c.MiscInfo != null, // conditional
                                k = new BigCityComplexType(), // constant
                                l = (c.MiscInfo * c.MiscInfo) / c.MiscInfo, // multiply, divide
                                m = checked((checked(c.MiscInfo * c.MiscInfo)) / c.MiscInfo), // multiply, divide checked
                                n = (c.MiscInfo > c.MiscInfo) && (c.MiscInfo < c.MiscInfo) && (c.MiscInfo <= c.MiscInfo) && (c.MiscInfo >= c.MiscInfo), // <, >, <=, >=
                                o = c.MiscInfo is object, // type is
                                p = c.MiscInfo as BigCityComplexType2, // type as
                                q = c.MiscInfo - c.MiscInfo, // subtraction
                                r = checked(c.MiscInfo - c.MiscInfo), //subtraction checked
                                s = c.MiscInfo % c.MiscInfo, // modula
                                t = checked(c.MiscInfo % c.MiscInfo), // modula checked
                                u = -c.MiscInfo, // negate
                                v = checked(-c.MiscInfo), // negate checked
                                w = !c.MiscInfo, // not
                                x = c.MiscInfo == c.MiscInfo, // equal
                                y = c.MiscInfo != c.MiscInfo, // not equal
                                z = (int)c.MiscInfo, // cast
                                aa = c.MiscInfo & c.MiscInfo, // &
                                bb = c.MiscInfo | c.MiscInfo, // |
                                cc = c.MiscInfo << 5, // <<
                                dd = c.MiscInfo >> 3, // >>
                                ee = c.MiscInfo ?? c.MiscInfo // Coalesce
                            };

            RunTest(baseline8, queryable8, true);

            Trace.WriteLine("operations over complex types in entity types - always error.");
            ReadOnlyTestContext.ClearBaselineIncludes();

            List<Expression<Func<BigCity, DummyEntity>>> selectors4 = new List<Expression<Func<BigCity, DummyEntity>>>() 
            { 
                bc => new DummyEntity{ a = bc.MiscInfo + bc.MiscInfo },  // addition
                bc => new DummyEntity{ a = checked( bc.MiscInfo + bc.MiscInfo) }, // addition checked
                bc => new DummyEntity{ a = bc.MiscInfo - bc.MiscInfo }, // subtraction
                bc => new DummyEntity{ a = checked( bc.MiscInfo - bc.MiscInfo) }, // subtraction checked
                bc => new DummyEntity{ a = bc.MiscInfo * bc.MiscInfo },  // multiplication
                bc => new DummyEntity{ a = checked( bc.MiscInfo * bc.MiscInfo) }, // multiplication checked
                bc => new DummyEntity{ a = bc.MiscInfo / bc.MiscInfo }, // division
                bc => new DummyEntity{ a = checked( bc.MiscInfo / bc.MiscInfo) }, // division checked
                bc => new DummyEntity{ a = bc.MiscInfo % bc.MiscInfo },  // modula
                bc => new DummyEntity{ a = checked( bc.MiscInfo % bc.MiscInfo) }, // modula checked
                bc => new DummyEntity{ a = bc.MiscInfo ? 1 : 0 }, // conditional
                bc => new DummyEntity{ a = true ? bc.MiscInfo : null }, // conditional
                bc => new DummyEntity{ a = false ? null : bc.MiscInfo }, // conditional
                bc => new DummyEntity{ a = bc.MiscInfo < new BigCityComplexType() }, // <
                bc => new DummyEntity{ a = new BigCityComplexType() <= bc.MiscInfo }, // <=
                bc => new DummyEntity{ a = new BigCityComplexType() < bc.MiscInfo }, // >
                bc => new DummyEntity{ a = bc.MiscInfo <= new BigCityComplexType() }, // >=
                bc => new DummyEntity{ a = bc.MiscInfo is BigCityComplexType2 }, // type is
                bc => new DummyEntity{ a = bc.MiscInfo as BigCityComplexType2},
                bc => new DummyEntity{ a = - bc.MiscInfo }, // negate
                bc => new DummyEntity{ a = checked (-bc.MiscInfo)}, // negate checked
                bc => new DummyEntity{ a = !bc.MiscInfo  }, // not
                bc => new DummyEntity{ a = bc.MiscInfo == bc.MiscInfo }, // equal
                bc => new DummyEntity{ a = bc.MiscInfo != bc.MiscInfo }, // not equal
                bc => new DummyEntity{ a = bc.MiscInfo & bc.MiscInfo }, // &
                bc => new DummyEntity{ a = bc.MiscInfo | bc.MiscInfo }, // |
                bc => new DummyEntity{ a = bc.MiscInfo << 5 }, // <<
                bc => new DummyEntity{ a = bc.MiscInfo >> 3 }, // >>
                bc => new DummyEntity{ a = ~bc.MiscInfo }, // ~
                bc => new DummyEntity{ a = bc.MiscInfo.GetType() }, // method call
                bc => new DummyEntity{ a = bc.MiscInfo ?? bc.MiscInfo } // ??
            };

            passed = false;

            foreach (var e in selectors4)
            {
                var queryable9 = context.CreateQuery<BigCity>("BigCities").Select(e);

                try
                {
                    passed = false;
                    ((IEnumerable)queryable9).GetEnumerator();
                }
                catch (NotSupportedException ex)
                {
                    if (ex.Message == "Projection not supported") //TODO, put in individual error messages
                    { }
                    passed = true;
                }

                if (!passed)
                    throw new Exception("Test Failed");

            }

            // true constants - non Entity Types, entire expression becomes constant
            ReadOnlyTestContext.ClearBaselineIncludes();

            BigCityComplexType bcct = new BigCityComplexType();
            IEnumerable<BigCityComplexType> sequence = new List<BigCityComplexType>() { bcct };

            var queryable10 = from s in context.CreateQuery<BigCity>("BigCities")
                              select new
                              {
                                  a = new BigCityComplexType(),
                                  b = new DateTime(),
                                  c = bcct,
                                  d = bcct.A,
                                  e = sequence,
                                  f = new BigCity(),
                                  g = localBigCity,
                                  h = localBigCity.Mayor,
                                  //i = sequence.Where(ct => ct.B == bcct.B).Reverse()  //TODO: need to investigate why this is not being funcletized
                              };


            var baseline10 = from s in baseLineContext.BigCities
                             select new
                              {
                                  a = new BigCityComplexType(),
                                  b = new DateTime(),
                                  c = bcct,
                                  d = bcct.A,
                                  e = sequence,
                                  f = new BigCity(),
                                  g = localBigCity,
                                  h = localBigCity.Mayor,
                                  //i = sequence.Where(ct => ct.B == bcct.B).Reverse()
                              };

            RunTest(baseline10, queryable10, true);

            Trace.WriteLine("true contants.  Known entity types are always errors.");
            ReadOnlyTestContext.ClearBaselineIncludes();

            List<Expression<Func<BigCity, DummyEntity>>> selectors5 = new List<Expression<Func<BigCity, DummyEntity>>>() 
            { 
                bc => new DummyEntity(),  // constant
                bc => new DummyEntity{ a = new BigCity(), ID = (int)bc.BigCityID}, // entity constant
                bc => new DummyEntity{ a = localBigCity, ID = (int)bc.BigCityID}, // referecing local constant
                bc => new DummyEntity{ a = cities, ID = (int)bc.BigCityID}, // referecing local constant of IEnum entity type
            };

            passed = false;

            foreach (var e in selectors5)
            {
                var queryable11 = context.CreateQuery<BigCity>("BigCities").Select(e);

                try
                {
                    passed = false;
                    ((IEnumerable)queryable11).GetEnumerator();
                }
                catch (NotSupportedException ex)
                {
                    if (ex.Message == "Projection not supported")  //TODO, put in individual error messages
                    { }
                    passed = true;
                }

                if (!passed)
                    throw new Exception("Test Failed");
            }
        }

        // [TestMethod]
        public void LinqV2ProjectionsTransparentIdentifiers()
        {
            // basic TransparentIdentifier
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable1 = from s in context.CreateQuery<BigState>("BigStates")
                             where s.StateName == "Oregon"
                             from c in s.Cities
                             where c.BigCityID == 1
                             select new LittleCity3() { Mayor = c.Mayor, Population = (int)c.Population };

            var baseline1 = from s in baseLineContext.BigStates
                            where s.StateName == "Oregon"
                            from c in s.Cities
                            where c.BigCityID == 1
                            select new LittleCity3() { Mayor = c.Mayor, Population = (int)c.Population };

            RunTest(baseline1, queryable1, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

            // alternative syntax - multiple froms
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable2 = from s in context.CreateQuery<BigState>("BigStates")
                             from c in s.Cities
                             where s.StateName == "Oregon"
                             where c.BigCityID == 1
                             select new LittleCity3() { Mayor = c.Mayor, Population = (int)c.Population };

            var baseline2 = from s in baseLineContext.BigStates
                            from c in s.Cities
                            where s.StateName == "Oregon"
                            where c.BigCityID == 1
                            select new LittleCity3() { Mayor = c.Mayor, Population = (int)c.Population };

            RunTest(baseline2, queryable2, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

            // into non entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable3 = from s in context.CreateQuery<BigState>("BigStates")
                             where s.StateName == "Oregon"
                             from c in s.Cities
                             where c.BigCityID == 1
                             select new { Mayor = c.Mayor, Population = (int)c.Population };

            var baseline3 = from s in baseLineContext.BigStates
                            where s.StateName == "Oregon"
                            from c in s.Cities
                            where c.BigCityID == 1
                            select new { Mayor = c.Mayor, Population = (int)c.Population };

            RunTest(baseline3, queryable3, true);
            ReadOnlyTestContext.ClearBaselineIncludes();


            // non-Entity Type can't rewrite projection, error case
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable4 = from s in context.CreateQuery<BigState>("BigStates")
                             where s.StateName == "Oregon"
                             from c in s.Cities
                             where c.BigCityID == 1
                             select new { Mayor = c.Mayor, Population = (int)c.Population, StateName = s.StateName };

            bool passed = false;

            try
            {
                queryable4.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Can only project the last entity type in the query being translated.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();
            // Entity Type can't rewrite projection, error case
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");

            var queryable5 = from s in context.CreateQuery<BigState>("BigStates")
                             where s.StateName == "Oregon"
                             from c in s.Cities
                             where c.BigCityID == 1
                             select new LittleCity { Mayor = c.Mayor, Population = (int)c.Population, Name = s.StateName };

            passed = false;

            try
            {
                queryable5.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Can only project the last entity type in the query being translated.")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();


            // Multiple level TransparentIdentifier - known entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable6 = from s in context.CreateQuery<BigState>("BigStates")
                             from c in s.Cities
                             where s.StateName == "Oregon"
                             where c.BigCityID == 1
                             from st in c.Streets
                             select new NarrowStreet { StreetName = st.StreetName, NumberOfBlocks = st.NumberOfBlocks };

            var baseline6 = from s in baseLineContext.BigStates
                            from c in s.Cities
                            where s.StateName == "Oregon"
                            where c.BigCityID == 1
                            from st in c.Streets
                            select new NarrowStreet { StreetName = st.StreetName, NumberOfBlocks = st.NumberOfBlocks };

            RunTest(baseline6, queryable6, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

            // Multiple level TransparentIdentifier - non entity type
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable7 = from s in context.CreateQuery<BigState>("BigStates")
                             from c in s.Cities
                             where s.StateName == "Oregon"
                             where c.BigCityID == 1
                             from st in c.Streets
                             select new { StreetName = st.StreetName, NumberOfBlocks = st.NumberOfBlocks, A = 5 };

            var baseline7 = from s in baseLineContext.BigStates
                            from c in s.Cities
                            where s.StateName == "Oregon"
                            where c.BigCityID == 1
                            from st in c.Streets
                            select new { StreetName = st.StreetName, NumberOfBlocks = st.NumberOfBlocks, A = 5 };

            RunTest(baseline7, queryable7, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

            // TransparentIdentifier with query options
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigCity), "Streets");

            var queryable8 = from x in
                                 (from s in context.CreateQuery<BigState>("BigStates")
                                  from c in s.Cities
                                  where s.StateName == "Oregon"
                                  where c.BigCityID == 1
                                  from st in c.Streets
                                  where st.NumberOfBlocks > 100 && st.NumberOfBlocks < 200
                                  orderby st.StreetName
                                  select st).Take(1)
                             select new NarrowStreet { StreetName = x.StreetName, NumberOfBlocks = x.NumberOfBlocks };

            var baseline8 = from x in
                                (from s in baseLineContext.BigStates
                                 from c in s.Cities
                                 where s.StateName == "Oregon"
                                 where c.BigCityID == 1
                                 from st in c.Streets
                                 where st.NumberOfBlocks > 100 && st.NumberOfBlocks < 200
                                 orderby st.StreetName
                                 select st).Take(1)
                            select new NarrowStreet { StreetName = x.StreetName, NumberOfBlocks = x.NumberOfBlocks };

            RunTest(baseline8, queryable8, true);
            ReadOnlyTestContext.ClearBaselineIncludes();

        }

        // [TestMethod]
        public void LinqV2ProjectionsRegressions()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable = from t in context.CreateQuery<Team>("Teams")
                            where ReturnBool()
                            orderby ReturnString("foo")
                            select new { Id = ReturnInt(96) };


            var baseline = from t in baseLineContext.Teams
                           where ReturnBool()
                           orderby ReturnString("foo")
                           select new { Id = ReturnInt(96) };

            RunTest(baseline, queryable);

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable2 = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                             where t.NullableDateTimeOffset != null
                             select new
                             {
                                 a = t.NullableDateTimeOffset.Value,
                                 b = t.NullableDecimal.Value,
                                 c = t.DateTimeOffset.Year,
                                 d = t.String.Length,
                                 e = t.Int.In(5),
                                 f = t.Long.GetType()
                             };


            var baseline2 = from t in baseLineContext.Var1
                            where t.NullableDateTimeOffset != null
                            select new
                            {
                                a = t.NullableDateTimeOffset.Value,
                                b = t.NullableDecimal.Value,
                                c = t.DateTimeOffset.Year,
                                d = t.String.Length,
                                e = t.Int.In(5),
                                f = t.Long.GetType()
                            };

            RunTest(baseline2, queryable2);

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable3 = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                             where (string)(object)(t.String) != "foo"
                             select new
                             {
                                 a = (int)(t.Float * 100) + "% Off",
                                 b = (object)(t.Int)
                             };


            var baseline3 = from t in baseLineContext.Var1
                            where (string)(object)(t.String) != "foo"
                            select new
                            {
                                a = (int)(t.Float * 100) + "% Off",
                                b = (object)(t.Int)
                            };

            RunTest(baseline3, queryable3);

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable4 = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                             select new YetAnotherAllPrimitiveTypesNotEntityType
                             {
                                 Boolean = t.Boolean,
                                 DateTimeOffset = t.DateTimeOffset
                             };


            var baseline4 = from t in baseLineContext.Var1
                            select new YetAnotherAllPrimitiveTypesNotEntityType
                            {
                                Boolean = t.Boolean,
                                DateTimeOffset = t.DateTimeOffset
                            };

            RunTest(baseline4, queryable4);

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable5 = ((DataServiceQuery<YetAnotherAllPrimitiveTypesNotEntityType>)from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                                                                                          where t.Boolean
                                                                                          orderby t.Int
                                                                                          select new YetAnotherAllPrimitiveTypesNotEntityType
                                                                                          {
                                                                                              Boolean = t.Boolean,
                                                                                              DateTimeOffset = t.DateTimeOffset
                                                                                          }).AddQueryOption("TestQueryOption", "TestValue");

            if (!queryable5.ToString().Contains("TestQueryOption=TestValue"))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();
            var queryable6 = (from s in context.CreateQuery<BigState>("BigStates")
                              where s.StateName == "Oregon"
                              select new
                              {
                                  f = s,
                                  b = s.Population
                              }).Select(p => new { p });

            bool passed = false;

            try { queryable6.GetEnumerator(); }
            catch (NotSupportedException nse)
            {
                Assert.AreEqual(nse.Message, "Cannot translate multiple Linq Select operations in a single 'select' query option.");
                passed = true;
            }

            Assert.AreEqual(passed, true);

            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable7 = from t in context.CreateQuery<Team>("Teams")
                             select new
                             {
                                 Field1 = (int?)t.TeamID
                             };


            var baseline7 = from t in baseLineContext.Teams
                            select new
                            {
                                Field1 = (int?)t.TeamID
                            };

            RunTest(baseline7, queryable7);

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        [TestMethod]
        public void LinqV2Projections()
        {
            // projecting entity collection into non-entity type
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable = from s in context.CreateQuery<BigState>("States")
                            where s.StateName != "Vermont"
                            orderby s.Population
                            select new { Navigation = new DataServiceCollection<BigCity>(s.Cities, TrackingMode.None).Continuation };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable.ToString(), "with the expression new DataServiceCollection`1(s.Cities, None).Continuation is not supported");

            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable2 = from s in context.CreateQuery<BigState>("States")
                             where s.StateName != "Vermont"
                             orderby s.Population
                             select new { Navigation = new DataServiceCollection<BigCity>(s.Cities.ToList(), TrackingMode.None).Continuation };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable2.ToString(), "with the expression new DataServiceCollection`1(s.Cities.ToList(), None).Continuation is not supported");

            var queryable3 = from s in context.CreateQuery<BigState>("States")
                             where s.StateName != "Vermont"
                             orderby s.Population
                             select new { Navigation = new DataServiceCollection<BigCity>(s.Cities, TrackingMode.None).Count };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable3.ToString(), "with the expression new DataServiceCollection`1(s.Cities, None).Count is not supported");

            // method
            var queryable4 = from s in context.CreateQuery<BigState>("States")
                             where s.StateName != "Vermont"
                             orderby s.Population
                             select new { Navigation = new DataServiceCollection<BigCity>(s.Cities, TrackingMode.None).GetEnumerator() };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable4.ToString(), "with the expression new DataServiceCollection`1(s.Cities, None).GetEnumerator() is not supported");

            // sequence method
            var queryable5 = from s in context.CreateQuery<BigState>("States")
                             where s.StateName != "Vermont"
                             orderby s.Population
                             select new { Navigation = new DataServiceCollection<BigCity>(s.Cities, TrackingMode.None).Distinct() };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable5.ToString(), "with the expression new DataServiceCollection`1(s.Cities, None).Distinct() is not supported");

            // projecting into entity
            var queryable6 = from s in context.CreateQuery<BigState>("States")
                             where s.StateName != "Vermont"
                             orderby s.Population
                             select new BigState { Cities = new DataServiceCollection<BigCity>(s.Cities, TrackingMode.None).ToList() };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable6.ToString(), "with the expression new DataServiceCollection`1(s.Cities, None).ToList() is not supported");

            // projecting non-entity type collection
            var queryable7 = from s in context.CreateQuery<BigState>("States")
                             where s.StateName != "Vermont"
                             orderby s.Population
                             select new { X = new DataServiceCollection<BigCityComplexType>(new BigCityComplexType[] { s.CoolestCity.MiscInfo }, TrackingMode.None).ToList() };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable7.ToString(), "with the expression new DataServiceCollection`1(new [] {s.CoolestCity.MiscInfo}, None).ToList() is not supported");

            // projecting non-entity type collection
            var queryable8 = from s in context.CreateQuery<BigState>("States")
                             where s.StateName != "Vermont"
                             orderby s.Population
                             select new
                             {
                                 X = (new MiscList(s.CoolestCity.MiscInfo)).ToList()
                             };

            System.Data.Test.Astoria.TestUtil.AssertContains(queryable8.ToString(), "with the expression new MiscList(s.CoolestCity.MiscInfo).ToList() is not supported.");


            // TODO: enumerate expression types.
        }

        public class MiscList : List<BigCityComplexType>
        {
            public MiscList(BigCityComplexType mi)
            {
            }
        }

        // [TestMethod]
        public void LinqTypeIs()
        {
            // TypeIs over member

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            System.Data.Test.Astoria.TestUtil.TraceScopeForException("LinqTypeIs - is", delegate()
            {
                var query = from t in context.CreateQuery<Team>("Teams")
                            where t.HomeStadium is Stadium
                            select t;


                var baseline = from t in baseLineContext.Teams
                               where t.HomeStadium is Stadium
                               select t;

                Trace.WriteLine(new System.Net.WebClient().DownloadString(query.ToString()));

                RunTest(baseline, ((DataServiceQuery<Team>)query).Expand("HomeStadium"));
            });

            ReadOnlyTestContext.ClearBaselineIncludes();

            // TypeIs over iterator

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            System.Data.Test.Astoria.TestUtil.TraceScopeForException("LinqTypeIs - not is", delegate()
            {
                var query2 = from t in context.CreateQuery<Team>("Teams")
                             where !(t is AllStarTeam)
                             select t;


                var baseline2 = from t in baseLineContext.Teams
                                where !(t is AllStarTeam)
                                select t;

                Trace.WriteLine(new System.Net.WebClient().DownloadString(query2.ToString()));

                RunTest(baseline2, ((DataServiceQuery<Team>)query2).Expand("HomeStadium"));
            });

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqConvert()
        {
            // cast member

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var query = from t in context.CreateQuery<Team>("Teams")
                        where t.HomeStadium != null &&
                        ((Stadium)t.HomeStadium).Capacity > 40000
                        select t;


            var baseline = from t in baseLineContext.Teams
                           where t.HomeStadium != null &&
                           ((Stadium)t.HomeStadium).Capacity > 40000
                           select t;

            RunTest(baseline, ((DataServiceQuery<Team>)query).Expand("HomeStadium"));

            ReadOnlyTestContext.ClearBaselineIncludes();

            // convert over iterator
            //ReadOnlyTestContext.ClearBaselineIncludes();
            //ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            //var query2 = from t in context.CreateQuery<AllStarTeam>("Teams")
            //             where (Team)t != null
            //             select t;


            //var baseline2 = from t in baseLineContext.Teams
            //                where (Team)t != null
            //                select new AllStarTeam
            //                {
            //                    City = t.City,
            //                    HomeStadium = t.HomeStadium,
            //                    TeamID = t.TeamID,
            //                    Players = null,
            //                    TeamName = t.TeamName
            //                };

            //RunTest(baseline2, ((DataServiceQuery<AllStarTeam>)query2).Expand("HomeStadium"));

            //ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqImmediateExecute()
        {
            // First on Resource Set

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var left = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                where t.HomeStadium != null &&
                                                t.HomeStadium.Capacity > 40000
                                                select t).Expand("HomeStadium").First();

            var right = (from t in baseLineContext.Teams
                         where t.HomeStadium != null &&
                           t.HomeStadium.Capacity > 40000
                         select t).First();

            if (!left.Equals(right))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // first on key predicate

            var left2 = (from t in context.CreateQuery<Team>("Teams")
                         where t.TeamID == 1
                         select t).First();


            var right2 = (from t in baseLineContext.Teams
                          where t.TeamID == 1
                          select t).First();

            if (!left2.Equals(right2))
                throw new Exception("Test Failed");

            // first on navigation to singleton

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var left3 = (from t in context.CreateQuery<Team>("Teams")
                         where t.TeamID == 2
                         select t.HomeStadium).First();


            var right3 = (from t in baseLineContext.Teams
                          where t.TeamID == 2
                          select t.HomeStadium).First();

            if (!left3.Equals(right3))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // first on navigation to primitive

            var left4 = (from t in context.CreateQuery<Team>("Teams")
                         where t.TeamID == 2
                         select t.TeamName).First();


            var right4 = (from t in baseLineContext.Teams
                          where t.TeamID == 2
                          select t.TeamName).First();

            if (!left4.Equals(right4))
                throw new Exception("Test Failed");

            // first with other query options

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var left5 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                 where t.HomeStadium != null &&
                                                 t.HomeStadium.Capacity > 40000
                                                 orderby t.TeamName
                                                 select t).Expand("HomeStadium").Skip(1).Take(2).First();


            var right5 = (from t in baseLineContext.Teams
                          where t.HomeStadium != null &&
                         t.HomeStadium.Capacity > 40000
                          orderby t.TeamName
                          select t).Skip(1).Take(2).First();

            if (!left5.Equals(right5))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // Single

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var left6 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                 where t.HomeStadium != null &&
                                                 t.HomeStadium.City == "Milwaukee"
                                                 select t).Expand("HomeStadium").Single();


            var right6 = (from t in baseLineContext.Teams
                          where t.HomeStadium != null &&
                            t.HomeStadium.City == "Milwaukee"
                          select t).Single();

            if (!left6.Equals(right6))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // Single - should throw since more then 1 result.

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            bool passed = false;

            try
            {
                var left7 = (from t in context.CreateQuery<Team>("Teams")
                             where t.HomeStadium != null &&
                             t.HomeStadium.Capacity > 1
                             select t).Single();
            }
            catch (InvalidOperationException e)
            {
                if (e.Message == "Sequence contains more than one element")
                    passed = true;
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // FirstOrDefault

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var left8 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                 where false
                                                 select t).Expand("HomeStadium").FirstOrDefault();


            var right8 = (from t in baseLineContext.Teams
                          where false
                          select t).FirstOrDefault();

            if (left8 != right8 || left8 != null)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var left9 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                 where false
                                                 select t).Expand("HomeStadium").SingleOrDefault();


            var right9 = (from t in baseLineContext.Teams
                          where false
                          select t).SingleOrDefault();

            if (left9 != right9 || left9 != null)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqNonGenericQueryableAPI()
        {
            // nongeneric Provider.Execute

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            IQueryable baseQuery = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                            where t.HomeStadium != null &&
                                                            t.HomeStadium.Capacity > 40000
                                                            select t).Expand("HomeStadium");

            MethodInfo mi = (from m in typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                             where m.Name == "First" &&
                             m.GetParameters().Length == 1
                             select m).First();

            MethodCallExpression e = Expression.Call(null, mi.MakeGenericMethod(typeof(Team)), baseQuery.Expression);

            object left = context.CreateQuery<Team>("Teams").Provider.Execute(e);

            object right = (from t in baseLineContext.Teams
                            where t.HomeStadium != null &&
                            t.HomeStadium.Capacity > 40000
                            select t).First();

            if (!left.Equals(right))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // Create query

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            IQueryable baseQuery2 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                             where t.HomeStadium != null &&
                                                             t.HomeStadium.Capacity > 40000
                                                             select t).Expand("HomeStadium");

            MethodInfo mi2 = (from m in typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                              where m.Name == "Take"
                              select m).Single();

            MethodCallExpression e2 = Expression.Call(null, mi2.MakeGenericMethod(typeof(Team)), baseQuery.Expression, Expression.Constant(2));

            var query = context.CreateQuery<Team>("Teams").Provider.CreateQuery(e2);

            var baseline = (from t in baseLineContext.Teams
                            where t.HomeStadium != null &&
                            t.HomeStadium.Capacity > 40000
                            select t).Take(2);

            RunTest(baseline, query);
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqRegressions()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            // this projection should happen on client side - so since nothing
            // is expanded in, there should be not results

            var query = from x in
                            (from l in context.CreateQuery<League>("Leagues")
                             where l.ID == 1
                             select l).AsEnumerable()
                        select x.Teams;

            IEnumerator i = query.GetEnumerator();
            i.MoveNext();

            if (((IList)i.Current).Count != 0)
            {
                throw new Exception("Test Failed");
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var teamLeft = (from t in context.CreateQuery<Team>("Teams")
                            select t).Take(1).Single();


            var teamRight = (from t in baseLineContext.Teams
                             select t).Take(1).Single();

            if (!teamLeft.Equals(teamRight))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            teamLeft = (from t in context.CreateQuery<Team>("Teams")
                        select t).Take(0).FirstOrDefault();


            teamRight = (from t in baseLineContext.Teams
                         select t).Take(0).FirstOrDefault();

            if (!(teamLeft == null && teamRight == null))
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        [TestMethod]
        public void LinqToString()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();

            // error case 
            var query = from l in context.CreateQuery<League>("League")
                        where l.ID == 999
                        select l.Teams;

            string error = query.ToString();
            System.Data.Test.Astoria.TestUtil.AssertContains(error, "Error translating Linq expression to URI: The method 'Select' is not supported.");
        }

        [TestMethod]
        public void LingDoubleInFilter()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query = from l in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where l.Double != 16d
                        select l;

            Assert.IsTrue(query.ToString().Contains("16.0"));

            query = from l in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where l.Double != 16.1234d
                    select l;

            Assert.IsTrue(!query.ToString().Contains(".0"));

            query = from l in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where l.Double != 1.3e15d
                    select l;

            Assert.IsTrue(!query.ToString().Contains(".0"));

            query = from l in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where l.Double != Double.NaN
                    select l;

            Assert.IsTrue(!query.ToString().Contains(".0"));

            query = from l in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where l.Double != Double.NegativeInfinity
                    select l;

            Assert.IsTrue(!query.ToString().Contains(".0"));
        }

        // [TestMethod]
        public void LinqRegressions2()
        {
            // TODO: this will raise a new error
            //ReadOnlyTestContext.ClearBaselineIncludes();
            //ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            //DataServiceQuery<Team> teams = (DataServiceQuery<Team>) context.CreateQuery<Team>("Teams").Where(t => t.TeamID == 1);

            //var query  = teams.Select(t => new { t.TeamID, LastNames = t.Players.Select(p => p.Lastname)}).AsQueryable();

            //try
            //{
            //    query.GetEnumerator();
            //}
            //catch (NotSupportedException e)
            //{
            //    if (e.Message != "The method 'Select' is not supported.")
            //    {
            //        throw new Exception("Test Failed");
            //    }
            //}

            //ReadOnlyTestContext.ClearBaselineIncludes();


            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var query3 = context.CreateQuery<League>("Leagues")
                            .Where(l => l.ID == 2)
                            .SelectMany(x => x.Teams);


            var baseline3 = baseLineContext.Leagues
                            .Where(l => l.ID == 2)
                            .SelectMany(x => x.Teams);

            RunTest(baseline3, query3);
            ReadOnlyTestContext.ClearBaselineIncludes();

            // with query options
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var query4 = context.CreateQuery<League>("Leagues")
                            .Where(l => l.ID == 2)
                            .SelectMany(x => x.Teams)
                            .Take(2);


            var baseline4 = baseLineContext.Leagues
                            .Where(l => l.ID == 2)
                            .SelectMany(x => x.Teams)
                            .Take(2);

            RunTest(baseline4, query4);
            ReadOnlyTestContext.ClearBaselineIncludes();


            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");

            var query5 = from t in context.CreateQuery<Team>("Teams")
                         where t.TeamID == 4
                         select t.HomeStadium.Sponsor;

            var baseline5 = from t in baseLineContext.Teams
                            where t.TeamID == 4
                            select t.HomeStadium.Sponsor;

            RunTest(baseline5, query5);


            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");

            var query6 = from s in
                             (from t in context.CreateQuery<Team>("Teams")
                              where t.TeamID == 4
                              select t.HomeStadium)
                         select s.Sponsor;


            var baseline6 = from s in
                                (from t in baseLineContext.Teams
                                 where t.TeamID == 4
                                 select t.HomeStadium)
                            select s.Sponsor;

            RunTest(baseline6, query6);
            ReadOnlyTestContext.ClearBaselineIncludes();


            ReadOnlyTestContext.ClearBaselineIncludes();

            var query7 = (from t in context.CreateQuery<Team>("Teams")
                          where t.TeamID == 4
                          select t).Take(2);

            try
            {
                query7.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "Cannot specify query options (orderby, where, take, skip, count) on single resource.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query7b = (from t in context.CreateQuery<Team>("Teams")
                           where t.TeamID == 4
                           select t.City).Where(t => t != "Seattle");

            try
            {
                query7b.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "Cannot specify query options (orderby, where, take, skip, count) on single resource.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query7c = (from t in context.CreateQuery<Team>("Teams")
                           where t.TeamID == 4
                           select t).OrderBy(t => t.City);

            try
            {
                query7c.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "Cannot specify query options (orderby, where, take, skip, count) on single resource.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();


            ReadOnlyTestContext.ClearBaselineIncludes();

            var query8 = from t in context.CreateQuery<Stadium>("Stadiums")
                         where (ulong)t.Capacity == (ulong)t.Capacity
                         select t;

            try
            {
                query8.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "Can't cast to unsupported type 'UInt64'")
                {
                    throw e;
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();


            ReadOnlyTestContext.ClearBaselineIncludes();
            var query9 = context.CreateQuery<League>("Leagues").Expand("Teams")
                         .Select(l => l.Teams);
            System.Data.Test.Astoria.TestUtil.AssertContains(query9.ToString(), "Error translating Linq expression to URI: The method 'Select' is not supported.");


            ReadOnlyTestContext.ClearBaselineIncludes();

            var query11 = from t in context.CreateQuery<Team>("Teams")
                          where t.City + "XXX" != "SeattleXXX"
                          orderby "ZZZ" + t.City
                          select t;

            var baseline11 = from t in baseLineContext.Teams
                             where t.City + "XXX" != "SeattleXXX"
                             orderby "ZZZ" + t.City
                             select t;

            RunTest(baseline11, query11);


            ReadOnlyTestContext.ClearBaselineIncludes();

            var query12 = from t in context.CreateQuery<LameTypeWithPublicFields>("LameTypeWithPublicFields")
                          where t.Name != ""
                          orderby t.Name
                          select t;

            var baseline12 = from t in baseLineContext.LameTypeWithPublicFields
                             where t.Name != ""
                             orderby t.Name
                             select t;

            try
            {
                query12.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "Referencing public field 'Name' not supported in query option expression.  Use public property instead.")
                {
                    throw new Exception("Test Failed");
                }
            }
        }

        // [TestMethod]
        public void LinqRegressions3()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query = (from t in context.CreateQuery<Team>("Teams")
                         select t).Take(2).Where(t => t.City == "Seattle");

            try
            {
                query.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "The filter query option cannot be specified after the top query option.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query2 = (from t in context.CreateQuery<Team>("Teams")
                          select t).Skip(2).Where(t => t.City == "Seattle");

            try
            {
                query2.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "The filter query option cannot be specified after the skip query option.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query3 = (from t in context.CreateQuery<Team>("Teams")
                          select t).Take(2).OrderBy(t => t.City);

            try
            {
                query3.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "The orderby query option cannot be specified after the top query option.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query4 = (from t in context.CreateQuery<Team>("Teams")
                          select t).Take(2).Skip(3);

            try
            {
                query4.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message != "The skip query option cannot be specified after the top query option.")
                {
                    throw new Exception("Test Failed");
                }
            }

            DataServiceQuery<Team> query5 = (DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                                    select t;

            try
            {
                query5 = (DataServiceQuery<Team>)query5.AddQueryOption("fog", 89).OrderBy(t => t.TeamName).Take(1);
                query5.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                throw new Exception(e.Message);
            }

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            byte[] photo = context.CreateQuery<League>("Leagues").Where(l => l.ID == 1).SelectMany(l => l.Teams).
                Where(t => t.TeamID == 1).Select(t => t.Photo).Single();

            byte[] photoBaseline = baseLineContext.Leagues.Where(l => l.ID == 1).SelectMany(l => l.Teams).
                Where(t => t.TeamID == 1).Select(t => t.Photo).Single();

            System.Data.Test.Astoria.TestUtil.AssertAreIEnumerablesEqual(photo, photoBaseline);

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query7 = from t in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                         where t.NullableDateTimeOffset != null
                         && t.NullableDateTimeOffset.Value.Year > 1965
                         orderby t.NullableDateTimeOffset.Value.Year
                         select t;


            var baseline7 = from t in baseLineContext.Var1
                            where t.NullableDateTimeOffset != null
                            && t.NullableDateTimeOffset.Value.Year > 1965
                            orderby t.NullableDateTimeOffset.Value.Year
                            select t;

            RunTest(baseline7, query7);
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        [TestMethod]
        public void LinqRegression4()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();

            var q = from t in context.CreateQuery<BigCity>("BigCities")
                    select t is DerivedBigCity ? new DerivedBigCity() { Name = t.Name } : new BigCity() { Name = t.Name };

            var error = q.ToString();

            System.Data.Test.Astoria.TestUtil.AssertContains(error, "Error translating Linq expression to URI: The method 'Select' is not supported.");

            var q2 = from t in context.CreateQuery<BigCity>("BigCities")
                     select t is DerivedBigCity ? new BigCity() { Name = t.Name } : new DerivedBigCity() { Name = t.Name };

            error = q2.ToString();

            System.Data.Test.Astoria.TestUtil.AssertContains(error, "Error translating Linq expression to URI: The method 'Select' is not supported.");

        }

        [TestMethod]
        public void LinqIllegalCases()
        {
            bool passed = false;

            // navigation after query options. - Filter
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query = (from t in context.CreateQuery<Team>("Teams")
                         select t).Where(t => t.City == "Seattle").Select(t => t.HomeStadium);

            passed = false;

            try
            {
                query.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Can only specify query options (orderby, where, take, skip) after last navigation.")
                {
                    passed = true;
                }
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // navigation after query options. - OrderBy
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query2 = (from t in context.CreateQuery<Team>("Teams")
                          select t).OrderBy(t => t.City).Select(t => t.Players);

            passed = false;

            try
            {
                query2.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Can only specify query options (orderby, where, take, skip) after last navigation.")
                {
                    passed = true;
                }
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // take after query options. - Take
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query3 = (from t in context.CreateQuery<Team>("Teams")
                          select t).Take(1).Select(t => t.Players);

            passed = false;

            try
            {
                query3.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Can only specify query options (orderby, where, take, skip) after last navigation.")
                {
                    passed = true;
                }
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();

            // navigation after query options. - Skip
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query4 = (from t in context.CreateQuery<Team>("Teams")
                          select t).Skip(1).Select(t => t.Players);

            passed = false;

            try
            {
                query4.GetEnumerator();
            }
            catch (NotSupportedException e)
            {
                if (e.Message == "Can only specify query options (orderby, where, take, skip) after last navigation.")
                {
                    passed = true;
                }
            }

            if (!passed)
                throw new Exception("Test Failed");

            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // NOTE: This test has a VB equivalent in ClientRegressionTests.SelectManyWithTypeCast
        // [TestMethod]
        public void LinqCast()
        {
            {
                Trace.WriteLine("type specified explicitly in comprehension.");

                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

                var query = from League l in context.CreateQuery<League>("Leagues")
                            where l.ID == 1
                            from t in l.Teams
                            select t;


                var baseline = from League l in baseLineContext.Leagues
                               where l.ID == 1
                               from t in l.Teams
                               select t;

                RunTest(baseline, query);

                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("type specified explicitly in comprehension - Select Many.");

                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

                var query2 = from League l in context.CreateQuery<League>("Leagues")
                             where l.ID == 1
                             from Team t in l.Teams
                             select t;


                var baseline2 = from League l in baseLineContext.Leagues
                                where l.ID == 1
                                from Team t in l.Teams
                                select t;

                RunTest(baseline2, query2);

                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("arbitrary cast of resource set (nbo base type)");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "Rows");

                Row r = new Row();
                CustomerRow x = (CustomerRow)r;

                var query3 = from CustomerRow cr in context.CreateQuery<Row>("Rows")
                             select cr;

                var baseline3 = from cr in baseLineContext.Rows
                                select (CustomerRow)cr;

                RunTest(baseline3, query3);

                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("arbitrary case of resource set, select many");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "Rows");
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Row), "Details");

                var query4 = from MyTable t in context.CreateQuery<Table>("Tables")
                             where t.TableName == "Customers"
                             from CustomerRow cr in t.Rows
                             where cr.Id == 1
                             from CustomerRowDetail crd in cr.Details
                             select crd;

                var baseline4 = from Table t in baseLineContext.Tables
                                where t.TableName == "Customers"
                                from cr in t.Rows
                                where cr.Id == 1
                                from crd in cr.Details
                                select (CustomerRowDetail)crd;

                RunTest(baseline4, query4);
            }

            {
                Trace.WriteLine("arbitrary case of resource set, select many");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "Rows");
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Row), "Details");

                var query4a = from MyTable t in context.CreateQuery<Table>("Tables")
                              from CustomerRow cr in t.Rows
                              from CustomerRowDetail crd in cr.Details
                              where t.TableName == "Customers"
                              where cr.Id == 1
                              select crd;

                var baseline4a = from Table t in baseLineContext.Tables
                                 from cr in t.Rows
                                 from crd in cr.Details
                                 where t.TableName == "Customers"
                                 where cr.Id == 1
                                 select (CustomerRowDetail)crd;

                RunTest(baseline4a, query4a);
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            {
                Trace.WriteLine("arbitrary cast of resource set singleton");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "TableInfo");

                var query5 = from CustomTableInfo cti in
                                 (from Table t in context.CreateQuery<Table>("Tables")
                                  where t.TableName == "Customers"
                                  select t.TableInfo)
                             select cti;

                var baseline5 = from cti in
                                    (from Table t in baseLineContext.Tables
                                     where t.TableName == "Customers"
                                     select t.TableInfo)
                                select (CustomTableInfo)cti;

                RunTest(baseline5, query5);

                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("query options of new type");

                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "Rows");
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Row), "Details");

                var query6 = from MyTable t in context.CreateQuery<Table>("Tables")
                             where t.TableName == "Customers"
                             from CustomerRow cr in t.Rows
                             where cr.Id == 1
                             from CustomerRowDetail crd in cr.Details
                             where crd.Word != "xxx"
                             orderby crd.Word
                             select crd;

                var baseline6 = from Table t in baseLineContext.Tables
                                where t.TableName == "Customers"
                                from cr in t.Rows
                                where cr.Id == 1
                                from crd in cr.Details
                                where crd.Word != "xxx"
                                orderby crd.Word
                                select (CustomerRowDetail)crd;

                RunTest(baseline6, query6);
                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("single casts -- select many");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "Rows");

                var query7 = from t in context.CreateQuery<Table>("Tables")
                             where t.TableName == "Customers"
                             from cr in t.Rows.Cast<CustomerRow>()
                             select cr;

                var baseline7 = from Table t in baseLineContext.Tables
                                where t.TableName == "Customers"
                                from cr in t.Rows
                                select (CustomerRow)cr;

                RunTest(baseline7, query7);
                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("multiple casts.");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "Rows");

                var query7 = from t in context.CreateQuery<Table>("Tables").Cast<int>().Cast<MyTable>()
                             where t.TableName == "Customers"
                             from cr in t.Rows.Cast<object>().Cast<CustomerRow>()
                             select cr;

                var baseline7 = from Table t in baseLineContext.Tables
                                where t.TableName == "Customers"
                                from cr in t.Rows
                                select (CustomerRow)cr;

                RunTest(baseline7, query7);
                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("after custom queryoptions, expand");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Table), "Rows");

                var query8 = ((DataServiceQuery<Table>)from t in context.CreateQuery<Table>("Tables")
                                                       where t.TableName == "Customers"
                                                       select t).AddQueryOption("ghghgh", "xxx").Expand("Rows").Cast<MyTable>();

                var baseline8 = from Table t in baseLineContext.Tables
                                where t.TableName == "Customers"
                                select (MyTable)t;

                RunTest(baseline8, query8);
                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("Cast of singleton returning result");

                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

                var query9 = (from Team t in context.CreateQuery<Team>("Teams")
                              where t.TeamID == 1
                              select t.HomeStadium).Cast<Stadium>();

                var baseline9 = (from Team t in baseLineContext.Teams
                                 where t.TeamID == 1
                                 select t.HomeStadium).Cast<Stadium>();

                RunTest(baseline9, query9);
                ReadOnlyTestContext.ClearBaselineIncludes();
            }

            {
                Trace.WriteLine("Cast to narrow type");
                ReadOnlyTestContext.ClearBaselineIncludes();
                ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
                context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;

                var query10 = (from Team t in context.CreateQuery<Team>("Teams")
                               where t.TeamID == 1
                               select t.HomeStadium).Cast<NarrowStadium>();

                var baseline10 = (from Team t in baseLineContext.Teams
                                  where t.TeamID == 1
                                  select t.HomeStadium).Select(s => (NarrowStadium)s);

                RunTest(baseline10, query10);
                ReadOnlyTestContext.ClearBaselineIncludes();
            }

        }

        // [TestMethod]
        public void LinqExpand()
        {
            // simple expand
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            DataServiceQuery<League> query = (DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                                       select l;

            query = query.Expand("Teams");

            var baseline = from l in baseLineContext.Leagues
                           select l;

            RunTest(baseline, query);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // mulitple expands
            ReadOnlyTestContext.ClearBaselineIncludes();

            DataServiceQuery<Team> query2 = (DataServiceQuery<Team>)from Team t in context.CreateQuery<Team>("Teams")
                                                                    where t.TeamID == 1
                                                                    select t;

            query2 = query2.Expand("HomeStadium").Expand("Players");

            var cachedQuery2 = query2.ToList().AsQueryable();

            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            var baseline2 = from Team t in baseLineContext.Teams
                            where t.TeamID == 1
                            select t;

            RunTest(baseline2, cachedQuery2);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // empty expands
            ReadOnlyTestContext.ClearBaselineIncludes();

            DataServiceQuery<Team> query3 = (DataServiceQuery<Team>)from Team t in context.CreateQuery<Team>("Teams")
                                                                    where t.TeamID == 2
                                                                    select t;

            query3 = query3.Expand("Players").Expand("HomeStadium");

            var cachedQuery3 = query3.ToList().AsQueryable();

            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var baseline3 = from Team t in baseLineContext.Teams
                            where t.TeamID == 2
                            select t;

            RunTest(baseline3, cachedQuery3);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // expand after key predicate
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            DataServiceQuery<Team> query4 = (DataServiceQuery<Team>)from Team t in context.CreateQuery<Team>("Teams")
                                                                    where t.TeamID == 1
                                                                    select t;
            query4 = query4.Expand("HomeStadium");

            var cachedQuery4 = query4.ToList().AsQueryable();

            var baseline4 = from Team t in baseLineContext.Teams
                            where t.TeamID == 1
                            select t;

            RunTest(baseline4, cachedQuery4);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // expand after singleton navigation
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");

            DataServiceQuery<Stadium> query5 = (DataServiceQuery<Stadium>)from Team t in context.CreateQuery<Team>("Teams")
                                                                          where t.TeamID == 1
                                                                          select t.HomeStadium;

            query5 = query5.Expand("Sponsor");

            var cachedQuery5 = query5.ToList().AsQueryable();

            var baseline5 = from Team t in baseLineContext.Teams
                            where t.TeamID == 1
                            select t.HomeStadium;

            RunTest(baseline5, cachedQuery5);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // complex expand paths (multi-level)
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Mascot");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");

            DataServiceQuery<League> query6 = (DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                                        select l;

            query6 = query.Expand("Teams").Expand("Teams($expand=HomeStadium)").Expand("Teams($expand=Mascot)")
                .Expand("Teams($expand=HomeStadium($expand=Sponsor))");

            var baseline6 = from l in baseLineContext.Leagues
                            select l;

            RunTest(baseline6, query6);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // expand with other query options
            ReadOnlyTestContext.ClearBaselineIncludes();

            DataServiceQuery<Team> query7 = (DataServiceQuery<Team>)(from Team t in context.CreateQuery<Team>("Teams")
                                                                     where t.City == "New York"
                                                                     orderby t.TeamName
                                                                     select t).Take(1);

            query7 = query7.Expand("HomeStadium");

            var cachedQuery7 = query7.ToList().AsQueryable();

            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var baseline7 = (from Team t in baseLineContext.Teams
                             where t.City == "New York"
                             orderby t.TeamName
                             select t).Take(1);

            RunTest(baseline7, cachedQuery7);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // expand with circular reference
            ReadOnlyTestContext.ClearBaselineIncludes();

            DataServiceQuery<Employee> query8 = (DataServiceQuery<Employee>)(from e in context.CreateQuery<Employee>("Employees")
                                                                             where e.Name == "Tester"
                                                                             select e);

            //context.MergeOption = MergeOption.AppendOnly;

            query8 = query8.Expand("Reports")
                            .Expand("Reports($expand=Reports)")
                            .Expand("Reports($expand=Reports($expand=Reports))")
                            .Expand("Reports($expand=Reports($expand=Reports($expand=Reports)))")
                            .Expand("Manager")
                            .Expand("Manager($expand=Reports)")
                            .Expand("Manager($expand=Manager)")
                            .Expand("Manager($expand=Manager($expand=Reports))");


            Employee tester = query8.ToList()[0];

            context.MergeOption = MergeOption.NoTracking;

            // expand with null singleton links
            ReadOnlyTestContext.ClearBaselineIncludes();

            DataServiceQuery<Team> query9 = (DataServiceQuery<Team>)from Team t in context.CreateQuery<Team>("Teams")
                                                                    select t;

            query9 = query9.Expand("HomeStadium");

            var cachedQuery9 = query9.ToList().AsQueryable();

            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var baseline9 = from Team t in baseLineContext.Teams
                            select t;

            RunTest(baseline9, cachedQuery9);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // try navigation + query options after expand
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            DataServiceQuery<League> query10 = (DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                                         where l.ID == 1
                                                                         select l;

            var expandedQuery = query10.Expand("Players").SelectMany(l => l.Teams).Take(1);

            var baselineExpandedQuery = (from l in baseLineContext.Leagues
                                         where l.ID == 1
                                         from t in l.Teams
                                         select t).Take(1);

            RunTest(baselineExpandedQuery, expandedQuery);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // client side projection after expand
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            DataServiceQuery<League> query11 = (DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                                         select l;

            var expandedQuery11 = query11.Expand("Teams").AsEnumerable().Select(l => new { l.Name, l.Teams.Count });

            var baseline11 = from League l in baseLineContext.Leagues
                             select new { l.Name, l.Teams.Count };

            RunTest(baseline11, expandedQuery11.AsQueryable());
            ReadOnlyTestContext.ClearBaselineIncludes();

            // back pointers

            ReadOnlyTestContext.ClearBaselineIncludes();
            DataServiceQuery<Parent> query12 = (DataServiceQuery<Parent>)context.CreateQuery<Parent>("Parents").Where(p => p.Id == 1);
            query12 = query12.Expand("Children").
                              Expand("Children($expand=Parents)").
                              Expand("Children($expand=Mom)").
                              Expand("Children($expand=Dad)").
                              Expand("Children($expand=Mom($expand=Children))").
                              Expand("Children($expand=Dad($expand=Children))");

            foreach (var p in query12)
            {
                Assert.AreEqual(p.Children.Count, 1);
                Assert.AreEqual(p.Children.Single().Parents.Count, 2);
                Assert.AreEqual(p.Children.Single().Mom.Children.Count, 1);
                Assert.AreEqual(p.Children.Single().Dad.Children.Count, 1);
            }
            ReadOnlyTestContext.ClearBaselineIncludes();

            // Expand before Orderby, ThenBy
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            DataServiceQuery<League> query13 = (DataServiceQuery<League>)(from l in context.CreateQuery<League>("Leagues")
                                                .Expand("Teams")
                                                                          where l.ConceptionDate > new DateTime(1800, 1, 1)
                                                                          orderby l.ConceptionDate descending, l.Name descending
                                                                          select l).Take(10);

            var baseline13 = (from League l in baseLineContext.Leagues
                              where l.ConceptionDate > new DateTime(1800, 1, 1)
                              orderby l.ConceptionDate descending, l.Name descending
                              select l).Take(10);

            RunTest(baseline13, query13.AsQueryable());
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqMultipleKeys()
        {
            // using multiple keys - multiple predicates
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                        where m.Key1 == 1
                        where m.Key2 == "23"
                        where m.Key3 == 37
                        select m;

            var baseline = from m in baseLineContext.MoreVar1
                           where m.Key1 == 1
                           where m.Key2 == "23"
                           where m.Key3 == 37
                           select m;

            RunTest(baseline, query);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // using multiple keys - single predicate

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query2 = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                         where m.Key1 == 1
                         && m.Key2 == "23"
                         && m.Key3 == 37
                         select m;

            var baseline2 = from m in baseLineContext.MoreVar1
                            where m.Key1 == 1
                            && m.Key2 == "23"
                            && m.Key3 == 37
                            select m;

            RunTest(baseline2, query2);

            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();

            var query3 = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                         where m.Key2 == "23"
                         && m.Key1 == 1
                         && m.Key3 == 37
                         select m;

            var baseline3 = from m in baseLineContext.MoreVar1
                            where m.Key2 == "23"
                         && m.Key1 == 1
                         && m.Key3 == 37
                            select m;

            RunTest(baseline3, query3);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // using multiple keys then navigating
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(MultipleKeyType), "Related");

            var query4 = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                         where m.Key1 == 1
                         && m.Key2 == "23"
                         && m.Key3 == 37
                         from n in m.Related
                         select n;

            var baseline4 = from m in baseLineContext.MoreVar1
                            where m.Key1 == 1
                            && m.Key2 == "23"
                            && m.Key3 == 37
                            from n in m.Related
                            select n;

            RunTest(baseline4, query4);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // multiple keys with other query options
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(MultipleKeyType), "Related");

            var query5 = (from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                          where m.Key1 == 1
                          && m.Key2 == "23"
                          && m.Key3 == 37
                          from n in m.Related
                          orderby n.Data2
                          select n).Take(1);

            var baseline5 = (from m in baseLineContext.MoreVar1
                             where m.Key1 == 1
                             && m.Key2 == "23"
                             && m.Key3 == 37
                             from n in m.Related
                             orderby n.Data2
                             select n).Take(1);

            RunTest(baseline5, query5);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // mixing keys with other predicate
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query6 = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                         where m.Key2 == "23"
                         && m.Key1 == 1
                         && m.Data1 != ""
                         select m;

            var baseline6 = from m in baseLineContext.MoreVar1
                            where m.Key2 == "23"
                         && m.Key1 == 1
                         && m.Data1 != ""
                            select m;

            RunTest(baseline6, query6);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // not specifying all keys then navigating - error
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(MultipleKeyType), "Related");

            var query7 = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                         where m.Key1 == 1
                         && m.Key3 == 37
                         from n in m.Related
                         select n;

            try
            {
                query7.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message != "Can only specify query options (orderby, where, take, skip) after last navigation.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            // expression is not AndAlso, then navigate - error
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(MultipleKeyType), "Related");

            var query8 = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                         where m.Key1 == 1
                         && m.Key2 == "23"
                         || m.Key3 == 37
                         from n in m.Related
                         select n;

            try
            {
                query8.GetEnumerator();
            }
            catch (NotSupportedException nse)
            {
                if (nse.Message != "Can only specify query options (orderby, where, take, skip) after last navigation.")
                {
                    throw new Exception("Test Failed");
                }
            }

            ReadOnlyTestContext.ClearBaselineIncludes();

            // not specifying all keys -> turned into filter
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query9 = from m in context.CreateQuery<MultipleKeyType>("MoreVar1")
                         where m.Key1 == 1
                         where m.Key2 == "23"
                         select m;

            var baseline9 = from m in baseLineContext.MoreVar1
                            where m.Key1 == 1
                            where m.Key2 == "23"
                            where m.Key3 == 37
                            select m;

            RunTest(baseline9, query9);

            ReadOnlyTestContext.ClearBaselineIncludes();

        }

        // [TestMethod]
        public void LinqTypesFromOtherNamespaces()
        {
            // simple case - root type differs
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable1 = from l in context.CreateQuery<BaseballLeague>("Leagues")
                             select l;

            var baseline1 = from l in baseLineContext.Leagues
                            select new BaseballLeague
                            {
                                ID = l.ID,
                                Name = l.Name,
                                ConceptionDate = l.ConceptionDate,
                                Teams = null
                            };

            RunTest(baseline1, queryable1);

            // expand in nested entity
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");

            var queryable2 = ((DataServiceQuery<BaseballLeague>)from l in context.CreateQuery<BaseballLeague>("Leagues")
                                                                select l).Expand("Teams");

            var baseline2 = from l in baseLineContext.Leagues
                            select new BaseballLeague
                            {
                                ID = l.ID,
                                Name = l.Name,
                                ConceptionDate = l.ConceptionDate,
                                Teams = (from t in l.Teams
                                         select new BaseballTeam()
                                         {
                                             TeamID = t.TeamID,
                                             City = t.City,
                                             TeamName = t.TeamName,
                                             Mascot = null,
                                             HomeStadium = null,
                                             Players = null
                                         }).ToList()
                            };

            RunTest(baseline2, queryable2);
        }

        // [TestMethod]
        public void LinqNormalizeCompareAgainstZero()
        {
            var queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                            where String.Compare(s.City, "Seattle") == 0
                            select s;
            var baseline = from s in baseLineContext.Stadiums
                           where String.Compare(s.City, "Seattle") == 0
                           select s;
            RunTest(baseline, queryable);


            queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                        where String.Compare(s.City, "Seattle") >= 0
                        select s;
            baseline = from s in baseLineContext.Stadiums
                       where String.Compare(s.City, "Seattle") >= 0
                       select s;
            RunTest(baseline, queryable);


            queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                        where String.Compare(s.City, "Seattle") <= 0
                        select s;
            baseline = from s in baseLineContext.Stadiums
                       where String.Compare(s.City, "Seattle") <= 0
                       select s;
            RunTest(baseline, queryable);


            queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                        where String.Compare(s.City, "Seattle") < 0
                        select s;

            baseline = from s in baseLineContext.Stadiums
                       where String.Compare(s.City, "Seattle") < 0
                       select s;
            RunTest(baseline, queryable);


            queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                        where String.Compare(s.City, "Seattle") > 0
                        select s;
            baseline = from s in baseLineContext.Stadiums
                       where String.Compare(s.City, "Seattle") > 0
                       select s;
            RunTest(baseline, queryable);


            queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                        where String.Compare(s.City, "Seattle") != 0
                        select s;
            baseline = from s in baseLineContext.Stadiums
                       where String.Compare(s.City, "Seattle") != 0
                       select s;
            RunTest(baseline, queryable);

            queryable = from s in context.CreateQuery<Stadium>("Stadiums")
                        where 0 == String.Compare(s.City, "Seattle")
                        select s;
            baseline = from s in baseLineContext.Stadiums
                       where 0 == String.Compare(s.City, "Seattle")
                       select s;
        }

        // [TestMethod]
        public void LinqMethods()
        {
            // complex, nested methods
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var query1 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                  where t.HomeStadium != null && t.HomeStadium.City.Trim().ToUpper().Length != 7
                                                  orderby (t.HomeStadium.City.Trim() + "abcde").Replace("abcde", "")
                                                  select t).Expand("HomeStadium");

            var baseline1 = from t in baseLineContext.Teams
                            where t.HomeStadium != null && t.HomeStadium.City.Trim().ToUpper().Length != 7
                            orderby (t.HomeStadium.City.Trim() + "abcde").Replace("abcde", "")
                            select t;

            RunTest(baseline1, query1);
            ReadOnlyTestContext.ClearBaselineIncludes();

            // mix methods with other expressions
            ReadOnlyTestContext.ClearBaselineIncludes();

            var query2 = from t in context.CreateQuery<Team>("Teams")
                         where String.Concat((t.City + t.City).Trim(), "foo").Length > 8
                            && t.City.Length < 100
                         orderby String.Concat(t.City, "bar").Trim().Length + 17
                         select t;

            var baseline2 = from t in baseLineContext.Teams
                            where String.Concat((t.City + t.City).Trim(), "foo").Length > 8
                                && t.City.Length < 100
                            orderby String.Concat(t.City, "bar").Trim().Length + 17
                            select t;

            RunTest(baseline2, query2);
            ReadOnlyTestContext.ClearBaselineIncludes();

            //// not supported methods
            //ReadOnlyTestContext.ClearBaselineIncludes();

            //var query3 = from t in context.CreateQuery<Team>("Teams")
            //             where t.City.Normalize() != "xxx"
            //             orderby Int16.Parse(t.City)
            //             select t;


            //try
            //{
            //    query3.GetEnumerator();
            //}
            //catch (NotSupportedException e)
            //{
            //    if (e.Message != "")
            //        throw new Exception("The expression ([10007].City.Normalize() != "xxx") is not supported.");
            //}
            //ReadOnlyTestContext.ClearBaselineIncludes();

            // method after navigation

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var query99 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                   where t.HomeStadium != null && t.HomeStadium.City.Trim() != "Seattle"
                                                   orderby t.HomeStadium.City.Trim()
                                                   select t).Expand("HomeStadium");

            var baseline99 = from t in baseLineContext.Teams
                             where t.HomeStadium != null && t.HomeStadium.City.Trim() != "Seattle"
                             orderby t.HomeStadium.City.Trim()
                             select t;

            RunTest(baseline99, query99);
            ReadOnlyTestContext.ClearBaselineIncludes();

            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            var query100 = ((DataServiceQuery<Team>)from t in context.CreateQuery<Team>("Teams")
                                                    where t.HomeStadium != null && t.HomeStadium.City.Length > 7
                                                    orderby t.HomeStadium.City.Length
                                                    select t).Expand("HomeStadium"); ;

            var baseline100 = from t in baseLineContext.Teams
                              where t.HomeStadium != null && t.HomeStadium.City.Length > 7
                              orderby t.HomeStadium.City.Length
                              select t;

            RunTest(baseline100, query100);
            ReadOnlyTestContext.ClearBaselineIncludes();
        }

        // [TestMethod]
        public void LinqStringMethods()
        {
            // methods in predicate and order by

            // contains
            var query = from t in context.CreateQuery<Team>("Teams")
                        where t.City.Contains("Sea") || t.City.Contains("Mon")
                        orderby t.City.Contains("Sea")
                        select t;

            var baseline = from t in baseLineContext.Teams
                           where t.City.Contains("Sea") || t.City.Contains("Mon")
                           orderby t.City.Contains("Sea")
                           select t;

            RunTest(baseline, query);

            //endswith
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.TeamName.EndsWith("s")
                    orderby t.TeamName.EndsWith("s")
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.TeamName.EndsWith("s")
                       orderby t.TeamName.EndsWith("s")
                       select t;

            RunTest(baseline, query);

            //startswith
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.TeamName.StartsWith("W")
                    orderby t.TeamName.StartsWith("W")
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.TeamName.StartsWith("W")
                       orderby t.TeamName.StartsWith("W")
                       select t;

            RunTest(baseline, query);

            //indexof
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.City.IndexOf("a") > 0
                    orderby t.TeamName.IndexOf("a")
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.City.IndexOf("a") > 0
                       orderby t.TeamName.IndexOf("a")
                       select t;

            RunTest(baseline, query);

            //replace
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.City.Replace("ea", "xxx") != "Sxxxttle"
                    orderby t.TeamName.Replace("s", "zzz")
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.City.Replace("ea", "xxx") != "Sxxxttle"
                       orderby t.TeamName.Replace("s", "zzz")
                       select t;

            RunTest(baseline, query);

            //substring
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.City.Substring(1) != "eattle"
                    orderby t.TeamName.Substring(4)
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.City.Substring(1) != "eattle"
                       orderby t.TeamName.Substring(4)
                       select t;

            RunTest(baseline, query);

            query = from t in context.CreateQuery<Team>("Teams")
                    where t.City.Substring(1, 2) != "ea"
                    orderby t.TeamName.Substring(2, 2)
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.City.Substring(1, 2) != "ea"
                       orderby t.TeamName.Substring(2, 2)
                       select t;

            RunTest(baseline, query);

            //tolower
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.City.ToLower() != "seattle"
                    orderby t.City.ToLower() descending
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.City.ToLower() != "seattle"
                       orderby t.City.ToLower() descending
                       select t;

            RunTest(baseline, query);

            //toupper
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.City.ToUpper() != "seattle"
                    orderby t.City.ToUpper() descending
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.City.ToUpper() != "seattle"
                       orderby t.City.ToUpper() descending
                       select t;

            RunTest(baseline, query);

            // trim
            query = from t in context.CreateQuery<Team>("Teams")
                    where String.Concat(t.City, " ").Trim() != "Seattle"
                    orderby t.City.Trim()
                    select t;

            baseline = from t in baseLineContext.Teams
                       where String.Concat(t.City, " ").Trim() != "Seattle"
                       orderby t.City.Trim()
                       select t;

            RunTest(baseline, query);

            // concat
            query = from t in context.CreateQuery<Team>("Teams")
                    where String.Concat(t.City, "xxx") != "Seattlexxx"
                    orderby String.Concat(t.City, "xxx")
                    select t;

            baseline = from t in baseLineContext.Teams
                       where String.Concat(t.City, "xxx") != "Seattlexxx"
                       orderby String.Concat(t.City, "xxx")
                       select t;

            RunTest(baseline, query);

            //length
            query = from t in context.CreateQuery<Team>("Teams")
                    where t.City.Length != 7
                    orderby t.City.Length
                    select t;

            baseline = from t in baseLineContext.Teams
                       where t.City.Length != 7
                       orderby t.City.Length
                       select t;

            RunTest(baseline, query);

            //equals
            query = from t in context.CreateQuery<Team>("Teams")
                    where !t.City.Equals("Seattle")
                    orderby !t.City.Equals("Seattle")
                    select t;

            baseline = from t in baseLineContext.Teams
                       where !t.City.Equals("Seattle")
                       orderby !t.City.Equals("Seattle")
                       select t;

            RunTest(baseline, query);

            //String.Equals
            query = from t in context.CreateQuery<Team>("Teams")
                    where !String.Equals(t.City, "Seattle")
                    orderby !String.Equals(t.City, "Seattle")
                    select t;

            baseline = from t in baseLineContext.Teams
                       where !String.Equals(t.City, "Seattle")
                       orderby !String.Equals(t.City, "Seattle")
                       select t;

            RunTest(baseline, query);
        }

        // [TestMethod]
        public void LinqMathMethods()
        {
            //Round
            var query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where Math.Round(j.Double) > 10
                        orderby Math.Round(j.Double)
                        select j;

            var baseline = from j in baseLineContext.Var1
                           where Math.Round(j.Double) > 10
                           orderby Math.Round(j.Double)
                           select j;

            RunTest(baseline, query);

            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where Math.Round(j.Decimal) > 10
                    orderby Math.Round(j.Decimal)
                    select j;

            baseline = from j in baseLineContext.Var1
                       where Math.Round(j.Decimal) > 10
                       orderby Math.Round(j.Decimal)
                       select j;

            RunTest(baseline, query);

            //Floor
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where Math.Floor(j.Double) > 10
                    orderby Math.Floor(j.Double)
                    select j;

            baseline = from j in baseLineContext.Var1
                       where Math.Floor(j.Double) > 10
                       orderby Math.Floor(j.Double)
                       select j;

            RunTest(baseline, query);

            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where Math.Floor(j.Decimal) > 10
                    orderby Math.Floor(j.Decimal)
                    select j;

            baseline = from j in baseLineContext.Var1
                       where Math.Floor(j.Decimal) > 10
                       orderby Math.Floor(j.Decimal)
                       select j;

            RunTest(baseline, query);

            //Ceiling
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where Math.Ceiling(j.Double) > 10
                    orderby Math.Ceiling(j.Double)
                    select j;

            baseline = from j in baseLineContext.Var1
                       where Math.Ceiling(j.Double) > 10
                       orderby Math.Ceiling(j.Double)
                       select j;

            RunTest(baseline, query);

            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where Math.Ceiling(j.Decimal) > 10
                    orderby Math.Ceiling(j.Decimal)
                    select j;

            baseline = from j in baseLineContext.Var1
                       where Math.Ceiling(j.Decimal) > 10
                       orderby Math.Ceiling(j.Decimal)
                       select j;

            RunTest(baseline, query);
        }

        // [TestMethod]
        public void LinqDateTimeOffsetMethods()
        {
            //Day
            var query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where j.DateTimeOffset.Day != 1
                        orderby j.DateTimeOffset.Day
                        select j;

            var baseline = from j in baseLineContext.Var1
                           where j.DateTimeOffset.Day != 1
                           orderby j.DateTimeOffset.Day
                           select j;

            RunTest(baseline, query);

            //Month
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where j.DateTimeOffset.Month != 9
                    orderby j.DateTimeOffset.Month
                    select j;

            baseline = from j in baseLineContext.Var1
                       where j.DateTimeOffset.Month != 9
                       orderby j.DateTimeOffset.Month
                       select j;

            RunTest(baseline, query);

            //Year
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where j.DateTimeOffset.Year != 1970
                    orderby j.DateTimeOffset.Year
                    select j;

            baseline = from j in baseLineContext.Var1
                       where j.DateTimeOffset.Year != 1970
                       orderby j.DateTimeOffset.Year
                       select j;

            RunTest(baseline, query);

            //hour
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where j.DateTimeOffset.Hour != 12
                    orderby j.DateTimeOffset.Hour
                    select j;

            baseline = from j in baseLineContext.Var1
                       where j.DateTimeOffset.Hour != 12
                       orderby j.DateTimeOffset.Hour
                       select j;

            RunTest(baseline, query);

            //minute
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where j.DateTimeOffset.Minute != 16
                    orderby j.DateTimeOffset.Minute
                    select j;

            baseline = from j in baseLineContext.Var1
                       where j.DateTimeOffset.Minute != 16
                       orderby j.DateTimeOffset.Minute
                       select j;

            RunTest(baseline, query);

            //second
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where j.DateTimeOffset.Second != 18
                    orderby j.DateTimeOffset.Second
                    select j;

            baseline = from j in baseLineContext.Var1
                       where j.DateTimeOffset.Second != 18
                       orderby j.DateTimeOffset.Second
                       select j;

            RunTest(baseline, query);
        }

        // [TestMethod]
        public void LinqTimeSpanMethods()
        {
            //hour
            var query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                        where j.TimeSpan.Hours != 12
                        orderby j.TimeSpan.Hours
                        select j;

            var baseline = from j in baseLineContext.Var1
                           where j.TimeSpan.Hours != 12
                           orderby j.TimeSpan.Hours
                           select j;

            RunTest(baseline, query);

            //minute
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where j.TimeSpan.Minutes != 16
                    orderby j.TimeSpan.Minutes
                    select j;

            baseline = from j in baseLineContext.Var1
                       where j.TimeSpan.Minutes != 16
                       orderby j.TimeSpan.Minutes
                       select j;

            RunTest(baseline, query);

            //second
            query = from j in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                    where j.TimeSpan.Seconds != 18
                    orderby j.TimeSpan.Seconds
                    select j;

            baseline = from j in baseLineContext.Var1
                       where j.TimeSpan.Seconds != 18
                       orderby j.TimeSpan.Seconds
                       select j;

            RunTest(baseline, query);
        }

        // [TestMethod]
        public void LinqVBSpecificMethods()
        {
            // Strings.Trim
            var query = from t in context.CreateQuery<Team>("Teams")
                        where Microsoft.VisualBasic.Strings.Trim(String.Concat(t.City, " ")) != "Seattle"
                        orderby Microsoft.VisualBasic.Strings.Trim(t.City)
                        select t;

            var baseline = from t in baseLineContext.Teams
                           where Microsoft.VisualBasic.Strings.Trim(String.Concat(t.City, " ")) != "Seattle"
                           orderby Microsoft.VisualBasic.Strings.Trim(t.City)
                           select t;

            RunTest(baseline, query);

            //Strings.Len
            query = from t in context.CreateQuery<Team>("Teams")
                    where Microsoft.VisualBasic.Strings.Len(t.City) != 7
                    orderby Microsoft.VisualBasic.Strings.Len(t.City)
                    select t;

            baseline = from t in baseLineContext.Teams
                       where Microsoft.VisualBasic.Strings.Len(t.City) != 7
                       orderby Microsoft.VisualBasic.Strings.Len(t.City)
                       select t;

            RunTest(baseline, query);

            //Strings.UCase
            query = from t in context.CreateQuery<Team>("Teams")
                    where Microsoft.VisualBasic.Strings.UCase(t.City) != "SEATTLE"
                    orderby Microsoft.VisualBasic.Strings.UCase(t.City) descending
                    select t;

            baseline = from t in baseLineContext.Teams
                       where Microsoft.VisualBasic.Strings.UCase(t.City) != "SEATTLE"
                       orderby Microsoft.VisualBasic.Strings.UCase(t.City) descending
                       select t;

            RunTest(baseline, query);

            //Strings.LCase
            query = from t in context.CreateQuery<Team>("Teams")
                    where Microsoft.VisualBasic.Strings.UCase(t.City) != "seattle"
                    orderby Microsoft.VisualBasic.Strings.UCase(t.City) descending
                    select t;

            baseline = from t in baseLineContext.Teams
                       where Microsoft.VisualBasic.Strings.UCase(t.City) != "seattle"
                       orderby Microsoft.VisualBasic.Strings.UCase(t.City) descending
                       select t;

            RunTest(baseline, query);
        }

        // [TestMethod]
        public void LinqFuncletization()
        {
            // constants
            var query1 = (from t in context.CreateQuery<Team>("Teams")
                          where true
                          orderby "Bad"
                          select t).AsEnumerable().Select(t => 5);

            var baseline1 = from t in baseLineContext.Teams
                            where true
                            orderby "Bad"
                            select 5;

            RunTest(baseline1, query1.AsQueryable());

            // locals
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");

            bool b = true;
            string s = "Bad";
            int i = 4;

            var query2 = ((DataServiceQuery<Team>)(from t in context.CreateQuery<Team>("Teams")
                                                   where b
                                                   orderby s
                                                   select t).Take(i)).Expand("HomeStadium");

            var baseline2 = (from t in baseLineContext.Teams
                             where b
                             orderby s
                             select t).Take(i);

            RunTest(baseline2, query2);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // methods
            int x = 8;

            var query3 = from t in context.CreateQuery<Team>("Teams")
                         where ReturnBool() && ReturnInt(x) == x
                         orderby ReturnString("Bad")
                         select t;

            var baseline3 = from t in baseLineContext.Teams
                            where ReturnBool() && ReturnInt(x) == x
                            orderby ReturnString("Bad")
                            select t;

            RunTest(baseline3, query3.AsQueryable());
        }

        internal bool ReturnBool()
        {
            return true;
        }

        internal int ReturnInt(int i)
        {
            return i;
        }

        internal string ReturnString(string value)
        {
            return value;
        }

        // [TestMethod]
        public void LinqAddQueryOption()
        {
            {
                Trace.WriteLine("simple case - add a couple of options.");
                ReadOnlyTestContext.ClearBaselineIncludes();

                var writer1 = TestURIWriter.CreateURIWriter<League>(context.BaseUri.ToString(), "Leagues").
                    FindByKey("ID", 1).AddParam("foo", 5).AddParam("bar", 6.7f);

                var queryable1 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                            where l.ID == 1
                                                            select l).AddQueryOption("foo", 5).AddQueryOption("bar", 6.7f);

                var baseline1 = from l in baseLineContext.Leagues
                                where l.ID == 1
                                select l;

                CheckURI(writer1, queryable1);
                RunTest(baseline1, queryable1);
            }

            {
                Trace.WriteLine("on root");
                ReadOnlyTestContext.ClearBaselineIncludes();

                var queryable2 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                            select l).AddQueryOption("foo", "myString");

                var baseline2 = from l in baseLineContext.Leagues
                                select l;

                if (new Uri(queryable2.ToString()).Query != "?foo=myString")
                {
                    throw new Exception("Test Failed");
                }

                RunTest(baseline2, queryable2);
            }

            // with navigation & query options
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            var baseQuery = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                       select l).AddQueryOption("foo", "myString").Expand("Players");

            var queryable3 = (from l in baseQuery
                              where l.ID == 1
                              from t in l.Teams
                              where t.TeamID != 1
                              orderby t.TeamID
                              select t).Take(1);

            var baseline3 = (from l in baseLineContext.Leagues
                             where l.ID == 1
                             from t in l.Teams
                             where t.TeamID != 1
                             orderby t.TeamID
                             select t).Take(1);

            if (!new Uri(queryable3.ToString()).Query.Contains("foo=myString"))
            {
                throw new Exception("Test Failed");
            }

            RunTest(baseline3, queryable3);

            ReadOnlyTestContext.ClearBaselineIncludes();

            // strange characters
            ReadOnlyTestContext.ClearBaselineIncludes();

            var writer4 = TestURIWriter.CreateURIWriter<League>(context.BaseUri.ToString(), "Leagues").
                FindByKey("ID", 1).AddParam("a", "\"bad\"").AddParam("bar$", 6.7f).AddParam("55", "lex");

            var queryable4 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                        where l.ID == 1
                                                        select l).AddQueryOption("a", "\"bad\"").AddQueryOption("bar$", 6.7f)
                                                        .AddQueryOption("55", "lex");

            var baseline4 = from l in baseLineContext.Leagues
                            where l.ID == 1
                            select l;

            CheckURI(writer4, queryable4);
            RunTest(baseline4, queryable4);

            Trace.WriteLine("duplicate options");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable5 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                        where l.ID == 1
                                                        select l)
                                                        .AddQueryOption("dupname", 34)
                                                        .AddQueryOption("dupname", 35)
                                                        .AddQueryOption("dupvalue", 101)
                                                        .AddQueryOption("dupvalue2", 101)
                                                        .AddQueryOption("dupvalue2", "101");

            var baseline5 = from l in baseLineContext.Leagues
                            where l.ID == 1
                            select l;


            Assert.AreEqual("?dupname=34,35&dupvalue=101&dupvalue2=101,101", queryable5.RequestUri.Query);
            RunTest(baseline5, queryable5);

            // user specified Astoria option.
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable6 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                        select l)
                                                        .AddQueryOption("$skip", 1)
                                                        .AddQueryOption("$top", 1);

            var baseline6 = (from l in baseLineContext.Leagues
                             select l).Skip(1).Take(1);

            RunTest(baseline6, queryable6);

            Trace.WriteLine("user specified custom query option in $ namespace ");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable7 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                        select l)
                                                        .AddQueryOption("$custom", 1)
                                                        .AddQueryOption("$broken", 1);

            try
            {
                queryable7.GetEnumerator();
            }
            catch (Exception e)
            {
                if (e.Message != "Can't add query option '$custom' because it begins with reserved character '$'.")
                {
                    throw new Exception("Test Failed");
                }
            }


            Trace.WriteLine("user specified Astoria option which is dup");

            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable8 = ((DataServiceQuery<League>)(from l in context.CreateQuery<League>("Leagues")
                                                         select l).Take(1)).AddQueryOption("$skip", 2).AddQueryOption("$filter", 2)
                                                        .AddQueryOption("$top", 2);

            try
            {
                queryable8.GetEnumerator();
            }
            catch (Exception e)
            {
                if (e.Message != "Can't add query option '$top' because it would conflict with the query options from the translated Linq expression.")
                {
                    throw new Exception("Test Failed");
                }
            }

            Trace.WriteLine("user specified just $");
            ReadOnlyTestContext.ClearBaselineIncludes();

            var queryable9 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues")
                                                        select l)
                                                        .AddQueryOption("$", 1)
                                                        .AddQueryOption("$$", 1);

            try
            {
                queryable9.GetEnumerator();
            }
            catch (Exception e)
            {
                if (e.Message != "Can't add query option '$' because it begins with reserved character '$'.")
                {
                    throw new Exception("Test Failed");
                }
            }

            Trace.WriteLine("user specified Astoria option.");
            ReadOnlyTestContext.ClearBaselineIncludes();
            ReadOnlyTestContext.AddBaselineIncludes(typeof(League), "Teams");
            var queryable10 = ((DataServiceQuery<League>)from l in context.CreateQuery<League>("Leagues").Expand("Teams")
                                                        .AddQueryOption("$skip", 1)
                                                        .AddQueryOption("$top", 1)
                                                         select l);

            var baseline10 = (from l in baseLineContext.Leagues
                              select l).Skip(1).Take(1);

            RunTest(baseline10, queryable10);
        }

        [TestMethod]
        public void KeysOnBaseType()
        {
            var queryType = Expression.Parameter(typeof(AllStarTeam), "row");
            var keyProperty = Expression.Property(queryType, typeof(AllStarTeam).GetProperty("TeamID"));
            var equal = Expression.Equal(keyProperty, Expression.Constant(3));
            var whereLambda = Expression.Lambda(equal, queryType);
            var whereCall = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { typeof(AllStarTeam) },
                context.CreateQuery<AllStarTeam>("Teams").Expression, whereLambda);
            var query = context.CreateQuery<AllStarTeam>("Teams").Provider.CreateQuery(whereCall);
            string requestUri = query.ToString();
            Assert.IsFalse(requestUri.Contains("filter"), "request uri should not have a $filter query parameter");
        }

        [TestMethod]
        public void QueryRowCount_UriBuilder()
        {
            IQueryable[] baseQueries = new IQueryable[] {
                context.CreateQuery<League>("Leagues"),
                context.CreateQuery<League>("Leagues").Where(l=>l.ID < 5),
                context.CreateQuery<League>("Leagues").Where(l=>l.ID==1).SelectMany(l=>l.Teams),
                context.CreateQuery<League>("Leagues").Where(l=>l.Name.EndsWith("AAA")).Take(2)
            };

            String[] expectedUris = new String[] {
                "service.svc/Leagues?$count=true",
                "service.svc/Leagues/$count",
                "service.svc/Leagues?$filter=ID lt 5&$count=true",
                "service.svc/Leagues/$count?$filter=ID lt 5",
                "service.svc/Leagues(1)/Teams?$count=true",
                "service.svc/Leagues(1)/Teams/$count",
                "service.svc/Leagues?$filter=endswith(Name,'AAA')&$top=2&$count=true",
                "service.svc/Leagues/$count?$filter=endswith(Name,'AAA')&$top=2"
            };

            MethodInfo mi = typeof(Queryable).GetMethods().Where(m => m.Name == "LongCount" && m.GetParameters().Count() == 1).FirstOrDefault();

            for (int i = 0; i < baseQueries.Length; ++i)
            {
                IQueryable q = baseQueries[i];

                // 1: Inline Counting
                MethodInfo IncludeTotalCountMethod = typeof(DataServiceQuery<>).MakeGenericType(q.ElementType).GetMethod("IncludeTotalCount");
                IQueryable inlineQuery = (IQueryable)IncludeTotalCountMethod.Invoke(q, null);
                string uri = inlineQuery.ToString();
                Assert.IsTrue(uri.EndsWith(expectedUris[i * 2]));

                // 2: Value Counting
                IQueryable valueQuery = q.Provider.CreateQuery(Expression.Call(null, mi.MakeGenericMethod(q.ElementType), q.Expression));
                uri = valueQuery.ToString();
                Assert.IsTrue(uri.EndsWith(expectedUris[i * 2 + 1]));
            }
        }

        // [TestMethod]
        public void ProjectionAndCountTest()
        {
            DataServiceQuery q = (DataServiceQuery)from t in context.CreateQuery<Team>("Teams").IncludeTotalCount()
                                                   select new { TID = t.TeamID };

            // Try both the sync and async version
            var qor = (QueryOperationResponse)q.Execute();
            var qor1 = (QueryOperationResponse)q.EndExecute(q.BeginExecute(null, null));

            Assert.IsTrue(qor.TotalCount == qor1.TotalCount, "The counts must be the same from sync and async versions");
        }

        // [TestMethod]
        public void ResourceBinderAnalyzeProjectionTest()
        {
            var engine = System.Data.Test.Astoria.CombinatorialEngine.FromDimensions(
                new System.Data.Test.Astoria.Dimension("ArgumentCount", new int[] { 1, 2 }),
                new System.Data.Test.Astoria.Dimension("CanProject", new bool[] { true, false }),
                new System.Data.Test.Astoria.Dimension("Cast", new bool[] { true, false }));
            System.Data.Test.Astoria.TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    int argumentCount = (int)values["ArgumentCount"];
                    bool canProject = (bool)values["CanProject"];
                    bool cast = (bool)values["Cast"];

                    IQueryable<Team> q = context.CreateQuery<Team>("Teams");
                    IQueryable<int> qint = null;
                    IQueryable<float> qfloat = null;
                    q = q.Where(o => o.TeamID == 1);
                    if (argumentCount == 1)
                    {
                        if (cast)
                        {
                            qfloat = canProject ? q.Select(o => (float)o.TeamID) : q.Select(o => (float)new Team().TeamID);
                        }
                        else
                        {
                            qint = canProject ? q.Select(o => o.TeamID) : q.Select(o => new Team().TeamID);
                        }
                    }
                    else
                    {
                        // Ignoring cast, as this fails pretty early.
                        Debug.Assert(argumentCount == 2, "argumentCount == 2");
                        qint = canProject ? q.Select((o, position) => o.TeamID + position) : q.Select((o, position) => new Team().TeamID + position);
                    }

                    string url = null;
                    Exception exception = System.Data.Test.Astoria.TestUtil.RunCatching(() =>
                    {
                        if (qint != null)
                        {
                            url = qint.ToString();
                            foreach (var i in qint) { }
                        }
                        else
                        {
                            url = qfloat.ToString();
                            foreach (var f in qfloat) { }
                        }
                    });

                    if (argumentCount == 1 && (!canProject || !cast))
                    {
                        System.Data.Test.Astoria.TestUtil.AssertExceptionExpected(exception, false);
                        // NOTE: should always be Teams(1) when $select=navprop&$expand=navprop pattern is supported
                        System.Data.Test.Astoria.TestUtil.AssertContains(url, "Teams");
                    }
                    else
                    {
                        System.Data.Test.Astoria.TestUtil.AssertExceptionExpected(exception, true);
                        System.Data.Test.Astoria.TestUtil.AssertContains(exception.ToString(), "NotSupportedException");
                    }
                });
        }

        [TestMethod]
        public void ExpressionNestingTest()
        {
            // Ensures that generated expressions in the URI do not contain unnecessary levels of nesting
            // This combination of test cases was generated using pair-wise combination reduction with the
            // following dimensions: precedence, associativity and where the parentheses are.
            Tuple<IQueryable, string>[] queriesAndExpectedUris = new Tuple<IQueryable, string>[]
            {
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID > l.ID + (l.ID * l.ID)),
                    @"/Leagues?$filter=ID gt ID add ID mul ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID > l.ID - (l.ID / l.ID)),
                    @"/Leagues?$filter=ID gt ID sub ID div ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID < (l.ID + l.ID) - l.ID),
                    @"/Leagues?$filter=ID lt ID add ID sub ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID < l.ID / (l.ID * l.ID)),
                    @"/Leagues?$filter=ID lt ID div (ID mul ID)"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>((l.ID == l.ID) && (l.ID >= l.ID)) || (l.ID < l.ID)),
                    @"/Leagues?$filter=ID eq ID and ID ge ID or ID lt ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID == (l.ID + l.ID) * l.ID),
                    @"/Leagues?$filter=ID eq (ID add ID) mul ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>(((((((l.ID * l.ID >= l.ID)))))))),
                    @"/Leagues?$filter=ID mul ID ge ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID >= (l.ID + (l.ID % l.ID))),
                    @"/Leagues?$filter=ID ge ID add ID mod ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID <= l.ID * l.ID * l.ID),
                    @"/Leagues?$filter=ID le ID mul ID mul ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID >= l.ID != false),
                    @"/Leagues?$filter=ID ge ID ne false"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID != l.ID - l.ID * l.ID),
                    @"/Leagues?$filter=ID ne ID sub ID mul ID"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>true == ((l.ID % l.ID) < l.ID) == false),
                    @"/Leagues?$filter=true eq (ID mod ID lt ID) eq false"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID != l.ID * (l.ID - l.ID / (l.ID * (l.ID + l.ID)))),
                    @"/Leagues?$filter=ID ne ID mul (ID sub ID div (ID mul (ID add ID)))"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID > 1 && (((l.ID > 1))) || (l.ID > 1 && l.ID > 1)),
                    @"/Leagues?$filter=ID gt 1 and ID gt 1 or ID gt 1 and ID gt 1"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID > 1 && (l.ID > 1 || l.ID > 1) && l.ID > 1),
                    @"/Leagues?$filter=ID gt 1 and (ID gt 1 or ID gt 1) and ID gt 1"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID > l.ID * (l.ID / l.ID)),
                    @"/Leagues?$filter=ID gt ID mul (ID div ID)"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID > l.ID * (l.ID % l.ID)),
                    @"/Leagues?$filter=ID gt ID mul (ID mod ID)"),
                new Tuple<IQueryable, string>(
                    context.CreateQuery<League>("Leagues").Where(l=>l.ID > l.ID + (l.ID - l.ID)),
                    @"/Leagues?$filter=ID gt ID add (ID sub ID)"),
            };

            foreach (var queryAndUri in queriesAndExpectedUris)
            {
                IQueryable query = queryAndUri.Item1;
                string expected = queryAndUri.Item2;
                string uri = query.ToString();

                Assert.IsTrue(uri.EndsWith(expected), string.Format("Uri {0} does not end with {1}.", uri, expected));
            }
        }

        private static void VerifyMimeTypeForCountRequests(object sender, SendingRequest2EventArgs args)
        {
            if (args.RequestMessage.Url.AbsoluteUri.Contains("/$count"))
            {
                Assert.AreEqual(args.RequestMessage.GetHeader("Accept"), "text/plain", "the accept header should be set before firing sending request event");
            }
        }

        #region Projection Client Service Operations cross feature testing

        public class Projections_ServiceOperations_NorthWindDataContextService : NorthwindDefaultTempDbService
        {
            [WebGet]
            public IQueryable<NorthwindModel.Customers> GetCustomersByCity(string city)
            {
                return this.CurrentDataSource.Customers.Where(c => c.City == city).AsQueryable();
            }

            [WebGet]
            public IQueryable<NorthwindModel.Customers> LondonCustomers()
            {
                return GetCustomersByCity("London");
            }
        }

        // [TestMethod]
        public void ProjectionClient_X_ServiceOp()
        {
            using (System.Data.Test.Astoria.TestUtil.MetadataCacheCleaner())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(Projections_ServiceOperations_NorthWindDataContextService);
                request.StartService();
                var ctx = new DataServiceContext(request.ServiceRoot);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                ctx.MergeOption = Microsoft.OData.Client.MergeOption.NoTracking;
                ctx.Credentials = System.Net.CredentialCache.DefaultCredentials;

                //Queries without a projection
                //LondonCustomers using DataServiceContext.CreateQuery()
                var qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers")
                                 where c.CustomerID == "AROUT"
                                 select c;
                var customer = qCustomers.First();
                Assert.IsTrue(customer.City == "London", "customer.City == 'London'");
                Assert.IsTrue(customer.Orders.Count == 0, "Orders are not materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers").Expand("Orders")
                             where c.CustomerID == "AROUT"
                             select c;
                customer = qCustomers.First();
                Assert.IsTrue(customer.City == "London", "customer.City == 'London'");
                Assert.AreEqual(6, customer.Orders.Count, "Orders are materialized");
                Assert.IsTrue(customer.Orders.All(o => o.Order_Details.Count == 0), "Orders_Details are not materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers").Expand("Orders").Expand("Orders($expand=Order_Details)")
                             where c.CustomerID == "AROUT"
                             select c;
                customer = qCustomers.First();
                Assert.IsTrue(customer.City == "London", "customer.City == 'London'");
                Assert.AreEqual(6, customer.Orders.Count, "Orders are materialized");
                Assert.IsTrue(customer.Orders.First().Order_Details.Count != 0, "Orders_Details are materialized");

                var qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("LondonCustomers('AROUT')/Orders") select o;
                var orderList = qOrders.ToList();
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.Customers == null), "Customers references are not materialized");

                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("LondonCustomers('Huseyin')/Orders") select o;
                orderList = qOrders.ToList();
                Assert.IsTrue(orderList.Count == 0, "orderList.Count == 0");

                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("LondonCustomers('AROUT')/Orders").Expand("Customers") select o;
                orderList = qOrders.ToList();
                customer = orderList[0].Customers;
                Assert.IsTrue(customer.CustomerID == "AROUT", "customer.CustomerID == 'AROUT'");
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.Customers.CustomerID == "AROUT"), "Customers references are materialized");
                Assert.IsTrue(orderList.All(o => o.Order_Details.Count == 0), "Order_Details are not materialized");

                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("LondonCustomers('AROUT')/Orders").Expand("Customers,Order_Details") select o;
                orderList = qOrders.ToList();
                customer = orderList[0].Customers;
                Assert.IsTrue(customer.CustomerID == "AROUT", "customer.CustomerID == 'AROUT'");
                Assert.AreEqual(6, orderList.Count , "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.Customers.CustomerID == "AROUT"), "Customers references are materialized");
                Assert.IsTrue(orderList.Any(o => o.Order_Details.Count != 0), "Order_Details are materialized");


                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers") select c;
                var customerList = qCustomers.ToList();
                Assert.IsTrue(customerList.All(c => c.City == "London"), "customerList.All(c=>c.City =='London')");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.IsTrue(customerList.All(c => c.Orders.Count == 0), "Orders are not materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers").Expand("Orders") select c;
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList.All(c => c.City == "London"), "customerList.All(c=>c.City =='London')");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.AreEqual(6, customerList.First().Orders.Count, "Orders are materialized");
                Assert.IsTrue(customerList.First().Orders.All(o => o.Order_Details.Count() == 0), "Orders details are not materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers").Expand("Orders,Orders($expand=Order_Details)") select c;
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList.All(c => c.City == "London"), "customerList.All(c=>c.City =='London')");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.AreEqual(6, customerList.First().Orders.Count, "Orders are materialized");
                Assert.IsTrue(customerList.First().Orders.Any(o => o.Order_Details.Count() > 0), "Orders details are materialized");

                //GetCustomersByCity("London") using DataServiceContext.CreateQuery()
                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("GetCustomersByCity").AddQueryOption("city", "'London'")
                             where c.City == "London"
                             select c;
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList[0].City == "London", "customerList[0].City =='London'");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.IsTrue(customerList[0].Orders.Count == 0, "Orders are not materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("GetCustomersByCity").AddQueryOption("city", "'London'") select c;
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList[0].City == "London", "customerList[0].City =='London'");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.IsTrue(customerList[0].Orders.Count == 0, "Orders are not materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("GetCustomersByCity").Expand("Orders").AddQueryOption("city", "'London'") select c;
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList[0].City == "London", "customerList[0].City =='London'");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.AreEqual(6, customerList[0].Orders.Count, "Orders are materialized");
                Assert.IsTrue(customerList[0].Orders.Any(o => o.Order_Details.Count == 0), "Orders_Details are not materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("GetCustomersByCity").Expand("Orders,Orders($expand=Order_Details)").AddQueryOption("city", "'London'") select c;
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList[0].City == "London", "customerList[0].City =='London'");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.AreEqual(6, customerList[0].Orders.Count, "Orders are materialized");
                Assert.IsTrue(customerList[0].Orders.Any(o => o.Order_Details.Count != 0), "Orders_Details are materialized");

                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("GetCustomersByCity('AROUT')/Orders").AddQueryOption("city", "'London'").Expand("Customers") select o;
                orderList = qOrders.ToList();
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.Customers.CustomerID == "AROUT"), "o=>o.Customers.CustomerID == 'AROUT'");

                //Queries with Projection

                //LondonCustomers using DataServiceQuery.CreateQuery and projections
                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers")
                             select new northwindClient.Customers()
                             {
                                 CustomerID = c.CustomerID,
                                 City = c.City,
                                 Orders = c.Orders
                             };
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList.All(c => c.City == "London" && c.CustomerID != null
                                               && c.Address == null && c.CompanyName == null
                                               && c.ContactTitle == null), "customerList.All(c=>c.City =='London') && only specified parts are materialized");
                Assert.AreEqual(3, customerList.Count(), "customerList.Count == 3");
                Assert.IsTrue(customerList.Any(c => c.Orders.Count != 0), "Orders are materialized");

                qCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers")
                             where c.CustomerID == "AROUT"
                             select new northwindClient.Customers()
                             {
                                 CustomerID = c.CustomerID,
                                 City = c.City,
                                 Orders = c.Orders
                             };
                customerList = qCustomers.ToList();
                Assert.IsTrue(customerList.All(c => c.City == "London" && c.CustomerID == "AROUT"
                                               && c.Address == null && c.CompanyName == null
                                               && c.ContactTitle == null), "customerList.All(c=>c.City =='London') && only specified parts are materialized");
                Assert.AreEqual(1, customerList.Count(), "customerList.Count == 1");
                Assert.AreEqual(6, customerList.Single().Orders.Count, "Orders are materialized");

                //LondonCustomers using DataServiceQuery.CreateQuery and projections
                var qAnonymousCustomers = from c in ctx.CreateQuery<northwindClient.Customers>("LondonCustomers")
                                          where c.CustomerID == "AROUT"
                                          select new
                                          {
                                              CustomerID = c.CustomerID,
                                              City = c.City,
                                              Orders = from o in c.Orders select new northwindClient.Orders() { OrderID = o.OrderID }
                                          };
                var anonymousCustomerList = qAnonymousCustomers.ToList();
                Assert.IsTrue(anonymousCustomerList.Count == 1, "customerList.Count == 1");
                var aCustomer = anonymousCustomerList[0];
                Assert.IsTrue(aCustomer.City == "London" && aCustomer.CustomerID == "AROUT", "aCustomer.City == 'London' && aCustomer.CustomerID == 'AROUT'");
                orderList = anonymousCustomerList[0].Orders.ToList();
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.OrderID != 0 && o.OrderDate == null && o.ShipAddress == null && o.ShipName == null), "only OrderID is materialized");


                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("LondonCustomers('AROUT')/Orders")
                          select new northwindClient.Orders()
                          {
                              OrderID = o.OrderID,
                              Customers = o.Customers,
                          };
                orderList = qOrders.ToList();
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.Any(o => o.OrderID != 0 && o.Customers.CustomerID == "AROUT" && o.OrderDate == null && o.ShipAddress == null && o.ShipName == null), "only OrderID and Customers are materialized");

                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("GetCustomersByCity('AROUT')/Orders").AddQueryOption("city", "'London'")
                          select new northwindClient.Orders()
                          {
                              OrderID = o.OrderID,
                              Customers = o.Customers,
                          };
                orderList = qOrders.ToList();
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.OrderID != 0 && o.Customers.CustomerID == "AROUT" && o.OrderDate == null && o.ShipAddress == null && o.ShipName == null), "only OrderID and Customers are materialized");

                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("GetCustomersByCity('AROUT')/Orders").AddQueryOption("city", "'London'")
                          select new northwindClient.Orders()
                          {
                              OrderID = o.OrderID,
                              Customers = new northwindClient.Customers() { CustomerID = o.Customers.CustomerID },
                          };
                orderList = qOrders.ToList();
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.OrderID != 0 && o.Customers.CustomerID == "AROUT" && o.Customers.ContactName == null && o.Customers.Address == null
                                            && o.OrderDate == null && o.ShipAddress == null && o.ShipName == null), "only OrderID and Customers are materialized");

                qOrders = from o in ctx.CreateQuery<northwindClient.Orders>("GetCustomersByCity('AROUT')/Orders").AddQueryOption("city", "'London'")
                          where o.Employees.EmployeeID == 1
                          select new northwindClient.Orders()
                          {
                              OrderID = o.OrderID,
                              Customers = new northwindClient.Customers() { CustomerID = o.Customers.CustomerID },
                          };
                orderList = qOrders.ToList();
                Assert.AreEqual(6, orderList.Count, "orderList.Count == 6");
                Assert.IsTrue(orderList.All(o => o.OrderID != 0 && o.Customers.CustomerID == "AROUT" && o.Customers.ContactName == null && o.Customers.Address == null
                                            && o.OrderDate == null && o.ShipAddress == null && o.ShipName == null && o.Employees == null), "only OrderID and Customers are materialized");
            }
        }

        #endregion

        #region Helpers
        internal void CheckURI(TestURIWriter writer, IQueryable linqQuery)
        {
            Uri u1 = new Uri(writer.GetUri());
            Uri u2 = new Uri(linqQuery.ToString());

            if (Uri.Compare(u1, u2, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.Ordinal) != 0)
            {
                throw new Exception("Uris don't match");
            }

        }

        public static void RunTest(IQueryable baseline, IQueryable linqQuery)
        {
            RunTest(baseline, linqQuery, false);
        }

        internal static void RunTest(IEnumerable baseline, IQueryable linqQuery, bool ignoreTypes)
        {
            string description = "Comparing " + baseline.ToString() + " to " + linqQuery.ToString() + "...";
            System.Data.Test.Astoria.TestUtil.TraceScopeForException(description, delegate()
            {
                try
                {
                    IEnumerator l = null;
                    IEnumerator r = null;
                    try
                    {
                        l = baseline.GetEnumerator();
                        r = linqQuery.GetEnumerator();
                        VerifyResults(l, r, ignoreTypes);
                    }
                    finally
                    {
                        if (l is IDisposable) ((IDisposable)l).Dispose();
                        if (r is IDisposable) ((IDisposable)r).Dispose();
                    }
                }
                catch (System.Net.WebException exception)
                {
                    Trace.WriteLine("Exception caught: " + exception);
                    if (exception.Response != null)
                    {
                        using (System.IO.Stream stream = exception.Response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                Trace.WriteLine(new System.IO.StreamReader(stream).ReadToEnd());
                            }
                        }
                    }
                    throw;
                }
            });
        }

        internal static void VerifyResults(IEnumerator l, IEnumerator r, bool ignoreTypes)
        {
            while (true)
            {
                bool lMoved, rMoved;
                bool lMovedOK = false;
                bool rMovedOK = false;
                try
                {
                    lMoved = l.MoveNext();
                    lMovedOK = true;
                    rMoved = r.MoveNext();
                    rMovedOK = true;
                }
                finally
                {
                    if (!lMovedOK)
                    {
                        Trace.WriteLine("Problem advancing the left query (LINQ-to-Objects baseline).");
                    }
                    else if (!rMovedOK)
                    {
                        Trace.WriteLine("Problem advancing the right query (LINQ-to-Astoria).");
                    }
                }

                if (!lMoved && !rMoved)
                {
                    break;
                }

                Assert.AreEqual(lMoved, rMoved,
                    "Both left(" + lMoved + ") and right(" + rMoved + ") enumerators moved/stopped together.");
                Debug.Assert(lMoved && rMoved);

                if (l.Current == null && r.Current == null)
                {
                    continue;
                }

                if (l.Current == null || r.Current == null)
                {
                    Assert.Fail("Only one null between left (" + l.Current + ") and right (" + r.Current + ")");
                }

                if (l.Current is IEnumerable && r.Current is IEnumerable)
                {
                    VerifyResults(((IEnumerable)l.Current).GetEnumerator(), ((IEnumerable)r.Current).GetEnumerator(), ignoreTypes);
                    continue;
                }

                if (!ignoreTypes)  //for Anonymous types, need to compare directly inst
                {
                    Assert.AreEqual(l.Current.GetType(), r.Current.GetType());
                    Assert.IsTrue(l.Current.Equals(r.Current), "Left value(" + l.Current + ") equals right value(" + r.Current + ")");
                }
                else
                {
                    Assert.IsTrue(CompareObjects(l.Current, r.Current, new List<object>()), "Left value(" + l.Current + ") equals right value(" + r.Current + ")");
                }
            }

            if (l.MoveNext() || r.MoveNext())
            {
                throw new Exception("Test Failed");
            }
        }

        internal static bool CompareObjects(object l, object r, List<object> seenobjects)
        {
            PropertyInfo[] leftProperties = l.GetType().GetProperties();
            var rightProperties = r.GetType().GetProperties().ToDictionary(x => x.Name);

            if (leftProperties.Count() != rightProperties.Keys.Count())
                return false;

            foreach (PropertyInfo lpi in leftProperties)
            {
                PropertyInfo rpi;
                if (!rightProperties.TryGetValue(lpi.Name, out rpi))
                    return false;

                if (lpi.PropertyType != rpi.PropertyType)
                    return false;

                if (lpi.PropertyType.IsValueType || lpi.PropertyType.Namespace == "System")
                {
                    object lv = lpi.GetValue(l, null);
                    object rv = rpi.GetValue(r, null);

                    if ((lv == null && rv != null) || (lv != null && rv == null))
                        return false;

                    if (lv == null && rv == null)
                        continue;

                    if (!lv.Equals(rv))
                        return false;
                }
                else
                {
                    object lv = lpi.GetValue(l, null);
                    object rv = rpi.GetValue(r, null);

                    if ((lv == null && rv != null) || (lv != null && rv == null))
                        return false;

                    if (lv == null && rv == null)
                        continue;

                    if (seenobjects.Contains(lv) || seenobjects.Contains(rv))
                        continue;

                    seenobjects.Add(lv);
                    seenobjects.Add(rv);

                    if (lv is IEnumerable && rv is IEnumerable)
                    {
                        var lit = (lv as IEnumerable).GetEnumerator();
                        var rit = (rv as IEnumerable).GetEnumerator();

                        while (true)
                        {
                            bool lMoved = lit.MoveNext();
                            bool rMoved = rit.MoveNext();

                            if (!lMoved && !rMoved)
                            {
                                break;
                            }

                            Assert.AreEqual(lMoved, rMoved);

                            if (lit.Current == null && rit.Current == null)
                                continue;

                            if ((lit.Current == null && rit.Current != null) || (lit.Current != null && rit.Current == null))
                                return false;

                            if (seenobjects.Contains(lit.Current) || seenobjects.Contains(rit.Current))
                                continue;

                            seenobjects.Add(lit.Current);
                            seenobjects.Add(rit.Current);

                            if (lit.Current.GetType().IsValueType || lit.Current.GetType().Namespace == "System")
                            {

                                if (!lit.Current.Equals(rit.Current))
                                    return false;
                            }
                            else
                            {
                                Assert.IsTrue(CompareObjects(lit.Current, rit.Current, seenobjects));
                            }
                        }
                    }
                    else if (!CompareObjects(lv, rv, seenobjects))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private class LinqTestCase
        {
            public LinqTestCase()
            {
            }

            public IQueryable Query { get; set; }
            public string ExpectedUri { get; set; }
            public IQueryable ExpectedResult { get; set; }
        }

        private static void TestLinqQueries(LinqTestCase[] testCases)
        {
            var baseUri = context.BaseUri.AbsoluteUri;

            foreach (var testCase in testCases)
            {
                string expectedUri = string.Format("{0}/Teams{1}", baseUri, testCase.ExpectedUri);
                Assert.AreEqual(expectedUri, testCase.Query.ToString(), "LINQ query did not produce the expected URI.");
                RunTest(testCase.ExpectedResult, testCase.Query);
            }
        }
        #endregion

    }
}
