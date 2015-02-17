//---------------------------------------------------------------------
// <copyright file="FunctionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.Types;
    
    /// <summary>
    /// FunctionImport in EntityContainer
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class FunctionImport : AnnotatedItem
    {
        /// <summary>
        /// Initializes a new instance of the FunctionImport class.
        /// </summary>
        public FunctionImport()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FunctionImport class with the given name.
        /// </summary>
        /// <param name="name">the name of FunctionImport.</param>
        public FunctionImport(string name)
        {
            this.Name = name;
            this.Parameters = new List<FunctionParameter>();
            this.ReturnTypes = new List<FunctionImportReturnType>();
            this.IsSideEffecting = true;
        }

        /// <summary>
        /// Gets or sets type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the parameters of the Function
        /// </summary>
        public IList<FunctionParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets the return types of the Function
        /// </summary>
        public IList<FunctionImportReturnType> ReturnTypes { get; private set; }

        /// <summary>
        /// Gets the entity container this function import belongs to. If it is null, then the function import has not been added to any container.
        /// NOTE: even though function import has an entity set property, it may not ever be set, so we need to record its container as well
        /// </summary>
        public EntityContainer Container { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether FunctionImport is composable.
        /// </summary>
        public bool IsComposable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether FunctionImport is sideeffecting
        /// </summary>
        public bool IsSideEffecting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether FunctionImport is bindable.
        /// </summary>
        public bool IsBindable { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.FunctionImport"/>.
        /// </summary>
        /// <param name="name">Name of the function import.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator FunctionImport(string name)
        {
            return new FunctionImportReference(name);
        }

        /// <summary>
        /// Adds a new parameter to this function
        /// </summary>
        /// <param name="parameter">The parameter to be added</param>
        public void Add(FunctionParameter parameter)
        {
            this.Parameters.Add(parameter);
        }

        /// <summary>
        /// Adds a new Return Type to this function
        /// </summary>
        /// <param name="returnType">The parameter to be added</param>
        public void Add(FunctionImportReturnType returnType)
        {
            this.ReturnTypes.Add(returnType);
        }
    }
}
