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

namespace System.Data.Services
{
    /// <summary>
    /// This interface declares the methods required to support enumerators for results and for
    /// associated segments on a WCF Data Service $expand query option.
    /// </summary>
    public interface IExpandedResult
    {
        /// <summary>Gets the element with expanded properties.</summary>
        /// <returns>The object in a property expanded by <see cref="T:System.Data.Services.IExpandedResult" />.</returns>
        object ExpandedElement
        { 
            get;
        }

        /// <summary>Gets the value for a named property of the result.</summary>
        /// <returns>The value of the property.</returns>
        /// <param name="name">The name of the property for which to get enumerable results.</param>
        /// <remarks>
        /// If the element returned in turn has properties which are expanded out-of-band
        /// of the object model, then the result will also be of type <see cref="IExpandedResult"/>,
        /// and the value will be available through <see cref="ExpandedElement"/>.
        /// </remarks>
        object GetExpandedPropertyValue(string name);
    }
}
