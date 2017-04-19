//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchAtomicGroupCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
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
    /// Cache for atomic groups along with the stably-ordered request Ids in each group.
    /// It also keeps track of atomic group start and atomic group end status during the reading
    /// of batch request.
    /// </summary>
    internal class ODataJsonLightBatchAtomicGroupCache
    {
        // Lookup table for atomicitGroup.
        private Dictionary<string, IList<string>> groupToRequestIds = new Dictionary<string, IList<string>>();

        // Group Id of the preceeding request. Could be null.
        private string preceedingRequestGroupId = null;

        // Latest status of whether the processing is within scope of an atomic group.
        // The scope is ended by a top-level request, the starting of another atomic group, or
        // end of the batch requests array.
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
            // Preceeding group Id cannot null when we are within an atomic group.
            Debug.Assert(!(isWithinAtomicGroup && preceedingRequestGroupId == null),
                "!(isWithinAtomicGroup && preceedingGroupId == null)");

            if (!isWithinAtomicGroup
                || ((preceedingRequestGroupId != null) && preceedingRequestGroupId.Equals(groupId))
                /*groupId is member of existing atomic group scope*/)
            {
                return false;
            }

            // This groupId ends the preceeding atomic group.
            this.isWithinAtomicGroup = false;
            this.preceedingRequestGroupId = null;
            return true;
        }

        /// <summary>
        /// Construct the list of request Ids for the group, and determine whether the
        /// request Id is the start of an atomic group.
        /// </summary>
        /// <param name="requestId">Request id to add.</param>
        /// <param name="groupId">Id of the group to add the requstId. Cannot be null.</param>
        /// <returns>
        /// True if changeset start is detected; false otherwise.
        /// Ensure all request Ids of the same groups are adjacent, otherwise throw an error.
        /// </returns>
        internal bool AddRequestToGroup(string requestId, string groupId)
        {
            Debug.Assert(groupId != null, "groupId != null");

            bool isChangesetStart = false;
            if (groupId.Equals(this.preceedingRequestGroupId, StringComparison.Ordinal))
            {
                // Adjacent groupIds, add requestId to the existing group
                IList<string> requestIds;
                groupToRequestIds.TryGetValue(groupId, out requestIds);

                Debug.Assert(requestIds != null, "requestIds != null");
                requestIds.Add(requestId);
            }
            else if (!groupToRequestIds.ContainsKey(groupId))
            {
                // We get a new groupId.
                groupToRequestIds.Add(groupId, new List<string> { requestId });

                this.preceedingRequestGroupId = groupId;

                // Mark the atomic group status.
                this.isWithinAtomicGroup = true;

                isChangesetStart = true;
            }
            else
            {
                throw new ODataException(String.Format(
                    CultureInfo.InvariantCulture,
                    " Request with id {0} is positioned incorrectly: all requests of same groupId {1} must be adjacent.",
                    requestId,
                    groupId));
            }

            return isChangesetStart;
        }

        /// <summary>
        /// Get the atomic group Id of the request; null if the request does not belong to any groups.
        /// </summary>
        /// <param name="targetRequestId">Id of the request from the request property.</param>
        /// <returns>The group Id if found; null otherwise.</returns>
        internal string GetGroupId(string targetRequestId)
        {
            foreach (string groupId in groupToRequestIds.Keys)
            {
                IList<string> requestIds;
                groupToRequestIds.TryGetValue(groupId, out requestIds);
                Debug.Assert(requestIds != null, "requestIds != null");

                if (requestIds.Contains(targetRequestId))
                {
                    return groupId;
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
            return this.groupToRequestIds.Keys.Contains(id);
        }

        /// <summary>
        /// Flatten a given list of groupIds and requestIds into a string containing comma-separated request Ids.
        /// </summary>
        /// <param name="ids">List of ids to be flattened.</param>
        /// <returns>The flatten string containing comma-separated request Ids.</returns>
        internal string GetFlattenedRequestIds(IList<string> ids)
        {
            if (ids.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (string id in ids)
            {
                if (!IsGroupId(id))
                {
                    sb.Append(id).Append(',');
                }
                else
                {
                    IList<string> reqIds;
                    if (this.groupToRequestIds.TryGetValue(id, out reqIds))
                    {
                        foreach (string reqId in reqIds)
                        {
                            sb.Append(reqId).Append(',');
                        }
                    }
                }
            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }

    }
}
