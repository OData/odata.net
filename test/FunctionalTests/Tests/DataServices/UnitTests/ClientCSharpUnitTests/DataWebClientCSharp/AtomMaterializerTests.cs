//---------------------------------------------------------------------
// <copyright file="AtomMaterializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    #endregion Namespaces

    [TestClass]
    public class AtomMaterializerTests
    {
        private Uri serviceRoot;
        private DataServiceContext context;

        internal const string FeedStart = AtomParserTests.FeedStart;

        internal static Dictionary<string, string> EmptyHeaders
        {
            [DebuggerStepThrough]
            get { return AtomParserTests.EmptyHeaders; }
        }

        [TestInitialize]
        public void Initialize()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
            this.serviceRoot = new Uri("http://localhost/");
            this.context = new DataServiceContext(serviceRoot);
            this.context.EnableAtom = true;
        }

        #region AtomMaterializer tests.

        [TestMethod]
        public void AtomMaterializerInitializeEntity()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("EntryIsNull", new bool[] { true, false }),
                new Dimension("AlreadyResolved", new bool[] { true, false }),
                new Dimension("UpdateLocal", new bool[] { true, false }),
                new Dimension("MergeOption", Enum.GetValues(typeof(MergeOption))),
                new Dimension("TypeChange", new bool[] { true, false }),
                new Dimension("HasContinuation", new bool[] { true, false }));
            TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    bool entryIsNull = (bool)values["EntryIsNull"];
                    bool alreadyResolved = (bool)values["AlreadyResolved"];
                    MergeOption option = (MergeOption)values["MergeOption"];
                    bool typeChange = (bool)values["TypeChange"];
                    bool hasContinuation = (bool)values["HasContinuation"];
                    bool updateLocal = (bool)values["UpdateLocal"];

                    context.ResolveType = null;

                    string continuationXml = hasContinuation ? "<link rel='next' href='http://next/' />" : "";
                    string leafEntry = AnyEntry("l1", "<d:ID>10</d:ID><d:Member>10</d:Member>", null);
                    string collectionEntry = alreadyResolved ? leafEntry : leafEntry.Replace('1', '2');
                    string collection = FeedStart + collectionEntry + continuationXml + "</feed>";
                    string entry = AnyEntry("e1", "<d:ID>1</d:ID>",
                        "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='foo' />" +
                        LinkEntry("Member", entryIsNull ? "" : leafEntry) + LinkFeed("Member2", collection));
                    string xml = FeedStart + entry + "</feed>";
                    bool resolveTypeInvoked = false;

                    if (typeChange)
                    {
                        context.ResolveType += (n) =>
                            {
                                resolveTypeInvoked = true;
                                return typeof(DoubleMemberTypedEntity<int, TypedEntity<int, int>, List<TypedEntity<int, int>>>);
                            };
                    }

                    this.context.MergeOption = MergeOption.AppendOnly;
                    var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, TypedEntity<int, int>, List<TypedEntity<int, int>>>>("T")
                            select new DoubleMemberTypedEntity<int, TypedEntity<int, int>, List<TypedEntity<int, int>>>()
                            {
                                ID = t.ID,
                                Member = (t.Member == null) ? null : new TypedEntity<int, int>()
                                {
                                    ID = t.Member.ID,
                                    Member = t.Member.Member
                                },
                                Member2 = t.Member2
                            };

                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    {
                        Assert.IsNotNull(item, "item");
                        Assert.AreEqual(1, item.ID, "item.ID");
                        if (entryIsNull)
                        {
                            Assert.IsNull(item.Member, "item.Member");
                        }
                        else
                        {
                            Assert.IsNotNull(item.Member, "item.Member");
                            Assert.AreEqual(10, item.Member.ID, "item.Member.ID");
                        }

                        Assert.IsNotNull(item.Member2, "item.Member2");

                        Assert.AreEqual(1, item.Member2.Count, "item.Member2.Count");
                        Assert.AreEqual((alreadyResolved) ? 10 : 20, item.Member2[0].ID, "item.Member2[0].ID");

                        if (updateLocal && !entryIsNull)
                        {
                            this.context.UpdateObject(item.Member);
                        }

                        Assert.IsTrue(resolveTypeInvoked == typeChange, "resolveTypeInvoked == typeChange");
                    }

                    int expectedEntities = alreadyResolved ? 2 : 3;
                    if (entryIsNull && !alreadyResolved)
                    {
                        expectedEntities--;
                    }

                    AssertEntityCount(expectedEntities, "already resolved (an item is duplicate) is " + alreadyResolved + ", expecting root + child(ren)");
                    AssertLinkCount(2, "root to each item, one through each property");

                    resolveTypeInvoked = false;
                    this.context.MergeOption = option;
                    xml = xml.Replace("<d:ID>10</d:ID>", "<d:ID>100</d:ID>");
                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    {
                        Assert.IsNotNull(item, "item");
                        Assert.AreEqual(1, item.ID, "item.ID");

                        int expectedItemId;
                        if (option == MergeOption.AppendOnly ||
                            (option == MergeOption.PreserveChanges && updateLocal && !entryIsNull))
                        {
                            expectedItemId = 10;
                        }
                        else
                        {
                            expectedItemId = 100;
                        }

                        if (entryIsNull)
                        {
                            Assert.IsNull(item.Member, "item.Member");
                        }
                        else
                        {
                            Assert.IsNotNull(item.Member, "item.Member");
                            Assert.AreEqual(expectedItemId, item.Member.ID, "item.Member.ID");
                        }

                        Assert.IsNotNull(item.Member2, "item.Member2");

                        Assert.AreEqual(1, item.Member2.Count, "item.Member2.Count");
                        Assert.AreEqual((alreadyResolved) ? expectedItemId : 20, item.Member2[0].ID, "item.Member2[0].ID");

                        Assert.IsTrue(resolveTypeInvoked == typeChange, "resolveTypeInvoked == typeChange");
                    }

                    AssertEntityCount(expectedEntities, "When already resolved (an item is duplicate) is " + alreadyResolved + ", expecting root + child(ren)");
                    AssertLinkCount(2, "Root to each item, one through each property");

                    context.ResolveType = null;
                    this.ClearContext();
                });
        }

        [TestMethod]
        public void MaterializerMergeEPMnProjection()
        {
            bool hasContinuation = false;
            bool alreadyResolved = false;
            bool entryIsNull = false;
            bool typeChange = true;
            bool updateLocal = true;
            MergeOption option = MergeOption.OverwriteChanges;

            string continuationXml = hasContinuation ? "<link rel='next' href='http://next/' />" : "";
            string leafEntry = AnyEntry("l1", "<d:ID>10</d:ID><d:Member>10</d:Member>", null);
            string collectionEntry = alreadyResolved ? leafEntry : leafEntry.Replace('1', '2');
            string collection = FeedStart + collectionEntry + continuationXml + "</feed>";
            string entry = AnyEntry("e1", "<d:ID>1</d:ID>",
                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='foo' />" +
                LinkEntry("Member", entryIsNull ? "" : leafEntry) + LinkFeed("Member2", collection));
            string xml = FeedStart + entry + "</feed>";
            bool resolveTypeInvoked = false;

            if (typeChange)
            {
                context.ResolveType += (n) =>
                {
                    resolveTypeInvoked = true;
                    return typeof(DoubleMemberTypedEntity<int, TypedEntity<int, int>, List<TypedEntity<int, int>>>);
                };
            }

            this.context.MergeOption = MergeOption.AppendOnly;
            var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, TypedEntity<int, int>, List<TypedEntity<int, int>>>>("T")
                    select new DoubleMemberTypedEntity<int, TypedEntity<int, int>, List<TypedEntity<int, int>>>()
                    {
                        ID = t.ID,
                        Member = (t.Member == null) ? null : new TypedEntity<int, int>()
                        {
                            ID = t.Member.ID,
                            Member = t.Member.Member
                        },
                        Member2 = t.Member2
                    };

            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item, "item");
                Assert.AreEqual(1, item.ID, "item.ID");
                if (entryIsNull)
                {
                    Assert.IsNull(item.Member, "item.Member");
                }
                else
                {
                    Assert.IsNotNull(item.Member, "item.Member");
                    Assert.AreEqual(10, item.Member.ID, "item.Member.ID");
                }

                Assert.IsNotNull(item.Member2, "item.Member2");

                Assert.AreEqual(1, item.Member2.Count, "item.Member2.Count");
                Assert.AreEqual((alreadyResolved) ? 10 : 20, item.Member2[0].ID, "item.Member2[0].ID");

                if (updateLocal && !entryIsNull)
                {
                    this.context.UpdateObject(item.Member);
                }

                Assert.IsTrue(resolveTypeInvoked == typeChange, "resolveTypeInvoked == typeChange");
            }

            int expectedEntities = alreadyResolved ? 2 : 3;
            if (entryIsNull && !alreadyResolved)
            {
                expectedEntities--;
            }

            AssertEntityCount(expectedEntities, "already resolved (an item is duplicate) is " + alreadyResolved + ", expecting root + child(ren)");
            AssertLinkCount(2, "root to each item, one through each property");

            resolveTypeInvoked = false;
            this.context.MergeOption = option;
            xml = xml.Replace("<d:ID>10</d:ID>", "<d:ID>100</d:ID>");
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item, "item");
                Assert.AreEqual(1, item.ID, "item.ID");

                int expectedItemId;
                if (option == MergeOption.AppendOnly ||
                    (option == MergeOption.PreserveChanges && updateLocal && !entryIsNull))
                {
                    expectedItemId = 10;
                }
                else
                {
                    expectedItemId = 100;
                }

                if (entryIsNull)
                {
                    Assert.IsNull(item.Member, "item.Member");
                }
                else
                {
                    Assert.IsNotNull(item.Member, "item.Member");
                    Assert.AreEqual(expectedItemId, item.Member.ID, "item.Member.ID");
                }

                Assert.IsNotNull(item.Member2, "item.Member2");

                Assert.AreEqual(1, item.Member2.Count, "item.Member2.Count");
                Assert.AreEqual((alreadyResolved) ? expectedItemId : 20, item.Member2[0].ID, "item.Member2[0].ID");

                Assert.IsTrue(resolveTypeInvoked == typeChange, "resolveTypeInvoked == typeChange");
            }

            AssertEntityCount(expectedEntities, "When already resolved (an item is duplicate) is " + alreadyResolved + ", expecting root + child(ren)");
            AssertLinkCount(2, "Root to each item, one through each property");

            context.ResolveType = null;
        }

        [TestMethod]
        public void AtomMaterializerForPath()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            context.MergeOption = MergeOption.NoTracking;

            string stadium = AnyEntry("1", "<d:ID>1</d:ID><d:Name>Stadium 1</d:Name>", null);
            string players = FeedStart +
                AnyEntry("p1", "<d:ID>10</d:ID>", null) +
                AnyEntry("p2", "<d:ID>20</d:ID>", null) +
                "</feed>";
            string playersWithContinuation = FeedStart +
                AnyEntry("p1", "<d:ID>10</d:ID>", null) +
                AnyEntry("p2", "<d:ID>20</d:ID>", null) +
                "<link rel='next' href='http://next/' />" +
                "</feed>";

            {
                Trace.WriteLine("Leaf materialization.");
                string xml = FeedStart + AnyEntry("10", "<d:TeamID>10</d:TeamID>", LinkEntry("HomeStadium", stadium) + LinkFeed("Players", players)) + "</feed>";
                var q = from t in context.CreateQuery<Team>("T")
                        select new
                        {
                            team = t,
                            team2 = new Team() { Players = t.Players },
                            team3 = new Team() { HomeStadium = t.HomeStadium }
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    // Because of identity resolution, all team instances are actually the same object.

                    Assert.IsNotNull(item, "item");

                    Assert.IsNotNull(item.team, "item.team");
                    Assert.IsNotNull(item.team.HomeStadium, "item.team.HomeStadium");
                    Assert.IsNotNull(item.team.Players, "item.team.Players");
                    Assert.AreEqual(10, item.team.TeamID, "item.team.TeamID");

                    Assert.AreEqual(1, item.team.HomeStadium.ID, "item.team.HomeStadium.ID");
                    Assert.AreEqual("Stadium 1", item.team.HomeStadium.Name, "item.team.HomeStadium.Name");

                    Assert.AreEqual(2, item.team.Players.Count, "item.team.Players.Count");

                    Assert.IsNotNull(item.team2, "item.team2");
                    Assert.AreEqual(item.team, item.team2, "item.team and item.team2");
                    Assert.AreEqual(item.team, item.team3, "item.team and item.team3");
                }
            }

            {
                Trace.WriteLine("Feed materialization.");
                string xml = FeedStart + AnyEntry("10", "<d:TeamID>10</d:TeamID>", LinkEntry("HomeStadium", stadium) + LinkFeed("Players", playersWithContinuation)) + "</feed>";
                var q = from t in context.CreateQuery<Team>("T")
                        select new
                        {
                            players = t.Players,
                            players2 = t.Players.Select(p => p),
                            players3 = t.Players.ToList(),
                            players4 = t.Players.Select(p => p).ToList(),
                            players5 = new DataServiceCollection<Player>(t.Players, TrackingMode.None),
                            players5_0 = new DataServiceCollection<Player>(t.Players),
                            players5_1 = new DataServiceCollection<Player>(t.Players, TrackingMode.AutoChangeTracking, "Players", (e) => false, (e2) => false),
                            players6 = new DataServiceCollection<Player>(t.Players.Select(p => p), TrackingMode.None),
                            players7 = new DataServiceCollection<Player>(t.Players.Select(p => p), TrackingMode.None),
                            players8 = t.Players.Select(p => p.ID),
                            players9 = t.Players.Select(p => p.ID).ToList(),
                            playersA = new DataServiceCollection<int>(t.Players.Select(p => p.ID), TrackingMode.None),
                        };
                var qor = CreateTestQueryOperationResponse(xml, q);
                foreach (var item in qor)
                {
                    Assert.IsNotNull(item, "item");

                    Assert.IsNotNull(item.players, "item.players");
                    Assert.IsNotNull(item.players2, "item.players2");
                    Assert.IsNotNull(item.players3, "item.players3");
                    Assert.IsNotNull(item.players4, "item.players4");
                    Assert.IsNotNull(item.players5, "item.players5");
                    Assert.IsNotNull(item.players5_0, "item.players5_0");
                    Assert.IsNotNull(item.players5_1, "item.players5_1");
                    Assert.IsNotNull(item.players6, "item.players6");
                    Assert.IsNotNull(item.players7, "item.players7");
                    Assert.IsNotNull(item.players8, "item.players8");
                    Assert.IsNotNull(item.players9, "item.players9");
                    Assert.IsNotNull(item.playersA, "item.playersA");

                    Player[] playerInstances = new Player[2];
                    foreach (var p in item.GetType().GetProperties())
                    {
                        var enumerable = p.GetValue(item, null) as System.Collections.IEnumerable;
                        if (enumerable == null) continue;

                        Trace.WriteLine("Checking " + p.Name);

                        int count = 0;
                        foreach (object o in enumerable)
                        {
                            count++;
                            Player player = o as Player;
                            if (player != null)
                            {
                                if (playerInstances[count - 1] == null)
                                {
                                    playerInstances[count - 1] = player;
                                }
                                else
                                {
                                    Assert.AreSame(playerInstances[count - 1], player, "player from enumeration");
                                }

                                Assert.AreEqual(10 * count, player.ID, "player.ID");
                                Exception exception = TestUtil.RunCatching(() => { qor.GetContinuation(enumerable); });
                                TestUtil.AssertExceptionExpected(exception, false);
                                if (enumerable is DataServiceCollection<Player>)
                                {
                                    exception = TestUtil.RunCatching(() =>
                                    {
                                        var c = ((DataServiceCollection<Player>)enumerable).Continuation;
                                        Assert.IsNotNull(c, "All players collections should have continuation.");
                                    });
                                    TestUtil.AssertExceptionExpected(exception, false);
                                }
                            }
                            else
                            {
                                int id = (int)o;
                                Assert.AreEqual(playerInstances[count - 1].ID, id, "projected id");
                            }
                        }

                        Assert.AreEqual(2, count, "count of elements in " + p.Name);
                    }
                }
            }
        }

        [TestMethod]
        public void PrimitiveTypes_Generated_WithIncorrectNullability()
        {
            string orderPayloadWithNullableProperties = @"HTTP/1.1 200 OK
Proxy-Connection: Keep-Alive
Connection: Keep-Alive
Content-Length: 2542
Via: 1.1 TK5-PRXY-08
Expires: Wed, 04 Jan 2012 23:39:02 GMT
Date: Wed, 04 Jan 2012 23:38:02 GMT
Content-Type: application/atom+xml;charset=utf-8
Server: Microsoft-IIS/7.5
Cache-Control: private
Vary: *
OData-Version: 4.0;
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET

<?xml version='1.0' encoding='utf-8' standalone='yes'?>
<feed xml:base='http://services.odata.org/Northwind/Northwind.svc/' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
  <title type='text'>OrderDetails</title>
  <id>http://services.odata.org/Northwind/Northwind.svc/OrderDetails</id>
  <updated>2012-01-04T23:32:26Z</updated>
  <link rel='self' title='OrderDetails' href='OrderDetails' />
  <entry>
    <id>http://services.odata.org/Northwind/Northwind.svc/OrderDetails(10248)</id>
    <title type='text'></title>
    <updated>2012-01-04T23:32:26Z</updated>
    <author>
      <name />
    </author>
    <link rel='edit' title='OrderDetail' href='OrderDetails(10248)' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/Customer' type='application/atom+xml;type=entry' title='Customer' href='OrderDetails(10248)/Customer' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/Employee' type='application/atom+xml;type=entry' title='Employee' href='OrderDetails(10248)/Employee' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/OrderDetail_Details' type='application/atom+xml;type=feed' title='OrderDetail_Details' href='OrderDetails(10248)/OrderDetail_Details' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/Shipper' type='application/atom+xml;type=entry' title='Shipper' href='OrderDetails(10248)/Shipper' />
    <category term='NorthwindModel.OrderDetail' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
      <m:properties>
        <d:OrderID m:type='Edm.Int32'>10248</d:OrderID>
        <d:ProductID m:type='Edm.Int32'>10248</d:ProductID>
        <d:UnitPrice m:null='true'/>
        <d:Quantity m:null='true'/> 
      </m:properties>
    </content>
  </entry>
</feed>";


            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();
            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();
                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                playbackService.OverridingPlayback = orderPayloadWithNullableProperties;
                InvalidOperationException nullAssignmentError = null;
                try
                {
                    foreach (var orderDetail in ctx.CreateQuery<OrderDetail>("OrderDetails"))
                    {
                    }
                }
                catch (InvalidOperationException ioException)
                {
                    nullAssignmentError = ioException;

                }
                finally
                {
                    request.StopService();
                }

                Assert.IsNotNull(nullAssignmentError, "Client library should not allow assignment of null values to non-nullable properties");
                Assert.AreEqual(ODataLibResourceUtil.GetString("ReaderValidationUtils_NullNamedValueForNonNullableType", "UnitPrice", "Edm.Double"), nullAssignmentError.Message);
            }
        }

        #endregion AtomMaterializer tests.

        #region AtomMaterializerLog tests.

        [TestMethod]
        public void AtomMaterializerLogAddedLink()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string players = FeedStart +
                AnyEntry("p1", "<d:ID>10</d:ID>", null) +
                AnyEntry("p2", "<d:ID>20</d:ID>", null) +
                "</feed>";
            string xml = FeedStart +
                AnyEntry("t1", "<d:TeamID>1</d:TeamID>", LinkFeed("Players", players)) +
                "</feed>";

            foreach (MergeOption o in new MergeOption[] { MergeOption.AppendOnly, MergeOption.NoTracking, MergeOption.PreserveChanges })
            {
                this.context = new DataServiceContext(serviceRoot);
                this.context.EnableAtom = true;
                this.context.MergeOption = o;

                var q = from t in context.CreateQuery<Team>("T").Expand("Players")
                        select t;

                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(1, item.TeamID, "item.TeamID");
                    Assert.IsNotNull(item.Players, "item.Players");

                    Assert.AreEqual(2, item.Players.Count, "item.Players.Count");
                    Assert.AreEqual(10, item.Players[0].ID, "item.Players[0].ID");
                    Assert.AreEqual(20, item.Players[1].ID, "item.Players[1].ID");
                }

                if (o == MergeOption.NoTracking)
                {
                    AssertEntityCount(0, "nothing tracked for NoTracking");
                    AssertLinkCount(0, "nothing tracked for NoTracking");
                }
                else
                {
                    AssertEntityCount(3, "one team, two players");
                    AssertLinkCount(2, "team to each player");
                }
            }
        }

        [TestMethod]
        public void AtomMaterializerLogTryResolve()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string players = FeedStart +
                AnyEntry("p1", "<d:ID>10</d:ID>", null) +
                AnyEntry("p2", "<d:ID>20</d:ID>", null) +
                "</feed>";
            string playersTweaked = FeedStart +
                AnyEntry("p1", "<d:ID>100</d:ID>", null) +
                AnyEntry("p2", "<d:ID>200</d:ID>", null) +
                "</feed>";
            string xml = FeedStart +
                AnyEntry("t1", "<d:TeamID>1</d:TeamID>", LinkFeed("Players", players)) +
                AnyEntry("t2", "<d:TeamID>2</d:TeamID>", LinkFeed("Players", playersTweaked)) +
                "</feed>";

            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("MergeOption", Enum.GetValues(typeof(MergeOption))),
                new Dimension("ChangeBetweenReads", new bool[] { true, false }));
            using (TestUtil.RestoreStaticMembersOnDispose(typeof(Player)))
                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    Player.EqualsUsesReferences = true;

                    MergeOption option = (MergeOption)values["MergeOption"];
                    bool changeBetweenReads = (bool)values["ChangeBetweenReads"];
                    this.context = new DataServiceContext(serviceRoot);
                    this.context.EnableAtom = true;
                    this.context.MergeOption = option;

                    var q = from t in context.CreateQuery<Team>("T").Expand("Players")
                            select t;

                    bool firstTeam = true;
                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    {
                        if (firstTeam)
                        {
                            Assert.AreEqual(1, item.TeamID, "item.TeamID");
                            Assert.IsNotNull(item.Players, "item.Players");

                            Assert.AreEqual(2, item.Players.Count, "item.Players.Count");
                            Assert.AreEqual(10, item.Players[0].ID, "item.Players[0].ID");
                            Assert.AreEqual(20, item.Players[1].ID, "item.Players[1].ID");

                            if (changeBetweenReads && option != MergeOption.NoTracking)
                            {
                                context.UpdateObject(item.Players[0]);
                                context.UpdateObject(item.Players[1]);
                            }
                        }
                        else
                        {
                            Assert.AreEqual(2, item.TeamID, "item.TeamID");
                            Assert.IsNotNull(item.Players, "item.Players");

                            Assert.AreEqual(2, item.Players.Count, "item.Players.Count");

                            // The players will have changed if:
                            // - In NoTracking: always
                            // - In PreserveClient: only if unchanged
                            // - In AppendOnly: only if unchanged
                            // - In OverwriteChanged: always
                            if (option == MergeOption.NoTracking || option == MergeOption.OverwriteChanges || !changeBetweenReads)
                            {
                                Assert.AreEqual(100, item.Players[0].ID, "item.Players[0].ID");
                                Assert.AreEqual(200, item.Players[1].ID, "item.Players[1].ID");
                            }
                            else
                            {
                                Assert.AreEqual(10, item.Players[0].ID, "item.Players[0].ID");
                                Assert.AreEqual(20, item.Players[1].ID, "item.Players[1].ID");
                            }
                        }

                        firstTeam = false;
                    }

                    if (option == MergeOption.NoTracking)
                    {
                        AssertEntityCount(0, "nothing tracked for NoTracking");
                        AssertLinkCount(0, "nothing tracked for NoTracking");
                    }
                    else
                    {
                        AssertEntityCount(4, "two teams, two players");
                        AssertLinkCount(4, "each team to each player");
                    }
                });
        }

        /// OperationDescriptor not refreshed with OverwriteChanges
        [TestMethod]
        public void OperationDescriptorsShouldBeUpdatedForEachRequest()
        {
            #region Response payload
            var response = @"HTTP/1.1 200 OK
Connection: Keep-Alive
Content-Length: 2011
Date: Thu, 06 Jun 2013 22:07:07 GMT
Content-Type: application/atom+xml;type=entry;charset=utf-8
Server: Microsoft-IIS/7.5
Cache-Control: no-cache
X-Content-Type-Options: nosniff
OData-Version: 4.0;
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET

<?xml version='1.0' encoding='utf-8'?>
<entry xml:base='http://services.odata.org/V3/(S(imdxx1ujw4ludze1t3k2wmgs))/OData/OData.svc/' xmlns='http://www.w3.org/2005/Atom' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns:georss='http://www.georss.org/georss' xmlns:gml='http://www.opengis.net/gml'>
    <id>http://services.odata.org/V3/(S(imdxx1ujw4ludze1t3k2wmgs))/OData/OData.svc/Products(0)</id>
    <category term='ODataDemo.Product' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <link rel='edit' title='Product' href='Products(0)' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/Category' type='application/atom+xml;type=entry' title='Category' href='Products(0)/Category' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/Supplier' type='application/atom+xml;type=entry' title='Supplier' href='Products(0)/Supplier' />
    <title type='text'>Bread</title>
    <summary type='text'>Whole grain bread</summary>
    <updated>2013-06-06T22:07:08Z</updated>
    <author>
        <name />
    </author>
    <link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/OrderDetails' type='application/xml' title='OrderDetails' href='Products(0)/OrderDetails/$ref' />
    <m:action metadata='http://services.odata.org/V3/(S(imdxx1ujw4ludze1t3k2wmgs))/OData/OData.svc/$metadata#DemoService.{0}' title='{0}' target='http://services.odata.org/V3/(S(imdxx1ujw4ludze1t3k2wmgs))/OData/OData.svc/Products(0)/{0}' />
    <content type='application/xml'>
        <m:properties>
            <d:ID m:type='Edm.Int32'>0</d:ID>
            <d:ProductName m:type='Edm.String'>Demo Product</d:ProductName>
            <d:Discontinued m:type='Edm.Boolean'>false</d:Discontinued>
        </m:properties>
    </content>
</entry>
";

            #endregion

            // Consider two actions that an entity Product supports, and they are mutually exclusive, i.e. only one action
            // is returned in the response
            string action1 = "Discount";
            string action2 = "Discount1234";

            // Create two responses with each of above actions
            var response1 = string.Format(response, action1);
            var response2 = string.Format(response, action2);

            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();
            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();
                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                ctx.EnableAtom = true;

                try
                {
                    for (int i = 0; i < 2; i++)
                    {
                        playbackService.OverridingPlayback = (i%2 == 0) ? response1 : response2;

                        Product product = ctx.Execute<Product>(new Uri("Products(0)", UriKind.Relative)).First();

                        EntityDescriptor descriptor = ctx.GetEntityDescriptor(product);

                        descriptor.OperationDescriptors.Count.Should().Be(1);

                        var actionName = descriptor.OperationDescriptors.First().Title;

                        if (i%2 == 0)
                        {
                            actionName.Should().Be(action1);
                        }
                        else
                        {
                            actionName.Should().Be(action2);
                        }
                    }
                }
                finally
                {
                    request.StopService();
                }
            }
        }

        #endregion AtomMaterializerLog tests.

        private void ClearContext()
        {
            DataServiceContextTests.ClearContext(this.context);
        }

        #region Assert helpers.

        internal void AssertEntityCount(int expectedCount, string description)
        {
            ProjectionTests.AssertEntityCountForContext(expectedCount, description, this.context);
        }

        internal void AssertLinkCount(int expectedCount, string description)
        {
            ProjectionTests.AssertLinkCountForContext(expectedCount, description, this.context);
        }

        #endregion Assert helpers.

        #region Payload builder helpers.

        internal static string AnyEntry(string id, string properties, string links)
        {
            return AtomParserTests.AnyEntry(id, properties, links);
        }

        internal static string LinkEntry(string name, string content)
        {
            return ProjectionTests.Link(true, name, content);
        }

        internal static string LinkFeed(string name, string content)
        {
            return ProjectionTests.Link(false, name, content);
        }

        #endregion Payload builder helpers.

        #region Materialization API helpers.

        internal QueryOperationResponse<T> CreateTestQueryOperationResponse<T>(string text, IQueryable<T> query)
        {
            return (QueryOperationResponse<T>)CreateTestMaterializeAtom(text, query);
        }

        internal IEnumerable<T> CreateTestMaterializeAtom<T>(
            string text,
            IQueryable<T> query)
        {
            return AtomParserTests.CreateQueryResponse(this.context, EmptyHeaders, query, text);
        }

        #endregion Materialization API helpers.
    }
}
