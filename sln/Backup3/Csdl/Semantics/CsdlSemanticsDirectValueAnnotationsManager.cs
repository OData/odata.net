//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsDirectValueAnnotationsManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides a CSDL-specific annotations manager.
    /// </summary>
    internal class CsdlSemanticsDirectValueAnnotationsManager : EdmDirectValueAnnotationsManager
    {
        protected override IEnumerable<IEdmDirectValueAnnotation> GetAttachedAnnotations(IEdmElement element)
        {
            CsdlSemanticsElement csdlElement = element as CsdlSemanticsElement;
            if (csdlElement != null)
            {
                return csdlElement.DirectValueAnnotations;
            }

            return Enumerable.Empty<IEdmDirectValueAnnotation>();
        }
    }
}
