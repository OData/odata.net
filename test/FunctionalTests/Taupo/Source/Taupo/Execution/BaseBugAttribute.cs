//---------------------------------------------------------------------
// <copyright file="BaseBugAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents a bug attribute that targets a bug in one of the databases.
    /// </summary>
    [Serializable]
    public abstract class BaseBugAttribute : BugAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBugAttribute"/> class.
        /// </summary>
        /// <param name="server">The server that holds the bug.</param>
        /// <param name="product">The product or project on the <paramref name="server"/> where the bug can be found.</param>
        /// <param name="bugId">The ID of the bug.</param>
        protected BaseBugAttribute(string server, string product, int bugId) :
            base(server, product, bugId)
        {
        }

        /// <summary>
        /// Gets a link to the bug in the database.
        /// </summary>
        public string Link
        {
            get { return this.Server + "WorkItemTracking/WorkItem.aspx?artifactMoniker=" + this.BugId; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "This variation may have failed due to {0} bug #{1} ({2}).{3}",
                this.Product,
                this.BugId,
                this.Link,
                this.Description == null ? null : " Description: " + this.Description);
        }
    }
}
