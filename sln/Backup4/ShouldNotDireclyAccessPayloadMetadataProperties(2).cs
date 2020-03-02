//---------------------------------------------------------------------
// <copyright file="ShouldNotDireclyAccessPayloadMetadataProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using System;
    using Microsoft.OData.Service.Serializers;
    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// This rule checks that certain ODL object-model properties which are affected
    /// by the metadata query-option are not set directly.
    /// </summary>
    public sealed class ShouldNotDireclyAccessPayloadMetadataProperties : BaseDataWebRule
    {
        /// <summary>Initializes a new <see cref="ShouldNotDireclyAccessPayloadMetadataProperties"/> instance.</summary>
        public ShouldNotDireclyAccessPayloadMetadataProperties()
            : base("ShouldNotDireclyAccessPayloadMetadataProperties")
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
            Visit(member);
            return Problems.Count > 0 ? Problems : null;
        }

        public override void VisitMethodCall(MethodCall call)
        {
            if (call.SourceContext.FileName == null
                || !call.SourceContext.FileName.Contains(@"System\Data\Services")
                || call.SourceContext.FileName.Contains(@"System\Data\Services\Client"))
            {
                return;
            }

            MemberBinding callBinding = call.Callee as MemberBinding;
            if (callBinding != null)
            {
                Method method = (Method)callBinding.BoundMember;

                if (callBinding.TargetObject == null || callBinding.TargetObject.Type == null)
                {
                    return;
                }

                string[] disallowedPropertyNames;
                string targetTypeName = callBinding.TargetObject.Type.FullName;
                switch (targetTypeName)
                {
                    case "Microsoft.OData.Core.ODataEntry":
                        disallowedPropertyNames = Enum.GetNames(typeof(PayloadMetadataKind.Entry));
                        break;

                    case "Microsoft.OData.Core.ODataResourceSet":
                        disallowedPropertyNames = Enum.GetNames(typeof(PayloadMetadataKind.Feed));
                        break;

                    case "Microsoft.OData.Core.ODataNestedResourceInfo":
                        disallowedPropertyNames = Enum.GetNames(typeof(PayloadMetadataKind.Navigation));
                        break;

                    case "Microsoft.OData.Core.ODataAssociationLink":
                        disallowedPropertyNames = Enum.GetNames(typeof(PayloadMetadataKind.Association));
                        break;

                    case "Microsoft.OData.Core.ODataStreamReferenceValue":
                        disallowedPropertyNames = Enum.GetNames(typeof(PayloadMetadataKind.Stream));
                        break;

                    case "Microsoft.OData.Core.ODataOperation":
                    case "Microsoft.OData.Core.ODataAction":
                    case "Microsoft.OData.Core.ODataFunction":
                        disallowedPropertyNames = Enum.GetNames(typeof(PayloadMetadataKind.Operation));
                        break;

                    default:
                        return;
                }

                foreach (var propertyName in disallowedPropertyNames)
                {
                    bool isSetter = method.FullName.Contains(".set_" + propertyName);
                    bool isGetter = method.FullName.Contains(".get_" + propertyName);
                    if (isSetter || isGetter)
                    {
                        Problem problem = new Problem(this.GetResolution(propertyName, targetTypeName));
                        this.Problems.Add(problem);
                    }
                }
            }

            base.VisitMethodCall(call);
        }
    }
}
