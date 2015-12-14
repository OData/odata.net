//---------------------------------------------------------------------
// <copyright file="FakeBindMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    /// <summary>
    /// Class that contains fake bind methods to help unit semantic tests from growing in scope.
    /// </summary>
    internal class FakeBindMethods
    {
        private static readonly EntityRangeVariable DogsEntityRangeVariable = new EntityRangeVariable("A_DOG", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
        private static readonly EntityRangeVariable PeopleEntityRangeVariable = new EntityRangeVariable("A_PERSON", HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
        private static readonly EntityRangeVariable PaintingEntityRangeVariable = new EntityRangeVariable("A_PAINTING", HardCodedTestModel.GetPaintingTypeReference(), HardCodedTestModel.GetPaintingsSet());
        private static readonly EntityRangeVariable LionsEntityRangeVariable = new EntityRangeVariable("A_LION", HardCodedTestModel.GetLionTypeReference(), HardCodedTestModel.GetLionSet());
        private const int KeyBinderConstantValue = 0xBAD;
        
        public static readonly ConstantNode KeyBinderConstantToken = new ConstantNode(KeyBinderConstantValue);
        
        public static readonly EntityRangeVariableReferenceNode FakeDogNode = 
            new EntityRangeVariableReferenceNode(DogsEntityRangeVariable.Name, DogsEntityRangeVariable);

        public static readonly EntityRangeVariableReferenceNode FakePersonNode = 
            new EntityRangeVariableReferenceNode(PeopleEntityRangeVariable.Name, PeopleEntityRangeVariable);

        public static readonly EntityRangeVariableReferenceNode FakePaintingNode =
            new EntityRangeVariableReferenceNode(PaintingEntityRangeVariable.Name, PaintingEntityRangeVariable);

        public static readonly SingleEntityNode FakeLionNode = 
            new EntityRangeVariableReferenceNode(LionsEntityRangeVariable.Name, LionsEntityRangeVariable);        

        public static readonly SingleValueNode FakeSinglePrimitive =
            new ConstantNode("A_STRING");

        public static readonly SingleValueNode FakeSingleIntPrimitive =
            new ConstantNode(100);

        public static readonly SingleValueNode FakeSingleOpenProperty =
            new SingleValueOpenPropertyAccessNode(new ConstantNode(null), "A_OPENPROPERTY");

        public static readonly EntityCollectionNode FakeEntityCollectionNode =
            new EntitySetNode(HardCodedTestModel.GetPeopleSet());

        public static readonly ConstantNode FakeNullLiteralNode =
            new ConstantNode(null);

        public static readonly SingleValuePropertyAccessNode FakeSingleValueProperty =
            new SingleValuePropertyAccessNode(FakePersonNode, HardCodedTestModel.GetPersonAddressProp());

        public static readonly SingleValuePropertyAccessNode FakePersonDogNameNode =
            new SingleValuePropertyAccessNode(
                new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(),FakePersonNode),HardCodedTestModel.GetAddressCityProperty());


        public static readonly CollectionPropertyAccessNode FakeCollectionValueProperty =
            new CollectionPropertyAccessNode(FakePersonNode, HardCodedTestModel.GetPersonPreviousAddressesProp());

        public static SingleEntityNode BindMethodReturningASingleLion(QueryToken token)
        {
            return FakeLionNode;
        }

        public static SingleEntityNode BindMethodReturningASingleDog(QueryToken token)
        {
            return FakeDogNode;
        }

        public static SingleEntityNode BindMethodReturningASinglePerson(QueryToken token)
        {
            return FakePersonNode;
        }

        public static SingleEntityNode BindMethodReturningASinglePainting(QueryToken token)
        {
            return FakePaintingNode;
        }

        public static SingleValueNode BindMethodReturningASinglePrimitive(QueryToken token)
        {
            return FakeSinglePrimitive;
        }

        public static SingleValueNode BindMethodReturningASingleIntPrimitive(QueryToken token)
        {
            return FakeSingleIntPrimitive;
        }

        public static SingleValueNode BindMethodReturningASingleOpenProperty(QueryToken token)
        {
            return FakeSingleOpenProperty;
        }

        public static SingleValueNode BindMethodReturningANullLiteralConstantNode(QueryToken token)
        {
            return FakeNullLiteralNode;
        }

        public static EntityCollectionNode BindMethodThatReturnsEntitySetNode(QueryToken queryToken)
        {
            return FakeEntityCollectionNode;
        }

        public static SingleValueNode KeyValueBindMethod(QueryToken queryToken)
        {
            return KeyBinderConstantToken;
        }

        public static SingleValueNode BindMethodReturnsNull(QueryToken queryToken)
        {
            return null;
        }

        public static SingleValuePropertyAccessNode BindSingleValueProperty(QueryToken queryToken)
        {
            return FakeSingleValueProperty;
        }

        public static SingleValuePropertyAccessNode BindMethodReturnsPersonDogNameNavigation(QueryToken queryToken)
        {
            return FakePersonDogNameNode;
        }

        public static CollectionPropertyAccessNode BindCollectionProperty(QueryToken queryToken)
        {
            return FakeCollectionValueProperty;
        }
    }
}
