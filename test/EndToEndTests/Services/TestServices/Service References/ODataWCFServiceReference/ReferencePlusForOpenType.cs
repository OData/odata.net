//---------------------------------------------------------------------
// <copyright file="ReferencePlusForOpenType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus
{
    public partial class AccountInfoPlus
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("MiddleName")]
        public string MiddleNamePlus
        {
            get
            {
                return this._MiddleNamePlus;
            }
            set
            {
                this.OnMiddleNamePlusChanging(value);
                this._MiddleNamePlus = value;
                this.OnMiddleNamePlusChanged();
                this.OnPropertyChanged("MiddleName");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        private string _MiddleNamePlus = "DefaultMiddleName";
        partial void OnMiddleNamePlusChanging(string value);
        partial void OnMiddleNamePlusChanged();

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("IsActive")]
        public bool IsActivePlus
        {
            get
            {
                return this._IsActivePlus;
            }
            set
            {
                this.OnIsActivePlusChanging(value);
                this._IsActivePlus = value;
                this.OnIsActivePlusChanged();
                this.OnPropertyChanged("IsActive");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        private bool _IsActivePlus;
        partial void OnIsActivePlusChanging(bool value);
        partial void OnIsActivePlusChanged();
    }

    public partial class CompanyPlus
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("FullName")]
        public string FullNamePlus
        {
            get
            {
                return this._FullNamePlus;
            }
            set
            {
                this.OnFullNamePlusChanging(value);
                this._FullNamePlus = value;
                this.OnFullNamePlusChanged();
                this.OnPropertyChanged("FullName");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        private string _FullNamePlus;
        partial void OnFullNamePlusChanging(string value);
        partial void OnFullNamePlusChanged();

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("TotalAssets")]
        public long TotalAssetsPlus
        {
            get
            {
                return this._TotalAssetsPlus;
            }
            set
            {
                this.OnTotalAssetsPlusChanging(value);
                this._TotalAssetsPlus = value;
                this.OnTotalAssetsPlusChanged();
                this.OnPropertyChanged("TotalAssets");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "1.0.0")]
        private long _TotalAssetsPlus;
        partial void OnTotalAssetsPlusChanging(long value);
        partial void OnTotalAssetsPlusChanged();
    }
}