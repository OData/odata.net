//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Client
{
    using System;

    /// <summary>
    /// This class marks a type that represents an Astoria client entity
    /// such that the Astoria client will treat it as a media entry 
    /// according to ATOM's "media link entry" concept.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MediaEntryAttribute : Attribute
    {
        /// <summary>Name of the member that contains the data for the media entry</summary>
        private readonly string mediaMemberName;

        /// <summary>Creates a new instance of <see cref="T:System.Data.Services.Client.MediaEntryAttribute" />.</summary>
        /// <param name="mediaMemberName">A string value that identifies the property that holds media data.</param>
        /// <remarks>
        /// Creates a new MediaEntryAttribute attribute and sets the name
        /// of the member that contains the actual data of the media entry
        /// (e.g. a byte[] containing a picture, a string containing HTML, etc.)
        /// </remarks>
        public MediaEntryAttribute(string mediaMemberName)
        {
            Util.CheckArgumentNull(mediaMemberName, "mediaMemberName");
            this.mediaMemberName = mediaMemberName;
        }

        /// <summary>The name of the property on the class that holds the media, usually binary data.</summary>
        /// <returns>A string value that identifies the property that holds media data.</returns>
        public string MediaMemberName
        {
            get { return this.mediaMemberName; }
        }
    }
}
