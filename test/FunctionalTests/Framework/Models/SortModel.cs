//---------------------------------------------------------------------
// <copyright file="SortModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using Microsoft.Test.KoKoMo;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    //  Sort Model
    //
    ////////////////////////////////////////////////////////   
    public class SortModel : ExpressionModel
    {
        protected ResourceType _resourceType;
        protected Workspace _workspace;
        protected PropertyExpression[] _sortResult;
      
        //Constructor
        public SortModel(Workspace w, ResourceType resType)
        {
            _resourceType = resType;
            _workspace = w;
        }

        public PropertyExpression[] SortResult
        {
            set { _sortResult = value; }
            get { return _sortResult; }
        }

        public  Workspace Workspace
        {
            get { return _workspace; }
        }

        //Actions
        [ModelAction]
        public void GetSortedExpression()
        {
            List<PropertyExpression> anyProperties = new List<PropertyExpression>();
            int iCount = _resourceType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation && p.PrimaryKey == null && !p.IsComplexType && p.Facets.Sortable).Count();

            int i = AstoriaTestProperties.Random.Next(0, iCount);
            int j = 0;
            foreach (ResourceProperty resourceProperty in _resourceType.Properties)
            {
                if (!resourceProperty.IsNavigation && !resourceProperty.IsComplexType && resourceProperty.Facets.Sortable)
                {
                    anyProperties.Add(new PropertyExpression(resourceProperty));
                    j++;
                }
                if (j > i) { break; }
            }

            this.SortResult = anyProperties.ToArray<PropertyExpression>();

            this.Disabled = true;
        }

        public override ExpressionModel CreateModel()
        {
            return new SortModel(this.Workspace, this._resourceType);
        }       
    }
}
