//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderSettingsInspector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    // These tests and helpers are disabled on Silverlight and Phone because they  
    // use private reflection not available on Silverlight and Phone
    internal static class ODataMessageReaderSettingsInspector
    {
        public static object GetReaderBehavior(this ODataMessageReaderSettings settings)
        {
            return ReflectionUtils.GetProperty(settings, "ReaderBehavior");
        }

        public static Func<ODataResource, XmlReader, Uri, XmlReader> GetAtomEntryXmlCustomizationCallback(this ODataMessageReaderSettings settings)
        {
            return (Func<ODataResource, XmlReader, Uri, XmlReader>)ReflectionUtils.GetProperty(settings, "AtomEntryXmlCustomizationCallback");
        }
    }
}
