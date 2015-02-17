//---------------------------------------------------------------------
// <copyright file="ODataPayloadOrderReaderTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Base class for tests cases which need to verify payload order reported by the readers.
    /// </summary>
    public class ODataPayloadOrderReaderTestCase : ODataReaderTestCase
    {
        /// <summary>
        /// Configure Dependencies specific to Reader Test Cases
        /// </summary>
        /// <param name="container">container to set dependencies on</param>
        protected override void ConfigureDependencies(Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            container.Register<ObjectModelToPayloadElementConverter, ObjectModelToPayloadElementWithPayloadOrderConverter>();
            container.RegisterInstance<MessageToObjectModelReader>(new MessageToObjectModelReader(true));
        }
    }
}
