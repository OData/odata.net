//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Client
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
