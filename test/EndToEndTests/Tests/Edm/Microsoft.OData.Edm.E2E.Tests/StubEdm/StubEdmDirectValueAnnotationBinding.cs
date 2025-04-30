//---------------------------------------------------------------------
// <copyright file="StubEdmDirectValueAnnotationBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.StubEdm;

/// <summary>
/// Stub implementation of EdmDirectValueAnnotationBinding.
/// </summary>
public class StubEdmDirectValueAnnotationBinding : IEdmDirectValueAnnotationBinding
{
    /// <summary>
    /// Gets or sets the element to which the annotation is bound.
    /// </summary>
    public IEdmElement Element { get; set; }

    /// <summary>
    /// Gets or sets the namespace of the annotation.
    /// </summary>
    public string NamespaceUri { get; set; }

    /// <summary>
    /// Gets or sets the local name of the annotation.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value of the annotation.
    /// </summary>
    public object Value { get; set; }
}
