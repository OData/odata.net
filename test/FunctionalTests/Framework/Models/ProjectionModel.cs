//---------------------------------------------------------------------
// <copyright file="ProjectionModel.cs" company="Microsoft">
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
    //  Projection Model
    //
    ///////////////////////////////////////////////////////
    public class ProjectionModel : ExpressionModel
    {
        protected new ResourceContainer _result;

        public new ResourceContainer Result
        {
            get { return _result; }
            set { _result = value; }
        }


        //Constructor
        public ProjectionModel(ExpressionModel parent)
            : base(parent)
        {
            //Operand
            // UnaryExpressionModel model = (UnaryExpressionModel)this.CreateModel();
            // model.Types = this.Types;
            // ModelEngine engine = new ModelEngine(this.Engine, model);
            // engine.Run();

            // NodeType e = model.NodeSelect;
        }

        //Overrides
        public override ExpressionModel CreateModel()
        {
            return new ProjectionModel(this);
        }

    }


}
