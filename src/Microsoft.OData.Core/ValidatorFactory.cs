//---------------------------------------------------------------------
// <copyright file="ValidatorFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    /// <summary>
    /// The factory to create validators.
    /// </summary>
    internal static class ValidatorFactory
    {
        /// <summary>The instance of writer validator with full validation.</summary>
        private static IWriterValidator fullWriterValidator;

        /// <summary>The instance of writer validator with minimal validation.</summary>
        private static IWriterValidator minimalWriterValidator;

        public static IWriterValidator CreateWriterValidator(bool enableFullValidation)
        {
            if (enableFullValidation)
            {
                return fullWriterValidator ?? (fullWriterValidator = new WriterValidatorFullValidation());
            }
            else
            {
                return minimalWriterValidator ?? (minimalWriterValidator = new WriterValidatorMinimalValidation());
            }
        }
    }
}
