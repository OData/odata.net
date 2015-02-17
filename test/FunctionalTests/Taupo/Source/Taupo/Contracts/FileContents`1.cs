//---------------------------------------------------------------------
// <copyright file="FileContents`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Represents the contents of the file along with a filename.
    /// </summary>
    /// <typeparam name="TContents">The type of the contents.</typeparam>
    public class FileContents<TContents>
    {
        /// <summary>
        /// Initializes a new instance of the FileContents class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="contents">The contents.</param>
        public FileContents(string fileName, TContents contents)
        {
            this.FileName = fileName;
            this.Contents = contents;
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the contents of the file.
        /// </summary>
        /// <value>The contents.</value>
        public TContents Contents { get; set; }
    }
}
