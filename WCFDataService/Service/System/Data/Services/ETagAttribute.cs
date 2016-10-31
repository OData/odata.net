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

namespace System.Data.Services
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

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.ETagAttribute" /> class. </summary>
        /// <param name="propertyName">The string value containing properties used in eTag value.</param>
        public ETagAttribute(string propertyName)
        {
            WebUtil.CheckArgumentNull(propertyName, "propertyName");
            this.propertyNames = new ReadOnlyCollection<string>(new List<string>(new string[1] { propertyName }));
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.ETagAttribute" /> class.</summary>
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

        /// <summary>Gets the names of properties used in the <see cref="T:System.Data.Services.ETagAttribute" />.</summary>
        /// <returns>String collection containing property names.</returns>
        public ReadOnlyCollection<string> PropertyNames
        {
            get { return this.propertyNames; }
        }
    }
}
