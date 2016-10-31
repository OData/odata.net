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

namespace System.Data.Services.Providers
{
    /// <summary>
    /// Build attribute names corresponding to ones in csdl file
    /// </summary>
    internal sealed class EpmAttributeNameBuilder
    {
        /// <summary>Current index</summary>
        private int index;
        
        /// <summary>PostFix for current attribute names</summary>
        private String postFix;

        /// <summary>Constructor</summary>
        internal EpmAttributeNameBuilder()
        {
            this.postFix = String.Empty;
        }

        /// <summary>KeepInContent</summary>
        internal String EpmKeepInContent
        {
            get
            {
                return XmlConstants.MetadataAttributeEpmKeepInContent + this.postFix;
            }
        }

        /// <summary>SourcePath</summary>
        internal String EpmSourcePath
        {
            get
            {
                return XmlConstants.MetadataAttributeEpmSourcePath + this.postFix;
            }
        }

        /// <summary>Target Path</summary>
        internal String EpmTargetPath
        {
            get
            {
                return XmlConstants.MetadataAttributeEpmTargetPath + this.postFix;
            }
        }

        /// <summary>ContentKind</summary>
        internal String EpmContentKind
        {
            get
            {
                return XmlConstants.MetadataAttributeEpmContentKind + this.postFix;
            }
        }

        /// <summary>Namespace Prefix</summary>
        internal String EpmNsPrefix
        {
            get
            {
                return XmlConstants.MetadataAttributeEpmNsPrefix + this.postFix;
            }
        }

        /// <summary>Namespace Uri</summary>
        internal String EpmNsUri
        {
            get
            {
                return XmlConstants.MetadataAttributeEpmNsUri + this.postFix;
            }
        }

        /// <summary>Move to next attribute name generation</summary>
        internal void MoveNext()
        {
            this.index++;
            this.postFix = "_" + this.index.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
