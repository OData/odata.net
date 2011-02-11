//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData
{
    #region Namespaces.
    #endregion Namespaces.

    /// <summary>
    /// Generic  utility methods.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Calls IDisposable.Dispose() on the argument if it is not null 
        /// and is an IDisposable.
        /// </summary>
        /// <param name="o">The instance to dispose.</param>
        /// <returns>'True' if IDisposable.Dispose() was called; 'false' otherwise.</returns>
        internal static bool TryDispose(object o)
        {
            DebugUtils.CheckNoExternalCallers();

            IDisposable disposable = o as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                return true;
            }

            return false;
        }
    }
}
