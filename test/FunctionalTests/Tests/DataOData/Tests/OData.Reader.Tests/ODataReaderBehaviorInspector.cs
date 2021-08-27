//---------------------------------------------------------------------
// <copyright file="ODataReaderBehaviorInspector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    // These tests and helpers are disabled on Silverlight and Phone because they  
    // use private reflection not available on Silverlight and Phone
    internal static class ODataReaderBehaviorInspector
    {
        public static int GetFormatBehaviorKind(this ODataMessageReaderSettings readerSettings)
        {
            object readerBehavior = ODataMessageReaderSettingsInspector.GetReaderBehavior(readerSettings);
            return GetFormatBehaviorKind(readerBehavior);
        }

        public static int GetFormatBehaviorKind(object readerBehavior)
        {
            return (int)ReflectionUtils.GetProperty(readerBehavior, "FormatBehaviorKind");
        }

        public static int GetApiBehaviorKind(this ODataMessageReaderSettings readerSettings)
        {
            object readerBehavior = ODataMessageReaderSettingsInspector.GetReaderBehavior(readerSettings);
            return GetApiBehaviorKind(readerBehavior);
        }

        public static int GetApiBehaviorKind(object readerBehavior)
        {
            return (int)ReflectionUtils.GetProperty(readerBehavior, "ApiBehaviorKind");
        }

        public static bool GetAllowDuplicatePropertyNames(this ODataMessageReaderSettings readerSettings)
        {
            object readerBehavior = ODataMessageReaderSettingsInspector.GetReaderBehavior(readerSettings);
            return (bool)ReflectionUtils.GetProperty(readerBehavior, "AllowDuplicatePropertyNames");
        }

        public static bool GetAllowDuplicatePropertyNames(object readerBehavior)
        {
            return (bool)ReflectionUtils.GetProperty(readerBehavior, "AllowDuplicatePropertyNames");
        }

        public static bool GetUsesV1ProviderBehavior(object readerBehavior)
        {
            return (bool)ReflectionUtils.GetProperty(readerBehavior, "UseV1ProviderBehavior");
        }

        public static string GetODataNamespace(object readerBehavior)
        {
            return (string)ReflectionUtils.GetProperty(readerBehavior, "ODataNamespace");
        }

        public static string GetODataTypeScheme(object readerBehavior)
        {
            return (string)ReflectionUtils.GetProperty(readerBehavior, "ODataTypeScheme");
        }

        public static Func<IEdmType, string, IEdmType> GetTypeResolver(object readerBehavior)
        {
            return (Func<IEdmType, string, IEdmType>)ReflectionUtils.GetProperty(readerBehavior, "TypeResolver");
        }
    }
}
