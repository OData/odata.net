//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    using System.Globalization;

    using Microsoft.OData.Edm;

    /// <summary>
    /// OData enum value
    /// </summary>
    public sealed class ODataEnumValue : ODataValue
    {
        /// <summary>
        /// The str value.
        /// </summary>
        private readonly string strValue;

        /// <summary>
        /// The int value.
        /// </summary>
        private readonly long? intValue;

        /// <summary>
        /// Create an odata enum value from string
        /// </summary>
        /// <param name="value">input value</param>
        public ODataEnumValue(string value)
        {
            this.strValue = value;
        }

        /// <summary>
        /// Create an odata enum value from integer
        /// </summary>
        /// <param name="value">input value</param>
        public ODataEnumValue(long value)
        {
            this.intValue = value;
        }
        
        /// <summary>
        /// An enum value
        /// If an enum value is constructed by a reader, then it shall have access to model
        /// then we get a string value
        /// </summary>
        public string Value
        {
            get
            {
                return this.strValue ?? this.intValue.ToString();
            }
        }

        /// <summary>
        /// The type name
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Used by writer
        /// </summary>
        /// <param name="expectedTypeReference">expected enum type reference</param>
        /// <returns>string value of the enum</returns>
        internal string GetValueForSerialization(IEdmTypeReference expectedTypeReference)
        {
            string enumValueString = this.Value;
            if (expectedTypeReference == null)
            {
                return enumValueString;
            }

            IEdmEnumTypeReference enumTypeReference = expectedTypeReference.AsEnum();
            if (enumTypeReference == null)
            {
                return enumValueString;
            }

            // validate and / or parse the enum value, and write it
            IEdmEnumType enumType = enumTypeReference.Definition as IEdmEnumType;
            long enumValue = 0;
            bool success = enumType.TryParseEnum(enumValueString, true, ref enumValue);

            if (success)
            {
                enumValueString = enumTypeReference.ToStringLiteral(enumValue);
            }

            return enumValueString;
        }
    }
}
