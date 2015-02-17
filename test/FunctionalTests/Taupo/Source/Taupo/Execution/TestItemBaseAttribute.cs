//---------------------------------------------------------------------
// <copyright file="TestItemBaseAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Common base class for test item attributes.
    /// </summary>
    public abstract class TestItemBaseAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the TestItemBaseAttribute class with default priority of 2.
        /// </summary>
        protected TestItemBaseAttribute()
        {
            this.Priority = 2;
            this.Timeout = -1;
        }

        /// <summary>
        /// Gets or sets Test Item Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Test Item Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Test Item Owner
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets Test Item Priority (default = 2)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets Test Item Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets Test Item Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the date this test item should be skipped until.
        /// </summary>
        public DateTime? SkipUntilDate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.SkipUntil))
                {
                    return DateTime.Parse(this.SkipUntil, CultureInfo.CurrentCulture);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the reason this test item is skipped. Only used when <see cref="SkipUntil"/> is set.
        /// </summary>
        public string SkipReason { get; set; }

        /// <summary>
        /// Gets or sets the date until which this test item should be skipped. After this date,
        /// the test item will throw an exception.
        /// </summary>
        public string SkipUntil { get; set; }

        /// <summary>
        /// Gets or sets the asynchronous execution timeout (in milliseconds, negative values mean no timeout).
        /// </summary>
        public int Timeout { get; set; }
    }
}
