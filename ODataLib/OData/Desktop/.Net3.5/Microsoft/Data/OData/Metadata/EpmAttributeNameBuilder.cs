//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
