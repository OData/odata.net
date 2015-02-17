//---------------------------------------------------------------------
// <copyright file="DataTypeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with DataType instances
    /// </summary>
    public static class DataTypeUtils
    {
        /// <summary>
        /// Parses the full type name and returns the local name and namespace.
        /// </summary>
        /// <param name="fullTypeName">The full type name to parse.</param>
        /// <param name="localName">The local name.</param>
        /// <param name="namespaceName">The namespace (can be null).</param>
        public static void ParseFullTypeName(string fullTypeName, out string localName, out string namespaceName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(fullTypeName, "fullTypeName");

            int i = fullTypeName.LastIndexOf('.');
            if (i > 0)
            {
                namespaceName = fullTypeName.Substring(0, i);
                localName = fullTypeName.Substring(i + 1);
            }
            else
            {
                namespaceName = null;
                localName = fullTypeName;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ComplexDataType"/> which has the same properties as
        /// <paramref name="complexDataType"/>, except it references given complex type.
        /// </summary>
        /// <param name="complexDataType">The complex data type instance to start with.</param>
        /// <param name="fullTypeName">The full type name (including namespace).</param>
        /// <returns>
        /// New instance of <see cref="ComplexDataType"/>.
        /// </returns>
        public static ComplexDataType WithFullTypeName(this ComplexDataType complexDataType, string fullTypeName)
        {
            string localName, namespaceName;
            ParseFullTypeName(fullTypeName, out localName, out namespaceName);
            return complexDataType.WithName(namespaceName, localName);
        }
    }
}