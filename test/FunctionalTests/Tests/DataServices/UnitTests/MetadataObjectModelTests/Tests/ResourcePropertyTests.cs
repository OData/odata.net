//---------------------------------------------------------------------
// <copyright file="ResourcePropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTestNS = System.Data.Test.Astoria;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ResourceProperty class
    /// </summary>
    [TestClass, TestCase]
    public class ResourcePropertyTests
    {
        [TestMethod, Variation("Verifies that resource property doesn't allow certain operations.")]
        public void InvalidCasesTest()
        {
            ResourceType rt = (ResourceType)ResourceTypeUtils.GetTestInstance(typeof(ResourceType));

            ResourceType complexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "Namespace", "Address", false);
            ResourceType entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Namespace", "Order", false);
            ResourceType primitiveOrComplexCollectionType = ResourceType.GetCollectionResourceType(complexType);
            ResourceType entityCollectionType = ResourceType.GetEntityCollectionResourceType(entityType);
            ResourceType namedStreamType = ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream));

            AstoriaTestNS.TestUtil.RunCombinations(
                GetPropertyKindValues(),
                new ResourceType[] { entityType, complexType, primitiveOrComplexCollectionType, entityCollectionType }.Concat(ResourceTypeUtils.GetPrimitiveResourceTypes()),
                (kind, type) =>
                {
                    bool invalidCase = true;

                    if (IsValidValue(kind))
                    {
                        if ((kind.HasFlag(ResourcePropertyKind.Primitive) && type.ResourceTypeKind == ResourceTypeKind.Primitive && type != namedStreamType) ||
                            (kind.HasFlag(ResourcePropertyKind.ComplexType) && type.ResourceTypeKind == ResourceTypeKind.ComplexType) ||
                            (kind.HasFlag(ResourcePropertyKind.ResourceReference) && type.ResourceTypeKind == ResourceTypeKind.EntityType) ||
                            (kind.HasFlag(ResourcePropertyKind.ResourceSetReference) && type.ResourceTypeKind == ResourceTypeKind.EntityType) ||
                            (kind.HasFlag(ResourcePropertyKind.Collection) && type.ResourceTypeKind == ResourceTypeKind.Collection) ||
                            (kind.HasFlag(ResourcePropertyKind.Stream) && type == namedStreamType))
                        {
                            invalidCase = false;
                        }

                        if ((kind & ResourcePropertyKind.Key) == ResourcePropertyKind.Key &&
                            type.InstanceType.IsGenericType && type.InstanceType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            invalidCase = true;
                        }
                    }

                    if (invalidCase)
                    {
                        ExceptionUtils.ThrowsException<ArgumentException>(
                            () => new ResourceProperty("TestProperty", kind, type),
                            string.Format("resource property constructor test case. Kind: '{0}', Type: '{1}: {2}'", kind, type.ResourceTypeKind, type.InstanceType));
                    }
                    else
                    {
                        ResourceProperty p = new ResourceProperty("TestProperty", kind, type);
                        Assert.AreEqual("TestProperty", p.Name, "Name was not stored properly");
                        Assert.AreEqual(kind, p.Kind, "Kind was not stored properly");
                        Assert.AreEqual(type, p.ResourceType, "Type was not stored properly");
                        Assert.AreEqual(p.Kind.HasFlag(ResourcePropertyKind.Stream) ? false : true, p.CanReflectOnInstanceTypeProperty, "CanReflectOnInstanceTypeProperty should be true by default on non-NamedStreams, and false on NamedStreams.");
                        Assert.IsNull(p.MimeType, "MimeType should be null by default.");

                        if (p.Kind.HasFlag(ResourcePropertyKind.Stream))
                        {
                            ExceptionUtils.ThrowsException<InvalidOperationException>(
                                () => { p.CanReflectOnInstanceTypeProperty = false; },
                                "CanReflectOnInstanceTypeProperty should be settable on non-namedstreams, and not settable on namedstreams");
                        }
                        else
                        {
                            p.CanReflectOnInstanceTypeProperty = false;
                        }

                        Assert.AreEqual(false, p.CanReflectOnInstanceTypeProperty, "CanReflectOnInstanceTypeProperty should be false.");

                        bool shouldFail = true;
                        if ((kind & ResourcePropertyKind.Primitive) == ResourcePropertyKind.Primitive)
                        {
                            shouldFail = false;
                        }

                        if (shouldFail)
                        {
                            ExceptionUtils.ThrowsException<InvalidOperationException>(
                                () => p.MimeType = "plain/text",
                                string.Format("Setting MimeType on non-primitive property should fail. Kind: '{0}', Type: '{1}: {2}'", kind, type.ResourceTypeKind, type.InstanceType));
                        }
                        else
                        {
                            p.MimeType = "plain/text";
                        }
                    }
                });
        }

        /// <summary>
        /// Enumerates all interesting combinations of property kind flags.
        /// </summary>
        /// <returns>Enumeration of property kind flags.</returns>
        private IEnumerable<ResourcePropertyKind> GetPropertyKindValues()
        {
            ResourcePropertyKind[] kinds = (ResourcePropertyKind[])Enum.GetValues(typeof(ResourcePropertyKind));

            foreach (ResourcePropertyKind k in kinds)
            {
                yield return k;
            }

            for (int i = 0; i < kinds.Length; i++)
            {
                ResourcePropertyKind k = kinds[i];
                for (int j = i + 1; j < kinds.Length; j++)
                {
                    k |= kinds[j];
                    yield return k;
                }
            }
        }

        /// <summary>
        /// Determines if the specified property kind is valid.
        /// </summary>
        /// <param name="kind">The property kind to inspect.</param>
        /// <returns>true if the property kind is valid; false otherwise.</returns>
        private static bool IsValidValue(ResourcePropertyKind kind)
        {
            if (kind != ResourcePropertyKind.ResourceReference &&
                kind != ResourcePropertyKind.ResourceSetReference &&
                kind != ResourcePropertyKind.ComplexType &&
                kind != ResourcePropertyKind.Primitive &&
                kind != ResourcePropertyKind.Collection &&
                kind != ResourcePropertyKind.Stream &&
                kind != (ResourcePropertyKind.Primitive | ResourcePropertyKind.Key) &&
                kind != (ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag))
            {
                return false;
            }

            return true;
        }
    }
}
