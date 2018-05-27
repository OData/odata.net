//---------------------------------------------------------------------
// <copyright file="LinkDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Client.Metadata;

    /// <summary>
    /// represents the association between two entities
    /// </summary>
    [DebuggerDisplay("State = {state}")]
    public sealed class LinkDescriptor : Descriptor
    {
        #region Fields

        /// <summary>equivalence comparer</summary>
        internal static readonly System.Collections.Generic.IEqualityComparer<LinkDescriptor> EquivalenceComparer = new Equivalent();

        /// <summary>source entity</summary>
        private object source;

        /// <summary>name of property on source entity that references the target entity</summary>
        private string sourceProperty;

        /// <summary>target entity</summary>
        private object target;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Source entity</param>
        /// <param name="sourceProperty">Navigation property on the source entity</param>
        /// <param name="target">Target entity</param>
        /// <param name="model">The client model.</param>
        internal LinkDescriptor(object source, string sourceProperty, object target, ClientEdmModel model)
            : this(source, sourceProperty, target, EntityStates.Unchanged)
        {
            this.IsSourcePropertyCollection = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(source.GetType())).GetProperty(sourceProperty, UndeclaredPropertyBehavior.ThrowException).IsEntityCollection;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Source entity</param>
        /// <param name="sourceProperty">Navigation property on the source entity</param>
        /// <param name="target">Target entity</param>
        /// <param name="state">The link state</param>
        internal LinkDescriptor(object source, string sourceProperty, object target, EntityStates state)
            : base(state)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(!String.IsNullOrEmpty(sourceProperty), "!String.IsNullOrEmpty(propertyName)");
            Debug.Assert(!sourceProperty.Contains("/"), "!sourceProperty.Contains('/')");

            this.source = source;
            this.sourceProperty = sourceProperty;
            this.target = target;
        }

        #region Public Properties

        /// <summary>The source entity in a link returned by a <see cref="T:Microsoft.OData.Client.DataServiceResponse" />. </summary>
        /// <returns><see cref="T:System.Object" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = "The setter is called during de-serialization")]
        public object Target
        {
            get
            {
                return this.target;
            }

            internal set
            {
                this.target = value;
            }
        }

        /// <summary>A source entity in a link returned by a <see cref="T:Microsoft.OData.Client.DataServiceResponse" />.</summary>
        /// <returns><see cref="T:System.Object" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = "The setter is called during de-serialization")]
        public object Source
        {
            get
            {
                return this.source;
            }

            internal set
            {
                this.source = value;
            }
        }

        /// <summary>The identifier property of the source entity in a link returned by a <see cref="T:Microsoft.OData.Client.DataServiceResponse" />.</summary>
        /// <returns>The string identifier of an identity property in a source entity. </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = "The setter is called during de-serialization")]
        public string SourceProperty
        {
            get
            {
                return this.sourceProperty;
            }

            internal set
            {
                this.sourceProperty = value;
            }
        }

        #endregion

        /// <summary>this is a link</summary>
        internal override DescriptorKind DescriptorKind
        {
            get { return DescriptorKind.Link; }
        }

        /// <summary>is this a collection property or not</summary>
        internal bool IsSourcePropertyCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Clear all the changes associated with this descriptor
        /// This method is called when the client is done with sending all the pending requests.
        /// </summary>
        internal override void ClearChanges()
        {
            // Do nothing
        }

        /// <summary>
        /// If the current instance of link descriptor is equivalent to the parameters supplied
        /// </summary>
        /// <param name="src">The source entity</param>
        /// <param name="srcPropName">The source property name</param>
        /// <param name="targ">The target entity</param>
        /// <returns>true if equivalent</returns>
        internal bool IsEquivalent(object src, string srcPropName, object targ)
        {
            return (this.source == src &&
                this.target == targ &&
                this.sourceProperty == srcPropName);
        }

        /// <summary>equivalence comparer</summary>
        private sealed class Equivalent : System.Collections.Generic.IEqualityComparer<LinkDescriptor>
        {
            /// <summary>are two LinkDescriptors equivalent, ignore state</summary>
            /// <param name="x">link descriptor x</param>
            /// <param name="y">link descriptor y</param>
            /// <returns>true if equivalent</returns>
            public bool Equals(LinkDescriptor x, LinkDescriptor y)
            {
                return (null != x) && (null != y) && x.IsEquivalent(y.source, y.sourceProperty, y.target);
            }

            /// <summary>compute hashcode for LinkDescriptor</summary>
            /// <param name="obj">link descriptor</param>
            /// <returns>hashcode</returns>
            public int GetHashCode(LinkDescriptor obj)
            {
                return (null != obj) ? (obj.Source.GetHashCode() ^ ((null != obj.Target) ? obj.Target.GetHashCode() : 0) ^ obj.SourceProperty.GetHashCode()) : 0;
            }
        }
    }
}
