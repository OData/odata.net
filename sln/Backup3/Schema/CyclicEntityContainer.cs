//---------------------------------------------------------------------
// <copyright file="CyclicEntityContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM entity container that cannot be determined due to a cyclic reference.
    /// </summary>
    internal class CyclicEntityContainer : BadEntityContainer
    {
        public CyclicEntityContainer(string name, EdmLocation location)
            : base(name, new EdmError[] { new EdmError(location, EdmErrorCode.BadCyclicEntityContainer, Edm.Strings.Bad_CyclicEntityContainer(name)) })
        {
        }
    }
}