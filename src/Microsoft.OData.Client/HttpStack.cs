//---------------------------------------------------------------------
// <copyright file="HttpStack.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    /// <summary>Represents the type of HTTP implementation to use when accessing the data service.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Accurately describes what the enum represents, and also this class has already shipped as a public type.")]
    internal enum HttpStack
    {
        /// <summary>
        /// Automatically choose the HTTP stack
        /// When possible XmlHttp stack will be used, otherwise the Client stack will be used
        /// </summary>
        Auto = 0,
    }
}