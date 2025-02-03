namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _entitySetName_keyPredicateⳆsingletonEntityParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity> Instance { get; } = (_entitySetName_keyPredicateParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity>(_singletonEntityParser.Instance);
        
        public static class _entitySetName_keyPredicateParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate> Instance { get; } = from _entitySetName_1 in __GeneratedOdataV3.Parsers.Rules._entitySetNameParser.Instance
from _keyPredicate_1 in __GeneratedOdataV3.Parsers.Rules._keyPredicateParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate(_entitySetName_1, _keyPredicate_1);
        }
        
        public static class _singletonEntityParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity> Instance { get; } = from _singletonEntity_1 in __GeneratedOdataV3.Parsers.Rules._singletonEntityParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity(_singletonEntity_1);
        }
    }
    
}
