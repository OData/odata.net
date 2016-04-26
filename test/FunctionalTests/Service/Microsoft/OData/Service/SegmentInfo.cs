//---------------------------------------------------------------------
// <copyright file="SegmentInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>Contains the information regarding a segment that makes up the uri</summary>
    [DebuggerDisplay("SegmentInfo={Identifier} -> {TargetKind} '{TargetResourceType.InstanceType}'")]
    internal class SegmentInfo
    {
        /// <summary>Empty constructor.</summary>
        internal SegmentInfo()
        {
        }

        /// <summary>Copy constructor.</summary>
        /// <param name="other">Another <see cref="SegmentInfo"/> to get a shallow copy of.</param>
        internal SegmentInfo(SegmentInfo other)
        {
            Debug.Assert(other != null, "other != null");
            this.Identifier = other.Identifier;
            this.Key = other.Key;
            this.Operation = other.Operation;
            this.ProjectedProperty = other.ProjectedProperty;
            this.RequestExpression = other.RequestExpression;
            this.RequestEnumerable = other.RequestEnumerable;
            this.SingleResult = other.SingleResult;
            this.TargetResourceSet = other.TargetResourceSet;
            this.TargetKind = other.TargetKind;
            this.TargetSource = other.TargetSource;
            this.TargetResourceType = other.TargetResourceType;
        }

        /// <summary>Returns the identifier for this segment i.e. string part without the keys.</summary>
        internal string Identifier { get; set; }

        /// <summary>Returns the values that constitute the key as specified in the request.</summary>
        internal KeySegment Key { get; set; }

        /// <summary>Returns the query that's being composed for this segment</summary>
        internal IEnumerable RequestEnumerable { get; set; }

        /// <summary>Whether the segment targets a single result or not.</summary>
        internal bool SingleResult { get; set; }

        /// <summary>resource set if applicable.</summary>
        internal ResourceSetWrapper TargetResourceSet { get; set; }

        /// <summary>The type of element targeted by this segment.</summary>
        internal ResourceType TargetResourceType { get; set; }

        /// <summary>The kind of resource targeted by this segment.</summary>
        internal RequestTargetKind TargetKind { get; set; }

        /// <summary>Returns the source for this segment</summary>
        internal RequestTargetSource TargetSource { get; set; }

        /// <summary>Service operation being invoked.</summary>
        internal OperationWrapper Operation { get; set; }

        /// <summary>Returns the property that is being projected in this segment, if there's any.</summary>
        internal ResourceProperty ProjectedProperty { get; set; }

        /// <summary>Returns the expression for this segment.</summary>
        internal Expression RequestExpression { get; set; }

        /// <summary>
        /// Returns true if the current segment is a type identifier segment.
        /// </summary>
        internal bool IsTypeIdentifierSegment { get; set; }

        /// <summary>Returns true if this segment has a key filter with values; false otherwise.</summary>
        internal bool HasKeyValues
        {
            get { return this.Key != null; }
        }

        /// <summary>
        /// Determines whether the target kind is a direct reference to an element
        /// i.e. either you have a $value or you are accessing a resource via key property
        /// (/Customers(1) or /Customers(1)/BestFriend/Orders('Foo'). Either case the value
        /// cannot be null.
        /// </summary>
        /// <returns>
        /// A characteristic of a direct reference is that if its value
        /// is null, a 404 error should be returned.
        /// </returns>
        internal bool IsDirectReference
        {
            get
            {
                return
                    this.TargetKind == RequestTargetKind.PrimitiveValue ||
                    this.TargetKind == RequestTargetKind.OpenPropertyValue ||
                    this.HasKeyValues;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this segemnt represents a service action.
        /// </summary>
        /// <value> true if this segment represents a service action; false otherwise. </value>
        internal bool IsServiceActionSegment
        {
            get
            {
                return this.Operation != null && this.Operation.Kind == OperationKind.Action;
            }
        }

        /// <summary>
        /// Gets the binding type of the segment to use for action/function resolution.
        /// </summary>
        internal ResourceType BindingType
        {
            get
            {
                if (this.SingleResult)
                {
                    return this.TargetResourceType;
                }

                if (this.TargetResourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
                {
                    return ResourceType.GetEntityCollectionResourceType(this.TargetResourceType);
                }

                return ResourceType.GetCollectionResourceType(this.TargetResourceType);
            }
        }

#if DEBUG
        /// <summary>In DEBUG builds, ensures that invariants for the class hold.</summary>
        internal void AssertValid()
        {
            WebUtil.DebugEnumIsDefined(this.TargetKind);
            WebUtil.DebugEnumIsDefined(this.TargetSource);
            Debug.Assert(this.TargetKind != RequestTargetKind.Nothing, "targetKind != RequestTargetKind.Nothing");
            Debug.Assert(
                this.TargetResourceSet == null || this.TargetSource != RequestTargetSource.None,
                "'None' targets should not have a resource set.");
            Debug.Assert(
                this.TargetKind != RequestTargetKind.Resource ||
                this.TargetResourceSet != null ||
                this.TargetKind == RequestTargetKind.OpenProperty ||
                this.TargetSource == RequestTargetSource.ServiceOperation,
                "All resource targets (except for some service operations and open properties) should have a container.");
            Debug.Assert(
                !String.IsNullOrEmpty(this.Identifier) || RequestTargetSource.None == this.TargetSource || RequestTargetKind.VoidOperation == this.TargetKind,
                "identifier must not be empty or null except for none or void service operation");
            Debug.Assert(
                    (this.TargetSource == RequestTargetSource.None)
                        == (this.TargetKind == RequestTargetKind.Metadata
                        || this.TargetKind == RequestTargetKind.ServiceDirectory
                        || this.TargetKind == RequestTargetKind.Batch),
                    "RequestTargetSource.None only allowed for certain segments");
            Debug.Assert(
                this.TargetSource != RequestTargetSource.ServiceOperation || this.Operation != null,
                "If TargetSource is ServiceOperation, then an operation must be provided");
        }
#endif
        /// <summary>
        /// Checks the EntitySetRights and ServiceOperationRights of a segment.
        /// </summary>
        internal void CheckSegmentRights()
        {
            // DEVNOTE(pqian):
            // In V2, we used to check only service operation rights inside URI Processor, and we only check for middle segments (non-last-segment)
            // This means the case where it's IQueryable<Entity> and the uri looks like /SOP(1)/NavigationProperty
            // If this is a last segment, the rights is not checked until we execute the operation, and ready to write out the response
            // There's also a problem in V1 where we did not check Service Operation Rights at all. Therefore, there's a flag in V2 which says
            // "OverrideEntitySetRights", it means we should honor service operation rights but not entity set rights
            // In V3, we must keep this behavior the same, but make it happen earlier, before the service operation is invoked, to avoid potential data corruption

            // we now know whether this call will return a single or a multiple result. Check rights
            // If specified Override EntitySet Rights, then we should ONLY rely on service operation rights
            // Otherwise, we ONLY check entity set rights (ignore service operation rights because it's a breaking change from V1)
            if (this.Operation != null && this.Operation.Kind == OperationKind.ServiceOperation && (0 != (this.Operation.ServiceOperationRights & ServiceOperationRights.OverrideEntitySetRights)))
            {
                DataServiceConfiguration.CheckServiceOperationRights(this.Operation, this.SingleResult);
            }
            else if (this.TargetKind == RequestTargetKind.Resource)
            {
                Debug.Assert(this.TargetResourceSet != null, "segment.TargetContainer != null");
                DataServiceConfiguration.CheckResourceRightsForRead(this.TargetResourceSet, this.SingleResult);
            }
        }
    }
}
