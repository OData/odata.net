//---------------------------------------------------------------------
// <copyright file="Function.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Function in EntitySchemaModel
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Function", Justification = "This represents function in entity model but EntityFunction is not appropriate.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class Function : NamedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the Function class.
        /// </summary>
        public Function()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Function class with given name.
        /// </summary>
        /// <param name="name">Name of the Function</param>
        public Function(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Function class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">Namespace of the Function</param>
        /// <param name="name">Name of the Function</param>
        public Function(string namespaceName, string name)
            : base(namespaceName, name)
        {
            this.Parameters = new List<FunctionParameter>();
        }

        /// <summary>
        /// Gets the parameters of the Function
        /// </summary>
        public IList<FunctionParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets or sets the return type of the Function
        /// </summary>
        public DataType ReturnType { get; set; }

        /// <summary>
        /// Gets the model this function belongs to. If it is null, then the function has not been added to a model.
        /// </summary>
        public EntityModelSchema Model { get; internal set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.Function"/>.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator Function(string functionName)
        {
            return new FunctionReference(functionName);
        }

        /// <summary>
        /// Determines whether the specified <see cref="INamedItem"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="INamedItem"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="INamedItem"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(INamedItem other)
        {
            var otherFunction = other as Function;
            if (otherFunction == null)
            {
                return false;
            }

            return (this.Name == otherFunction.Name) && (this.NamespaceName == otherFunction.NamespaceName);
        }

        /// <summary>
        /// Adds a new parameter to this function
        /// </summary>
        /// <param name="parameter">The parameter to be added</param>
        public void Add(FunctionParameter parameter)
        {
            this.Parameters.Add(parameter);
        }
    }
}
