//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
