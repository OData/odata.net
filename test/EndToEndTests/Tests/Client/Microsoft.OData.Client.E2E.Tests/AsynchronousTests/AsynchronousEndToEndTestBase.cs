//-----------------------------------------------------------------------------
// <copyright file="AsynchronousEndToEndTestBase.cs" company=".NET Foundation">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.E2E.TestCommon;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;

public class AsynchronousEndToEndTestBase<T> : EndToEndTestBase<T> where T : class
{
    public bool TestCompleted { get; set; }
    protected AsynchronousEndToEndTestBase(TestWebApplicationFactory<T> factory) : base(factory)
    {
        this.TestCompleted = false;
    }
}
