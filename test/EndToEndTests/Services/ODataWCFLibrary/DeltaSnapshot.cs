//---------------------------------------------------------------------
// <copyright file="DeltaSnapshot.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;

    public class DeltaSnapshot
    {
        public DeltaSnapshot(Uri query)
        {
            this.QueryUri = query;
            this.TimeStamp = DateTime.Now;
            this.Entries = new List<DeltaSnapshotEntry>();
        }

        public Uri QueryUri { get; set; }

        public List<DeltaSnapshotEntry> Entries { get; set; }

        public DateTime TimeStamp { get; set; }

        public DeltaSnapshot Clone()
        {
            DeltaSnapshot snapshot = new DeltaSnapshot(this.QueryUri);
            snapshot.TimeStamp = this.TimeStamp;
            foreach (var entry in this.Entries)
            {
                snapshot.Entries.Add(entry.Clone());
            }
            return snapshot;
        }
    }

    public class DeltaSnapshotEntry
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string RelationShip { get; set; }

        public DeltaSnapshotEntry(string id, string parentId, string relationShip)
        {
            this.Id = id;
            this.ParentId = parentId;
            this.RelationShip = relationShip;
        }

        public DeltaSnapshotEntry Clone()
        {
            DeltaSnapshotEntry entry = new DeltaSnapshotEntry(this.Id, this.ParentId, this.RelationShip);
            return entry;
        }
    }
}