//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core
{
    using System;
    using System.Xml;
    using Microsoft.OData.Core.JsonLight;

    /// <summary>
    /// Represents an instance annotation.
    /// </summary>
    public sealed class ODataInstanceAnnotation : ODataAnnotatable
    {
        /// <summary>
        /// Constructs a new <see cref="ODataInstanceAnnotation"/> instance.
        /// </summary>
        /// <param name="name">The name of the instance annotation.</param>
        /// <param name="value">The value of the instance annotation.</param>
        public ODataInstanceAnnotation(string name, ODataValue value)
        {
            ValidateName(name);
            ValidateValue(value);
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Instance annotation name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Instance annotation value.
        /// </summary>
        public ODataValue Value { get; private set; }

        /// <summary>
        /// Validates that the given <paramref name="name"/> is a valid instance annotation name.
        /// </summary>
        /// <param name="name">Name to validate.</param>
        internal static void ValidateName(string name)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");

            if (name.IndexOf('.') < 0 || name[0] == '.' || name[name.Length - 1] == '.')
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_NeedPeriodInName(name));
            }

            if (ODataAnnotationNames.IsODataAnnotationName(name))
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_ReservedNamesNotAllowed(name, JsonLightConstants.ODataAnnotationNamespacePrefix));
            }

            try
            {
                XmlConvert.VerifyNCName(name);
            }
            catch (XmlException e)
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_BadTermName(name), e);
            }
        }

        /// <summary>
        /// Validates the given <paramref name="value"/> is a valid instance annotation value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        internal static void ValidateValue(ODataValue value)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            if (value is ODataStreamReferenceValue)
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue, "value");
            }
        }
    }
}
