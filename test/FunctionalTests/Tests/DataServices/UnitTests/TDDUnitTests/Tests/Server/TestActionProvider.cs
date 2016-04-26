//---------------------------------------------------------------------
// <copyright file="TestActionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData;

    internal delegate bool ServiceResolutionDelegateWithString(DataServiceOperationContext operationContext, string actionName, out ServiceAction serviceAction);

    internal class TestActionProvider : IDataServiceActionProvider
    {
        internal Func<DataServiceOperationContext, IEnumerable<ServiceAction>> GetServiceActionsCallback { get; set; }

        internal ServiceResolutionDelegateWithString TryResolveServiceActionCallback { get; set; }

        internal Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>> GetByBindingTypeCallback { get; set; }

        public IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext)
        {
            if (this.GetServiceActionsCallback != null)
            {
                return this.GetServiceActionsCallback(operationContext);
            }

            throw new NotImplementedException();
        }

        public bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction)
        {
            if (this.TryResolveServiceActionCallback != null)
            {
                return this.TryResolveServiceActionCallback(operationContext, serviceActionName, out serviceAction);
            }

            throw new NotImplementedException();
        }

        public IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType)
        {
            if (this.GetByBindingTypeCallback != null)
            {
                return this.GetByBindingTypeCallback(operationContext, bindingParameterType);
            }

            throw new NotImplementedException();
        }

        public IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
        {
            throw new NotImplementedException();
        }

        public bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
        {
            throw new NotImplementedException();
        }
    }
}