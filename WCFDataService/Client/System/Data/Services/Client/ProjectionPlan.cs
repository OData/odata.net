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
    #region Namespaces

    using System;
    using System.Data.Services.Client.Materialization;
    using System.Diagnostics;
    using Microsoft.Data.OData;

    #endregion Namespaces

    /// <summary>Use this class to store a materialization plan used with projections.</summary>
    internal class ProjectionPlan
    {
#if DEBUG
        /// <summary>Source projection for this plan.</summary>
        internal System.Linq.Expressions.Expression SourceProjection
        {
            get;
            set;
        }

        /// <summary>Target projection for this plan.</summary>
        internal System.Linq.Expressions.Expression TargetProjection
        {
            get;
            set;
        }
#endif

        /// <summary>Last segment type for query.</summary>
        /// <remarks>This typically matches the expected element type at runtime.</remarks>
        internal Type LastSegmentType
        {
            get;
            set;
        }

        /// <summary>Provides a method to materialize a payload.</summary>
        internal Func<object, object, Type, object> Plan 
        { 
            get;
            set;
        }

        /// <summary>Expected type to project.</summary>
        internal Type ProjectedType
        {
            get;
            set;
        }

#if DEBUG
        /// <summary>Returns a string representation for this object.</summary>
        /// <returns>A string representation for this object, suitable for debugging.</returns>
        public override string ToString()
        {
            return "Plan - projection: " + this.SourceProjection + "\r\nBecomes: " + this.TargetProjection;
        }
#endif

        /// <summary>Runs this plan.</summary>
        /// <param name="materializer">Materializer under which materialization should happen.</param>
        /// <param name="entry">Root entry to materialize.</param>
        /// <param name="expectedType">Expected type for the <paramref name="entry"/>.</param>
        /// <returns>The materialized object.</returns>
        internal object Run(ODataEntityMaterializer materializer, ODataEntry entry, Type expectedType)
        {
            Debug.Assert(materializer != null, "materializer != null");
            Debug.Assert(entry != null, "entry != null");

            return this.Plan(materializer, entry, expectedType);
        }
    }
}
