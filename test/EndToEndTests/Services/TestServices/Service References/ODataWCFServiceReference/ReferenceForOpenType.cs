//---------------------------------------------------------------------
// <copyright file="ReferenceForOpenType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference
{
    using System;

    public partial class AccountInfo
    {
        public string MiddleName
        {
            get
            {
                return this._MiddleName;
            }
            set
            {
                this.OnMiddleNameChanging(value);
                this._MiddleName = value;
                this.OnMiddleNameChanged();
                this.OnPropertyChanged("MiddleName");
            }
        }
        private string _MiddleName;
        partial void OnMiddleNameChanging(string value);
        partial void OnMiddleNameChanged();

        public Nullable<Color> FavoriteColor
        {
            get
            {
                return this._FavoriteColor;
            }
            set
            {
                this.OnFavoriteColorChanging(value);
                this._FavoriteColor = value;
                this.OnFavoriteColorChanged();
                this.OnPropertyChanged("FavoriteColor");
            }
        }
        private Nullable<Color> _FavoriteColor;
        partial void OnFavoriteColorChanging(Nullable<Color> value);
        partial void OnFavoriteColorChanged();

        public Address Address
        {
            get
            {
                return this._Address;
            }
            set
            {
                this.OnAddressChanging(value);
                this._Address = value;
                this.OnAddressChanged();
                this.OnPropertyChanged("Address");
            }
        }
        private Address _Address;
        partial void OnAddressChanging(Address value);
        partial void OnAddressChanged();
    }
}
