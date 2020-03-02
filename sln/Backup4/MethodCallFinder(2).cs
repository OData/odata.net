//---------------------------------------------------------------------
// <copyright file="MethodCallFinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using System;
    using Microsoft.FxCop.Sdk;
    using System.Collections.Generic;
    internal class MethodCallFinder : BinaryReadOnlyVisitor
    {
        private readonly Dictionary<string, int> timesFound = new Dictionary<string, int>();
        private readonly HashSet<string> alreadyExplored = new HashSet<string>();
        private bool exploreGraph = false;

        internal MethodCallFinder(params string[] fullNames)
            : this(false, (IEnumerable<string>)fullNames)
        {
        }

        internal MethodCallFinder(bool exploreGraph, params string[] fullNames)
            : this(exploreGraph, (IEnumerable<string>)fullNames)
        {
        }

        internal MethodCallFinder(bool exploreGraph, IEnumerable<string> fullNames)
        {
            this.exploreGraph = exploreGraph;
            foreach (string fullName in fullNames)
            {
                this.timesFound.Add(fullName, 0);
            }
        }

        internal IDictionary<string, int> TimesFound { get { return this.timesFound; } }
        internal bool Found(string fullName) { return this.timesFound[fullName] > 0; }

        public override void VisitMethodCall(MethodCall call)
        {
            if (call != null)
            {
                MemberBinding binding = call.Callee as MemberBinding;
                Method calleeMethod = (binding == null) ? null : binding.BoundMember as Method;
                if (calleeMethod != null)
                {

                    string fullName = calleeMethod.FullName;
                    if (exploreGraph && alreadyExplored.Add(fullName))
                    {
                        // see if the method(s) is is the call graph
                        Visit(calleeMethod);
                    }

                    int hits;
                    if (this.timesFound.TryGetValue(fullName, out hits))
                    {
                        this.timesFound[fullName] = hits + 1;
                    }
                }
            }

            base.VisitMethodCall(call);
        }
    }
}