//---------------------------------------------------------------------
// <copyright file="DuplicateOperationValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Validation
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    internal class DuplicateOperationValidator
    {
        private readonly HashSetInternal<string> functionsParameterNameHash = new HashSetInternal<string>();
        private readonly HashSetInternal<string> functionsParameterTypeHash = new HashSetInternal<string>();
        private readonly HashSetInternal<string> actionsNameHash = new HashSetInternal<string>();

        private readonly ValidationContext context;

        internal DuplicateOperationValidator(ValidationContext context)
        {
            this.context = context;
        }

        public static bool IsDuplicateOperation(IEdmOperation operation, IEnumerable<IEdmOperation> candidateDuplicateOperations)
        {
            var validator = new DuplicateOperationValidator(null);
            foreach (var candidateOperation in candidateDuplicateOperations)
            {
                validator.ValidateNotDuplicate(candidateOperation, true /*skipError*/);
            }

            return validator.ValidateNotDuplicate(operation, true /*skipError*/);
        }

        public bool ValidateNotDuplicate(IEdmOperation operation, bool skipError)
        {
            bool duplicate = false;
            string fullName = operation.FullName();
            var function = operation as IEdmFunction;
            if (function != null)
            {
                var uniqueFunctionParameterName = BuildInternalUniqueParameterNameFunctionString(function);
                if (functionsParameterNameHash.Contains(uniqueFunctionParameterName))
                {
                    duplicate = true;
                    if (!skipError)
                    {
                        this.context.AddError(
                            function.Location(),
                            EdmErrorCode.DuplicateFunctions,
                            function.IsBound ? Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames(fullName) : Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames(fullName));
                    }
                }
                else
                {
                    functionsParameterNameHash.Add(uniqueFunctionParameterName);
                }

                var uniqueFunctionParameterType = BuildInternalUniqueParameterTypeFunctionString(function);
                if (functionsParameterTypeHash.Contains(uniqueFunctionParameterType))
                {
                    duplicate = true;
                    if (!skipError)
                    {
                        this.context.AddError(
                            function.Location(),
                            EdmErrorCode.DuplicateFunctions,
                            function.IsBound ? Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes(fullName) : Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes(fullName));
                    }
                }
                else
                {
                    functionsParameterTypeHash.Add(uniqueFunctionParameterType);
                }
            }
            else
            {
                var action = operation as IEdmAction;
                Debug.Assert(action != null, "action should not be null");
                var uniqueActionName = BuildInternalUniqueActionString(action);
                if (actionsNameHash.Contains(uniqueActionName))
                {
                    duplicate = true;
                    if (!skipError)
                    {
                        this.context.AddError(
                            action.Location(),
                            EdmErrorCode.DuplicateActions,
                            action.IsBound ? Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundActions(fullName) : Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundActions(fullName));
                    }
                }
                else
                {
                    actionsNameHash.Add(uniqueActionName);
                }
            }

            return duplicate;
        }

        /// <summary>
        /// Creates a unique function name based on the type. Used to find duplicates of functions.
        /// - The combination of function name, binding parameter type, and unordered set of non-binding parameter names MUST be unique within a namespace.
        /// - An unbound function MAY have the same name as a bound function. (Note this is why IsBound is added into the string)
        /// </summary>
        /// <param name="function">function to create the hash for.</param>
        /// <returns>A unique string that identifies a function.</returns>
        private static string BuildInternalUniqueParameterNameFunctionString(IEdmFunction function)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(function.IsBound);
            builder.Append("-");
            builder.Append(function.Namespace);
            builder.Append("-");
            builder.Append(function.Name);
            builder.Append("-");
            if (!function.Parameters.Any())
            {
                return builder.ToString();
            }

            if (function.IsBound)
            {
                IEdmOperationParameter bindingParameter = function.Parameters.FirstOrDefault();
                builder.Append(bindingParameter.Type.FullName());
                builder.Append("-");
                foreach (IEdmOperationParameter parameter in function.Parameters.Skip(1).OrderBy(p => p.Name))
                {
                    builder.Append(parameter.Name);
                    builder.Append("-");
                }
            }
            else
            {
                foreach (IEdmOperationParameter parameter in function.Parameters.OrderBy(p => p.Name))
                {
                    builder.Append(parameter.Name);
                    builder.Append("-");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a unique function name based on the type. Used to validate duplicates of functions. Rules this is validating
        /// - The combination of function name, binding parameter type, and ordered set of parameter types MUST be unique within a namespace.
        /// - An unbound function MAY have the same name as a bound function. (Note this is why IsBound is added into the string)
        /// </summary>
        /// <param name="function">function to use to create the hash. </param>
        /// <returns>A unique string that identifies a function.</returns>
        private static string BuildInternalUniqueParameterTypeFunctionString(IEdmFunction function)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(function.IsBound);
            builder.Append("-");
            builder.Append(function.Namespace);
            builder.Append("-");
            builder.Append(function.Name);
            builder.Append("-");

            foreach (IEdmOperationParameter parameter in function.Parameters)
            {
                builder.Append(parameter.Type.FullName());
                builder.Append("-");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a unique function name based on the type. Used to find duplicates of functions.
        /// - Bound actions support overloading (multiple actions having the same name within the same namespace) by binding parameter type. The combination of action name and the binding parameter type MUST be unique within a namespace.
        /// - Unbound actions do not support overloads. The names of all unbound actions MUST be unique within a namespace. An unbound action MAY have the same name as a bound action.
        /// </summary>
        /// <param name="action">action to build the hash from.</param>
        /// <returns>A unique string that identifies a function.</returns>
        private static string BuildInternalUniqueActionString(IEdmAction action)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(action.IsBound);
            builder.Append("-");
            builder.Append(action.Namespace);
            builder.Append("-");
            builder.Append(action.Name);
            builder.Append("-");
            if (!action.Parameters.Any())
            {
                return builder.ToString();
            }

            if (action.IsBound)
            {
                IEdmOperationParameter bindingParameter = action.Parameters.FirstOrDefault();
                builder.Append(bindingParameter.Type.FullName());
            }

            return builder.ToString();
        }
    }
}
