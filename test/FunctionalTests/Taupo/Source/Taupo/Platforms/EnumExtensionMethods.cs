//---------------------------------------------------------------------
// <copyright file="EnumExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Platforms
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extenstion methods to fake platform support for missing System.Enum APIs
    /// </summary>
    public static class EnumExtensionMethods
    {
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. 
        /// </summary>
        /// <typeparam name="TEnumType">The type of the enum</typeparam>
        /// <param name="value">The string representation of the enumeration name or underlying value to convert</param>
        /// <param name="ignoreCase">True to ignore case; false to consider case</param>
        /// <param name="parsedValue">When this method returns, contains an object of type TEnum whose value is represented by value. </param>
        /// <returns>Equivalent enumerated object</returns>
        public static bool TryParse<TEnumType>(string value, bool ignoreCase, out TEnumType parsedValue) where TEnumType : struct
        {
            return Enum.TryParse<TEnumType>(value, ignoreCase, out parsedValue);
        }

        /// <summary>
        /// Reflection based implementation for the Enum.GetValues Method
        /// </summary>
        /// <typeparam name="TEnumType">This is the Enum. Type will be inferred from this</typeparam>
        /// <returns>Constant Values for the Enum</returns>
        public static Array GetValues<TEnumType>() where TEnumType : struct
        {
            Type enumerationType = typeof(TEnumType);
            return GetValues(enumerationType);
        }

        /// <summary>
        /// Reflection based implementation for the Enum.GetValues Method
        /// </summary>
        /// <param name="enumerationType">This is the Enumeration Type</param>
        /// <returns>Constant Values for the Enum</returns>
        public static Array GetValues(Type enumerationType)
        {
            return Enum.GetValues(enumerationType);
        }
    }
}
