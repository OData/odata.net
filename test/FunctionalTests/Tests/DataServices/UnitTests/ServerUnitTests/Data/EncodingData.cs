//---------------------------------------------------------------------
// <copyright file="EncodingData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>Provides information about interesting encoding values.</summary>
    public sealed class EncodingData
    {
        /// <summary>The .NET version of the specified encoding.</summary>
        private Encoding encoding;

        /// <summary>The string name of the specified encoding.</summary>
        private string name;

        private static EncodingData[] values;

        /// <summary>Hideden constructor.</summary>
        private EncodingData() { }

        /// <summary>Initializes a new dummy EncodingData instance.</summary>
        /// <param name="name">Encoding name.</param>
        /// <returns>A new EncodingData instance.</returns>
        private static EncodingData ForDummy(string name)
        {
            EncodingData result = new EncodingData();
            result.name = name;
            return result;
        }

        /// <summary>
        /// Initializes a new EncodingData instance for the specified encoding. 
        /// </summary>
        /// <param name="encoding">Encoding to encapsulate.</param>
        /// <returns>A new EncodingData instance.</returns>
        private static EncodingData ForEncoding(Encoding encoding)
        {
            Debug.Assert(encoding != null, "encoding != null");

            EncodingData result = new EncodingData();
            result.encoding = encoding;
            result.name = encoding.WebName;
            return result;
        }

        public static EncodingData FindWebName(string name)
        {
            foreach(EncodingData result in Values)
            {
                if (result.encoding.WebName == name)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>Interesting values for testing encodings.</summary>
        public static EncodingData[] Values
        {
            get
            {
                if (values == null)
                {
                    var systemEncodings = Encoding.GetEncodings();
                    var encodings = new List<EncodingData>();
                    foreach (var encodingInfo in systemEncodings)
                    {
                        encodings.Add(ForEncoding(encodingInfo.GetEncoding()));
                    }
                    encodings.Add(ForDummy("fake-encoding-name"));
                    values = encodings.ToArray();
                }
                return values;
            }
        }

        /// <summary>The encoding encapsulated, possibly null.</summary>
        public Encoding Encoding
        {
            get { return this.encoding; }
        }

        /// <summary>The name for the encoding.</summary>
        public string Name
        {
            get { return this.name; }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
