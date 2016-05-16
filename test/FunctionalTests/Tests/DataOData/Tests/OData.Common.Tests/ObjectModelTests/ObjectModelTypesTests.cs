//---------------------------------------------------------------------
// <copyright file="ObjectModelTypesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// Tests for the public OM types properties.
    /// </summary>
    [TestClass, TestCase]
    public class ObjectModelTypesTests : ODataTestCase
    {
        // List of types explicitly allowed to be not-sealed.
        // Make sure that it's the right thing if you're adding a type to this list.
        private static Type[] UnsealedPublicTypes = new Type[]
        {
            typeof(ODataException),
            typeof(ODataContentTypeException),
            typeof(ODataMediaTypeResolver),
            typeof(ODataPayloadValueConverter),
            typeof(ODataUntypedValue),
        };

        // List of types explicitly allowed to be not-sealed.
        // Make sure that it's the right thing if you're adding a type to this list.
        // Only sealed types can be easily serviced with targetted patching.
        private static string[] UnsealedNonPublicTypeNames = new string[]
        {
            "Microsoft.OData.UriParser.MetadataBinder",
        };

        [TestMethod, Variation(Description = "Verifies that all OData public types are sealed as appropriate.")]
        public void SealedPublicTypes()
        {
            Assembly assembly = typeof(ODataAnnotatable).Assembly;

            var odataTypes = assembly.GetTypes().Where(t => t.IsPublic).ToList();

            IEnumerable<Type> allowedUnsealedTypes = UnsealedPublicTypes;

            // TODO: Enable this once we're done with Query OM.
            // Exclude all Query types for now.
            allowedUnsealedTypes = allowedUnsealedTypes.Concat(odataTypes.Where(t => t.Namespace == "Microsoft.OData.UriParser"));
            allowedUnsealedTypes = allowedUnsealedTypes.Concat(odataTypes.Where(t => t.Namespace == "Microsoft.OData.UriParser"));
            allowedUnsealedTypes = allowedUnsealedTypes.Concat(odataTypes.Where(t => t.Namespace == "Microsoft.OData.UriParser.Metadata"));

            var allowedUnsealeadTypesList = allowedUnsealedTypes.ToList();
            foreach (var odataType in odataTypes)
            {
                if (odataType.IsAbstract)
                {
                    continue;
                }

                if (allowedUnsealeadTypesList.Contains(odataType))
                {
                    continue;
                }

                this.Assert.IsTrue(odataType.IsSealed, "Public class '{0}' is not sealed.", odataType.FullName);
            }
        }

        [TestMethod, Variation(Description = "Verifies that all internal classes are sealed as appropriate.")]
        public void SealedInternalTypes()
        {
            Assembly assembly = typeof(ODataAnnotatable).Assembly;

            var nonPublicTypes = assembly.GetTypes().Where(t => !t.IsPublic).ToList();

            IEnumerable<string> allowedUnsealedTypeNames = UnsealedNonPublicTypeNames;

            var allowedUnsealedTypeNamesList = allowedUnsealedTypeNames.ToList();
            foreach (var nonPublicType in nonPublicTypes)
            {
                // Skip abstract types
                if (nonPublicType.IsAbstract)
                {
                    continue;
                }

                // Skip white listed types
                if (allowedUnsealedTypeNamesList.Contains(nonPublicType.FullName))
                {
                    continue;
                }

                // Skip sealed types since those are OK
                if (nonPublicType.IsSealed)
                {
                    continue;
                }

                // Skip types not in the Microsoft.OData.Core namespace since those are compiler generated
                if (nonPublicType.Namespace == null || !nonPublicType.Namespace.StartsWith("Microsoft.OData.Core"))
                {
                    continue;
                }

                if (nonPublicTypes.Any(t => t.BaseType == nonPublicType || (nonPublicType.IsGenericTypeDefinition && t.IsGenericType && t.GetGenericTypeDefinition() == nonPublicType)))
                {
                    continue;
                }

                this.Assert.Fail("Non public class '{0}' is not sealed.", nonPublicType.FullName);
            }
        }
    }
}