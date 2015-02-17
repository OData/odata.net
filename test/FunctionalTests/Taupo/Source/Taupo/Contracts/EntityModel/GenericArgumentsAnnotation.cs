//---------------------------------------------------------------------
// <copyright file="GenericArgumentsAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections.ObjectModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// An annotation used on a derived <see cref="EntityType"/> to indicate
    /// the type arguments it uses for inheriting from a generic base type.
    /// </summary>
    public class GenericArgumentsAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericArgumentsAnnotation"/> class.
        /// </summary>
        /// <param name="baseTypeGenericTypeArguments">The base type generic type arguments. You can pass
        /// strings (type parameter names) or <see cref="DataType"/>s directly.</param>
        public GenericArgumentsAnnotation(params GenericArgument[] baseTypeGenericTypeArguments)
        {
            this.GenericArguments = new ReadOnlyCollection<GenericArgument>(baseTypeGenericTypeArguments);
        }

        /// <summary>
        /// Gets the generic arguments used to fill the type parameters for a base class.
        /// </summary>
        public ReadOnlyCollection<GenericArgument> GenericArguments { get; private set; }
    }
}
