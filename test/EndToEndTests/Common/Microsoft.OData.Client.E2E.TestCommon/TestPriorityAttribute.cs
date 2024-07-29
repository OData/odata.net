//-----------------------------------------------------------------------------
// <copyright file="TestPriorityAttribute.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.TestCommon
{
    /// <summary>
    /// Specifies the priority of a test method.
    /// </summary>
    public class TestPriorityAttribute : Attribute
    {
        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }
        public int Priority { get; private set; }
    }
}
