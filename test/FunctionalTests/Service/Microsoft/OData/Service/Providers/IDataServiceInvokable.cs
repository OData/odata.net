//---------------------------------------------------------------------
// <copyright file="IDataServiceInvokable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// This interface declares the methods required to support invoking of an operation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "Invokable", Justification = "Invokable is the correct spelling")]
    public interface IDataServiceInvokable
    {
        /// <summary> Invokes the underlying operation. </summary>
        void Invoke();

        /// <summary> Gets the result of the call to Invoke. </summary>
        /// <returns>The result of the call to Invoke.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is more appropriate here.")]
        object GetResult();
    }
}
