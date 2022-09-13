//---------------------------------------------------------------------
// <copyright file="ODataWriterBehaviorInspector.cs" company="Microsoft">
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

    internal static class ODataWriterBehaviorInspector
    {
        public static int GetFormatBehaviorKind(this ODataMessageWriterSettings writerSettings)
        {
            object writerBehavior = ODataMessageWriterSettingsInspector.GetWriterBehavior(writerSettings);
            return GetFormatBehaviorKind(writerBehavior);
        }

        public static int GetFormatBehaviorKind(object writerBehavior)
        {
            return (int)ReflectionUtils.GetProperty(writerBehavior, "FormatBehaviorKind");
        }

        public static int GetApiBehaviorKind(this ODataMessageWriterSettings writerSettings)
        {
            object writerBehavior = ODataMessageWriterSettingsInspector.GetWriterBehavior(writerSettings);
            return GetApiBehaviorKind(writerBehavior);
        }

        public static int GetApiBehaviorKind(object writerBehavior)
        {
            return (int)ReflectionUtils.GetProperty(writerBehavior, "ApiBehaviorKind");
        }

        public static bool GetUseV1ProviderBehavior(this ODataMessageWriterSettings writerSettings)
        {
            object writerBehavior = ODataMessageWriterSettingsInspector.GetWriterBehavior(writerSettings);
            return (bool)ReflectionUtils.GetProperty(writerBehavior, "UseV1ProviderBehavior");
        }

        public static bool GetUseV1ProviderBehavior(object writerBehavior)
        {
            return (bool)ReflectionUtils.GetProperty(writerBehavior, "UseV1ProviderBehavior");
        }

        public static bool GetAllowNullValuesForNonNullablePrimitiveTypes(this ODataMessageWriterSettings writerSettings)
        {
            object writerBehavior = ODataMessageWriterSettingsInspector.GetWriterBehavior(writerSettings);
            return (bool)ReflectionUtils.GetProperty(writerBehavior, "AllowNullValuesForNonNullablePrimitiveTypes");
        }

        public static bool GetAllowNullValuesForNonNullablePrimitiveTypes(object writerBehavior)
        {
            return (bool)ReflectionUtils.GetProperty(writerBehavior, "AllowNullValuesForNonNullablePrimitiveTypes");
        }

        public static bool GetAllowDuplicatePropertyNames(this ODataMessageWriterSettings writerSettings)
        {
            object writerBehavior = ODataMessageWriterSettingsInspector.GetWriterBehavior(writerSettings);
            return (bool)ReflectionUtils.GetProperty(writerBehavior, "AllowDuplicatePropertyNames");
        }

        public static bool GetAllowDuplicatePropertyNames(object writerBehavior)
        {
            return (bool)ReflectionUtils.GetProperty(writerBehavior, "AllowDuplicatePropertyNames");
        }
    }
}
