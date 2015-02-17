//---------------------------------------------------------------------
// <copyright file="RandomNumberGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Random number generator
    /// </summary>
    [ImplementationName(typeof(IRandomNumberGenerator), "Default")]
    public class RandomNumberGenerator : IRandomNumberGenerator, IInitializable
    {
        private Random random;

        /// <summary>
        /// Initializes a new instance of the RandomNumberGenerator class.
        /// </summary>
        public RandomNumberGenerator()
            : this(Logger.Null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RandomNumberGenerator class
        /// </summary>
        /// <param name="logger">Logger to use.</param>
        public RandomNumberGenerator(Logger logger)
        {
            if (logger == null)
            {
                throw new TaupoArgumentException("Use Logger.Null if you don't want logging.");
            }

            this.Logger = logger;
            this.InitialSeed = (int)DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets seed to give to the RandomNumber Generator
        /// </summary>
        [InjectTestParameter("Seed", HelpText = "Seed for random number generator", DefaultValueDescription = "(use random seed)")]
        public int InitialSeed { get; set; }

        /// <summary>
        /// Initializes this object (after construction and property assignment but before use).
        /// </summary>
        public void Initialize()
        {
            if (this.random == null)
            {
                ExceptionUtilities.CheckAllRequiredDependencies(this);
                this.Logger.WriteLine(LogLevel.Verbose, "Initializing random number generator with seed: {0}", this.InitialSeed);
                this.random = new Random(this.InitialSeed);
            }
        }

        /// <summary>
        /// Returns a random value between 0 and <paramref name="maxValue"/> - 1.
        /// </summary>
        /// <param name="maxValue">maximum value</param>
        /// <returns>Random value beween 0 and <paramref name="maxValue"/> - 1.</returns>
        public int Next(int maxValue)
        {
            this.Initialize();

            return maxValue / 2;
        }

        /// <summary>
        /// Returns a random sequence of bytes
        /// </summary>
        /// <param name="length">The length</param>
        /// <returns>Random sequence of bytes</returns>
        public byte[] NextBytes(int length)
        {
            this.Initialize();
            byte[] bytes = new byte[length];
            this.random.NextBytes(bytes);
            return bytes;
        }
    }
}
