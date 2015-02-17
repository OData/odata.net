//---------------------------------------------------------------------
// <copyright file="ExpressionModel.cs" company="Microsoft">
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
    //  Expression Model
    //
    ////////////////////////////////////////////////////////   
    public abstract class ExpressionModel : Model
    {
        //Data
        protected ExpressionModel _parent = null;
        protected ExpNode _result = null;
        protected NodeTypes _types = null;

        //Constructor
        public ExpressionModel(ExpressionModel parent)
        {
            _parent = parent;
            if (parent != null)
            {
                _types = parent.Types;
            }
        }

        public ExpressionModel()   { }

        //Helpers
        public abstract ExpressionModel CreateModel();

        public virtual NodeTypes Types
        {
            get { return _types; }
            set { _types = value; }
        }


        public virtual ExpNode Result
        {
            get { return _result; }
            set
            {
                _result = value;
                this.Disabled = true;   //Done
            }
        }

        public virtual ExpressionModel Parent
        {
            get { return _parent; }
        }

        //public virtual QueryModel QueryModel
        //{
        //    get
        //    {
        //        if (this is QueryModel)
        //            return (QueryModel)this;
        //        return _parent.QueryModel;  //Recurse
        //    }
        //}

        //public virtual Workspace Workspace
        //{
        //    get { return QueryModel.Workspace; }
        //}

    }

    ////////////////////////////////////////////////////////
    //  Expressions Model
    //
    ////////////////////////////////////////////////////////   
    public abstract class ExpressionsModel : ExpressionModel
    {
        //Data
        protected NodeTypes _results;

        //Constructor
        public ExpressionsModel(QueryModel parent)
            : base(parent)
        {
        }

        //Accessors
        public virtual NodeTypes Results
        {
            get { return _results; }
        }
    }

    ////////////////////////////////////////////////////////
    //  UnaryExpression Model
    //
    ////////////////////////////////////////////////////////   
    public abstract class UnaryExpressionModel : ExpressionModel
    {
        //Data
        protected NodeType _nodeSelect;

        //Constructor
        public UnaryExpressionModel(ExpressionModel parent)
            : base(parent)
        {
        }

        public NodeType NodeSelect
        {
            get { return _nodeSelect;}
            set { _nodeSelect = value; }
        }

        //Actions
        [ModelAction(Weight = 1)]
        [ModelRequirement(Variable = "_types.Count", GreaterThan = 0)]
        public virtual void ChooseInput()
        {
            //Target directly
            //ie: SELECT c FROM c
            //
            //Member directly
            //ie: SELECT p1, p2, p3 FROM c
            //...
            this.NodeSelect = _types.Choose();
            this.Disabled = true;
        }
    }

}
