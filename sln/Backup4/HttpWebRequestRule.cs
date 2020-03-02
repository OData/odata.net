//---------------------------------------------------------------------
// <copyright file="HttpWebRequestRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using Microsoft.FxCop.Sdk;
    using System.Collections.Generic;

    /// <summary>
    /// This rule checks that methods (mentioned above) on HttpWebRequest and HttpWebResponse are called
    /// via the WebUtil methods
    /// </summary>
    public class HttpWebRequestRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        private static readonly List<string> RequestTypeNames = new List<string>
        {
            "System.Net.WebRequest",
            "System.Net.HttpWebRequest",
            "Microsoft.OData.Service.Http.WebRequest",
            "Microsoft.OData.Service.Http.HttpWebRequest",
            "Microsoft.OData.Service.Http.ClientHttpWebRequest",
            "Microsoft.OData.Service.Http.XHRHttpWebRequest"
        };

        private static readonly List<string> RequestMethodNames = new List<string>
        {
            "GetResponse",
            "BeginGetResponse",
            "EndGetResponse",
            "GetStream",
            "BeginGetRequestStream",
            "EndGetRequestStream"
        };

        private static readonly List<string> ResponseTypeNames = new List<string>
        {
            "System.Net.WebResponse",
            "System.Net.HttpWebResponse",
            "Microsoft.OData.Service.Http.WebResponse",
            "Microsoft.OData.Service.Http.HttpWebResponse",
            "Microsoft.OData.Service.Http.ClientHttpWebResponse",
            "Microsoft.OData.Service.Http.XHRHttpWebResponse"
        };

        private static readonly List<string> ResponseMethodNames = new List<string>
        {
            "GetResponseStream"
        };

        /// <summary>Initializes a new <see cref="HttpWebRequestRule"/> instance.</summary>
        public HttpWebRequestRule()
            : base("HttpWebRequestRule")
        {
        }

        /// <summary>Visibility of targets to which this rule commonly applies.</summary>
        public override TargetVisibilities TargetVisibility
        {
            get
            {
                return TargetVisibilities.All;
            }
        }

        /// <summary>Checks type members.</summary>
        /// <param name="member">Member being checked.</param>
        /// <returns>A collection of problems found in <paramref name="member"/> or null.</returns>
        public override ProblemCollection Check(Member member)
        {
            Method method = member as Method;
            if (method == null)
            {
                return null;
            }

            methodUnderCheck = method;
            Visit(method);
            return Problems.Count > 0 ? Problems : null;
        }

        /// <summary>
        /// Verifies that the only access to the specified methods is through the corresponding WebUtil wrapper method.
        /// Using MemberBinding visitor instead of MethodCall because it will cover both direct method access (e.g. response.GetResponseStream())
        /// as well as delegate invocation (e.g. SomeMethod.GetResponseStream, where SomeMethod actually is the one to invoke the method).
        /// </summary>
        /// <param name="memberBinding">MemberBinding to validate.</param>
        public override void VisitMemberBinding(MemberBinding memberBinding)
        {
            if (memberBinding != null && memberBinding.BoundMember.NodeType == NodeType.Method)
            {
                Method method = (Method)memberBinding.BoundMember;
                if (RequestTypeNames.Contains(method.DeclaringType.FullName) && RequestMethodNames.Contains(method.Name.Name))
                {
                    if (methodUnderCheck.DeclaringType.FullName != "Microsoft.OData.Client.HttpWebRequestMessage" || methodUnderCheck.Name.Name != method.Name.Name)
                    {
                        this.Problems.Add(new Problem(GetResolution(method.Name.Name)));
                    }
                }
                else if (ResponseTypeNames.Contains(method.DeclaringType.FullName) && ResponseMethodNames.Contains(method.Name.Name))
                {
                    if (methodUnderCheck.DeclaringType.FullName != "Microsoft.OData.Client.HttpWebResponseMessage")
                    {
                        this.Problems.Add(new Problem(GetResolution(method.Name.Name)));
                    }
                }
            }

            base.VisitMemberBinding(memberBinding);
        }
    }
}
