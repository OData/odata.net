//---------------------------------------------------------------------
// <copyright file="BasicWriterValidatorFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// The factory class to create basic writer validators.
    /// </summary>
    internal static class BasicWriterValidatorFactory
    {
        /// <summary>The instance of functional basic writer validator.</summary>
        private static IBasicWriterValidator basicWriterValidator;

        /// <summary>The instance of dummy basic writer validator.</summary>
        private static IBasicWriterValidator dummyWriterValidator;

        public static IBasicWriterValidator CreateBasicWriterValidator(bool enableBasicValidation)
        {
            if (enableBasicValidation)
            {
                return basicWriterValidator ?? (basicWriterValidator = new BasicWriterValidator());
            }
            else
            {
                return dummyWriterValidator ?? (dummyWriterValidator = new DummyBasicWriterValidator());
            }
        }
    }
}
