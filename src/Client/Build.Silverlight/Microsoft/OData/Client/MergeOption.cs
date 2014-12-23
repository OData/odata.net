//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Used to specify a value synchronization strategy. 
    /// </summary>
    /// <remarks>
    /// Equivalent to System.Data.dll!System.Data.LoadOption
    /// Equivalent to System.Data.Linq.dll!System.Data.Linq.RefreshMode
    /// Equivalent to System.Data.Entity.dll!System.Data.Objects.MergeOption
    /// </remarks>
    public enum MergeOption
    {
        /// <summary>
        /// No current values are modified.
        /// </summary>
        /// <remarks>
        /// Equivalent to System.Data.Objects.MergeOption.AppendOnly
        /// Equivalent to System.Data.Linq.RefreshMode.KeepCurrentValues
        /// </remarks>
        AppendOnly = 0,

        /// <summary>
        /// All current values are overwritten with current store values,
        /// regardless of whether they have been changed.
        /// </summary>
        /// <remarks>
        /// Equivalent to System.Data.LoadOption.OverwriteChanges
        /// Equivalent to System.Data.Objects.MergeOption.OverwriteChanges
        /// Equivalent to System.Data.Linq.RefreshMode.OverwriteCurrentValues
        /// </remarks>
        OverwriteChanges = 1,

        /// <summary>
        /// Current values that have been changed are not modified, but
        /// any unchanged values are updated with the current store
        /// values.  No changes are lost in this merge.
        /// </summary>
        /// <remarks>
        /// Equivalent to System.Data.LoadOption.PreserveChanges
        /// Equivalent to System.Data.Objects.MergeOption.PreserveChanges
        /// Equivalent to System.Data.Linq.RefreshMode.KeepChanges
        /// </remarks>
        PreserveChanges = 2,

        /// <summary>
        /// Equivalent to System.Data.Objects.MergeOption.NoTracking
        /// </summary>
        NoTracking = 3,
    }
}
