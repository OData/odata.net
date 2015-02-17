//---------------------------------------------------------------------
// <copyright file="ConfigurationManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Windows.ApplicationModel;
    using Windows.Storage;

    public class ConfigurationManager
    {
        private static Dictionary<string, string> appSettings = null;

        /// <summary>
        /// Loads the app settings data from the app.config file.
        /// </summary>
        public static Dictionary<string, string> AppSettings
        {
            get
            {
                if (appSettings == null)
                {
                    LoadAppSettingsAsync().Wait();
                }

                return appSettings;
            }
        }

        /// <summary>
        /// Reads the contents of the app.config file, 
        /// looking for the file in the documents library first and in the app package contents if not found, 
        /// and stores the values in-memory 
        /// </summary>
        /// <returns></returns>
        private static async Task LoadAppSettingsAsync()
        {
            const string fileName = "App.config";
            
            StorageFile appSettingsFile = null;
            try
            {
                appSettingsFile = await KnownFolders.DocumentsLibrary.GetFileAsync(fileName);
            }
            catch (FileNotFoundException)
            {
            }

            if (appSettingsFile == null)
            {
                appSettingsFile = await Package.Current.InstalledLocation.GetFileAsync(fileName);
            }

            var appSettingsContents = await FileIO.ReadTextAsync(appSettingsFile);

            appSettings = new Dictionary<string, string>();
            var keyValues = XElement.Parse(appSettingsContents).Descendants("add").Select(d => new { Key = (string)d.Attribute("key"), Value = (string)d.Attribute("value") });
            foreach (var setting in keyValues)
            {
                appSettings.Add(setting.Key, setting.Value);
            }
        }
    }
}
