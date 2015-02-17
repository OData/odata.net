//---------------------------------------------------------------------
// <copyright file="Folder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Collections.Generic;

    public class Folder : IEquatable<Folder>
    {
        List<Item> _items;
        List<Folder> _folders;

        public Folder()
        {
            ReInit();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Item Icon { get; set; }
        public List<Item> Items
        {
            get { return _items; }
        }

        public List<Folder> Folders
        {
            get { return _folders; }
        }

        public void ReInit()
        {
            Name = null;
            Icon = null;
            _items = new List<Item>();
            _folders = new List<Folder>();
        }

        #region IEquatable<Folder> Members

        public bool Equals(Folder other)
        {
            return this.ID == other.ID;
        }

        #endregion
    }
}
