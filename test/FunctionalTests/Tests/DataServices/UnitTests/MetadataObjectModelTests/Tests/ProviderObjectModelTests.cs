//---------------------------------------------------------------------
// <copyright file="ProviderObjectModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTestNS = System.Data.Test.Astoria;
    using Microsoft.Test.ModuleCore;
    #endregion Namespaces

    /// <summary>
    /// Tests for the generic things shared by all provider classes
    /// </summary>
    [TestClass, TestCase]
    public class ProviderObjectModelTests
    {
        [TestMethod, Variation( "Validates correct behavior of SetReadOnly.")]
        public void ReadOnlyValidationTest()
        {
            Type[] types = new Type[] {
                    typeof(ResourceSet),
                    typeof(ResourceType),
                    typeof(ResourceProperty),
                    typeof(ServiceOperation),
                    typeof(ServiceOperationParameter) 
                };

            Dictionary<Type, object> values = new Dictionary<Type, object>();
            values.Add(typeof(string), "foo");
            values.Add(typeof(bool), true);

            AstoriaTestNS.TestUtil.RunCombinations(
                types,
                (t) =>
                {
                    object instance = ResourceTypeUtils.GetTestInstance(t);

                    bool isReadOnly = (bool)ReflectionUtils.GetProperty(instance, "IsReadOnly");
                    Assert.IsTrue(!isReadOnly, "If the type is not set to readonly yet, the IsReadOnly property should return false");

                    // Make it readonly
                    ReflectionUtils.InvokeMethod(instance, "SetReadOnly");

                    isReadOnly = (bool)ReflectionUtils.GetProperty(instance, "IsReadOnly");
                    Assert.IsTrue(isReadOnly, "Once the type is set to readonly, the readonly method should return true");

                    // Make sure exception is throw if any of the public setters are called
                    AstoriaTestNS.TestUtil.RunCombinations(
                        t.GetProperties(BindingFlags.Instance | BindingFlags.Public),
                        (propertyInfo) =>
                        {
                            //Ignoring custom state, since its not governed by part of readonly
                            if (propertyInfo.Name == "CustomState")
                            {
                                return;
                            }

                            MethodInfo setter = propertyInfo.GetSetMethod();
                            if (setter != null)
                            {
                                try
                                {
                                    setter.Invoke(instance, new object[] { values[propertyInfo.PropertyType] });
                                    Assert.Fail(String.Format("Public properties cannot be modified after the type is set to ReadOnly. Property '{0}' did not throw", propertyInfo.Name));
                                }
                                catch (TargetInvocationException e)
                                {
                                    Assert.IsTrue(e.InnerException is InvalidOperationException, "Expecting invalid operation exception");
                                    Assert.IsTrue(e.InnerException.Message.Contains("cannot be modified since it is already set to read-only."), String.Format("Expecting a message with read-only in it. Actual: '{0}'", e.InnerException.Message));
                                }
                            }

                            // call the getter twice and make sure they return the same instance for reference types i.e. caching takes place.
                            MethodInfo getter = propertyInfo.GetGetMethod();
                            if (getter != null && !propertyInfo.PropertyType.IsValueType)
                            {
                                object value1 = getter.Invoke(instance, null);
                                object value2 = getter.Invoke(instance, null);
                                Assert.IsTrue(Object.ReferenceEquals(value1, value2), String.Format("For Readonly types, the reference type properties should return the same instance. Property: {0}", propertyInfo.Name));
                            }
                        });

                    // make sure the custom state property can be set even after the type is set to readonly
                    string state = "foo";
                    ReflectionUtils.SetProperty(instance, "CustomState", state);
                    Assert.AreSame(state, ReflectionUtils.GetProperty(instance, "CustomState"), "custom state should be modified");

                    // make sure calling SetReadOnly again is a no-op
                    t.GetMethod("SetReadOnly", BindingFlags.Public | BindingFlags.Instance).Invoke(instance, null);
                });
        }

        [TestMethod, Variation( "Verify that IList values are not cached when non-read only.")]
        public void NonReadOnlyValidation()
        {
            Type[] types = new Type[] {
                    // cannot add service operation, since the parameters collection is passed in the constructor and is always cached.
                    typeof(ResourceSet),
                    typeof(ResourceType),
                    typeof(ResourceProperty),
                    typeof(ServiceOperationParameter) 
            };

            AstoriaTestNS.TestUtil.RunCombinations(
                types,
                (t) =>
                {
                    object instance = ResourceTypeUtils.GetTestInstance(t);

                    // Make sure exception is throw if any of the public setters are called
                    AstoriaTestNS.TestUtil.RunCombinations(
                        t.GetProperties(BindingFlags.Instance | BindingFlags.Public),
                        (propertyInfo) =>
                        {
                            //Ignoring custom state, since its not governed by part of readonly
                            if (propertyInfo.Name == "CustomState")
                            {
                                return;
                            }

                            // call the getter twice and make sure they return the same instance for reference types i.e. caching takes place.
                            MethodInfo getter = propertyInfo.GetGetMethod();
                            if (getter != null && typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                            {
                                object value1 = getter.Invoke(instance, null);
                                object value2 = getter.Invoke(instance, null);
                                Assert.IsTrue(!Object.ReferenceEquals(value1, value2), String.Format("For non-readonly types, the collection properties should not return the same instance. Property: {0}", propertyInfo.Name));
                            }
                        });
                });
        }
    }
}
