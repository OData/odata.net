//---------------------------------------------------------------------
// <copyright file="DoNotHandleProhibitedExceptionsRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.FxCop.Sdk;
using System.Linq;

namespace DataWebRules
{
    class DoNotHandleProhibitedExceptionsRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="DoNotHandleProhibitedExceptionsRule"/> instance.</summary>
        public DoNotHandleProhibitedExceptionsRule()
            : base("DoNotHandleProhibitedExceptionsRule")
        {
        }

        public override Microsoft.FxCop.Sdk.ProblemCollection Check(Microsoft.FxCop.Sdk.Member member)
        {
            Method method = member as Method;
            if (method != null)
            {
                methodUnderCheck = method;
                Visit(method.Body);
            }
            base.Check(member);
            return Problems;
        }

        public override void VisitCatch(Microsoft.FxCop.Sdk.CatchNode catchNode)
        {
            if (catchNode.Type == FrameworkTypes.Exception)
            {
                string[] doNotHandleMethods = new string[] { 
                    "Microsoft.OData.Service.CommonUtil.IsCatchableExceptionType(System.Exception)", 
                    "Microsoft.OData.Client.CommonUtil.IsCatchableExceptionType(System.Exception)",
                    "Microsoft.OData.Core.ExceptionUtils.IsCatchableExceptionType(System.Exception)",
                    "Microsoft.Spatial.Util.IsCatchableExceptionType(System.Exception)",
                };
                MethodCallFinder finder = new MethodCallFinder(true, doNotHandleMethods);
                finder.Visit(catchNode.Block);
                if(!doNotHandleMethods.Any(m => finder.Found(m)))
                {
                    Problems.Add(new Problem(GetResolution(methodUnderCheck.FullName), catchNode));
                }
            }
            base.VisitCatch(catchNode);
        }
    }
}
