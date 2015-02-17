//---------------------------------------------------------------------
// <copyright file="DSPActionAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Diagnostics;
    using System;

    /// <summary>
    /// Attribute to help the <see cref="DSPActionProvider"/> to construct the <see cref="ServiceAction"/> metadata object
    /// from the attributed method through reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DSPActionAttribute : Attribute
    {
        /// <summary>
        /// Empty string array.
        /// </summary>
        private static readonly string[] EmptyStringArray = new string[0];

        /// <summary>
        /// Constructs a new instance of the attribute.
        /// </summary>
        /// <param name="isBindable">true if the first parameter to the method is the binding parameter; false otherwise.</param>
        public DSPActionAttribute(OperationParameterBindingKind operationParameterBindingKind)
        {
            this.OperationParameterBindingKind = operationParameterBindingKind;
            this.ParameterTypeNames = DSPActionAttribute.EmptyStringArray;
        }

        /// <summary>
        /// The return element type of the method. If the method returns a <see cref="DSPResource"/> or a collection of <see cref="DSPResource"/>, this should
        /// be the full type name of the element type represented by the <see cref="DSPResource"/>; otherwise the value of this property is ignored.
        /// </summary>
        public string ReturnElementTypeName { get; set; }

        /// <summary>
        /// If the method returns an entity type, this is the name of the ResourceSet which containing the entity type.
        /// Note that ReturnSet and ReturnSetPath must be mutually exclusive.
        /// </summary>
        public string ReturnSet { get; set; }

        /// <summary>
        /// If the method returns an entity type, this is the path expression to the ResourceSet which containing the entity type.
        /// Note that ReturnSet and ReturnSetPath must be mutually exclusive.
        /// </summary>
        public string ReturnSetPath { get; set; }

        /// <summary>
        /// Enumeration for classifying the different kinds of operation parameter binding.
        /// </summary>
        public OperationParameterBindingKind OperationParameterBindingKind { get; private set; }

        /// <summary>
        /// List of parameter type names for the action. Note that since reflection cannot determine the type name of a <see cref="DSPResource"/>, the <see cref="DSPActionProvider"/>
        /// will use the corresponding type name in the array for each parameter that is of <see cref="DSPResource"/> type or collection type of <see cref="DSPResource"/>.
        /// Names of other types in the array will be ignored since the <see cref="DSPActionProvider"/> will use reflection to determine the actual type name from the parameter, however
        /// an entry must be provided for those parameters. For parameters that are of type collection of <see cref="DSPResource"/>, only the element type name is necessary.
        /// </summary>
        public string[] ParameterTypeNames { get; set; }
    }
}
