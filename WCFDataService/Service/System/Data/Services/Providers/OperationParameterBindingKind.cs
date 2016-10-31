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

namespace System.Data.Services.Providers
{
    /// <summary> Enumeration for classifying the different kinds of operation parameter binding. </summary>
    public enum OperationParameterBindingKind
    {
        /// <summary>
        /// Used when the first parameter of a service action is not a binding parameter.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Used when the first parameter of a service action is a binding parameter and some or all instances of the binding parameter type 
        /// may be bound to the service action.
        /// </summary>
        Sometimes = 1,

        /// <summary>
        /// Used when the first parameter of a service action is a binding parameter and all instances of the binding parameter type 
        /// must be bound to the service action.
        /// </summary>
        /// <remarks>When this value is set, the <see cref="IDataServiceActionProvider.AdvertiseServiceAction"/> method will not be called for the service action."/> </remarks>
        Always = 2,
    }
}
