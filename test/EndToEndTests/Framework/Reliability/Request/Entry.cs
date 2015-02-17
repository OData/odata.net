//---------------------------------------------------------------------
// <copyright file="Entry.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The entry class
    /// </summary>
    public class Entry : IEnumerable<Property>
    {
        private readonly Dictionary<string, Property> dic = new Dictionary<string, Property>();

        /// <summary>
        /// Initializes a new instance of the Entry class
        /// </summary>
        public Entry()
        {
            this.Links = new List<Link>();
        }

        /// <summary>
        /// Gets links
        /// </summary>
        public List<Link> Links { get; private set; }

        /// <summary>
        /// Gets id
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets properties
        /// </summary>
        public Dictionary<string, Property> Properties
        {
            get { return this.dic; }
        }

        /// <summary>
        /// Gets the i property
        /// </summary>
        /// <param name="i">The index</param>
        /// <returns>The property value</returns>
        public dynamic this[int i]
        {
            get { return this.dic.Values.ElementAt(i).Value; }
        }

        /// <summary>
        /// Gets the property by name
        /// </summary>
        /// <param name="column">Propterty name</param>
        /// <returns>the property value</returns>
        public dynamic this[string column]
        {
            get { return this.dic[column].Value; }
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<Property> GetEnumerator()
        {
            return this.dic.Values.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Add property
        /// </summary>
        /// <param name="entry">The property</param>
        public void Add(Property entry)
        {
            if (entry != null)
            {
                this.dic[entry.Name] = entry;
            }
        }

        /// <summary>
        /// Add properties
        /// </summary>
        /// <param name="entries">The properties</param>
        public void Add(IEnumerable<Property> entries)
        {
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    this.Add(entry);
                }
            }
        }
    }
}
