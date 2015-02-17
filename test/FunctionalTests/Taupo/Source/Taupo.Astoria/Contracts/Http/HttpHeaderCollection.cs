//---------------------------------------------------------------------
// <copyright file="HttpHeaderCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System.Collections.Generic;

    /// <summary>
    /// An initializer for various commonly used HTTP headers
    /// Example:
    /// new HttpHeaderCollection()
    /// {
    ///     ContentType = MimeTypes.ApplicationXml,
    ///     Accept = MimeTypes.ApplicationJson
    /// }
    /// </summary>
    public class HttpHeaderCollection : IEnumerable<KeyValuePair<string, string>>
    {
        private List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Sets the value of HttpHeaders.Accept
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string Accept
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.Accept, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.AcceptCharset
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string AcceptCharset
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.AcceptCharset, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.CacheControl
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string CacheControl
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.CacheControl, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.ContentType
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string ContentType
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.ContentType, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.ContentLength
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string ContentLength
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.ContentLength, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.TransferEncoding
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string TransferEncoding
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.TransferEncoding, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.ETag
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string ETag
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.ETag, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.IfMatch
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string IfMatch
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.IfMatch, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.IfNoneMatch
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string IfNoneMatch
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.IfNoneMatch, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.DataServiceVersion
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string DataServiceVersion
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.DataServiceVersion, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.MaxDataServiceVersion
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string MaxDataServiceVersion
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.MaxDataServiceVersion, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.Prefer
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string Prefer
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.Prefer, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.PreferenceApplied
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string PreferenceApplied
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.PreferenceApplied, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.HttpMethod
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string HttpMethod
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.HttpMethod, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.DataServiceId
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string DataServiceId
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.DataServiceId, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.Location
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string Location
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.Location, value));
            }
        }

        /// <summary>
        /// Sets the value of HttpHeaders.Slug
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly",
            Justification = "Class is only meant to be used with the initializer pattern")]
        public string Slug
        {
            set
            {
                this.values.Add(new KeyValuePair<string, string>(HttpHeaders.Slug, value));
            }
        }

        /// <summary>
        /// Gets the enumerator of the header values
        /// </summary>
        /// <returns>The enumerator of header values</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of the header values
        /// </summary>
        /// <returns>The enumerator of header values</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }
    }
}