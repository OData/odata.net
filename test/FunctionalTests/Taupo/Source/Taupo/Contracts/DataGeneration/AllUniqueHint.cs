//---------------------------------------------------------------------
// <copyright file="AllUniqueHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to generate all unique values.
    /// </summary>
    public sealed class AllUniqueHint : SingletonDataGenerationHint
    {
        private static AllUniqueHint instance = new AllUniqueHint();
        
        /// <summary>
        /// Prevents a default instance of the AllUniqueHint class from being created.
        /// </summary>
        private AllUniqueHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the AllUniqueHint.
        /// </summary>
        /// <value>The AllUniqueHint.</value>
        internal static AllUniqueHint Instance
        {
            get { return instance; }
        }
    }
}
