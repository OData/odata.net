//---------------------------------------------------------------------
// <copyright file="IDataServiceUpdateProvider2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// This interface declares the methods required to support ServiceActions.
    /// </summary>
    public interface IDataServiceUpdateProvider2 : IDataServiceUpdateProvider
    {
        /// <summary>Queues up the invokable to be invoked during IUpdatable.SaveChanges().</summary>
        /// <param name="invokable">The invokable instance whose Invoke() method will be called during IUpdatable.SaveChanges().</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "Invokable", Justification = "Invokable is the correct spelling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "invokable", Justification = "Invokable is the correct spelling")]
        void ScheduleInvokable(IDataServiceInvokable invokable);
    }
}
