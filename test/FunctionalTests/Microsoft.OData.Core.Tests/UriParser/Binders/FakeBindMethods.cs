//---------------------------------------------------------------------
// <copyright file="FakeBindMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Class that contains fake bind methods to help unit semantic tests from growing in scope.
    /// </summary>
    internal class FakeBindMethods
    {
        private static readonly ResourceRangeVariable DogsEntityRangeVariable = new ResourceRangeVariable("A_DOG", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
        private static readonly ResourceRangeVariable PeopleEntityRangeVariable = new ResourceRangeVariable("A_PERSON", HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
        private static readonly ResourceRangeVariable PaintingEntityRangeVariable = new ResourceRangeVariable("A_PAINTING", HardCodedTestModel.GetPaintingTypeReference(), HardCodedTestModel.GetPaintingsSet());
        private static readonly ResourceRangeVariable LionsEntityRangeVariable = new ResourceRangeVariable("A_LION", HardCodedTestModel.GetLionTypeReference(), HardCodedTestModel.GetLionSet());
        private const int KeyBinderConstantValue = 0xBAD;

        public static readonly ConstantNode KeyBinderConstantToken = new ConstantNode(KeyBinderConstantValue);

        public static readonly ResourceRangeVariableReferenceNode FakeDogNode =
            new ResourceRangeVariableReferenceNode(DogsEntityRangeVariable.Name, DogsEntityRangeVariable);

        public static readonly ResourceRangeVariableReferenceNode FakePersonNode =
            new ResourceRangeVariableReferenceNode(PeopleEntityRangeVariable.Name, PeopleEntityRangeVariable);

        public static readonly ResourceRangeVariableReferenceNode FakePaintingNode =
            new ResourceRangeVariableReferenceNode(PaintingEntityRangeVariable.Name, PaintingEntityRangeVariable);

        public static readonly SingleResourceNode FakeLionNode =
            new ResourceRangeVariableReferenceNode(LionsEntityRangeVariable.Name, LionsEntityRangeVariable);

        public static readonly SingleValueNode FakeSinglePrimitive =
            new ConstantNode("A_STRING");

        public static readonly SingleValueNode FakeSingleIntPrimitive =
            new ConstantNode(100);

        public static readonly SingleValueNode FakeSingleFloatPrimitive =
            new ConstantNode(100.50f);

        public static readonly SingleValueNode FakeSingleOpenProperty =
            new SingleValueOpenPropertyAccessNode(new ConstantNode(null), "A_OPENPROPERTY");

        public static readonly CollectionResourceNode FakeEntityCollectionNode =
            new EntitySetNode(HardCodedTestModel.GetPeopleSet());

        public static readonly ConstantNode FakeNullLiteralNode =
            new ConstantNode(null);

        public static readonly SingleComplexNode FakeSingleComplexProperty =
            new SingleComplexNode(FakePersonNode, HardCodedTestModel.GetPersonAddressProp());

        public static readonly SingleValuePropertyAccessNode FakePersonDogColorNode =
            new SingleValuePropertyAccessNode(
                new SingleNavigationNode(FakePersonNode, HardCodedTestModel.GetPersonMyDogNavProp(), new EdmPathExpression("MyDog")), HardCodedTestModel.GetDogColorProp());


        public static readonly CollectionComplexNode FakeCollectionComplexProperty =
            new CollectionComplexNode(FakePersonNode, HardCodedTestModel.GetPersonPreviousAddressesProp());

        public static SingleResourceNode BindMethodReturningASingleLion(QueryToken token)
        {
            return FakeLionNode;
        }

        public static SingleResourceNode BindMethodReturningASingleDog(QueryToken token)
        {
            return FakeDogNode;
        }

        public static SingleResourceNode BindMethodReturningASinglePerson(QueryToken token)
        {
            return FakePersonNode;
        }

        public static SingleResourceNode BindMethodReturningASinglePainting(QueryToken token)
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

        public static SingleValueNode BindMethodReturningASingleFloatPrimitive(QueryToken token)
        {
            return FakeSingleFloatPrimitive;
        }

        public static SingleValueNode BindMethodReturningASingleOpenProperty(QueryToken token)
        {
            return FakeSingleOpenProperty;
        }

        public static SingleValueNode BindMethodReturningANullLiteralConstantNode(QueryToken token)
        {
            return FakeNullLiteralNode;
        }

        public static CollectionResourceNode BindMethodThatReturnsEntitySetNode(QueryToken queryToken)
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

        public static SingleComplexNode BindSingleComplexProperty(QueryToken queryToken)
        {
            return FakeSingleComplexProperty;
        }

        public static SingleValuePropertyAccessNode BindMethodReturnsPersonDogColorNavigation(QueryToken queryToken)
        {
            return FakePersonDogColorNode;
        }

        public static CollectionComplexNode BindCollectionComplex(QueryToken queryToken)
        {
            return FakeCollectionComplexProperty;
        }
    }
}
