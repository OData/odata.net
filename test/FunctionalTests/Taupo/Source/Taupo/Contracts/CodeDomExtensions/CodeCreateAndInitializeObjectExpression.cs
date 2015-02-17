//---------------------------------------------------------------------
// <copyright file="CodeCreateAndInitializeObjectExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Object creation and initialization expression (can be used to instantiate named and anonymous types
    /// and set their initial properties).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class CodeCreateAndInitializeObjectExpression : CodeExpression, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the CodeCreateAndInitializeObjectExpression class.
        /// </summary>
        /// <param name="constructorParameters">The constructor parameters.</param>
        public CodeCreateAndInitializeObjectExpression(params CodeExpression[] constructorParameters)
            : this(null, constructorParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeCreateAndInitializeObjectExpression class.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="constructorParameters">The constructor parameters.</param>
        public CodeCreateAndInitializeObjectExpression(CodeTypeReference objectType,  params CodeExpression[] constructorParameters)
        {
            this.ObjectType = objectType;
            this.ConstructorParameters = new CodeExpressionCollection();
            this.ConstructorParameters.AddRange(constructorParameters);
            this.PropertyInitializers = new List<KeyValuePair<string, CodeExpression>>();
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type to instantiate.</value>
        public CodeTypeReference ObjectType { get; set; }

        /// <summary>
        /// Gets the constructor parameters.
        /// </summary>
        /// <value>The constructor parameters.</value>
        public CodeExpressionCollection ConstructorParameters { get; private set; }

        /// <summary>
        /// Gets property initializers.
        /// </summary>
        /// <value>The initializers.</value>
        public IList<KeyValuePair<string, CodeExpression>> PropertyInitializers { get; private set; }

        /// <summary>
        /// Adds the specified property initializer.
        /// </summary>
        /// <param name="propertyInitializer">The property initializer.</param>
        public void Add(CodeExpression propertyInitializer)
        {
            this.PropertyInitializers.Add(new KeyValuePair<string, CodeExpression>(null, propertyInitializer));
        }

        /// <summary>
        /// Adds the specified named property initializer.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="propertyInitializer">The property initializer.</param>
        public void Add(string memberName, CodeExpression propertyInitializer)
        {
            this.PropertyInitializers.Add(new KeyValuePair<string, CodeExpression>(memberName, propertyInitializer));
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}