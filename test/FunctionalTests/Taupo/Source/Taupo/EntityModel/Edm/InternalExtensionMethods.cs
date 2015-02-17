//---------------------------------------------------------------------
// <copyright file="InternalExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Internal extension methods useful for XElement processing.
    /// </summary>
    internal static class InternalExtensionMethods
    {
        /// <summary>
        /// Gets the required attribute value for a given XElement or throws a readable exception if the attribute is not found.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>Value of the attribute.</returns>
        public static string GetRequiredAttributeValue(this XElement element, XName attributeName)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
            {
                throw new TaupoInvalidOperationException("Attribute '" + attributeName + "' was not specified on <" + element.Name + " />.");
            }

            return attr.Value;
        }

        /// <summary>
        /// Gets the attribute value for a given XElement or a default value if the attribute is not found.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Value of the attribute or the default value.</returns>
        public static string GetOptionalAttributeValue(this XElement element, XName attributeName, string defaultValue)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
            {
                return defaultValue;
            }

            return attr.Value;
        }
    }
}
