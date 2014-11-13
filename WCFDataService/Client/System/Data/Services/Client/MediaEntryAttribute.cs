//   OData .NET Libraries ver. 5.6.3
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
