//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif

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

    /// <summary>Abstract class from which <see cref="T:System.Data.Services.Client.EntityDescriptor" /> is derived.</summary>
#if WINDOWS_PHONE
    [DataContract(IsReference = true)]
#endif
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
        /// <returns>An <see cref="T:System.Data.Services.Client.EntityStates" /> of the object returned at the time this instance was constructed.  </returns>
#if WINDOWS_PHONE
    [DataMember]
#endif
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
#if WINDOWS_PHONE
    [DataMember]
#endif
        internal uint ChangeOrder
        {
            get { return this.changeOrder; }
            set { this.changeOrder = value; }
        }

        /// <summary>was content generated for the entity</summary>
#if WINDOWS_PHONE
    [DataMember]
#endif
        internal bool ContentGeneratedForSave
        {
            get { return this.saveContentGenerated; }
            set { this.saveContentGenerated = value; }
        }

        /// <summary>was this entity save result processed</summary>
#if WINDOWS_PHONE
    [DataMember]
#endif
        internal EntityStates SaveResultWasProcessed
        {
            get { return this.saveResultProcessed; }
            set { this.saveResultProcessed = value; }
        }

        /// <summary>last save exception per entry</summary>
#if WINDOWS_PHONE
    [DataMember]
#endif
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
