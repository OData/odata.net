//---------------------------------------------------------------------
// <copyright file="PropertyAliasHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;

namespace Microsoft.OData.Edm
{
    internal static class PropertyAliasHelpers
    {
        internal static IEdmStructuralProperty GetKeyProperty(IEdmStructuralProperty property, string[] keyPropertySegments, string propertyAlias)
        {
            IEdmStructuralProperty structuralProperty = property;
            for (int i = 1; i < keyPropertySegments.Length; i++)
            {
                if (structuralProperty.Type.Definition.TypeKind == EdmTypeKind.Complex)
                {
                    structuralProperty = structuralProperty.Type.AsComplex().FindProperty(keyPropertySegments[i]) as IEdmStructuralProperty;
                }
                else if (structuralProperty.Type.Definition.TypeKind == EdmTypeKind.Entity)
                {
                    structuralProperty = structuralProperty.Type.AsEntity().FindProperty(keyPropertySegments[i]) as IEdmStructuralProperty;
                }
                else
                {
                    throw new InvalidOperationException(Strings.Bad_UnresolvedType(structuralProperty));
                }
            }

            if (structuralProperty != null)
            {
                structuralProperty = new EdmStructuralPropertyAlias(structuralProperty.DeclaringType, structuralProperty.Name, structuralProperty.Type, propertyAlias, keyPropertySegments);
            }

            return structuralProperty;

        }
    }
}
