//---------------------------------------------------------------------
// <copyright file="LoggingReporter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Common
{
    using System;
    using System.Security;
    using ApprovalTests.Core;

    /// <summary>
    /// Creates a batch file which can be manually run to update all baselines.
    /// </summary>
    
    public class LoggingReporter : IApprovalFailureReporter
    {
        
        public void Report(string approved, string received)
        {
            Console.WriteLine("BaseLine:\n \"{0}\"\nReceived:\n \"{1}\"\n", approved, received);
        }
    }
}
