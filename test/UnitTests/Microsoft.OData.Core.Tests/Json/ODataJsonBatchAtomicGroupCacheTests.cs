//---------------------------------------------------------------------
// <copyright file="ODataJsonBatchAtomicGroupCacheTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class ODataJsonBatchAtomicGroupCacheTests
    {
        [Fact]
        public void GetGroupIdReturnsNullForEmptyCache()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();

            Assert.Null(cache.GetGroupId("missing"));
            Assert.Null(cache.GetGroupId(null));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 4)]
        [InlineData(3, 4)]
        [InlineData(100, 100)]
        public void GetGroupIdResolvesMembersAcrossGroups(int groupCount, int memberCount)
        {
            var cache = new ODataJsonBatchAtomicGroupCache();

            for (int group = 0; group < groupCount; group++)
            {
                string groupId = $"g{group.ToString(CultureInfo.InvariantCulture)}";
                cache.IsChangesetEnd(groupId);
                for (int member = 0; member < memberCount; member++)
                {
                    string messageId = $"{groupId}r{member.ToString(CultureInfo.InvariantCulture)}";
                    Assert.Equal(member == 0, cache.AddMessageIdAndGroupId(messageId, groupId));
                }
            }

            for (int group = 0; group < groupCount; group++)
            {
                string groupId = $"g{group.ToString(CultureInfo.InvariantCulture)}";
                for (int member = 0; member < memberCount; member++)
                {
                    Assert.Equal(groupId, cache.GetGroupId($"{groupId}r{member.ToString(CultureInfo.InvariantCulture)}"));
                }
            }

            Assert.Null(cache.GetGroupId("missing"));
        }

        [Fact]
        public void GetGroupIdUsesOrdinalMemberComparison()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("member", "group");
            cache.IsChangesetEnd("GROUP");
            cache.AddMessageIdAndGroupId("MEMBER", "GROUP");

            Assert.Equal("group", cache.GetGroupId("member"));
            Assert.Equal("GROUP", cache.GetGroupId("MEMBER"));
            Assert.Null(cache.GetGroupId("Member"));
            Assert.Null(cache.GetGroupId("group"));
            Assert.Null(cache.GetGroupId("top-level"));
            Assert.Null(cache.GetGroupId(null));
        }

        [Fact]
        public void GetGroupIdKeepsFirstGroupForRepeatedMember()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("repeated", "g1");
            cache.AddMessageIdAndGroupId("repeated", "g1");
            cache.IsChangesetEnd("g2");
            cache.AddMessageIdAndGroupId("other", "g2");
            cache.AddMessageIdAndGroupId("repeated", "g2");

            Assert.Equal("g1", cache.GetGroupId("repeated"));
            Assert.Equal("g2", cache.GetGroupId("other"));
            Assert.Equal(new[] { "repeated", "repeated", "other", "repeated" }, cache.GetFlattenedMessageIds(new[] { "g1", "g2" }));
        }

        [Fact]
        public void GetGroupIdRetainsEarlierMembersAfterGroupEnds()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("r1", "g1");

            Assert.True(cache.IsChangesetEnd(null));
            Assert.False(cache.IsWithinAtomicGroup);
            Assert.Equal("g1", cache.GetGroupId("r1"));
            Assert.True(cache.AddMessageIdAndGroupId("r2", "g2"));
            Assert.Equal("g1", cache.GetGroupId("r1"));
            Assert.Equal("g2", cache.GetGroupId("r2"));
        }

        [Fact]
        public void GetGroupIdDoesNotRecordRejectedInsertion()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("r1", "g1");
            cache.IsChangesetEnd("g2");
            cache.AddMessageIdAndGroupId("r2", "g2");
            cache.IsChangesetEnd("g1");

            var exception = Assert.Throws<ODataException>(() => cache.AddMessageIdAndGroupId("rejected", "g1"));

            Assert.Equal(Error.Format(SRResources.ODataBatchReader_MessageIdPositionedIncorrectly, "rejected", "g1"), exception.Message);
            Assert.Null(cache.GetGroupId("rejected"));
            Assert.Equal("g1", cache.GetGroupId("r1"));
            Assert.Equal("g2", cache.GetGroupId("r2"));
            Assert.Equal(new[] { "r1" }, cache.GetFlattenedMessageIds(new[] { "g1" }));
        }

#if !DEBUG
        [Fact]
        public void GetGroupIdRetainsFirstGroupForNullMember()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId(null, "g1");
            cache.AddMessageIdAndGroupId(null, "g1");
            cache.IsChangesetEnd("g2");
            cache.AddMessageIdAndGroupId(null, "g2");
            cache.AddMessageIdAndGroupId("member", "g2");

            Assert.Equal("g1", cache.GetGroupId(null));
            Assert.Equal("g2", cache.GetGroupId("member"));
            Assert.Equal(new string[] { null, null, null, "member" }, cache.GetFlattenedMessageIds(new[] { "g1", "g2" }));
        }
#endif

        [Theory]
        [InlineData(new string[0], new string[0])]
        [InlineData(new[] { "direct" }, new[] { "direct" })]
        [InlineData(new[] { "g1" }, new[] { "r1", "r2" })]
        [InlineData(new[] { "g2", "g1", "direct" }, new[] { "r3", "r1", "r2", "direct" })]
        [InlineData(new[] { "g1", "direct", "g1", "direct" }, new[] { "r1", "r2", "direct", "r1", "r2", "direct" })]
        [InlineData(new[] { "r2", "direct", "g1", "direct", "r1" }, new[] { "r2", "direct", "r1", "r2", "direct", "r1" })]
        public void GetFlattenedMessageIdsPreservesOrderAndMultiplicity(string[] ids, string[] expected)
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("r1", "g1");
            cache.AddMessageIdAndGroupId("r2", "g1");
            cache.IsChangesetEnd("g2");
            cache.AddMessageIdAndGroupId("r3", "g2");

            Assert.Equal(expected, cache.GetFlattenedMessageIds(ids));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 1000)]
        [InlineData(1000, 1)]
        [InlineData(1000, 1000)]
        public void GetFlattenedMessageIdsPreservesRepeatedGroupMembers(int memberCount, int repeatCount)
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            var expectedMembers = new List<string>();
            for (int member = 0; member < memberCount; member++)
            {
                string messageId = $"r{member.ToString(CultureInfo.InvariantCulture)}";
                cache.AddMessageIdAndGroupId(messageId, "group");
                expectedMembers.Add(messageId);
            }

            var ids = new List<string>();
            for (int repeat = 0; repeat < repeatCount; repeat++)
            {
                ids.Add("group");
            }

            var actual = cache.GetFlattenedMessageIds(ids);

            Assert.Equal(memberCount * repeatCount, actual.Count);
            Assert.Equal(Enumerable.Repeat(expectedMembers, repeatCount).SelectMany(members => members), actual);
        }

        [Fact]
        public void GetFlattenedMessageIdsReflectsClearedAndReaddedInput()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("member", "group");
            var ids = new List<string> { "group", "direct" };
            var first = cache.GetFlattenedMessageIds(ids);

            ids.Clear();
            Assert.Empty(cache.GetFlattenedMessageIds(ids));
            ids.Add("direct");
            ids.Add("group");
            ids.Add("group");

            Assert.Equal(new[] { "direct", "member", "member" }, cache.GetFlattenedMessageIds(ids));
            Assert.Equal(new[] { "member", "direct" }, first);
            Assert.Equal("group", cache.GetGroupId("member"));
            Assert.Null(new ODataJsonBatchAtomicGroupCache().GetGroupId("member"));
        }

        [Theory]
        [InlineData("first", "g1")]
        [InlineData("last", "g2")]
        [InlineData("missing", null)]
        public void GetGroupIdDoesNotSearchGroupMembers(string messageId, string expectedGroupId)
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("first", "g1");
            cache.IsChangesetEnd("g2");
            cache.AddMessageIdAndGroupId("last", "g2");
            var field = typeof(ODataJsonBatchAtomicGroupCache).GetField("groupToMessageIds", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            var groups = Assert.IsType<Dictionary<string, IList<string>>>(field.GetValue(cache));
            // Keep the original members while detecting any fallback to list searches.
            var firstMembers = new MembershipCountingCollection(groups["g1"]);
            var lastMembers = new MembershipCountingCollection(groups["g2"]);
            groups["g1"] = firstMembers;
            groups["g2"] = lastMembers;

            Assert.Equal(expectedGroupId, cache.GetGroupId(messageId));
            Assert.Equal(0, firstMembers.ContainsCalls + lastMembers.ContainsCalls);
        }

        [Fact]
        public void GetGroupIdDoesNotAllocatePerLookup()
        {
            var cache = new ODataJsonBatchAtomicGroupCache();
            cache.AddMessageIdAndGroupId("member", "group");
            cache.GetGroupId("member");
            cache.GetGroupId("missing");
            cache.GetGroupId(null);
            string groupId = null;
            string missingGroupId = null;
            long before = GC.GetAllocatedBytesForCurrentThread();

            for (int i = 0; i < 1000; i++)
            {
                groupId = cache.GetGroupId("member");
                missingGroupId = cache.GetGroupId("missing");
                cache.GetGroupId(null);
            }

            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.Equal("group", groupId);
            Assert.Null(missingGroupId);
            Assert.Equal(0, allocated);
        }

        private sealed class MembershipCountingCollection : Collection<string>, ICollection<string>
        {
            public MembershipCountingCollection(IList<string> members) : base(members)
            {
            }

            public int ContainsCalls { get; private set; }

            bool ICollection<string>.Contains(string item)
            {
                this.ContainsCalls++;
                return this.Contains(item);
            }
        }
    }
}
