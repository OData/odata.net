//---------------------------------------------------------------------
// <copyright file="StoreGeneratedPatternAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Defines a store-generated identity or computed property.
    /// </summary>
    public sealed class StoreGeneratedPatternAnnotation : Annotation
    {
        /// <summary>
        /// Initializes static members of the StoreGeneratedPatternAnnotation class.
        /// </summary>
        static StoreGeneratedPatternAnnotation()
        {
            None = new StoreGeneratedPatternAnnotation
            {
                ServerGeneratedOnInsert = false,
                ServerGeneratedOnUpdate = false,
                ClientProvidesValueOnInsert = true,
                ClientProvidesValueOnUpdate = true,
                Name = "None",
            };
            
            Identity = new StoreGeneratedPatternAnnotation
            {
                ServerGeneratedOnUpdate = false,
                ServerGeneratedOnInsert = true,
                ClientProvidesValueOnInsert = false,
                ClientProvidesValueOnUpdate = false,
                Name = "Identity",
            };

            Computed = new StoreGeneratedPatternAnnotation
            {
                ServerGeneratedOnUpdate = true,
                ServerGeneratedOnInsert = true,
                ClientProvidesValueOnInsert = false,
                ClientProvidesValueOnUpdate = false,
                Name = "Computed",
            };
        }

        /// <summary>
        /// Prevents a default instance of the StoreGeneratedPatternAnnotation class from being created.
        /// </summary>
        private StoreGeneratedPatternAnnotation()
        {
        }

        /// <summary>
        /// Gets a predefined <see cref="StoreGeneratedPatternAnnotation"/> indicating that it is not a server generated property;
        /// </summary>
        /// <value>The <see cref="StoreGeneratedPatternAnnotation" /> indicating a non-server-generated property.</value>
        public static StoreGeneratedPatternAnnotation None { get; private set; }

        /// <summary>
        /// Gets a predefined <see cref="StoreGeneratedPatternAnnotation"/> indicating that value is generated on insert and remains unchanged on update.;
        /// </summary>
        /// <value>The <see cref="StoreGeneratedPatternAnnotation" /> indicating an identity property.</value>
        public static StoreGeneratedPatternAnnotation Identity { get; private set; }

        /// <summary>
        /// Gets a predefined <see cref="StoreGeneratedPatternAnnotation"/> indicating that value is generated on both insert and update.;
        /// </summary>
        /// <value>The <see cref="StoreGeneratedPatternAnnotation" /> indicating a computed property.</value>
        public static StoreGeneratedPatternAnnotation Computed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the value is server generated on INSERT.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if the value is server generated on INSERT; otherwise, <c>false</c>.
        /// </value>
        public bool ServerGeneratedOnInsert { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the value is server generated on UPDATE.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if the value is server generated on UPDATE; otherwise, <c>false</c>.
        /// </value>
        public bool ServerGeneratedOnUpdate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the client should provide a value during INSERT.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if the value is the client should provide a value during INSERT; otherwise, <c>false</c>.
        /// </value>
        public bool ClientProvidesValueOnInsert { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the client should provide a value during UPDATE.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if the value is the client should provide a value during UPDATE; otherwise, <c>false</c>.
        /// </value>
        public bool ClientProvidesValueOnUpdate { get; private set; }

        /// <summary>
        /// Gets the name of the pattern to use when serializing it into xsdl
        /// </summary>
        public string Name { get; private set; }
    }
}
