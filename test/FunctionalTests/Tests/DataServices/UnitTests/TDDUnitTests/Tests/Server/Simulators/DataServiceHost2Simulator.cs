//---------------------------------------------------------------------
// <copyright file="DataServiceHost2Simulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server.Simulators
{
    using Microsoft.OData.Service;
    using System.Net;

    internal class DataServiceHost2Simulator : DataServiceHostSimulator, IDataServiceHost2
    {
        internal DataServiceHost2Simulator()
        {
            this.RequestHeaders = new WebHeaderCollection();
            this.ResponseHeaders = new WebHeaderCollection();
        }

        public WebHeaderCollection RequestHeaders
        {
            get; 
            set;
        }

        public WebHeaderCollection ResponseHeaders
        {
            get;
            set;
        }
    }
}
