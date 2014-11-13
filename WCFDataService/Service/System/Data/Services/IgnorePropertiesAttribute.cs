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
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>Attribute to be annotated on types with ETags.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Processed values are available.")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class IgnorePropertiesAttribute : System.Attribute
    {
        /// <summary>Name of the properties that form the ETag.</summary>
        private readonly ReadOnlyCollection<string> propertyNames;

        // This constructor was added since string[] is not a CLS-compliant type and
        // compiler gives a warning as error saying this attribute doesn't have any
        // constructor that takes CLS-compliant type

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.IgnorePropertiesAttribute" /> class. </summary>
        /// <param name="propertyName">A string value that contains the property or properties to be attributed.</param>
        public IgnorePropertiesAttribute(string propertyName)
        {
            WebUtil.CheckArgumentNull(propertyName, "propertyName");
            this.propertyNames = new ReadOnlyCollection<string>(new List<string>(new string[1] { propertyName }));
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.IgnorePropertiesAttribute" /> class. </summary>
        /// <param name="propertyNames">A string value that contains the property or properties to be attributed.</param>
        public IgnorePropertiesAttribute(params string[] propertyNames)
        {
            WebUtil.CheckArgumentNull(propertyNames, "propertyNames");
            if (propertyNames.Length == 0)
            {
                throw new ArgumentException(Strings.ETagAttribute_MustSpecifyAtleastOnePropertyName, "propertyNames");
            }

            this.propertyNames = new ReadOnlyCollection<string>(new List<string>(propertyNames));
        }

        /// <summary>Gets the property name or names to controlled by the <see cref="T:System.Data.Services.IgnorePropertiesAttribute" /> attribute.</summary>
        public ReadOnlyCollection<string> PropertyNames
        {
            get
            {
                return this.propertyNames;
            }
        }

        /// <summary>
        /// Validate and get the list of properties specified by this attribute on the given type.
        /// </summary>
        /// <param name="type">clr type on which this attribute must have defined.</param>
        /// <param name="inherit">whether we need to inherit this attribute or not.
        /// For context types,we need to, since we can have one context dervied from another, and we want to ignore all the properties on the base ones too.
        /// For resource types, we don't need to, since we don't want derived types to know about ignore properties of the base type. Also
        /// from derived type, you cannot change the definition of the base type.</param>
        /// <param name="bindingFlags">binding flags to be used for validating property names.</param>
        /// <returns>list of property names specified on IgnoreProperties on the given type.</returns>
        internal static IEnumerable<string> GetProperties(Type type, bool inherit, BindingFlags bindingFlags)
        {
            IgnorePropertiesAttribute[] attributes = (IgnorePropertiesAttribute[])type.GetCustomAttributes(typeof(IgnorePropertiesAttribute), inherit);
            Debug.Assert(attributes.Length == 0 || attributes.Length == 1, "There should be atmost one IgnoreProperties specified");
            if (attributes.Length == 1)
            {
                foreach (string propertyName in attributes[0].PropertyNames)
                {
                    if (String.IsNullOrEmpty(propertyName))
                    {
                        throw new InvalidOperationException(Strings.IgnorePropertiesAttribute_PropertyNameCannotBeNullOrEmpty);
                    }

                    PropertyInfo property = type.GetProperty(propertyName, bindingFlags);
                    if (property == null)
                    {
                        throw new InvalidOperationException(Strings.IgnorePropertiesAttribute_InvalidPropertyName(propertyName, type.FullName));
                    }
                }

                return attributes[0].PropertyNames;
            }

            return WebUtil.EmptyStringArray;
        }
    }
}
