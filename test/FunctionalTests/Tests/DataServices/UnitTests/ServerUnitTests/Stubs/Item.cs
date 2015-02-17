//---------------------------------------------------------------------
// <copyright file="Item.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;

    #endregion Namespaces

    [ETag("Name", "Description")]
    public class Item : IEquatable<Item>
    {
        List<Item> _relatedItems;
        List<Folder> _relatedFolders;

        public Item()
        {
            ReInit();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Item> RelatedItems
        {
            get { return this._relatedItems; }
        }
        public Item Icon { get; set; }
        public List<Folder> RelatedFolders
        {
            get { return this._relatedFolders; }
        }
        public Folder ParentFolder { get; set; }

        public virtual void ReInit()
        {
            Name = null;
            Description = null;
            _relatedItems = new List<Item>();
            Icon = null;
            _relatedFolders = new List<Folder>();
            ParentFolder = null;
        }

        public bool Equals(Item other)
        {
            return this.ID == other.ID;
        }
    }
}