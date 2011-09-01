//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM documentation.
    /// </summary>
    public class EdmDocumentation : IEdmDocumentation
    {
        /// <summary>
        /// Initializes a new instance of the EdmDocumentation class.
        /// </summary>
        /// <param name="summary">Summary of the documentation.</param>
        /// <param name="longDescription">A long description of the documentation.</param>
        public EdmDocumentation(string summary, string longDescription)
        {
            this.Summary = summary;
            this.LongDescription = longDescription;
        }

        /// <summary>
        /// Initializes a new instance of the EdmDocumentation class.
        /// </summary>
        public EdmDocumentation()
        {
        }

        /// <summary>
        /// Gets or sets a summary of this documentation.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets a long description of this documentation.
        /// </summary>
        public string LongDescription { get; set; }
    }
}
