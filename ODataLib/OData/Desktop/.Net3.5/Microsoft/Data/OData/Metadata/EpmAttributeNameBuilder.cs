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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System.Globalization;
    #endregion Namespaces

    /// <summary>
    /// Builder class for the name of EPM attributes as serialized in CSDL.
    /// This class keeps track of the number of mappings for a given type/property and appends post fixes to the names as needed.
    /// </summary>
    internal sealed class EpmAttributeNameBuilder
    {
        /// <summary>Separator character for building attribute names.</summary>
        private const string Separator = "_";

        /// <summary>Current index.</summary>
        /// <remarks>The first time the name builder is used the names have no suffix. 
        /// The second time (after calling MoveNext once)
        /// the name builder will use suffix '_1', then suffix '_2' and so on.
        /// </remarks>
        private int index;

        /// <summary>Suffix for current attribute names.</summary>
        private string suffix;

        /// <summary>Constructor</summary>
        internal EpmAttributeNameBuilder()
        {
            DebugUtils.CheckNoExternalCallers();
            this.suffix = string.Empty;
        }

        /// <summary>Current keep-in-content attribute name.</summary>
        internal string EpmKeepInContent
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return EpmConstants.ODataEpmKeepInContent + this.suffix;
            }
        }

        /// <summary>Current source path attribute name.</summary>
        internal string EpmSourcePath
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return EpmConstants.ODataEpmSourcePath + this.suffix;
            }
        }

        /// <summary>Current target path attribute name.</summary>
        internal string EpmTargetPath
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return EpmConstants.ODataEpmTargetPath + this.suffix;
            }
        }

        /// <summary>Current content kind attribute name.</summary>
        internal string EpmContentKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return EpmConstants.ODataEpmContentKind + this.suffix;
            }
        }

        /// <summary>Current namespace prefix attribute name.</summary>
        internal string EpmNsPrefix
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return EpmConstants.ODataEpmNsPrefix + this.suffix;
            }
        }

        /// <summary>Current namespace Uri attribute name.</summary>
        internal string EpmNsUri
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return EpmConstants.ODataEpmNsUri + this.suffix;
            }
        }

        /// <summary>Move to next attribute name generation.</summary>
        internal void MoveNext()
        {
            DebugUtils.CheckNoExternalCallers();
            this.index++;
            this.suffix = Separator + this.index.ToString(CultureInfo.InvariantCulture);
        }
    }
}
