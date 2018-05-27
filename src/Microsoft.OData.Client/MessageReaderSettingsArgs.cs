//---------------------------------------------------------------------
// <copyright file="MessageReaderSettingsArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using Microsoft.OData;

    /// <summary>
    /// Arguments used to configure the odata message reader settings.
    /// </summary>
    public class MessageReaderSettingsArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReaderSettingsArgs"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MessageReaderSettingsArgs(ODataMessageReaderSettings settings)
        {
            WebUtil.CheckArgumentNull(settings, "settings");

            this.Settings = settings;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public ODataMessageReaderSettings Settings { get; private set; }
    }
}
