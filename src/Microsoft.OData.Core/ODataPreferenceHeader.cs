//---------------------------------------------------------------------
// <copyright file="ODataPreferenceHeader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Class to set the "Prefer" header on an <see cref="IODataRequestMessage"/> or 
    /// the "Preference-Applied" header on an <see cref="IODataResponseMessage"/>.
    /// </summary>
    public sealed class ODataPreferenceHeader
    {
        /// <summary>
        /// The return preference token.
        /// </summary>
        private const string ReturnPreferenceTokenName = "return";

        /// <summary>
        /// The return=representation preference token value.
        /// </summary>
        private const string ReturnRepresentationPreferenceTokenValue = "representation";

        /// <summary>
        /// The return=minimalpreference token value.
        /// </summary>
        private const string ReturnMinimalPreferenceTokenValue = "minimal";

        /// <summary>
        /// The odata-annotations preference-extensions token.
        /// </summary>
        private const string ODataAnnotationPreferenceToken = "odata.include-annotations";

        /// <summary>
        /// The respond-async preference token.
        /// </summary>
        private const string RespondAsyncPreferenceToken = "respond-async";

        /// <summary>
        /// The wait preference token.
        /// </summary>
        private const string WaitPreferenceTokenName = "wait";

        /// <summary>
        /// The odata.continue-on-error preference token.
        /// </summary>
        private const string ODataContinueOnErrorPreferenceToken = "odata.continue-on-error";

        /// <summary>
        /// The odata.maxpagesize=# preference token.
        /// </summary>
        private const string ODataMaxPageSizePreferenceToken = "odata.maxpagesize";

        /// <summary>
        /// The odata.track-changes preference token.
        /// </summary>
        private const string ODataTrackChangesPreferenceToken = "odata.track-changes";

        /// <summary>
        /// The Prefer header name.
        /// </summary>
        private const string PreferHeaderName = "Prefer";

        /// <summary>
        /// The Preference-Applied header name.
        /// </summary>
        private const string PreferenceAppliedHeaderName = "Preference-Applied";

        /// <summary>
        /// Empty header parameters
        /// </summary>
        private static readonly KeyValuePair<string, string>[] EmptyParameters = new KeyValuePair<string, string>[0];

        /// <summary>
        /// The odata.continue-on-error preference.
        /// </summary>
        private static readonly HttpHeaderValueElement ContinueOnErrorPreference = new HttpHeaderValueElement(ODataContinueOnErrorPreferenceToken, null, EmptyParameters);

        /// <summary>
        /// The return=minimal preference.
        /// </summary>
        private static readonly HttpHeaderValueElement ReturnMinimalPreference = new HttpHeaderValueElement(ReturnPreferenceTokenName, ReturnMinimalPreferenceTokenValue, EmptyParameters);

        /// <summary>
        /// The return=representation preference.
        /// </summary>
        private static readonly HttpHeaderValueElement ReturnRepresentationPreference = new HttpHeaderValueElement(ReturnPreferenceTokenName, ReturnRepresentationPreferenceTokenValue, EmptyParameters);

        /// <summary>
        /// The respond-async preference.
        /// </summary>
        private static readonly HttpHeaderValueElement RespondAsyncPreference = new HttpHeaderValueElement(RespondAsyncPreferenceToken, null, EmptyParameters);

        /// <summary>
        /// The odata.track-changes preference.
        /// </summary>
        private static readonly HttpHeaderValueElement TrackChangesPreference = new HttpHeaderValueElement(ODataTrackChangesPreferenceToken, null, EmptyParameters);

        /// <summary>
        /// The message to set the preference header to and to get the preference header from.
        /// </summary>
        private readonly ODataMessage message;

        /// <summary>
        /// "Prefer" if message is an IODataRequestMessage; "Preference-Applied" if message is an IODataResponseMessage.
        /// </summary>
        private readonly string preferenceHeaderName;

        /// <summary>
        /// Dictionary of preferences in the header
        /// </summary>
        private HttpHeaderValue preferences;

        /// <summary>
        /// Internal constructor to instantiate an <see cref="ODataPreferenceHeader"/> from an <see cref="IODataRequestMessage"/>.
        /// </summary>
        /// <param name="requestMessage">The request message to get and set the "Prefer" header.</param>
        internal ODataPreferenceHeader(IODataRequestMessage requestMessage)
        {
            Debug.Assert(requestMessage != null, "requestMessage != null");
            this.message = new ODataRequestMessage(requestMessage, /*writing*/ true, /*disableMessageStreamDisposal*/ false, /*maxMessageSize*/ -1);
            this.preferenceHeaderName = PreferHeaderName;
        }

        /// <summary>
        /// Internal constructor to instantiate an <see cref="ODataPreferenceHeader"/> from an <see cref="IODataResponseMessage"/>.
        /// </summary>
        /// <param name="responseMessage">The response message to get and set the "Preference-Applied" header.</param>
        internal ODataPreferenceHeader(IODataResponseMessage responseMessage)
        {
            Debug.Assert(responseMessage != null, "responseMessage != null");
            this.message = new ODataResponseMessage(responseMessage, /*writing*/ true, /*disableMessageStreamDisposal*/ false, /*maxMessageSize*/ -1);
            this.preferenceHeaderName = PreferenceAppliedHeaderName;
        }

        /// <summary>
        /// Property to get and set the "return=representation" and "return=minimal" preferences to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// Setting true sets the "return=representation" preference and clears the "return=minimal" preference.
        /// Setting false sets the "return=minimal" preference and clears the "return=representation" preference.
        /// Setting null clears the "return=representation" and "return=minimal" preferences.
        /// Returns true if the "return=representation" preference is on the header. Otherwise returns false if the "return=minimal" is on the header.
        /// Returning null indicates that "return=representation" and "return=minimal" are not on the header.
        /// </summary>
        public bool? ReturnContent
        {
            get
            {
                var returnContentPreference = this.Get(ReturnPreferenceTokenName);
                if (returnContentPreference != null)
                {
                    if (returnContentPreference.Value.ToLowerInvariant().Equals(ReturnRepresentationPreferenceTokenValue))
                    {
                        return true;
                    }

                    if (returnContentPreference.Value.ToLowerInvariant().Equals(ReturnMinimalPreferenceTokenValue))
                    {
                        return false;
                    }
                }

                return null;
            }

            set
            {
                // if the value is null, the "ReturnPreferenceTokenName" is cleared.   
                this.Clear(ReturnPreferenceTokenName);

                if (value == true)
                {
                    this.Set(ReturnRepresentationPreference);
                }

                if (value == false)
                {
                    this.Set(ReturnMinimalPreference);
                }
            }
        }

        /// <summary>
        /// Property to get and set the "odata.include-annotations" preference with the given filter to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// If the "odata-annotations" preference is already on the header, set replaces the existing instance.
        /// Returning null indicates that the "odata.include-annotations" preference is not on the header.
        /// 
        /// The filter string may be a comma delimited list of any of the following supported patterns:
        ///   "*"        -- Matches all annotation names.
        ///   "ns.*"     -- Matches all annotation names under the namespace "ns".
        ///   "ns.name"  -- Matches only the annotation name "ns.name".
        ///   "-"        -- The exclude operator may be used with any of the supported pattern, for example:
        ///                 "-ns.*"    -- Excludes all annotation names under the namespace "ns".
        ///                 "-ns.name" -- Excludes only the annotation name "ns.name".
        /// Null or empty filter is equivalent to "-*".
        /// 
        /// The relative priority of the pattern is base on the relative specificity of the patterns being compared. If pattern1 is under the namespace pattern2,
        /// pattern1 is more specific than pattern2 because pattern1 matches a subset of what pattern2 matches. We give higher priority to the pattern that is more specific.
        /// For example:
        ///  "ns.*" has higher priority than "*"
        ///  "ns.name" has higher priority than "ns.*"
        ///  "ns1.name" has same priority as "ns2.*"
        /// 
        /// Patterns with the exclude operator takes higher precedence than the same pattern without.
        /// For example: "-ns.name" has higher priority than "ns.name".
        /// 
        /// Examples:
        ///   "ns1.*,ns.name"       -- Matches any annotation name under the "ns1" namespace and the "ns.name" annotation.
        ///   "*,-ns.*,ns.name"     -- Matches any annotation name outside of the "ns" namespace and only "ns.name" under the "ns" namespace.
        /// </summary>
        public string AnnotationFilter
        {
            get
            {
                var odataAnnotations = this.Get(ODataAnnotationPreferenceToken);

                if (odataAnnotations != null)
                {
                    return odataAnnotations.Value.Trim('"');
                }

                return null;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotEmpty(value, "AnnotationFilter");

                if (value == null)
                {
                    this.Clear(ODataAnnotationPreferenceToken);
                }
                else
                {
                    this.Set(new HttpHeaderValueElement(ODataAnnotationPreferenceToken, AddQuotes(value), EmptyParameters));
                }
            }
        }

        /// <summary>
        /// Property to get and set the "respond-async" preference to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// Setting true sets the "respond-async" preference.
        /// Setting false clears the "respond-async" preference.
        /// Returns true if the "respond-async" preference is on the header. Otherwise returns false if the "respond-async" is not on the header.
        /// </summary>
        public bool RespondAsync
        {
            get
            {
                return this.Get(RespondAsyncPreferenceToken) != null;
            }

            set
            {
                if (value)
                {
                    this.Set(RespondAsyncPreference);
                }
                else
                {
                    this.Clear(RespondAsyncPreferenceToken);
                }
            }
        }

        /// <summary>
        /// Property to get and set the "wait" preference to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// Setting N sets the "wait=N" preference.
        /// Setting null clears the "wait" preference.
        /// Returns N if the "wait=N" preference is on the header.
        /// Returning null indicates that "wait" is not on the header.
        /// </summary>
        public int? Wait
        {
            get
            {
                var wait = this.Get(WaitPreferenceTokenName);

                if (wait != null)
                {
                    int value;
                    if (int.TryParse(wait.Value, out value))
                    {
                        return value;
                    }

                    // TODO: Fix hard code string before Loc of 6.16 release 
                    throw new ODataException(string.Format(CultureInfo.InvariantCulture,
                        "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.",
                        wait.Value, ODataPreferenceHeader.WaitPreferenceTokenName));
                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    this.Set(new HttpHeaderValueElement(WaitPreferenceTokenName, string.Format(CultureInfo.InvariantCulture, "{0}", value), EmptyParameters));
                }
                else
                {
                    this.Clear(WaitPreferenceTokenName);
                }
            }
        }

        /// <summary>
        /// Property to get and set the "odata.continue-on-error" preference to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// Setting true sets the "odata.continue-on-error" preference.
        /// Setting false clears the "odata.continue-on-error" preference.
        /// Returns true of the "odata.continue-on-error" preference is on the header.  Otherwise returns false if the "odata.continue-on-error" is not on the header.
        /// </summary>
        public bool ContinueOnError
        {
            get
            {
                return this.Get(ODataContinueOnErrorPreferenceToken) != null;
            }

            set
            {
                if (value)
                {
                    this.Set(ContinueOnErrorPreference);
                }
                else
                {
                    this.Clear(ODataContinueOnErrorPreferenceToken);
                }
            }
        }

        /// <summary>
        /// Property to get and set the "odata.maxpagesize" preference to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// Setting N sets the "odata.maxpagesize=N" preference.
        /// Setting null clears the "odata.maxpagesize" preference.
        /// Returns N if the "odata.maxpagesize=N" preference is on the header.
        /// Returning null indicates that "odata.maxpagesize" is not on the header.
        /// </summary>
        public int? MaxPageSize
        {
            get
            {
                var maxPageSizeHttpHeaderValueElement = this.Get(ODataMaxPageSizePreferenceToken);

                if (maxPageSizeHttpHeaderValueElement != null)
                {
                    int value;
                    if (int.TryParse(maxPageSizeHttpHeaderValueElement.Value, out value))
                    {
                        return value;
                    }

                    // TODO: Fix hard code string before Loc of 6.16 release 
                    throw new ODataException(string.Format(CultureInfo.InvariantCulture,
                        "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.",
                        maxPageSizeHttpHeaderValueElement.Value, ODataPreferenceHeader.ODataMaxPageSizePreferenceToken));
                }

                return null;
            }

            set
            {
                if (value.HasValue)
                {
                    this.Set(new HttpHeaderValueElement(ODataMaxPageSizePreferenceToken, string.Format(CultureInfo.InvariantCulture, "{0}", value.Value), EmptyParameters));
                }
                else
                {
                    this.Clear(ODataMaxPageSizePreferenceToken);
                }
            }
        }

        /// <summary>
        /// Property to get and set the "odata.track-changes" preference to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// Setting true sets the "odata.track-changes" preference.
        /// Setting false clears the "odata.track-changes" preference.
        /// Returns true of the "odata.track-changes" preference is on the header.  Otherwise returns false if the "odata.track-changes" is not on the header.
        /// </summary>
        public bool TrackChanges
        {
            get
            {
                return this.Get(ODataTrackChangesPreferenceToken) != null;
            }

            set
            {
                if (value)
                {
                    this.Set(TrackChangesPreference);
                }
                else
                {
                    this.Clear(ODataTrackChangesPreferenceToken);
                }
            }
        }

        /// <summary>
        /// Dictionary of preferences in the header.
        /// </summary>
        private HttpHeaderValue Preferences
        {
            get { return this.preferences ?? (this.preferences = this.ParsePreferences()); }
        }

        /// <summary>
        /// Adds quotes around the given text value.
        /// </summary>
        /// <param name="text">text to quote.</param>
        /// <returns>Returns the quoted text.</returns>
        private static string AddQuotes(string text)
        {
            return "\"" + text + "\"";
        }

        /// <summary>
        /// Clears the <paramref name="preference"/> from the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// </summary>
        /// <param name="preference">The preference to clear.</param>
        private void Clear(string preference)
        {
            Debug.Assert(!string.IsNullOrEmpty(preference), "!string.IsNullOrEmpty(preference)");
            if (this.Preferences.Remove(preference))
            {
                this.SetPreferencesToMessageHeader();
            }
        }

        /// <summary>
        /// Sets the <paramref name="preference"/> to the "Prefer" header on the underlying IODataRequestMessage or
        /// the "Preference-Applied" header on the underlying IODataResponseMessage.
        /// </summary>
        /// <param name="preference">The preference to set.</param>
        /// <remarks>
        /// If <paramref name="preference"/> is already on the header, this method does a replace rather than adding another instance of the same preference.
        /// </remarks>
        private void Set(HttpHeaderValueElement preference)
        {
            Debug.Assert(preference != null, "preference != null");
            this.Preferences[preference.Name] = preference;
            this.SetPreferencesToMessageHeader();
        }

        /// <summary>
        /// Gets the <paramref name="preferenceName"/> from the "Prefer" header from the underlying <see cref="IODataRequestMessage"/> or
        /// the "Preference-Applied" header from the underlying <see cref="IODataResponseMessage"/>.
        /// </summary>
        /// <param name="preferenceName">The preference to get.</param>
        /// <returns>Returns a key value pair of the <paramref name="preferenceName"/> and its value. The Value property of the key value pair may be null since not
        /// all preferences have value. If the <paramref name="preferenceName"/> is missing from the header, null is returned.</returns>
        private HttpHeaderValueElement Get(string preferenceName)
        {
            Debug.Assert(!string.IsNullOrEmpty(preferenceName), "!string.IsNullOrEmpty(preferenceName)");
            HttpHeaderValueElement value;
            if (!this.Preferences.TryGetValue(preferenceName, out value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// Parses the current preference values to a dictionary of preference and value pairs.
        /// </summary>
        /// <returns>Returns a dictionary of preference and value pairs; null if the preference header has not been set.</returns>
        private HttpHeaderValue ParsePreferences()
        {
            string preferenceHeaderValue = this.message.GetHeader(this.preferenceHeaderName);
            HttpHeaderValueLexer preferenceHeaderLexer = HttpHeaderValueLexer.Create(this.preferenceHeaderName, preferenceHeaderValue);
            return preferenceHeaderLexer.ToHttpHeaderValue();
        }

        /// <summary>
        /// Sets the "Prefer" or the "Preference-Applied" header to the underlying message.
        /// </summary>
        private void SetPreferencesToMessageHeader()
        {
            Debug.Assert(this.preferences != null, "this.preferences != null");
            this.message.SetHeader(this.preferenceHeaderName, this.Preferences.ToString());
        }
    }
}