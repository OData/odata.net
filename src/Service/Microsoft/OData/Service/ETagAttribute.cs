//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>Attribute to be annotated on types with ETags.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Processed value is available")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ETagAttribute : System.Attribute
    {
        /// <summary>Name of the properties that form the ETag.</summary>
        private readonly ReadOnlyCollection<string> propertyNames;

        // This constructor was added since string[] is not a CLS-compliant type and
        // compiler gives a warning as error saying this attribute doesn't have any
        // constructor that takes CLS-compliant type

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.ETagAttribute" /> class. </summary>
        /// <param name="propertyName">The string value containing properties used in eTag value.</param>
        public ETagAttribute(string propertyName)
        {
            WebUtil.CheckArgumentNull(propertyName, "propertyName");
            this.propertyNames = new ReadOnlyCollection<string>(new List<string>(new string[1] { propertyName }));
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.ETagAttribute" /> class.</summary>
        /// <param name="propertyNames">String values containing properties used in eTag value.</param>
        public ETagAttribute(params string[] propertyNames)
        {
            WebUtil.CheckArgumentNull(propertyNames, "propertyNames");
            if (propertyNames.Length == 0)
            {
                throw new ArgumentException(Strings.ETagAttribute_MustSpecifyAtleastOnePropertyName, "propertyNames");
            }

            this.propertyNames = new ReadOnlyCollection<string>(new List<string>(propertyNames));
        }

        /// <summary>Gets the names of properties used in the <see cref="T:Microsoft.OData.Service.ETagAttribute" />.</summary>
        /// <returns>String collection containing property names.</returns>
        public ReadOnlyCollection<string> PropertyNames
        {
            get { return this.propertyNames; }
        }
    }
}
