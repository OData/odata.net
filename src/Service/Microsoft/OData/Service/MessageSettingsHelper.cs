//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
