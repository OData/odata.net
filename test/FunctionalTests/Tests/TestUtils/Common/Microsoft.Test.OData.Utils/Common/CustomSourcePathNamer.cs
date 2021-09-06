//---------------------------------------------------------------------
// <copyright file="CustomSourcePathNamer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Common
{
    using System;
    using ApprovalTests.Core;

    /// <summary>
    /// Implementation of IApprovalNamer that returns a custom SourcePath value.
    /// </summary>
    public class CustomSourcePathNamer : IApprovalNamer
    {
        private readonly string sourcePath;
        private readonly IApprovalNamer internalNamer = ApprovalTests.Approvals.GetDefaultNamer();

        public CustomSourcePathNamer(string sourcePath)
        {
            this.sourcePath = sourcePath;
        }

        public string Name
        {
            get { return this.internalNamer.Name; }
        }

        public string SourcePath
        {
            get
            {
                return this.sourcePath;
            }
        }
    }
}
