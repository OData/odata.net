// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Restier.Core.Submit;

namespace DataSourceManager.Submit
{
    public class SubmitExecutor : ISubmitExecutor
    {
        public Task<SubmitResult> ExecuteSubmitAsync(SubmitContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SubmitResult(context.ChangeSet));
        }
    }
}