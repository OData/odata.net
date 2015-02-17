//---------------------------------------------------------------------
// <copyright file="MetadataDeclaredPropertyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// MetadataDeclaredPropertyAnnotation determines if a property should be surfaced via metadata even through it may or may a specific CLR Property backing it
    /// </summary>
    public class MetadataDeclaredPropertyAnnotation : TagAnnotation
    { 
    }
}