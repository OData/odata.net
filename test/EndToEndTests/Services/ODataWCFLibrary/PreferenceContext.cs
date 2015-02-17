//---------------------------------------------------------------------
// <copyright file="PreferenceContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Globalization;
    using System.Linq;

    public class PreferenceContext
    {
        public string Preference { get; private set; }

        public PreferenceContext(string preference)
        {
            if (preference != null)
            {
                this.Preference = preference;
                this.ParsePreference();
            }
            else
            {
                throw new ArgumentNullException("Preference");
            }
        }

        /// <summary>
        /// odata.track-changes in Prefer Header which is used to enable track changes
        /// </summary>
        public bool TrackingChanges { get; private set; }

        /// <summary>
        /// respond-async in Prefer Header which is used to enable server async
        /// </summary>
        public bool RespondAsync { get; private set; }

        /// <summary>
        /// include-annotations values in Prefer Header
        /// </summary>
        public string IncludeAnnotations { get; private set; }

        /// <summary>
        /// If the request contains 'odata.maxpagesize' preference header, then returns the user's setting, otherwise null.
        /// </summary>
        public int? MaxPageSize { get; private set; }

        public string Return { get; private set; }

        private void ParsePreference()
        {
            string[] parameters = this.Preference.Split(';');
            foreach (string parameter in parameters)
            {
                if (parameter.Contains('='))
                {
                    string[] keyValue = parameter.Split('=');
                    switch (keyValue[0])
                    {
                        case ServiceConstants.Preference_IncludeAnnotations:
                            this.IncludeAnnotations = keyValue[1];
                            break;
                        case ServiceConstants.Preference_MaxPageSize:
                            this.MaxPageSize = int.Parse(keyValue[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;
                        case ServiceConstants.Preference_Return:
                            this.Return = keyValue[1];
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (parameter)
                    {
                        case ServiceConstants.Preference_TrackChanging:
                            this.TrackingChanges = true;
                            break;
                        case ServiceConstants.Preference_RespondAsync:
                            this.RespondAsync = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}