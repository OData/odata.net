//---------------------------------------------------------------------
// <copyright file="MemberValueGenerators.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;

    /// <summary>
    /// Generator that generates data in form of key-value pairs where key is a member path and value is a member value.
    /// <seealso cref="INamedValuesGenerator"/>
    /// </summary>
    internal class MemberValueGenerators : DataGenerator<IList<NamedValue>>, INamedValuesGenerator
    {
        private const int MaxRecursionLimit = 3;

        private readonly Dictionary<string, IDataGenerator> membersDataGenerators = new Dictionary<string, IDataGenerator>();
        private readonly IRandomNumberGenerator random;
        
        private int recursionLimit;
        private int currentRecursionDepth = 0;

        /// <summary>
        /// Initializes a new instance of the MemberValueGenerators class
        /// </summary>
        /// <param name="random">The random generator to use when determining maximum recursion depth</param>
        internal MemberValueGenerators(IRandomNumberGenerator random)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            this.random = random;
        }

        /// <summary>
        /// Adds the data generator for the member with the specified name.
        /// If for the specified member data generator already exists - overwrites the old one.
        /// If null data generator passed in - removes member from the generation.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <value>The data generator.</value>
        /// <exception cref="TaupoArgumentNullException">When memberName is null.</exception>
        /// <exception cref="TaupoArgumentException">When memberName is empty.</exception>
        public IDataGenerator this[string memberName]
        {
            get
            {
                return this.GetMemberDataGenerator(memberName);
            }

            set
            {
                ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");
                ExceptionUtilities.Assert(!memberName.Contains('.'), "Cannot set data generator for the nested member '{0}'. Member name cannot contain '.'.", memberName);
                if (value == null)
                {
                    this.membersDataGenerators.Remove(memberName);

                    return;
                }

                this.membersDataGenerators[memberName] = value;
            }
        }

        /// <summary>
        /// Generates data for members for which data generators are added.
        /// </summary>
        /// <returns>Key-value pairs where key is a member path and value is a member value.</returns>
        /// <remarks>If there is a loop in the data generators, it will randomly decide how deep to recurse.</remarks>
        public override IList<NamedValue> GenerateData()
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            if (this.currentRecursionDepth > this.recursionLimit)
            {
                // TODO: should this be null instead?
                return new List<NamedValue>();
            }

            if (this.currentRecursionDepth == 0)
            {
                this.recursionLimit = this.random.Next(MaxRecursionLimit);
            }

            try
            {
                this.currentRecursionDepth++;

                var result = new List<NamedValue>();
                foreach (string currentMemberPath in this.membersDataGenerators.Keys)
                {
                    IDataGenerator memberDataGenerator = this.GetMemberDataGenerator(currentMemberPath);
                    var memberData = memberDataGenerator.GenerateData();
                    result.AddMemberData(currentMemberPath, memberData);
                }

                return result;
            }
            finally
            {
                this.currentRecursionDepth--;
            }
        }

        /// <summary>
        /// Gets a data generator for the specified member.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <returns>A data generator for the specified member.</returns>
        public IDataGenerator GetMemberDataGenerator(string memberName)
        {
            string nextMemberName;
            string currenMemberName = GetCurrentMemberName(memberName, out nextMemberName);

            IDataGenerator memberDataGenerator;
            this.membersDataGenerators.TryGetValue(currenMemberName, out memberDataGenerator);
            ExceptionUtilities.CheckObjectNotNull(memberDataGenerator, "Cannot find data generator for member '{0}'. Note that foreign keys cannot have standalone data generator as they involve related entities.", memberName);

            if (nextMemberName != null)
            {
                var memberDataGenerators = memberDataGenerator as IMemberDataGenerators;
                ExceptionUtilities.CheckObjectNotNull(memberDataGenerators, "Cannot find data generator for member '{0}'. Note that foreign keys cannot have standalone data generator as they involve related entities.", memberName);

                memberDataGenerator = memberDataGenerators.GetMemberDataGenerator(nextMemberName);
            }

            return memberDataGenerator;
        }

        internal static string GetCurrentMemberName(string memberName, out string nextMemberName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");
            string currenMemberName = memberName;
            nextMemberName = null;
            int indexOfDot = memberName.IndexOf('.');
            if (indexOfDot > 0)
            {
                currenMemberName = memberName.Substring(0, indexOfDot);
                ExceptionUtilities.Assert(indexOfDot + 1 < memberName.Length, "Member name is invalid. It cannot end with '.'. Member name: '{0}'.", memberName);
                nextMemberName = memberName.Substring(indexOfDot + 1);
            }

            return currenMemberName;
        }
    }
}
