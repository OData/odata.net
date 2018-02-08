//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchAtomicGroupCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    #endregion Namespaces
    /// <summary>
    /// Cache for atomic groups along with the stably-ordered message Ids in each group.
    /// It also keeps track of atomic group start and atomic group end status during the reading
    /// of batch message.
    /// </summary>
    internal sealed class ODataJsonLightBatchAtomicGroupCache
    {
        /// <summary>
        /// Lookup table for atomicitGroup.
        /// </summary>
        private readonly Dictionary<string, IList<string>> groupToMessageIds = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Group Id of the preceding message. Could be null.
        /// </summary>
        private string precedingMessageGroupId = null;

        /// <summary>
        /// Latest status of whether the processing is within scope of an atomic group.
        /// The scope is ended by a top-level message, the starting of another atomic group, or
        /// end of the batch messages array.
        /// </summary>
        private bool isWithinAtomicGroup = false;

        /// <summary>
        /// Whether the processing is within scope of an atomic group.
        /// </summary>
        internal bool IsWithinAtomicGroup
        {
            get { return this.isWithinAtomicGroup; }
            set { this.isWithinAtomicGroup = value; }
        }

        /// <summary>
        /// Given the group Id from reader's current state, determine whether atomic group end is detected.
        /// </summary>
        /// <param name="groupId">The group Id from the reader's current state.</param>
        /// <returns>True if atomic group end is detected; false otherwise.</returns>
        internal bool IsChangesetEnd(string groupId)
        {
            // Preceding group Id cannot null when we are within an atomic group.
            Debug.Assert(!(isWithinAtomicGroup && precedingMessageGroupId == null),
                "!(isWithinAtomicGroup && precedingMessageGroupId == null)");

            if (!isWithinAtomicGroup
                || ((precedingMessageGroupId != null) && precedingMessageGroupId.Equals(groupId))
                /*groupId is member of existing atomic group scope*/)
            {
                return false;
            }

            // This groupId ends the preceding atomic group.
            this.isWithinAtomicGroup = false;
            this.precedingMessageGroupId = null;
            return true;
        }

        /// <summary>
        /// Construct the list of Ids for the group, and determine whether this
        /// is the start of an atomic group.
        /// </summary>
        /// <param name="messageId">Message Id to add.</param>
        /// <param name="groupId">Id of the group to add the message Id. Cannot be null.</param>
        /// <returns>
        /// True if changeset start is detected; false otherwise.
        /// Ensure all message Ids of the same groups are adjacent, otherwise throw an error.
        /// </returns>
        internal bool AddMessageIdAndGroupId(string messageId, string groupId)
        {
            Debug.Assert(messageId != null, "messageId  != null");
            Debug.Assert(groupId != null, "groupId != null");

            bool isChangesetStart = false;
            if (groupId.Equals(this.precedingMessageGroupId, StringComparison.Ordinal))
            {
                // Adjacent groupIds, add messageId to the existing group
                Debug.Assert(groupToMessageIds[groupId] != null, "groupToMessageIds[groupId] != null");
                groupToMessageIds[groupId].Add(messageId);
            }
            else if (!groupToMessageIds.ContainsKey(groupId))
            {
                // We get a new groupId.
                groupToMessageIds.Add(groupId, new List<string> { messageId });

                this.precedingMessageGroupId = groupId;

                // Mark the atomic group status.
                this.isWithinAtomicGroup = true;

                isChangesetStart = true;
            }
            else
            {
                throw new ODataException(Strings.ODataBatchReader_MessageIdPositionedIncorrectly(messageId, groupId));
            }

            return isChangesetStart;
        }

        /// <summary>
        /// Get the atomic group Id of the message; null if the message does not belong to any groups.
        /// </summary>
        /// <param name="targetMessageId">Id of the message from the json property.</param>
        /// <returns>The group Id if found; null otherwise.</returns>
        internal string GetGroupId(string targetMessageId)
        {
            foreach (KeyValuePair<string, IList<string>> pair in this.groupToMessageIds)
            {
                IList<string> messageIds = pair.Value;
                if (messageIds != null && messageIds.Contains(targetMessageId))
                {
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Testing whether the given Id is a group Id.
        /// </summary>
        /// <param name="id">The id under test.</param>
        /// <returns>True if it is group Id of the batch; false otherwise.</returns>
        internal bool IsGroupId(string id)
        {
            return this.groupToMessageIds.Keys.Contains(id);
        }

        /// <summary>
        /// Flatten a given list of groupIds and messageIds into a string containing comma-separated message Ids.
        /// </summary>
        /// <param name="ids">List of ids to be flattened.</param>
        /// <returns>The list containing comma-separated message Ids.</returns>
        internal IList<string> GetFlattenedMessageIds(IList<string> ids)
        {
            List<string> result = new List<string>();
            if (ids.Count == 0)
            {
                return result;
            }

            foreach (string id in ids)
            {
                IList<string> reqIds;
                if (this.groupToMessageIds.TryGetValue(id, out reqIds))
                {
                    result.AddRange(reqIds);
                }
                else
                {
                    result.Add(id);
                }
            }

            return result;
        }
    }
}