//---------------------------------------------------------------------
// <copyright file="MessageSettingsHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !WINRT
#if ASTORIA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Service
#endif
{
    using System.Reflection;
    using Microsoft.OData.Core;

    /// <summary>
    /// Message setting helper
    /// </summary>
    internal static class MessageSettingsHelper
    {
        /// <summary>
        /// PropertyInfo for ODataMessageWriterSettings.EnableAtom
        /// </summary>
        private static readonly PropertyInfo WriterSettingsEnableAtom = typeof(ODataMessageWriterSettings).GetProperty("EnableAtom", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// PropertyInfo for ODataMessageReaderSettings.EnableAtom
        /// </summary>
        private static readonly PropertyInfo ReaderSettingsEnableAtom = typeof(ODataMessageReaderSettings).GetProperty("EnableAtom", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Enable Atom for given ODataMessageWriterSettings
        /// </summary>
        /// <param name="settings">The settings to operate on.</param>
        public static void EnableAtomSupport(this ODataMessageWriterSettings settings)
        {
            WriterSettingsEnableAtom.SetValue(settings, true, null);
        }

        /// <summary>
        /// Enable Atom for given ODataMessageReaderSettings
        /// </summary>
        /// <param name="settings">The settings to operate on.</param>
        public static void EnableAtomSupport(this ODataMessageReaderSettings settings)
        {
            ReaderSettingsEnableAtom.SetValue(settings, true, null);
        }
    }
}
#endif
