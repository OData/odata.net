//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationsDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData;

    public class CustomInstanceAnnotationsDescriptor
    {
        public Type TypeOfAnnotatedItem { get; set; }
        public CustomInstanceAnnotationsDescriptor Parent { get; set; }
        public ICollection<ODataInstanceAnnotation> AnnotationsOnStart { get; set; }
        public ICollection<ODataInstanceAnnotation> AnnotationsOnEnd { get; set; }
        public bool hasNestedResourceInfo { get; set; }
    }
}
