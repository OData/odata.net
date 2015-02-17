//---------------------------------------------------------------------
// <copyright file="FeatureTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using System.Reflection;

namespace System.Data.Test.Astoria
{
    public abstract class AstoriaFeatureTestCase<TWorkspaces> : AstoriaTestCase 
        where TWorkspaces : FeatureWorkspaces, new()
    {
        protected FeatureWorkspaces FeatureWorkspaces
        {
            get
            {
                return FeatureWorkspaces.GetWorkspaces<TWorkspaces>();
            }
        }
    }
}
