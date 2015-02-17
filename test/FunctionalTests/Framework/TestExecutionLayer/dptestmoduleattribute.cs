//---------------------------------------------------------------------
// <copyright file="dptestmoduleattribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.ModuleCore
{
    public class DpTestModuleAttribute:TestModuleAttribute
    {
        private string _overview = null;
        private string _strategy = null;
        private string _productVersion = null;
        private string _productMilestone = null;
        private string _areasCovered = null;
        private string _areasNotCovered = null;
        private string _createdBy = null;
        private string _createdDate = null;
        private string[] _specLinks = new string[0];
        private SpecType[] _specTypes = new SpecType[0];
        public DpTestModuleAttribute()
        {
        }

        public string Overview { get { return _overview; } set { _overview = value; } }
        public string Strategy { get { return _strategy; } set { _strategy = value; } }
        public string ProductVersion { get { return _productVersion; } set { _productVersion = value; } }
        public string ProductMilestone { get { return _productMilestone; } set { _productMilestone = value; } }
        public string AreasCovered { get { return _areasCovered; } set { _areasCovered = value; } }
        public string AreasNotCovered { get { return _areasNotCovered; } set { _areasNotCovered = value; } }
        public string CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public string CreatedDate { get { return _createdDate; } set { _createdDate = value; } }
        public string[] SpecLinks { get { return _specLinks; } set { _specLinks = value; } }
        public SpecType[] SpecTypes { get { return _specTypes; } set { _specTypes = value; } }

    }

    public enum SpecType
    {
        Test,Dev,PM,Other
    }
}
