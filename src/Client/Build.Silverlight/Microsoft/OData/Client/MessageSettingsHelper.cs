//   OData .NET Libraries ver. 6.9
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
