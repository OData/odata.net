//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System.Text;
    
    /// <summary>
    /// This class holds extension methods for objects that have new capabilities
    /// in newer versions of .net, and this lets us make the calls look the same and reduces the #if noise
    /// </summary>
    internal static class OrcasExtensions
    {
        /// <summary>
        /// StringBuilder didn't have a clear method in Orcas, so we added and extension method to give it one.
        /// </summary>
        /// <param name="builder">The StringBuilder instance to clear.</param>
        internal static void Clear(this StringBuilder builder)
         {
             builder.Length = 0;
             builder.Capacity = 0;
         }
    }
}
