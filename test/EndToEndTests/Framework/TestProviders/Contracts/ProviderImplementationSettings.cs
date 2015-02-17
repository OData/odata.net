//---------------------------------------------------------------------
// <copyright file="ProviderImplementationSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Contracts
{
    using System;
    using Microsoft.Test.OData.Framework.TestProviders.Common;

    /// <summary>
    /// Semi-singleton class for checking universal settings across test implementations of service-providers. 
    /// Can be temporarily overridden to support cases where verification needs to be disabled or other behavior modified.
    /// </summary>
    public class ProviderImplementationSettings
    {
        private static readonly ProviderImplementationSettings defaultSettings = new ProviderImplementationSettings();
        private static ProviderImplementationSettings currentSettings = defaultSettings;
        private static bool allowEditing = false;

        private bool strictInputVerification = true;
        private bool enforceMetadataCaching = true;

        /// <summary>
        /// Prevents a default instance of the ProviderImplementationSettings class from being created.
        /// </summary>
        private ProviderImplementationSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ProviderImplementationSettings class by copying the given settings.
        /// </summary>
        /// <param name="toCopy">The settings to copy. Must be the current settings.</param>
        private ProviderImplementationSettings(ProviderImplementationSettings toCopy)
        {
            ExceptionUtilities.Assert(Current == toCopy, "Only the current settings can be copied");
            this.strictInputVerification = toCopy.strictInputVerification;
        }

        /// <summary>
        /// Gets the current settings
        /// </summary>
        public static ProviderImplementationSettings Current
        {
            get { return currentSettings; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to strictly verify provider input. 
        /// Can only be changed within a call to Override.
        /// Should only be set to false when provider calls are being made by test code, never when being called by the product.
        /// </summary>
        public virtual bool StrictInputVerification
        {
            get
            {
                return this.strictInputVerification;
            }

            set
            {
                ExceptionUtilities.Assert(allowEditing, "Cannot change settings outside of a call to Override");
                this.strictInputVerification = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether providers should enforce metadata caching.
        /// Can only be changed within a call to Override.
        /// Should only be set to false when provider calls are being made by test code, never when being called by the product.
        /// </summary>
        public virtual bool EnforceMetadataCaching
        {
            get
            {
                return this.enforceMetadataCaching;
            }

            set
            {
                ExceptionUtilities.Assert(allowEditing, "Cannot change settings outside of a call to Override");
                this.enforceMetadataCaching = value;
            }
        }

        /// <summary>
        /// Temporarily overrides the current provider settings for the scope of the given action
        /// </summary>
        /// <param name="change">The callback to temporarily change the current settings</param>
        /// <param name="action">The action to run while the settings are changed</param>
        public static void Override(Action<ProviderImplementationSettings> change, Action action)
        {
            // default settings is used for lock because it never changes. Current settings will change, and so is a bad choice for the lock
            lock (defaultSettings)
            {
                var originalSettings = currentSettings;
                try
                {
                    var settings = new ProviderImplementationSettings(Current);
                    currentSettings = settings;
                    allowEditing = true;
                    change(settings);
                    allowEditing = false;
                    action();
                }
                finally
                {
                    allowEditing = false;
                    currentSettings = originalSettings;
                }
            }
        }
    }
}
