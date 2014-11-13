//   OData .NET Libraries ver. 6.8.1
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
    /// <summary>Represents the type of HTTP implementation to use when accessing the data service.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Accurately describes what the enum represents, and also this class has already shipped as a public type.")]
#if !ASTORIA_LIGHT
    internal enum HttpStack
#else
    public enum HttpStack
#endif
    {
        /// <summary>
        /// Automatically choose the HTTP stack
        /// When possible XmlHttp stack will be used, otherwise the Client stack will be used
        /// </summary>
        Auto = 0,
#if ASTORIA_LIGHT
        /// <summary>
        /// The ClientHttp stack will be used for HTTP requests
        /// </summary>
        ClientHttp,

        /// <summary>
        /// The XmlHttpRequest stack will be used for HTTP requests
        /// </summary>
        XmlHttp
#endif
    }
}
