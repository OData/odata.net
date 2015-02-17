//---------------------------------------------------------------------
// <copyright file="CodeAttributeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Class to represent CodeAttributes 
    /// </summary>
    public abstract class CodeAttributeAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the CodeAttributeAnnotation class
        /// </summary>
        /// <param name="attributeType">Type of Annotation</param>
        /// <param name="arguments">Argument list of the Annotation</param>
        protected CodeAttributeAnnotation(Type attributeType, params KeyValuePair<string, object>[] arguments)
        {
            ExceptionUtilities.CheckArgumentNotNull(attributeType, "attributeType");
            ExceptionUtilities.CheckArgumentNotNull(arguments, "arguments");

            this.TypeOfAttribute = attributeType;

            this.Arguments = arguments.ToArray();
        }

        /// <summary>
        /// Gets Type of Annotation
        /// </summary>
        public Type TypeOfAttribute { get; private set; }

        /// <summary>
        /// Gets or sets KeyValuePair to represent Name\Value pair of attributes of the Annotation
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Arguments { get; protected set; }

        /// <summary>
        /// Creates new CodeAttributreAnnotation from the passed argument values
        /// Will throw for all the Annotations which do not have any arguments
        /// </summary>
        /// <param name="arguments">List of arguments for the Annotation</param>
        /// <returns>new instance of CodeAttributeAnnotation</returns>
        protected virtual CodeAttributeAnnotation CreateFromArguments(IEnumerable<KeyValuePair<string, object>> arguments)
        {
            throw new TaupoInvalidOperationException("This code attribute does not support creation from arguments.");
        }

        /// <summary>
        /// Adds argument to the Annotation and recreates it
        /// </summary>
        /// <typeparam name="T">Type of CodeAttributeAnnotation</typeparam>
        /// <param name="argumentName">Name of the argument</param>
        /// <param name="value">Value of the argument</param>
        /// <returns>new CodeAttributeAnnotation of type T</returns>
        protected T WithArgument<T>(string argumentName, object value)
            where T : CodeAttributeAnnotation
        {
            return (T)this.CreateFromArguments(this.MergeArguments(this.Arguments, new KeyValuePair<string, object>(argumentName, value)).ToArray());
        }

        /// <summary>
        /// Adds\Updates the Annotation with the new argument passed
        /// </summary>
        /// <param name="arguments">The List of existing arguments on the Annotation</param>
        /// <param name="argument">argument to be added</param>
        /// <returns>List of Arguments</returns>
        private IEnumerable<KeyValuePair<string, object>> MergeArguments(IEnumerable<KeyValuePair<string, object>> arguments, KeyValuePair<string, object> argument)
        {
            List<KeyValuePair<string, object>> argumentList = arguments.ToList();
            argumentList.Remove(arguments.SingleOrDefault(kvp => kvp.Key == argument.Key));
            argumentList.Add(argument);
            return argumentList;
        }
    }
}
