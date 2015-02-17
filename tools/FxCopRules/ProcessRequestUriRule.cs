//---------------------------------------------------------------------
// <copyright file="ProcessRequestUriRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using System;
    using Microsoft.FxCop.Sdk;
    using System.Collections.Generic;

    /// <summary>
    /// This rule checks that every method that calls ProcessRequestUri also
    /// checks rights.
    /// </summary>
    public class ProcessRequestUriRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="ProcessRequestUriRule"/> instance.</summary>
        public ProcessRequestUriRule()
            : base("ProcessRequestUriRule")
        {
        }

        /// <summary>Visibility of targets to which this rule commonly applies.</summary>
        public override TargetVisibilities TargetVisibility
        {
            get
            {
                return TargetVisibilities.All;
            }
        }

        /// <summary>Checks type members.</summary>
        /// <param name="member">Member being checked.</param>
        /// <returns>A collection of problems found in <paramref name="member"/> or null.</returns>
        public override ProblemCollection Check(Member member)
        {
            Method method = member as Method;
            if (method == null)
            {
                return null;
            }

            methodUnderCheck = method;
            const string RequestUriName = "Microsoft.OData.Service.RequestUriProcessor.ProcessRequestUri(System.String,Microsoft.OData.Service.IDataService)";
            const string CheckResourceRightsName = "Microsoft.OData.Service.DataServiceConfiguration.CheckResourceRights(Microsoft.OData.Service.Providers.ResourceContainer,Microsoft.OData.Service.EntitySetRights)";
            const string CheckResourceRightsForReadName = "Microsoft.OData.Service.DataServiceConfiguration.CheckResourceRightsForRead(Microsoft.OData.Service.Providers.ResourceContainer,System.Boolean)";
            MethodCallFinder finder = new MethodCallFinder(RequestUriName, CheckResourceRightsName, CheckResourceRightsForReadName);
            finder.Visit(method);
            if (finder.Found(RequestUriName) &&
                (!finder.Found(CheckResourceRightsForReadName) &&
                 !finder.Found(CheckResourceRightsName)))
            {
                this.Problems.Add(new Problem(GetResolution(method.FullName)));
            }

            return Problems.Count > 0 ? Problems : null;
        }
    }


}
