//---------------------------------------------------------------------
// <copyright file="Descriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// enum to describe the descriptor kind
    /// </summary>
    internal enum DescriptorKind
    {
        /// <summary>Entity Descriptor</summary>
        Entity = 0,

        /// <summary>Link Descriptor</summary>
        Link,

        /// <summary>Named stream descriptor</summary>
        NamedStream,

        /// <summary>Service Operation descriptor</summary>
        OperationDescriptor,
    }

    /// <summary>Abstract class from which <see cref="Microsoft.OData.Client.EntityDescriptor" /> is derived.</summary>
    public abstract class Descriptor
    {
        #region Fields

        /// <summary>change order</summary>
        private uint changeOrder = UInt32.MaxValue;

        /// <summary>was content generated for the entity</summary>
        private bool saveContentGenerated;

        /// <summary>was this entity save result processed</summary>
        /// <remarks>0 - no processed, otherwise reflects the previous state</remarks>
        private EntityStates saveResultProcessed;

        /// <summary>last save exception per entry</summary>
        private Exception saveError;

        /// <summary>State of the modified entity or link.</summary>
        private EntityStates state;

        /// <summary>DependsOnIds for the modified entity.</summary>
        private List<string> dependsOnIds;

        /// <summary>DependsOnChangeSetIds for the modified entity.</summary>
        private List<string> dependsOnChangeSetIds;

        /// <summary>ChangeSetId for the modified entity.</summary>
        private string changeSetId;

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="state">entity state</param>
        internal Descriptor(EntityStates state)
        {
            this.state = state;
        }

        #region Public Properties

        /// <summary>When overridden in a derived class, gets the state of the object at the time this instance was constructed.</summary>
        /// <returns>An <see cref="Microsoft.OData.Client.EntityStates" /> of the object returned at the time this instance was constructed.  </returns>
        public EntityStates State
        {
            get { return this.state; }
            internal set { this.state = value; }
        }

        #endregion

        #region Internal Properties

        /// <summary>true if resource, false if link</summary>
        internal abstract DescriptorKind DescriptorKind
        {
            get;
        }

        /// <summary>changeOrder</summary>
        internal uint ChangeOrder
        {
            get { return this.changeOrder; }
            set { this.changeOrder = value; }
        }

        /// <summary>was content generated for the entity</summary>
        internal bool ContentGeneratedForSave
        {
            get { return this.saveContentGenerated; }
            set { this.saveContentGenerated = value; }
        }

        /// <summary>was this entity save result processed</summary>
        internal EntityStates SaveResultWasProcessed
        {
            get { return this.saveResultProcessed; }
            set { this.saveResultProcessed = value; }
        }

        /// <summary>last save exception per entry</summary>
        internal Exception SaveError
        {
            get { return this.saveError; }
            set { this.saveError = value; }
        }

        /// <summary>
        /// Returns true if the entry has been modified (and thus should participate in SaveChanges).
        /// </summary>
        internal virtual bool IsModified
        {
            get
            {
                System.Diagnostics.Debug.Assert(
                    (EntityStates.Added == this.state) ||
                    (EntityStates.Modified == this.state) ||
                    (EntityStates.Unchanged == this.state) ||
                    (EntityStates.Deleted == this.state),
                    "entity state is not valid");

                return (EntityStates.Unchanged != this.state);
            }
        }

        /// <summary>DependsOnIds for the modified entity.</summary>
        internal List<string> DependsOnIds
        {
            get { return this.dependsOnIds; }
            set { this.dependsOnIds = value; }
        }

        /// <summary>DependsOnChangeSetIds for the modified entity.</summary>
        internal List<string> DependsOnChangeSetIds
        {
            get { return this.dependsOnChangeSetIds; }
            set { this.dependsOnChangeSetIds = value; }
        }

        /// <summary>ChangeSetId for the modified entity.</summary>
        internal string ChangeSetId
        {
            get { return this.changeSetId; }
            set { this.changeSetId = value; }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Clear all the changes associated with this descriptor
        /// This method is called when the client is done with sending all the pending requests.
        /// </summary>
        internal abstract void ClearChanges();
        #endregion
    }
}
