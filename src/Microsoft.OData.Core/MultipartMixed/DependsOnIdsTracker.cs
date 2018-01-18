//---------------------------------------------------------------------
// <copyright file="DependsOnIdsTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.MultipartMixed
{
    /// <summary>
    /// This class is to keep track of dependsOn ids and associated change set state,
    /// and to return dependsOn ids pertaining to current state of the batch processing.
    /// </summary>
    internal sealed class DependsOnIdsTracker
    {
        /// <summary>
        /// List of top-level dependsOn ids seen so far.
        /// </summary>
        private readonly IList<string> topLevelDependsOnIds;

        /// <summary>
        /// List of depeondsOn ids seen so far in current change set.
        /// It should be empty if current processing is not within a change set.
        /// </summary>
        private readonly IList<string> changeSetDependsOnIds;

        /// <summary>
        /// Indicates whether current processing is inside a change set.
        /// </summary>
        private bool isInChangeSet;

        /// <summary>
        /// Constructor.
        /// Initial state is batch start, and within change set.
        /// </summary>
        internal DependsOnIdsTracker()
        {
            this.topLevelDependsOnIds = new List<string>();
            this.changeSetDependsOnIds = new List<string>();
            this.isInChangeSet = false;
        }

        /// <summary>
        /// Sets up the internal states corresponding to starting of change set.
        /// </summary>
        internal void ChangeSetStarted()
        {
            Debug.Assert(!isInChangeSet, "!isInChangeSet");
            Debug.Assert(this.changeSetDependsOnIds.Count == 0);
            this.isInChangeSet = true;
        }

        /// <summary>
        /// Sets up the internal states corresponding to ending of change set.
        /// </summary>
        internal void ChangeSetEnded()
        {
            Debug.Assert(isInChangeSet, "isInChangeSet");
            this.isInChangeSet = false;
            this.changeSetDependsOnIds.Clear();
        }

        /// <summary>
        /// Adds the id into the tracker.
        /// </summary>
        /// <param name="id">The operation request id to add.</param>
        internal void AddDependsOnId(string id)
        {
            if (isInChangeSet)
            {
                this.changeSetDependsOnIds.Add(id);
            }
            else
            {
                this.topLevelDependsOnIds.Add(id);
            }
        }

        /// <summary>
        /// Get the list of dependsOn ids for the current state.
        /// </summary>
        /// <returns>The read-only list of dependsOn ids for the current batch operation request.</returns>
        internal IEnumerable<string> GetDependsOnIds()
        {
            return isInChangeSet ? this.changeSetDependsOnIds : this.topLevelDependsOnIds;
        }
    }
}
