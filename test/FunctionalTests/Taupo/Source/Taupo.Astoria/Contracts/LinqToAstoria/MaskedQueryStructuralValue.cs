//---------------------------------------------------------------------
// <copyright file="MaskedQueryStructuralValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Query structural value that supports hiding certain property values by removing them from the list of member names
    /// </summary>
    internal class MaskedQueryStructuralValue : QueryStructuralValue
    {
        private List<string> hiddenNames = new List<string>();

        private MaskedQueryStructuralValue(QueryStructuralType type, bool isNull, QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base(type, isNull, evaluationError, evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets the names of this instance's member properties
        /// </summary>
        public override IEnumerable<string> MemberNames
        {
            get
            {
                return base.MemberNames.Except(this.hiddenNames);
            }
        }

        /// <summary>
        /// Gets the names of this instance's member properties, including any that have been hidden
        /// </summary>
        public IEnumerable<string> AllMemberNames
        {
            get
            {
                return base.MemberNames;
            }
        }

        /// <summary>
        /// Hides the given member name, causing it to be left out of the list of member names
        /// </summary>
        /// <param name="memberName">The member name to hide</param>
        public void HideMember(string memberName)
        {
            this.hiddenNames.Add(memberName);
        }

        /// <summary>
        /// Unhides the given member name, causing it to be in the list of member names
        /// </summary>
        /// <param name="memberName">The member name to hide</param>
        public void ShowMember(string memberName)
        {
            this.hiddenNames.Remove(memberName);
        }

        /// <summary>
        /// Static method for creating masked structural values. Will not hide any properties initially.
        /// </summary>
        /// <param name="toMask">The structural value to mask</param>
        /// <returns>A masked structural value</returns>
        internal static MaskedQueryStructuralValue Create(QueryStructuralValue toMask)
        {
            ExceptionUtilities.CheckArgumentNotNull(toMask, "toMask");
            var masked = new MaskedQueryStructuralValue(toMask.Type, toMask.IsNull, toMask.EvaluationError, toMask.Type.EvaluationStrategy);
            foreach (var memberName in toMask.MemberNames)
            {
                masked.SetValue(memberName, toMask.GetValue(memberName));
            }

            masked.Annotations.AddRange(toMask.Annotations.Select(a => a.Clone()));

            return masked;
        }
    }
}
