//---------------------------------------------------------------------
// <copyright file="KeyExpressionSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A segment that addresses a single entity by key
    /// </summary>
    public class KeyExpressionSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the KeyExpressionSegment class
        /// </summary>
        /// <param name="values">The key values</param>
        internal KeyExpressionSegment(IEnumerable<KeyValuePair<MemberProperty, object>> values)
            : base()
        {
            ExceptionUtilities.CheckCollectionNotEmpty(values, "values");

            this.IncludedValues = new ReadOnlyCollection<KeyValuePair<MemberProperty, object>>(values.ToList());
            this.IsComposite = this.IncludedValues.Count > 1;
        }

        /// <summary>
        /// Gets the values present in the key. If not a composite key, then the member property may be null
        /// </summary>
        public ReadOnlyCollection<KeyValuePair<MemberProperty, object>> IncludedValues { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the key has multiple properties
        /// </summary>
        public bool IsComposite { get; private set; }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.Key; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return false; }
        }
    }
}
