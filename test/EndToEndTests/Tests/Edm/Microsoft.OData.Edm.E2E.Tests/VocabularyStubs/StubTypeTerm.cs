//---------------------------------------------------------------------
// <copyright file="StubTypeTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.StubEdm;

namespace Microsoft.OData.Edm.E2E.Tests.VocabularyStubs;

public class StubTypeTerm : StubEdmEntityType
{
    public StubTypeTerm(string namespaceName, string name)
        : base(namespaceName, name)
    {
    }
}
