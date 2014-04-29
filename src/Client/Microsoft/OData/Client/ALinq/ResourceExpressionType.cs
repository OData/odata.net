//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    /// <summary>Enum for resource expression types</summary>
    internal enum ResourceExpressionType
    {
        /// <summary>ResourceSet Expression</summary>
        RootResourceSet = 10000,

        /// <summary>Single resource expression, used to represent singleton.</summary>
        RootSingleResource,

        /// <summary>Resource Navigation Expression</summary>
        ResourceNavigationProperty,

        /// <summary>Resource Navigation Expression to Singleton</summary>
        ResourceNavigationPropertySingleton,

        /// <summary>Take Query Option Expression</summary>
        TakeQueryOption,

        /// <summary>Skip Query Option Expression</summary>
        SkipQueryOption,

        /// <summary>OrderBy Query Option Expression</summary>
        OrderByQueryOption,

        /// <summary>Filter Query Option Expression</summary>
        FilterQueryOption,

        /// <summary>Reference to a bound component of the resource set path</summary>
        InputReference,

        /// <summary>Projection Query Option Expression</summary>
        ProjectionQueryOption,

        /// <summary>Expand Query Option Expression</summary>
        ExpandQueryOption,
    }
}
