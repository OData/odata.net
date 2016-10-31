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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    #endregion Namespaces

    /// <summary> Represents the behavior of readers when reading property with null value. </summary>
    public enum ODataNullValueBehaviorKind
    {
        /// <summary>
        /// The default behavior - this means validate the null value against the declared type
        /// and then report the null value.
        /// </summary>
        Default = 0,

        /// <summary>
        /// This means to not report the value and not validate it against the model.
        /// </summary>
        /// <remarks>
        /// This setting can be used to correctly work with clients that send null values
        /// for uninitialized properties in requests instead of omitting them altogether.
        /// </remarks>
        IgnoreValue = 1,

        /// <summary>
        /// This means to report the value, but not validate it against the model.
        /// </summary>
        DisableValidation = 2,
    }
}
