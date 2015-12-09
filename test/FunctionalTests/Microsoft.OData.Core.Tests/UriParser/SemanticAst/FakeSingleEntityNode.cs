//---------------------------------------------------------------------
// <copyright file="FakeSingleEntityNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Visitors;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Fake implementation of SingleEntityNode. This is intended to be used as a parent node in unit tests.
    /// </summary>
    public class FakeSingleEntityNode : SingleEntityNode
    {
        private readonly IEdmEntityTypeReference typeReference;
        private readonly IEdmEntitySetBase set;

        public FakeSingleEntityNode(IEdmEntityTypeReference type, IEdmEntitySetBase set)
        {
            this.typeReference = type;
            this.set = set;
        }

        public static FakeSingleEntityNode CreateFakeSingleEntityNodeForPerson()
        {
            return new FakeSingleEntityNode(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
        }

        public static FakeSingleEntityNode CreateFakeSingleEntityNodeForDog()
        {
            return new FakeSingleEntityNode(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
        }

        public static FakeSingleEntityNode CreateFakeSingleEntityNodeForLion()
        {
            return new FakeSingleEntityNode(HardCodedTestModel.GetLionTypeReference(), HardCodedTestModel.GetLionSet());
        }

        public override IEdmTypeReference TypeReference
        {
            get { return this.typeReference;  }
        }

        public override IEdmNavigationSource NavigationSource
        {
            get { return this.set; }
        }

        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.typeReference; }
        }

        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}
