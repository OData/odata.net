//---------------------------------------------------------------------
// <copyright file="ODataValueAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Common
{
    using System;
    using FluentAssertions;
    using FluentAssertions.Primitives;
    using Microsoft.OData;

    [CLSCompliant(false)]
    public static class ODataValueAssertionsExtensions
    {
        public static ODataValueAssertions Should(this ODataValue subject)
        {
            return new ODataValueAssertions(subject);
        }
    }

    [CLSCompliant(false)]
    public class ODataValueAssertions : ObjectAssertions
    {
        protected internal ODataValueAssertions(ODataValue value) 
            : base(value)
        {
        }

        public AndConstraint<ODataValueAssertions> BePrimitive()
        {
            this.Subject.Should().BeAssignableTo<ODataPrimitiveValue>();
            return new AndConstraint<ODataValueAssertions>(this);
        }

        public AndConstraint<ODataValueAssertions> BeCollection()
        {
            this.Subject.Should().BeAssignableTo<ODataCollectionValue>();
            return new AndConstraint<ODataValueAssertions>(this);
        }

        public AndConstraint<ODataValueAssertions> BeODataNullValue()
        {
            this.Subject.Should().BeAssignableTo<ODataNullValue>();
            return new AndConstraint<ODataValueAssertions>(this);
        }

        public AndConstraint<ODataValueAssertions> HavePrimitiveValue(object value)
        {
            this.Subject.Should().BeAssignableTo<ODataPrimitiveValue>();
            this.Subject.As<ODataPrimitiveValue>().Value.Should().Be(value);
            return new AndConstraint<ODataValueAssertions>(this);
        }

        public AndConstraint<ODataValueAssertions> HaveSerializationTypeName(string value)
        {
            var annotation = this.Subject.As<ODataValue>().TypeAnnotation;
            annotation.Should().NotBeNull();
            annotation.TypeName.Should().Be(value);
            return new AndConstraint<ODataValueAssertions>(this);
        }

        public AndConstraint<ODataValueAssertions> NotHaveSerializationTypeName()
        {
            var annotation = this.Subject.As<ODataValue>().TypeAnnotation;
            annotation.Should().BeNull();
            return new AndConstraint<ODataValueAssertions>(this);
        }
    }

    [CLSCompliant(false)]
    public static class ODataItemAssertionsExtensions
    {
        public static ODataItemAssertions Should(this ODataItem subject)
        {
            return new ODataItemAssertions(subject);
        }
    }

    [CLSCompliant(false)]
    public class ODataItemAssertions : ObjectAssertions
    {
        protected internal ODataItemAssertions(ODataItem value)
            : base(value)
        {
        }

        public AndConstraint<ODataItemAssertions> BeResource()
        {
            this.Subject.Should().BeAssignableTo<ODataResource>();
            return new AndConstraint<ODataItemAssertions>(this);
        }

        public AndConstraint<ODataItemAssertions> BeResourceSet()
        {
            this.Subject.Should().BeAssignableTo<ODataResourceSet>();
            return new AndConstraint<ODataItemAssertions>(this);
        }

        public AndConstraint<ODataItemAssertions> HaveSerializationTypeName(string value)
        {
            var annotation = this.Subject.As<ODataItem>().TypeAnnotation;
            annotation.Should().NotBeNull();
            annotation.TypeName.Should().Be(value);
            return new AndConstraint<ODataItemAssertions>(this);
        }
    }
}
