//---------------------------------------------------------------------
// <copyright file="InternetInformationServerMajorVersion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// This enumeration delineates the different versions of IIS 
    /// </summary>
    public enum InternetInformationServerMajorVersion
    {
        /// <summary>
        /// Host a DataService on a IIS webserver running version 5.1 
        /// </summary>
        V51,
       
        /// <summary>
        /// Host a DataService on a IIS webserver running version 6 
        /// </summary>
        V6,
        
        /// <summary>
        /// Host a DataService on a IIS webserver running version 7
        /// </summary>
        V7
    }
}