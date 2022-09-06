//---------------------------------------------------------------------
// <copyright file="IndexedKeyValuePairCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Indexable KeyValuePairCollection so that its easier to synchronize selected Value with the UI
    /// </summary>
    [Serializable]
    public class IndexedKeyValuePairCollection : ReadOnlyCollection<KeyValuePair<string, string>>, INotifyPropertyChanged
    {
        private int selectedIndex;

        /// <summary>
        /// Initializes a new instance of the IndexedKeyValuePairCollection class.
        /// </summary>
        /// <param name="list">The set of possible values </param>
        public IndexedKeyValuePairCollection(IList<KeyValuePair<string, string>> list)
            : base(list)
        {
            this.selectedIndex = -1;
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// This event is raised when the value of a property of this type is changed
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Gets or sets the index of the current selected choice in the Possible Values
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.selectedIndex = value;
                this.RaisePropertyChanged("SelectedIndex");
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
