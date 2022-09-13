//---------------------------------------------------------------------
// <copyright file="AsynchronousAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;

    /// <summary>
    /// Mark the test method as one which expects asynchronous execution.
    /// Shim to match Silverlight Unit Test Framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AsynchronousAttribute : Attribute
    {
    }
}
