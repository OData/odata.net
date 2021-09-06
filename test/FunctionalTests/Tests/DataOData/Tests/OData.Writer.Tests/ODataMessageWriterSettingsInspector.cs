//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsInspector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    internal static class ODataMessageWriterSettingsInspector
    {
        public static object GetWriterBehavior(this ODataMessageWriterSettings settings)
        {
            return ReflectionUtils.GetProperty(settings, "WriterBehavior");
        }

        public static string GetAcceptableMediaTypes(this ODataMessageWriterSettings settings)
        {
            return (string)ReflectionUtils.GetProperty(settings, "AcceptableMediaTypes");
        }

        public static string GetAcceptableCharsets(this ODataMessageWriterSettings settings)
        {
            return (string)ReflectionUtils.GetProperty(settings, "AcceptableCharsets");
        }

        public static ODataFormat GetFormat(this ODataMessageWriterSettings settings)
        {
            return (ODataFormat)ReflectionUtils.GetProperty(settings, "Format");
        }

        public static Func<ODataResource, XmlWriter, XmlWriter> GetAtomStartResourceXmlCustomizationCallback(this ODataMessageWriterSettings settings)
        {
            return (Func<ODataResource, XmlWriter, XmlWriter>)ReflectionUtils.GetProperty(settings, "AtomStartResourceXmlCustomizationCallback");
        }

        public static Action<ODataResource, XmlWriter, XmlWriter> GetAtomEndResourceXmlCustomizationCallback(this ODataMessageWriterSettings settings)
        {
            return (Action<ODataResource, XmlWriter, XmlWriter>)ReflectionUtils.GetProperty(settings, "AtomEndResourceXmlCustomizationCallback");
        }
    }
}
