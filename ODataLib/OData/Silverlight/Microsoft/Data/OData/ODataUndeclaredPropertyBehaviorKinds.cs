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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>Enumerates the behavior of readers when reading undeclared property.</summary>
    [Flags]
    public enum ODataUndeclaredPropertyBehaviorKinds
    {
        /// <summary>
        /// The default behavior - the reader will fail if it finds a property which is not declared by the model
        /// and the type is not open.
        /// </summary>
        None = 0,

        /// <summary>
        /// The reader will skip reading the property if it's not declared by the model and the current type is not open.
        /// </summary>
        /// <remarks>
        /// This flag can only be used when reading responses.
        /// All information about the undeclared property is going to be ignored, so for example ATOM metadata related to that property
        /// will not be reported either.
        /// </remarks>
        IgnoreUndeclaredValueProperty = 1,

        /// <summary>
        /// The reader will read and report link properties which are not declared by the model.
        /// </summary>
        /// <remarks>
        /// This flag can only be used when reading responses.
        /// If a link property in the payload is defined in the model it will be read as usual. If it is not declared
        /// it will still be read and reported, but it won't be validated against the model.
        /// 
        /// Link properties are:
        /// - Navigation links
        /// - Association links
        /// - Stream properties
        /// </remarks>
        ReportUndeclaredLinkProperty = 2,

        /// <summary>
        /// Reading/writing undeclared properties will be supported.
        /// </summary>
        SupportUndeclaredValueProperty = 4,
    }
}
