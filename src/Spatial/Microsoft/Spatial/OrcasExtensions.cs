//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
