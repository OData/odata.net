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

namespace Microsoft.Data.OData
{
    using System;
    using System.Xml;
    using Microsoft.Data.OData.JsonLight;

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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            if (value is ODataStreamReferenceValue)
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue, "value");
            }
        }
    }
}
