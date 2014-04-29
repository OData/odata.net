//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    #endregion

    /// <summary>
    /// Atom metadata description for a person.
    /// </summary>
    public sealed class AtomPersonMetadata : ODataAnnotatable
    {
        /// <summary>The name of the person.</summary>
        private string name;

        /// <summary>The email of the person.</summary>
        private string email;

        /// <summary>The URI value comming from EPM.</summary>
        /// <remarks>In WCF DS when mapping a property through EPM to person/uri element we convert the value of the property to string
        /// and then set the syndication APIs Uri property which is also of type string. Syndication API doesn't do any validation on the value
        /// and just writes it out. So it's risky to try to convert the string to a Uri instance due to the unknown validation the Uri class
        /// might be doing. Instead we use internal property to set from EPM.</remarks>
        private string uriFromEpm;

        /// <summary>Gets or sets the name of the person (required).</summary>
        /// <returns>The name of the person (required).</returns>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                // TODO: validate that the value is not null
                this.name = value;
            }
        }

        /// <summary>Gets or sets an IRI associated with the person.</summary>
        /// <returns>An IRI associated with the person.</returns>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>Gets or sets an email address associated with the person.</summary>
        /// <returns>An email address associated with the person.</returns>
        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                // TODO: validate required format
                // xsd:string { pattern = ".+@.+" }
                // If we add this validation we will have to make an exception for EPM, so either some internal setter
                // or internal property as for Uri.
                this.email = value;
            }
        }

        /// <summary>The URI value comming from EPM.</summary>
        /// <remarks>In WCF DS when mapping a property through EPM to person/uri element we convert the value of the property to string
        /// and then set the syndication APIs Uri property which is also of type string. Syndication API doesn't do any validation on the value
        /// and just writes it out. So it's risky to try to convert the string to a Uri instance due to the unknown validation the Uri class
        /// might be doing. Instead we use internal property to set from EPM.</remarks>
        internal string UriFromEpm
        {
            get
            {
                return this.uriFromEpm;
            }

            set
            {
                this.uriFromEpm = value;
            }
        }

        /// <summary> Converts a string to an <see cref="T:Microsoft.OData.Core.Atom.AtomPersonMetadata" /> instance. </summary>
        /// <returns>The <see cref="T:Microsoft.OData.Core.Atom.AtomPersonMetadata" /> instance created for name.</returns>
        /// <param name="name">The name used in the person metadata.</param>
        public static AtomPersonMetadata ToAtomPersonMetadata(string name)
        {
            return new AtomPersonMetadata
            {
                Name = name
            };
        }

        /// <summary>
        /// Implicit conversion from string to <see cref="AtomPersonMetadata"/>.
        /// </summary>
        /// <param name="name">The <see cref="System.String"/> to convert to an <see cref="AtomPersonMetadata"/>.</param>
        /// <returns>The <see cref="AtomPersonMetadata"/> result.</returns>
        public static implicit operator AtomPersonMetadata(string name)
        {
            return ToAtomPersonMetadata(name);
        }
    }
}
