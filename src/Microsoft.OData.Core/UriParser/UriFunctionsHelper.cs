//---------------------------------------------------------------------
// <copyright file="UriFunctionsHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region NameSpaces
    
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;

    #endregion

    internal static class UriFunctionsHelper
    {
        /// <summary>Builds a description of a list of function signatures.</summary>
        /// <param name="name">Function name.</param>
        /// <param name="signatures">Function signatures.</param>
        /// <returns>A string with ';'-separated list of function signatures.</returns>
        public static string BuildFunctionSignatureListDescription(string name, IEnumerable<FunctionSignatureWithReturnType> signatures)
        {
            Debug.Assert(name != null, "name != null");
            Debug.Assert(signatures != null, "signatures != null");

            StringBuilder builder = new StringBuilder();
            string descriptionSeparator = "";
            foreach (FunctionSignatureWithReturnType signature in signatures)
            {
                builder.Append(descriptionSeparator);
                descriptionSeparator = "; ";

                string parameterSeparator = "";
                builder.Append(name);
                builder.Append('(');
                foreach (IEdmTypeReference type in signature.ArgumentTypes)
                {
                    builder.Append(parameterSeparator);
                    parameterSeparator = ", ";

                    if (type.IsODataPrimitiveTypeKind() && type.IsNullable)
                    {
                        builder.Append(type.FullName());
                        builder.Append(" Nullable=true");
                    }
                    else
                    {
                        builder.Append(type.FullName());
                    }
                }

                builder.Append(')');
            }

            return builder.ToString();
        }
    }
}
