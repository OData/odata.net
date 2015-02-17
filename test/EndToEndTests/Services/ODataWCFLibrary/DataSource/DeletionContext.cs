//---------------------------------------------------------------------
// <copyright file="DeletionContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;

    public class DeletionContext : IDisposable
    {
        Dictionary<object, Action> actions = new Dictionary<object, Action>();

        // Dummy context to make the code easier.
        static DeletionContext originalContext = new DeletionContext();

        static DeletionContext currentContext = null;

        private DeletionContext()
        {
        }

        public static DeletionContext Current
        {
            get
            {
                if (currentContext != null)
                {
                    return currentContext;
                }

                return originalContext;
            }
        }

        public static DeletionContext Create()
        {
            if (currentContext == null)
            {
                currentContext = new DeletionContext();
                return currentContext;
            }
            else
            {
                throw new InvalidOperationException("DelectionContext can be created only one instance at same time.");
            }
        }

        public void ExecuteAction(object target)
        {
            if (currentContext != null && this.actions.ContainsKey(target) && this.actions[target] != null)
            {
                this.actions[target]();
            }
            else
            {
                throw new InvalidOperationException("The delete cannot be apply to this item.");
            }
        }

        public void BindAction(object target, Action action)
        {
            if (target != null)
            {
                this.actions[target] = action;
            }
        }

        public void Dispose()
        {
            actions.Clear();
            currentContext = null;
        }
    }
}