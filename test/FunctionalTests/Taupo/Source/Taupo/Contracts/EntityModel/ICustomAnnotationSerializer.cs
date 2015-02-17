//---------------------------------------------------------------------
// <copyright file="ICustomAnnotationSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Xml.Linq;

    /// <summary>
    /// Interface implemented by annotations which are serialized in a non-standard special way.
    /// </summary>
    /// <remarks>
    /// Types which implement this interface must also provide a constructor which takes
    /// XObject (XAttribute or XElement), which will be used for deserialization.
    /// </remarks>
    public interface ICustomAnnotationSerializer
    {
        /// <summary>
        /// Gets the XObject representing the annotation when serialized to CSDL/SSDL file.
        /// </summary>
        /// <returns>XObject representing the annotation.</returns>
        XObject GetXObject();
    }
}
