//---------------------------------------------------------------------
// <copyright file="DynamicProxyMethodGeneratorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace AstoriaUnitTests.Tests.Client
{
    public class DynamicProxyMethodGeneratorTests
    {
        private class DynamicProxyMethodGeneratorSimulator : DynamicProxyMethodGenerator
        {
            public DynamicProxyMethodGeneratorSimulator()
            {
                this.HasPermissionsToCreateDynamicMethodsWithSkipVisibility = true;
            }

            public bool HasPermissionsToCreateDynamicMethodsWithSkipVisibility { get; set; }

#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            protected bool ThisAssemblyCanCreateHostedDynamicMethodsWithSkipVisibility()
#else
            protected override bool ThisAssemblyCanCreateHostedDynamicMethodsWithSkipVisibility()
#endif
            {
                return this.HasPermissionsToCreateDynamicMethodsWithSkipVisibility;
            }
        }

        [Fact]
        public void Sut_creates_Expression_method_call_bound_to_arguments()
        {
            // Arrange
            MethodInfo target;
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            target = ((Func<int, int, int>)Sum).GetMethodInfo();
#else
            target = ((Func<int, int, int>)Sum).Method;
#endif

            const int firstAddend = 5;
            const int secondAddend = 17;
            var expectedSum = firstAddend + secondAddend;
            var first = Expression.Constant(firstAddend);
            var second = Expression.Constant(secondAddend);

            var sut = new DynamicProxyMethodGeneratorSimulator();

            // Act
            var expr = sut.GetCallWrapper(target, first, second);

            // Assert
            var mce = expr as MethodCallExpression;

            Assert.NotNull(mce);

            Assert.Equal(2, mce.Arguments.Count);
            Assert.Equal(first, mce.Arguments[0]);
            Assert.Same(second, mce.Arguments[1]);

            var sum = Expression.Lambda<Func<int>>(mce).Compile();
            var result = sum();

            Assert.Equal(expectedSum, result);
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [Fact]
        public void When_client_can_create_DynamicMethod_with_skip_visibility_GetCallWrapper_returns_Expression_which_calls_DynamicMethod()
        {
            // Arrange
            MethodInfo target = typeof(DataServiceContext).GetMethod("GetMetadataUri", BindingFlags.Public | BindingFlags.Instance);
            var sut = new DynamicProxyMethodGeneratorSimulator
                      {
                          HasPermissionsToCreateDynamicMethodsWithSkipVisibility = true
                      };

            // Act
            var expr = sut.GetCallWrapper(target);

            // Assert
            var mce = expr as MethodCallExpression;

            //Expected return value to be a MethodCallExpression.
            Assert.NotNull(mce);
            Assert.IsType<DynamicMethod>(mce.Method);
        }
#endif

        [Fact]
        public void When_client_cannot_create_DynamicMethod_with_skip_visibility_GetCallWrapper_returns_Expression_which_calls_method_directly()
        {
            // Arrange
            MethodInfo target;
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            target = ((Action)Simple).GetMethodInfo();
#else
            target = ((Action)Simple).Method;
#endif
            var sut = new DynamicProxyMethodGeneratorSimulator
            {
                HasPermissionsToCreateDynamicMethodsWithSkipVisibility = false
            };

            // Act
            var expr = sut.GetCallWrapper(target);

            // Assert
            var mce = expr as MethodCallExpression;

            //Expected return value to be a MethodCallExpression.
            Assert.NotNull(mce);
            Assert.Equal(target, mce.Method);
        }

        [Fact]
        public void When_client_cannot_create_DynamicMethod_with_skip_visibility_GetCallWrapper_returns_Expression_which_calls_constructor_directly()
        {
            // Arrange
            var target = typeof(DynamicProxyMethodGeneratorTests).GetConstructor(Type.EmptyTypes);
            var sut = new DynamicProxyMethodGeneratorSimulator
            {
                HasPermissionsToCreateDynamicMethodsWithSkipVisibility = false
            };

            // Act
            var expr = sut.GetCallWrapper(target);

            // Assert
            var nex = expr as NewExpression;

            //Expected return value to be a NewExpression.
            Assert.NotNull(nex);
            Assert.Equal(target, nex.Constructor);
        }

        [Fact]
        public void When_client_can_create_DynamicMethods_with_skip_visibility_GetCallWrapper_returns_cached_DynamicMethod_on_multiple_calls()
        {
            // Arrange
            MethodInfo target;
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            target = ((Action)Simple).GetMethodInfo();
#else
            target = ((Action)Simple).Method;
#endif
            var sut = new DynamicProxyMethodGeneratorSimulator
            {
                HasPermissionsToCreateDynamicMethodsWithSkipVisibility = true
            };
            var sut2 = new DynamicProxyMethodGeneratorSimulator
            {
                HasPermissionsToCreateDynamicMethodsWithSkipVisibility = true
            };

            // Act
            var expr = (MethodCallExpression)sut.GetCallWrapper(target);
            var expr2 = (MethodCallExpression)sut2.GetCallWrapper(target);

            // Assert
            Assert.Same(expr.Method, expr2.Method);
        }

        private static void Simple()
        {
        }

        private static int Sum(int first, int second)
        {
            return first + second;
        }
    }
}
